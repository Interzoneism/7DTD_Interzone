using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CA5 RID: 3237
[Preserve]
public class XUiC_DropDown : XUiController
{
	// Token: 0x1400009F RID: 159
	// (add) Token: 0x060063E0 RID: 25568 RVA: 0x00287898 File Offset: 0x00285A98
	// (remove) Token: 0x060063E1 RID: 25569 RVA: 0x002878D0 File Offset: 0x00285AD0
	public event XUiEvent_InputOnSubmitEventHandler OnSubmitHandler;

	// Token: 0x140000A0 RID: 160
	// (add) Token: 0x060063E2 RID: 25570 RVA: 0x00287908 File Offset: 0x00285B08
	// (remove) Token: 0x060063E3 RID: 25571 RVA: 0x00287940 File Offset: 0x00285B40
	public event XUiEvent_InputOnChangedEventHandler OnChangeHandler;

	// Token: 0x17000A2D RID: 2605
	// (get) Token: 0x060063E4 RID: 25572 RVA: 0x00287975 File Offset: 0x00285B75
	public int PageLength
	{
		get
		{
			XUiC_DropDown.Entry[] array = this.listEntryControllers;
			if (array == null)
			{
				return 1;
			}
			return array.Length;
		}
	}

	// Token: 0x17000A2E RID: 2606
	// (get) Token: 0x060063E5 RID: 25573 RVA: 0x00287985 File Offset: 0x00285B85
	// (set) Token: 0x060063E6 RID: 25574 RVA: 0x0028798D File Offset: 0x00285B8D
	public bool DropdownOpen
	{
		get
		{
			return this.dropdownOpen;
		}
		set
		{
			if (value != this.dropdownOpen)
			{
				this.dropdownOpen = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000A2F RID: 2607
	// (get) Token: 0x060063E7 RID: 25575 RVA: 0x002879A6 File Offset: 0x00285BA6
	// (set) Token: 0x060063E8 RID: 25576 RVA: 0x002879B0 File Offset: 0x00285BB0
	public int Page
	{
		get
		{
			return this.page;
		}
		set
		{
			int num = Mathf.Clamp(value, 0, this.LastPage);
			if (num != this.page)
			{
				this.page = num;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000A30 RID: 2608
	// (get) Token: 0x060063E9 RID: 25577 RVA: 0x002879E2 File Offset: 0x00285BE2
	public int LastPage
	{
		get
		{
			return Math.Max(0, Mathf.CeilToInt((float)this.filteredEntries.Count / (float)this.PageLength) - 1);
		}
	}

	// Token: 0x17000A31 RID: 2609
	// (get) Token: 0x060063EA RID: 25578 RVA: 0x00287A05 File Offset: 0x00285C05
	public int EntryCount
	{
		get
		{
			return this.filteredEntries.Count;
		}
	}

	// Token: 0x17000A32 RID: 2610
	// (get) Token: 0x060063EB RID: 25579 RVA: 0x00287A12 File Offset: 0x00285C12
	// (set) Token: 0x060063EC RID: 25580 RVA: 0x00287A1F File Offset: 0x00285C1F
	public string Text
	{
		get
		{
			return this.input.Text;
		}
		set
		{
			if (value != this.input.Text)
			{
				this.input.Text = value;
				this.UpdateFilteredList();
			}
		}
	}

	// Token: 0x17000A33 RID: 2611
	// (get) Token: 0x060063ED RID: 25581 RVA: 0x00287A46 File Offset: 0x00285C46
	public XUiC_TextInput TextInput
	{
		get
		{
			return this.input;
		}
	}

	// Token: 0x060063EE RID: 25582 RVA: 0x00287A50 File Offset: 0x00285C50
	public override void Init()
	{
		base.Init();
		base.OnScroll += this.HandleOnScroll;
		XUiController childById = base.GetChildById("pageUp");
		if (childById != null)
		{
			childById.OnPress += this.HandlePageUpPress;
		}
		XUiController childById2 = base.GetChildById("pageDown");
		if (childById2 != null)
		{
			childById2.OnPress += this.HandlePageDownPress;
		}
		this.handlePageDownAction = new Func<bool>(this.HandlePageDown);
		this.handlePageUpAction = new Func<bool>(this.HandlePageUp);
		XUiController childById3 = base.GetChildById("list");
		if (childById3 != null)
		{
			this.listEntryControllers = new XUiC_DropDown.Entry[childById3.Children.Count];
			for (int i = 0; i < childById3.Children.Count; i++)
			{
				this.listEntryControllers[i] = (childById3.Children[i] as XUiC_DropDown.Entry);
				if (this.listEntryControllers[i] != null)
				{
					this.listEntryControllers[i].OnScroll += this.HandleOnScroll;
					this.listEntryControllers[i].Owner = this;
				}
				else
				{
					Log.Warning("[XUi] DropDown elements do not have the correct controller set (should be \"XUiC_DropDown+Entry\")");
				}
			}
		}
		this.input = (base.GetChildById("input") as XUiC_TextInput);
		if (this.input != null)
		{
			this.input.OnChangeHandler += this.OnInputChanged;
			this.input.OnSubmitHandler += this.OnInputSubmit;
			this.input.OnSelect += this.OnInputSelected;
		}
		XUiController childById4 = base.GetChildById("btnDropdown");
		if (childById4 != null)
		{
			childById4.OnPress += this.BtnDropdown_OnPress;
		}
	}

	// Token: 0x060063EF RID: 25583 RVA: 0x00287BFD File Offset: 0x00285DFD
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDropdown_OnPress(XUiController _sender, int _mouseButton)
	{
		this.DropdownOpen = !this.DropdownOpen;
	}

	// Token: 0x060063F0 RID: 25584 RVA: 0x00287C0E File Offset: 0x00285E0E
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnInputSelected(XUiController _sender, bool _selected)
	{
		if (_selected)
		{
			this.DropdownOpen = true;
			return;
		}
		ThreadManager.StartCoroutine(this.CloseDropdownLater());
	}

	// Token: 0x060063F1 RID: 25585 RVA: 0x00287C27 File Offset: 0x00285E27
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator CloseDropdownLater()
	{
		while (base.xui.playerUI.playerInput.GUIActions.LeftClick.IsPressed)
		{
			yield return null;
		}
		yield return null;
		if (!this.input.IsSelected)
		{
			this.DropdownOpen = false;
		}
		yield break;
	}

	// Token: 0x060063F2 RID: 25586 RVA: 0x00287C36 File Offset: 0x00285E36
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnInputSubmit(XUiController _sender, string _text)
	{
		this.DropdownOpen = false;
		this.input.SetSelected(false, false);
		this.OnInputChanged(_sender, _text, false);
		this.SendSubmitEvent();
	}

	// Token: 0x060063F3 RID: 25587 RVA: 0x00287C5B File Offset: 0x00285E5B
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnInputChanged(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.UpdateFilteredList();
		this.SendChangedEvent(_changeFromCode);
	}

	// Token: 0x060063F4 RID: 25588 RVA: 0x00287C6A File Offset: 0x00285E6A
	[PublicizedFrom(EAccessModifier.Private)]
	public void SendChangedEvent(bool _changeFromCode)
	{
		XUiEvent_InputOnChangedEventHandler onChangeHandler = this.OnChangeHandler;
		if (onChangeHandler == null)
		{
			return;
		}
		onChangeHandler(this, this.Text, _changeFromCode);
	}

	// Token: 0x060063F5 RID: 25589 RVA: 0x00287C84 File Offset: 0x00285E84
	[PublicizedFrom(EAccessModifier.Private)]
	public void SendSubmitEvent()
	{
		XUiEvent_InputOnSubmitEventHandler onSubmitHandler = this.OnSubmitHandler;
		if (onSubmitHandler == null)
		{
			return;
		}
		onSubmitHandler(this, this.Text);
	}

	// Token: 0x060063F6 RID: 25590 RVA: 0x00287CA0 File Offset: 0x00285EA0
	public void UpdateFilteredList()
	{
		XUiC_TextInput xuiC_TextInput = this.input;
		string text = (xuiC_TextInput != null) ? xuiC_TextInput.Text : null;
		this.filteredEntries.Clear();
		if (!string.IsNullOrEmpty(text))
		{
			using (List<string>.Enumerator enumerator = this.AllEntries.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string text2 = enumerator.Current;
					if (text2.ContainsCaseInsensitive(text))
					{
						this.filteredEntries.Add(text2);
					}
				}
				goto IL_7B;
			}
		}
		this.filteredEntries.AddRange(this.AllEntries);
		IL_7B:
		if (this.sortEntries)
		{
			this.filteredEntries.Sort();
		}
		this.Page = 0;
		this.IsDirty = true;
	}

	// Token: 0x060063F7 RID: 25591 RVA: 0x00287D5C File Offset: 0x00285F5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnScroll(XUiController _sender, float _delta)
	{
		if (_delta > 0f)
		{
			this.HandlePageDown();
			return;
		}
		this.HandlePageUp();
	}

	// Token: 0x060063F8 RID: 25592 RVA: 0x00287D75 File Offset: 0x00285F75
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandlePageDownPress(XUiController _sender, int _mouseButton)
	{
		this.HandlePageDown();
	}

	// Token: 0x060063F9 RID: 25593 RVA: 0x00287D7E File Offset: 0x00285F7E
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandlePageUpPress(XUiController _sender, int _mouseButton)
	{
		this.HandlePageUp();
	}

	// Token: 0x060063FA RID: 25594 RVA: 0x00287D88 File Offset: 0x00285F88
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HandlePageDown()
	{
		this.input.SetSelected(true, false);
		if (this.page > 0)
		{
			int num = this.Page;
			this.Page = num - 1;
			return true;
		}
		return false;
	}

	// Token: 0x060063FB RID: 25595 RVA: 0x00287DC0 File Offset: 0x00285FC0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HandlePageUp()
	{
		this.input.SetSelected(true, false);
		if ((this.page + 1) * this.PageLength < this.filteredEntries.Count)
		{
			int num = this.Page;
			this.Page = num + 1;
			return true;
		}
		return false;
	}

	// Token: 0x060063FC RID: 25596 RVA: 0x00287E0C File Offset: 0x0028600C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateCurrentPageContents()
	{
		for (int i = 0; i < this.PageLength; i++)
		{
			int num = i + this.PageLength * this.page;
			this.listEntryControllers[i].Text = ((num < this.filteredEntries.Count) ? this.filteredEntries[num] : null);
		}
	}

	// Token: 0x060063FD RID: 25597 RVA: 0x00287E64 File Offset: 0x00286064
	public override void Update(float _dt)
	{
		if (this.IsDirty)
		{
			if (this.page > this.LastPage)
			{
				this.Page = this.LastPage;
			}
			this.UpdateCurrentPageContents();
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
		base.Update(_dt);
		if (base.ViewComponent.IsVisible)
		{
			XUi.HandlePaging(base.xui, this.handlePageUpAction, this.handlePageDownAction, false);
		}
	}

	// Token: 0x060063FE RID: 25598 RVA: 0x00287ED4 File Offset: 0x002860D4
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "thumbareasize")
		{
			this.thumbAreaSize = StringParsers.ParseFloat(_value, 0, -1, NumberStyles.Any);
			return true;
		}
		if (_name == "dropdown_textcolor")
		{
			this.dropdownTextcolor = _value;
			return true;
		}
		if (_name == "dropdown_hovercolor")
		{
			this.dropdownHovercolor = _value;
			return true;
		}
		if (_name == "sortentries")
		{
			this.sortEntries = StringParsers.ParseBool(_value, 0, -1, true);
			return true;
		}
		if (!(_name == "clearonopen"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.clearOnOpen = StringParsers.ParseBool(_value, 0, -1, true);
		return true;
	}

	// Token: 0x060063FF RID: 25599 RVA: 0x00287F78 File Offset: 0x00286178
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		int num = Mathf.RoundToInt(this.thumbAreaSize / (float)(this.LastPage + 1));
		if (_bindingName == "flip_dropdownbutton")
		{
			_value = (this.dropdownOpen ? UIBasicSprite.Flip.Vertically : UIBasicSprite.Flip.Nothing).ToStringCached<UIBasicSprite.Flip>();
			return true;
		}
		if (_bindingName == "dropdown_open")
		{
			_value = this.dropdownOpen.ToString();
			return true;
		}
		if (_bindingName == "thumb_size")
		{
			_value = num.ToString();
			return true;
		}
		if (!(_bindingName == "thumb_position"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = Mathf.RoundToInt((float)this.Page / (float)(this.LastPage + 1) * this.thumbAreaSize).ToString();
		return true;
	}

	// Token: 0x06006400 RID: 25600 RVA: 0x00288031 File Offset: 0x00286231
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.clearOnOpen)
		{
			this.Text = string.Empty;
		}
		this.DropdownOpen = false;
		this.IsDirty = true;
		base.RefreshBindings(false);
	}

	// Token: 0x04004B31 RID: 19249
	public readonly List<string> AllEntries = new List<string>();

	// Token: 0x04004B32 RID: 19250
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<string> filteredEntries = new List<string>();

	// Token: 0x04004B33 RID: 19251
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DropDown.Entry[] listEntryControllers;

	// Token: 0x04004B34 RID: 19252
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput input;

	// Token: 0x04004B35 RID: 19253
	[PublicizedFrom(EAccessModifier.Private)]
	public Func<bool> handlePageDownAction;

	// Token: 0x04004B36 RID: 19254
	[PublicizedFrom(EAccessModifier.Private)]
	public Func<bool> handlePageUpAction;

	// Token: 0x04004B37 RID: 19255
	[PublicizedFrom(EAccessModifier.Private)]
	public float thumbAreaSize;

	// Token: 0x04004B38 RID: 19256
	[PublicizedFrom(EAccessModifier.Private)]
	public string dropdownHovercolor;

	// Token: 0x04004B39 RID: 19257
	[PublicizedFrom(EAccessModifier.Private)]
	public string dropdownTextcolor;

	// Token: 0x04004B3A RID: 19258
	[PublicizedFrom(EAccessModifier.Private)]
	public bool sortEntries;

	// Token: 0x04004B3B RID: 19259
	[PublicizedFrom(EAccessModifier.Private)]
	public bool clearOnOpen;

	// Token: 0x04004B3C RID: 19260
	[PublicizedFrom(EAccessModifier.Private)]
	public int page;

	// Token: 0x04004B3D RID: 19261
	[PublicizedFrom(EAccessModifier.Private)]
	public bool dropdownOpen;

	// Token: 0x02000CA6 RID: 3238
	[Preserve]
	public class Entry : XUiController
	{
		// Token: 0x17000A34 RID: 2612
		// (set) Token: 0x06006402 RID: 25602 RVA: 0x0028807F File Offset: 0x0028627F
		public string Text
		{
			set
			{
				if (value != this.text)
				{
					this.text = value;
					this.IsDirty = true;
				}
			}
		}

		// Token: 0x06006403 RID: 25603 RVA: 0x0028809D File Offset: 0x0028629D
		public override void OnOpen()
		{
			base.OnOpen();
			this.hovered = false;
			this.IsDirty = true;
		}

		// Token: 0x06006404 RID: 25604 RVA: 0x002880B3 File Offset: 0x002862B3
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnPressed(int _mouseButton)
		{
			base.OnPressed(_mouseButton);
			this.Owner.Text = this.text;
			this.Owner.DropdownOpen = false;
			this.Owner.SendChangedEvent(true);
			this.Owner.SendSubmitEvent();
		}

		// Token: 0x06006405 RID: 25605 RVA: 0x002880F0 File Offset: 0x002862F0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnHovered(bool _isOver)
		{
			base.OnHovered(_isOver);
			this.hovered = _isOver;
			this.IsDirty = true;
		}

		// Token: 0x06006406 RID: 25606 RVA: 0x0007FB71 File Offset: 0x0007DD71
		public override void Update(float _dt)
		{
			base.Update(_dt);
			if (this.IsDirty)
			{
				base.RefreshBindings(false);
				this.IsDirty = false;
			}
		}

		// Token: 0x06006407 RID: 25607 RVA: 0x00288108 File Offset: 0x00286308
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool GetBindingValueInternal(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = this.text;
				return true;
			}
			if (!(_bindingName == "textcolor"))
			{
				return base.GetBindingValueInternal(ref _value, _bindingName);
			}
			_value = ((this.Owner == null) ? "100,100,100" : (this.hovered ? this.Owner.dropdownHovercolor : this.Owner.dropdownTextcolor));
			return true;
		}

		// Token: 0x04004B3E RID: 19262
		public XUiC_DropDown Owner;

		// Token: 0x04004B3F RID: 19263
		[PublicizedFrom(EAccessModifier.Private)]
		public string text;

		// Token: 0x04004B40 RID: 19264
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hovered;
	}
}
