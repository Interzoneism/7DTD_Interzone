using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000DEC RID: 3564
[Preserve]
public class XUiC_SavegamesList : XUiC_List<XUiC_SavegamesList.ListEntry>
{
	// Token: 0x140000BD RID: 189
	// (add) Token: 0x06006FB2 RID: 28594 RVA: 0x002D9364 File Offset: 0x002D7564
	// (remove) Token: 0x06006FB3 RID: 28595 RVA: 0x002D939C File Offset: 0x002D759C
	public event XUiEvent_OnPressEventHandler OnEntryDoubleClicked;

	// Token: 0x06006FB4 RID: 28596 RVA: 0x002D93D4 File Offset: 0x002D75D4
	public override void Init()
	{
		base.Init();
		XUiC_ListEntry<XUiC_SavegamesList.ListEntry>[] listEntryControllers = this.listEntryControllers;
		for (int i = 0; i < listEntryControllers.Length; i++)
		{
			XUiC_ListEntry<XUiC_SavegamesList.ListEntry> xuiC_ListEntry = listEntryControllers[i];
			xuiC_ListEntry.OnDoubleClick += delegate(XUiController _sender, int _mouseButton)
			{
				this.EntryDoubleClicked(_sender, _mouseButton, _sender.ViewComponent);
			};
			XUiC_ListEntry<XUiC_SavegamesList.ListEntry> closure = xuiC_ListEntry;
			XUiEvent_OnHoverEventHandler value = delegate(XUiController _controller, bool _isOver)
			{
				closure.ForceHovered = _isOver;
			};
			xuiC_ListEntry.GetChildById("Version").OnScroll += base.HandleOnScroll;
			xuiC_ListEntry.GetChildById("Version").OnPress += xuiC_ListEntry.XUiC_ListEntry_OnPress;
			xuiC_ListEntry.GetChildById("Version").OnDoubleClick += delegate(XUiController _sender, int _args)
			{
				this.EntryDoubleClicked(_sender, _args, _sender.Parent.ViewComponent);
			};
			xuiC_ListEntry.GetChildById("Version").OnHover += value;
			xuiC_ListEntry.GetChildById("Version").ViewComponent.IsSnappable = false;
			xuiC_ListEntry.GetChildById("World").OnScroll += base.HandleOnScroll;
			xuiC_ListEntry.GetChildById("World").OnPress += xuiC_ListEntry.XUiC_ListEntry_OnPress;
			xuiC_ListEntry.GetChildById("World").OnDoubleClick += delegate(XUiController _sender, int _args)
			{
				this.EntryDoubleClicked(_sender, _args, _sender.Parent.ViewComponent);
			};
			xuiC_ListEntry.GetChildById("World").OnHover += value;
			xuiC_ListEntry.GetChildById("World").ViewComponent.IsSnappable = false;
		}
	}

	// Token: 0x06006FB5 RID: 28597 RVA: 0x002D9526 File Offset: 0x002D7726
	public override void OnOpen()
	{
		base.OnOpen();
		this.RebuildList(false);
	}

