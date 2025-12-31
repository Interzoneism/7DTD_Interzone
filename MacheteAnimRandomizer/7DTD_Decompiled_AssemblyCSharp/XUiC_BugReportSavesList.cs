using System;
using System.Collections.ObjectModel;
using UnityEngine.Scripting;

// Token: 0x02000C15 RID: 3093
[Preserve]
public class XUiC_BugReportSavesList : XUiC_List<XUiC_BugReportSavesList.ListEntry>
{
	// Token: 0x06005EF5 RID: 24309 RVA: 0x00002914 File Offset: 0x00000B14
	public override void RebuildList(bool _resetFilter = false)
	{
	}

	// Token: 0x06005EF6 RID: 24310 RVA: 0x00268774 File Offset: 0x00266974
	public void RebuildList(ReadOnlyCollection<SaveInfoProvider.SaveEntryInfo> saveEntryInfos, bool _resetFilter = false)
	{
		this.allEntries.Clear();
		foreach (SaveInfoProvider.SaveEntryInfo saveEntryInfo in saveEntryInfos)
		{
			this.allEntries.Add(new XUiC_BugReportSavesList.ListEntry(saveEntryInfo));
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x02000C16 RID: 3094
	[Preserve]
	public class ListEntry : XUiListEntry<XUiC_BugReportSavesList.ListEntry>
	{
		// Token: 0x06005EF8 RID: 24312 RVA: 0x002687EC File Offset: 0x002669EC
		public ListEntry(SaveInfoProvider.SaveEntryInfo saveEntryInfo)
		{
			this.saveEntryInfo = saveEntryInfo;
			this.saveName = saveEntryInfo.Name;
			this.worldKey = saveEntryInfo.WorldEntry.WorldKey;
			this.saveDirectory = saveEntryInfo.SaveDir;
			this.lastSaved = saveEntryInfo.LastSaved;
		}

		// Token: 0x06005EF9 RID: 24313 RVA: 0x0026883B File Offset: 0x00266A3B
		public override int CompareTo(XUiC_BugReportSavesList.ListEntry _otherEntry)
		{
			if (_otherEntry != null)
			{
				return this.saveEntryInfo.CompareTo(_otherEntry.saveEntryInfo);
			}
			return 1;
		}

		// Token: 0x06005EFA RID: 24314 RVA: 0x00268854 File Offset: 0x00266A54
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "savename")
			{
				_value = this.saveName;
				return true;
			}
			if (_bindingName == "worldname")
			{
				_value = this.saveEntryInfo.WorldEntry.Name;
				return true;
			}
			if (_bindingName == "lastplayed")
			{
				_value = this.lastSaved.ToString("yyyy-MM-dd HH:mm");
				return true;
			}
			if (!(_bindingName == "hasentry"))
			{
				return false;
			}
			_value = true.ToString();
			return true;
		}

		// Token: 0x06005EFB RID: 24315 RVA: 0x002688DA File Offset: 0x00266ADA
		public override bool MatchesSearch(string _searchString)
		{
			return this.saveName.ContainsCaseInsensitive(_searchString);
		}

		// Token: 0x06005EFC RID: 24316 RVA: 0x002688E8 File Offset: 0x00266AE8
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

		// Token: 0x040047A4 RID: 18340
		public readonly string saveName;

		// Token: 0x040047A5 RID: 18341
		public readonly string worldKey;

		// Token: 0x040047A6 RID: 18342
		public readonly string saveDirectory;

		// Token: 0x040047A7 RID: 18343
		public readonly DateTime lastSaved;

		// Token: 0x040047A8 RID: 18344
		public readonly SaveInfoProvider.SaveEntryInfo saveEntryInfo;
	}
}
