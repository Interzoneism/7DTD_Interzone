using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000EC8 RID: 3784
[Preserve]
public class XUiC_WorldList : XUiC_List<XUiC_WorldList.WorldListEntry>
{
	// Token: 0x140000D5 RID: 213
	// (add) Token: 0x060077A9 RID: 30633 RVA: 0x0030BC60 File Offset: 0x00309E60
	// (remove) Token: 0x060077AA RID: 30634 RVA: 0x0030BC98 File Offset: 0x00309E98
	public event XUiEvent_OnPressEventHandler OnEntryClicked;

	// Token: 0x140000D6 RID: 214
	// (add) Token: 0x060077AB RID: 30635 RVA: 0x0030BCD0 File Offset: 0x00309ED0
	// (remove) Token: 0x060077AC RID: 30636 RVA: 0x0030BD08 File Offset: 0x00309F08
	public event XUiEvent_OnPressEventHandler OnEntryDoubleClicked;

	// Token: 0x060077AD RID: 30637 RVA: 0x0030BD40 File Offset: 0x00309F40
	public override void Init()
	{
		base.Init();
		foreach (XUiC_ListEntry<XUiC_WorldList.WorldListEntry> xuiC_ListEntry in this.listEntryControllers)
		{
			xuiC_ListEntry.OnPress += this.EntryClicked;
			xuiC_ListEntry.OnDoubleClick += this.EntryDoubleClicked;
		}
	}

	// Token: 0x060077AE RID: 30638 RVA: 0x0030BD8E File Offset: 0x00309F8E
	public override void OnOpen()
	{
		base.OnOpen();
		this.RebuildList(false);
	}

