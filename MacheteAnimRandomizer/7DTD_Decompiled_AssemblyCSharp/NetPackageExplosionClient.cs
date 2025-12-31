using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000749 RID: 1865
[Preserve]
public class NetPackageExplosionClient : NetPackage
{
	// Token: 0x06003675 RID: 13941 RVA: 0x001670DC File Offset: 0x001652DC
	public NetPackageExplosionClient Setup(int _clrIdx, Vector3 _center, Quaternion _rotation, int _expType, int _blastPower, float _blastRadius, float _blockDamage, int _entityId, List<BlockChangeInfo> _explosionChanges)
	{
		this.clrIdx = _clrIdx;
		this.center = _center;
		this.rotation = _rotation;
		this.expType = _expType;
		this.blastPower = _blastPower;
		this.blastRadius = (int)_blastRadius;
		this.blockDamage = (int)_blockDamage;
		this.entityId = _entityId;
		this.explosionChanges.Clear();
		this.explosionChanges.AddRange(_explosionChanges);
		return this;
	}

	// Token: 0x06003676 RID: 13942 RVA: 0x00167144 File Offset: 0x00165344
	public override void read(PooledBinaryReader _br)
	{
		this.clrIdx = (int)_br.ReadUInt16();
		this.center = StreamUtils.ReadVector3(_br);
		this.rotation = StreamUtils.ReadQuaterion(_br);
		this.expType = (int)_br.ReadInt16();
		this.blastPower = (int)_br.ReadInt16();
		this.blastRadius = (int)_br.ReadInt16();
		this.blockDamage = (int)_br.ReadInt16();
		this.entityId = _br.ReadInt32();
		int num = (int)_br.ReadUInt16();
		this.explosionChanges.Clear();
		for (int i = 0; i < num; i++)
		{
			BlockChangeInfo blockChangeInfo = new BlockChangeInfo();
			blockChangeInfo.Read(_br);
			this.explosionChanges.Add(blockChangeInfo);
		}
	}

	// Token: 0x06003677 RID: 13943 RVA: 0x001671E8 File Offset: 0x001653E8
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((ushort)this.clrIdx);
		StreamUtils.Write(_bw, this.center);
		StreamUtils.Write(_bw, this.rotation);
		_bw.Write((short)this.expType);
		_bw.Write((ushort)this.blastPower);
		_bw.Write((ushort)this.blastRadius);
		_bw.Write((ushort)this.blockDamage);
		_bw.Write(this.entityId);
		_bw.Write((ushort)this.explosionChanges.Count);
		for (int i = 0; i < this.explosionChanges.Count; i++)
		{
			this.explosionChanges[i].Write(_bw);
		}
	}

	// Token: 0x06003678 RID: 13944 RVA: 0x0016729C File Offset: 0x0016549C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_world.GetGameManager().ExplosionClient(this.clrIdx, this.center, this.rotation, this.expType, this.blastPower, (float)this.blastRadius, (float)this.blockDamage, this.entityId, this.explosionChanges);
	}

	// Token: 0x17000583 RID: 1411
	// (get) Token: 0x06003679 RID: 13945 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x0600367A RID: 13946 RVA: 0x001672F1 File Offset: 0x001654F1
	public override int GetLength()
	{
		return 24 + this.explosionChanges.Count * 30;
	}

	// Token: 0x04002C29 RID: 11305
	[PublicizedFrom(EAccessModifier.Private)]
	public int clrIdx;

	// Token: 0x04002C2A RID: 11306
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 center;

	// Token: 0x04002C2B RID: 11307
	[PublicizedFrom(EAccessModifier.Private)]
	public Quaternion rotation;

	// Token: 0x04002C2C RID: 11308
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BlockChangeInfo> explosionChanges = new List<BlockChangeInfo>();

	// Token: 0x04002C2D RID: 11309
	[PublicizedFrom(EAccessModifier.Private)]
	public int expType;

	// Token: 0x04002C2E RID: 11310
	[PublicizedFrom(EAccessModifier.Private)]
	public int blastPower;

	// Token: 0x04002C2F RID: 11311
	[PublicizedFrom(EAccessModifier.Private)]
	public int blastRadius;

	// Token: 0x04002C30 RID: 11312
	[PublicizedFrom(EAccessModifier.Private)]
	public int blockDamage;

	// Token: 0x04002C31 RID: 11313
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;
}
