using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200074A RID: 1866
[Preserve]
public class NetPackageExplosionInitiate : NetPackage
{
	// Token: 0x0600367C RID: 13948 RVA: 0x00167318 File Offset: 0x00165518
	public NetPackageExplosionInitiate Setup(int _clrIdx, Vector3 _worldPos, Vector3i _blockPos, Quaternion _rotation, ExplosionData _explosionData, int _entityId, float _delay, bool _bRemoveBlockAtExplPosition, ItemValue _itemValueExplosive)
	{
		this.clrIdx = _clrIdx;
		this.worldPos = _worldPos;
		this.blockPos = _blockPos;
		this.rotation = _rotation;
		this.explosionData = _explosionData;
		this.entityId = _entityId;
		this.delay = _delay;
		this.bRemoveBlockAtExplPosition = _bRemoveBlockAtExplPosition;
		this.itemValueExplosive = _itemValueExplosive;
		return this;
	}

	// Token: 0x0600367D RID: 13949 RVA: 0x0016736C File Offset: 0x0016556C
	public override void read(PooledBinaryReader _br)
	{
		this.clrIdx = (int)_br.ReadUInt16();
		this.worldPos = StreamUtils.ReadVector3(_br);
		this.blockPos = StreamUtils.ReadVector3i(_br);
		this.rotation = StreamUtils.ReadQuaterion(_br);
		int count = (int)_br.ReadUInt16();
		this.explosionData = new ExplosionData(_br.ReadBytes(count));
		this.entityId = _br.ReadInt32();
		this.delay = _br.ReadSingle();
		this.bRemoveBlockAtExplPosition = _br.ReadBoolean();
		if (_br.ReadBoolean())
		{
			this.itemValueExplosive = new ItemValue();
			this.itemValueExplosive.Read(_br);
		}
	}

	// Token: 0x0600367E RID: 13950 RVA: 0x00167408 File Offset: 0x00165608
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((ushort)this.clrIdx);
		StreamUtils.Write(_bw, this.worldPos);
		StreamUtils.Write(_bw, this.blockPos);
		StreamUtils.Write(_bw, this.rotation);
		byte[] array = this.explosionData.ToByteArray();
		_bw.Write((ushort)array.Length);
		_bw.Write(array);
		_bw.Write(this.entityId);
		_bw.Write(this.delay);
		_bw.Write(this.bRemoveBlockAtExplPosition);
		_bw.Write(this.itemValueExplosive != null);
		if (this.itemValueExplosive != null)
		{
			this.itemValueExplosive.Write(_bw);
		}
	}

	// Token: 0x0600367F RID: 13951 RVA: 0x001674B4 File Offset: 0x001656B4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_world.GetGameManager().ExplosionServer(this.clrIdx, this.worldPos, this.blockPos, this.rotation, this.explosionData, this.entityId, this.delay, this.bRemoveBlockAtExplPosition, this.itemValueExplosive);
	}

	// Token: 0x17000584 RID: 1412
	// (get) Token: 0x06003680 RID: 13952 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x06003681 RID: 13953 RVA: 0x0015DDFE File Offset: 0x0015BFFE
	public override int GetLength()
	{
		return 70;
	}

	// Token: 0x04002C32 RID: 11314
	[PublicizedFrom(EAccessModifier.Private)]
	public int clrIdx;

	// Token: 0x04002C33 RID: 11315
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 worldPos;

	// Token: 0x04002C34 RID: 11316
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockPos;

	// Token: 0x04002C35 RID: 11317
	[PublicizedFrom(EAccessModifier.Private)]
	public Quaternion rotation;

	// Token: 0x04002C36 RID: 11318
	[PublicizedFrom(EAccessModifier.Private)]
	public ExplosionData explosionData;

	// Token: 0x04002C37 RID: 11319
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002C38 RID: 11320
	[PublicizedFrom(EAccessModifier.Private)]
	public float delay;

	// Token: 0x04002C39 RID: 11321
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bRemoveBlockAtExplPosition;

	// Token: 0x04002C3A RID: 11322
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue itemValueExplosive;
}
