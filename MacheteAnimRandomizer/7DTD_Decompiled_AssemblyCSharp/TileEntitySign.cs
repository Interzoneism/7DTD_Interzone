using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;

// Token: 0x02000B0F RID: 2831
public class TileEntitySign : TileEntity, ILockable, ITileEntitySignable, ITileEntity
{
	// Token: 0x060057C4 RID: 22468 RVA: 0x00238F9C File Offset: 0x0023719C
	public TileEntitySign(Chunk _chunk) : base(_chunk)
	{
		this.allowedUserIds = new List<PlatformUserIdentifierAbs>();
		this.isLocked = true;
		this.ownerID = null;
		this.password = "";
		this.signText = new AuthoredText();
		PlatformUserManager.BlockedStateChanged += this.UserBlockedStateChanged;
	}

	// Token: 0x060057C5 RID: 22469 RVA: 0x00238FFF File Offset: 0x002371FF
	public override void OnUnload(World world)
	{
		base.OnUnload(world);
		PlatformUserManager.BlockedStateChanged -= this.UserBlockedStateChanged;
	}

	// Token: 0x060057C6 RID: 22470 RVA: 0x00239019 File Offset: 0x00237219
	public override void OnDestroy()
	{
		base.OnDestroy();
		PlatformUserManager.BlockedStateChanged -= this.UserBlockedStateChanged;
	}

	// Token: 0x060057C7 RID: 22471 RVA: 0x00239032 File Offset: 0x00237232
	public bool IsLocked()
	{
		return this.isLocked;
	}

	// Token: 0x060057C8 RID: 22472 RVA: 0x0023903A File Offset: 0x0023723A
	public void SetLocked(bool _isLocked)
	{
		this.isLocked = _isLocked;
		this.setModified();
	}

