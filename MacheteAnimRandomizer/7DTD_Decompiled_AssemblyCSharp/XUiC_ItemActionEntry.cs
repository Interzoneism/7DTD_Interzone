using System;
using Audio;
using GUI_2;
using InControl;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000BFA RID: 3066
[UnityEngine.Scripting.Preserve]
public class XUiC_ItemActionEntry : XUiController
{
	// Token: 0x170009B1 RID: 2481
	// (get) Token: 0x06005DD0 RID: 24016 RVA: 0x002625A9 File Offset: 0x002607A9
	// (set) Token: 0x06005DD1 RID: 24017 RVA: 0x002625B4 File Offset: 0x002607B4
	public BaseItemActionEntry ItemActionEntry
	{
		get
		{
			return this.itemActionEntry;
		}
		set
		{
			if (this.itemActionEntry != null)
			{
				this.itemActionEntry.ParentItem = null;
			}
			this.itemActionEntry = value;
			this.background.Enabled = (value != null);
			if (this.itemActionEntry != null)
			{
				PlayerAction playerAction;
				switch (this.itemActionEntry.ShortCut)
				{
				case BaseItemActionEntry.GamepadShortCut.DPadUp:
					playerAction = base.xui.playerUI.playerInput.GUIActions.DPad_Up;
					break;
				case BaseItemActionEntry.GamepadShortCut.DPadLeft:
					playerAction = base.xui.playerUI.playerInput.GUIActions.DPad_Left;
					break;
				case BaseItemActionEntry.GamepadShortCut.DPadRight:
					playerAction = base.xui.playerUI.playerInput.GUIActions.DPad_Right;
					break;
				case BaseItemActionEntry.GamepadShortCut.DPadDown:
					playerAction = base.xui.playerUI.playerInput.GUIActions.DPad_Down;
					break;
				default:
					playerAction = null;
					break;
				}
				PlayerAction action = playerAction;
				this.gamepadIcon.SpriteName = UIUtils.GetSpriteName(UIUtils.GetButtonIconForAction(action));
				this.keyboardButton.Text = action.GetBindingString(false, PlayerInputManager.InputStyle.Undefined, XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.KeyboardWithAngleBrackets, false, null);
				this.itemActionEntry.ParentItem = this;
				this.itemActionEntry.RefreshEnabled();
			}
			this.background.IsNavigatable = (this.itemActionEntry != null);
			this.UpdateBindingsVisibility();
			base.RefreshBindings(false);
		}
	}

	// Token: 0x170009B2 RID: 2482
	// (get) Token: 0x06005DD2 RID: 24018 RVA: 0x002626F7 File Offset: 0x002608F7
	public XUiController Background
	{
		get
		{
			return this.background.Controller;
		}
	}

	// Token: 0x170009B3 RID: 2483
	// (get) Token: 0x06005DD3 RID: 24019 RVA: 0x00262704 File Offset: 0x00260904
	public XUiV_GamepadIcon GamepadIcon
	{
		get
		{
			return this.gamepadIcon;
		}
	}

	// Token: 0x06005DD4 RID: 24020 RVA: 0x0026270C File Offset: 0x0026090C
	public override void Init()
	{
		base.Init();
		this.lblName = (base.GetChildById("name").ViewComponent as XUiV_Label);
		this.icoIcon = (base.GetChildById("icon").ViewComponent as XUiV_Sprite);
		this.background = (base.GetChildById("background").ViewComponent as XUiV_Sprite);
		this.gamepadIcon = (base.GetChildById("gamepadIcon").ViewComponent as XUiV_GamepadIcon);
		this.keyboardButton = (base.GetChildById("keyboardButton").ViewComponent as XUiV_Label);
		this.background.Controller.OnPress += this.OnPressAction;
		this.background.Controller.OnHover += this.OnHover;
		this.isDirty = true;
		base.RegisterForInputStyleChanges();
	}

	// Token: 0x06005DD5 RID: 24021 RVA: 0x002627EC File Offset: 0x002609EC
	[PublicizedFrom(EAccessModifier.Private)]
	public new void OnHover(XUiController _sender, bool _isOver)
	{
		XUiV_Sprite xuiV_Sprite = (XUiV_Sprite)_sender.ViewComponent;
		this.isOver = _isOver;
		if (this.itemActionEntry == null)
		{
			xuiV_Sprite.Color = this.defaultBackgroundColor;
			xuiV_Sprite.SpriteName = "menu_empty";
			return;
		}
		if (xuiV_Sprite != null)
		{
			if (_isOver)
			{
				xuiV_Sprite.Color = Color.white;
				xuiV_Sprite.SpriteName = "ui_game_select_row";
				return;
			}
			xuiV_Sprite.Color = this.defaultBackgroundColor;
			xuiV_Sprite.SpriteName = "menu_empty";
		}
	}

	// Token: 0x06005DD6 RID: 24022 RVA: 0x0026286C File Offset: 0x00260A6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPressAction(XUiController _sender, int _mouseButton)
	{
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (this.itemActionEntry != null)
		{
			if (this.itemActionEntry.Enabled)
			{
				Manager.PlayInsidePlayerHead(this.itemActionEntry.SoundName, -1, 0f, false, false);
				this.itemActionEntry.OnActivated();
			}
			else
			{
				Manager.PlayInsidePlayerHead(this.itemActionEntry.DisabledSound, -1, 0f, false, false);
				this.itemActionEntry.OnDisabledActivate();
			}
			this.background.Color = this.defaultBackgroundColor;
			this.background.SpriteName = "menu_empty";
			this.wasPressed = true;
		}
	}

