using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GUI_2;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CB1 RID: 3249
[Preserve]
public class XUiC_EulaWindow : XUiController
{
	// Token: 0x0600646F RID: 25711 RVA: 0x0028B430 File Offset: 0x00289630
	public override void Init()
	{
		base.Init();
		XUiC_EulaWindow.ID = base.WindowGroup.ID;
		this.btnAccept = (XUiC_SimpleButton)base.GetChildById("btnAccept");
		this.btnDecline = (XUiC_SimpleButton)base.GetChildById("btnDecline");
		this.lblContent = (base.GetChildById("lblContent").ViewComponent as XUiV_Label);
		this.footerContainer = (base.GetChildById("footer").ViewComponent as XUiV_Rect);
		this.background = base.GetChildById("background");
		this.btnPageUp = base.GetChildById("btnPageUp");
		this.btnPageDown = base.GetChildById("btnPageDown");
		this.btnAccept.OnPressed += this.btnAccept_OnPressed;
		this.btnDecline.OnPressed += this.btnDecline_OnPressed;
		this.btnPageDown.OnPress += this.btnPageDown_OnPressed;
		this.btnPageUp.OnPress += this.btnPageUp_OnPressed;
		((XUiC_SimpleButton)base.GetChildById("btnDone")).OnPressed += this.btnDecline_OnPressed;
		TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Data/EULA/eula_{0}", Localization.language.ToLower()));
		if (textAsset != null)
		{
			this.LoadDefaultXML(textAsset.bytes);
			return;
		}
		Log.Error("Could not load default EULA text asset");
	}

	// Token: 0x06006470 RID: 25712 RVA: 0x0028B59F File Offset: 0x0028979F
	public static void Open(XUi _xui, bool _viewMode = false)
	{
		XUiC_EulaWindow.viewMode = _viewMode;
		_xui.playerUI.windowManager.Open(XUiC_EulaWindow.ID, true, true, true);
	}

	// Token: 0x06006471 RID: 25713 RVA: 0x0028B5C0 File Offset: 0x002897C0
	public override void OnOpen()
	{
		base.OnOpen();
		this.pageFormatted = false;
		this.currentPage = 0;
		this.SetVisibility(false);
		this.btnAccept.Enabled = false;
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		this.lblContent.Label.ResetAndUpdateAnchors();
		base.RefreshBindings(true);
		if (XUiC_EulaWindow.viewMode)
		{
			base.GetChildById("btnDone").SelectCursorElement(true, false);
		}
	}

	// Token: 0x06006472 RID: 25714 RVA: 0x0028B638 File Offset: 0x00289838
	[PublicizedFrom(EAccessModifier.Private)]
	public void Close()
	{
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		this.SetVisibility(false);
		base.xui.playerUI.windowManager.Close(XUiC_EulaWindow.ID);
		if (XUiC_EulaWindow.viewMode)
		{
			base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoSelect", XUiC_GamepadCalloutWindow.CalloutType.Menu);
			base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoBack", XUiC_GamepadCalloutWindow.CalloutType.Menu);
			base.xui.playerUI.windowManager.Open(XUiC_OptionsGeneral.ID, true, false, true);
			return;
		}
		XUiC_MainMenu.Open(base.xui);
	}

	// Token: 0x06006473 RID: 25715 RVA: 0x0028B6D8 File Offset: 0x002898D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void LoadDefaultXML(byte[] _data)
	{
		XmlFile xmlFile;
		try
		{
			xmlFile = new XmlFile(_data, true);
		}
		catch (Exception ex)
		{
			Log.Error("Failed loading default EULA XML: {0}", new object[]
			{
				ex.Message
			});
			return;
		}
		XElement root = xmlFile.XmlDoc.Root;
		if (root == null)
		{
			return;
		}
		this.defaultEulaVersion = int.Parse(root.GetAttribute("version").Trim());
		this.defaultEula = root.Value;
		if (this.defaultEulaVersion > GamePrefs.GetInt(EnumGamePrefs.EulaLatestVersion))
		{
			GamePrefs.Set(EnumGamePrefs.EulaLatestVersion, this.defaultEulaVersion);
		}
		Log.Out("Loaded default EULA");
	}

