using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E19 RID: 3609
[Preserve]
public class XUiC_ServersList : XUiC_List<XUiC_ServersList.ListEntry>
{
	// Token: 0x140000BF RID: 191
	// (add) Token: 0x06007100 RID: 28928 RVA: 0x002E0D38 File Offset: 0x002DEF38
	// (remove) Token: 0x06007101 RID: 28929 RVA: 0x002E0D70 File Offset: 0x002DEF70
	public event XUiEvent_OnPressEventHandler OnEntryDoubleClicked;

	// Token: 0x140000C0 RID: 192
	// (add) Token: 0x06007102 RID: 28930 RVA: 0x002E0DA8 File Offset: 0x002DEFA8
	// (remove) Token: 0x06007103 RID: 28931 RVA: 0x002E0DE0 File Offset: 0x002DEFE0
	public event Action<int> OnFilterResultsChanged;

	// Token: 0x140000C1 RID: 193
	// (add) Token: 0x06007104 RID: 28932 RVA: 0x002E0E18 File Offset: 0x002DF018
	// (remove) Token: 0x06007105 RID: 28933 RVA: 0x002E0E50 File Offset: 0x002DF050
	public event Action CountsChanged;

	// Token: 0x17000B5F RID: 2911
	// (get) Token: 0x06007106 RID: 28934 RVA: 0x002E0E85 File Offset: 0x002DF085
	public XUiC_ServersList.EnumServerLists CurrentServerTypeList
	{
		get
		{
			return this.currentServerTypeList;
		}
	}

	// Token: 0x06007107 RID: 28935 RVA: 0x002E0E90 File Offset: 0x002DF090
	public override void Init()
	{
		base.Init();
		this.sortButtons = new XUiController[5];
		for (int i = 0; i < 5; i++)
		{
			this.sortButtons[i] = base.GetChildById("serverlistheader").GetChildById(((XUiC_ServersList.EnumColumns)i).ToStringCached<XUiC_ServersList.EnumColumns>());
			if (this.sortButtons[i] != null)
			{
				this.sortButtons[i].ViewComponent.Value = i.ToString();
				this.sortButtons[i].OnPress += this.SortButton_OnPress;
				this.sortButtons[i].OnHover += this.SortButton_OnHover;
			}
		}
		for (int j = 0; j < this.listEntryControllers.Length; j++)
		{
			XUiC_ListEntry<XUiC_ServersList.ListEntry> xuiC_ListEntry = this.listEntryControllers[j];
			xuiC_ListEntry.OnDoubleClick += this.EntryDoubleClicked;
			XUiV_Button xuiV_Button = (XUiV_Button)xuiC_ListEntry.GetChildById("favorite").ViewComponent;
			xuiV_Button.Value = j.ToString();
			xuiV_Button.Controller.OnPress += delegate(XUiController _sender, int _args)
			{
				int num = StringParsers.ParseSInt32(_sender.ViewComponent.Value, 0, -1, NumberStyles.Integer);
				XUiC_ListEntry<XUiC_ServersList.ListEntry> xuiC_ListEntry2 = this.listEntryControllers[num];
				GameServerInfo gameServerInfo = xuiC_ListEntry2.GetEntry().gameServerInfo;
				this.AddRemoveServerCount(gameServerInfo, false);
				ServerInfoCache.Instance.ToggleFavorite(gameServerInfo);
				this.AddRemoveServerCount(gameServerInfo, true);
				xuiC_ListEntry2.IsDirty = true;
			};
		}
	}

	// Token: 0x06007108 RID: 28936 RVA: 0x002E0F92 File Offset: 0x002DF192
	public override void OnOpen()
	{
		base.OnOpen();
		this.RebuildList(false);
		this.stopListUpdateThread = false;
		ThreadManager.StartThread("ServerBrowserListUpdater", null, new ThreadManager.ThreadFunctionLoopDelegate(this.currentListUpdateThread), null, null, null, false, false);
	}

	// Token: 0x06007109 RID: 28937 RVA: 0x002E0FC5 File Offset: 0x002DF1C5
	public override void OnClose()
	{
		base.OnClose();
		this.stopListUpdateThread = true;
	}

