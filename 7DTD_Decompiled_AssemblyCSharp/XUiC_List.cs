using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;

// Token: 0x02000D00 RID: 3328
public abstract class XUiC_List<T> : XUiController where T : XUiListEntry<T>
{
	// Token: 0x140000A6 RID: 166
	// (add) Token: 0x06006755 RID: 26453 RVA: 0x0029EA5C File Offset: 0x0029CC5C
	// (remove) Token: 0x06006756 RID: 26454 RVA: 0x0029EA94 File Offset: 0x0029CC94
	public event XUiEvent_ListSelectionChangedEventHandler<T> SelectionChanged;

	// Token: 0x140000A7 RID: 167
	// (add) Token: 0x06006757 RID: 26455 RVA: 0x0029EACC File Offset: 0x0029CCCC
	// (remove) Token: 0x06006758 RID: 26456 RVA: 0x0029EB04 File Offset: 0x0029CD04
	public event XUiEvent_ListPageNumberChangedEventHandler PageNumberChanged;

	// Token: 0x140000A8 RID: 168
	// (add) Token: 0x06006759 RID: 26457 RVA: 0x0029EB3C File Offset: 0x0029CD3C
	// (remove) Token: 0x0600675A RID: 26458 RVA: 0x0029EB74 File Offset: 0x0029CD74
	public event XUiEvent_ListEntryClickedEventHandler<T> ListEntryClicked;

	// Token: 0x140000A9 RID: 169
	// (add) Token: 0x0600675B RID: 26459 RVA: 0x0029EBAC File Offset: 0x0029CDAC
	// (remove) Token: 0x0600675C RID: 26460 RVA: 0x0029EBE4 File Offset: 0x0029CDE4
	public event XUiEvent_PageContentsChangedEventHandler PageContentsChanged;

	// Token: 0x17000A8E RID: 2702
	// (get) Token: 0x0600675D RID: 26461 RVA: 0x0029EC19 File Offset: 0x0029CE19
	public int PageLength
	{
		get
		{
			if (this.listEntryControllers == null)
			{
				return 0;
			}
			return this.listEntryControllers.Length;
		}
	}

	// Token: 0x17000A8F RID: 2703
	// (get) Token: 0x0600675E RID: 26462 RVA: 0x0029EC2D File Offset: 0x0029CE2D
	// (set) Token: 0x0600675F RID: 26463 RVA: 0x0029EC38 File Offset: 0x0029CE38
	public int Page
	{
		get
		{
			return this.page;
		}
		set
		{
			int num = Math.Max(0, Math.Min(value, this.LastPage));
			if (num != this.page)
			{
				this.page = num;
				XUiC_Paging xuiC_Paging = this.pager;
				if (xuiC_Paging != null)
				{
					xuiC_Paging.SetPage(this.page);
				}
				this.IsDirty = true;
				this.SelectedEntry = null;
				XUiEvent_ListPageNumberChangedEventHandler pageNumberChanged = this.PageNumberChanged;
				if (pageNumberChanged == null)
				{
					return;
				}
				pageNumberChanged(this.page);
			}
		}
	}

	// Token: 0x17000A90 RID: 2704
	// (get) Token: 0x06006760 RID: 26464 RVA: 0x0029ECA3 File Offset: 0x0029CEA3
	public int LastPage
	{
		get
		{
			return Math.Max(0, Mathf.CeilToInt((float)this.filteredEntries.Count / (float)this.PageLength) - 1);
		}
	}

