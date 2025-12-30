using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C7C RID: 3196
[Preserve]
public class XUiC_DataManagement : XUiController
{
	// Token: 0x0600628B RID: 25227 RVA: 0x0028039C File Offset: 0x0027E59C
	public override void Init()
	{
		base.Init();
		XUiC_DataManagement.ID = base.WindowGroup.ID;
		this.worldList = (base.GetChildById("worlds") as XUiC_DMWorldList);
		this.worldList.SelectionChanged += this.WorldList_SelectionChanged;
		this.worldList.OnEntryClicked += this.WorldList_OnEntryClicked;
		this.worldList.OnChildElementHovered += this.WorldList_OnChildElementHovered;
		this.savesList = (base.GetChildById("saves") as XUiC_DMSavegamesList);
		this.savesList.SelectionChanged += this.SavesList_SelectionChanged;
		this.savesList.OnEntryClicked += this.SavesList_OnEntryClicked;
		this.savesList.OnChildElementHovered += this.SavesList_OnChildElementHovered;
		this.playersList = (base.GetChildById("players") as XUiC_DMPlayersList);
		this.playersList.SelectionChanged += this.PlayersList_SelectionChanged;
		this.playersList.OnChildElementHovered += this.PlayersList_OnChildElementHovered;
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnBack") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += this.BtnBack_OnPressed;
		}
		this.btnDeleteWorld = (XUiC_SimpleButton)base.GetChildById("btnDeleteWorld");
		this.btnDeleteSave = (XUiC_SimpleButton)base.GetChildById("btnDeleteSave");
		this.btnArchiveSave = (XUiC_SimpleButton)base.GetChildById("btnArchiveSave");
		this.btnDeletePlayer = (XUiC_SimpleButton)base.GetChildById("btnDeletePlayer");
		this.btnDeleteBlockedPlayers = (XUiC_SimpleButton)base.GetChildById("btnDeleteBlockedPlayers");
		this.btnDeleteWorld.OnPressed += this.BtnDeleteWorld_OnPressed;
		this.btnDeleteWorld.Button.Controller.OnHover += this.BtnDeleteWorld_OnHover;
		this.btnDeleteSave.OnPressed += this.BtnDeleteSave_OnPressed;
		this.btnDeleteSave.Button.Controller.OnHover += this.BtnDeleteSave_OnHover;
		this.btnArchiveSave.OnPressed += this.BtnArchiveSave_OnPressed;
		this.btnArchiveSave.Button.Controller.OnHover += this.BtnArchiveSave_OnHover;
		this.btnDeletePlayer.OnPressed += this.BtnDeletePlayer_OnPressed;
		this.btnDeletePlayer.Button.Controller.OnHover += this.BtnDeletePlayer_OnHover;
		this.btnDeleteBlockedPlayers.OnPressed += this.BtnDeleteBlockedPlayers_OnPressed;
		this.btnDeleteBlockedPlayers.Button.Controller.OnHover += this.BtnDeleteBlockedPlayers_OnHover;
		this.confirmationPrompt = (base.GetChildById("confirmation_prompt_controller") as XUiC_ConfirmationPrompt);
		this.dataManagementBar = (base.GetChildById("data_bar_controller") as XUiC_DataManagementBar);
		this.dataManagementBarEnabled = (this.dataManagementBar != null && SaveInfoProvider.DataLimitEnabled);
		this.IsDirty = true;
	}

	// Token: 0x0600628C RID: 25228 RVA: 0x002806B0 File Offset: 0x0027E8B0
	public override void OnOpen()
	{
		base.OnOpen();
		this.windowGroup.openWindowOnEsc = XUiC_EditingTools.ID;
		if (this.dataManagementBarEnabled)
		{
			this.dataManagementBar.SetDisplayMode(XUiC_DataManagementBar.DisplayMode.Selection);
			this.dataManagementBar.SetSelectedByteRegion(XUiC_DataManagementBar.BarRegion.None);
		}
		this.Refresh();
		if (!this.worldList.SelectByKey(SaveInfoProvider.GetWorldEntryKey(GamePrefs.GetString(EnumGamePrefs.GameWorld), "Local")))
		{
			this.worldList.SelectedEntryIndex = -1;
			this.savesList.SelectedEntryIndex = -1;
			this.savesList.SetWorldFilter(string.Empty);
			this.playersList.ClearList(true);
		}
		else if (!this.savesList.SelectByName(GamePrefs.GetString(EnumGamePrefs.GameName)))
		{
			this.savesList.SelectedEntryIndex = -1;
			this.playersList.ClearList(true);
		}
		this.playersList.SelectedEntryIndex = -1;
		if (this.dataManagementBarEnabled)
		{
			this.dataManagementBar.SetSelectionDepth((this.savesList.SelectedEntryIndex == -1) ? XUiC_DataManagementBar.SelectionDepth.Primary : XUiC_DataManagementBar.SelectionDepth.Secondary);
		}
		this.RefreshButtonStates();
		this.UpdateBarSelectionValues();
		this.previousLockView = base.xui.playerUI.CursorController.lockNavigationToView;
		base.xui.playerUI.CursorController.SetNavigationLockView(this.viewComponent, null);
		base.WindowGroup.isEscClosable = false;
		this.IsDirty = true;
	}

	// Token: 0x0600628D RID: 25229 RVA: 0x00280804 File Offset: 0x0027EA04
	public override void OnClose()
	{
		base.OnClose();
		this.worldList.ClearList();
		this.savesList.ClearList();
		this.playersList.ClearList(true);
	}

	// Token: 0x0600628E RID: 25230 RVA: 0x00280830 File Offset: 0x0027EA30
	[PublicizedFrom(EAccessModifier.Private)]
	public void Refresh()
	{
		SaveInfoProvider instance = SaveInfoProvider.Instance;
		this.worldList.RebuildList(instance.WorldEntryInfos, false);
		this.savesList.RebuildList(instance.SaveEntryInfos, false);
		if (this.dataManagementBarEnabled)
		{
			this.dataManagementBar.SetUsedBytes(instance.TotalUsedBytes);
			this.dataManagementBar.SetAllowanceBytes(instance.TotalAllowanceBytes);
		}
	}

	// Token: 0x0600628F RID: 25231 RVA: 0x00280894 File Offset: 0x0027EA94
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.xui.playerUI.playerInput != null && base.xui.playerUI.playerInput.PermanentActions.Cancel.WasPressed)
		{
			if (!this.confirmationPrompt.IsVisible)
			{
				this.CloseDataManagementWindow();
				return;
			}
			this.confirmationPrompt.Cancel();
		}
		if (this.playerEntryResolver != null && this.playerEntryResolver.IsComplete)
		{
			this.playersList.RebuildList(this.playerEntryResolver.pendingPlayerEntries, false);
			this.playerEntryResolver = null;
			this.IsDirty = true;
		}
		if (!this.IsDirty)
		{
			return;
		}
		base.RefreshBindings(true);
		this.IsDirty = false;
	}

	// Token: 0x06006290 RID: 25232 RVA: 0x00280950 File Offset: 0x0027EB50
	[PublicizedFrom(EAccessModifier.Private)]
	public void WorldList_SelectionChanged(XUiC_ListEntry<XUiC_DMWorldList.ListEntry> _previousEntry, XUiC_ListEntry<XUiC_DMWorldList.ListEntry> _newEntry)
	{
		string worldFilter = "";
		if (_newEntry != null)
		{
			XUiC_DMWorldList.ListEntry entry = _newEntry.GetEntry();
			worldFilter = ((entry != null) ? entry.Key : null);
		}
		this.savesList.SetWorldFilter(worldFilter);
		this.savesList.ClearSelection();
		this.playersList.ClearSelection();
		this.playersList.ClearList(true);
		this.RefreshButtonStates();
		this.UpdateBarSelectionValues();
	}

	// Token: 0x06006291 RID: 25233 RVA: 0x002809B3 File Offset: 0x0027EBB3
	[PublicizedFrom(EAccessModifier.Private)]
	public void WorldList_OnEntryClicked(XUiController _sender, int _mouseButton)
	{
		this.savesList.ClearSelection();
		this.playersList.ClearSelection();
	}

	// Token: 0x06006292 RID: 25234 RVA: 0x002809CC File Offset: 0x0027EBCC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SavesList_SelectionChanged(XUiC_ListEntry<XUiC_DMSavegamesList.ListEntry> _previousEntry, XUiC_ListEntry<XUiC_DMSavegamesList.ListEntry> _newEntry)
	{
		XUiC_DMSavegamesList.ListEntry saveEntry = (_newEntry != null) ? _newEntry.GetEntry() : null;
		if (saveEntry != null)
		{
			IEnumerable<SaveInfoProvider.PlayerEntryInfo> playerEntries = from entry in SaveInfoProvider.Instance.PlayerEntryInfos
			where entry.SaveEntry.WorldEntry.WorldKey.Equals(saveEntry.worldKey) && entry.SaveEntry.Name.Equals(saveEntry.saveName)
			select entry;
			this.playerEntryResolver = SaveInfoProvider.PlayerEntryInfoPlatformDataResolver.StartNew(playerEntries);
			if (this.playerEntryResolver.IsComplete)
			{
				this.playersList.RebuildList(this.playerEntryResolver.pendingPlayerEntries, false);
				this.playerEntryResolver = null;
			}
			else
			{
				this.playersList.ShowLoading();
				this.playersList.ClearList(false);
			}
		}
		this.playersList.ClearSelection();
		this.RefreshButtonStates();
		this.UpdateBarSelectionValues();
	}

	// Token: 0x06006293 RID: 25235 RVA: 0x00280A7C File Offset: 0x0027EC7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SavesList_OnEntryClicked(XUiController _sender, int _mouseButton)
	{
		this.playersList.ClearSelection();
	}

	// Token: 0x06006294 RID: 25236 RVA: 0x00280A89 File Offset: 0x0027EC89
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayersList_SelectionChanged(XUiC_ListEntry<XUiC_DMPlayersList.ListEntry> _previousEntry, XUiC_ListEntry<XUiC_DMPlayersList.ListEntry> _newEntry)
	{
		this.RefreshButtonStates();
		this.UpdateBarSelectionValues();
	}

	// Token: 0x06006295 RID: 25237 RVA: 0x00280A98 File Offset: 0x0027EC98
	[PublicizedFrom(EAccessModifier.Private)]
	public void WorldList_OnChildElementHovered(XUiController _sender, bool _isOver)
	{
		if (!this.dataManagementBarEnabled)
		{
			return;
		}
		XUiC_ListEntry<XUiC_DMWorldList.ListEntry> xuiC_ListEntry = _sender as XUiC_ListEntry<XUiC_DMWorldList.ListEntry>;
		if (xuiC_ListEntry != null)
		{
			if (_isOver)
			{
				XUiC_DMWorldList.ListEntry entry = xuiC_ListEntry.GetEntry();
				SaveInfoProvider.WorldEntryInfo worldEntryInfo = (entry != null) ? entry.WorldEntryInfo : null;
				if (worldEntryInfo != null)
				{
					long size = worldEntryInfo.Deletable ? (worldEntryInfo.SaveDataSize + worldEntryInfo.WorldDataSize) : worldEntryInfo.SaveDataSize;
					XUiC_DataManagementBar.BarRegion hoveredByteRegion = new XUiC_DataManagementBar.BarRegion(worldEntryInfo.BarStartOffset, size);
					this.dataManagementBar.SetHoveredByteRegion(hoveredByteRegion);
					goto IL_76;
				}
			}
			this.dataManagementBar.SetHoveredByteRegion(XUiC_DataManagementBar.BarRegion.None);
		}
		IL_76:
		this.dataManagementBar.SetSelectionDepth(XUiC_DataManagementBar.SelectionDepth.Primary);
	}

	// Token: 0x06006296 RID: 25238 RVA: 0x00280B28 File Offset: 0x0027ED28
	[PublicizedFrom(EAccessModifier.Private)]
	public void SavesList_OnChildElementHovered(XUiController _sender, bool _isOver)
	{
		if (!this.dataManagementBarEnabled)
		{
			return;
		}
		XUiC_ListEntry<XUiC_DMSavegamesList.ListEntry> xuiC_ListEntry = _sender as XUiC_ListEntry<XUiC_DMSavegamesList.ListEntry>;
		if (xuiC_ListEntry != null)
		{
			if (_isOver)
			{
				XUiC_DMSavegamesList.ListEntry entry = xuiC_ListEntry.GetEntry();
				SaveInfoProvider.SaveEntryInfo saveEntryInfo = (entry != null) ? entry.saveEntryInfo : null;
				if (saveEntryInfo != null)
				{
					XUiC_DataManagementBar.BarRegion hoveredByteRegion = new XUiC_DataManagementBar.BarRegion(saveEntryInfo.BarStartOffset, saveEntryInfo.SizeInfo.ReportedSize);
					this.dataManagementBar.SetHoveredByteRegion(hoveredByteRegion);
					goto IL_62;
				}
			}
			this.dataManagementBar.SetHoveredByteRegion(XUiC_DataManagementBar.BarRegion.None);
		}
		IL_62:
		this.dataManagementBar.SetSelectionDepth(XUiC_DataManagementBar.SelectionDepth.Secondary);
	}

	// Token: 0x06006297 RID: 25239 RVA: 0x00280BA4 File Offset: 0x0027EDA4
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayersList_OnChildElementHovered(XUiController _sender, bool _isOver)
	{
		if (!this.dataManagementBarEnabled)
		{
			return;
		}
		XUiC_ListEntry<XUiC_DMPlayersList.ListEntry> xuiC_ListEntry = _sender as XUiC_ListEntry<XUiC_DMPlayersList.ListEntry>;
		if (xuiC_ListEntry != null)
		{
			if (_isOver)
			{
				XUiC_DMPlayersList.ListEntry entry = xuiC_ListEntry.GetEntry();
				SaveInfoProvider.PlayerEntryInfo playerEntryInfo = (entry != null) ? entry.playerEntryInfo : null;
				if (playerEntryInfo != null)
				{
					XUiC_DataManagementBar.BarRegion hoveredByteRegion = new XUiC_DataManagementBar.BarRegion(playerEntryInfo.BarStartOffset, playerEntryInfo.Size);
					this.dataManagementBar.SetHoveredByteRegion(hoveredByteRegion);
					goto IL_5D;
				}
			}
			this.dataManagementBar.SetHoveredByteRegion(XUiC_DataManagementBar.BarRegion.None);
		}
		IL_5D:
		this.dataManagementBar.SetSelectionDepth(XUiC_DataManagementBar.SelectionDepth.Tertiary);
	}

	// Token: 0x06006298 RID: 25240 RVA: 0x00280C1A File Offset: 0x0027EE1A
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.CloseDataManagementWindow();
	}

	// Token: 0x06006299 RID: 25241 RVA: 0x00280C24 File Offset: 0x0027EE24
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDeleteWorld_OnPressed(XUiController _sender, int _mouseButton)
	{
		XUiC_DMSavegamesList xuiC_DMSavegamesList = this.savesList;
		XUiC_ListEntry<XUiC_DMWorldList.ListEntry> selectedEntry = this.worldList.SelectedEntry;
		int num = xuiC_DMSavegamesList.GetSavesInWorld((selectedEntry != null) ? selectedEntry.GetEntry().Key : null).Count<XUiC_DMSavegamesList.ListEntry>();
		this.confirmationPrompt.ShowPrompt(Localization.Get("xuiWorldDelete", false), Localization.Get((num > 0) ? string.Format(Localization.Get("xuiWorldDeleteConfirmation", false), num) : Localization.Get("xuiWorldDeleteConfirmationNoSaves", false), false), Localization.Get("xuiCancel", false), Localization.Get("btnConfirm", false), new Action<XUiC_ConfirmationPrompt.Result>(this.OnDeleteWorldPromptConfirmationResult));
		this.dataManagementBar.SetDeleteWindowDisplayed(true);
	}

	// Token: 0x0600629A RID: 25242 RVA: 0x00280CCF File Offset: 0x0027EECF
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDeleteWorld_OnHover(XUiController _sender, bool _isOver)
	{
		if (this.dataManagementBarEnabled)
		{
			this.dataManagementBar.SetSelectionDepth(XUiC_DataManagementBar.SelectionDepth.Primary);
			this.dataManagementBar.SetDeleteHovered(_isOver);
		}
	}

	// Token: 0x0600629B RID: 25243 RVA: 0x00280CF4 File Offset: 0x0027EEF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDeleteSave_OnPressed(XUiController _sender, int _mouseButton)
	{
		XUiC_ListEntry<XUiC_DMWorldList.ListEntry> selectedEntry = this.worldList.SelectedEntry;
		bool flag = ((selectedEntry != null) ? selectedEntry.GetEntry().Type : null) == SaveInfoProvider.RemoteWorldsType;
		this.confirmationPrompt.ShowPrompt(Localization.Get(flag ? "xuiDmDeleteRemoteSave" : "xuiDeleteSaveGame", false), Localization.Get(flag ? "xuiDmDeleteRemoteSaveConfirmation" : "xuiDeleteSaveGame", false), Localization.Get("xuiCancel", false), Localization.Get("btnConfirm", false), new Action<XUiC_ConfirmationPrompt.Result>(this.OnDeleteSavePromptConfirmationResult));
		this.dataManagementBar.SetDeleteWindowDisplayed(true);
	}

	// Token: 0x0600629C RID: 25244 RVA: 0x00280D8B File Offset: 0x0027EF8B
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDeleteSave_OnHover(XUiController _sender, bool _isOver)
	{
		if (this.dataManagementBarEnabled)
		{
			this.dataManagementBar.SetSelectionDepth(XUiC_DataManagementBar.SelectionDepth.Secondary);
			this.dataManagementBar.SetDeleteHovered(_isOver);
		}
	}

	// Token: 0x0600629D RID: 25245 RVA: 0x00280DB0 File Offset: 0x0027EFB0
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnArchiveSave_OnPressed(XUiController _sender, int _mouseButton)
	{
		XUiC_DMSavegamesList xuiC_DMSavegamesList = this.savesList;
		XUiC_DMSavegamesList.ListEntry listEntry;
		if (xuiC_DMSavegamesList == null)
		{
			listEntry = null;
		}
		else
		{
			XUiC_ListEntry<XUiC_DMSavegamesList.ListEntry> selectedEntry = xuiC_DMSavegamesList.SelectedEntry;
			listEntry = ((selectedEntry != null) ? selectedEntry.GetEntry() : null);
		}
		XUiC_DMSavegamesList.ListEntry listEntry2 = listEntry;
		if (listEntry2 != null)
		{
			int selectedEntryIndex = this.worldList.SelectedEntryIndex;
			int selectedEntryIndex2 = this.savesList.SelectedEntryIndex;
			string text = Path.Combine(listEntry2.saveDirectory, "archived.flag");
			if (listEntry2.saveEntryInfo.SizeInfo.IsArchived)
			{
				long num = this.archivableSizeInfo.BytesReserved - this.archivableSizeInfo.BytesOnDisk;
				if (SaveInfoProvider.Instance.TotalAvailableBytes < num)
				{
					this.confirmationPrompt.ShowPrompt(Localization.Get("xuiDmRestoreFailureTitle", false), Localization.Get("xuiDmRestoreFailureBody", false), Localization.Get("xuiOk", false), string.Empty, new Action<XUiC_ConfirmationPrompt.Result>(this.OnRestoreFailurePromptConfirmationResult));
					return;
				}
				Log.Out("Unarchiving save by deleting: " + text);
				SdFile.Delete(text);
			}
			else
			{
				Log.Out("Archiving save by creating: " + text);
				using (SdFile.Create(text))
				{
				}
			}
			SaveInfoProvider.Instance.SetDirty();
			this.Refresh();
			this.worldList.SelectedEntryIndex = selectedEntryIndex;
			this.savesList.SelectedEntryIndex = selectedEntryIndex2;
			this.SetBarArchivePreviewModeActive(true);
		}
	}

	// Token: 0x0600629E RID: 25246 RVA: 0x00280F00 File Offset: 0x0027F100
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDeleteWorldPromptConfirmationResult(XUiC_ConfirmationPrompt.Result result)
	{
		this.ProcessDeletionConfirmationResult(XUiC_DataManagement.DeletionMode.World, result);
	}

	// Token: 0x0600629F RID: 25247 RVA: 0x00280F0A File Offset: 0x0027F10A
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDeleteSavePromptConfirmationResult(XUiC_ConfirmationPrompt.Result result)
	{
		this.ProcessDeletionConfirmationResult(XUiC_DataManagement.DeletionMode.Save, result);
	}

	// Token: 0x060062A0 RID: 25248 RVA: 0x00280F14 File Offset: 0x0027F114
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDeletePlayerPromptConfirmationResult(XUiC_ConfirmationPrompt.Result result)
	{
		this.ProcessDeletionConfirmationResult(XUiC_DataManagement.DeletionMode.Player, result);
	}

	// Token: 0x060062A1 RID: 25249 RVA: 0x00280F1E File Offset: 0x0027F11E
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDeleteBlockedPlayersPromptConfirmationResult(XUiC_ConfirmationPrompt.Result result)
	{
		this.ProcessDeletionConfirmationResult(XUiC_DataManagement.DeletionMode.BlockedPlayers, result);
	}

	// Token: 0x060062A2 RID: 25250 RVA: 0x00280F28 File Offset: 0x0027F128
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnRestoreFailurePromptConfirmationResult(XUiC_ConfirmationPrompt.Result result)
	{
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
		{
			this.SetBarArchivePreviewModeActive(false);
		}
		base.xui.playerUI.CursorController.SetNavigationLockView(this.viewComponent, this.btnArchiveSave.ViewComponent);
	}

	// Token: 0x060062A3 RID: 25251 RVA: 0x00280F74 File Offset: 0x0027F174
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnArchiveSave_OnHover(XUiController _sender, bool _isOver)
	{
		if (this.confirmationPrompt.IsVisible)
		{
			return;
		}
		this.SetBarArchivePreviewModeActive(_isOver);
	}

	// Token: 0x060062A4 RID: 25252 RVA: 0x00280F8C File Offset: 0x0027F18C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetBarArchivePreviewModeActive(bool _isActive)
	{
		if (this.dataManagementBarEnabled)
		{
			this.dataManagementBar.SetSelectionDepth(XUiC_DataManagementBar.SelectionDepth.Secondary);
			XUiC_ListEntry<XUiC_DMSavegamesList.ListEntry> selectedEntry = this.savesList.SelectedEntry;
			XUiC_DMSavegamesList.ListEntry listEntry = (selectedEntry != null) ? selectedEntry.GetEntry() : null;
			if (_isActive && this.archiveButtonVisible && listEntry != null)
			{
				long num = this.archivableSizeInfo.BytesReserved - this.archivableSizeInfo.BytesOnDisk;
				if (this.archivableSizeInfo.IsArchived)
				{
					this.dataManagementBar.SetDisplayMode(XUiC_DataManagementBar.DisplayMode.Preview);
					this.dataManagementBar.SetPendingBytes(num);
					return;
				}
				this.dataManagementBar.SetDisplayMode(XUiC_DataManagementBar.DisplayMode.Selection);
				long offset = listEntry.saveEntryInfo.BarStartOffset + listEntry.saveEntryInfo.SizeInfo.BytesOnDisk;
				XUiC_DataManagementBar.BarRegion archivePreviewRegion = new XUiC_DataManagementBar.BarRegion(offset, num);
				this.dataManagementBar.SetArchivePreviewRegion(archivePreviewRegion);
				return;
			}
			else
			{
				this.dataManagementBar.SetDisplayMode(XUiC_DataManagementBar.DisplayMode.Selection);
				this.dataManagementBar.SetArchivePreviewRegion(XUiC_DataManagementBar.BarRegion.None);
			}
		}
	}

	// Token: 0x060062A5 RID: 25253 RVA: 0x00281078 File Offset: 0x0027F278
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDeletePlayer_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.confirmationPrompt.ShowPrompt(Localization.Get("xuiDmDeletePlayer", false), Localization.Get("xuiDmDeletePlayerConfirmation", false), Localization.Get("xuiCancel", false), Localization.Get("btnConfirm", false), new Action<XUiC_ConfirmationPrompt.Result>(this.OnDeletePlayerPromptConfirmationResult));
		this.dataManagementBar.SetDeleteWindowDisplayed(true);
	}

	// Token: 0x060062A6 RID: 25254 RVA: 0x002810D4 File Offset: 0x0027F2D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDeletePlayer_OnHover(XUiController _sender, bool _isOver)
	{
		if (this.dataManagementBarEnabled)
		{
			this.dataManagementBar.SetSelectionDepth(XUiC_DataManagementBar.SelectionDepth.Tertiary);
			this.dataManagementBar.SetDeleteHovered(_isOver);
		}
	}

	// Token: 0x060062A7 RID: 25255 RVA: 0x002810F8 File Offset: 0x0027F2F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDeleteBlockedPlayers_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.playersList.ClearSelection();
		this.showBlockedPlayersInBar = true;
		this.UpdateBarSelectionValues();
		this.confirmationPrompt.ShowPrompt(Localization.Get("xuiDmDeleteBlockedPlayers", false), Localization.Get("xuiDmDeletePlayerConfirmation", false), Localization.Get("xuiCancel", false), Localization.Get("btnConfirm", false), new Action<XUiC_ConfirmationPrompt.Result>(this.OnDeleteBlockedPlayersPromptConfirmationResult));
	}

	// Token: 0x060062A8 RID: 25256 RVA: 0x00281160 File Offset: 0x0027F360
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDeleteBlockedPlayers_OnHover(XUiController _sender, bool _isOver)
	{
		if (!this.confirmationPrompt.IsVisible && _isOver != this.showBlockedPlayersInBar)
		{
			this.showBlockedPlayersInBar = _isOver;
			this.UpdateBarSelectionValues();
		}
		if (this.dataManagementBarEnabled)
		{
			this.dataManagementBar.SetSelectionDepth(XUiC_DataManagementBar.SelectionDepth.Tertiary);
			this.dataManagementBar.SetDeleteHovered(_isOver);
		}
	}

	// Token: 0x060062A9 RID: 25257 RVA: 0x002811B0 File Offset: 0x0027F3B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcessDeletionConfirmationResult(XUiC_DataManagement.DeletionMode deletionMode, XUiC_ConfirmationPrompt.Result result)
	{
		if (result == XUiC_ConfirmationPrompt.Result.Confirmed)
		{
			this.ProcessDeletion(deletionMode);
			return;
		}
		this.CancelDeletion(deletionMode);
	}

	// Token: 0x060062AA RID: 25258 RVA: 0x002811C8 File Offset: 0x0027F3C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CancelDeletion(XUiC_DataManagement.DeletionMode deletionMode)
	{
		XUiC_SimpleButton xuiC_SimpleButton;
		switch (deletionMode)
		{
		case XUiC_DataManagement.DeletionMode.World:
			xuiC_SimpleButton = this.btnDeleteWorld;
			break;
		case XUiC_DataManagement.DeletionMode.Save:
			xuiC_SimpleButton = this.btnDeleteSave;
			break;
		case XUiC_DataManagement.DeletionMode.Player:
			xuiC_SimpleButton = this.btnDeletePlayer;
			break;
		default:
			xuiC_SimpleButton = null;
			break;
		}
		XUiController xuiController = xuiC_SimpleButton;
		this.dataManagementBar.SetDeleteWindowDisplayed(false);
		base.xui.playerUI.CursorController.SetNavigationLockView(this.viewComponent, (xuiController != null) ? xuiController.ViewComponent : null);
		this.showBlockedPlayersInBar = false;
		this.UpdateBarSelectionValues();
	}

	// Token: 0x060062AB RID: 25259 RVA: 0x0028124C File Offset: 0x0027F44C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcessDeletion(XUiC_DataManagement.DeletionMode deletionMode)
	{
		int num = this.worldList.SelectedEntryIndex;
		int num2 = this.savesList.SelectedEntryIndex;
		int num3 = this.playersList.SelectedEntryIndex;
		XUiController xuiController = null;
		switch (deletionMode)
		{
		case XUiC_DataManagement.DeletionMode.World:
			try
			{
				XUiC_ListEntry<XUiC_DMWorldList.ListEntry> selectedEntry = this.worldList.SelectedEntry;
				XUiC_DMWorldList.ListEntry listEntry = (selectedEntry != null) ? selectedEntry.GetEntry() : null;
				if (listEntry == null)
				{
					throw new NotSupportedException("Failed to retrieve selected world.");
				}
				if (!listEntry.Deletable)
				{
					throw new NotSupportedException("Tried to delete non-generated world.");
				}
				if (listEntry.Location == PathAbstractions.AbstractedLocation.None)
				{
					throw new NotSupportedException("Tried to delete world entry with location == none.");
				}
				GameUtils.DeleteWorld(listEntry.Location);
				if (GamePrefs.GetString(EnumGamePrefs.GameWorld).EqualsCaseInsensitive(listEntry.Location.Name))
				{
					GamePrefs.Set(EnumGamePrefs.GameWorld, "Navezgane");
				}
				num = Mathf.Clamp(num, -1, this.worldList.EntryCount - 1);
				num2 = -1;
				num3 = -1;
			}
			catch (Exception ex)
			{
				Log.Error("Error occurred while deleting world: \"" + ex.Message + "\"");
			}
			xuiController = this.btnDeleteWorld;
			goto IL_3DD;
		case XUiC_DataManagement.DeletionMode.Save:
			try
			{
				XUiC_ListEntry<XUiC_DMSavegamesList.ListEntry> selectedEntry2 = this.savesList.SelectedEntry;
				XUiC_DMSavegamesList.ListEntry listEntry2 = (selectedEntry2 != null) ? selectedEntry2.GetEntry() : null;
				if (listEntry2 == null)
				{
					throw new NotSupportedException("Failed to retrieve selected save.");
				}
				SdDirectory.Delete(listEntry2.saveDirectory, true);
				num2 = Mathf.Clamp(num2, -1, this.savesList.EntryCount - 1);
				num3 = -1;
			}
			catch (Exception ex2)
			{
				Log.Error("Error occurred while deleting save: \"" + ex2.Message + "\"");
			}
			xuiController = this.btnDeleteSave;
			goto IL_3DD;
		case XUiC_DataManagement.DeletionMode.Player:
			try
			{
				XUiC_ListEntry<XUiC_DMPlayersList.ListEntry> selectedEntry3 = this.playersList.SelectedEntry;
				XUiC_DMPlayersList.ListEntry listEntry3 = (selectedEntry3 != null) ? selectedEntry3.GetEntry() : null;
				if (listEntry3 == null)
				{
					throw new Exception("Failed to retrieve selected player entry.");
				}
				XUiC_ListEntry<XUiC_DMSavegamesList.ListEntry> selectedEntry4 = this.savesList.SelectedEntry;
				XUiC_DMSavegamesList.ListEntry listEntry4 = (selectedEntry4 != null) ? selectedEntry4.GetEntry() : null;
				if (listEntry4 == null)
				{
					throw new Exception("Failed to retrieve selected save entry.");
				}
				string text = listEntry4.saveDirectory + "/Player";
				if (!SdDirectory.Exists(text))
				{
					throw new Exception("Player save data directory not found at expected path: " + text + ".");
				}
				foreach (SdFileSystemInfo sdFileSystemInfo in new SdDirectoryInfo(text).GetFileSystemInfos(listEntry3.id + "*"))
				{
					if (string.IsNullOrEmpty(sdFileSystemInfo.Extension) && SdDirectory.Exists(sdFileSystemInfo.FullName))
					{
						SdDirectory.Delete(sdFileSystemInfo.FullName);
					}
					else
					{
						SdFile.Delete(sdFileSystemInfo.FullName);
					}
				}
				num3 = Mathf.Clamp(num3, -1, this.playersList.EntryCount - 1);
			}
			catch (Exception ex3)
			{
				Log.Error("Error occurred while deleting player: \"" + ex3.Message + "\"");
			}
			xuiController = this.btnDeletePlayer;
			goto IL_3DD;
		case XUiC_DataManagement.DeletionMode.BlockedPlayers:
			try
			{
				XUiC_ListEntry<XUiC_DMSavegamesList.ListEntry> selectedEntry5 = this.savesList.SelectedEntry;
				XUiC_DMSavegamesList.ListEntry listEntry5 = (selectedEntry5 != null) ? selectedEntry5.GetEntry() : null;
				if (listEntry5 == null)
				{
					throw new Exception("Failed to retrieve selected save entry.");
				}
				string text2 = listEntry5.saveDirectory + "/Player";
				if (!SdDirectory.Exists(text2))
				{
					throw new Exception("Player save data directory not found at expected path: " + text2 + ".");
				}
				SdDirectoryInfo sdDirectoryInfo = new SdDirectoryInfo(text2);
				foreach (SaveInfoProvider.PlayerEntryInfo playerEntryInfo in this.playersList.BlockedPlayers)
				{
					try
					{
						SdFileSystemInfo[] fileSystemInfos = sdDirectoryInfo.GetFileSystemInfos(playerEntryInfo.Id + "*");
						for (int i = 0; i < fileSystemInfos.Length; i++)
						{
							SdFile.Delete(fileSystemInfos[i].FullName);
						}
					}
					catch (Exception ex4)
					{
						Log.Error("Error occurred while deleting blocked player save files: " + ex4.Message);
					}
				}
			}
			catch (Exception ex5)
			{
				Log.Error("Error occurred while deleting blocked players: \"" + ex5.Message + "\"");
			}
			this.showBlockedPlayersInBar = false;
			goto IL_3DD;
		}
		UnityEngine.Debug.LogError("Error in internal logic: invalid deletion mode. No data will be deleted.");
		IL_3DD:
		SaveInfoProvider.Instance.SetDirty();
		this.Refresh();
		this.worldList.SelectedEntryIndex = num;
		this.savesList.SelectedEntryIndex = num2;
		this.playersList.SelectedEntryIndex = num3;
		this.dataManagementBar.SetDeleteWindowDisplayed(false);
		base.xui.playerUI.CursorController.SetNavigationLockView(this.viewComponent, (xuiController != null) ? xuiController.ViewComponent : null);
	}

	// Token: 0x060062AC RID: 25260 RVA: 0x00281734 File Offset: 0x0027F934
	[Conditional("DELETION_LOG_ENABLED")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogDeletion(string v)
	{
		UnityEngine.Debug.Log(v);
	}

	// Token: 0x060062AD RID: 25261 RVA: 0x0028173C File Offset: 0x0027F93C
	public static int BytesToMebibytes(long bytes)
	{
		return Mathf.CeilToInt((float)bytes / 1024f / 1024f);
	}

	// Token: 0x060062AE RID: 25262 RVA: 0x00281754 File Offset: 0x0027F954
	public static string FormatMemoryString(long bytes)
	{
		return XUiC_DataManagement.BytesToMebibytes(bytes).ToString() + " MB";
	}

	// Token: 0x060062AF RID: 25263 RVA: 0x0028177C File Offset: 0x0027F97C
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshButtonStates()
	{
		XUiC_DMWorldList xuiC_DMWorldList = this.worldList;
		XUiC_DMWorldList.ListEntry listEntry;
		if (xuiC_DMWorldList == null)
		{
			listEntry = null;
		}
		else
		{
			XUiC_ListEntry<XUiC_DMWorldList.ListEntry> selectedEntry = xuiC_DMWorldList.SelectedEntry;
			listEntry = ((selectedEntry != null) ? selectedEntry.GetEntry() : null);
		}
		XUiC_DMWorldList.ListEntry listEntry2 = listEntry;
		this.worldDeletable = (listEntry2 != null && listEntry2.Deletable && !SaveInfoProvider.Instance.IsDirectoryProtected(listEntry2.Location.FullPath));
		XUiC_DMSavegamesList xuiC_DMSavegamesList = this.savesList;
		XUiC_DMSavegamesList.ListEntry listEntry3;
		if (xuiC_DMSavegamesList == null)
		{
			listEntry3 = null;
		}
		else
		{
			XUiC_ListEntry<XUiC_DMSavegamesList.ListEntry> selectedEntry2 = xuiC_DMSavegamesList.SelectedEntry;
			listEntry3 = ((selectedEntry2 != null) ? selectedEntry2.GetEntry() : null);
		}
		XUiC_DMSavegamesList.ListEntry listEntry4 = listEntry3;
		if (listEntry4 != null)
		{
			this.saveDeletable = !SaveInfoProvider.Instance.IsDirectoryProtected(listEntry4.saveDirectory);
			this.archiveButtonVisible = (this.saveDeletable && listEntry4.saveEntryInfo.SizeInfo.Archivable);
			this.archivableSizeInfo = listEntry4.saveEntryInfo.SizeInfo;
		}
		else
		{
			this.saveDeletable = false;
			this.archiveButtonVisible = false;
		}
		this.playerDeletable = (this.saveDeletable && this.playersList.SelectedEntryIndex >= 0);
		this.IsDirty = true;
	}

	// Token: 0x060062B0 RID: 25264 RVA: 0x0028187C File Offset: 0x0027FA7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateBarSelectionValues()
	{
		if (!this.dataManagementBarEnabled)
		{
			return;
		}
		XUiC_DataManagementBar.BarRegion none = XUiC_DataManagementBar.BarRegion.None;
		XUiC_DataManagementBar.BarRegion none2 = XUiC_DataManagementBar.BarRegion.None;
		XUiC_DataManagementBar.BarRegion none3 = XUiC_DataManagementBar.BarRegion.None;
		XUiC_ListEntry<XUiC_DMWorldList.ListEntry> selectedEntry = this.worldList.SelectedEntry;
		XUiC_DMWorldList.ListEntry listEntry = (selectedEntry != null) ? selectedEntry.GetEntry() : null;
		if (listEntry != null)
		{
			long size = listEntry.Deletable ? (listEntry.SaveDataSize + listEntry.WorldDataSize) : listEntry.SaveDataSize;
			none = new XUiC_DataManagementBar.BarRegion(listEntry.WorldEntryInfo.BarStartOffset, size);
			XUiC_ListEntry<XUiC_DMSavegamesList.ListEntry> selectedEntry2 = this.savesList.SelectedEntry;
			XUiC_DMSavegamesList.ListEntry listEntry2 = (selectedEntry2 != null) ? selectedEntry2.GetEntry() : null;
			if (listEntry2 != null)
			{
				none2 = new XUiC_DataManagementBar.BarRegion(listEntry2.saveEntryInfo.BarStartOffset, listEntry2.saveEntryInfo.SizeInfo.ReportedSize);
				if (this.showBlockedPlayersInBar)
				{
					long num = 0L;
					foreach (SaveInfoProvider.PlayerEntryInfo playerEntryInfo in this.playersList.BlockedPlayers)
					{
						num += playerEntryInfo.Size;
					}
					none3 = new XUiC_DataManagementBar.BarRegion(none2.End - num, num);
				}
				else
				{
					XUiC_ListEntry<XUiC_DMPlayersList.ListEntry> selectedEntry3 = this.playersList.SelectedEntry;
					XUiC_DMPlayersList.ListEntry listEntry3 = (selectedEntry3 != null) ? selectedEntry3.GetEntry() : null;
					if (listEntry3 != null)
					{
						none3 = new XUiC_DataManagementBar.BarRegion(listEntry3.playerEntryInfo.BarStartOffset, listEntry3.playerEntryInfo.Size);
					}
				}
			}
		}
		this.dataManagementBar.SetSelectedByteRegion(none, none2, none3);
	}

	// Token: 0x060062B1 RID: 25265 RVA: 0x002819F4 File Offset: 0x0027FBF4
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "default_font_color")
		{
			this.defaultFontColor = _value;
			return true;
		}
		if (!(_name == "invalid_font_color"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.invalidFontColor = _value;
		return true;
	}

	// Token: 0x060062B2 RID: 25266 RVA: 0x00281A30 File Offset: 0x0027FC30
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 1457541641U)
		{
			if (num <= 594028389U)
			{
				if (num != 184981848U)
				{
					if (num == 594028389U)
					{
						if (_bindingName == "worlddeletable")
						{
							_value = this.worldDeletable.ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "false")
				{
					_value = "false";
					return true;
				}
			}
			else if (num != 631641732U)
			{
				if (num != 881969661U)
				{
					if (num == 1457541641U)
					{
						if (_bindingName == "showbar")
						{
							_value = this.dataManagementBarEnabled.ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "archivelabel")
				{
					if (this.archiveButtonVisible)
					{
						long num2 = this.archivableSizeInfo.BytesReserved - this.archivableSizeInfo.BytesOnDisk;
						long num3 = (long)(XUiC_DataManagement.BytesToMebibytes(this.archivableSizeInfo.BytesReserved) - XUiC_DataManagement.BytesToMebibytes(this.archivableSizeInfo.BytesOnDisk));
						if (this.archivableSizeInfo.IsArchived)
						{
							if (SaveInfoProvider.Instance.TotalAvailableBytes >= num2)
							{
								_value = string.Format("{0} ({1} MB)", Localization.Get("xuiDmRestoreLabel", false), num3);
							}
							else
							{
								_value = string.Format("{0} ([ff9999]{1} MB[-])", Localization.Get("xuiDmRestoreLabel", false), num3);
							}
						}
						else
						{
							_value = string.Format("{0} ({1} MB)", Localization.Get("xuiDmArchiveLabel", false), num3);
						}
					}
					else
					{
						_value = string.Empty;
					}
					return true;
				}
			}
			else if (_bindingName == "playerdeletable")
			{
				_value = this.playerDeletable.ToString();
				return true;
			}
		}
		else if (num <= 2266769916U)
		{
			if (num != 1705753217U)
			{
				if (num == 2266769916U)
				{
					if (_bindingName == "savedeletable")
					{
						_value = this.saveDeletable.ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "hasblockedplayers")
			{
				XUiC_DMPlayersList xuiC_DMPlayersList = this.playersList;
				_value = (((xuiC_DMPlayersList != null) ? xuiC_DMPlayersList.HasBlockedPlayers.ToString() : null) ?? string.Empty);
				return true;
			}
		}
		else if (num != 3033975131U)
		{
			if (num != 3267589955U)
			{
				if (num == 4077752995U)
				{
					if (_bindingName == "savearchivable")
					{
						_value = this.archiveButtonVisible.ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "blockedplayers")
			{
				XUiC_DMPlayersList xuiC_DMPlayersList2 = this.playersList;
				if (xuiC_DMPlayersList2 != null && xuiC_DMPlayersList2.HasBlockedPlayers)
				{
					_value = string.Format(Localization.Get("xuiDmBlockedPlayers", false), this.playersList.BlockedPlayerCount);
				}
				else
				{
					_value = string.Empty;
				}
				return true;
			}
		}
		else if (_bindingName == "saveversioncolor")
		{
			_value = (this.saveVersionValid ? this.defaultFontColor : this.invalidFontColor);
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x060062B3 RID: 25267 RVA: 0x00281D4C File Offset: 0x0027FF4C
	public static void OpenDataManagementWindow(XUiController parentController, Action onClosedCallback = null)
	{
		GUIWindowManager windowManager = parentController.xui.playerUI.windowManager;
		windowManager.Open(XUiC_DataManagement.ID, true, false, false);
		XUiWindowGroup xuiWindowGroup = (XUiWindowGroup)windowManager.GetWindow(XUiC_DataManagement.ID);
		XUiC_DataManagement xuiC_DataManagement;
		if (xuiWindowGroup == null)
		{
			xuiC_DataManagement = null;
		}
		else
		{
			XUiController controller = xuiWindowGroup.Controller;
			xuiC_DataManagement = ((controller != null) ? controller.GetChildByType<XUiC_DataManagement>() : null);
		}
		XUiC_DataManagement xuiC_DataManagement2 = xuiC_DataManagement;
		if (xuiC_DataManagement2 == null)
		{
			UnityEngine.Debug.LogError("Failed to retrieve reference to XUiC_DataManagement instance.");
			return;
		}
		xuiC_DataManagement2.SetParentController(parentController);
		xuiC_DataManagement2.onClosedCallback = onClosedCallback;
	}

	// Token: 0x060062B4 RID: 25268 RVA: 0x00281DBB File Offset: 0x0027FFBB
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetParentController(XUiController parentController)
	{
		this.m_parentControllerState = new ParentControllerState(parentController);
		this.m_parentControllerState.Hide();
	}

	// Token: 0x060062B5 RID: 25269 RVA: 0x00281DD4 File Offset: 0x0027FFD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void CloseDataManagementWindow()
	{
		base.xui.playerUI.CursorController.SetNavigationLockView(this.previousLockView, null);
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		this.m_parentControllerState.Restore();
		Action action = this.onClosedCallback;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x060062B6 RID: 25270 RVA: 0x00281E38 File Offset: 0x00280038
	public static bool IsWindowOpen(XUi xui)
	{
		return xui.playerUI.windowManager.IsWindowOpen(XUiC_DataManagement.ID);
	}

	// Token: 0x04004A30 RID: 18992
	[PublicizedFrom(EAccessModifier.Private)]
	public static string ID = "";

	// Token: 0x04004A31 RID: 18993
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DMWorldList worldList;

	// Token: 0x04004A32 RID: 18994
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DMSavegamesList savesList;

	// Token: 0x04004A33 RID: 18995
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DMPlayersList playersList;

	// Token: 0x04004A34 RID: 18996
	[PublicizedFrom(EAccessModifier.Private)]
	public SaveInfoProvider.PlayerEntryInfoPlatformDataResolver playerEntryResolver;

	// Token: 0x04004A35 RID: 18997
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDeleteWorld;

	// Token: 0x04004A36 RID: 18998
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDeleteSave;

	// Token: 0x04004A37 RID: 18999
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnArchiveSave;

	// Token: 0x04004A38 RID: 19000
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDeletePlayer;

	// Token: 0x04004A39 RID: 19001
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDeleteBlockedPlayers;

	// Token: 0x04004A3A RID: 19002
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ConfirmationPrompt confirmationPrompt;

	// Token: 0x04004A3B RID: 19003
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DataManagementBar dataManagementBar;

	// Token: 0x04004A3C RID: 19004
	[PublicizedFrom(EAccessModifier.Private)]
	public string invalidFontColor = "255,0,0";

	// Token: 0x04004A3D RID: 19005
	[PublicizedFrom(EAccessModifier.Private)]
	public string defaultFontColor = "255,255,255";

	// Token: 0x04004A3E RID: 19006
	[PublicizedFrom(EAccessModifier.Private)]
	public bool worldDeletable;

	// Token: 0x04004A3F RID: 19007
	[PublicizedFrom(EAccessModifier.Private)]
	public bool saveDeletable;

	// Token: 0x04004A40 RID: 19008
	[PublicizedFrom(EAccessModifier.Private)]
	public bool archiveButtonVisible;

	// Token: 0x04004A41 RID: 19009
	[PublicizedFrom(EAccessModifier.Private)]
	public SaveInfoProvider.SaveSizeInfo archivableSizeInfo;

	// Token: 0x04004A42 RID: 19010
	[PublicizedFrom(EAccessModifier.Private)]
	public bool saveVersionValid;

	// Token: 0x04004A43 RID: 19011
	[PublicizedFrom(EAccessModifier.Private)]
	public bool playerDeletable;

	// Token: 0x04004A44 RID: 19012
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showBlockedPlayersInBar;

	// Token: 0x04004A45 RID: 19013
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView previousLockView;

	// Token: 0x04004A46 RID: 19014
	[PublicizedFrom(EAccessModifier.Private)]
	public ParentControllerState m_parentControllerState;

	// Token: 0x04004A47 RID: 19015
	[PublicizedFrom(EAccessModifier.Private)]
	public Action onClosedCallback;

	// Token: 0x04004A48 RID: 19016
	[PublicizedFrom(EAccessModifier.Private)]
	public bool dataManagementBarEnabled;

	// Token: 0x02000C7D RID: 3197
	[PublicizedFrom(EAccessModifier.Private)]
	public enum DeletionMode
	{
		// Token: 0x04004A4A RID: 19018
		None,
		// Token: 0x04004A4B RID: 19019
		World,
		// Token: 0x04004A4C RID: 19020
		Save,
		// Token: 0x04004A4D RID: 19021
		Player,
		// Token: 0x04004A4E RID: 19022
		BlockedPlayers
	}
}
