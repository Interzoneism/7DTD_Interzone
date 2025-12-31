using System;
using System.Collections.Generic;
using GUI_2;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D8E RID: 3470
[Preserve]
public class XUiC_PowerTriggerOptions : XUiController
{
	// Token: 0x17000AEE RID: 2798
	// (get) Token: 0x06006C9B RID: 27803 RVA: 0x002C60D8 File Offset: 0x002C42D8
	// (set) Token: 0x06006C9C RID: 27804 RVA: 0x002C60E0 File Offset: 0x002C42E0
	public TileEntityPoweredTrigger TileEntity
	{
		get
		{
			return this.tileEntity;
		}
		set
		{
			this.tileEntity = value;
		}
	}

	// Token: 0x17000AEF RID: 2799
	// (get) Token: 0x06006C9D RID: 27805 RVA: 0x002C60E9 File Offset: 0x002C42E9
	// (set) Token: 0x06006C9E RID: 27806 RVA: 0x002C60F1 File Offset: 0x002C42F1
	public XUiC_PowerTriggerWindowGroup Owner { get; [PublicizedFrom(EAccessModifier.Internal)] set; }

	// Token: 0x06006C9F RID: 27807 RVA: 0x002C60FC File Offset: 0x002C42FC
	public override void Init()
	{
		base.Init();
		this.optionSelector1 = (base.GetChildById("optionSelector1") as XUiC_OptionsSelector);
		this.optionSelector2 = (base.GetChildById("optionSelector2") as XUiC_OptionsSelector);
		this.pnlTargeting = base.GetChildById("pnlTargeting");
		this.btnOn = base.GetChildById("btnOn");
		this.btnOn_Background = (XUiV_Button)this.btnOn.GetChildById("clickable").ViewComponent;
		this.btnOn_Background.Controller.OnPress += this.btnOn_OnPress;
		this.btnTargetSelf = base.GetChildById("btnTargetSelf");
		this.btnTargetAllies = base.GetChildById("btnTargetAllies");
		this.btnTargetStrangers = base.GetChildById("btnTargetStrangers");
		this.btnTargetZombies = base.GetChildById("btnTargetZombies");
		if (this.btnTargetSelf != null)
		{
			this.btnTargetSelf.OnPress += this.btnTargetSelf_OnPress;
		}
		if (this.btnTargetAllies != null)
		{
			this.btnTargetAllies.OnPress += this.btnTargetAllies_OnPress;
		}
		if (this.btnTargetStrangers != null)
		{
			this.btnTargetStrangers.OnPress += this.btnTargetStrangers_OnPress;
		}
		if (this.btnTargetZombies != null)
		{
			this.btnTargetZombies.OnPress += this.btnTargetZombies_OnPress;
		}
		this.isDirty = true;
		this.startTimeText = Localization.Get("xuiStartTime", false);
		this.endTimeText = Localization.Get("xuiEndTime", false);
		string format = Localization.Get("goSecond", false);
		string format2 = Localization.Get("goSeconds", false);
		string format3 = Localization.Get("goMinute", false);
		string format4 = Localization.Get("goMinutes", false);
		this.delayStrings.Add(Localization.Get("xuiInstant", false));
		this.delayStrings.Add(string.Format(format, 1));
		this.delayStrings.Add(string.Format(format2, 2));
		this.delayStrings.Add(string.Format(format2, 3));
		this.delayStrings.Add(string.Format(format2, 4));
		this.delayStrings.Add(string.Format(format2, 5));
		this.durationStrings.Add(Localization.Get("xuiAlways", false));
		this.durationStrings.Add(Localization.Get("xuiTriggered", false));
		this.durationStrings.Add(string.Format(format, 1));
		this.durationStrings.Add(string.Format(format2, 2));
		this.durationStrings.Add(string.Format(format2, 3));
		this.durationStrings.Add(string.Format(format2, 4));
		this.durationStrings.Add(string.Format(format2, 5));
		this.durationStrings.Add(string.Format(format2, 6));
		this.durationStrings.Add(string.Format(format2, 7));
		this.durationStrings.Add(string.Format(format2, 8));
		this.durationStrings.Add(string.Format(format2, 9));
		this.durationStrings.Add(string.Format(format2, 10));
		this.durationStrings.Add(string.Format(format2, 15));
		this.durationStrings.Add(string.Format(format2, 30));
		this.durationStrings.Add(string.Format(format2, 45));
		this.durationStrings.Add(string.Format(format3, 1));
		this.durationStrings.Add(string.Format(format4, 5));
		this.durationStrings.Add(string.Format(format4, 10));
		this.durationStrings.Add(string.Format(format4, 30));
		this.durationStrings.Add(string.Format(format4, 60));
		this.btnTargetAllies.ViewComponent.NavDownTarget = (this.btnTargetSelf.ViewComponent.NavDownTarget = (this.btnTargetStrangers.ViewComponent.NavDownTarget = (this.btnTargetZombies.ViewComponent.NavDownTarget = this.optionSelector1.ViewComponent)));
	}

