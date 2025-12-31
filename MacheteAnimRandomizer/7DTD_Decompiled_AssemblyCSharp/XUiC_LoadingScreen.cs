using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D03 RID: 3331
[Preserve]
public class XUiC_LoadingScreen : XUiController
{
	// Token: 0x0600679B RID: 26523 RVA: 0x0029FD30 File Offset: 0x0029DF30
	public override void Init()
	{
		base.Init();
		XUiC_LoadingScreen.ID = base.WindowGroup.ID;
		XUiController childById = base.GetChildById("loading_image");
		if (childById != null)
		{
			this.backgroundTextureView = (childById.ViewComponent as XUiV_Texture);
		}
		base.GetChildById("pnlBlack").ViewComponent.IsSnappable = false;
	}

	// Token: 0x0600679C RID: 26524 RVA: 0x0029FD8C File Offset: 0x0029DF8C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.showTips && !XUiC_VideoPlayer.IsVideoPlaying)
		{
			if (base.xui.playerUI.playerInput.PermanentActions.PageTipsForward.WasPressed)
			{
				this.cycle(1);
			}
			else if (base.xui.playerUI.playerInput.PermanentActions.PageTipsBack.WasPressed)
			{
				this.cycle(-1);
			}
		}
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x0600679D RID: 26525 RVA: 0x0029FE18 File Offset: 0x0029E018
	[PublicizedFrom(EAccessModifier.Private)]
	public void cycle(int _increment)
	{
		this.currentTipIndex += _increment;
		if (this.currentTipIndex >= XUiC_LoadingScreen.tips.Count)
		{
			this.currentTipIndex = 0;
		}
		else if (this.currentTipIndex < 0)
		{
			this.currentTipIndex = XUiC_LoadingScreen.tips.Count - 1;
		}
		if (this.browseSound != null)
		{
			Manager.PlayXUiSound(this.browseSound, 1f);
		}
		this.IsDirty = true;
	}

	// Token: 0x0600679E RID: 26526 RVA: 0x0029FE90 File Offset: 0x0029E090
	public override void OnOpen()
	{
		base.OnOpen();
		((XUiV_Window)base.ViewComponent).Panel.alpha = 1f;
		if (XUiC_LoadingScreen.backgrounds.Count > 0)
		{
			UnityEngine.Random.InitState(Time.frameCount);
			this.currentBackground = XUiC_LoadingScreen.backgrounds[UnityEngine.Random.Range(0, XUiC_LoadingScreen.backgrounds.Count)];
		}
		this.showTips = true;
		this.currentTipIndex = GamePrefs.GetInt(EnumGamePrefs.LastLoadingTipRead) + 1;
		if (this.currentTipIndex >= XUiC_LoadingScreen.tips.Count)
		{
			this.currentTipIndex = 0;
		}
		this.currentTipIndex = Mathf.Clamp(this.currentTipIndex, 0, XUiC_LoadingScreen.tips.Count - 1);
		base.RefreshBindings(true);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
	}

	// Token: 0x0600679F RID: 26527 RVA: 0x0029FF5C File Offset: 0x0029E15C
	public override void OnClose()
	{
		base.OnClose();
		GamePrefs.Set(EnumGamePrefs.LastLoadingTipRead, this.currentTipIndex);
		XUiV_Texture xuiV_Texture = this.backgroundTextureView;
		if (xuiV_Texture != null)
		{
			xuiV_Texture.UnloadTexture();
		}
		base.xui.playerUI.CursorController.SetCursorHidden(false);
	}

	// Token: 0x060067A0 RID: 26528 RVA: 0x0029FF9B File Offset: 0x0029E19B
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "browse_sound")
		{
			base.xui.LoadData<AudioClip>(_value, delegate(AudioClip _o)
			{
				this.browseSound = _o;
			});
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x060067A1 RID: 26529 RVA: 0x0029FFD0 File Offset: 0x0029E1D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "background_texture")
		{
			_value = this.currentBackground;
			return true;
		}
		if (_bindingName == "index")
		{
			_value = (this.currentTipIndex + 1).ToString();
			return true;
		}
		if (_bindingName == "count")
		{
			_value = XUiC_LoadingScreen.tips.Count.ToString();
			return true;
		}
		if (_bindingName == "show_tips")
		{
			_value = this.showTips.ToString();
			return true;
		}
		if (_bindingName == "title")
		{
			_value = ((this.currentTipIndex < 0) ? "" : Localization.Get(XUiC_LoadingScreen.tips[this.currentTipIndex] + "_title", false));
			return true;
		}
		if (!(_bindingName == "text"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = ((this.currentTipIndex < 0) ? "" : Localization.Get(XUiC_LoadingScreen.tips[this.currentTipIndex], false));
		return true;
	}

	// Token: 0x060067A2 RID: 26530 RVA: 0x002A00D9 File Offset: 0x0029E2D9
	public void SetTipsVisible(bool visible)
	{
		if (this.showTips == visible)
		{
			return;
		}
		this.showTips = visible;
		this.IsDirty = true;
	}

	// Token: 0x060067A3 RID: 26531 RVA: 0x002A00F3 File Offset: 0x0029E2F3
	public static IEnumerator LoadXml(XmlFile _xmlFile)
	{
		XContainer root = _xmlFile.XmlDoc.Root;
		XUiC_LoadingScreen.backgrounds.Clear();
		XUiC_LoadingScreen.tips.Clear();
		using (IEnumerator<XElement> enumerator = root.Elements().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				XElement xelement = enumerator.Current;
				if (xelement.Name == "backgrounds")
				{
					using (IEnumerator<XElement> enumerator2 = xelement.Elements("tex").GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							XElement element = enumerator2.Current;
							if (!element.HasAttribute("file"))
							{
								Log.Warning("Backgrounds entry is missing file attribute, skipping.");
							}
							else
							{
								XUiC_LoadingScreen.backgrounds.Add(element.GetAttribute("file"));
							}
						}
						continue;
					}
				}
				if (xelement.Name == "tips")
				{
					foreach (XElement element2 in xelement.Elements("tip"))
					{
						if (!element2.HasAttribute("key"))
						{
							Log.Warning("Loading tips entry is missing file attribute, skipping.");
						}
						else
						{
							XUiC_LoadingScreen.tips.Add(element2.GetAttribute("key"));
						}
					}
				}
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x04004E25 RID: 20005
	public static string ID = "";

	// Token: 0x04004E26 RID: 20006
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentTipIndex = -1;

	// Token: 0x04004E27 RID: 20007
	[PublicizedFrom(EAccessModifier.Private)]
	public string currentBackground = "";

	// Token: 0x04004E28 RID: 20008
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showTips = true;

	// Token: 0x04004E29 RID: 20009
	[PublicizedFrom(EAccessModifier.Private)]
	public AudioClip browseSound;

	// Token: 0x04004E2A RID: 20010
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Texture backgroundTextureView;

	// Token: 0x04004E2B RID: 20011
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<string> backgrounds = new List<string>();

	// Token: 0x04004E2C RID: 20012
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<string> tips = new List<string>();
}
