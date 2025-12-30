using System;
using UnityEngine.Scripting;

// Token: 0x02000CCE RID: 3278
[Preserve]
public class XUiC_InteractionPrompt : XUiController
{
	// Token: 0x17000A5A RID: 2650
	// (get) Token: 0x06006579 RID: 25977 RVA: 0x00291F9E File Offset: 0x0029019E
	// (set) Token: 0x0600657A RID: 25978 RVA: 0x00291FA6 File Offset: 0x002901A6
	public string Text
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.text;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (value != this.text)
			{
				this.text = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x0600657B RID: 25979 RVA: 0x00291FC4 File Offset: 0x002901C4
	public override void Init()
	{
		base.Init();
		XUiC_InteractionPrompt.ID = base.WindowGroup.ID;
	}

	// Token: 0x0600657C RID: 25980 RVA: 0x0028C056 File Offset: 0x0028A256
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x0600657D RID: 25981 RVA: 0x00291FDC File Offset: 0x002901DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "text")
		{
			_value = this.text;
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x0600657E RID: 25982 RVA: 0x00292000 File Offset: 0x00290200
	public static void SetText(LocalPlayerUI _playerUi, string _text)
	{
		if (_playerUi != null && _playerUi.xui != null)
		{
			XUiController xuiController = _playerUi.xui.FindWindowGroupByName(XUiC_InteractionPrompt.ID);
			XUiC_InteractionPrompt xuiC_InteractionPrompt = (xuiController != null) ? xuiController.GetChildByType<XUiC_InteractionPrompt>() : null;
			if (xuiC_InteractionPrompt == null)
			{
				return;
			}
			xuiC_InteractionPrompt.Text = _text;
			if (string.IsNullOrEmpty(_text))
			{
				_playerUi.windowManager.Close(XUiC_InteractionPrompt.ID);
				return;
			}
			_playerUi.windowManager.Open(XUiC_InteractionPrompt.ID, false, false, false);
		}
	}

	// Token: 0x04004C9B RID: 19611
	public static string ID = "";

	// Token: 0x04004C9C RID: 19612
	[PublicizedFrom(EAccessModifier.Private)]
	public string text;
}
