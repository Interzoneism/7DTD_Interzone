using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000D94 RID: 3476
[Preserve]
public class XUiC_PrefabFileList : XUiC_List<XUiC_PrefabFileList.PrefabFileEntry>
{
	// Token: 0x140000B9 RID: 185
	// (add) Token: 0x06006CD4 RID: 27860 RVA: 0x002C7590 File Offset: 0x002C5790
	// (remove) Token: 0x06006CD5 RID: 27861 RVA: 0x002C75C8 File Offset: 0x002C57C8
	public event XUiC_PrefabFileList.EntryDoubleClickedDelegate OnEntryDoubleClicked;

	// Token: 0x06006CD6 RID: 27862 RVA: 0x002C7600 File Offset: 0x002C5800
	public override void Init()
	{
		base.Init();
		XUiC_ListEntry<XUiC_PrefabFileList.PrefabFileEntry>[] listEntryControllers = this.listEntryControllers;
		for (int i = 0; i < listEntryControllers.Length; i++)
		{
			listEntryControllers[i].OnDoubleClick += this.EntryDoubleClicked;
		}
	}

	// Token: 0x06006CD7 RID: 27863 RVA: 0x002C763C File Offset: 0x002C583C
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		this.prefabSearchList.Clear();
		PrefabEditModeManager.Instance.FindPrefabs(this.groupFilter, this.prefabSearchList);
		foreach (PathAbstractions.AbstractedLocation location in this.prefabSearchList)
		{
			this.allEntries.Add(new XUiC_PrefabFileList.PrefabFileEntry(location));
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x06006CD8 RID: 27864 RVA: 0x002C76D8 File Offset: 0x002C58D8
	public void SetGroupFilter(string _filter)
	{
		this.groupFilter = _filter;
		this.RebuildList(true);
	}

	// Token: 0x06006CD9 RID: 27865 RVA: 0x002C76E8 File Offset: 0x002C58E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void EntryDoubleClicked(XUiController _sender, int _mouseButton)
	{
		if (this.OnEntryDoubleClicked != null)
		{
			this.OnEntryDoubleClicked(((XUiC_ListEntry<XUiC_PrefabFileList.PrefabFileEntry>)_sender).GetEntry());
		}
	}

	// Token: 0x06006CDA RID: 27866 RVA: 0x002C7708 File Offset: 0x002C5908
	public bool SelectByName(string _name)
	{
		if (!string.IsNullOrEmpty(_name))
		{
			for (int i = 0; i < this.filteredEntries.Count; i++)
			{
				if (this.filteredEntries[i].location.Name.EqualsCaseInsensitive(_name))
				{
					base.SelectedEntryIndex = i;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06006CDB RID: 27867 RVA: 0x002C775C File Offset: 0x002C595C
	public bool SelectByLocation(PathAbstractions.AbstractedLocation _location)
	{
		if (_location.Type == PathAbstractions.EAbstractedLocationType.None)
		{
			return false;
		}
		for (int i = 0; i < this.filteredEntries.Count; i++)
		{
			if (this.filteredEntries[i].location == _location)
			{
				base.SelectedEntryIndex = i;
				return true;
			}
		}
		return false;
	}

	// Token: 0x040052C7 RID: 21191
	[PublicizedFrom(EAccessModifier.Private)]
	public string groupFilter;

	// Token: 0x040052C9 RID: 21193
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<PathAbstractions.AbstractedLocation> prefabSearchList = new List<PathAbstractions.AbstractedLocation>();

	// Token: 0x02000D95 RID: 3477
	// (Invoke) Token: 0x06006CDE RID: 27870
	public delegate void EntryDoubleClickedDelegate(XUiC_PrefabFileList.PrefabFileEntry _entry);

	// Token: 0x02000D96 RID: 3478
	public class PrefabFileEntry : XUiListEntry<XUiC_PrefabFileList.PrefabFileEntry>
	{
		// Token: 0x06006CE1 RID: 27873 RVA: 0x002C77C0 File Offset: 0x002C59C0
		public PrefabFileEntry(PathAbstractions.AbstractedLocation _location)
		{
			this.location = _location;
		}

		// Token: 0x06006CE2 RID: 27874 RVA: 0x002C77CF File Offset: 0x002C59CF
		public override int CompareTo(XUiC_PrefabFileList.PrefabFileEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return 1;
			}
			return this.location.CompareTo(_otherEntry.location);
		}

		// Token: 0x06006CE3 RID: 27875 RVA: 0x002C77E8 File Offset: 0x002C59E8
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				string str = "";
				if (this.location.Type == PathAbstractions.EAbstractedLocationType.Mods)
				{
					str = " (Mod: " + this.location.ContainingMod.Name + ")";
				}
				else if (this.location.Type != PathAbstractions.EAbstractedLocationType.GameData)
				{
					str = " (from " + this.location.Type.ToStringCached<PathAbstractions.EAbstractedLocationType>() + ")";
				}
				_value = this.location.Name + str;
				return true;
			}
			if (!(_bindingName == "localizedname"))
			{
				return false;
			}
			_value = Localization.Get(this.location.Name, false);
			return true;
		}

		// Token: 0x06006CE4 RID: 27876 RVA: 0x002C78A3 File Offset: 0x002C5AA3
		public override bool MatchesSearch(string _searchString)
		{
			return this.location.Name.ContainsCaseInsensitive(_searchString);
		}

		// Token: 0x06006CE5 RID: 27877 RVA: 0x002C78B6 File Offset: 0x002C5AB6
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = string.Empty;
				return true;
			}
			if (!(_bindingName == "localizedname"))
			{
				return false;
			}
			_value = string.Empty;
			return true;
		}

		// Token: 0x040052CA RID: 21194
		public readonly PathAbstractions.AbstractedLocation location;
	}
}
