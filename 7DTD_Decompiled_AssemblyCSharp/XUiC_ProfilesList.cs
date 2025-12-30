using System;
using UniLinq;
using UnityEngine.Scripting;

// Token: 0x02000DA5 RID: 3493
[Preserve]
public class XUiC_ProfilesList : XUiC_List<XUiC_ProfilesList.ListEntry>
{
	// Token: 0x06006D45 RID: 27973 RVA: 0x002C9707 File Offset: 0x002C7907
	public override void OnOpen()
	{
		base.OnOpen();
		this.RebuildList(false);
	}

	// Token: 0x06006D46 RID: 27974 RVA: 0x002C9718 File Offset: 0x002C7918
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		foreach (string text in Archetype.s_Archetypes.Keys)
		{
			if (text != "BaseMale" && text != "BaseFemale")
			{
				this.allEntries.Add(new XUiC_ProfilesList.ListEntry(text, true));
			}
		}
		foreach (string name in (from s in ProfileSDF.GetProfiles()
		where (Archetype.GetArchetype(s) == null && ProfileSDF.GetArchetype(s) != null && (ProfileSDF.GetArchetype(s).Equals("BaseMale") || ProfileSDF.GetArchetype(s).Equals("BaseFemale"))) || Archetype.GetArchetype(s) != null
		select s).ToArray<string>())
		{
			bool flag = Archetype.GetArchetype(name) != null;
			if (!flag)
			{
				this.allEntries.Add(new XUiC_ProfilesList.ListEntry(name, flag));
			}
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
		this.SelectByName(ProfileSDF.CurrentProfileName());
	}

	// Token: 0x06006D47 RID: 27975 RVA: 0x002C9824 File Offset: 0x002C7A24
	public bool SelectByName(string _name)
	{
		if (!string.IsNullOrEmpty(_name))
		{
			for (int i = 0; i < this.filteredEntries.Count; i++)
			{
				if (this.filteredEntries[i].name.Equals(_name, StringComparison.OrdinalIgnoreCase))
				{
					base.SelectedEntryIndex = i;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x02000DA6 RID: 3494
	[Preserve]
	public class ListEntry : XUiListEntry<XUiC_ProfilesList.ListEntry>
	{
		// Token: 0x06006D49 RID: 27977 RVA: 0x002C987B File Offset: 0x002C7A7B
		public ListEntry(string _name, bool _isArchetype)
		{
			this.name = _name;
			this.isArchetype = _isArchetype;
		}

		// Token: 0x06006D4A RID: 27978 RVA: 0x002C9894 File Offset: 0x002C7A94
		public override int CompareTo(XUiC_ProfilesList.ListEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return 1;
			}
			int num = -this.isArchetype.CompareTo(_otherEntry.isArchetype);
			if (num != 0)
			{
				return num;
			}
			return string.Compare(this.name, _otherEntry.name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06006D4B RID: 27979 RVA: 0x002C98D3 File Offset: 0x002C7AD3
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = this.name;
				return true;
			}
			return false;
		}

		// Token: 0x06006D4C RID: 27980 RVA: 0x002C98ED File Offset: 0x002C7AED
		public override bool MatchesSearch(string _searchString)
		{
			return this.name.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x06006D4D RID: 27981 RVA: 0x0028C55A File Offset: 0x0028A75A
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

		// Token: 0x040052F3 RID: 21235
		public readonly string name;

		// Token: 0x040052F4 RID: 21236
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool isArchetype;
	}
}