	// Token: 0x0600710A RID: 28938 RVA: 0x002E0FD4 File Offset: 0x002DF1D4
	public override void Update(float _dt)
	{
		bool flag = false;
		if (this.pageUpdateRequired && (double)(Time.realtimeSinceStartup - this.lastPageUpdate) > 0.1)
		{
			flag = true;
			this.pageUpdateRequired = false;
			this.lastPageUpdate = Time.realtimeSinceStartup;
			this.IsDirty = true;
		}
		base.Update(_dt);
		if (flag)
		{
			Action<int> onFilterResultsChanged = this.OnFilterResultsChanged;
			if (onFilterResultsChanged == null)
			{
				return;
			}
			onFilterResultsChanged(base.EntryCount);
		}
	}

	// Token: 0x0600710B RID: 28939 RVA: 0x002E1040 File Offset: 0x002DF240
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		Dictionary<XUiC_ServersList.EnumServerLists, int> obj = this.serverCounts;
		lock (obj)
		{
			this.serverCounts.Clear();
		}
		base.RebuildList(_resetFilter);
	}

	// Token: 0x0600710C RID: 28940 RVA: 0x002E1098 File Offset: 0x002DF298
	public void RefreshBindingListEntry(XUiC_ServersList.ListEntry entry)
	{
		foreach (XUiC_ListEntry<XUiC_ServersList.ListEntry> xuiC_ListEntry in this.listEntryControllers)
		{
			if (xuiC_ListEntry.GetEntry() == entry)
			{
				xuiC_ListEntry.RefreshBindings(false);
				return;
			}
		}
	}

	// Token: 0x0600710D RID: 28941 RVA: 0x00002914 File Offset: 0x00000B14
	public override void RefreshView(bool _resetFilter = false, bool _resetPage = true)
	{
	}

	// Token: 0x0600710E RID: 28942 RVA: 0x002E10D0 File Offset: 0x002DF2D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnSearchInputChanged(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (!string.IsNullOrEmpty(_text))
		{
			this.SetFilter(new XUiC_ServersList.UiServerFilter("servername", XUiC_ServersList.EnumServerLists.All, (XUiC_ServersList.ListEntry _entry) => _entry.MatchesSearch(_text), IServerListInterface.ServerFilter.EServerFilterType.Any, 0, 0, false, null));
		}
		else
		{
			this.SetFilter(new XUiC_ServersList.UiServerFilter("servername", XUiC_ServersList.EnumServerLists.All, null, IServerListInterface.ServerFilter.EServerFilterType.Any, 0, 0, false, null));
		}
		this.updateCurrentList = true;
		this.IsDirty = true;
	}

	// Token: 0x0600710F RID: 28943 RVA: 0x002E1142 File Offset: 0x002DF342
	[PublicizedFrom(EAccessModifier.Private)]
	public void EntryDoubleClicked(XUiController _sender, int _mouseButton)
	{
		XUiEvent_OnPressEventHandler onEntryDoubleClicked = this.OnEntryDoubleClicked;
		if (onEntryDoubleClicked == null)
		{
			return;
		}
		onEntryDoubleClicked(_sender, _mouseButton);
	}

	// Token: 0x06007110 RID: 28944 RVA: 0x002E1156 File Offset: 0x002DF356
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "icon_enabled_color")
		{
			XUiC_ServersList.iconEnabledColor = _value;
			return true;
		}
		if (!(_name == "icon_disabled_color"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		XUiC_ServersList.iconDisabledColor = _value;
		return true;
	}

	// Token: 0x06007111 RID: 28945 RVA: 0x002E1190 File Offset: 0x002DF390
	public void SetServerTypeFilter(XUiC_ServersList.EnumServerLists _typelist)
	{
		Func<XUiC_ServersList.ListEntry, bool> func;
		switch (_typelist)
		{
		case XUiC_ServersList.EnumServerLists.Dedicated:
			func = ((XUiC_ServersList.ListEntry _line) => _line.gameServerInfo.IsDedicated);
			goto IL_D8;
		case XUiC_ServersList.EnumServerLists.Peer:
			func = ((XUiC_ServersList.ListEntry _line) => _line.gameServerInfo.IsPeerToPeer);
			goto IL_D8;
		case XUiC_ServersList.EnumServerLists.Regular:
			break;
		case XUiC_ServersList.EnumServerLists.Friends:
			func = ((XUiC_ServersList.ListEntry _line) => _line.gameServerInfo.IsFriends);
			goto IL_D8;
		default:
			if (_typelist == XUiC_ServersList.EnumServerLists.History)
			{
				func = ((XUiC_ServersList.ListEntry _line) => _line.gameServerInfo.IsFavoriteHistory);
				goto IL_D8;
			}
			if (_typelist == XUiC_ServersList.EnumServerLists.LAN)
			{
				func = ((XUiC_ServersList.ListEntry _line) => _line.gameServerInfo.IsLAN);
				goto IL_D8;
			}
			break;
		}
		func = null;
		IL_D8:
		Func<XUiC_ServersList.ListEntry, bool> func2 = func;
		this.currentServerTypeList = _typelist;
		this.SetFilter(new XUiC_ServersList.UiServerFilter("servertype", XUiC_ServersList.EnumServerLists.All, func2, IServerListInterface.ServerFilter.EServerFilterType.Any, 0, 0, false, null));
		base.ClearSelection();
	}

	// Token: 0x06007112 RID: 28946 RVA: 0x002E129C File Offset: 0x002DF49C
	public void SetFilter(XUiC_ServersList.UiServerFilter _filter)
	{
		Dictionary<string, XUiC_ServersList.UiServerFilter> obj = this.currentFilters;
		lock (obj)
		{
			if (_filter.Func == null)
			{
				this.currentFilters.Remove(_filter.Name);
			}
			else
			{
				this.currentFilters[_filter.Name] = _filter;
			}
		}
		this.updateCurrentList = true;
	}

	// Token: 0x06007113 RID: 28947 RVA: 0x002E130C File Offset: 0x002DF50C
	public int GetActiveFilterCount()
	{
		Dictionary<string, XUiC_ServersList.UiServerFilter> obj = this.currentFilters;
		int result;
		lock (obj)
		{
			result = (this.currentFilters.ContainsKey("servertype") ? (this.currentFilters.Count - 1) : this.currentFilters.Count);
		}
		return result;
	}

	// Token: 0x06007114 RID: 28948 RVA: 0x002E1374 File Offset: 0x002DF574
	public int GetServerCount(XUiC_ServersList.EnumServerLists _list)
	{
		Dictionary<XUiC_ServersList.EnumServerLists, int> obj = this.serverCounts;
		int result;
		lock (obj)
		{
			this.serverCounts.TryGetValue(_list, out result);
		}
		return result;
	}

	// Token: 0x06007115 RID: 28949 RVA: 0x002E13C0 File Offset: 0x002DF5C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateListFiltering(ref IEnumerable<XUiC_ServersList.ListEntry> _list)
	{
		Dictionary<string, XUiC_ServersList.UiServerFilter> obj = this.currentFilters;
		lock (obj)
		{
			foreach (KeyValuePair<string, XUiC_ServersList.UiServerFilter> keyValuePair in this.currentFilters)
			{
				if ((this.currentServerTypeList & keyValuePair.Value.ApplyingLists) != XUiC_ServersList.EnumServerLists.None)
				{
					_list = _list.Where(keyValuePair.Value.Func);
				}
			}
		}
	}

	// Token: 0x06007116 RID: 28950 RVA: 0x002E1460 File Offset: 0x002DF660
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateListSorting(ref IEnumerable<XUiC_ServersList.ListEntry> _list)
	{
		if (this.currentServerTypeList == XUiC_ServersList.EnumServerLists.History && this.sortFuncInt == null && this.sortFuncString == null)
		{
			_list = _list.OrderByDescending(this.sortDefaultFavHistory);
		}
		if (this.sortFuncInt != null)
		{
			_list = (this.sortAscending ? _list.OrderBy(this.sortFuncInt) : _list.OrderByDescending(this.sortFuncInt));
		}
		if (this.sortFuncString != null)
		{
			_list = (this.sortAscending ? _list.OrderBy(this.sortFuncString) : _list.OrderByDescending(this.sortFuncString));
		}
	}

	// Token: 0x06007117 RID: 28951 RVA: 0x002E14F4 File Offset: 0x002DF6F4
	[PublicizedFrom(EAccessModifier.Private)]
	public int indexOfGameServerInfo(GameServerInfo _gameServerInfo)
	{
		if (_gameServerInfo == null)
		{
			return -1;
		}
		int hashCode = this.uniqueIdComparer.GetHashCode(_gameServerInfo);
		int count = this.allEntries.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.uniqueIdComparer.GetHashCode(this.allEntries[i].gameServerInfo) == hashCode && this.uniqueIdComparer.Equals(_gameServerInfo, this.allEntries[i].gameServerInfo))
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06007118 RID: 28952 RVA: 0x002E156C File Offset: 0x002DF76C
	public bool AddGameServer(GameServerInfo _gameServerInfo, EServerRelationType _source)
	{
		int num = this.indexOfGameServerInfo(_gameServerInfo);
		bool result;
		if (num >= 0)
		{
			GameServerInfo gameServerInfo = this.allEntries[num].gameServerInfo;
			this.AddRemoveServerCount(gameServerInfo, false);
			gameServerInfo.Merge(_gameServerInfo, _source);
			this.AddRemoveServerCount(gameServerInfo, true);
			XUiC_ListEntry<XUiC_ServersList.ListEntry> xuiC_ListEntry = base.IsVisible(this.allEntries[num]);
			if (xuiC_ListEntry != null)
			{
				xuiC_ListEntry.IsDirty = true;
			}
			result = false;
		}
		else
		{
			List<XUiC_ServersList.ListEntry> allEntries = this.allEntries;
			lock (allEntries)
			{
				this.allEntries.Add(new XUiC_ServersList.ListEntry(_gameServerInfo, this));
			}
			this.AddRemoveServerCount(_gameServerInfo, true);
			result = true;
		}
		this.updateCurrentList = true;
		return result;
	}

	// Token: 0x06007119 RID: 28953 RVA: 0x002E1628 File Offset: 0x002DF828
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddRemoveServerCount(GameServerInfo _gsi, bool _add)
	{
		if (_gsi.IsDedicated)
		{
			this.addRemoveCountSingleListType(_gsi, _add, XUiC_ServersList.EnumServerLists.Dedicated);
		}
		if (_gsi.IsPeerToPeer)
		{
			this.addRemoveCountSingleListType(_gsi, _add, XUiC_ServersList.EnumServerLists.Peer);
		}
		if (_gsi.IsFriends)
		{
			this.addRemoveCountSingleListType(_gsi, _add, XUiC_ServersList.EnumServerLists.Friends);
		}
		if (_gsi.IsFavoriteHistory)
		{
			this.addRemoveCountSingleListType(_gsi, _add, XUiC_ServersList.EnumServerLists.History);
		}
		if (_gsi.IsLAN)
		{
			this.addRemoveCountSingleListType(_gsi, _add, XUiC_ServersList.EnumServerLists.LAN);
		}
		Action countsChanged = this.CountsChanged;
		if (countsChanged == null)
		{
			return;
		}
		countsChanged();
	}

	// Token: 0x0600711A RID: 28954 RVA: 0x002E169C File Offset: 0x002DF89C
	[PublicizedFrom(EAccessModifier.Private)]
	public void addRemoveCountSingleListType(GameServerInfo _gsi, bool _add, XUiC_ServersList.EnumServerLists _list)
	{
		Dictionary<XUiC_ServersList.EnumServerLists, int> obj = this.serverCounts;
		lock (obj)
		{
			int num;
			if (this.serverCounts.TryGetValue(_list, out num))
			{
				this.serverCounts[_list] = num + (_add ? 1 : -1);
			}
			else if (_add)
			{
				this.serverCounts[_list] = 1;
			}
		}
	}

	// Token: 0x0600711B RID: 28955 RVA: 0x002E1710 File Offset: 0x002DF910
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentListUpdateThread(ThreadManager.ThreadInfo _tInfo)
	{
		if (!this.stopListUpdateThread && !_tInfo.TerminationRequested())
		{
			if (this.updateCurrentList)
			{
				this.updateCurrentList = false;
				List<XUiC_ServersList.ListEntry> allEntries = this.allEntries;
				lock (allEntries)
				{
					this.filteredEntriesTmp.AddRange(this.allEntries);
				}
				IEnumerable<XUiC_ServersList.ListEntry> source = this.filteredEntriesTmp;
				this.updateListFiltering(ref source);
				this.updateListSorting(ref source);
				this.filteredEntriesTmp = source.ToList<XUiC_ServersList.ListEntry>();
				lock (this)
				{
					allEntries = this.filteredEntriesTmp;
					List<XUiC_ServersList.ListEntry> filteredEntries = this.filteredEntries;
					this.filteredEntries = allEntries;
					this.filteredEntriesTmp = filteredEntries;
				}
				this.pageUpdateRequired = true;
				this.filteredEntriesTmp.Clear();
			}
			return 50;
		}
		return -1;
	}

	// Token: 0x0600711C RID: 28956 RVA: 0x002E1800 File Offset: 0x002DFA00
	[PublicizedFrom(EAccessModifier.Private)]
	public void SortButton_OnHover(XUiController _sender, bool _isOver)
	{
		for (int i = 0; i < this.sortButtons.Length; i++)
		{
			if (_sender == this.sortButtons[i])
			{
				Color color = (this.sortColumn == (XUiC_ServersList.EnumColumns)i) ? new Color32(222, 206, 163, byte.MaxValue) : (_isOver ? new Color32(250, byte.MaxValue, 163, byte.MaxValue) : Color.white);
				XUiView viewComponent = _sender.ViewComponent;
				XUiV_Label xuiV_Label = viewComponent as XUiV_Label;
				if (xuiV_Label == null)
				{
					XUiV_Sprite xuiV_Sprite = viewComponent as XUiV_Sprite;
					if (xuiV_Sprite != null)
					{
						xuiV_Sprite.Color = color;
					}
				}
				else
				{
					xuiV_Label.Color = color;
				}
			}
		}
	}

	// Token: 0x0600711D RID: 28957 RVA: 0x002E18B8 File Offset: 0x002DFAB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SortButton_OnPress(XUiController _sender, int _mouseButton)
	{
		for (int i = 0; i < this.sortButtons.Length; i++)
		{
			if (_sender == this.sortButtons[i])
			{
				this.updateSortType((XUiC_ServersList.EnumColumns)i);
				for (int j = 0; j < this.sortButtons.Length; j++)
				{
					if (this.sortButtons[j] != null)
					{
						this.SortButton_OnHover(this.sortButtons[j], i == j);
					}
				}
				return;
			}
		}
	}

	// Token: 0x0600711E RID: 28958 RVA: 0x002E191C File Offset: 0x002DFB1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateSortType(XUiC_ServersList.EnumColumns _sortType)
	{
		switch (_sortType)
		{
		case XUiC_ServersList.EnumColumns.ServerName:
			this.sortFuncString = ((XUiC_ServersList.ListEntry _line) => _line.gameServerInfo.GetValue(GameInfoString.GameHost));
			this.sortFuncInt = null;
			break;
		case XUiC_ServersList.EnumColumns.World:
			this.sortFuncString = ((XUiC_ServersList.ListEntry _line) => _line.gameServerInfo.GetValue(GameInfoString.LevelName));
			this.sortFuncInt = null;
			break;
		case XUiC_ServersList.EnumColumns.Difficulty:
			this.sortFuncString = null;
			this.sortFuncInt = ((XUiC_ServersList.ListEntry _line) => _line.gameServerInfo.GetValue(GameInfoInt.GameDifficulty));
			break;
		case XUiC_ServersList.EnumColumns.Players:
			this.sortFuncString = null;
			this.sortFuncInt = ((XUiC_ServersList.ListEntry _line) => _line.gameServerInfo.GetValue(GameInfoInt.CurrentPlayers));
			break;
		case XUiC_ServersList.EnumColumns.Ping:
			this.sortFuncString = null;
			this.sortFuncInt = ((XUiC_ServersList.ListEntry _line) => _line.gameServerInfo.GetValue(GameInfoInt.Ping));
			break;
		default:
			this.sortFuncString = null;
			this.sortFuncInt = null;
			break;
		}
		if (this.sortColumn == _sortType)
		{
			if (!this.sortAscending)
			{
				this.sortColumn = XUiC_ServersList.EnumColumns.Count;
				this.sortFuncString = null;
				this.sortFuncInt = null;
				return;
			}
			this.sortAscending = false;
		}
		else
		{
			this.sortAscending = true;
		}
		this.sortColumn = _sortType;
		this.updateCurrentList = true;
	}

	// Token: 0x040055D7 RID: 21975
	[PublicizedFrom(EAccessModifier.Private)]
	public const string serverTypeFilterName = "servertype";

	// Token: 0x040055D8 RID: 21976
	[PublicizedFrom(EAccessModifier.Private)]
	public const string serverNameFilterName = "servername";

	// Token: 0x040055D9 RID: 21977
	[PublicizedFrom(EAccessModifier.Private)]
	public static string textEnabledColor = "255,255,255";

	// Token: 0x040055DA RID: 21978
	[PublicizedFrom(EAccessModifier.Private)]
	public static string textDisabledColor = "160,160,160";

	// Token: 0x040055DB RID: 21979
	[PublicizedFrom(EAccessModifier.Private)]
	public static string iconEnabledColor = "222,206,163";

	// Token: 0x040055DC RID: 21980
	[PublicizedFrom(EAccessModifier.Private)]
	public static string iconDisabledColor = "2,2,2,2";

	// Token: 0x040055E0 RID: 21984
	[PublicizedFrom(EAccessModifier.Private)]
	public bool stopListUpdateThread;

	// Token: 0x040055E1 RID: 21985
	[PublicizedFrom(EAccessModifier.Private)]
	public bool updateCurrentList;

	// Token: 0x040055E2 RID: 21986
	[PublicizedFrom(EAccessModifier.Private)]
	public bool pageUpdateRequired;

	// Token: 0x040055E3 RID: 21987
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastPageUpdate;

	// Token: 0x040055E4 RID: 21988
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_ServersList.ListEntry> filteredEntriesTmp = new List<XUiC_ServersList.ListEntry>();

	// Token: 0x040055E5 RID: 21989
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController[] sortButtons;

	// Token: 0x040055E6 RID: 21990
	[PublicizedFrom(EAccessModifier.Private)]
	public bool sortAscending = true;

	// Token: 0x040055E7 RID: 21991
	[PublicizedFrom(EAccessModifier.Private)]
	public Func<XUiC_ServersList.ListEntry, string> sortFuncString;

	// Token: 0x040055E8 RID: 21992
	[PublicizedFrom(EAccessModifier.Private)]
	public Func<XUiC_ServersList.ListEntry, int> sortFuncInt;

	// Token: 0x040055E9 RID: 21993
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Func<XUiC_ServersList.ListEntry, int> sortDefaultFavHistory = delegate(XUiC_ServersList.ListEntry _line)
	{
		if (!_line.gameServerInfo.IsFavorite)
		{
			return _line.gameServerInfo.LastPlayedLinux;
		}
		return int.MaxValue;
	};

	// Token: 0x040055EA RID: 21994
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServersList.EnumServerLists currentServerTypeList;

	// Token: 0x040055EB RID: 21995
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, XUiC_ServersList.UiServerFilter> currentFilters = new Dictionary<string, XUiC_ServersList.UiServerFilter>();

	// Token: 0x040055EC RID: 21996
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly GameServerInfo.UniqueIdEqualityComparer uniqueIdComparer = GameServerInfo.UniqueIdEqualityComparer.Instance;

	// Token: 0x040055ED RID: 21997
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<XUiC_ServersList.EnumServerLists, int> serverCounts = new EnumDictionary<XUiC_ServersList.EnumServerLists, int>();

	// Token: 0x040055EE RID: 21998
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServersList.EnumColumns sortColumn = XUiC_ServersList.EnumColumns.Count;

	// Token: 0x02000E1A RID: 3610
	[Preserve]
	public class ListEntry : XUiListEntry<XUiC_ServersList.ListEntry>
	{
		// Token: 0x06007122 RID: 28962 RVA: 0x002E1B7A File Offset: 0x002DFD7A
		public ListEntry(GameServerInfo _serverInfo, XUiC_ServersList _serversList)
		{
			this.gameServerInfo = _serverInfo;
			this.serversList = _serversList;
		}

		// Token: 0x06007123 RID: 28963 RVA: 0x002E1B90 File Offset: 0x002DFD90
		public override int CompareTo(XUiC_ServersList.ListEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return 1;
			}
			return this.gameServerInfo.LastPlayedLinux.CompareTo(_otherEntry.gameServerInfo.LastPlayedLinux);
		}

		// Token: 0x06007124 RID: 28964 RVA: 0x002E1BC0 File Offset: 0x002DFDC0
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 933488787U)
			{
				if (num <= 646081446U)
				{
					if (num != 164617456U)
					{
						if (num != 375255177U)
						{
							if (num == 646081446U)
							{
								if (_bindingName == "playersonline")
								{
									_value = this.gameServerInfo.GetValue(GameInfoInt.CurrentPlayers).ToString();
									return true;
								}
							}
						}
						else if (_bindingName == "ping")
						{
							int value = this.gameServerInfo.GetValue(GameInfoInt.Ping);
							_value = ((value >= 0) ? value.ToString() : "N/A");
							return true;
						}
					}
					else if (_bindingName == "difficulty")
					{
						_value = this.gameServerInfo.GetValue(GameInfoInt.GameDifficulty).ToString();
						return true;
					}
				}
				else if (num <= 774435485U)
				{
					if (num != 724407982U)
					{
						if (num == 774435485U)
						{
							if (_bindingName == "playersmax")
							{
								_value = this.gameServerInfo.GetValue(GameInfoInt.MaxPlayers).ToString();
								return true;
							}
						}
					}
					else if (_bindingName == "servericonatlas")
					{
						_value = "SymbolAtlas";
						return true;
					}
				}
				else if (num != 796832095U)
				{
					if (num == 933488787U)
					{
						if (_bindingName == "world")
						{
							_value = GeneratedTextManager.GetDisplayTextImmediately(this.gameServerInfo.ServerWorldName, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.NotSupported);
							if (!GeneratedTextManager.IsFiltering(this.gameServerInfo.ServerWorldName) && !GeneratedTextManager.IsFiltered(this.gameServerInfo.ServerWorldName))
							{
								GeneratedTextManager.GetDisplayText(this.gameServerInfo.ServerWorldName, delegate(string _)
								{
									this.serversList.RefreshBindingListEntry(this);
								}, false, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.NotSupported);
							}
							return true;
						}
					}
				}
				else if (_bindingName == "servericon")
				{
					_value = PlatformManager.NativePlatform.Utils.GetCrossplayPlayerIcon(this.gameServerInfo.PlayGroup, true, EPlatformIdentifier.None);
					return true;
				}
			}
			else if (num <= 1820482109U)
			{
				if (num <= 1346672274U)
				{
					if (num != 1244254369U)
					{
						if (num == 1346672274U)
						{
							if (_bindingName == "isdedicated")
							{
								_value = this.gameServerInfo.IsDedicated.ToString();
								return true;
							}
						}
					}
					else if (_bindingName == "textcolor")
					{
						_value = (this.gameServerInfo.IsCompatibleVersion ? XUiC_ServersList.textEnabledColor : XUiC_ServersList.textDisabledColor);
						return true;
					}
				}
				else if (num != 1566407741U)
				{
					if (num == 1820482109U)
					{
						if (_bindingName == "isfavorite")
						{
							_value = this.gameServerInfo.IsFavorite.ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "hasentry")
				{
					_value = true.ToString();
					return true;
				}
			}
			else if (num <= 3175373231U)
			{
				if (num != 2675616140U)
				{
					if (num == 3175373231U)
					{
						if (_bindingName == "passwordcolor")
						{
							_value = (this.gameServerInfo.GetValue(GameInfoBool.IsPasswordProtected) ? XUiC_ServersList.iconEnabledColor : XUiC_ServersList.iconDisabledColor);
							return true;
						}
					}
				}
				else if (_bindingName == "pingcolor")
				{
					_value = ((this.gameServerInfo.IsCompatibleVersion && this.gameServerInfo.GetValue(GameInfoInt.Ping) >= 0) ? XUiC_ServersList.textEnabledColor : XUiC_ServersList.textDisabledColor);
					return true;
				}
			}
			else if (num != 3810676121U)
			{
				if (num == 4010301179U)
				{
					if (_bindingName == "anticheatcolor")
					{
						_value = (this.gameServerInfo.EACEnabled ? XUiC_ServersList.iconEnabledColor : XUiC_ServersList.iconDisabledColor);
						return true;
					}
				}
			}
			else if (_bindingName == "servername")
			{
				_value = GeneratedTextManager.GetDisplayTextImmediately(this.gameServerInfo.ServerDisplayName, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.NotSupported);
				if (!GeneratedTextManager.IsFiltering(this.gameServerInfo.ServerDisplayName) && !GeneratedTextManager.IsFiltered(this.gameServerInfo.ServerDisplayName))
				{
					GeneratedTextManager.GetDisplayText(this.gameServerInfo.ServerDisplayName, delegate(string _)
					{
						this.serversList.RefreshBindingListEntry(this);
					}, false, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.NotSupported);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06007125 RID: 28965 RVA: 0x002E2010 File Offset: 0x002E0210
		public override bool MatchesSearch(string _searchString)
		{
			return this.gameServerInfo.GetValue(GameInfoString.GameHost).ContainsCaseInsensitive(_searchString);
		}

		// Token: 0x06007126 RID: 28966 RVA: 0x002E2024 File Offset: 0x002E0224
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num > 933488787U)
			{
				if (num <= 1820482109U)
				{
					if (num <= 1346672274U)
					{
						if (num != 1244254369U)
						{
							if (num != 1346672274U)
							{
								return false;
							}
							if (!(_bindingName == "isdedicated"))
							{
								return false;
							}
						}
						else
						{
							if (!(_bindingName == "textcolor"))
							{
								return false;
							}
							goto IL_213;
						}
					}
					else if (num != 1566407741U)
					{
						if (num != 1820482109U)
						{
							return false;
						}
						if (!(_bindingName == "isfavorite"))
						{
							return false;
						}
					}
					else if (!(_bindingName == "hasentry"))
					{
						return false;
					}
					_value = false.ToString();
					return true;
				}
				if (num <= 3175373231U)
				{
					if (num != 2675616140U)
					{
						if (num != 3175373231U)
						{
							return false;
						}
						if (!(_bindingName == "passwordcolor"))
						{
							return false;
						}
					}
					else if (!(_bindingName == "pingcolor"))
					{
						return false;
					}
				}
				else if (num != 3810676121U)
				{
					if (num != 4010301179U)
					{
						return false;
					}
					if (!(_bindingName == "anticheatcolor"))
					{
						return false;
					}
				}
				else
				{
					if (!(_bindingName == "servername"))
					{
						return false;
					}
					goto IL_20A;
				}
				IL_213:
				_value = "0,0,0";
				return true;
			}
			if (num <= 646081446U)
			{
				if (num != 164617456U)
				{
					if (num != 375255177U)
					{
						if (num != 646081446U)
						{
							return false;
						}
						if (!(_bindingName == "playersonline"))
						{
							return false;
						}
					}
					else if (!(_bindingName == "ping"))
					{
						return false;
					}
				}
				else if (!(_bindingName == "difficulty"))
				{
					return false;
				}
			}
			else if (num <= 774435485U)
			{
				if (num != 724407982U)
				{
					if (num != 774435485U)
					{
						return false;
					}
					if (!(_bindingName == "playersmax"))
					{
						return false;
					}
				}
				else if (!(_bindingName == "servericonatlas"))
				{
					return false;
				}
			}
			else if (num != 796832095U)
			{
				if (num != 933488787U)
				{
					return false;
				}
				if (!(_bindingName == "world"))
				{
					return false;
				}
			}
			else if (!(_bindingName == "servericon"))
			{
				return false;
			}
			IL_20A:
			_value = "";
			return true;
		}

		// Token: 0x040055EF RID: 21999
		public readonly GameServerInfo gameServerInfo;

		// Token: 0x040055F0 RID: 22000
		[PublicizedFrom(EAccessModifier.Private)]
		public XUiC_ServersList serversList;
	}

	// Token: 0x02000E1B RID: 3611
	public class UiServerFilter : IServerListInterface.ServerFilter
	{
		// Token: 0x06007129 RID: 28969 RVA: 0x002E2269 File Offset: 0x002E0469
		public UiServerFilter(string _name, XUiC_ServersList.EnumServerLists _applyingTo, Func<XUiC_ServersList.ListEntry, bool> _func = null, IServerListInterface.ServerFilter.EServerFilterType _type = IServerListInterface.ServerFilter.EServerFilterType.Any, int _intMinValue = 0, int _intMaxValue = 0, bool _boolValue = false, string _stringNeedle = null) : base(_name, _type, _intMinValue, _intMaxValue, _boolValue, _stringNeedle)
		{
			this.Func = _func;
			this.ApplyingLists = _applyingTo;
		}

		// Token: 0x040055F1 RID: 22001
		public readonly Func<XUiC_ServersList.ListEntry, bool> Func;

		// Token: 0x040055F2 RID: 22002
		public readonly XUiC_ServersList.EnumServerLists ApplyingLists;
	}

	// Token: 0x02000E1C RID: 3612
	[Flags]
	public enum EnumServerLists
	{
		// Token: 0x040055F4 RID: 22004
		None = 0,
		// Token: 0x040055F5 RID: 22005
		Dedicated = 1,
		// Token: 0x040055F6 RID: 22006
		Peer = 2,
		// Token: 0x040055F7 RID: 22007
		Friends = 4,
		// Token: 0x040055F8 RID: 22008
		History = 8,
		// Token: 0x040055F9 RID: 22009
		LAN = 16,
		// Token: 0x040055FA RID: 22010
		Regular = 3,
		// Token: 0x040055FB RID: 22011
		Special = 28,
		// Token: 0x040055FC RID: 22012
		All = 31
	}

	// Token: 0x02000E1D RID: 3613
	[PublicizedFrom(EAccessModifier.Private)]
	public enum EnumColumns
	{
		// Token: 0x040055FE RID: 22014
		ServerName,
		// Token: 0x040055FF RID: 22015
		World,
		// Token: 0x04005600 RID: 22016
		Difficulty,
		// Token: 0x04005601 RID: 22017
		Players,
		// Token: 0x04005602 RID: 22018
		Ping,
		// Token: 0x04005603 RID: 22019
		Count
	}
}
