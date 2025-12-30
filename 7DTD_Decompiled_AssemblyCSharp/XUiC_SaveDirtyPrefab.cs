using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DE8 RID: 3560
[Preserve]
public class XUiC_SaveDirtyPrefab : XUiController
{
	// Token: 0x06006F9F RID: 28575 RVA: 0x002D8E84 File Offset: 0x002D7084
	public override void Init()
	{
		base.Init();
		XUiC_SaveDirtyPrefab.ID = base.WindowGroup.ID;
		((XUiC_SimpleButton)base.GetChildById("btnSave")).OnPressed += this.BtnSave_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnCancel")).OnPressed += this.BtnCancel_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnDontSave")).OnPressed += this.BtnDontSave_OnPressed;
		this.txtSaveName = (XUiC_TextInput)base.GetChildById("txtSaveName");
		this.txtSaveName.OnChangeHandler += this.TxtSaveNameOnOnChangeHandler;
	}

	// Token: 0x06006FA0 RID: 28576 RVA: 0x002D8F38 File Offset: 0x002D7138
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtSaveNameOnOnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (this.nameRequired)
		{
			this.nameInvalid = (_text.Length <= 0 || _text.Contains(" ") || !GameUtils.ValidateGameName(_text));
			this.nameExists = (!this.nameInvalid && Prefab.LocationForNewPrefab(_text, null).Exists());
		}
		else
		{
			this.nameInvalid = false;
			this.nameExists = false;
		}
		base.RefreshBindings(false);
	}

	// Token: 0x06006FA1 RID: 28577 RVA: 0x002D8FAC File Offset: 0x002D71AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnSave_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.CloseWith(XUiC_SaveDirtyPrefab.ESelectedAction.Save);
	}

	// Token: 0x06006FA2 RID: 28578 RVA: 0x002D8FB5 File Offset: 0x002D71B5
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.CloseWith(XUiC_SaveDirtyPrefab.ESelectedAction.Cancel);
	}

	// Token: 0x06006FA3 RID: 28579 RVA: 0x002D8FBE File Offset: 0x002D71BE
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDontSave_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.CloseWith(XUiC_SaveDirtyPrefab.ESelectedAction.DontSave);
	}

	// Token: 0x06006FA4 RID: 28580 RVA: 0x002D8FC8 File Offset: 0x002D71C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CloseWith(XUiC_SaveDirtyPrefab.ESelectedAction _action)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		if (_action == XUiC_SaveDirtyPrefab.ESelectedAction.Save)
		{
			if (this.nameRequired)
			{
				string text = this.txtSaveName.Text;
				PrefabEditModeManager.Instance.VoxelPrefab.location = Prefab.LocationForNewPrefab(text, null);
			}
			if (PrefabEditModeManager.Instance.SaveVoxelPrefab())
			{
				GameManager.ShowTooltip(GameManager.Instance.World.GetLocalPlayers()[0], string.Format(Localization.Get("xuiPrefabsPrefabSaved", false), PrefabEditModeManager.Instance.LoadedPrefab.Name), false, false, 0f);
			}
			else
			{
				GameManager.ShowTooltip(GameManager.Instance.World.GetLocalPlayers()[0], Localization.Get("xuiPrefabsPrefabSavingError", false), false, false, 0f);
				_action = XUiC_SaveDirtyPrefab.ESelectedAction.Cancel;
			}
		}
		ThreadManager.StartCoroutine(this.delayCallback(_action));
	}

	// Token: 0x06006FA5 RID: 28581 RVA: 0x002D90AF File Offset: 0x002D72AF
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator delayCallback(XUiC_SaveDirtyPrefab.ESelectedAction _action)
	{
		yield return new WaitForSeconds(0.1f);
		Action<XUiC_SaveDirtyPrefab.ESelectedAction> action = this.onCloseAction;
		if (action != null)
		{
			action(_action);
		}
		yield break;
	}

	// Token: 0x06006FA6 RID: 28582 RVA: 0x002D90C5 File Offset: 0x002D72C5
	public override void OnOpen()
	{
		base.OnOpen();
		this.txtSaveName.Text = "";
		this.TxtSaveNameOnOnChangeHandler(this, "", true);
		this.IsDirty = true;
	}

	// Token: 0x06006FA7 RID: 28583 RVA: 0x0007FB71 File Offset: 0x0007DD71
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x06006FA8 RID: 28584 RVA: 0x002D90F4 File Offset: 0x002D72F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		PathAbstractions.AbstractedLocation loadedPrefab = PrefabEditModeManager.Instance.LoadedPrefab;
		if (_bindingName == "is_save_new")
		{
			_value = (loadedPrefab.Type == PathAbstractions.EAbstractedLocationType.None).ToString();
			return true;
		}
		if (_bindingName == "current_prefab_name")
		{
			_value = ((loadedPrefab.Type != PathAbstractions.EAbstractedLocationType.None) ? loadedPrefab.Name : "");
			return true;
		}
		if (_bindingName == "request_name")
		{
			_value = this.nameRequired.ToString();
			return true;
		}
		if (_bindingName == "prefab_name_exists")
		{
			_value = this.nameExists.ToString();
			return true;
		}
		if (!(_bindingName == "prefab_name_invalid"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = this.nameInvalid.ToString();
		return true;
	}

	// Token: 0x06006FA9 RID: 28585 RVA: 0x002D91B4 File Offset: 0x002D73B4
	public static void Show(XUi _xui, Action<XUiC_SaveDirtyPrefab.ESelectedAction> _onCloseAction, XUiC_SaveDirtyPrefab.EMode _mode = XUiC_SaveDirtyPrefab.EMode.AskSaveIfDirty)
	{
		PathAbstractions.AbstractedLocation loadedPrefab = PrefabEditModeManager.Instance.LoadedPrefab;
		if (_mode != XUiC_SaveDirtyPrefab.EMode.AskSaveIfDirty)
		{
			if (_mode == XUiC_SaveDirtyPrefab.EMode.ForceSave)
			{
				if (loadedPrefab.Type != PathAbstractions.EAbstractedLocationType.None && Prefab.CanSaveIn(loadedPrefab))
				{
					if (PrefabEditModeManager.Instance.SaveVoxelPrefab())
					{
						GameManager.ShowTooltip(_xui.playerUI.entityPlayer, string.Format(Localization.Get("xuiPrefabsPrefabSaved", false), loadedPrefab.Name), false, false, 0f);
						if (_onCloseAction != null)
						{
							_onCloseAction(XUiC_SaveDirtyPrefab.ESelectedAction.Save);
							return;
						}
					}
					else
					{
						GameManager.ShowTooltip(_xui.playerUI.entityPlayer, Localization.Get("xuiPrefabsPrefabSavingError", false), false, false, 0f);
						if (_onCloseAction != null)
						{
							_onCloseAction(XUiC_SaveDirtyPrefab.ESelectedAction.Cancel);
						}
					}
					return;
				}
			}
		}
		else if (PrefabEditModeManager.Instance.VoxelPrefab == null || !PrefabEditModeManager.Instance.NeedsSaving)
		{
			if (_onCloseAction != null)
			{
				_onCloseAction(XUiC_SaveDirtyPrefab.ESelectedAction.DontSave);
			}
			return;
		}
		XUiC_SaveDirtyPrefab childByType = ((XUiWindowGroup)_xui.playerUI.windowManager.GetWindow(XUiC_SaveDirtyPrefab.ID)).Controller.GetChildByType<XUiC_SaveDirtyPrefab>();
		childByType.nameRequired = (loadedPrefab.Type == PathAbstractions.EAbstractedLocationType.None || !Prefab.CanSaveIn(loadedPrefab));
		childByType.onCloseAction = _onCloseAction;
		_xui.playerUI.windowManager.Open(XUiC_SaveDirtyPrefab.ID, true, true, true);
	}

	// Token: 0x040054B7 RID: 21687
	public static string ID = "";

	// Token: 0x040054B8 RID: 21688
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtSaveName;

	// Token: 0x040054B9 RID: 21689
	[PublicizedFrom(EAccessModifier.Private)]
	public bool nameRequired;

	// Token: 0x040054BA RID: 21690
	[PublicizedFrom(EAccessModifier.Private)]
	public bool nameInvalid;

	// Token: 0x040054BB RID: 21691
	[PublicizedFrom(EAccessModifier.Private)]
	public bool nameExists;

	// Token: 0x040054BC RID: 21692
	[PublicizedFrom(EAccessModifier.Private)]
	public Action<XUiC_SaveDirtyPrefab.ESelectedAction> onCloseAction;

	// Token: 0x02000DE9 RID: 3561
	public enum EMode
	{
		// Token: 0x040054BE RID: 21694
		AskSaveIfDirty,
		// Token: 0x040054BF RID: 21695
		ForceSave
	}

	// Token: 0x02000DEA RID: 3562
	public enum ESelectedAction
	{
		// Token: 0x040054C1 RID: 21697
		Save,
		// Token: 0x040054C2 RID: 21698
		Cancel,
		// Token: 0x040054C3 RID: 21699
		DontSave
	}
}
