using System;
using System.Collections.Generic;
using Platform;

// Token: 0x02000B09 RID: 2825
public class TileEntitySecure : TileEntityLootContainer, ILockable
{
	// Token: 0x06005780 RID: 22400 RVA: 0x002385DC File Offset: 0x002367DC
	public TileEntitySecure(Chunk _chunk) : base(_chunk)
	{
		this.allowedUserIds = new List<PlatformUserIdentifierAbs>();
		this.isLocked = false;
		this.ownerID = null;
		this.password = "";
		this.bPlayerPlaced = false;
	}

	// Token: 0x06005781 RID: 22401 RVA: 0x00238610 File Offset: 0x00236810
	public bool IsLocked()
	{
		return this.isLocked;
	}

	// Token: 0x06005782 RID: 22402 RVA: 0x00238618 File Offset: 0x00236818
	public void SetLocked(bool _isLocked)
	{
		this.isLocked = _isLocked;
		this.setModified();
	}

	// Token: 0x06005783 RID: 22403 RVA: 0x00238627 File Offset: 0x00236827
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.ownerID = _userIdentifier;
		this.setModified();
	}

	// Token: 0x06005784 RID: 22404 RVA: 0x00238636 File Offset: 0x00236836
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.ownerID;
	}

	// Token: 0x06005785 RID: 22405 RVA: 0x0023863E File Offset: 0x0023683E
	public bool IsUserAllowed(PlatformUserIdentifierAbs _userIdentifier)
	{
		return (_userIdentifier != null && _userIdentifier.Equals(this.ownerID)) || this.allowedUserIds.Contains(_userIdentifier);
	}

	// Token: 0x06005786 RID: 22406 RVA: 0x00238662 File Offset: 0x00236862
	public List<PlatformUserIdentifierAbs> GetUsers()
	{
		return this.allowedUserIds;
	}

	// Token: 0x06005787 RID: 22407 RVA: 0x0023866A File Offset: 0x0023686A
	public bool LocalPlayerIsOwner()
	{
		return this.IsOwner(PlatformManager.InternalLocalUserIdentifier);
	}

	// Token: 0x06005788 RID: 22408 RVA: 0x00238677 File Offset: 0x00236877
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return _userIdentifier != null && _userIdentifier.Equals(this.ownerID);
	}

	// Token: 0x06005789 RID: 22409 RVA: 0x0023868A File Offset: 0x0023688A
	public bool HasPassword()
	{
		return !string.IsNullOrEmpty(this.password);
	}

	// Token: 0x0600578A RID: 22410 RVA: 0x0023869A File Offset: 0x0023689A
	public string GetPassword()
	{
		return this.password;
	}

	// Token: 0x0600578B RID: 22411 RVA: 0x002386A4 File Offset: 0x002368A4
	public override void UpgradeDowngradeFrom(TileEntity _other)
	{
		base.UpgradeDowngradeFrom(_other);
		ILockable lockable = _other as ILockable;
		if (lockable != null)
		{
			this.entityId = lockable.EntityId;
			this.isLocked = lockable.IsLocked();
			this.ownerID = lockable.GetOwner();
			this.allowedUserIds = new List<PlatformUserIdentifierAbs>(lockable.GetUsers());
			this.password = lockable.GetPassword();
		}
	}

	// Token: 0x0600578C RID: 22412 RVA: 0x00238704 File Offset: 0x00236904
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

	// Token: 0x0600578D RID: 22413 RVA: 0x00238784 File Offset: 0x00236984
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		if (_br.ReadInt32() > 0)
		{
			this.bPlayerPlaced = _br.ReadBoolean();
			this.isLocked = _br.ReadBoolean();
			this.ownerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
			int num = _br.ReadInt32();
			this.allowedUserIds = new List<PlatformUserIdentifierAbs>();
			for (int i = 0; i < num; i++)
			{
				this.allowedUserIds.Add(PlatformUserIdentifierAbs.FromStream(_br, false, false));
			}
			this.password = _br.ReadString();
		}
	}

	// Token: 0x0600578E RID: 22414 RVA: 0x00238808 File Offset: 0x00236A08
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write(1);
		_bw.Write(this.bPlayerPlaced);
		_bw.Write(this.isLocked);
		this.ownerID.ToStream(_bw, false);
		_bw.Write(this.allowedUserIds.Count);
		for (int i = 0; i < this.allowedUserIds.Count; i++)
		{
			this.allowedUserIds[i].ToStream(_bw, false);
		}
		_bw.Write(this.password);
	}

	// Token: 0x170008C3 RID: 2243
	// (get) Token: 0x0600578F RID: 22415 RVA: 0x002322E1 File Offset: 0x002304E1
	// (set) Token: 0x06005790 RID: 22416 RVA: 0x00234CF8 File Offset: 0x00232EF8
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

	// Token: 0x0400434C RID: 17228
	[PublicizedFrom(EAccessModifier.Private)]
	public const int ver = 1;

	// Token: 0x0400434D RID: 17229
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isLocked;

	// Token: 0x0400434E RID: 17230
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs ownerID;

	// Token: 0x0400434F RID: 17231
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PlatformUserIdentifierAbs> allowedUserIds;

	// Token: 0x04004350 RID: 17232
	[PublicizedFrom(EAccessModifier.Private)]
	public string password;

	// Token: 0x04004351 RID: 17233
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bPlayerPlaced;
}
