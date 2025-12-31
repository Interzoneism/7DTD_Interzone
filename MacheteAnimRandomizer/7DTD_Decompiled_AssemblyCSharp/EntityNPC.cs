using System;
using System.IO;
using UnityEngine.Scripting;

// Token: 0x02000451 RID: 1105
[Preserve]
public class EntityNPC : EntityAlive
{
	// Token: 0x170003BC RID: 956
	// (get) Token: 0x060022BD RID: 8893 RVA: 0x000DB538 File Offset: 0x000D9738
	public override string LocalizedEntityName
	{
		get
		{
			return Localization.Get(this.EntityName, false);
		}
	}

	// Token: 0x170003BD RID: 957
	// (get) Token: 0x060022BE RID: 8894 RVA: 0x000DB546 File Offset: 0x000D9746
	public NPCInfo NPCInfo
	{
		get
		{
			if (this.npcID != "")
			{
				return NPCInfo.npcInfoList[this.npcID];
			}
			return null;
		}
	}

	// Token: 0x060022BF RID: 8895 RVA: 0x000DB56C File Offset: 0x000D976C
	public override void CopyPropertiesFromEntityClass()
	{
		base.CopyPropertiesFromEntityClass();
		EntityClass entityClass = EntityClass.list[this.entityClass];
		if (entityClass.Properties.Values.ContainsKey(EntityClass.PropNPCID))
		{
			this.npcID = entityClass.Properties.Values[EntityClass.PropNPCID];
		}
	}

	// Token: 0x060022C0 RID: 8896 RVA: 0x000DB5C2 File Offset: 0x000D97C2
	public override void Read(byte _version, BinaryReader _br)
	{
		base.Read(_version, _br);
		this.bag.SetSlots(GameUtils.ReadItemStack(_br));
	}

	// Token: 0x060022C1 RID: 8897 RVA: 0x000DB5DD File Offset: 0x000D97DD
	public override void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		base.Write(_bw, _bNetworkWrite);
		GameUtils.WriteItemStack(_bw, this.bag.GetSlots());
	}

	// Token: 0x060022C2 RID: 8898 RVA: 0x000D61E8 File Offset: 0x000D43E8
	public override bool IsSavedToFile()
	{
		return (base.GetSpawnerSource() != EnumSpawnerSource.Dynamic || this.IsDead()) && base.IsSavedToFile();
	}

	// Token: 0x060022C3 RID: 8899 RVA: 0x000DB5F8 File Offset: 0x000D97F8
	public override float GetSeeDistance()
	{
		return 80f;
	}

	// Token: 0x060022C4 RID: 8900 RVA: 0x000DB600 File Offset: 0x000D9800
	public override void VisiblityCheck(float _distanceSqr, bool _masterIsZooming)
	{
		bool bVisible = _distanceSqr < (float)(_masterIsZooming ? 14400 : 8100);
		this.emodel.SetVisible(bVisible, false);
	}

	// Token: 0x060022C5 RID: 8901 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanBePushed()
	{
		return false;
	}

	// Token: 0x060022C6 RID: 8902 RVA: 0x000DB62E File Offset: 0x000D982E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool canDespawn()
	{
		return this.world.GetPlayers().Count == 0 && base.canDespawn();
	}

	// Token: 0x060022C7 RID: 8903 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isRadiationSensitive()
	{
		return false;
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDetailedHeadBodyColliders()
	{
		return true;
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isGameMessageOnDeath()
	{
		return false;
	}

	// Token: 0x060022CA RID: 8906 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void PlayVoiceSetEntry(string name, EntityPlayer player, bool ignoreTime = true, bool showReactionAnim = true)
	{
	}

	// Token: 0x040019E9 RID: 6633
	public string npcID = "";
}
