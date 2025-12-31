using System;
using System.Collections.Generic;
using Platform;

// Token: 0x02000B0D RID: 2829
public class TileEntitySecureLootContainer : TileEntityLootContainer, ILockable, ILockPickable
{
	// Token: 0x170008C5 RID: 2245
	// (get) Token: 0x060057A1 RID: 22433 RVA: 0x00238898 File Offset: 0x00236A98
	public override float LootStageMod
	{
		get
		{
			return ((BlockSecureLoot)base.blockValue.Block).LootStageMod;
		}
	}

	// Token: 0x170008C6 RID: 2246
	// (get) Token: 0x060057A2 RID: 22434 RVA: 0x002388C0 File Offset: 0x00236AC0
	public override float LootStageBonus
	{
		get
		{
			return ((BlockSecureLoot)base.blockValue.Block).LootStageBonus;
		}
	}

	// Token: 0x060057A3 RID: 22435 RVA: 0x002388E5 File Offset: 0x00236AE5
	public TileEntitySecureLootContainer(Chunk _chunk) : base(_chunk)
	{
		this.allowedUserIds = new List<PlatformUserIdentifierAbs>();
		this.isLocked = true;
		this.ownerID = null;
		this.password = "";
		this.bPlayerPlaced = false;
	}

	// Token: 0x060057A4 RID: 22436 RVA: 0x00238924 File Offset: 0x00236B24
	public bool IsLocked()
	{
		return this.isLocked;
	}

	// Token: 0x060057A5 RID: 22437 RVA: 0x0023892C File Offset: 0x00236B2C
	public void SetLocked(bool _isLocked)
	{
		this.isLocked = _isLocked;
		this.setModified();
	}

	// Token: 0x060057A6 RID: 22438 RVA: 0x0023893B File Offset: 0x00236B3B
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.ownerID = _userIdentifier;
		this.setModified();
	}

	// Token: 0x060057A7 RID: 22439 RVA: 0x0023894A File Offset: 0x00236B4A
	public bool IsUserAllowed(PlatformUserIdentifierAbs _userIdentifier)
	{
		return (_userIdentifier != null && _userIdentifier.Equals(this.ownerID)) || this.allowedUserIds.Contains(_userIdentifier);
	}

	// Token: 0x060057A8 RID: 22440 RVA: 0x0023896E File Offset: 0x00236B6E
	public bool LocalPlayerIsOwner()
	{
		return this.IsOwner(PlatformManager.InternalLocalUserIdentifier);
	}

	// Token: 0x060057A9 RID: 22441 RVA: 0x0023897B File Offset: 0x00236B7B
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return _userIdentifier != null && _userIdentifier.Equals(this.ownerID);
	}

	// Token: 0x060057AA RID: 22442 RVA: 0x0023898E File Offset: 0x00236B8E
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.ownerID;
	}

	// Token: 0x060057AB RID: 22443 RVA: 0x00238996 File Offset: 0x00236B96
	public bool HasPassword()
	{
		return !string.IsNullOrEmpty(this.password);
	}

	// Token: 0x060057AC RID: 22444 RVA: 0x002389A6 File Offset: 0x00236BA6
	public string GetPassword()
	{
		return this.password;
	}

	// Token: 0x060057AD RID: 22445 RVA: 0x002389B0 File Offset: 0x00236BB0
	public bool CheckPassword(string _password, PlatformUserIdentifierAbs _userIdentifier, out bool changed)
	{
		changed = false;
		if (_userIdentifier != null && _userIdentifier.Equals(this.ownerID))
		{
			if (Utils.HashString(_password) != this.password)
			{
				changed = true;
				this.password = Utils.HashString(_password);
				this.allowedUserIds.Clear();
				this.setModified();
			}
			return true;
		}
		if (Utils.HashString(_password) == this.password)
		{
			this.allowedUserIds.Add(_userIdentifier);
			this.setModified();
			return true;
		}
		return false;
	}

	// Token: 0x060057AE RID: 22446 RVA: 0x000768A9 File Offset: 0x00074AA9
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.SecureLoot;
	}

	// Token: 0x060057AF RID: 22447 RVA: 0x00238A30 File Offset: 0x00236C30
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		_br.ReadInt32();
		this.bPlayerPlaced = _br.ReadBoolean();
		this.isLocked = _br.ReadBoolean();
		this.ownerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
		this.password = _br.ReadString();
		this.allowedUserIds = new List<PlatformUserIdentifierAbs>();
		int num = _br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			this.allowedUserIds.Add(PlatformUserIdentifierAbs.FromStream(_br, false, false));
		}
	}

	// Token: 0x060057B0 RID: 22448 RVA: 0x00238AB0 File Offset: 0x00236CB0
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write(1);
		_bw.Write(this.bPlayerPlaced);
		_bw.Write(this.isLocked);
		this.ownerID.ToStream(_bw, false);
		_bw.Write(this.password);
		_bw.Write(this.allowedUserIds.Count);
		for (int i = 0; i < this.allowedUserIds.Count; i++)
		{
			this.allowedUserIds[i].ToStream(_bw, false);
		}
	}

	// Token: 0x170008C7 RID: 2247
	// (get) Token: 0x060057B1 RID: 22449 RVA: 0x002322E1 File Offset: 0x002304E1
	// (set) Token: 0x060057B2 RID: 22450 RVA: 0x00234CF8 File Offset: 0x00232EF8
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

	// Token: 0x060057B3 RID: 22451 RVA: 0x00238B38 File Offset: 0x00236D38
	public override void UpgradeDowngradeFrom(TileEntity _other)
	{
		base.UpgradeDowngradeFrom(_other);
		if (_other is ILockable)
		{
			ILockable lockable = _other as ILockable;
			this.EntityId = lockable.EntityId;
			this.SetLocked(lockable.IsLocked());
			this.SetOwner(lockable.GetOwner());
			this.allowedUserIds = new List<PlatformUserIdentifierAbs>(lockable.GetUsers());
			this.password = lockable.GetPassword();
			this.setModified();
		}
	}

	// Token: 0x060057B4 RID: 22452 RVA: 0x00238BA2 File Offset: 0x00236DA2
	public List<PlatformUserIdentifierAbs> GetUsers()
	{
		return this.allowedUserIds;
	}

	// Token: 0x060057B5 RID: 22453 RVA: 0x00238BAC File Offset: 0x00236DAC
	public void ShowLockpickUi(EntityPlayerLocal _player)
	{
		if (_player != null)
		{
			BlockSecureLoot blockSecureLoot = base.blockValue.Block as BlockSecureLoot;
			if (blockSecureLoot != null)
			{
				blockSecureLoot.ShowLockpickUi(this, _player);
			}
		}
	}

	// Token: 0x04004352 RID: 17234
	[PublicizedFrom(EAccessModifier.Private)]
	public const int ver = 1;

	// Token: 0x04004353 RID: 17235
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isLocked;

	// Token: 0x04004354 RID: 17236
	[PublicizedFrom(EAccessModifier.Protected)]
	public PlatformUserIdentifierAbs ownerID;

	// Token: 0x04004355 RID: 17237
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<PlatformUserIdentifierAbs> allowedUserIds;

	// Token: 0x04004356 RID: 17238
	public float PickTimeLeft = -1f;

	// Token: 0x04004357 RID: 17239
	[PublicizedFrom(EAccessModifier.Protected)]
	public string password;

	// Token: 0x04004358 RID: 17240
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bPlayerPlaced;
}
