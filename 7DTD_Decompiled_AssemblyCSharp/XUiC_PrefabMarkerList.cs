using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D9B RID: 3483
[Preserve]
public class XUiC_PrefabMarkerList : XUiC_List<XUiC_PrefabMarkerList.PrefabMarkerEntry>
{
	// Token: 0x06006D0E RID: 27918 RVA: 0x002C891C File Offset: 0x002C6B1C
	public override void OnOpen()
	{
		base.OnOpen();
		this.RebuildList(false);
	}

	// Token: 0x06006D0F RID: 27919 RVA: 0x002C892C File Offset: 0x002C6B2C
	public override void RebuildList(bool _resetFilter = false)
	{
		if (PrefabEditModeManager.Instance.VoxelPrefab == null)
		{
			return;
		}
		this.allEntries.Clear();
		foreach (Prefab.Marker marker in PrefabEditModeManager.Instance.VoxelPrefab.POIMarkers)
		{
			this.allEntries.Add(new XUiC_PrefabMarkerList.PrefabMarkerEntry(this, marker));
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x040052DD RID: 21213
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<string> groupsResult = new List<string>();

	// Token: 0x02000D9C RID: 3484
	[Preserve]
	public class PrefabMarkerEntry : XUiListEntry<XUiC_PrefabMarkerList.PrefabMarkerEntry>
	{
		// Token: 0x06006D11 RID: 27921 RVA: 0x002C89D3 File Offset: 0x002C6BD3
		public PrefabMarkerEntry(XUiC_PrefabMarkerList _parentList, Prefab.Marker _marker)
		{
			this.parentList = _parentList;
			this.marker = _marker;
		}

		// Token: 0x06006D12 RID: 27922 RVA: 0x002C89E9 File Offset: 0x002C6BE9
		public override int CompareTo(XUiC_PrefabMarkerList.PrefabMarkerEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return 1;
			}
			return string.Compare(this.marker.GroupName, _otherEntry.marker.GroupName, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06006D13 RID: 27923 RVA: 0x002C8A0C File Offset: 0x002C6C0C
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "groupname")
			{
				_value = this.marker.GroupName;
				return true;
			}
			if (_bindingName == "groupcolor")
			{
				_value = XUiC_PrefabMarkerList.PrefabMarkerEntry.colorFormatter.Format(new Color(this.marker.GroupColor.r, this.marker.GroupColor.g, this.marker.GroupColor.b, 1f));
				return true;
			}
			if (_bindingName == "markertype")
			{
				_value = this.marker.MarkerType.ToString();
				return true;
			}
			if (!(_bindingName == "markersize"))
			{
				return false;
			}
			if (Prefab.Marker.MarkerSizes.Contains(this.marker.Size))
			{
				_value = ((Prefab.Marker.MarkerSize)Prefab.Marker.MarkerSizes.IndexOf(this.marker.Size)).ToString();
			}
			else
			{
				_value = Prefab.Marker.MarkerSize.Custom.ToString();
			}
			return true;
		}

		// Token: 0x06006D14 RID: 27924 RVA: 0x002C8B1E File Offset: 0x002C6D1E
		public override bool MatchesSearch(string _searchString)
		{
			return this.marker.GroupName.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x06006D15 RID: 27925 RVA: 0x002C8B38 File Offset: 0x002C6D38
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			if (_bindingName == "groupname")
			{
				_value = string.Empty;
				return true;
			}
			if (_bindingName == "markertype")
			{
				_value = string.Empty;
				return true;
			}
			if (_bindingName == "groupcolor")
			{
				_value = XUiC_PrefabMarkerList.PrefabMarkerEntry.colorFormatter.Format(Color.clear).ToString();
				return true;
			}
			if (!(_bindingName == "markersize"))
			{
				return false;
			}
			_value = string.Empty;
			return true;
		}

		// Token: 0x040052DE RID: 21214
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XUiC_PrefabMarkerList parentList;

		// Token: 0x040052DF RID: 21215
		public readonly Prefab.Marker marker;

		// Token: 0x040052E0 RID: 21216
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly CachedStringFormatterXuiRgbaColor colorFormatter = new CachedStringFormatterXuiRgbaColor();
	}
}