	// Token: 0x060057C9 RID: 22473 RVA: 0x00239049 File Offset: 0x00237249
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.ownerID = _userIdentifier;
		this.setModified();
	}

	// Token: 0x060057CA RID: 22474 RVA: 0x00239058 File Offset: 0x00237258
	public bool IsUserAllowed(PlatformUserIdentifierAbs _userIdentifier)
	{
		return (_userIdentifier != null && _userIdentifier.Equals(this.ownerID)) || this.allowedUserIds.Contains(_userIdentifier);
	}

	// Token: 0x060057CB RID: 22475 RVA: 0x0023907C File Offset: 0x0023727C
	public bool LocalPlayerIsOwner()
	{
		return this.IsOwner(PlatformManager.InternalLocalUserIdentifier);
	}

	// Token: 0x060057CC RID: 22476 RVA: 0x00239089 File Offset: 0x00237289
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return _userIdentifier != null && _userIdentifier.Equals(this.ownerID);
	}

	// Token: 0x060057CD RID: 22477 RVA: 0x0023909C File Offset: 0x0023729C
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.ownerID;
	}

	// Token: 0x060057CE RID: 22478 RVA: 0x002390A4 File Offset: 0x002372A4
	public bool HasPassword()
	{
		return !string.IsNullOrEmpty(this.password);
	}

	// Token: 0x060057CF RID: 22479 RVA: 0x002390B4 File Offset: 0x002372B4
	public string GetPassword()
	{
		return this.password;
	}

	// Token: 0x060057D0 RID: 22480 RVA: 0x002390BC File Offset: 0x002372BC
	public List<PlatformUserIdentifierAbs> GetUsers()
	{
		return this.allowedUserIds;
	}

	// Token: 0x060057D1 RID: 22481 RVA: 0x002390C4 File Offset: 0x002372C4
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

	// Token: 0x060057D2 RID: 22482 RVA: 0x00239144 File Offset: 0x00237344
	public void SetBlockEntityData(BlockEntityData _blockEntityData)
	{
		if (_blockEntityData != null && _blockEntityData.bHasTransform && !GameManager.IsDedicatedServer)
		{
			this.textMesh = _blockEntityData.transform.GetComponentInChildren<TextMesh>();
			this.smartTextMesh = this.textMesh.transform.gameObject.AddComponent<SmartTextMesh>();
			float num = (float)_blockEntityData.blockValue.Block.multiBlockPos.dim.x;
			this.smartTextMesh.MaxWidth = 0.48f * num;
			this.smartTextMesh.MaxLines = this.lineCount;
			this.smartTextMesh.ConvertNewLines = true;
			AuthoredText authoredText = this.signText;
			this.RefreshTextMesh((authoredText != null) ? authoredText.Text : null);
		}
	}

	// Token: 0x060057D3 RID: 22483 RVA: 0x002391FB File Offset: 0x002373FB
	public virtual void SetText(AuthoredText _authoredText, bool _syncData = true)
	{
		this.SetText((_authoredText != null) ? _authoredText.Text : null, _syncData, (_authoredText != null) ? _authoredText.Author : null);
	}

	// Token: 0x060057D4 RID: 22484 RVA: 0x0023921C File Offset: 0x0023741C
	public void SetText(string _text, bool _syncData = true, PlatformUserIdentifierAbs _signingPlayer = null)
	{
		if (!GameManager.Instance)
		{
			return;
		}
		if (_signingPlayer == null)
		{
			_signingPlayer = PlatformManager.MultiPlatform.User.PlatformUserId;
		}
		PersistentPlayerList persistentPlayers = GameManager.Instance.persistentPlayers;
		if (((persistentPlayers != null) ? persistentPlayers.GetPlayerData(_signingPlayer) : null) == null)
		{
			_signingPlayer = null;
		}
		if (_text == this.signText.Text)
		{
			return;
		}
		this.signText.Update(_text, _signingPlayer);
		GeneratedTextManager.GetDisplayText(this.signText, new Action<string>(this.RefreshTextMesh), true, true, GeneratedTextManager.TextFilteringMode.FilterWithSafeString, GeneratedTextManager.BbCodeSupportMode.NotSupported);
		if (_syncData)
		{
			this.setModified();
		}
	}

	// Token: 0x060057D5 RID: 22485 RVA: 0x002392AB File Offset: 0x002374AB
	public AuthoredText GetAuthoredText()
	{
		return this.signText;
	}

	// Token: 0x060057D6 RID: 22486 RVA: 0x002392B3 File Offset: 0x002374B3
	public bool CanRenderString(string _text)
	{
		return this.smartTextMesh.CanRenderString(_text);
	}

	// Token: 0x060057D7 RID: 22487 RVA: 0x002392C4 File Offset: 0x002374C4
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		int num = _br.ReadInt32();
		this.isLocked = _br.ReadBoolean();
		this.ownerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
		if (num > 1)
		{
			this.SetText(AuthoredText.FromStream(_br), false);
		}
		this.password = _br.ReadString();
		this.allowedUserIds = new List<PlatformUserIdentifierAbs>();
		int num2 = _br.ReadInt32();
		for (int i = 0; i < num2; i++)
		{
			this.allowedUserIds.Add(PlatformUserIdentifierAbs.FromStream(_br, false, false));
		}
		if (num <= 1)
		{
			this.SetText(_br.ReadString(), false, null);
		}
	}

	// Token: 0x060057D8 RID: 22488 RVA: 0x0023935C File Offset: 0x0023755C
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write(2);
		_bw.Write(this.isLocked);
		this.ownerID.ToStream(_bw, false);
		AuthoredText.ToStream(this.signText, _bw);
		_bw.Write(this.password);
		_bw.Write(this.allowedUserIds.Count);
		for (int i = 0; i < this.allowedUserIds.Count; i++)
		{
			this.allowedUserIds[i].ToStream(_bw, false);
		}
	}

	// Token: 0x060057D9 RID: 22489 RVA: 0x002393E4 File Offset: 0x002375E4
	public override TileEntity Clone()
	{
		return new TileEntitySign(this.chunk)
		{
			localChunkPos = base.localChunkPos,
			isLocked = this.isLocked,
			ownerID = this.ownerID,
			password = this.password,
			allowedUserIds = new List<PlatformUserIdentifierAbs>(this.allowedUserIds),
			signText = AuthoredText.Clone(this.signText)
		};
	}

	// Token: 0x060057DA RID: 22490 RVA: 0x00239450 File Offset: 0x00237650
	public override void CopyFrom(TileEntity _other)
	{
		TileEntitySign tileEntitySign = (TileEntitySign)_other;
		base.localChunkPos = tileEntitySign.localChunkPos;
		this.isLocked = tileEntitySign.isLocked;
		this.ownerID = tileEntitySign.ownerID;
		this.password = tileEntitySign.password;
		this.allowedUserIds = new List<PlatformUserIdentifierAbs>(tileEntitySign.allowedUserIds);
		this.signText = AuthoredText.Clone(tileEntitySign.signText);
	}

	// Token: 0x170008C8 RID: 2248
	// (get) Token: 0x060057DB RID: 22491 RVA: 0x002322E1 File Offset: 0x002304E1
	// (set) Token: 0x060057DC RID: 22492 RVA: 0x00234CF8 File Offset: 0x00232EF8
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

	// Token: 0x060057DD RID: 22493 RVA: 0x000E74AA File Offset: 0x000E56AA
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.Sign;
	}

	// Token: 0x060057DE RID: 22494 RVA: 0x002394B6 File Offset: 0x002376B6
	[PublicizedFrom(EAccessModifier.Private)]
	public void UserBlockedStateChanged(IPlatformUserData userData, EBlockType blockType, EUserBlockState blockState)
	{
		if (!userData.PrimaryId.Equals(this.signText.Author) || blockType != EBlockType.TextChat)
		{
			return;
		}
		GeneratedTextManager.GetDisplayText(this.signText, new Action<string>(this.RefreshTextMesh), true, true, GeneratedTextManager.TextFilteringMode.FilterWithSafeString, GeneratedTextManager.BbCodeSupportMode.NotSupported);
	}

	// Token: 0x060057DF RID: 22495 RVA: 0x002394EF File Offset: 0x002376EF
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshTextMesh(string _text)
	{
		if (this.smartTextMesh != null && !GameManager.IsDedicatedServer)
		{
			this.smartTextMesh.UnwrappedText = _text;
		}
	}

	// Token: 0x0400435D RID: 17245
	[PublicizedFrom(EAccessModifier.Private)]
	public const int ver = 2;

	// Token: 0x0400435E RID: 17246
	public int lineCharWidth = 19;

	// Token: 0x0400435F RID: 17247
	public int lineCount = 3;

	// Token: 0x04004360 RID: 17248
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isLocked;

	// Token: 0x04004361 RID: 17249
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs ownerID;

	// Token: 0x04004362 RID: 17250
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PlatformUserIdentifierAbs> allowedUserIds;

	// Token: 0x04004363 RID: 17251
	[PublicizedFrom(EAccessModifier.Private)]
	public string password;

	// Token: 0x04004364 RID: 17252
	[PublicizedFrom(EAccessModifier.Private)]
	public AuthoredText signText;

	// Token: 0x04004365 RID: 17253
	[PublicizedFrom(EAccessModifier.Private)]
	public TextMesh textMesh;

	// Token: 0x04004366 RID: 17254
	[PublicizedFrom(EAccessModifier.Private)]
	public SmartTextMesh smartTextMesh;
}