	// Token: 0x06006FB6 RID: 28598 RVA: 0x002D9535 File Offset: 0x002D7735
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		GameIO.GetPlayerSaves(new GameIO.FoundSave(this.AddSaveToEntries), false);
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x06006FB7 RID: 28599 RVA: 0x002D9568 File Offset: 0x002D7768
	public bool SelectByName(string _name)
	{
		if (!string.IsNullOrEmpty(_name))
		{
			for (int i = 0; i < this.filteredEntries.Count; i++)
			{
				if (this.filteredEntries[i].saveName.Equals(_name, StringComparison.OrdinalIgnoreCase))
				{
					base.SelectedEntryIndex = i;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06006FB8 RID: 28600 RVA: 0x002D95B7 File Offset: 0x002D77B7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnSearchInputChanged(XUiController _sender, string _text, bool _changeFromCode)
	{
		base.OnSearchInputChanged(_sender, _text, _changeFromCode);
	}

	// Token: 0x06006FB9 RID: 28601 RVA: 0x002D95C2 File Offset: 0x002D77C2
	public void SetWorldFilter(string _worldName)
	{
		this.worldFilter = _worldName;
	}

	// Token: 0x06006FBA RID: 28602 RVA: 0x002D95CC File Offset: 0x002D77CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void FilterResults(string _textMatch)
	{
		base.FilterResults(_textMatch);
		if (this.worldFilter == null)
		{
			return;
		}
		for (int i = 0; i < this.filteredEntries.Count; i++)
		{
			if (this.filteredEntries[i].worldName != this.worldFilter)
			{
				this.filteredEntries.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x06006FBB RID: 28603 RVA: 0x002D962D File Offset: 0x002D782D
	public IEnumerable<XUiC_SavegamesList.ListEntry> GetSavesInWorld(string _worldName)
	{
		if (string.IsNullOrEmpty(_worldName))
		{
			yield break;
		}
		int num;
		for (int i = 0; i < this.allEntries.Count; i = num + 1)
		{
			if (this.allEntries[i].worldName == _worldName)
			{
				yield return this.allEntries[i];
			}
			num = i;
		}
		yield break;
	}

	// Token: 0x06006FBC RID: 28604 RVA: 0x002D9644 File Offset: 0x002D7844
	public void SelectEntry(string worldName, string saveName)
	{
		if (this.filteredEntries == null)
		{
			Log.Error("filteredEntries is null");
			return;
		}
		for (int i = 0; i < this.filteredEntries.Count; i++)
		{
			XUiC_SavegamesList.ListEntry listEntry = this.filteredEntries[i];
			if (listEntry.worldName.EqualsCaseInsensitive(worldName) && listEntry.saveName.EqualsCaseInsensitive(saveName))
			{
				base.SelectedEntryIndex = i;
				return;
			}
		}
	}

	// Token: 0x06006FBD RID: 28605 RVA: 0x002D96AB File Offset: 0x002D78AB
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddSaveToEntries(string saveName, string worldName, DateTime lastSaved, WorldState worldState, bool isArchived)
	{
		this.allEntries.Add(new XUiC_SavegamesList.ListEntry(saveName, worldName, lastSaved, worldState, this.matchingVersionColor, this.compatibleVersionColor, this.incompatibleVersionColor));
	}

	// Token: 0x06006FBE RID: 28606 RVA: 0x002D96D4 File Offset: 0x002D78D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void EntryDoubleClicked(XUiController _sender, int _mouseButton, XUiView _listEntryView)
	{
		if (!_listEntryView.Enabled)
		{
			return;
		}
		XUiEvent_OnPressEventHandler onEntryDoubleClicked = this.OnEntryDoubleClicked;
		if (onEntryDoubleClicked == null)
		{
			return;
		}
		onEntryDoubleClicked(_sender, _mouseButton);
	}

	// Token: 0x06006FBF RID: 28607 RVA: 0x002D96F4 File Offset: 0x002D78F4
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "matching_version_color")
		{
			this.matchingVersionColor = _value;
			return true;
		}
		if (_name == "compatible_version_color")
		{
			this.compatibleVersionColor = _value;
			return true;
		}
		if (!(_name == "incompatible_version_color"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.incompatibleVersionColor = _value;
		return true;
	}

	// Token: 0x040054C8 RID: 21704
	[PublicizedFrom(EAccessModifier.Private)]
	public string matchingVersionColor;

	// Token: 0x040054C9 RID: 21705
	[PublicizedFrom(EAccessModifier.Private)]
	public string compatibleVersionColor;

	// Token: 0x040054CA RID: 21706
	[PublicizedFrom(EAccessModifier.Private)]
	public string incompatibleVersionColor;

	// Token: 0x040054CC RID: 21708
	public string worldFilter;

	// Token: 0x02000DED RID: 3565
	[Preserve]
	public class ListEntry : XUiListEntry<XUiC_SavegamesList.ListEntry>
	{
		// Token: 0x17000B40 RID: 2880
		// (get) Token: 0x06006FC4 RID: 28612 RVA: 0x002D977B File Offset: 0x002D797B
		public GameMode gameMode
		{
			get
			{
				return GameMode.GetGameModeForId(this.worldState.activeGameMode);
			}
		}

		// Token: 0x06006FC5 RID: 28613 RVA: 0x002D9790 File Offset: 0x002D7990
		public ListEntry(string _saveName, string _worldName, DateTime _lastSaved, WorldState _worldState, string matchingColor = "255,255,255", string compatibleColor = "255,255,255", string incompatibleColor = "255,255,255")
		{
			this.saveName = _saveName;
			this.worldName = _worldName;
			this.lastSaved = _lastSaved;
			this.worldState = _worldState;
			this.AbstractedLocation = PathAbstractions.WorldsSearchPaths.GetLocation(this.worldName, _worldName, _saveName);
			this.versionComparison = this.worldState.gameVersion.CompareToRunningBuild();
			this.matchingColor = matchingColor;
			this.compatibleColor = compatibleColor;
			this.incompatibleColor = incompatibleColor;
		}

		// Token: 0x06006FC6 RID: 28614 RVA: 0x002D9808 File Offset: 0x002D7A08
		public override int CompareTo(XUiC_SavegamesList.ListEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return -1;
			}
			return -1 * this.lastSaved.CompareTo(_otherEntry.lastSaved);
		}

		// Token: 0x06006FC7 RID: 28615 RVA: 0x002D9830 File Offset: 0x002D7A30
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 1355330520U)
			{
				if (num <= 205488363U)
				{
					if (num != 8204530U)
					{
						if (num != 205488363U)
						{
							return false;
						}
						if (!(_bindingName == "savename"))
						{
							return false;
						}
						_value = this.saveName;
						return true;
					}
					else if (!(_bindingName == "entrycolor"))
					{
						return false;
					}
				}
				else if (num != 719397874U)
				{
					if (num != 1181855383U)
					{
						if (num != 1355330520U)
						{
							return false;
						}
						if (!(_bindingName == "worldname"))
						{
							return false;
						}
						_value = this.worldName;
						return true;
					}
					else
					{
						if (!(_bindingName == "version"))
						{
							return false;
						}
						if (this.worldState.gameVersion.Major >= 0)
						{
							_value = this.worldState.gameVersion.ShortString;
						}
						else
						{
							_value = this.worldState.gameVersionString;
						}
						return true;
					}
				}
				else
				{
					if (!(_bindingName == "worldcolor"))
					{
						return false;
					}
					_value = ((this.AbstractedLocation.Type != PathAbstractions.EAbstractedLocationType.None) ? "255,255,255,128" : "255,0,0");
					return true;
				}
			}
			else if (num <= 1871248802U)
			{
				if (num != 1566407741U)
				{
					if (num != 1800901934U)
					{
						if (num != 1871248802U)
						{
							return false;
						}
						if (!(_bindingName == "worldtooltip"))
						{
							return false;
						}
						_value = ((this.AbstractedLocation.Type != PathAbstractions.EAbstractedLocationType.None) ? "" : Localization.Get("xuiSavegameWorldNotFound", false));
						return true;
					}
					else
					{
						if (!(_bindingName == "lastplayed"))
						{
							return false;
						}
						_value = this.lastSaved.ToString("yyyy-MM-dd HH:mm");
						return true;
					}
				}
				else
				{
					if (!(_bindingName == "hasentry"))
					{
						return false;
					}
					_value = true.ToString();
					return true;
				}
			}
			else if (num != 2049496678U)
			{
				if (num != 3966689298U)
				{
					if (num != 4086844294U)
					{
						return false;
					}
					if (!(_bindingName == "versiontooltip"))
					{
						return false;
					}
					_value = ((this.versionComparison == VersionInformation.EVersionComparisonResult.SameBuild || this.versionComparison == VersionInformation.EVersionComparisonResult.SameMinor) ? "" : ((this.versionComparison == VersionInformation.EVersionComparisonResult.NewerMinor) ? Localization.Get("xuiSavegameNewerMinor", false) : ((this.versionComparison == VersionInformation.EVersionComparisonResult.OlderMinor) ? Localization.Get("xuiSavegameOlderMinor", false) : Localization.Get("xuiSavegameDifferentMajor", false))));
					return true;
				}
				else
				{
					if (!(_bindingName == "mode"))
					{
						return false;
					}
					GameMode gameMode = this.gameMode;
					if (gameMode == null)
					{
						_value = "-Unknown-";
					}
					else
					{
						string name = gameMode.GetName();
						_value = Localization.Get(name, false);
					}
					return true;
				}
			}
			else if (!(_bindingName == "versioncolor"))
			{
				return false;
			}
			_value = ((this.versionComparison == VersionInformation.EVersionComparisonResult.SameBuild || this.versionComparison == VersionInformation.EVersionComparisonResult.SameMinor) ? this.matchingColor : ((this.versionComparison == VersionInformation.EVersionComparisonResult.NewerMinor || this.versionComparison == VersionInformation.EVersionComparisonResult.OlderMinor) ? this.compatibleColor : this.incompatibleColor));
			return true;
		}

		// Token: 0x06006FC8 RID: 28616 RVA: 0x002D9B39 File Offset: 0x002D7D39
		public override bool MatchesSearch(string _searchString)
		{
			return this.saveName.ContainsCaseInsensitive(_searchString);
		}

		// Token: 0x06006FC9 RID: 28617 RVA: 0x002D9B48 File Offset: 0x002D7D48
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 1355330520U)
			{
				if (num <= 205488363U)
				{
					if (num != 8204530U)
					{
						if (num != 205488363U)
						{
							return false;
						}
						if (!(_bindingName == "savename"))
						{
							return false;
						}
					}
					else
					{
						if (!(_bindingName == "entrycolor"))
						{
							return false;
						}
						goto IL_160;
					}
				}
				else if (num != 719397874U)
				{
					if (num != 1181855383U)
					{
						if (num != 1355330520U)
						{
							return false;
						}
						if (!(_bindingName == "worldname"))
						{
							return false;
						}
					}
					else if (!(_bindingName == "version"))
					{
						return false;
					}
				}
				else
				{
					if (!(_bindingName == "worldcolor"))
					{
						return false;
					}
					goto IL_160;
				}
			}
			else if (num <= 1871248802U)
			{
				if (num != 1566407741U)
				{
					if (num != 1800901934U)
					{
						if (num != 1871248802U)
						{
							return false;
						}
						if (!(_bindingName == "worldtooltip"))
						{
							return false;
						}
					}
					else if (!(_bindingName == "lastplayed"))
					{
						return false;
					}
				}
				else
				{
					if (!(_bindingName == "hasentry"))
					{
						return false;
					}
					_value = false.ToString();
					return true;
				}
			}
			else if (num != 2049496678U)
			{
				if (num != 3966689298U)
				{
					if (num != 4086844294U)
					{
						return false;
					}
					if (!(_bindingName == "versiontooltip"))
					{
						return false;
					}
				}
				else if (!(_bindingName == "mode"))
				{
					return false;
				}
			}
			else
			{
				if (!(_bindingName == "versioncolor"))
				{
					return false;
				}
				goto IL_160;
			}
			_value = "";
			return true;
			IL_160:
			_value = "0,0,0";
			return true;
		}

		// Token: 0x040054CD RID: 21709
		public readonly string saveName;

		// Token: 0x040054CE RID: 21710
		public readonly string worldName;

		// Token: 0x040054CF RID: 21711
		public readonly DateTime lastSaved;

		// Token: 0x040054D0 RID: 21712
		public readonly WorldState worldState;

		// Token: 0x040054D1 RID: 21713
		public readonly PathAbstractions.AbstractedLocation AbstractedLocation;

		// Token: 0x040054D2 RID: 21714
		public readonly VersionInformation.EVersionComparisonResult versionComparison;

		// Token: 0x040054D3 RID: 21715
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string matchingColor;

		// Token: 0x040054D4 RID: 21716
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string compatibleColor;

		// Token: 0x040054D5 RID: 21717
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string incompatibleColor;
	}
}
