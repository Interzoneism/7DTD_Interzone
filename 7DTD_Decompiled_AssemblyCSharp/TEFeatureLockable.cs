using System;
using System.Collections.Generic;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000158 RID: 344
[Preserve]
public class TEFeatureLockable : TEFeatureAbs, ILockable
{
	// Token: 0x06000A19 RID: 2585 RVA: 0x0004258D File Offset: 0x0004078D
	public override void Init(TileEntityComposite _parent, TileEntityFeatureData _featureData)
	{
		base.Init(_parent, _featureData);
		this.lockpickFeature = base.Parent.GetFeature<ILockPickable>();
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x000425A8 File Offset: 0x000407A8
	public override void UpgradeDowngradeFrom(TileEntityComposite _other)
	{
		base.UpgradeDowngradeFrom(_other);
		ILockable feature = _other.GetFeature<ILockable>();
		if (feature != null)
		{
			this.locked = feature.IsLocked();
			this.allowedUserIds.AddRange(feature.GetUsers());
			this.passwordHash = feature.GetPassword();
		}
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x000425EF File Offset: 0x000407EF
	public bool IsLocked()
	{
		return this.locked;
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x000425F7 File Offset: 0x000407F7
	public void SetLocked(bool _isLocked)
	{
		this.locked = _isLocked;
		base.SetModified();
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x00042606 File Offset: 0x00040806
	public PlatformUserIdentifierAbs GetOwner()
	{
		return base.Parent.Owner;
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x00042613 File Offset: 0x00040813
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		base.Parent.SetOwner(_userIdentifier);
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x00042621 File Offset: 0x00040821
	public bool IsUserAllowed(PlatformUserIdentifierAbs _userIdentifier)
	{
		return this.IsOwner(_userIdentifier) || this.allowedUserIds.Contains(_userIdentifier) || (_userIdentifier.Equals(PlatformManager.InternalLocalUserIdentifier) && GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled));
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x00042657 File Offset: 0x00040857
	public List<PlatformUserIdentifierAbs> GetUsers()
	{
		return this.allowedUserIds;
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x0004265F File Offset: 0x0004085F
	public bool LocalPlayerIsOwner()
	{
		return this.IsOwner(PlatformManager.InternalLocalUserIdentifier);
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x0004266C File Offset: 0x0004086C
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return _userIdentifier != null && _userIdentifier.Equals(base.Parent.Owner);
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x00042684 File Offset: 0x00040884
	public bool HasPassword()
	{
		return !string.IsNullOrEmpty(this.passwordHash);
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x00042694 File Offset: 0x00040894
	public bool CheckPassword(string _password, PlatformUserIdentifierAbs _userIdentifier, out bool _changed)
	{
		_changed = false;
		string a = _password.GetStableHashCode().ToString("X8");
		if (this.IsOwner(_userIdentifier))
		{
			if (a != this.passwordHash)
			{
				_changed = true;
				this.passwordHash = a;
				this.allowedUserIds.Clear();
				base.SetModified();
			}
			return true;
		}
		if (a == this.passwordHash)
		{
			this.allowedUserIds.Add(_userIdentifier);
			base.SetModified();
			return true;
		}
		return false;
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x0004270F File Offset: 0x0004090F
	public string GetPassword()
	{
		return this.passwordHash;
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x00042718 File Offset: 0x00040918
	public override string GetActivationText(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing, string _activateHotkeyMarkup, string _focusedTileEntityName)
	{
		base.GetActivationText(_world, _blockPos, _blockValue, _entityFocusing, _activateHotkeyMarkup, _focusedTileEntityName);
		if (!this.IsLocked())
		{
			return string.Format(Localization.Get("tooltipUnlocked", false), _activateHotkeyMarkup, _focusedTileEntityName);
		}
		if (this.lockpickFeature == null && !this.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier))
		{
			return string.Format(Localization.Get("tooltipJammed", false), _activateHotkeyMarkup, _focusedTileEntityName);
		}
		return string.Format(Localization.Get("tooltipLocked", false), _activateHotkeyMarkup, _focusedTileEntityName);
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x00042790 File Offset: 0x00040990
	public override void InitBlockActivationCommands(Action<BlockActivationCommand, TileEntityComposite.EBlockCommandOrder, TileEntityFeatureData> _addCallback)
	{
		base.InitBlockActivationCommands(_addCallback);
		_addCallback(new BlockActivationCommand("lock", "lock", false, false, null), TileEntityComposite.EBlockCommandOrder.Normal, base.FeatureData);
		_addCallback(new BlockActivationCommand("unlock", "unlock", false, false, null), TileEntityComposite.EBlockCommandOrder.Normal, base.FeatureData);
		_addCallback(new BlockActivationCommand("keypad", "keypad", false, false, null), TileEntityComposite.EBlockCommandOrder.Normal, base.FeatureData);
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x00042804 File Offset: 0x00040A04
	public override void UpdateBlockActivationCommands(ref BlockActivationCommand _command, ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing)
	{
		base.UpdateBlockActivationCommands(ref _command, _commandName, _world, _blockPos, _blockValue, _entityFocusing);
		PlatformUserIdentifierAbs internalLocalUserIdentifier = PlatformManager.InternalLocalUserIdentifier;
		PersistentPlayerData playerData = _world.GetGameManager().GetPersistentPlayerList().GetPlayerData(base.Parent.Owner);
		HashSet<PlatformUserIdentifierAbs> hashSet = (playerData != null) ? playerData.ACL : null;
		bool flag = !this.LocalPlayerIsOwner() && hashSet != null && hashSet.Contains(internalLocalUserIdentifier);
		if (base.CommandIs(_commandName, "lock"))
		{
			_command.enabled = (!this.IsLocked() && (this.LocalPlayerIsOwner() || flag));
			return;
		}
		if (base.CommandIs(_commandName, "unlock"))
		{
			_command.enabled = (this.IsLocked() && this.LocalPlayerIsOwner());
			return;
		}
		if (base.CommandIs(_commandName, "keypad"))
		{
			_command.enabled = ((!this.IsUserAllowed(internalLocalUserIdentifier) && this.HasPassword() && this.IsLocked()) || this.LocalPlayerIsOwner());
			return;
		}
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x000428EC File Offset: 0x00040AEC
	public override bool OnBlockActivated(ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		base.OnBlockActivated(_commandName, _world, _blockPos, _blockValue, _player);
		if (base.CommandIs(_commandName, "lock"))
		{
			this.SetLocked(true);
			Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, "Misc/locking");
			GameManager.ShowTooltip(_player, "containerLocked", false, false, 0f);
			return true;
		}
		if (base.CommandIs(_commandName, "unlock"))
		{
			this.SetLocked(false);
			Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, "Misc/unlocking");
			GameManager.ShowTooltip(_player, "containerUnlocked", false, false, 0f);
			return true;
		}
		if (base.CommandIs(_commandName, "keypad"))
		{
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_player);
			if (uiforPlayer != null)
			{
				XUiC_KeypadWindow.Open(uiforPlayer, this);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x000429D0 File Offset: 0x00040BD0
	public override void Read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode, int _readVersion)
	{
		base.Read(_br, _eStreamMode, _readVersion);
		this.locked = _br.ReadBoolean();
		int num = _br.ReadInt32();
		this.allowedUserIds.Clear();
		for (int i = 0; i < num; i++)
		{
			this.allowedUserIds.Add(PlatformUserIdentifierAbs.FromStream(_br, false, false));
		}
		this.passwordHash = _br.ReadString();
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x00042A30 File Offset: 0x00040C30
	public override void Write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.Write(_bw, _eStreamMode);
		_bw.Write(this.locked);
		_bw.Write(this.allowedUserIds.Count);
		for (int i = 0; i < this.allowedUserIds.Count; i++)
		{
			this.allowedUserIds[i].ToStream(_bw, false);
		}
		_bw.Write(this.passwordHash);
	}

	// Token: 0x04000919 RID: 2329
	[PublicizedFrom(EAccessModifier.Private)]
	public ILockPickable lockpickFeature;

	// Token: 0x0400091A RID: 2330
	[PublicizedFrom(EAccessModifier.Private)]
	public bool locked;

	// Token: 0x0400091B RID: 2331
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<PlatformUserIdentifierAbs> allowedUserIds = new List<PlatformUserIdentifierAbs>();

	// Token: 0x0400091C RID: 2332
	[PublicizedFrom(EAccessModifier.Private)]
	public string passwordHash = "";
}
