using System;

// Token: 0x02000B02 RID: 2818
public class TileEntityPoweredMeleeTrap : TileEntityPoweredBlock
{
	// Token: 0x06005720 RID: 22304 RVA: 0x00236D41 File Offset: 0x00234F41
	public TileEntityPoweredMeleeTrap(Chunk _chunk) : base(_chunk)
	{
	}

	// Token: 0x170008A9 RID: 2217
	// (get) Token: 0x06005721 RID: 22305 RVA: 0x00236D51 File Offset: 0x00234F51
	// (set) Token: 0x06005722 RID: 22306 RVA: 0x00236D68 File Offset: 0x00234F68
	public int OwnerEntityID
	{
		get
		{
			if (this.ownerEntityID == -1)
			{
				this.SetOwnerEntityID();
			}
			return this.ownerEntityID;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.ownerEntityID = value;
		}
	}

	// Token: 0x06005723 RID: 22307 RVA: 0x0005772E File Offset: 0x0005592E
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.PowerMeleeTrap;
	}

	// Token: 0x06005724 RID: 22308 RVA: 0x00236D71 File Offset: 0x00234F71
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return _userIdentifier != null && _userIdentifier.Equals(this.ownerID);
	}

	// Token: 0x06005725 RID: 22309 RVA: 0x00236D84 File Offset: 0x00234F84
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.ownerID;
	}

	// Token: 0x06005726 RID: 22310 RVA: 0x00236D8C File Offset: 0x00234F8C
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.ownerID = _userIdentifier;
		this.SetOwnerEntityID();
		this.setModified();
	}

	// Token: 0x06005727 RID: 22311 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnSetLocalChunkPosition()
	{
	}

	// Token: 0x06005728 RID: 22312 RVA: 0x00236DA4 File Offset: 0x00234FA4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetOwnerEntityID()
	{
		this.ownerEntityID = -1;
		PersistentPlayerList persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
		PersistentPlayerData persistentPlayerData = (persistentPlayerList != null) ? persistentPlayerList.GetPlayerData(this.ownerID) : null;
		if (persistentPlayerData != null)
		{
			this.ownerEntityID = persistentPlayerData.EntityId;
		}
	}

	// Token: 0x06005729 RID: 22313 RVA: 0x00236DE4 File Offset: 0x00234FE4
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		this.ownerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
		this.SetOwnerEntityID();
	}

	// Token: 0x0600572A RID: 22314 RVA: 0x00236E02 File Offset: 0x00235002
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		this.ownerID.ToStream(_bw, false);
	}

	// Token: 0x0400432B RID: 17195
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs ownerID;

	// Token: 0x0400432C RID: 17196
	[PublicizedFrom(EAccessModifier.Private)]
	public int ownerEntityID = -1;
}
