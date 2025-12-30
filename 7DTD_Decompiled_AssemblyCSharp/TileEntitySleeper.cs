using System;

// Token: 0x02000B10 RID: 2832
public class TileEntitySleeper : TileEntity
{
	// Token: 0x060057E0 RID: 22496 RVA: 0x00239512 File Offset: 0x00237712
	public TileEntitySleeper(Chunk _chunk) : base(_chunk)
	{
		this.priorityMultiplier = 1f;
		this.sightAngle = -1;
		this.sightRange = -1;
		this.hearingPercent = 1f;
	}

	// Token: 0x060057E1 RID: 22497 RVA: 0x00239540 File Offset: 0x00237740
	public override TileEntity Clone()
	{
		return new TileEntitySleeper(this.chunk)
		{
			localChunkPos = base.localChunkPos,
			priorityMultiplier = this.priorityMultiplier,
			sightAngle = this.sightAngle,
			sightRange = this.sightRange,
			hearingPercent = this.hearingPercent
		};
	}

	// Token: 0x060057E2 RID: 22498 RVA: 0x00239594 File Offset: 0x00237794
	public override void CopyFrom(TileEntity _other)
	{
		TileEntitySleeper tileEntitySleeper = (TileEntitySleeper)_other;
		this.priorityMultiplier = tileEntitySleeper.priorityMultiplier;
		this.sightAngle = tileEntitySleeper.sightAngle;
		this.sightRange = tileEntitySleeper.sightRange;
		this.hearingPercent = tileEntitySleeper.hearingPercent;
	}

	// Token: 0x060057E3 RID: 22499 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.Sleeper;
	}

	// Token: 0x060057E4 RID: 22500 RVA: 0x002395D8 File Offset: 0x002377D8
	public void SetPriorityMultiplier(float _priorityMultiplier)
	{
		this.priorityMultiplier = _priorityMultiplier;
		this.setModified();
	}

	// Token: 0x060057E5 RID: 22501 RVA: 0x002395E7 File Offset: 0x002377E7
	public float GetPriorityMultiplier()
	{
		return this.priorityMultiplier;
	}

	// Token: 0x060057E6 RID: 22502 RVA: 0x002395EF File Offset: 0x002377EF
	public void SetSightAngle(int _sightAngle)
	{
		this.sightAngle = _sightAngle;
		this.setModified();
	}

	// Token: 0x060057E7 RID: 22503 RVA: 0x002395FE File Offset: 0x002377FE
	public int GetSightAngle()
	{
		return this.sightAngle;
	}

	// Token: 0x060057E8 RID: 22504 RVA: 0x00239606 File Offset: 0x00237806
	public void SetSightRange(int _sightRange)
	{
		this.sightRange = _sightRange;
		this.setModified();
	}

	// Token: 0x060057E9 RID: 22505 RVA: 0x00239615 File Offset: 0x00237815
	public int GetSightRange()
	{
		return this.sightRange;
	}

	// Token: 0x060057EA RID: 22506 RVA: 0x0023961D File Offset: 0x0023781D
	public void SetHearingPercent(float _hearingPercent)
	{
		this.hearingPercent = _hearingPercent;
		this.setModified();
	}

	// Token: 0x060057EB RID: 22507 RVA: 0x0023962C File Offset: 0x0023782C
	public float GetHearingPercent()
	{
		return this.hearingPercent;
	}

	// Token: 0x060057EC RID: 22508 RVA: 0x00239634 File Offset: 0x00237834
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		this.priorityMultiplier = _br.ReadSingle();
		this.sightRange = (int)_br.ReadInt16();
		this.hearingPercent = _br.ReadSingle();
		this.sightAngle = (int)_br.ReadInt16();
	}

	// Token: 0x060057ED RID: 22509 RVA: 0x0023966E File Offset: 0x0023786E
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write(this.priorityMultiplier);
		_bw.Write((short)this.sightRange);
		_bw.Write(this.hearingPercent);
		_bw.Write((short)this.sightAngle);
	}

	// Token: 0x04004367 RID: 17255
	[PublicizedFrom(EAccessModifier.Private)]
	public float priorityMultiplier;

	// Token: 0x04004368 RID: 17256
	[PublicizedFrom(EAccessModifier.Private)]
	public int sightAngle;

	// Token: 0x04004369 RID: 17257
	[PublicizedFrom(EAccessModifier.Private)]
	public int sightRange;

	// Token: 0x0400436A RID: 17258
	[PublicizedFrom(EAccessModifier.Private)]
	public float hearingPercent;
}
