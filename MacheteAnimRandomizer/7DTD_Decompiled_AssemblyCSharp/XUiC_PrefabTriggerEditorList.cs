using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x02000DA2 RID: 3490
[Preserve]
public class XUiC_PrefabTriggerEditorList : XUiC_List<XUiC_PrefabTriggerEditorList.PrefabTriggerEntry>
{
	// Token: 0x06006D37 RID: 27959 RVA: 0x002C9389 File Offset: 0x002C7589
	public override void OnOpen()
	{
		base.OnOpen();
		this.RebuildList(false);
	}

	// Token: 0x06006D38 RID: 27960 RVA: 0x002C9398 File Offset: 0x002C7598
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		this.groupsResult.Clear();
		if (this.EditPrefab != null)
		{
			List<byte> triggerLayers = this.EditPrefab.TriggerLayers;
			for (int i = 0; i < triggerLayers.Count; i++)
			{
				this.allEntries.Add(new XUiC_PrefabTriggerEditorList.PrefabTriggerEntry(this, triggerLayers[i]));
			}
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x040052EB RID: 21227
	public Prefab EditPrefab;

	// Token: 0x040052EC RID: 21228
	public XUiC_TriggerProperties Owner;

	// Token: 0x040052ED RID: 21229
	public XUiC_WoPropsSleeperVolume SleeperOwner;

	// Token: 0x040052EE RID: 21230
	public bool IsTriggers;

	// Token: 0x040052EF RID: 21231
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<string> groupsResult = new List<string>();

	// Token: 0x02000DA3 RID: 3491
	[Preserve]
	public class PrefabTriggerEntry : XUiListEntry<XUiC_PrefabTriggerEditorList.PrefabTriggerEntry>
	{
		// Token: 0x06006D3A RID: 27962 RVA: 0x002C941D File Offset: 0x002C761D
		public PrefabTriggerEntry(XUiC_PrefabTriggerEditorList _parentList, byte _triggerLayer)
		{
			this.parentList = _parentList;
			this.TriggerLayer = _triggerLayer;
			this.name = _triggerLayer.ToString();
		}

		// Token: 0x06006D3B RID: 27963 RVA: 0x002C9440 File Offset: 0x002C7640
		public override int CompareTo(XUiC_PrefabTriggerEditorList.PrefabTriggerEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return 1;
			}
			return this.TriggerLayer.CompareTo(_otherEntry.TriggerLayer);
		}

		// Token: 0x06006D3C RID: 27964 RVA: 0x002C9458 File Offset: 0x002C7658
		[PublicizedFrom(EAccessModifier.Private)]
		public bool GetSelected()
		{
			bool result = false;
			if (this.parentList.Owner != null)
			{
				if (this.parentList.Owner.blockTrigger != null || this.parentList.Owner.TriggerVolume != null)
				{
					if (this.parentList.IsTriggers)
					{
						if (this.parentList.Owner.TriggersIndices != null)
						{
							result = this.parentList.Owner.TriggersIndices.Contains(StringParsers.ParseUInt8(this.name, 0, -1, NumberStyles.Integer));
						}
					}
					else if (this.parentList.Owner != null)
					{
						if (this.parentList.Owner.TriggeredByIndices != null)
						{
							result = this.parentList.Owner.TriggeredByIndices.Contains(StringParsers.ParseUInt8(this.name, 0, -1, NumberStyles.Integer));
						}
					}
					else if (this.parentList.SleeperOwner != null && this.parentList.SleeperOwner.TriggeredByIndices != null)
					{
						result = this.parentList.SleeperOwner.TriggeredByIndices.Contains(StringParsers.ParseUInt8(this.name, 0, -1, NumberStyles.Integer));
					}
				}
			}
			else if (this.parentList.SleeperOwner != null && !this.parentList.IsTriggers && this.parentList.SleeperOwner != null && this.parentList.SleeperOwner.TriggeredByIndices != null)
			{
				result = this.parentList.SleeperOwner.TriggeredByIndices.Contains(StringParsers.ParseUInt8(this.name, 0, -1, NumberStyles.Integer));
			}
			return result;
		}

		// Token: 0x06006D3D RID: 27965 RVA: 0x002C95E0 File Offset: 0x002C77E0
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = this.name;
				return true;
			}
			if (_bindingName == "selected")
			{
				_value = (this.GetSelected() ? "true" : "false");
				return true;
			}
			if (!(_bindingName == "assigned"))
			{
				return false;
			}
			_value = "true";
			return true;
		}

		// Token: 0x06006D3E RID: 27966 RVA: 0x002C9642 File Offset: 0x002C7842
		public override bool MatchesSearch(string _searchString)
		{
			return this.name.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x06006D3F RID: 27967 RVA: 0x002C9658 File Offset: 0x002C7858
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = string.Empty;
				return true;
			}
			if (_bindingName == "selected")
			{
				_value = "false";
				return true;
			}
			if (!(_bindingName == "assigned"))
			{
				return false;
			}
			_value = "false";
			return true;
		}

		// Token: 0x040052F0 RID: 21232
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XUiC_PrefabTriggerEditorList parentList;

		// Token: 0x040052F1 RID: 21233
		public readonly string name;

		// Token: 0x040052F2 RID: 21234
		public byte TriggerLayer;
	}
}
