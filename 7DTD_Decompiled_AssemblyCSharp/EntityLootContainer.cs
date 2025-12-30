using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200044B RID: 1099
[Preserve]
public class EntityLootContainer : EntityItem
{
	// Token: 0x170003BB RID: 955
	// (get) Token: 0x060022A3 RID: 8867 RVA: 0x000DAEA8 File Offset: 0x000D90A8
	public override string LocalizedEntityName
	{
		get
		{
			if (!string.IsNullOrEmpty(this.OverrideName))
			{
				return Localization.Get(this.OverrideName, false);
			}
			return base.LocalizedEntityName;
		}
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x000DAECC File Offset: 0x000D90CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		foreach (Collider collider in base.transform.GetComponentsInChildren<Collider>())
		{
			collider.gameObject.tag = "E_BP_Body";
			collider.enabled = true;
			collider.gameObject.layer = 13;
			collider.gameObject.AddMissingComponent<RootTransformRefEntity>().RootTransform = base.transform;
		}
		this.SetDead();
		if (this.lootContainer != null)
		{
			this.lootContainer.entityId = this.entityId;
		}
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x000DAF54 File Offset: 0x000D9154
	public void SetContent(ItemStack[] _inventory)
	{
		this.isInventory = _inventory;
		this.lootContainer = null;
		this.forceInventoryCreate = true;
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x000DAF6C File Offset: 0x000D916C
	public override void CopyPropertiesFromEntityClass()
	{
		base.CopyPropertiesFromEntityClass();
		EntityClass entityClass = EntityClass.list[this.entityClass];
		if (entityClass.Properties.Values.ContainsKey(EntityClass.PropTimeStayAfterDeath))
		{
			this.timeStayAfterDeath = (int)(StringParsers.ParseFloat(entityClass.Properties.Values[EntityClass.PropTimeStayAfterDeath], 0, -1, NumberStyles.Any) * 20f);
			return;
		}
		this.timeStayAfterDeath = 100;
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x000DAFE0 File Offset: 0x000D91E0
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		if (this.lootContainer != null && this.deathUpdateTime > 0)
		{
			bool flag = GameManager.Instance.GetEntityIDForLockedTileEntity(this.lootContainer) != -1;
			if (!this.bRemoved && !this.lootContainer.IsUserAccessing() && !flag && ((this.lootContainer.bTouched && this.lootContainer.IsEmpty()) || this.deathUpdateTime >= this.timeStayAfterDeath - 1))
			{
				this.removeBackpack();
			}
		}
		this.deathUpdateTime++;
		if (!this.world.IsRemote() && (this.forceInventoryCreate || this.lootContainer == null))
		{
			this.lootContainer = new TileEntityLootContainer(null);
			this.lootContainer.bTouched = false;
			this.lootContainer.entityId = this.entityId;
			this.lootContainer.lootListName = this.GetLootList();
			this.lootContainer.SetContainerSize(LootContainer.GetLootContainer(this.lootContainer.lootListName, true).size, true);
			if (this.isInventory != null)
			{
				this.lootContainer.bTouched = true;
				for (int i = 0; i < this.isInventory.Length; i++)
				{
					if (!this.isInventory[i].IsEmpty())
					{
						this.lootContainer.AddItem(this.isInventory[i]);
					}
				}
			}
			this.lootContainer.SetModified();
			this.forceInventoryCreate = false;
		}
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x000DB14C File Offset: 0x000D934C
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeBackpack()
	{
		this.deathUpdateTime = this.timeStayAfterDeath;
		this.bRemoved = true;
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x000DB161 File Offset: 0x000D9361
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale = 1f)
	{
		if (_strength >= 99999)
		{
			this.removeBackpack();
		}
		return base.DamageEntity(_damageSource, _strength, _criticalHit, impulseScale);
	}

	// Token: 0x060022AA RID: 8874 RVA: 0x000DB17C File Offset: 0x000D937C
	public override bool IsMarkedForUnload()
	{
		return base.IsMarkedForUnload() && this.bRemoved;
	}

	// Token: 0x060022AB RID: 8875 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createMesh()
	{
	}

	// Token: 0x060022AC RID: 8876 RVA: 0x000DB18E File Offset: 0x000D938E
	public override string GetLootList()
	{
		if (!(this.OverrideLootList != ""))
		{
			return this.lootListOnDeath;
		}
		return this.OverrideLootList;
	}

	// Token: 0x060022AD RID: 8877 RVA: 0x000DB1B0 File Offset: 0x000D93B0
	public override void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		base.Write(_bw, _bNetworkWrite);
		_bw.Write((ushort)((this.isInventory != null) ? this.isInventory.Length : 0));
		int num = 0;
		while (this.isInventory != null && num < this.isInventory.Length)
		{
			this.isInventory[num].Write(_bw);
			num++;
		}
		_bw.Write((ushort)((this.isBag != null) ? this.isBag.Length : 0));
		int num2 = 0;
		while (this.isBag != null && num2 < this.isBag.Length)
		{
			this.isBag[num2].Write(_bw);
			num2++;
		}
		_bw.Write(this.OverrideLootList);
		_bw.Write(this.OverrideName);
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x000DB264 File Offset: 0x000D9464
	public override void Read(byte _version, BinaryReader _br)
	{
		base.Read(_version, _br);
		int num = (int)_br.ReadUInt16();
		this.isInventory = new ItemStack[num];
		for (int i = 0; i < num; i++)
		{
			this.isInventory[i] = new ItemStack();
			this.isInventory[i].Read(_br);
		}
		num = (int)_br.ReadUInt16();
		this.isBag = new ItemStack[num];
		for (int j = 0; j < num; j++)
		{
			this.isBag[j] = new ItemStack();
			this.isBag[j].Read(_br);
		}
		if (_version >= 30)
		{
			this.OverrideLootList = _br.ReadString();
			this.OverrideName = _br.ReadString();
		}
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x000DB30C File Offset: 0x000D950C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleNavObject()
	{
		if (EntityClass.list[this.entityClass].NavObject != "")
		{
			this.NavObject = NavObjectManager.Instance.RegisterNavObject(EntityClass.list[this.entityClass].NavObject, this, "", false);
		}
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x000DB366 File Offset: 0x000D9566
	public override string ToString()
	{
		return string.Format("[type={0}, name={1}]", base.GetType().Name, (this.itemClass != null) ? this.itemClass.GetItemName() : "?");
	}

	// Token: 0x040019DD RID: 6621
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemStack[] isInventory;

	// Token: 0x040019DE RID: 6622
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemStack[] isBag;

	// Token: 0x040019DF RID: 6623
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int deathUpdateTime;

	// Token: 0x040019E0 RID: 6624
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int timeStayAfterDeath;

	// Token: 0x040019E1 RID: 6625
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bRemoved;

	// Token: 0x040019E2 RID: 6626
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool forceInventoryCreate;

	// Token: 0x040019E3 RID: 6627
	public string OverrideLootList = "";

	// Token: 0x040019E4 RID: 6628
	public string OverrideName = "";
}
