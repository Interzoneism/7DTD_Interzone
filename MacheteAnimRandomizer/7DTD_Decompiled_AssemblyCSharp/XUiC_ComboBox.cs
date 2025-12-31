using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using InControl;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C4D RID: 3149
[UnityEngine.Scripting.Preserve]
public abstract class XUiC_ComboBox<TValue> : XUiController
{
	// Token: 0x14000098 RID: 152
	// (add) Token: 0x060060B6 RID: 24758 RVA: 0x00274BA8 File Offset: 0x00272DA8
	// (remove) Token: 0x060060B7 RID: 24759 RVA: 0x00274BE0 File Offset: 0x00272DE0
	public event XUiC_ComboBox<TValue>.XUiEvent_ValueChanged OnValueChanged;

	// Token: 0x14000099 RID: 153
	// (add) Token: 0x060060B8 RID: 24760 RVA: 0x00274C18 File Offset: 0x00272E18
	// (remove) Token: 0x060060B9 RID: 24761 RVA: 0x00274C50 File Offset: 0x00272E50
	public event XUiC_ComboBox<TValue>.XUiEvent_GenericValueChanged OnValueChangedGeneric;

	// Token: 0x1400009A RID: 154
	// (add) Token: 0x060060BA RID: 24762 RVA: 0x00274C88 File Offset: 0x00272E88
	// (remove) Token: 0x060060BB RID: 24763 RVA: 0x00274CC0 File Offset: 0x00272EC0
	public event XUiC_ComboBox<TValue>.XUiEvent_HoveredStateChanged OnHoveredStateChanged;

