using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000CB6 RID: 3254
[Preserve]
public class XUiC_GameEventsList : XUiC_List<XUiC_GameEventsList.GameEventEntry>
{
	// Token: 0x17000A45 RID: 2629
	// (get) Token: 0x060064A7 RID: 25767 RVA: 0x0028C3F3 File Offset: 0x0028A5F3
	// (set) Token: 0x060064A8 RID: 25768 RVA: 0x0028C3FB File Offset: 0x0028A5FB
	public string Category
	{
		get
		{
			return this.category;
		}
		set
		{
			this.category = value;
			this.RebuildList(false);
		}
	}

	// Token: 0x060064A9 RID: 25769 RVA: 0x0028C40C File Offset: 0x0028A60C
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		foreach (KeyValuePair<string, GameEventActionSequence> keyValuePair in GameEventManager.GameEventSequences)
		{
			if (keyValuePair.Value.AllowUserTrigger && (this.category == "" || (keyValuePair.Value.CategoryNames != null && keyValuePair.Value.CategoryNames.ContainsCaseInsensitive(this.category))))
			{
				this.allEntries.Add(new XUiC_GameEventsList.GameEventEntry(keyValuePair.Key));
			}
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x060064AA RID: 25770 RVA: 0x0028C4D4 File Offset: 0x0028A6D4
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.allEntries.Count == 0)
		{
			this.RebuildList(false);
		}
	}

	// Token: 0x04004BE6 RID: 19430
	[PublicizedFrom(EAccessModifier.Private)]
	public string category = "";

	// Token: 0x02000CB7 RID: 3255
	[Preserve]
	public class GameEventEntry : XUiListEntry<XUiC_GameEventsList.GameEventEntry>
	{
		// Token: 0x060064AC RID: 25772 RVA: 0x0028C503 File Offset: 0x0028A703
		public GameEventEntry(string _name)
		{
			this.name = _name;
		}

		// Token: 0x060064AD RID: 25773 RVA: 0x0028C512 File Offset: 0x0028A712
		public override int CompareTo(XUiC_GameEventsList.GameEventEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return 1;
			}
			return string.Compare(this.name, _otherEntry.name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060064AE RID: 25774 RVA: 0x0028C52B File Offset: 0x0028A72B
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = this.name;
				return true;
			}
			return false;
		}

		// Token: 0x060064AF RID: 25775 RVA: 0x0028C545 File Offset: 0x0028A745
		public override bool MatchesSearch(string _searchString)
		{
			return this.name.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x060064B0 RID: 25776 RVA: 0x0028C55A File Offset: 0x0028A75A
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

		// Token: 0x04004BE7 RID: 19431
		public readonly string name;

		// Token: 0x04004BE8 RID: 19432
		public readonly string camelCase;
	}
}
