using System;
using System.Collections.Generic;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000E8D RID: 3725
[Preserve]
public class XUiC_TwitchHowToWindow : XUiController
{
	// Token: 0x06007572 RID: 30066 RVA: 0x002FD888 File Offset: 0x002FBA88
	public override void Init()
	{
		base.Init();
		base.GetChildById("leftButton").OnPress += this.Left_OnPress;
		base.GetChildById("rightButton").OnPress += this.Right_OnPress;
		this.lblTipHeader = Localization.Get("TwitchInfo_TipHeader", false);
	}

	// Token: 0x06007573 RID: 30067 RVA: 0x002FD8E4 File Offset: 0x002FBAE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Left_OnPress(XUiController _sender, int _mouseButton)
	{
		this.tipIndex--;
		if (this.tipIndex == -1)
		{
			this.tipIndex = this.TipNames.Count - 1;
		}
		base.RefreshBindings(false);
	}

	// Token: 0x06007574 RID: 30068 RVA: 0x002FD917 File Offset: 0x002FBB17
	[PublicizedFrom(EAccessModifier.Private)]
	public void Right_OnPress(XUiController _sender, int _mouseButton)
	{
		this.tipIndex++;
		if (this.tipIndex == this.TipNames.Count)
		{
			this.tipIndex = 0;
		}
		base.RefreshBindings(false);
	}

	// Token: 0x06007575 RID: 30069 RVA: 0x002FD948 File Offset: 0x002FBB48
	public override void OnOpen()
	{
		base.OnOpen();
		this.TipNames = TwitchManager.Current.tipTitleList;
		this.TipText = TwitchManager.Current.tipDescriptionList;
		base.RefreshBindings(false);
	}

	// Token: 0x06007576 RID: 30070 RVA: 0x002FD978 File Offset: 0x002FBB78
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "tipheader")
		{
			value = this.lblTipHeader;
			return true;
		}
		if (bindingName == "tiptitle")
		{
			value = ((this.TipNames.Count > 0) ? this.TipNames[this.tipIndex] : "");
			return true;
		}
		if (!(bindingName == "tiptext"))
		{
			return false;
		}
		value = ((this.TipText.Count > 0) ? this.TipText[this.tipIndex] : "");
		return true;
	}

	// Token: 0x04005997 RID: 22935
	[PublicizedFrom(EAccessModifier.Private)]
	public int tipIndex;

	// Token: 0x04005998 RID: 22936
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> TipNames = new List<string>();

	// Token: 0x04005999 RID: 22937
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> TipText = new List<string>();

	// Token: 0x0400599A RID: 22938
	[PublicizedFrom(EAccessModifier.Private)]
	public string lblTipHeader;
}
