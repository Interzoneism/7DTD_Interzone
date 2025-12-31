using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Scripting;

// Token: 0x02000C9A RID: 3226
[Preserve]
public class XUiC_DMSavegamesList : XUiC_DMBaseList<XUiC_DMSavegamesList.ListEntry>
{
	// Token: 0x0600638E RID: 25486 RVA: 0x00002914 File Offset: 0x00000B14
	public override void RebuildList(bool _resetFilter = false)
	{
	}

	// Token: 0x0600638F RID: 25487 RVA: 0x00285C7C File Offset: 0x00283E7C
	public void RebuildList(ReadOnlyCollection<SaveInfoProvider.SaveEntryInfo> saveEntryInfos, bool _resetFilter = false)
	{
		this.allEntries.Clear();
		foreach (SaveInfoProvider.SaveEntryInfo saveEntryInfo in saveEntryInfos)
		{
			this.allEntries.Add(new XUiC_DMSavegamesList.ListEntry(saveEntryInfo, this.matchingVersionColor, this.compatibleVersionColor, this.incompatibleVersionColor));
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x06006390 RID: 25488 RVA: 0x00285D00 File Offset: 0x00283F00
	public void ClearList()
	{
		this.allEntries.Clear();
		this.RebuildList(true);
	}

	// Token: 0x06006391 RID: 25489 RVA: 0x00285D14 File Offset: 0x00283F14
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

	// Token: 0x06006392 RID: 25490 RVA: 0x00285D63 File Offset: 0x00283F63
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnSearchInputChanged(XUiController _sender, string _text, bool _changeFromCode)
	{
		base.OnSearchInputChanged(_sender, _text, _changeFromCode);
	}

	// Token: 0x06006393 RID: 25491 RVA: 0x00285D6E File Offset: 0x00283F6E
	public void SetWorldFilter(string _worldKey)
	{
		this.worldFilter = _worldKey;
		this.filteredEntries.Clear();
		this.FilterResults(this.previousMatch);
		this.RefreshView(false, true);
	}

	// Token: 0x06006394 RID: 25492 RVA: 0x00285D98 File Offset: 0x00283F98
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
			if (this.filteredEntries[i].worldKey != this.worldFilter)
			{
				this.filteredEntries.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x06006395 RID: 25493 RVA: 0x00285DF9 File Offset: 0x00283FF9
	public IEnumerable<XUiC_DMSavegamesList.ListEntry> GetSavesInWorld(string _worldKey)
	{
		if (string.IsNullOrEmpty(_worldKey))
		{
			yield break;
		}
		int num;
		for (int i = 0; i < this.allEntries.Count; i = num + 1)
		{
			if (this.allEntries[i].worldKey == _worldKey)
			{
				yield return this.allEntries[i];
			}
			num = i;
		}
		yield break;
	}

	// Token: 0x06006396 RID: 25494 RVA: 0x00285E10 File Offset: 0x00284010
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

	// Token: 0x04004AEE RID: 19182
	[PublicizedFrom(EAccessModifier.Private)]
	public string matchingVersionColor;

	// Token: 0x04004AEF RID: 19183
	[PublicizedFrom(EAccessModifier.Private)]
	public string compatibleVersionColor;

	// Token: 0x04004AF0 RID: 19184
	[PublicizedFrom(EAccessModifier.Private)]
	public string incompatibleVersionColor;

	// Token: 0x04004AF1 RID: 19185
	public string worldFilter;

	// Token: 0x02000C9B RID: 3227
	[Preserve]
	public class ListEntry : XUiListEntry<XUiC_DMSavegamesList.ListEntry>
	{
		// Token: 0x06006398 RID: 25496 RVA: 0x00285E74 File Offset: 0x00284074
		public ListEntry(SaveInfoProvider.SaveEntryInfo saveEntryInfo, string matchingColor = "255,255,255", string compatibleColor = "255,255,255", string incompatibleColor = "255,255,255")
		{
			this.saveEntryInfo = saveEntryInfo;
			this.saveName = saveEntryInfo.Name;
			this.worldKey = saveEntryInfo.WorldEntry.WorldKey;
			this.saveDirectory = saveEntryInfo.SaveDir;
			this.lastSaved = saveEntryInfo.LastSaved;
			this.version = saveEntryInfo.Version;
			VersionInformation versionInformation = this.version;
			this.versionComparison = ((versionInformation != null) ? versionInformation.CompareToRunningBuild() : VersionInformation.EVersionComparisonResult.SameBuild);
			this.matchingColor = matchingColor;
			this.compatibleColor = compatibleColor;
			this.incompatibleColor = incompatibleColor;
		}

		// Token: 0x06006399 RID: 25497 RVA: 0x00285EFD File Offset: 0x002840FD
		public override int CompareTo(XUiC_DMSavegamesList.ListEntry _otherEntry)
		{
			if (_otherEntry != null)
			{
				return this.saveEntryInfo.CompareTo(_otherEntry.saveEntryInfo);
			}
			return 1;
		}

		// Token: 0x0600639A RID: 25498 RVA: 0x00285F18 File Offset: 0x00284118
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 1566407741U)
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
				else if (num != 1181855383U)
				{
					if (num != 1355330520U)
					{
						if (num != 1566407741U)
						{
							return false;
						}
						if (!(_bindingName == "hasentry"))
						{
							return false;
						}
						_value = true.ToString();
						return true;
					}
					else
					{
						if (!(_bindingName == "worldname"))
						{
							return false;
						}
						_value = this.worldKey;
						return true;
					}
				}
				else
				{
					if (!(_bindingName == "version"))
					{
						return false;
					}
					_value = ((this.version == null) ? string.Empty : ((this.version.Major >= 0) ? this.version.LongStringNoBuild : Constants.cVersionInformation.LongStringNoBuild));
					return true;
				}
			}
			else if (num <= 1823525230U)
			{
				if (num != 1800901934U)
				{
					if (num != 1823525230U)
					{
						return false;
					}
					if (!(_bindingName == "lastplayedinfo"))
					{
						return false;
					}
					if (this.saveEntryInfo.SizeInfo.IsArchived)
					{
						_value = "[fabc02ff]" + Localization.Get("xuiDmArchivedLabel", false) + "[-]";
					}
					else
					{
						int num2 = (int)(DateTime.Now - this.lastSaved).TotalDays;
						_value = string.Format("[ffffff88]{0} {1}[-]", num2, Localization.Get("xuiDmDaysAgo", false));
					}
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
			else if (num != 2049496678U)
			{
				if (num != 2595142937U)
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
					if (!(_bindingName == "savesize"))
					{
						return false;
					}
					string text = this.saveEntryInfo.SizeInfo.IsArchived ? "fabc02ff" : "ffffffbb";
					_value = string.Concat(new string[]
					{
						"[",
						text,
						"]",
						XUiC_DataManagement.FormatMemoryString(this.saveEntryInfo.SizeInfo.ReportedSize),
						"[-]"
					});
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

		// Token: 0x0600639B RID: 25499 RVA: 0x0028625E File Offset: 0x0028445E
		public override bool MatchesSearch(string _searchString)
		{
			return this.saveName.ContainsCaseInsensitive(_searchString);
		}

		// Token: 0x0600639C RID: 25500 RVA: 0x0028626C File Offset: 0x0028446C
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 1566407741U)
			{
				if (num <= 719397874U)
				{
					if (num != 8204530U)
					{
						if (num != 205488363U)
						{
							if (num != 719397874U)
							{
								return false;
							}
							if (!(_bindingName == "worldcolor"))
							{
								return false;
							}
							goto IL_186;
						}
						else if (!(_bindingName == "savename"))
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
						goto IL_186;
					}
				}
				else if (num != 1181855383U)
				{
					if (num != 1355330520U)
					{
						if (num != 1566407741U)
						{
							return false;
						}
						if (!(_bindingName == "hasentry"))
						{
							return false;
						}
						_value = false.ToString();
						return true;
					}
					else if (!(_bindingName == "worldname"))
					{
						return false;
					}
				}
				else if (!(_bindingName == "version"))
				{
					return false;
				}
			}
			else if (num <= 1871248802U)
			{
				if (num != 1800901934U)
				{
					if (num != 1823525230U)
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
					else if (!(_bindingName == "lastplayedinfo"))
					{
						return false;
					}
				}
				else if (!(_bindingName == "lastplayed"))
				{
					return false;
				}
			}
			else if (num != 2049496678U)
			{
				if (num != 2595142937U)
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
				else if (!(_bindingName == "savesize"))
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
				goto IL_186;
			}
			_value = "";
			return true;
			IL_186:
			_value = "0,0,0";
			return true;
		}

		// Token: 0x04004AF2 RID: 19186
		public readonly string saveName;

		// Token: 0x04004AF3 RID: 19187
		public readonly string worldKey;

		// Token: 0x04004AF4 RID: 19188
		public readonly string saveDirectory;

		// Token: 0x04004AF5 RID: 19189
		public readonly DateTime lastSaved;

		// Token: 0x04004AF6 RID: 19190
		public readonly VersionInformation version;

		// Token: 0x04004AF7 RID: 19191
		public readonly VersionInformation.EVersionComparisonResult versionComparison;

		// Token: 0x04004AF8 RID: 19192
		public readonly SaveInfoProvider.SaveEntryInfo saveEntryInfo;

		// Token: 0x04004AF9 RID: 19193
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string matchingColor;

		// Token: 0x04004AFA RID: 19194
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string compatibleColor;

		// Token: 0x04004AFB RID: 19195
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string incompatibleColor;
	}
}