	// Token: 0x06005DD7 RID: 24023 RVA: 0x00262918 File Offset: 0x00260B18
	public override void Update(float _dt)
	{
		if (this.isOver && UICamera.hoveredObject != this.background.UiTransform.gameObject)
		{
			this.background.Color = this.defaultBackgroundColor;
			this.background.SpriteName = "menu_empty";
			this.isOver = false;
		}
		if (this.isOver && this.wasPressed && this.itemActionEntry != null)
		{
			this.background.Color = Color.white;
			this.background.SpriteName = "ui_game_select_row";
			this.wasPressed = false;
		}
		if (this.isDirty)
		{
			base.RefreshBindings(false);
			this.isDirty = false;
		}
		base.RefreshBindings(false);
		base.Update(_dt);
	}

	// Token: 0x06005DD8 RID: 24024 RVA: 0x002629D9 File Offset: 0x00260BD9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		base.InputStyleChanged(_oldStyle, _newStyle);
		this.UpdateBindingsVisibility();
	}

	// Token: 0x06005DD9 RID: 24025 RVA: 0x002629EC File Offset: 0x00260BEC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "actionicon")
		{
			value = ((this.itemActionEntry != null) ? this.itemActionEntry.IconName : "");
			return true;
		}
		if (bindingName == "actionname")
		{
			value = ((this.itemActionEntry != null) ? this.itemActionEntry.ActionName : "");
			return true;
		}
		if (bindingName == "statuscolor")
		{
			value = "255,255,255,255";
			if (this.itemActionEntry != null)
			{
				Color32 v = this.itemActionEntry.Enabled ? this.defaultFontColor : this.disabledFontColor;
				value = this.statuscolorFormatter.Format(v);
			}
			return true;
		}
		if (!(bindingName == "inspectheld"))
		{
			return false;
		}
		value = ((this.itemActionEntry != null && base.xui.playerUI.playerInput.GUIActions.Inspect.IsPressed) ? "true" : "false");
		return true;
	}

	// Token: 0x06005DDA RID: 24026 RVA: 0x00262AE3 File Offset: 0x00260CE3
	public void StartTimedAction(float time)
	{
		GameManager.Instance.gameObject.AddComponent<XUiC_ItemActionEntry.TimedAction>().InitiateTimer(this.itemActionEntry, time);
	}

	// Token: 0x06005DDB RID: 24027 RVA: 0x00262B00 File Offset: 0x00260D00
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (!(name == "default_font_color"))
			{
				if (!(name == "disabled_font_color"))
				{
					if (!(name == "default_background_color"))
					{
						return false;
					}
					this.defaultBackgroundColor = StringParsers.ParseColor32(value);
				}
				else
				{
					this.disabledFontColor = StringParsers.ParseColor32(value);
				}
			}
			else
			{
				this.defaultFontColor = StringParsers.ParseColor32(value);
			}
			return true;
		}
		return flag;
	}

	// Token: 0x06005DDC RID: 24028 RVA: 0x00262B84 File Offset: 0x00260D84
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateBindingsVisibility()
	{
		bool flag = base.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard;
		bool flag2 = this.itemActionEntry != null && this.itemActionEntry.ShortCut != BaseItemActionEntry.GamepadShortCut.None;
		this.gamepadIcon.IsVisible = (!flag && flag2);
		this.keyboardButton.IsVisible = (flag && flag2);
	}

	// Token: 0x06005DDD RID: 24029 RVA: 0x00262BD7 File Offset: 0x00260DD7
	public void MarkDirty()
	{
		this.isDirty = true;
	}

	// Token: 0x040046FA RID: 18170
	[PublicizedFrom(EAccessModifier.Protected)]
	public BaseItemActionEntry itemActionEntry;

	// Token: 0x040046FB RID: 18171
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isDirty;

	// Token: 0x040046FC RID: 18172
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Label lblName;

	// Token: 0x040046FD RID: 18173
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Sprite icoIcon;

	// Token: 0x040046FE RID: 18174
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Sprite background;

	// Token: 0x040046FF RID: 18175
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_GamepadIcon gamepadIcon;

	// Token: 0x04004700 RID: 18176
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Label keyboardButton;

	// Token: 0x04004701 RID: 18177
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 defaultBackgroundColor = Color.gray;

	// Token: 0x04004702 RID: 18178
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 disabledFontColor = Color.gray;

	// Token: 0x04004703 RID: 18179
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 defaultFontColor = Color.white;

	// Token: 0x04004704 RID: 18180
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isOver;

	// Token: 0x04004705 RID: 18181
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor statuscolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004706 RID: 18182
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasPressed;

	// Token: 0x02000BFB RID: 3067
	public class TimedAction : MonoBehaviour
	{
		// Token: 0x06005DDF RID: 24031 RVA: 0x00262C2E File Offset: 0x00260E2E
		public void InitiateTimer(BaseItemActionEntry itemActionEntry, float _amount)
		{
			this.itemActionEntry = itemActionEntry;
			this.waitTime = Time.realtimeSinceStartup + _amount;
		}

		// Token: 0x06005DE0 RID: 24032 RVA: 0x00262C44 File Offset: 0x00260E44
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			if (this.waitTime != 0f && Time.realtimeSinceStartup >= this.waitTime)
			{
				this.waitTime = 0f;
				this.itemActionEntry.OnTimerCompleted();
				UnityEngine.Object.Destroy(this);
			}
		}

		// Token: 0x04004707 RID: 18183
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public BaseItemActionEntry itemActionEntry;

		// Token: 0x04004708 RID: 18184
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public float waitTime;
	}
}