	// Token: 0x06006474 RID: 25716 RVA: 0x0028B784 File Offset: 0x00289984
	[PublicizedFrom(EAccessModifier.Private)]
	public void DisplayLocalEulaOrClose()
	{
		if (GamePrefs.GetInt(EnumGamePrefs.EulaVersionAccepted) < this.defaultEulaVersion || XUiC_EulaWindow.viewMode)
		{
			this.SetVisibility(true);
			this.FormatPages(this.defaultEula);
			this.ShowGamepadCallouts();
			return;
		}
		this.Close();
	}

	// Token: 0x06006475 RID: 25717 RVA: 0x0028B7C0 File Offset: 0x002899C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void FormatPages(string content)
	{
		content = content.Replace("\t", "  ");
		this.pages.Clear();
		string[] array = content.Split('\n', StringSplitOptions.None);
		string language = Localization.language;
		int num;
		if (language == "japanese" || language == "koreana" || language == "schinese" || language == "tchinese")
		{
			num = 1000;
		}
		else
		{
			num = 2000;
		}
		int i = 0;
		while (i < array.Length)
		{
			string text = array[i];
			if (string.IsNullOrWhiteSpace(text))
			{
				i++;
			}
			else
			{
				i++;
				while (text.Length < num && i < array.Length)
				{
					text += "\n\n";
					text += array[i];
					i++;
				}
				this.pages.Add(text);
			}
		}
		this.SetPage(0);
	}

	// Token: 0x06006476 RID: 25718 RVA: 0x0028B8A4 File Offset: 0x00289AA4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetPage(int page)
	{
		if (page < 0 || page >= this.pages.Count)
		{
			return;
		}
		this.currentPage = page;
		this.lblContent.Text = this.pages[page];
		this.UpdatePageButtonVisibility();
		if (!XUiC_EulaWindow.viewMode && this.currentPage == this.pages.Count - 1 && !this.btnAccept.Enabled)
		{
			base.xui.playerUI.CursorController.SetNavigationTarget(this.btnDecline.ViewComponent);
			this.btnAccept.Enabled = true;
		}
	}

	// Token: 0x06006477 RID: 25719 RVA: 0x0028B940 File Offset: 0x00289B40
	[PublicizedFrom(EAccessModifier.Private)]
	public void ShowGamepadCallouts()
	{
		base.xui.playerUI.windowManager.OpenIfNotOpen("CalloutGroup", false, false, true);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoSelect", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		if (XUiC_EulaWindow.viewMode)
		{
			base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoBack", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		}
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.RightStickUpDown, "igcoScroll", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.SetCalloutsEnabled(XUiC_GamepadCalloutWindow.CalloutType.Menu, true);
	}

	// Token: 0x06006478 RID: 25720 RVA: 0x0028B9CC File Offset: 0x00289BCC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (!this.pageFormatted)
		{
			if (GameManager.UpdatingRemoteResources || !GameManager.RemoteResourcesLoaded)
			{
				return;
			}
			this.pageFormatted = true;
			if (string.IsNullOrEmpty(XUiC_EulaWindow.retrievedEula))
			{
				this.DisplayLocalEulaOrClose();
			}
			else if (GameManager.HasAcceptedLatestEula() && !XUiC_EulaWindow.viewMode)
			{
				this.Close();
			}
			else
			{
				this.SetVisibility(true);
				this.FormatPages(XUiC_EulaWindow.retrievedEula);
				this.ShowGamepadCallouts();
			}
		}
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
		{
			float value = base.xui.playerUI.playerInput.GUIActions.scroll.Value;
			if (value == 0f)
			{
				return;
			}
			if (value > 0f)
			{
				this.btnPageUp_OnPressed(null, 0);
			}
			else if (value < 0f)
			{
				this.btnPageDown_OnPressed(null, 0);
			}
		}
		else
		{
			XUi.HandlePaging(base.xui, new Func<bool>(this.TryPageUp), new Func<bool>(this.TryPageDown), true);
		}
		if (XUiC_EulaWindow.viewMode && base.xui.playerUI.playerInput.PermanentActions.Cancel.WasReleased)
		{
			this.Close();
		}
	}

	// Token: 0x06006479 RID: 25721 RVA: 0x0028BAF4 File Offset: 0x00289CF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdatePageButtonVisibility()
	{
		bool flag;
		bool flag2;
		if (!this.background.ViewComponent.IsVisible || !this.pageFormatted || GameManager.UpdatingRemoteResources || !GameManager.RemoteResourcesLoaded || this.pages == null)
		{
			flag = false;
			flag2 = false;
		}
		else
		{
			flag = (this.currentPage > 0);
			flag2 = (this.currentPage < this.pages.Count - 1);
		}
		if (this.btnPageUp.ViewComponent.IsVisible != flag)
		{
			this.btnPageUp.ViewComponent.IsVisible = flag;
		}
		if (this.btnPageDown.ViewComponent.IsVisible != flag2)
		{
			this.btnPageDown.ViewComponent.IsVisible = flag2;
		}
	}