	// Token: 0x06006CA0 RID: 27808 RVA: 0x002C6568 File Offset: 0x002C4768
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnTargetSelf_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiV_Button xuiV_Button = this.btnTargetSelf.ViewComponent as XUiV_Button;
		xuiV_Button.Selected = !xuiV_Button.Selected;
		if (xuiV_Button.Selected)
		{
			this.TileEntity.TargetType |= 1;
			return;
		}
		this.TileEntity.TargetType &= -2;
	}

	// Token: 0x06006CA1 RID: 27809 RVA: 0x002C65C4 File Offset: 0x002C47C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnTargetAllies_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiV_Button xuiV_Button = this.btnTargetAllies.ViewComponent as XUiV_Button;
		xuiV_Button.Selected = !xuiV_Button.Selected;
		if (xuiV_Button.Selected)
		{
			this.TileEntity.TargetType |= 2;
			return;
		}
		this.TileEntity.TargetType &= -3;
	}

	// Token: 0x06006CA2 RID: 27810 RVA: 0x002C6620 File Offset: 0x002C4820
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnTargetStrangers_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiV_Button xuiV_Button = this.btnTargetStrangers.ViewComponent as XUiV_Button;
		xuiV_Button.Selected = !xuiV_Button.Selected;
		if (xuiV_Button.Selected)
		{
			this.TileEntity.TargetType |= 4;
			return;
		}
		this.TileEntity.TargetType &= -5;
	}

	// Token: 0x06006CA3 RID: 27811 RVA: 0x002C667C File Offset: 0x002C487C
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnTargetZombies_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiV_Button xuiV_Button = this.btnTargetZombies.ViewComponent as XUiV_Button;
		xuiV_Button.Selected = !xuiV_Button.Selected;
		if (xuiV_Button.Selected)
		{
			this.TileEntity.TargetType |= 8;
			return;
		}
		this.TileEntity.TargetType &= -9;
	}

	// Token: 0x06006CA4 RID: 27812 RVA: 0x002C66D7 File Offset: 0x002C48D7
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnOn_OnPress(XUiController _sender, int _mouseButton)
	{
		this.TileEntity.ResetTrigger();
	}

	// Token: 0x06006CA5 RID: 27813 RVA: 0x002C66E4 File Offset: 0x002C48E4
	public override void Update(float _dt)
	{
		if (GameManager.Instance == null && GameManager.Instance.World == null)
		{
			return;
		}
		if (this.tileEntity == null)
		{
			return;
		}
		if (base.xui.playerUI.playerInput.GUIActions.WindowPagingRight.WasPressed && PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			this.btnOn_OnPress(this, 0);
		}
		base.Update(_dt);
	}

	// Token: 0x06006CA6 RID: 27814 RVA: 0x002C6758 File Offset: 0x002C4958
	public override void OnOpen()
	{
		base.OnOpen();
		this.tileEntity.SetUserAccessing(true);
		this.SetupSliders();
		base.RefreshBindings(false);
		this.tileEntity.SetModified();
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.RightBumper, "igcoPoweredTriggerReset", XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts, 0f);
	}

	// Token: 0x06006CA7 RID: 27815 RVA: 0x002C67BC File Offset: 0x002C49BC
	public override void OnClose()
	{
		GameManager instance = GameManager.Instance;
		Vector3i blockPos = this.tileEntity.ToWorldPos();
		if (!XUiC_CameraWindow.hackyIsOpeningMaximizedWindow)
		{
			this.tileEntity.SetUserAccessing(false);
			instance.TEUnlockServer(this.tileEntity.GetClrIdx(), blockPos, this.tileEntity.entityId, true);
			this.tileEntity.SetModified();
		}
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuShortcuts);
		base.OnClose();
	}

	// Token: 0x06006CA8 RID: 27816 RVA: 0x002C6830 File Offset: 0x002C4A30
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupSliders()
	{
		if (this.pnlTargeting != null)
		{
			this.pnlTargeting.ViewComponent.IsVisible = (this.tileEntity.TriggerType == PowerTrigger.TriggerTypes.Motion);
		}
		switch (this.tileEntity.TriggerType)
		{
		case PowerTrigger.TriggerTypes.PressurePlate:
		case PowerTrigger.TriggerTypes.Motion:
		case PowerTrigger.TriggerTypes.TripWire:
			this.optionSelector1.Title = Localization.Get("xuiPowerDelay", false);
			this.optionSelector1.ClearItems();
			for (int i = 0; i < this.delayStrings.Count; i++)
			{
				this.optionSelector1.AddItem(this.delayStrings[i]);
			}
			this.optionSelector2.Title = Localization.Get("xuiPowerDuration", false);
			this.optionSelector2.ClearItems();
			for (int j = 0; j < this.durationStrings.Count; j++)
			{
				this.optionSelector2.AddItem(this.durationStrings[j]);
			}
			this.optionSelector1.OnSelectionChanged -= this.OptionSelector1_OnSelectionChanged;
			this.optionSelector2.OnSelectionChanged -= this.OptionSelector2_OnSelectionChanged;
			this.optionSelector1.SetIndex((int)this.TileEntity.Property1);
			this.optionSelector2.SetIndex((int)this.TileEntity.Property2);
			this.optionSelector2.OnSelectionChanged += this.OptionSelector2_OnSelectionChanged;
			this.optionSelector1.OnSelectionChanged += this.OptionSelector1_OnSelectionChanged;
			if (this.btnOn != null)
			{
				this.btnOn.ViewComponent.IsVisible = true;
			}
			if (this.pnlTargeting != null)
			{
				this.pnlTargeting.ViewComponent.IsVisible = (this.tileEntity.TriggerType == PowerTrigger.TriggerTypes.Motion);
				if (this.TileEntity.TriggerType == PowerTrigger.TriggerTypes.Motion)
				{
					if (this.btnTargetSelf != null)
					{
						this.btnTargetSelf.OnPress -= this.btnTargetSelf_OnPress;
						((XUiV_Button)this.btnTargetSelf.ViewComponent).Selected = this.TileEntity.TargetSelf;
						this.btnTargetSelf.OnPress += this.btnTargetSelf_OnPress;
					}
					if (this.btnTargetAllies != null)
					{
						this.btnTargetAllies.OnPress -= this.btnTargetAllies_OnPress;
						((XUiV_Button)this.btnTargetAllies.ViewComponent).Selected = this.TileEntity.TargetAllies;
						this.btnTargetAllies.OnPress += this.btnTargetAllies_OnPress;
					}
					if (this.btnTargetStrangers != null)
					{
						this.btnTargetStrangers.OnPress -= this.btnTargetStrangers_OnPress;
						((XUiV_Button)this.btnTargetStrangers.ViewComponent).Selected = this.TileEntity.TargetStrangers;
						this.btnTargetStrangers.OnPress += this.btnTargetStrangers_OnPress;
					}
					if (this.btnTargetZombies != null)
					{
						this.btnTargetZombies.OnPress -= this.btnTargetZombies_OnPress;
						((XUiV_Button)this.btnTargetZombies.ViewComponent).Selected = this.TileEntity.TargetZombies;
						this.btnTargetZombies.OnPress += this.btnTargetZombies_OnPress;
					}
				}
			}
			break;
		case PowerTrigger.TriggerTypes.TimerRelay:
			this.optionSelector1.Title = this.startTimeText;
			this.optionSelector1.ClearItems();
			for (int k = 0; k < 48; k++)
			{
				int num = k / 2;
				bool flag = k % 2 == 1;
				this.optionSelector1.AddItem(num.ToString("00") + (flag ? ":30" : ":00"));
			}
			this.optionSelector1.OnSelectionChanged -= this.OptionSelector1_OnSelectionChanged;
			this.optionSelector1.OnSelectionChanged += this.OptionSelector1_OnSelectionChanged;
			this.optionSelector2.Title = this.endTimeText;
			this.optionSelector2.ClearItems();
			for (int l = 0; l < 48; l++)
			{
				int num2 = l / 2;
				bool flag2 = l % 2 == 1;
				this.optionSelector2.AddItem(num2.ToString("00") + (flag2 ? ":30" : ":00"));
			}
			this.optionSelector2.OnSelectionChanged -= this.OptionSelector2_OnSelectionChanged;
			this.optionSelector2.OnSelectionChanged += this.OptionSelector2_OnSelectionChanged;
			this.optionSelector1.SetIndex((int)this.TileEntity.Property1);
			this.optionSelector2.SetIndex((int)this.TileEntity.Property2);
			if (this.btnOn != null)
			{
				this.btnOn.ViewComponent.IsVisible = false;
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06006CA9 RID: 27817 RVA: 0x002C6CD1 File Offset: 0x002C4ED1
	[PublicizedFrom(EAccessModifier.Private)]
	public void OptionSelector1_OnSelectionChanged(XUiController _sender, int newSelectedIndex)
	{
		this.TileEntity.Property1 = (byte)newSelectedIndex;
		this.TileEntity.ResetTrigger();
	}

	// Token: 0x06006CAA RID: 27818 RVA: 0x002C6CEB File Offset: 0x002C4EEB
	[PublicizedFrom(EAccessModifier.Private)]
	public void OptionSelector2_OnSelectionChanged(XUiController _sender, int newSelectedIndex)
	{
		this.TileEntity.Property2 = (byte)newSelectedIndex;
		this.TileEntity.ResetTrigger();
	}

	// Token: 0x040052A0 RID: 21152
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController windowIcon;

	// Token: 0x040052A1 RID: 21153
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnOn;

	// Token: 0x040052A2 RID: 21154
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnOn_Background;

	// Token: 0x040052A3 RID: 21155
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblOnOff;

	// Token: 0x040052A4 RID: 21156
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite sprOnOff;

	// Token: 0x040052A5 RID: 21157
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController pnlTargeting;

	// Token: 0x040052A6 RID: 21158
	[PublicizedFrom(EAccessModifier.Private)]
	public string startTimeText;

	// Token: 0x040052A7 RID: 21159
	[PublicizedFrom(EAccessModifier.Private)]
	public string endTimeText;

	// Token: 0x040052A8 RID: 21160
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 onColor = new Color32(250, byte.MaxValue, 163, byte.MaxValue);

	// Token: 0x040052A9 RID: 21161
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 offColor = Color.white;

	// Token: 0x040052AA RID: 21162
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnTargetSelf;

	// Token: 0x040052AB RID: 21163
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnTargetAllies;

	// Token: 0x040052AC RID: 21164
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnTargetStrangers;

	// Token: 0x040052AD RID: 21165
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController btnTargetZombies;

	// Token: 0x040052AE RID: 21166
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityPoweredTrigger tileEntity;

	// Token: 0x040052B0 RID: 21168
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastOn;

	// Token: 0x040052B1 RID: 21169
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x040052B2 RID: 21170
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_OptionsSelector optionSelector1;

	// Token: 0x040052B3 RID: 21171
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_OptionsSelector optionSelector2;

	// Token: 0x040052B4 RID: 21172
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_OptionsSelector optionSelector3;

	// Token: 0x040052B5 RID: 21173
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> delayStrings = new List<string>();

	// Token: 0x040052B6 RID: 21174
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> durationStrings = new List<string>();

	// Token: 0x040052B7 RID: 21175
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateTime;
}
