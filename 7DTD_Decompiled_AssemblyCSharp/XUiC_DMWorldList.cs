using System;
using System.Collections.ObjectModel;
using UnityEngine.Scripting;

// Token: 0x02000C9D RID: 3229
[Preserve]
public class XUiC_DMWorldList : XUiC_DMBaseList<XUiC_DMWorldList.ListEntry>
{
	// Token: 0x060063A5 RID: 25509 RVA: 0x00002914 File Offset: 0x00000B14
	public override void RebuildList(bool _resetFilter = false)
	{
	}

	// Token: 0x060063A6 RID: 25510 RVA: 0x00286544 File Offset: 0x00284744
	public void RebuildList(ReadOnlyCollection<SaveInfoProvider.WorldEntryInfo> worldEntryInfos, bool _resetFilter = false)
	{
		this.allEntries.Clear();
		foreach (SaveInfoProvider.WorldEntryInfo worldEntryInfo in worldEntryInfos)
		{
			this.allEntries.Add(new XUiC_DMWorldList.ListEntry(worldEntryInfo, this.matchingVersionColor, this.compatibleVersionColor, this.incompatibleVersionColor));
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x060063A7 RID: 25511 RVA: 0x002865C8 File Offset: 0x002847C8
	public void ClearList()
	{
		this.allEntries.Clear();
		this.RebuildList(true);
	}

	// Token: 0x060063A8 RID: 25512 RVA: 0x002865DC File Offset: 0x002847DC
	public bool SelectByKey(string _key)
	{
		if (string.IsNullOrEmpty(_key))
		{
			return false;
		}
		for (int i = 0; i < this.filteredEntries.Count; i++)
		{
			if (this.filteredEntries[i].Key.Equals(_key, StringComparison.OrdinalIgnoreCase))
			{
				base.SelectedEntryIndex = i;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060063A9 RID: 25513 RVA: 0x0028662D File Offset: 0x0028482D
	public void UpdateHiddenEntryVisibility()
	{
		this.filteredEntries.Clear();
		this.FilterResults(this.previousMatch);
	}

	// Token: 0x060063AA RID: 25514 RVA: 0x00286648 File Offset: 0x00284848
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void FilterResults(string _textMatch)
	{
		base.FilterResults(_textMatch);
		for (int i = 0; i < this.filteredEntries.Count; i++)
		{
			XUiC_DMWorldList.ListEntry listEntry = this.filteredEntries[i];
			if (listEntry.HideIfEmpty && listEntry.SaveDataSize == 0L)
			{
				this.filteredEntries.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x060063AB RID: 25515 RVA: 0x002866A0 File Offset: 0x002848A0
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

	// Token: 0x04004B03 RID: 19203
	[PublicizedFrom(EAccessModifier.Private)]
	public string matchingVersionColor;

	// Token: 0x04004B04 RID: 19204
	[PublicizedFrom(EAccessModifier.Private)]
	public string compatibleVersionColor;

	// Token: 0x04004B05 RID: 19205
	[PublicizedFrom(EAccessModifier.Private)]
	public string incompatibleVersionColor;

	// Token: 0x02000C9E RID: 3230
	[Preserve]
	public class ListEntry : XUiListEntry<XUiC_DMWorldList.ListEntry>
	{
		// Token: 0x060063AD RID: 25517 RVA: 0x00286704 File Offset: 0x00284904
		public ListEntry(SaveInfoProvider.WorldEntryInfo worldEntryInfo, string matchingColor, string compatibleColor, string incompatibleColor)
		{
			this.WorldEntryInfo = worldEntryInfo;
			this.Key = worldEntryInfo.WorldKey;
			this.Name = worldEntryInfo.Name;
			this.Type = worldEntryInfo.Type;
			this.Location = worldEntryInfo.Location;
			this.Deletable = worldEntryInfo.Deletable;
			this.WorldDataSize = worldEntryInfo.WorldDataSize;
			this.Version = worldEntryInfo.Version;
			VersionInformation version = this.Version;
			this.versionComparison = ((version != null) ? version.CompareToRunningBuild() : VersionInformation.EVersionComparisonResult.SameBuild);
			this.SaveDataSize = worldEntryInfo.SaveDataSize;
			this.SaveDataCount = worldEntryInfo.SaveDataCount;
			this.matchingColor = matchingColor;
			this.compatibleColor = compatibleColor;
			this.incompatibleColor = incompatibleColor;
			this.HideIfEmpty = worldEntryInfo.HideIfEmpty;
		}

		// Token: 0x060063AE RID: 25518 RVA: 0x002867C4 File Offset: 0x002849C4
		public override int CompareTo(XUiC_DMWorldList.ListEntry _otherEntry)
		{
			if (_otherEntry != null)
			{
				return this.WorldEntryInfo.CompareTo(_otherEntry.WorldEntryInfo);
			}
			return 1;
		}

		// Token: 0x060063AF RID: 25519 RVA: 0x002867DC File Offset: 0x002849DC
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 1361572173U)
			{
				if (num <= 941220257U)
				{
					if (num != 572742324U)
					{
						if (num == 941220257U)
						{
							if (_bindingName == "saveDataSize")
							{
								_value = XUiC_DataManagement.FormatMemoryString(this.SaveDataSize);
								return true;
							}
						}
					}
					else if (_bindingName == "totalDataSize")
					{
						long bytes = this.Deletable ? (this.WorldDataSize + this.SaveDataSize) : this.SaveDataSize;
						_value = XUiC_DataManagement.FormatMemoryString(bytes);
						return true;
					}
				}
				else if (num != 1181855383U)
				{
					if (num != 1204961018U)
					{
						if (num == 1361572173U)
						{
							if (_bindingName == "type")
							{
								_value = this.Type;
								return true;
							}
						}
					}
					else if (_bindingName == "worldDataSize")
					{
						_value = (this.Deletable ? XUiC_DataManagement.FormatMemoryString(this.WorldDataSize) : "-");
						return true;
					}
				}
				else if (_bindingName == "version")
				{
					_value = ((this.Version == null) ? string.Empty : ((this.Version.Major >= 0) ? this.Version.LongStringNoBuild : Constants.cVersionInformation.LongStringNoBuild));
					return true;
				}
			}
			else if (num <= 2049496678U)
			{
				if (num != 1566407741U)
				{
					if (num == 2049496678U)
					{
						if (_bindingName == "versioncolor")
						{
							_value = ((this.Version == null) ? this.incompatibleColor : ((this.Version.Major < 0 || this.versionComparison == VersionInformation.EVersionComparisonResult.SameBuild || this.versionComparison == VersionInformation.EVersionComparisonResult.SameMinor) ? this.matchingColor : ((this.versionComparison == VersionInformation.EVersionComparisonResult.NewerMinor || this.versionComparison == VersionInformation.EVersionComparisonResult.OlderMinor) ? this.compatibleColor : this.incompatibleColor)));
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
			else if (num != 2369371622U)
			{
				if (num != 2497009599U)
				{
					if (num == 4086844294U)
					{
						if (_bindingName == "versiontooltip")
						{
							_value = ((this.Version == null) ? string.Empty : ((this.Version.Major < 0 || this.versionComparison == VersionInformation.EVersionComparisonResult.SameBuild || this.versionComparison == VersionInformation.EVersionComparisonResult.SameMinor) ? "" : ((this.versionComparison == VersionInformation.EVersionComparisonResult.NewerMinor) ? Localization.Get("xuiSavegameNewerMinor", false) : ((this.versionComparison == VersionInformation.EVersionComparisonResult.OlderMinor) ? Localization.Get("xuiSavegameOlderMinor", false) : Localization.Get("xuiSavegameDifferentMajor", false)))));
							return true;
						}
					}
				}
				else if (_bindingName == "saveDataCount")
				{
					_value = string.Format("{0} {1}", this.SaveDataCount, Localization.Get("xuiDmSaves", false));
					return true;
				}
			}
			else if (_bindingName == "name")
			{
				_value = this.Name;
				return true;
			}
			return false;
		}

		// Token: 0x060063B0 RID: 25520 RVA: 0x00286AEE File Offset: 0x00284CEE
		public override bool MatchesSearch(string _searchString)
		{
			return this.Name.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x060063B1 RID: 25521 RVA: 0x00286B04 File Offset: 0x00284D04
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
			if (num <= 1361572173U)
			{
				if (num <= 941220257U)
				{
					if (num != 572742324U)
					{
						if (num != 941220257U)
						{
							return false;
						}
						if (!(_bindingName == "saveDataSize"))
						{
							return false;
						}
					}
					else if (!(_bindingName == "totalDataSize"))
					{
						return false;
					}
				}
				else if (num != 1181855383U)
				{
					if (num != 1204961018U)
					{
						if (num != 1361572173U)
						{
							return false;
						}
						if (!(_bindingName == "type"))
						{
							return false;
						}
					}
					else if (!(_bindingName == "worldDataSize"))
					{
						return false;
					}
				}
				else if (!(_bindingName == "version"))
				{
					return false;
				}
			}
			else if (num <= 2049496678U)
			{
				if (num != 1566407741U)
				{
					if (num != 2049496678U)
					{
						return false;
					}
					if (!(_bindingName == "versioncolor"))
					{
						return false;
					}
					_value = "0,0,0";
					return true;
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
			else if (num != 2369371622U)
			{
				if (num != 2497009599U)
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
				else if (!(_bindingName == "saveDataCount"))
				{
					return false;
				}
			}
			else if (!(_bindingName == "name"))
			{
				return false;
			}
			_value = string.Empty;
			return true;
		}

		// Token: 0x04004B06 RID: 19206
		public readonly string Key;

		// Token: 0x04004B07 RID: 19207
		public readonly string Name;

		// Token: 0x04004B08 RID: 19208
		public readonly string Type;

		// Token: 0x04004B09 RID: 19209
		public readonly PathAbstractions.AbstractedLocation Location;

		// Token: 0x04004B0A RID: 19210
		public readonly bool Deletable;

		// Token: 0x04004B0B RID: 19211
		public readonly long WorldDataSize;

		// Token: 0x04004B0C RID: 19212
		public readonly VersionInformation Version;

		// Token: 0x04004B0D RID: 19213
		public readonly VersionInformation.EVersionComparisonResult versionComparison;

		// Token: 0x04004B0E RID: 19214
		public readonly long SaveDataSize;

		// Token: 0x04004B0F RID: 19215
		public readonly int SaveDataCount;

		// Token: 0x04004B10 RID: 19216
		public readonly bool HideIfEmpty;

		// Token: 0x04004B11 RID: 19217
		public readonly SaveInfoProvider.WorldEntryInfo WorldEntryInfo;

		// Token: 0x04004B12 RID: 19218
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string matchingColor;

		// Token: 0x04004B13 RID: 19219
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string compatibleColor;

		// Token: 0x04004B14 RID: 19220
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string incompatibleColor;
	}
}