	// Token: 0x0600647A RID: 25722 RVA: 0x0028BB9F File Offset: 0x00289D9F
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnAccept_OnPressed(XUiController _sender, int _mouseButton)
	{
		GamePrefs.Set(EnumGamePrefs.EulaVersionAccepted, GamePrefs.GetInt(EnumGamePrefs.EulaLatestVersion));
		GamePrefs.Instance.Save();
		this.Close();
	}

	// Token: 0x0600647B RID: 25723 RVA: 0x0028BBC5 File Offset: 0x00289DC5
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnDecline_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.Close();
	}

	// Token: 0x0600647C RID: 25724 RVA: 0x0028BBCD File Offset: 0x00289DCD
	[PublicizedFrom(EAccessModifier.Protected)]
	public void PageUpAction()
	{
		this.btnPageUp_OnPressed(null, 0);
	}

	// Token: 0x0600647D RID: 25725 RVA: 0x0028BBD7 File Offset: 0x00289DD7
	[PublicizedFrom(EAccessModifier.Protected)]
	public void PageDownAction()
	{
		this.btnPageDown_OnPressed(null, 0);
	}

	// Token: 0x0600647E RID: 25726 RVA: 0x0028BBE4 File Offset: 0x00289DE4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryPageUp()
	{
		int num = this.currentPage - 1;
		if (num < 0)
		{
			return false;
		}
		this.SetPage(num);
		return true;
	}

	// Token: 0x0600647F RID: 25727 RVA: 0x0028BC08 File Offset: 0x00289E08
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryPageDown()
	{
		int num = this.currentPage + 1;
		if (num >= this.pages.Count)
		{
			return false;
		}
		this.SetPage(num);
		return true;
	}

	// Token: 0x06006480 RID: 25728 RVA: 0x0028BC36 File Offset: 0x00289E36
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnPageUp_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.TryPageUp();
	}

	// Token: 0x06006481 RID: 25729 RVA: 0x0028BC3F File Offset: 0x00289E3F
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnPageDown_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.TryPageDown();
	}

	// Token: 0x06006482 RID: 25730 RVA: 0x0028BC48 File Offset: 0x00289E48
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetVisibility(bool _visible)
	{
		this.background.ViewComponent.IsVisible = _visible;
		this.footerContainer.IsVisible = _visible;
		this.UpdatePageButtonVisibility();
	}

	// Token: 0x06006483 RID: 25731 RVA: 0x0028BC6D File Offset: 0x00289E6D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "viewmode")
		{
			_value = XUiC_EulaWindow.viewMode.ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x04004BC4 RID: 19396
	public static string ID;

	// Token: 0x04004BC5 RID: 19397
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnAccept;

	// Token: 0x04004BC6 RID: 19398
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDecline;

	// Token: 0x04004BC7 RID: 19399
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblContent;

	// Token: 0x04004BC8 RID: 19400
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Rect footerContainer;

	// Token: 0x04004BC9 RID: 19401
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController background;

	// Token: 0x04004BCA RID: 19402
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnPageUp;

	// Token: 0x04004BCB RID: 19403
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnPageDown;

	// Token: 0x04004BCC RID: 19404
	[PublicizedFrom(EAccessModifier.Private)]
	public string defaultEula;

	// Token: 0x04004BCD RID: 19405
	[PublicizedFrom(EAccessModifier.Private)]
	public int defaultEulaVersion = -1;

	// Token: 0x04004BCE RID: 19406
	public static string retrievedEula;

	// Token: 0x04004BCF RID: 19407
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> pages = new List<string>();

	// Token: 0x04004BD0 RID: 19408
	[PublicizedFrom(EAccessModifier.Private)]
	public bool pageFormatted;

	// Token: 0x04004BD1 RID: 19409
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentPage;

	// Token: 0x04004BD2 RID: 19410
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool viewMode;

	// Token: 0x04004BD3 RID: 19411
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cAlphanumericPageCharacterLimit = 2000;

	// Token: 0x04004BD4 RID: 19412
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cScriptPageCharacterLimit = 1000;
}
