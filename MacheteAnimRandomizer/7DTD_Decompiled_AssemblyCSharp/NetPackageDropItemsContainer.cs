using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200071B RID: 1819
[Preserve]
public class NetPackageDropItemsContainer : NetPackage
{
	// Token: 0x06003576 RID: 13686 RVA: 0x00163E7A File Offset: 0x0016207A
	public NetPackageDropItemsContainer Setup(int _droppedByID, string _containerEntity, Vector3 _worldPos, ItemStack[] _items)
	{
		this.droppedByID = _droppedByID;
		this.worldPos = _worldPos;
		this.items = _items;
		this.containerEntity = _containerEntity;
		return this;
	}

	// Token: 0x06003577 RID: 13687 RVA: 0x00163E9A File Offset: 0x0016209A
	public override void read(PooledBinaryReader _br)
	{
		this.droppedByID = _br.ReadInt32();
		this.containerEntity = _br.ReadString();
		this.worldPos = StreamUtils.ReadVector3(_br);
		this.items = GameUtils.ReadItemStack(_br);
	}

	// Token: 0x06003578 RID: 13688 RVA: 0x00163ECC File Offset: 0x001620CC
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.droppedByID);
		_bw.Write(this.containerEntity);
		StreamUtils.Write(_bw, this.worldPos);
		_bw.Write((ushort)this.items.Length);
		for (int i = 0; i < this.items.Length; i++)
		{
			this.items[i].Write(_bw);
		}
	}

	// Token: 0x06003579 RID: 13689 RVA: 0x00163F34 File Offset: 0x00162134
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_world.GetGameManager().DropContentInLootContainerServer(this.droppedByID, this.containerEntity, this.worldPos, this.items, false);
	}

	// Token: 0x1700056C RID: 1388
	// (get) Token: 0x0600357A RID: 13690 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x0600357B RID: 13691 RVA: 0x00163F5F File Offset: 0x0016215F
	public override int GetLength()
	{
		return 16;
	}

	// Token: 0x04002B8C RID: 11148
	[PublicizedFrom(EAccessModifier.Private)]
	public int droppedByID;

	// Token: 0x04002B8D RID: 11149
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 worldPos;

	// Token: 0x04002B8E RID: 11150
	[PublicizedFrom(EAccessModifier.Private)]
	public string containerEntity = "";

	// Token: 0x04002B8F RID: 11151
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] items;
}
