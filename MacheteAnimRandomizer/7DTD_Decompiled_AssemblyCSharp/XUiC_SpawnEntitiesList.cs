using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000E4B RID: 3659
[Preserve]
public class XUiC_SpawnEntitiesList : XUiC_List<XUiC_SpawnEntitiesList.SpawnEntityEntry>
{
	// Token: 0x060072EE RID: 29422 RVA: 0x002EDF90 File Offset: 0x002EC190
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
		{
			if (keyValuePair.Value.userSpawnType == EntityClass.UserSpawnType.Menu)
			{
				this.allEntries.Add(new XUiC_SpawnEntitiesList.SpawnEntityEntry(keyValuePair.Value.entityClassName, keyValuePair.Key));
			}
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x060072EF RID: 29423 RVA: 0x002EE030 File Offset: 0x002EC230
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.allEntries.Count == 0)
		{
			this.RebuildList(false);
		}
	}

	// Token: 0x02000E4C RID: 3660
	[Preserve]
	public class SpawnEntityEntry : XUiListEntry<XUiC_SpawnEntitiesList.SpawnEntityEntry>
	{
		// Token: 0x060072F1 RID: 29425 RVA: 0x002EE054 File Offset: 0x002EC254
		public SpawnEntityEntry(string _name, int _key)
		{
			this.name = _name;
			this.key = _key;
			this.camelCase = this.name.SeparateCamelCase();
		}

		// Token: 0x060072F2 RID: 29426 RVA: 0x002EE07B File Offset: 0x002EC27B
		public override int CompareTo(XUiC_SpawnEntitiesList.SpawnEntityEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return 1;
			}
			return string.Compare(this.name, _otherEntry.name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x060072F3 RID: 29427 RVA: 0x002EE094 File Offset: 0x002EC294
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = this.camelCase;
				return true;
			}
			return false;
		}

		// Token: 0x060072F4 RID: 29428 RVA: 0x002EE0AE File Offset: 0x002EC2AE
		public override bool MatchesSearch(string _searchString)
		{
			return this.name.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x060072F5 RID: 29429 RVA: 0x0028C55A File Offset: 0x0028A75A
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

		// Token: 0x0400578D RID: 22413
		public readonly string name;

		// Token: 0x0400578E RID: 22414
		public readonly int key;

		// Token: 0x0400578F RID: 22415
		public readonly string camelCase;
	}
}
