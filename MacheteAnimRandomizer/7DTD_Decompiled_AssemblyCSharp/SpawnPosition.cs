using System;
using UnityEngine;

// Token: 0x02000A46 RID: 2630
public struct SpawnPosition : IEquatable<SpawnPosition>
{
	// Token: 0x0600505F RID: 20575 RVA: 0x001FF224 File Offset: 0x001FD424
	public SpawnPosition(bool _bInvalid)
	{
		this.ClrIdx = 0;
		this.position = Vector3.zero;
		this.heading = 0f;
		this.bInvalid = true;
	}

	// Token: 0x06005060 RID: 20576 RVA: 0x001FF24A File Offset: 0x001FD44A
	public SpawnPosition(Vector3i _blockPos, float _heading)
	{
		this.ClrIdx = 0;
		this.position = _blockPos.ToVector3() + new Vector3(0.5f, 0f, 0.5f);
		this.heading = _heading;
		this.bInvalid = false;
	}

	// Token: 0x06005061 RID: 20577 RVA: 0x001FF287 File Offset: 0x001FD487
	public SpawnPosition(Vector3 _position, float _heading)
	{
		this.ClrIdx = 0;
		this.position = _position;
		this.heading = _heading;
		this.bInvalid = false;
	}

	// Token: 0x06005062 RID: 20578 RVA: 0x001FF2A5 File Offset: 0x001FD4A5
	public Vector3i ToBlockPos()
	{
		return new Vector3i(Utils.Fastfloor(this.position.x), Utils.Fastfloor(this.position.y), Utils.Fastfloor(this.position.z));
	}

	// Token: 0x06005063 RID: 20579 RVA: 0x001FF2DC File Offset: 0x001FD4DC
	public void Read(IBinaryReaderOrWriter _readerOrWriter, uint _version)
	{
		if (_version > 1U)
		{
			this.ClrIdx = (int)_readerOrWriter.ReadWrite(0);
		}
		this.position = _readerOrWriter.ReadWrite(Vector3.zero);
		this.heading = _readerOrWriter.ReadWrite(0f);
	}

	// Token: 0x06005064 RID: 20580 RVA: 0x001FF311 File Offset: 0x001FD511
	public void Read(PooledBinaryReader _br, uint _version)
	{
		if (_version > 1U)
		{
			this.ClrIdx = (int)_br.ReadUInt16();
		}
		this.position = new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
		this.heading = _br.ReadSingle();
	}

	// Token: 0x06005065 RID: 20581 RVA: 0x001FF34C File Offset: 0x001FD54C
	public void Write(PooledBinaryWriter _bw)
	{
		_bw.Write((ushort)this.ClrIdx);
		_bw.Write(this.position.x);
		_bw.Write(this.position.y);
		_bw.Write(this.position.z);
		_bw.Write(this.heading);
	}

	// Token: 0x06005066 RID: 20582 RVA: 0x001FF3A5 File Offset: 0x001FD5A5
	public bool IsUndef()
	{
		return this.Equals(SpawnPosition.Undef);
	}

	// Token: 0x06005067 RID: 20583 RVA: 0x001FF3B2 File Offset: 0x001FD5B2
	public bool Equals(SpawnPosition _other)
	{
		return this.position.Equals(_other.position) && this.heading == _other.heading && this.bInvalid == _other.bInvalid;
	}

	// Token: 0x06005068 RID: 20584 RVA: 0x001FF3E5 File Offset: 0x001FD5E5
	public override bool Equals(object obj)
	{
		return obj != null && obj is SpawnPosition && this.Equals((SpawnPosition)obj);
	}

	// Token: 0x06005069 RID: 20585 RVA: 0x001FF404 File Offset: 0x001FD604
	public override int GetHashCode()
	{
		return ((this.position.GetHashCode() * 397 ^ this.heading.GetHashCode()) * 397 ^ this.bInvalid.GetHashCode()) * 397 ^ this.ClrIdx.GetHashCode();
	}

	// Token: 0x0600506A RID: 20586 RVA: 0x001FF458 File Offset: 0x001FD658
	public override string ToString()
	{
		return string.Format("SpawnPoint {0}/{1}", this.position.ToCultureInvariantString(), this.heading.ToCultureInvariantString("0.0"));
	}

	// Token: 0x04003D95 RID: 15765
	public static SpawnPosition Undef = new SpawnPosition(true);

	// Token: 0x04003D96 RID: 15766
	public int ClrIdx;

	// Token: 0x04003D97 RID: 15767
	public Vector3 position;

	// Token: 0x04003D98 RID: 15768
	public float heading;

	// Token: 0x04003D99 RID: 15769
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bInvalid;
}