	// Token: 0x170009F0 RID: 2544
	// (get) Token: 0x060060BC RID: 24764 RVA: 0x00274CF5 File Offset: 0x00272EF5
	// (set) Token: 0x060060BD RID: 24765 RVA: 0x00274CFD File Offset: 0x00272EFD
	public bool Enabled
	{
		get
		{
			return this.enabled;
		}
		set
		{
			if (value != this.enabled)
			{
				this.enabled = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x170009F1 RID: 2545
	// (get) Token: 0x060060BE RID: 24766 RVA: 0x00274D16 File Offset: 0x00272F16
	// (set) Token: 0x060060BF RID: 24767 RVA: 0x00274D1E File Offset: 0x00272F1E
	public Color TextColor
	{
		get
		{
			return this.colorEnabled;
		}
		set
		{
			if (this.colorEnabled != value)
			{
				this.colorEnabled = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x170009F2 RID: 2546
	// (get) Token: 0x060060C0 RID: 24768 RVA: 0x00274D3C File Offset: 0x00272F3C
	// (set) Token: 0x060060C1 RID: 24769 RVA: 0x00274D44 File Offset: 0x00272F44
	public string ValueText
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.valueText;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			if (this.valueText != value)
			{
				this.valueText = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x170009F3 RID: 2547
	// (get) Token: 0x060060C2 RID: 24770 RVA: 0x00274D62 File Offset: 0x00272F62
	// (set) Token: 0x060060C3 RID: 24771 RVA: 0x00274D6A File Offset: 0x00272F6A
	public int SegmentedFillCount
	{
		get
		{
			return this.segmentedFillCount;
		}
		set
		{
			if (value == this.segmentedFillCount)
			{
				return;
			}
			this.segmentedFillCount = value;
			this.UpdateSegmentedFillPositions();
		}
	}

	// Token: 0x170009F4 RID: 2548
	// (get) Token: 0x060060C4 RID: 24772
	// (set) Token: 0x060060C5 RID: 24773
	public abstract TValue Value { get; set; }

	// Token: 0x170009F5 RID: 2549
	// (get) Token: 0x060060C6 RID: 24774
	public abstract int IndexElementCount { [PublicizedFrom(EAccessModifier.Protected)] get; }

	// Token: 0x170009F6 RID: 2550
	// (get) Token: 0x060060C7 RID: 24775
	public abstract int IndexMarkerIndex { [PublicizedFrom(EAccessModifier.Protected)] get; }

	// Token: 0x170009F7 RID: 2551
	// (get) Token: 0x060060C8 RID: 24776 RVA: 0x00274D83 File Offset: 0x00272F83
	public virtual bool UsesIndexMarkers
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.indexMarkers && this.sprFill != null && this.IndexElementCount > 0 && this.IndexElementCount < this.IndexMarkerSprites.Count;
		}
	}

	// Token: 0x060060C9 RID: 24777 RVA: 0x00274DB4 File Offset: 0x00272FB4
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("forward");
		if (childById != null)
		{
			childById.OnPress += this.ForwardButton_OnPress;
			childById.OnHover += this.Button_OnHover;
			this.forwardButton = (childById.ViewComponent as XUiV_Button);
		}
		XUiController childById2 = base.GetChildById("back");
		if (childById2 != null)
		{
			childById2.OnPress += this.BackButton_OnPress;
			childById2.OnHover += this.Button_OnHover;
			this.backButton = (childById2.ViewComponent as XUiV_Button);
		}
		XUiController childById3 = base.GetChildById("fill");
		if (childById3 != null)
		{
			XUiV_Sprite xuiV_Sprite = childById3.ViewComponent as XUiV_Sprite;
			if (xuiV_Sprite != null)
			{
				this.sprFill = xuiV_Sprite;
			}
		}
		XUiController childById4 = base.GetChildById("indexMarkers");
		if (childById4 != null)
		{
			string input;
			if (childById4.CustomAttributes.TryGetValue("active_color", out input))
			{
				this.colorIndexMarkerActive = StringParsers.ParseColor32(input);
			}
			string input2;
			if (childById4.CustomAttributes.TryGetValue("inactive_color", out input2))
			{
				this.colorIndexMarkerInactive = StringParsers.ParseColor32(input2);
			}
			for (int i = 0; i < childById4.Children.Count; i++)
			{
				XUiV_Sprite xuiV_Sprite2 = childById4.Children[i].ViewComponent as XUiV_Sprite;
				if (xuiV_Sprite2 != null)
				{
					this.IndexMarkerSprites.Add(xuiV_Sprite2);
				}
			}
		}
		this.UpdateSegmentedFillPositions();
		this.clickable = base.GetChildById("directvalue");
		if (this.clickable != null)
		{
			this.clickable.OnPress += this.PressEvent;
			this.clickable.OnScroll += this.ScrollEvent;
			this.clickable.OnDrag += this.DragEvent;
			this.clickable.OnHover += this.HoverEvent;
		}
		if (string.IsNullOrEmpty(this.viewComponent.ToolTip) && base.Parent != null)
		{
			foreach (XUiController xuiController in base.Parent.Children)
			{
				XUiV_Label xuiV_Label = xuiController.ViewComponent as XUiV_Label;
				if (xuiV_Label != null)
				{
					this.viewComponent.ToolTip = xuiV_Label.ToolTip;
					break;
				}
			}
		}
	}

	// Token: 0x060060CA RID: 24778 RVA: 0x00275010 File Offset: 0x00273210
	public override void OnOpen()
	{
		this.IsDirty = true;
		base.OnOpen();
		this.UpdateLabel();
		this.UpdateIndexMarkerPositions();
		this.UpdateIndexMarkerStates();
		this.UpdateSegmentedFillVisibilities();
		base.RefreshBindings(false);
	}

	// Token: 0x060060CB RID: 24779 RVA: 0x0027503E File Offset: 0x0027323E
	public override void OnClose()
	{
		base.OnClose();
		if (this.isOver)
		{
			base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuComboBox);
		}
	}

	// Token: 0x060060CC RID: 24780 RVA: 0x00275060 File Offset: 0x00273260
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.enabled && this.clickable != null && base.xui.playerUI.CursorController.CurrentTarget == this.clickable.ViewComponent && !base.xui.playerUI.CursorController.Locked)
		{
			XUi.HandlePaging(base.xui, new Func<bool>(this.PageUpAction), new Func<bool>(this.PageDownAction), false);
		}
		if (this.enabled)
		{
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			int? num;
			if (nativePlatform == null)
			{
				num = null;
			}
			else
			{
				PlayerInputManager input = nativePlatform.Input;
				num = ((input != null) ? new PlayerInputManager.InputStyle?(input.CurrentInputStyle) : null);
			}
			if ((num ?? 1) != 1)
			{
				this.forwardButton.CurrentColor = (this.forwardButton.Enabled ? (this.isOver ? this.forwardButton.HoverSpriteColor : this.forwardButton.DefaultSpriteColor) : this.forwardButton.DisabledSpriteColor);
				this.backButton.CurrentColor = (this.backButton.Enabled ? (this.isOver ? this.backButton.HoverSpriteColor : this.backButton.DefaultSpriteColor) : this.backButton.DisabledSpriteColor);
			}
		}
		if (this.gamepadDecreaseShortcut != null && this.viewComponent.UiTransform.gameObject.activeInHierarchy && this.gamepadDecreaseShortcut.WasPressed)
		{
			this.PageDownAction();
		}
		if (this.gamepadIncreaseShortcut != null && this.viewComponent.UiTransform.gameObject.activeInHierarchy && this.gamepadIncreaseShortcut.WasPressed)
		{
			this.PageUpAction();
		}
		if (this.IsDirty)
		{
			this.IsDirty = false;
			this.UpdateIndexMarkerStates();
			base.RefreshBindings(false);
		}
	}

	// Token: 0x060060CD RID: 24781 RVA: 0x00275244 File Offset: 0x00273444
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_name);
		if (num <= 2185908414U)
		{
			if (num <= 1464747222U)
			{
				if (num != 470464957U)
				{
					if (num != 575017818U)
					{
						if (num == 1464747222U)
						{
							if (_name == "enabled_fill_color")
							{
								this.colorFillEnabled = StringParsers.ParseColor32(_value);
								return true;
							}
						}
					}
					else if (_name == "bg_color")
					{
						this.colorBg = StringParsers.ParseColor32(_value);
						return true;
					}
				}
				else if (_name == "gamepad_increase")
				{
					this.gamepadIncreaseShortcut = base.xui.playerUI.playerInput.GUIActions.GetPlayerActionByName(_value);
					return true;
				}
			}
			else if (num <= 1871259129U)
			{
				if (num != 1490568401U)
				{
					if (num == 1871259129U)
					{
						if (_name == "segmented_fill")
						{
							this.UsesSegmentedFill = StringParsers.ParseBool(_value, 0, -1, true);
							return true;
						}
					}
				}
				else if (_name == "gamepad_decrease")
				{
					this.gamepadDecreaseShortcut = base.xui.playerUI.playerInput.GUIActions.GetPlayerActionByName(_value);
					return true;
				}
			}
			else if (num != 2099093595U)
			{
				if (num == 2185908414U)
				{
					if (_name == "scroll_by_increment")
					{
						this.ScrollByIncrement = StringParsers.ParseBool(_value, 0, -1, true);
						return true;
					}
				}
			}
			else if (_name == "disabled_fill_color")
			{
				this.colorFillDisabled = StringParsers.ParseColor32(_value);
				return true;
			}
		}
		else if (num <= 2643151247U)
		{
			if (num != 2262080919U)
			{
				if (num != 2349648791U)
				{
					if (num == 2643151247U)
					{
						if (_name == "index_markers")
						{
							this.indexMarkers = StringParsers.ParseBool(_value, 0, -1, true);
							return true;
						}
					}
				}
				else if (_name == "value_wrap")
				{
					if (!_value.EqualsCaseInsensitive("@def"))
					{
						this.Wrap = StringParsers.ParseBool(_value, 0, -1, true);
					}
					return true;
				}
			}
			else if (_name == "combo_enabled")
			{
				this.Enabled = StringParsers.ParseBool(_value, 0, -1, true);
				return true;
			}
		}
		else if (num <= 3868148786U)
		{
			if (num != 3262104480U)
			{
				if (num == 3868148786U)
				{
					if (_name == "enabled_color")
					{
						this.colorEnabled = StringParsers.ParseColor32(_value);
						return true;
					}
				}
			}
			else if (_name == "segment_spacing")
			{
				this.SegmentedFillSpacing = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
				return true;
			}
		}
		else if (num != 4076031121U)
		{
			if (num == 4211930314U)
			{
				if (_name == "segment_count")
				{
					this.SegmentedFillCount = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
					return true;
				}
			}
		}
		else if (_name == "disabled_color")
		{
			this.colorDisabled = StringParsers.ParseColor32(_value);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x060060CE RID: 24782
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void UpdateLabel();

	// Token: 0x060060CF RID: 24783 RVA: 0x0027556C File Offset: 0x0027376C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void UpdateIndexMarkerStates()
	{
		if (this.UsesIndexMarkers)
		{
			for (int i = 0; i < this.IndexMarkerSprites.Count; i++)
			{
				this.IndexMarkerSprites[i].Color = this.colorIndexMarkerInactive;
			}
			if (this.IndexMarkerIndex >= 0 && this.IndexMarkerIndex < this.IndexMarkerSprites.Count)
			{
				this.IndexMarkerSprites[this.IndexMarkerIndex].Color = this.colorIndexMarkerActive;
			}
		}
	}

	// Token: 0x060060D0 RID: 24784 RVA: 0x002755E8 File Offset: 0x002737E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void UpdateIndexMarkerPositions()
	{
		if (this.UsesIndexMarkers)
		{
			float num = (float)(this.sprFill.Size.x + 5) / (float)this.IndexElementCount;
			int x = Mathf.RoundToInt(num - 5f);
			for (int i = 0; i < this.IndexMarkerSprites.Count; i++)
			{
				if (i >= this.IndexElementCount)
				{
					this.IndexMarkerSprites[i].IsVisible = false;
				}
				else
				{
					Vector2i position = this.IndexMarkerSprites[i].Position;
					position.x = Mathf.RoundToInt((float)i * num);
					this.IndexMarkerSprites[i].Position = position;
					this.IndexMarkerSprites[i].IsDirty = true;
					Vector2i size = this.IndexMarkerSprites[i].Size;
					size.x = x;
					if (i == this.IndexElementCount - 1)
					{
						size.x = this.sprFill.Size.x - position.x;
					}
					this.IndexMarkerSprites[i].Size = size;
				}
			}
		}
	}

	// Token: 0x060060D1 RID: 24785 RVA: 0x00275704 File Offset: 0x00273904
	[PublicizedFrom(EAccessModifier.Protected)]
	public void UpdateSegmentedFillPositions()
	{
		if (!this.UsesSegmentedFill || this.sprFill == null)
		{
			return;
		}
		XUiController childById = base.GetChildById("segmentedFill");
		if (childById == null)
		{
			return;
		}
		if (this.segmentedFillRects == null)
		{
			this.segmentedFillRects = new List<ValueTuple<XUiView, List<XUiView>>>();
			for (int i = 0; i < childById.Children.Count; i++)
			{
				XUiView viewComponent = childById.Children[i].ViewComponent;
				List<XUiView> list = new List<XUiView>();
				foreach (XUiController xuiController in viewComponent.Controller.Children)
				{
					list.Add(xuiController.ViewComponent);
				}
				this.segmentedFillRects.Add(new ValueTuple<XUiView, List<XUiView>>(viewComponent, list));
			}
		}
		if (this.segmentedFillRects.Count == 0)
		{
			return;
		}
		float num = (float)(this.sprFill.Size.x + this.SegmentedFillSpacing) / (float)this.SegmentedFillCount;
		int x = Mathf.RoundToInt(num - (float)this.SegmentedFillSpacing);
		for (int j = 0; j < this.segmentedFillRects.Count; j++)
		{
			XUiView item = this.segmentedFillRects[j].Item1;
			Vector2i position = item.Position;
			position.x = Mathf.RoundToInt((float)j * num);
			item.Position = position;
			item.IsDirty = true;
			foreach (XUiView xuiView in this.segmentedFillRects[j].Item2)
			{
				Vector2i size = xuiView.Size;
				size.x = x;
				if (j == this.SegmentedFillCount - 1)
				{
					size.x = this.sprFill.Size.x - position.x;
				}
				xuiView.Size = size;
				xuiView.IsDirty = true;
			}
		}
		this.UpdateSegmentedFillVisibilities();
	}

	// Token: 0x060060D2 RID: 24786 RVA: 0x00275914 File Offset: 0x00273B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateSegmentedFillVisibilities()
	{
		if (this.segmentedFillRects == null)
		{
			return;
		}
		for (int i = 0; i < this.segmentedFillRects.Count; i++)
		{
			this.segmentedFillRects[i].Item1.IsVisible = (i < this.SegmentedFillCount);
		}
	}

	// Token: 0x060060D3 RID: 24787 RVA: 0x0027595F File Offset: 0x00273B5F
	public void TriggerValueChangedEvent(TValue _oldVal)
	{
		this.UpdateIndexMarkerStates();
		XUiC_ComboBox<TValue>.XUiEvent_GenericValueChanged onValueChangedGeneric = this.OnValueChangedGeneric;
		if (onValueChangedGeneric != null)
		{
			onValueChangedGeneric(this);
		}
		XUiC_ComboBox<TValue>.XUiEvent_ValueChanged onValueChanged = this.OnValueChanged;
		if (onValueChanged != null)
		{
			onValueChanged(this, _oldVal, this.currentValue);
		}
		this.IsDirty = true;
	}

	// Token: 0x060060D4 RID: 24788 RVA: 0x0027599C File Offset: 0x00273B9C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryPageUp()
	{
		bool flag = false;
		if (this.enabled)
		{
			TValue oldVal = this.currentValue;
			this.ForwardPressed();
			this.UpdateLabel();
			flag = this.isDifferentValue(oldVal, this.currentValue);
			if (flag)
			{
				this.TriggerValueChangedEvent(oldVal);
			}
			this.IsDirty = true;
		}
		return flag;
	}

	// Token: 0x060060D5 RID: 24789 RVA: 0x002759E8 File Offset: 0x00273BE8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryPageDown()
	{
		bool flag = false;
		if (this.enabled)
		{
			TValue oldVal = this.currentValue;
			this.BackPressed();
			this.UpdateLabel();
			flag = this.isDifferentValue(oldVal, this.currentValue);
			if (flag)
			{
				this.TriggerValueChangedEvent(oldVal);
			}
			this.IsDirty = true;
		}
		return flag;
	}

	// Token: 0x060060D6 RID: 24790 RVA: 0x00275A32 File Offset: 0x00273C32
	[PublicizedFrom(EAccessModifier.Private)]
	public void Button_OnHover(XUiController _sender, bool _isOver)
	{
		this.isOverAnyPart = _isOver;
		XUiC_ComboBox<TValue>.XUiEvent_HoveredStateChanged onHoveredStateChanged = this.OnHoveredStateChanged;
		if (onHoveredStateChanged == null)
		{
			return;
		}
		onHoveredStateChanged(this, this.isOver, this.isOverAnyPart);
	}

	// Token: 0x060060D7 RID: 24791 RVA: 0x00275A58 File Offset: 0x00273C58
	[PublicizedFrom(EAccessModifier.Protected)]
	public void BackButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.TryPageDown();
	}

