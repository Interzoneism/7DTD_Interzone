using System;
using UnityEngine;

// Token: 0x02000AFC RID: 2812
public class TileEntityLandClaim : TileEntity
{
	// Token: 0x17000894 RID: 2196
	// (get) Token: 0x0600569E RID: 22174 RVA: 0x00234BD1 File Offset: 0x00232DD1
	// (set) Token: 0x0600569F RID: 22175 RVA: 0x00234BD9 File Offset: 0x00232DD9
	public bool ShowBounds
	{
		get
		{
			return this.showBounds;
		}
		set
		{
			this.showBounds = value;
			base.SetModified();
		}
	}

	// Token: 0x060056A0 RID: 22176 RVA: 0x00234BE8 File Offset: 0x00232DE8
	public TileEntityLandClaim(Chunk _chunk) : base(_chunk)
	{
		this.ownerID = null;
	}

	// Token: 0x060056A1 RID: 22177 RVA: 0x00234BF8 File Offset: 0x00232DF8
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.ownerID = _userIdentifier;
		this.setModified();
	}

	// Token: 0x060056A2 RID: 22178 RVA: 0x00234C07 File Offset: 0x00232E07
	public bool IsUserAllowed(PlatformUserIdentifierAbs _userIdentifier)
	{
		return _userIdentifier != null && _userIdentifier.Equals(this.ownerID);
	}

	// Token: 0x060056A3 RID: 22179 RVA: 0x00234C1D File Offset: 0x00232E1D
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return _userIdentifier != null && _userIdentifier.Equals(this.ownerID);
	}

	// Token: 0x060056A4 RID: 22180 RVA: 0x00234C30 File Offset: 0x00232E30
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.ownerID;
	}

	// Token: 0x060056A5 RID: 22181 RVA: 0x00234C38 File Offset: 0x00232E38
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		_br.ReadInt32();
		this.ownerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
		this.showBounds = _br.ReadBoolean();
	}

	// Token: 0x060056A6 RID: 22182 RVA: 0x00234C63 File Offset: 0x00232E63
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write(0);
		this.ownerID.ToStream(_bw, false);
		_bw.Write(this.showBounds);
	}

	// Token: 0x060056A7 RID: 22183 RVA: 0x00234C8D File Offset: 0x00232E8D
	public override TileEntity Clone()
	{
		return new TileEntityLandClaim(this.chunk)
		{
			localChunkPos = base.localChunkPos,
			ownerID = this.ownerID,
			showBounds = this.showBounds
		};
	}

	// Token: 0x060056A8 RID: 22184 RVA: 0x00234CC0 File Offset: 0x00232EC0
	public override void CopyFrom(TileEntity _other)
	{
		TileEntityLandClaim tileEntityLandClaim = (TileEntityLandClaim)_other;
		base.localChunkPos = tileEntityLandClaim.localChunkPos;
		this.ownerID = tileEntityLandClaim.ownerID;
		this.showBounds = tileEntityLandClaim.ShowBounds;
	}

	// Token: 0x060056A9 RID: 22185 RVA: 0x002322E1 File Offset: 0x002304E1
	public int GetEntityID()
	{
		return this.entityId;
	}

	// Token: 0x060056AA RID: 22186 RVA: 0x00234CF8 File Offset: 0x00232EF8
	public void SetEntityID(int _entityID)
	{
		this.entityId = _entityID;
	}

	// Token: 0x060056AB RID: 22187 RVA: 0x00234D04 File Offset: 0x00232F04
	public override void UpdateTick(World world)
	{
		base.UpdateTick(world);
		if (this.BoundsHelper != null)
		{
			this.BoundsHelper.localPosition = base.ToWorldPos().ToVector3() - Origin.position + new Vector3(0.5f, 0.5f, 0.5f);
		}
	}

	// Token: 0x060056AC RID: 22188 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.LandClaim;
	}

	// Token: 0x040042FF RID: 17151
	[PublicizedFrom(EAccessModifier.Private)]
	public const int ver = 0;

	// Token: 0x04004300 RID: 17152
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs ownerID;

	// Token: 0x04004301 RID: 17153
	public Transform BoundsHelper;

	// Token: 0x04004302 RID: 17154
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showBounds;
}
