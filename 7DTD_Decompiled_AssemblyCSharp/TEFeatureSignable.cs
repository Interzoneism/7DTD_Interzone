using System;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200015A RID: 346
[Preserve]
public class TEFeatureSignable : TEFeatureAbs, ITileEntitySignable, ITileEntity
{
	// Token: 0x06000A37 RID: 2615 RVA: 0x000431DC File Offset: 0x000413DC
	public TEFeatureSignable()
	{
		PlatformUserManager.BlockedStateChanged += this.UserBlockedStateChanged;
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x00043212 File Offset: 0x00041412
	public override void OnUnload(World _world)
	{
		PlatformUserManager.BlockedStateChanged -= this.UserBlockedStateChanged;
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x00043212 File Offset: 0x00041412
	public override void OnDestroy()
	{
		PlatformUserManager.BlockedStateChanged -= this.UserBlockedStateChanged;
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x00043228 File Offset: 0x00041428
	public override void Init(TileEntityComposite _parent, TileEntityFeatureData _featureData)
	{
		base.Init(_parent, _featureData);
		this.lockFeature = base.Parent.GetFeature<ILockable>();
		DynamicProperties props = _featureData.Props;
		props.ParseInt("LineCount", ref this.lineCount);
		props.ParseFloat("LineWidth", ref this.lineWidth);
		props.ParseInt("FontSize", ref this.fontSize);
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x00043288 File Offset: 0x00041488
	public override void SetBlockEntityData(BlockEntityData _blockEntityData)
	{
		if (_blockEntityData == null || !_blockEntityData.bHasTransform)
		{
			return;
		}
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		float num = 0.8f;
		Block.MultiBlockArray multiBlockPos = base.Parent.TeData.Block.multiBlockPos;
		float num2 = num * (float)((multiBlockPos != null) ? multiBlockPos.dim.x : 1);
		float maxWidthReal = (this.lineWidth > 0f) ? this.lineWidth : num2;
		TextMesh[] componentsInChildren = _blockEntityData.transform.GetComponentsInChildren<TextMesh>();
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			return;
		}
		this.smartTextMesh = new SmartTextMesh[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].fontSize = this.fontSize;
			this.smartTextMesh[i] = componentsInChildren[i].gameObject.AddComponent<SmartTextMesh>();
			this.smartTextMesh[i].MaxWidthReal = maxWidthReal;
			this.smartTextMesh[i].MaxLines = this.lineCount;
			this.smartTextMesh[i].ConvertNewLines = true;
		}
		string text;
		if ((text = this.displayText) == null)
		{
			AuthoredText authoredText = this.signText;
			text = ((authoredText != null) ? authoredText.Text : null);
		}
		this.RefreshTextMesh(text);
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x00043392 File Offset: 0x00041592
	[PublicizedFrom(EAccessModifier.Private)]
	public void UserBlockedStateChanged(IPlatformUserData _userData, EBlockType _blockType, EUserBlockState _blockState)
	{
		if (!_userData.PrimaryId.Equals(this.signText.Author) || _blockType != EBlockType.TextChat)
		{
			return;
		}
		GeneratedTextManager.GetDisplayText(this.signText, new Action<string>(this.RefreshTextMesh), true, true, GeneratedTextManager.TextFilteringMode.FilterWithSafeString, GeneratedTextManager.BbCodeSupportMode.NotSupported);
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x000433CC File Offset: 0x000415CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshTextMesh(string _text)
	{
		this.displayText = _text;
		if (GameManager.IsDedicatedServer || this.smartTextMesh == null || this.displayText == this.smartTextMesh[0].UnwrappedText)
		{
			return;
		}
		for (int i = 0; i < this.smartTextMesh.Length; i++)
		{
			this.smartTextMesh[i].UnwrappedText = this.displayText;
		}
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x00043430 File Offset: 0x00041630
	public override void UpgradeDowngradeFrom(TileEntityComposite _other)
	{
		base.UpgradeDowngradeFrom(_other);
		ITileEntitySignable feature = _other.GetFeature<ITileEntitySignable>();
		if (feature != null)
		{
			this.signText = feature.GetAuthoredText();
			if (this.signText != null)
			{
				GeneratedTextManager.GetDisplayText(this.signText, new Action<string>(this.RefreshTextMesh), true, true, GeneratedTextManager.TextFilteringMode.FilterWithSafeString, GeneratedTextManager.BbCodeSupportMode.NotSupported);
			}
		}
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x0004347D File Offset: 0x0004167D
	public virtual void SetText(AuthoredText _authoredText, bool _syncData = true)
	{
		this.SetText((_authoredText != null) ? _authoredText.Text : null, _syncData, (_authoredText != null) ? _authoredText.Author : null);
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x000434A0 File Offset: 0x000416A0
	public virtual void SetText(string _text, bool _syncData = true, PlatformUserIdentifierAbs _signingPlayer = null)
	{
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
			base.SetModified();
		}
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x00043522 File Offset: 0x00041722
	public virtual AuthoredText GetAuthoredText()
	{
		return this.signText;
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x0004352A File Offset: 0x0004172A
	public virtual bool CanRenderString(string _text)
	{
		return this.smartTextMesh != null && this.smartTextMesh.Length != 0 && this.smartTextMesh[0].CanRenderString(_text);
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x0004354D File Offset: 0x0004174D
	public override string GetActivationText(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing, string _activateHotkeyMarkup, string _focusedTileEntityName)
	{
		base.GetActivationText(_world, _blockPos, _blockValue, _entityFocusing, _activateHotkeyMarkup, _focusedTileEntityName);
		return Localization.Get("useWorkstation", false);
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x0004356C File Offset: 0x0004176C
	public override void InitBlockActivationCommands(Action<BlockActivationCommand, TileEntityComposite.EBlockCommandOrder, TileEntityFeatureData> _addCallback)
	{
		base.InitBlockActivationCommands(_addCallback);
		_addCallback(new BlockActivationCommand("edit", "pen", true, false, null), TileEntityComposite.EBlockCommandOrder.Normal, base.FeatureData);
		_addCallback(new BlockActivationCommand("report", "report", true, false, null), TileEntityComposite.EBlockCommandOrder.Last, base.FeatureData);
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x000435C0 File Offset: 0x000417C0
	public override void UpdateBlockActivationCommands(ref BlockActivationCommand _command, ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing)
	{
		base.UpdateBlockActivationCommands(ref _command, _commandName, _world, _blockPos, _blockValue, _entityFocusing);
		if (base.CommandIs(_commandName, "edit"))
		{
			_command.enabled = (this.lockFeature == null || GameManager.Instance.IsEditMode() || !this.lockFeature.IsLocked() || this.lockFeature.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier));
			return;
		}
		if (base.CommandIs(_commandName, "report"))
		{
			PlatformUserIdentifierAbs internalLocalUserIdentifier = PlatformManager.InternalLocalUserIdentifier;
			bool flag = PlatformManager.MultiPlatform.PlayerReporting != null && !string.IsNullOrEmpty(this.signText.Text) && !internalLocalUserIdentifier.Equals(this.signText.Author);
			PersistentPlayerData playerData = GameManager.Instance.persistentPlayers.GetPlayerData(this.signText.Author);
			bool flag2 = playerData != null && playerData.PlatformData.Blocked[EBlockType.TextChat].IsBlocked();
			_command.enabled = (flag && !flag2);
			return;
		}
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x000436BC File Offset: 0x000418BC
	public override bool OnBlockActivated(ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		base.OnBlockActivated(_commandName, _world, _blockPos, _blockValue, _player);
		if (base.CommandIs(_commandName, "edit"))
		{
			_player.AimingGun = false;
			Vector3i blockPos = base.Parent.ToWorldPos();
			_world.GetGameManager().TELockServer(0, blockPos, base.Parent.EntityId, _player.entityId, "sign");
			return true;
		}
		if (base.CommandIs(_commandName, "report"))
		{
			GeneratedTextManager.GetDisplayText(this.signText, delegate(string _filtered)
			{
				ThreadManager.AddSingleTaskMainThread("OpenReportWindow", delegate(object _)
				{
					PersistentPlayerData playerData = GameManager.Instance.persistentPlayers.GetPlayerData(this.signText.Author);
					XUiC_ReportPlayer.Open((playerData != null) ? playerData.PlayerData : null, EnumReportCategory.VerbalAbuse, string.Format(Localization.Get("xuiReportOffensiveTextMessage", false), _filtered), "");
				}, null);
			}, true, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
			return true;
		}
		return false;
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0004374B File Offset: 0x0004194B
	public override void Read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode, int _readVersion)
	{
		base.Read(_br, _eStreamMode, _readVersion);
		this.SetText(AuthoredText.FromStream(_br), false);
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x00043763 File Offset: 0x00041963
	public override void Write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.Write(_bw, _eStreamMode);
		AuthoredText.ToStream(this.signText, _bw);
	}

	// Token: 0x04000925 RID: 2341
	[PublicizedFrom(EAccessModifier.Private)]
	public ILockable lockFeature;

	// Token: 0x04000926 RID: 2342
	[PublicizedFrom(EAccessModifier.Private)]
	public AuthoredText signText = new AuthoredText();

	// Token: 0x04000927 RID: 2343
	[PublicizedFrom(EAccessModifier.Private)]
	public int fontSize = 132;

	// Token: 0x04000928 RID: 2344
	[PublicizedFrom(EAccessModifier.Private)]
	public int lineCount = 3;

	// Token: 0x04000929 RID: 2345
	[PublicizedFrom(EAccessModifier.Private)]
	public float lineWidth;

	// Token: 0x0400092A RID: 2346
	[PublicizedFrom(EAccessModifier.Private)]
	public string displayText;

	// Token: 0x0400092B RID: 2347
	[PublicizedFrom(EAccessModifier.Private)]
	public SmartTextMesh[] smartTextMesh;
}