	// Token: 0x060060D8 RID: 24792 RVA: 0x00275A61 File Offset: 0x00273C61
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ForwardButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.TryPageUp();
	}

	// Token: 0x060060D9 RID: 24793 RVA: 0x00275A6A File Offset: 0x00273C6A
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool PageUpAction()
	{
		bool result = this.TryPageUp();
		if (base.xui.playerUI.CursorController.CursorModeActive)
		{
			base.SelectCursorElement(false, true);
		}
		return result;
	}

	// Token: 0x060060DA RID: 24794 RVA: 0x00275A92 File Offset: 0x00273C92
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool PageDownAction()
	{
		bool result = this.TryPageDown();
		if (base.xui.playerUI.CursorController.CursorModeActive)
		{
			base.SelectCursorElement(false, true);
		}
		return result;
	}

	// Token: 0x060060DB RID: 24795
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract bool isDifferentValue(TValue _oldVal, TValue _currentValue);

	// Token: 0x060060DC RID: 24796
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void BackPressed();

	// Token: 0x060060DD RID: 24797
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void ForwardPressed();

	// Token: 0x060060DE RID: 24798
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract bool isMax();

	// Token: 0x060060DF RID: 24799
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract bool isMin();

	// Token: 0x060060E0 RID: 24800
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract bool isEmpty();

	// Token: 0x060060E1 RID: 24801 RVA: 0x00275ABC File Offset: 0x00273CBC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName.StartsWith("segment_fill_", StringComparison.Ordinal))
		{
			int index;
			return int.TryParse(_bindingName.AsSpan("segment_fill_".Length), out index) && this.handleSegmentedFillValueBinding(ref _value, index);
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 2803537331U)
		{
			if (num <= 56539099U)
			{
				if (num != 12543033U)
				{
					if (num == 56539099U)
					{
						if (_bindingName == "can_backward")
						{
							_value = (this.enabled && !this.isEmpty() && (this.Wrap || !this.isMin())).ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "fillvalue")
				{
					_value = "0";
					return true;
				}
			}
			else if (num != 122655197U)
			{
				if (num != 1222862367U)
				{
					if (num == 2803537331U)
					{
						if (_bindingName == "can_forward")
						{
							_value = (this.enabled && !this.isEmpty() && (this.Wrap || !this.isMax())).ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "fillcolor")
				{
					_value = (this.enabled ? this.colorFillEnabled : this.colorFillDisabled).ToXuiColorString();
					return true;
				}
			}
			else if (_bindingName == "valuecolor")
			{
				_value = (this.enabled ? this.colorEnabled : this.colorDisabled).ToXuiColorString();
				return true;
			}
		}
		else if (num <= 3154291628U)
		{
			if (num != 2939888673U)
			{
				if (num == 3154291628U)
				{
					if (_bindingName == "usesmarkers")
					{
						_value = this.UsesIndexMarkers.ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "valuetext")
			{
				_value = this.valueText;
				return true;
			}
		}
		else if (num != 3427432841U)
		{
			if (num != 3853892426U)
			{
				if (num == 4283090096U)
				{
					if (_bindingName == "isnumber")
					{
						_value = false.ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "hascontrollershortcuts")
			{
				_value = (this.gamepadIncreaseShortcut != null && this.gamepadDecreaseShortcut != null).ToString();
				return true;
			}
		}
		else if (_bindingName == "bgcolor")
		{
			_value = this.colorBg.ToXuiColorString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x060060E2 RID: 24802 RVA: 0x00275D7E File Offset: 0x00273F7E
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool handleSegmentedFillValueBinding(ref string _value, int _index)
	{
		_value = "0";
		return true;
	}

	// Token: 0x060060E3 RID: 24803 RVA: 0x00275D88 File Offset: 0x00273F88
	[PublicizedFrom(EAccessModifier.Private)]
	public void PressEvent(XUiController _sender, int _mouseButton)
	{
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			return;
		}
		if (this.enabled)
		{
			Vector2i mouseXUIPosition = base.xui.GetMouseXUIPosition();
			XUiController parent = this.clickable;
			Vector3 vector = parent.ViewComponent.UiTransform.localPosition;
			while (parent.Parent != null && parent.Parent.ViewComponent != null)
			{
				parent = parent.Parent;
				vector += parent.ViewComponent.UiTransform.localPosition;
			}
			vector += parent.ViewComponent.UiTransform.parent.localPosition;
			XUiV_Window xuiV_Window = parent.ViewComponent as XUiV_Window;
			if (xuiV_Window != null && xuiV_Window.IsInStackpanel)
			{
				Transform parent2 = xuiV_Window.UiTransform.parent.parent;
				vector *= parent2.localScale.x;
				vector += parent2.localPosition;
			}
			Vector2i vector2i = new Vector2i((int)vector.x, (int)vector.y);
			int num = (vector2i + this.clickable.ViewComponent.Size).x - vector2i.x;
			float num2 = (float)(mouseXUIPosition.x - vector2i.x) / (float)num;
			this.setRelativeValue((double)num2);
		}
	}

	// Token: 0x060060E4 RID: 24804 RVA: 0x00275ECB File Offset: 0x002740CB
	public void ScrollEvent(XUiController _sender, float _delta)
	{
		if (this.enabled && !this.isEmpty())
		{
			if (this.ScrollByIncrement)
			{
				if (_delta > 0f)
				{
					this.PageUpAction();
					return;
				}
				this.PageDownAction();
				return;
			}
			else
			{
				this.incrementalChangeValue((double)_delta);
			}
		}
	}

	// Token: 0x060060E5 RID: 24805 RVA: 0x00275F08 File Offset: 0x00274108
	[PublicizedFrom(EAccessModifier.Private)]
	public void HoverEvent(XUiController _sender, bool _isOver)
	{
		this.isOver = _isOver;
		this.isOverAnyPart = _isOver;
		XUiC_ComboBox<TValue>.XUiEvent_HoveredStateChanged onHoveredStateChanged = this.OnHoveredStateChanged;
		if (onHoveredStateChanged != null)
		{
			onHoveredStateChanged(this, this.isOver, this.isOverAnyPart);
		}
		LocalPlayerUI.IsAnyComboBoxFocused = this.isOver;
		if (this.isOver)
		{
			base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuComboBox, 0f);
			return;
		}
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.MenuComboBox);
	}

	// Token: 0x060060E6 RID: 24806 RVA: 0x00275F7C File Offset: 0x0027417C
	[PublicizedFrom(EAccessModifier.Private)]
	public void DragEvent(XUiController _sender, EDragType _dragType, Vector2 _mousePositionDelta)
	{
		if (!this.isOver)
		{
			return;
		}
		if (this.enabled && !this.isEmpty())
		{
			this.PressEvent(_sender, -1);
		}
	}

	// Token: 0x060060E7 RID: 24807
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void setRelativeValue(double _value);

	// Token: 0x060060E8 RID: 24808
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void incrementalChangeValue(double _value);

	// Token: 0x060060E9 RID: 24809 RVA: 0x00275FA0 File Offset: 0x002741A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_ComboBox()
	{
	}

	// Token: 0x040048F8 RID: 18680
	[PublicizedFrom(EAccessModifier.Private)]
	public bool enabled = true;

	// Token: 0x040048F9 RID: 18681
	public TValue Min;

	// Token: 0x040048FA RID: 18682
	public TValue Max;

	// Token: 0x040048FB RID: 18683
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool Wrap = true;

	// Token: 0x040048FC RID: 18684
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool ScrollByIncrement;

	// Token: 0x040048FD RID: 18685
	[PublicizedFrom(EAccessModifier.Private)]
	public Color colorEnabled;

	// Token: 0x040048FE RID: 18686
	[PublicizedFrom(EAccessModifier.Private)]
	public Color colorDisabled;

	// Token: 0x040048FF RID: 18687
	[PublicizedFrom(EAccessModifier.Private)]
	public Color colorBg;

	// Token: 0x04004900 RID: 18688
	[PublicizedFrom(EAccessModifier.Private)]
	public Color colorFillEnabled;

	// Token: 0x04004901 RID: 18689
	[PublicizedFrom(EAccessModifier.Private)]
	public Color colorFillDisabled;

	// Token: 0x04004902 RID: 18690
	[PublicizedFrom(EAccessModifier.Private)]
	public Color colorIndexMarkerActive = Color.white;

	// Token: 0x04004903 RID: 18691
	[PublicizedFrom(EAccessModifier.Private)]
	public Color colorIndexMarkerInactive = Color.grey;

	// Token: 0x04004904 RID: 18692
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerAction gamepadDecreaseShortcut;

	// Token: 0x04004905 RID: 18693
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerAction gamepadIncreaseShortcut;

	// Token: 0x04004906 RID: 18694
	[PublicizedFrom(EAccessModifier.Private)]
	public bool indexMarkers;

	// Token: 0x04004907 RID: 18695
	[PublicizedFrom(EAccessModifier.Private)]
	public string valueText = "";

	// Token: 0x04004908 RID: 18696
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Sprite sprFill;

	// Token: 0x04004909 RID: 18697
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly IList<XUiV_Sprite> IndexMarkerSprites = new List<XUiV_Sprite>();

	// Token: 0x0400490A RID: 18698
	[PublicizedFrom(EAccessModifier.Protected)]
	public TValue currentValue;

	// Token: 0x0400490B RID: 18699
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool UsesSegmentedFill;

	// Token: 0x0400490C RID: 18700
	[PublicizedFrom(EAccessModifier.Protected)]
	public int SegmentedFillSpacing;

	// Token: 0x0400490D RID: 18701
	[PublicizedFrom(EAccessModifier.Private)]
	public int segmentedFillCount = 1;

	// Token: 0x0400490E RID: 18702
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController clickable;

	// Token: 0x0400490F RID: 18703
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOver;

	// Token: 0x04004910 RID: 18704
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOverAnyPart;

	// Token: 0x04004911 RID: 18705
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDragging;

	// Token: 0x04004912 RID: 18706
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button forwardButton;

	// Token: 0x04004913 RID: 18707
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button backButton;

	// Token: 0x04004914 RID: 18708
	[TupleElementNames(new string[]
	{
		"rect",
		"sprites"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ValueTuple<XUiView, List<XUiView>>> segmentedFillRects;

	// Token: 0x04004915 RID: 18709
	[PublicizedFrom(EAccessModifier.Private)]
	public const string bindingPrefixSegmentFillValue = "segment_fill_";

	// Token: 0x02000C4E RID: 3150
	// (Invoke) Token: 0x060060EB RID: 24811
	public delegate void XUiEvent_ValueChanged(XUiController _sender, TValue _oldValue, TValue _newValue);

	// Token: 0x02000C4F RID: 3151
	// (Invoke) Token: 0x060060EF RID: 24815
	public delegate void XUiEvent_GenericValueChanged(XUiController _sender);

	// Token: 0x02000C50 RID: 3152
	// (Invoke) Token: 0x060060F3 RID: 24819
	public delegate void XUiEvent_HoveredStateChanged(XUiController _sender, bool _isOverMainArea, bool _isOverAnyPart);
}