	// Token: 0x17000A91 RID: 2705
	// (get) Token: 0x06006761 RID: 26465 RVA: 0x0029ECC6 File Offset: 0x0029CEC6
	// (set) Token: 0x06006762 RID: 26466 RVA: 0x0029ECD0 File Offset: 0x0029CED0
	public XUiC_ListEntry<T> SelectedEntry
	{
		get
		{
			return this.selectedEntry;
		}
		set
		{
			if (value != this.selectedEntry)
			{
				T currentSelectedEntry = this.CurrentSelectedEntry;
				XUiC_ListEntry<T> xuiC_ListEntry = this.selectedEntry;
				this.selectedEntry = null;
				if (xuiC_ListEntry != null)
				{
					xuiC_ListEntry.Selected = false;
				}
				this.selectedEntry = value;
				if (this.selectedEntry != null)
				{
					this.selectedEntry.Selected = true;
					this.CurrentSelectedEntry = this.selectedEntry.GetEntry();
					for (int i = 0; i < this.listEntryControllers.Length; i++)
					{
						if (this.selectedEntry == this.listEntryControllers[i])
						{
							this.selectedEntryIndex = this.page * this.PageLength + i;
							break;
						}
					}
				}
				else
				{
					this.CurrentSelectedEntry = default(T);
					this.selectedEntryIndex = -1;
				}
				if (currentSelectedEntry != this.CurrentSelectedEntry)
				{
					this.OnSelectionChanged(xuiC_ListEntry, this.selectedEntry);
				}
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000A92 RID: 2706
	// (get) Token: 0x06006763 RID: 26467 RVA: 0x0029EDAA File Offset: 0x0029CFAA
	// (set) Token: 0x06006764 RID: 26468 RVA: 0x0029EDB4 File Offset: 0x0029CFB4
	public int SelectedEntryIndex
	{
		get
		{
			return this.selectedEntryIndex;
		}
		set
		{
			if (value >= 0 && value < this.EntryCount && this.selectedEntryIndex != value)
			{
				this.Page = value / this.PageLength;
				this.selectedEntryIndex = value;
				this.updateSelectedItemByIndex = true;
				this.updateCurrentPageContents();
				this.updateSelectedItemByIndex = false;
				this.SelectedEntry = this.listEntryControllers[this.selectedEntryIndex % this.PageLength];
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000A93 RID: 2707
	// (get) Token: 0x06006765 RID: 26469 RVA: 0x0029EE21 File Offset: 0x0029D021
	public int EntryCount
	{
		get
		{
			return this.filteredEntries.Count;
		}
	}

	// Token: 0x06006766 RID: 26470 RVA: 0x0029EE2E File Offset: 0x0029D02E
	public T GetEntry(int _index)
	{
		return this.filteredEntries[_index];
	}

	// Token: 0x17000A94 RID: 2708
	// (get) Token: 0x06006767 RID: 26471 RVA: 0x0029EE3C File Offset: 0x0029D03C
	public int UnfilteredEntryCount
	{
		get
		{
			return this.allEntries.Count;
		}
	}

	// Token: 0x17000A95 RID: 2709
	// (get) Token: 0x06006768 RID: 26472 RVA: 0x0029EE49 File Offset: 0x0029D049
	// (set) Token: 0x06006769 RID: 26473 RVA: 0x0029EE51 File Offset: 0x0029D051
	public bool ClearSelectionOnOpenClose { get; set; } = true;

	// Token: 0x17000A96 RID: 2710
	// (get) Token: 0x0600676A RID: 26474 RVA: 0x0029EE5A File Offset: 0x0029D05A
	// (set) Token: 0x0600676B RID: 26475 RVA: 0x0029EE62 File Offset: 0x0029D062
	public bool ClearSearchTextOnOpenClose { get; set; }

	// Token: 0x17000A97 RID: 2711
	// (get) Token: 0x0600676C RID: 26476 RVA: 0x0029EE6B File Offset: 0x0029D06B
	// (set) Token: 0x0600676D RID: 26477 RVA: 0x0029EE73 File Offset: 0x0029D073
	public bool SelectableEntries { get; set; } = true;

	// Token: 0x17000A98 RID: 2712
	// (get) Token: 0x0600676E RID: 26478 RVA: 0x0029EE7C File Offset: 0x0029D07C
	// (set) Token: 0x0600676F RID: 26479 RVA: 0x0029EE84 File Offset: 0x0029D084
	public bool CursorControllable { get; set; }

	// Token: 0x06006770 RID: 26480 RVA: 0x0029EE90 File Offset: 0x0029D090
	public override void Init()
	{
		base.Init();
		base.OnScroll += this.HandleOnScroll;
		this.pager = base.GetChildByType<XUiC_Paging>();
		if (this.pager != null)
		{
			this.pager.OnPageChanged += delegate()
			{
				this.Page = this.pager.CurrentPageNumber;
			};
		}
		XUiController childById = base.GetChildById("list");
		if (childById == null)
		{
			Log.Warning("[XUi] List controller without a 'list' child element! (window group '" + base.WindowGroup.ID + "')");
			this.listEntryControllers = Array.Empty<XUiC_ListEntry<T>>();
		}
		else
		{
			this.listEntryControllers = new XUiC_ListEntry<T>[childById.Children.Count];
			for (int i = 0; i < childById.Children.Count; i++)
			{
				this.listEntryControllers[i] = (childById.Children[i] as XUiC_ListEntry<T>);
				if (this.listEntryControllers[i] != null)
				{
					this.listEntryControllers[i].OnScroll += this.HandleOnScroll;
					this.listEntryControllers[i].List = this;
				}
				else
				{
					Log.Warning("[XUi] List elements do not have the correct controller set (should be \"XUiC_ListEntry<" + typeof(T).FullName + ">\")");
				}
			}
			if (this.CursorControllable)
			{
				XUiV_Grid xuiV_Grid = childById.ViewComponent as XUiV_Grid;
				if (xuiV_Grid != null)
				{
					if (xuiV_Grid.Arrangement == UIGrid.Arrangement.Horizontal)
					{
						this.columns = xuiV_Grid.Columns;
						this.rows = this.PageLength / this.columns;
					}
					else
					{
						this.rows = xuiV_Grid.Rows;
						this.columns = this.PageLength / this.rows;
					}
				}
				XUiV_Table xuiV_Table = childById.ViewComponent as XUiV_Table;
				if (xuiV_Table != null)
				{
					this.columns = xuiV_Table.Columns;
					this.rows = this.PageLength / this.columns;
				}
			}
		}
		this.searchBox = (base.GetChildById("searchInput") as XUiC_TextInput);
		if (this.searchBox != null)
		{
			this.searchBox.OnChangeHandler += this.OnSearchInputChanged;
			this.searchBox.OnSubmitHandler += this.OnSearchInputSubmit;
		}
		this.RebuildList(true);
	}

	// Token: 0x06006771 RID: 26481 RVA: 0x0029F0A0 File Offset: 0x0029D2A0
	public virtual void RebuildList(bool _resetFilter = false)
	{
		this.SelectedEntry = null;
		if (this.filteredEntries != null)
		{
			this.filteredEntries.Clear();
		}
		this.RefreshView(_resetFilter, true);
	}

	// Token: 0x06006772 RID: 26482 RVA: 0x0029F0C4 File Offset: 0x0029D2C4
	public virtual void RefreshView(bool _resetFilter = false, bool _resetPage = true)
	{
		if (_resetFilter && this.searchBox != null)
		{
			this.searchBox.Text = "";
		}
		XUiC_TextInput xuiC_TextInput = this.searchBox;
		this.OnSearchInputChanged(this, (xuiC_TextInput != null) ? xuiC_TextInput.Text : null, true);
		if (_resetPage)
		{
			this.Page = 0;
		}
	}

	// Token: 0x06006773 RID: 26483 RVA: 0x0029F110 File Offset: 0x0029D310
	public XUiC_ListEntry<T> IsVisible(T _value)
	{
		foreach (XUiC_ListEntry<T> xuiC_ListEntry in this.listEntryControllers)
		{
			T entry = xuiC_ListEntry.GetEntry();
			if (entry != null && entry == _value)
			{
				return xuiC_ListEntry;
			}
		}
		return null;
	}

	// Token: 0x06006774 RID: 26484 RVA: 0x0029F156 File Offset: 0x0029D356
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleOnScroll(XUiController _sender, float _delta)
	{
		if (_delta > 0f)
		{
			XUiC_Paging xuiC_Paging = this.pager;
			if (xuiC_Paging == null)
			{
				return;
			}
			xuiC_Paging.PageDown();
			return;
		}
		else
		{
			XUiC_Paging xuiC_Paging2 = this.pager;
			if (xuiC_Paging2 == null)
			{
				return;
			}
			xuiC_Paging2.PageUp();
			return;
		}
	}

	// Token: 0x06006775 RID: 26485 RVA: 0x0029F183 File Offset: 0x0029D383
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnSearchInputSubmit(XUiController _sender, string _text)
	{
		this.OnSearchInputChanged(_sender, _text, false);
	}

	// Token: 0x06006776 RID: 26486 RVA: 0x0029F18E File Offset: 0x0029D38E
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnSearchInputChanged(XUiController _sender, string _text, bool _changeFromCode)
	{
		this.FilterResults(_text);
		this.IsDirty = true;
	}

	// Token: 0x06006777 RID: 26487 RVA: 0x0029F19E File Offset: 0x0029D39E
	public IReadOnlyList<T> AllEntries()
	{
		return this.allEntries;
	}

	// Token: 0x06006778 RID: 26488 RVA: 0x0029F1A8 File Offset: 0x0029D3A8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void FilterResults(string _textMatch)
	{
		if (_textMatch == null)
		{
			this.filteredEntries.Clear();
			this.filteredEntries.AddRange(this.allEntries);
			return;
		}
		if (_textMatch == this.previousMatch && this.filteredEntries.Count == this.allEntries.Count)
		{
			return;
		}
		this.previousMatch = _textMatch;
		this.filteredEntries.Clear();
		if (_textMatch.Length > 0)
		{
			using (List<T>.Enumerator enumerator = this.allEntries.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					T t = enumerator.Current;
					if (t.MatchesSearch(_textMatch))
					{
						this.filteredEntries.Add(t);
					}
				}
				return;
			}
		}
		this.filteredEntries.AddRange(this.allEntries);
	}

	// Token: 0x06006779 RID: 26489 RVA: 0x0029F284 File Offset: 0x0029D484
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnSelectionChanged(XUiC_ListEntry<T> _previousEntry, XUiC_ListEntry<T> _newEntry)
	{
		if (!this.ignore_selection_change && this.SelectionChanged != null)
		{
			this.SelectionChanged(_previousEntry, _newEntry);
			if (!this.SelectableEntries)
			{
				this.ignore_selection_change = true;
				this.SelectedEntry = null;
				this.ignore_selection_change = false;
			}
		}
	}

	// Token: 0x0600677A RID: 26490 RVA: 0x0029F2C0 File Offset: 0x0029D4C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void goToPageOfCurrentlySelectedEntry()
	{
		if (this.CurrentSelectedEntry != null)
		{
			T currentSelectedEntry = this.CurrentSelectedEntry;
			bool flag = false;
			for (int i = 0; i < this.filteredEntries.Count; i++)
			{
				if (this.filteredEntries[i] == this.CurrentSelectedEntry)
				{
					this.Page = i / this.PageLength;
					flag = true;
					break;
				}
			}
			this.CurrentSelectedEntry = (flag ? currentSelectedEntry : default(T));
		}
	}

	// Token: 0x0600677B RID: 26491 RVA: 0x0029F340 File Offset: 0x0029D540
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateCurrentPageContents()
	{
		if (this.filteredEntries == null)
		{
			Log.Error("filteredEntries is null!");
			return;
		}
		for (int i = 0; i < this.PageLength; i++)
		{
			int num = i + this.PageLength * this.page;
			XUiC_ListEntry<T> xuiC_ListEntry = this.listEntryControllers[i];
			if (xuiC_ListEntry == null)
			{
				Log.Error("listEntry is null! {0} items in listEntryControllers", new object[]
				{
					this.listEntryControllers.Length
				});
				return;
			}
			if (num < this.filteredEntries.Count)
			{
				xuiC_ListEntry.SetEntry(this.filteredEntries[num]);
			}
			else
			{
				xuiC_ListEntry.SetEntry(default(T));
				if (xuiC_ListEntry.Selected)
				{
					xuiC_ListEntry.Selected = false;
				}
			}
			if (!this.updateSelectedItemByIndex && this.CurrentSelectedEntry != null && this.CurrentSelectedEntry == xuiC_ListEntry.GetEntry() && this.SelectedEntry != xuiC_ListEntry)
			{
				this.SelectedEntry = xuiC_ListEntry;
			}
		}
	}

	// Token: 0x0600677C RID: 26492 RVA: 0x0029F431 File Offset: 0x0029D631
	[PublicizedFrom(EAccessModifier.Private)]
	public void updatePageLabel()
	{
		if (this.pager != null)
		{
			this.pager.CurrentPageNumber = this.page;
			this.pager.LastPageNumber = this.LastPage;
		}
	}

	// Token: 0x0600677D RID: 26493 RVA: 0x0029F460 File Offset: 0x0029D660
	public override void Update(float _dt)
	{
		if (this.SelectableEntries && this.CursorControllable && this.columns > 0 && this.rows > 0 && PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
		{
			PlayerActionsGUI guiactions = this.windowGroup.playerUI.playerInput.GUIActions;
			if (guiactions.Left.WasPressed)
			{
				this.SelectedEntryIndex = Math.Max(0, this.SelectedEntryIndex - this.rows);
			}
			if (guiactions.Right.WasPressed)
			{
				this.SelectedEntryIndex = Math.Min(this.EntryCount - 1, this.SelectedEntryIndex + this.rows);
			}
			if (guiactions.Up.WasPressed)
			{
				this.SelectedEntryIndex = Math.Max(0, this.SelectedEntryIndex - 1);
			}
			if (guiactions.Down.WasPressed)
			{
				this.SelectedEntryIndex = Math.Min(this.EntryCount - 1, this.SelectedEntryIndex + 1);
			}
		}
		if (this.IsDirty)
		{
			this.goToPageOfCurrentlySelectedEntry();
			if (this.page > this.LastPage)
			{
				this.Page = this.LastPage;
			}
			this.updateCurrentPageContents();
			if (this.SelectedEntry != null && this.SelectedEntry.GetEntry() != this.CurrentSelectedEntry)
			{
				this.ClearSelection();
			}
			this.updatePageLabel();
			XUiEvent_PageContentsChangedEventHandler pageContentsChanged = this.PageContentsChanged;
			if (pageContentsChanged != null)
			{
				pageContentsChanged();
			}
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x0600677E RID: 26494 RVA: 0x0029F5E8 File Offset: 0x0029D7E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "hasselection")
		{
			XUiC_ListEntry<T> xuiC_ListEntry = this.SelectedEntry;
			_value = (((xuiC_ListEntry != null) ? xuiC_ListEntry.GetEntry() : default(T)) != null).ToString();
			return true;
		}
		if (_bindingName == "entrycounttotal")
		{
			_value = this.allEntries.Count.ToString();
			return true;
		}
		if (!(_bindingName == "entrycountfiltered"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = this.filteredEntries.Count.ToString();
		return true;
	}

	// Token: 0x0600677F RID: 26495 RVA: 0x0029F684 File Offset: 0x0029D884
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (base.ParseAttribute(_name, _value, _parent))
		{
			return true;
		}
		if (!(_name == "selectable"))
		{
			if (!(_name == "clear_selection_on_open"))
			{
				if (!(_name == "clear_searchtext_on_open"))
				{
					if (!(_name == "cursor_controllable"))
					{
						return false;
					}
					this.CursorControllable = StringParsers.ParseBool(_value, 0, -1, true);
				}
				else
				{
					this.ClearSearchTextOnOpenClose = StringParsers.ParseBool(_value, 0, -1, true);
				}
			}
			else
			{
				this.ClearSelectionOnOpenClose = StringParsers.ParseBool(_value, 0, -1, true);
			}
		}
		else
		{
			this.SelectableEntries = StringParsers.ParseBool(_value, 0, -1, true);
		}
		return true;
	}

	// Token: 0x06006780 RID: 26496 RVA: 0x0029F71C File Offset: 0x0029D91C
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.ClearSelectionOnOpenClose)
		{
			this.ClearSelection();
		}
		if (this.ClearSearchTextOnOpenClose && this.searchBox != null)
		{
			this.searchBox.Text = string.Empty;
			this.OnSearchInputChanged(this, string.Empty, true);
		}
		this.IsDirty = true;
	}

	// Token: 0x06006781 RID: 26497 RVA: 0x0029F771 File Offset: 0x0029D971
	public override void OnClose()
	{
		base.OnClose();
		if (this.ClearSelectionOnOpenClose)
		{
			this.ClearSelection();
		}
		if (this.ClearSearchTextOnOpenClose && this.searchBox != null)
		{
			this.searchBox.Text = "";
		}
	}

	// Token: 0x06006782 RID: 26498 RVA: 0x0029F7A7 File Offset: 0x0029D9A7
	public void ClearSelection()
	{
		this.SelectedEntry = null;
	}

	// Token: 0x06006783 RID: 26499 RVA: 0x0029F7B0 File Offset: 0x0029D9B0
	public virtual void OnListEntryClicked(XUiC_ListEntry<T> _entry)
	{
		XUiEvent_ListEntryClickedEventHandler<T> listEntryClicked = this.ListEntryClicked;
		if (listEntryClicked == null)
		{
			return;
		}
		listEntryClicked(_entry);
	}

	// Token: 0x06006784 RID: 26500 RVA: 0x0029F7C3 File Offset: 0x0029D9C3
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_List()
	{
	}

	// Token: 0x04004E07 RID: 19975
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<T> filteredEntries = new List<T>();

	// Token: 0x04004E08 RID: 19976
	[PublicizedFrom(EAccessModifier.Protected)]
	public T CurrentSelectedEntry;

	// Token: 0x04004E09 RID: 19977
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_ListEntry<T>[] listEntryControllers;

	// Token: 0x04004E0A RID: 19978
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_ListEntry<T> selectedEntry;

	// Token: 0x04004E0B RID: 19979
	[PublicizedFrom(EAccessModifier.Protected)]
	public int selectedEntryIndex = -1;

	// Token: 0x04004E0C RID: 19980
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool updateSelectedItemByIndex;

	// Token: 0x04004E0D RID: 19981
	[PublicizedFrom(EAccessModifier.Protected)]
	public int page;

	// Token: 0x04004E0E RID: 19982
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_TextInput searchBox;

	// Token: 0x04004E0F RID: 19983
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_Paging pager;

	// Token: 0x04004E10 RID: 19984
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly List<T> allEntries = new List<T>();

	// Token: 0x04004E15 RID: 19989
	[PublicizedFrom(EAccessModifier.Private)]
	public int columns;

	// Token: 0x04004E16 RID: 19990
	[PublicizedFrom(EAccessModifier.Private)]
	public int rows;

	// Token: 0x04004E17 RID: 19991
	[PublicizedFrom(EAccessModifier.Protected)]
	public string previousMatch = "";

	// Token: 0x04004E18 RID: 19992
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ignore_selection_change;
}
