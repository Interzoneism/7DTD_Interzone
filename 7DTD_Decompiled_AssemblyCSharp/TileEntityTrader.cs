using System;

// Token: 0x02000B11 RID: 2833
public class TileEntityTrader : TileEntity
{
	// Token: 0x060057EE RID: 22510 RVA: 0x002396AC File Offset: 0x002378AC
	public TileEntityTrader(Chunk _chunk) : base(_chunk)
	{
		this.TraderData = new TraderData();
	}

	// Token: 0x060057EF RID: 22511 RVA: 0x002396C7 File Offset: 0x002378C7
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityTrader(TileEntityTrader _other) : base(null)
	{
		this.bUserAccessing = _other.bUserAccessing;
		this.TraderData = new TraderData(_other.TraderData);
	}

	// Token: 0x060057F0 RID: 22512 RVA: 0x002396F4 File Offset: 0x002378F4
	public override TileEntity Clone()
	{
		return new TileEntityTrader(this);
	}

	// Token: 0x060057F1 RID: 22513 RVA: 0x002396FC File Offset: 0x002378FC
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		_br.ReadInt32();
		if (this.TraderData == null)
		{
			this.TraderData = new TraderData();
		}
		this.TraderData.Read(0, _br);
		this.syncNeeded = false;
	}

	// Token: 0x060057F2 RID: 22514 RVA: 0x00239734 File Offset: 0x00237934
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write(1);
		this.TraderData.Write(_bw);
	}

	// Token: 0x170008C9 RID: 2249
	// (get) Token: 0x060057F3 RID: 22515 RVA: 0x002322E1 File Offset: 0x002304E1
	// (set) Token: 0x060057F4 RID: 22516 RVA: 0x00234CF8 File Offset: 0x00232EF8
	public new int EntityId
	{
		get
		{
			return this.entityId;
		}
		set
		{
			this.entityId = value;
		}
	}

	// Token: 0x060057F5 RID: 22517 RVA: 0x00076E19 File Offset: 0x00075019
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.Trader;
	}

	// Token: 0x0400436B RID: 17259
	[PublicizedFrom(EAccessModifier.Private)]
	public const int ver = 1;

	// Token: 0x0400436C RID: 17260
	public TraderData TraderData;

	// Token: 0x0400436D RID: 17261
	public bool syncNeeded = true;
}
