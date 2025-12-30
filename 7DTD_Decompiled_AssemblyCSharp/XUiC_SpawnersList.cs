using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000E4D RID: 3661
[Preserve]
public class XUiC_SpawnersList : XUiC_List<XUiC_SpawnersList.SpawnerEntry>
{
	// Token: 0x060072F6 RID: 29430 RVA: 0x002EE0C4 File Offset: 0x002EC2C4
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		foreach (KeyValuePair<string, GameStageGroup> keyValuePair in GameStageGroup.Groups)
		{
			string key = keyValuePair.Key;
			this.allEntries.Add(new XUiC_SpawnersList.SpawnerEntry(key));
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x02000E4E RID: 3662
	[Preserve]
	public class SpawnerEntry : XUiListEntry<XUiC_SpawnersList.SpawnerEntry>
	{
		// Token: 0x060072F8 RID: 29432 RVA: 0x002EE150 File Offset: 0x002EC350
		public SpawnerEntry(string _name)
		{
			this.name = _name;
			this.displayName = GameStageGroup.MakeDisplayName(_name);
		}

		// Token: 0x060072F9 RID: 29433 RVA: 0x002EE16B File Offset: 0x002EC36B
		public override int CompareTo(XUiC_SpawnersList.SpawnerEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return 1;
			}
			return string.Compare(this.name, _otherEntry.name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060072FA RID: 29434 RVA: 0x002EE184 File Offset: 0x002EC384
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = this.displayName;
				return true;
			}
			return false;
		}

		// Token: 0x060072FB RID: 29435 RVA: 0x002EE19E File Offset: 0x002EC39E
		public override bool MatchesSearch(string _searchString)
		{
			return this.name.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x060072FC RID: 29436 RVA: 0x0028C55A File Offset: 0x0028A75A
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = string.Empty;
				return true;
			}
			return false;
		}

		// Token: 0x04005790 RID: 22416
		public readonly string name;

		// Token: 0x04005791 RID: 22417
		public readonly string displayName;
	}
}