	// Token: 0x060077AF RID: 30639 RVA: 0x0030BDA0 File Offset: 0x00309FA0
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		foreach (PathAbstractions.AbstractedLocation abstractedLocation in PathAbstractions.WorldsSearchPaths.GetAvailablePathsList(null, null, null, false))
		{
			if (!XUiC_WorldList.forbiddenWorlds.ContainsWithComparer(abstractedLocation.Name, StringComparer.OrdinalIgnoreCase))
			{
				GameUtils.WorldInfo worldInfo = GameUtils.WorldInfo.LoadWorldInfo(abstractedLocation);
				if (worldInfo != null)
				{
					this.allEntries.Add(new XUiC_WorldList.WorldListEntry(abstractedLocation, GameIO.IsWorldGenerated(abstractedLocation.Name), worldInfo.GameVersionCreated, this.matchingVersionColor, this.compatibleVersionColor, this.incompatibleVersionColor));
				}
			}
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x060077B0 RID: 30640 RVA: 0x0030BE68 File Offset: 0x0030A068
	public bool SelectByName(string _name)
	{
		if (string.IsNullOrEmpty(_name))
		{
			return false;
		}
		for (int i = 0; i < this.filteredEntries.Count; i++)
		{
			if (this.filteredEntries[i].Location.Name.Equals(_name, StringComparison.OrdinalIgnoreCase))
			{
				base.SelectedEntryIndex = i;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060077B1 RID: 30641 RVA: 0x0030BEBE File Offset: 0x0030A0BE
	[PublicizedFrom(EAccessModifier.Private)]
	public void EntryClicked(XUiController _sender, int _mouseButton)
	{
		XUiEvent_OnPressEventHandler onEntryClicked = this.OnEntryClicked;
		if (onEntryClicked == null)
		{
			return;
		}
		onEntryClicked(_sender, _mouseButton);
	}

	// Token: 0x060077B2 RID: 30642 RVA: 0x0030BED2 File Offset: 0x0030A0D2
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

	// Token: 0x060077B3 RID: 30643 RVA: 0x0030BEE8 File Offset: 0x0030A0E8
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

	// Token: 0x04005B33 RID: 23347
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<string> forbiddenWorlds = new List<string>
	{
		"Empty",
		"Playtesting"
	};

	// Token: 0x04005B34 RID: 23348
	[PublicizedFrom(EAccessModifier.Private)]
	public string matchingVersionColor;

	// Token: 0x04005B35 RID: 23349
	[PublicizedFrom(EAccessModifier.Private)]
	public string compatibleVersionColor;

	// Token: 0x04005B36 RID: 23350
	[PublicizedFrom(EAccessModifier.Private)]
	public string incompatibleVersionColor;

	// Token: 0x02000EC9 RID: 3785
	[Preserve]
	public class WorldListEntry : XUiListEntry<XUiC_WorldList.WorldListEntry>
	{
		// Token: 0x060077B6 RID: 30646 RVA: 0x0030BF6C File Offset: 0x0030A16C
		public WorldListEntry(PathAbstractions.AbstractedLocation _location, bool _generatedWorld, VersionInformation _version, string matchingColor = "255,255,255", string compatibleColor = "255,255,255", string incompatibleColor = "255,255,255")
		{
			this.Location = _location;
			this.GeneratedWorld = _generatedWorld;
			this.Version = _version;
			this.versionComparison = this.Version.CompareToRunningBuild();
			this.matchingColor = matchingColor;
			this.compatibleColor = compatibleColor;
			this.incompatibleColor = incompatibleColor;
		}

		// Token: 0x060077B7 RID: 30647 RVA: 0x0030BFBD File Offset: 0x0030A1BD
		public override int CompareTo(XUiC_WorldList.WorldListEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return -1;
			}
			return string.Compare(this.Location.Name, _otherEntry.Location.Name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060077B8 RID: 30648 RVA: 0x0030BFE0 File Offset: 0x0030A1E0
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				string str = "";
				if (this.Location.Type == PathAbstractions.EAbstractedLocationType.Mods)
				{
					str = " (Mod: " + this.Location.ContainingMod.Name + ")";
				}
				else if (this.Location.Type != PathAbstractions.EAbstractedLocationType.GameData)
				{
					str = " (from " + this.Location.Type.ToStringCached<PathAbstractions.EAbstractedLocationType>() + ")";
				}
				_value = this.Location.Name + str;
				return true;
			}
			if (!(_bindingName == "entrycolor"))
			{
				return false;
			}
			if (!this.GeneratedWorld)
			{
				_value = this.matchingColor;
				return true;
			}
			_value = ((this.versionComparison == VersionInformation.EVersionComparisonResult.SameBuild || this.versionComparison == VersionInformation.EVersionComparisonResult.SameMinor) ? this.matchingColor : ((this.versionComparison == VersionInformation.EVersionComparisonResult.NewerMinor || this.versionComparison == VersionInformation.EVersionComparisonResult.OlderMinor) ? this.compatibleColor : this.incompatibleColor));
			return true;
		}

		// Token: 0x060077B9 RID: 30649 RVA: 0x0030C0D5 File Offset: 0x0030A2D5
		public override bool MatchesSearch(string _searchString)
		{
			return this.Location.Name.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x060077BA RID: 30650 RVA: 0x0030C0EF File Offset: 0x0030A2EF
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = string.Empty;
				return true;
			}
			if (!(_bindingName == "entrycolor"))
			{
				return false;
			}
			_value = "0,0,0";
			return true;
		}

		// Token: 0x04005B39 RID: 23353
		public readonly PathAbstractions.AbstractedLocation Location;

		// Token: 0x04005B3A RID: 23354
		public readonly bool GeneratedWorld;

		// Token: 0x04005B3B RID: 23355
		public readonly VersionInformation Version;

		// Token: 0x04005B3C RID: 23356
		public readonly VersionInformation.EVersionComparisonResult versionComparison;

		// Token: 0x04005B3D RID: 23357
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string matchingColor;

		// Token: 0x04005B3E RID: 23358
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string compatibleColor;

		// Token: 0x04005B3F RID: 23359
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string incompatibleColor;
	}
}
