using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000D91 RID: 3473
[Preserve]
public abstract class XUiC_PrefabFeatureEditorList : XUiC_List<XUiC_PrefabFeatureEditorList.FeatureEntry>
{
	// Token: 0x140000B8 RID: 184
	// (add) Token: 0x06006CBC RID: 27836 RVA: 0x002C7200 File Offset: 0x002C5400
	// (remove) Token: 0x06006CBD RID: 27837 RVA: 0x002C7238 File Offset: 0x002C5438
	public event XUiC_PrefabFeatureEditorList.FeatureChangedDelegate FeatureChanged;

	// Token: 0x06006CBE RID: 27838
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract bool FeatureEnabled(string _featureName);

	// Token: 0x06006CBF RID: 27839 RVA: 0x002C7270 File Offset: 0x002C5470
	public override void Init()
	{
		base.Init();
		base.SelectionChanged += this.FeatureListSelectionChanged;
		this.addInput = (base.GetChildById("addInput") as XUiC_TextInput);
		if (this.addInput != null)
		{
			this.addInput.OnSubmitHandler += this.OnAddInputSubmit;
		}
		XUiController childById = base.GetChildById("addButton");
		if (childById != null)
		{
			childById.OnPress += this.HandleAddEntry;
		}
	}

	// Token: 0x06006CC0 RID: 27840 RVA: 0x002C72EB File Offset: 0x002C54EB
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleAddEntry(XUiController _sender, int _mouseButton)
	{
		this.OnAddFeaturePressed(this.addInput.Text);
	}

	// Token: 0x06006CC1 RID: 27841 RVA: 0x002C72EB File Offset: 0x002C54EB
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnAddInputSubmit(XUiController _sender, string _text)
	{
		this.OnAddFeaturePressed(this.addInput.Text);
	}

	// Token: 0x06006CC2 RID: 27842
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void AddNewFeature(string _featureName);

	// Token: 0x06006CC3 RID: 27843 RVA: 0x002C7300 File Offset: 0x002C5500
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnAddFeaturePressed(string _s)
	{
		_s = _s.Trim();
		if (!this.validGroupName(_s))
		{
			return;
		}
		this.AddNewFeature(_s);
		this.RebuildList(false);
		this.addInput.Text = string.Empty;
		XUiC_PrefabFeatureEditorList.FeatureChangedDelegate featureChanged = this.FeatureChanged;
		if (featureChanged == null)
		{
			return;
		}
		featureChanged(this, _s, true);
	}

	// Token: 0x06006CC4 RID: 27844 RVA: 0x002C7350 File Offset: 0x002C5550
	[PublicizedFrom(EAccessModifier.Private)]
	public bool validGroupName(string _s)
	{
		_s = _s.Trim();
		return _s.Length > 0 && _s.IndexOf(",", StringComparison.OrdinalIgnoreCase) < 0 && !this.groupsResult.ContainsCaseInsensitive(_s);
	}

	// Token: 0x06006CC5 RID: 27845
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void ToggleFeature(string _featureName);

	// Token: 0x06006CC6 RID: 27846 RVA: 0x002C7384 File Offset: 0x002C5584
	[PublicizedFrom(EAccessModifier.Private)]
	public void FeatureListSelectionChanged(XUiC_ListEntry<XUiC_PrefabFeatureEditorList.FeatureEntry> _previousEntry, XUiC_ListEntry<XUiC_PrefabFeatureEditorList.FeatureEntry> _newEntry)
	{
		if (_newEntry == null)
		{
			return;
		}
		string name = _newEntry.GetEntry().Name;
		this.ToggleFeature(name);
		_newEntry.IsDirty = true;
		XUiC_PrefabFeatureEditorList.FeatureChangedDelegate featureChanged = this.FeatureChanged;
		if (featureChanged == null)
		{
			return;
		}
		featureChanged(this, name, this.FeatureEnabled(name));
	}

	// Token: 0x06006CC7 RID: 27847 RVA: 0x002C73C8 File Offset: 0x002C55C8
	public override void OnOpen()
	{
		base.OnOpen();
		this.RebuildList(false);
	}

	// Token: 0x06006CC8 RID: 27848
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void GetSupportedFeatures();

	// Token: 0x06006CC9 RID: 27849 RVA: 0x002C73D8 File Offset: 0x002C55D8
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		this.groupsResult.Clear();
		this.GetSupportedFeatures();
		foreach (string name in this.groupsResult)
		{
			this.allEntries.Add(new XUiC_PrefabFeatureEditorList.FeatureEntry(this, name));
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x06006CCA RID: 27850 RVA: 0x002C7464 File Offset: 0x002C5664
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_PrefabFeatureEditorList()
	{
	}

	// Token: 0x040052C2 RID: 21186
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput addInput;

	// Token: 0x040052C3 RID: 21187
	public Prefab EditPrefab;

	// Token: 0x040052C4 RID: 21188
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly List<string> groupsResult = new List<string>();

	// Token: 0x02000D92 RID: 3474
	// (Invoke) Token: 0x06006CCC RID: 27852
	public delegate void FeatureChangedDelegate(XUiC_PrefabFeatureEditorList _list, string _featureName, bool _selected);

	// Token: 0x02000D93 RID: 3475
	[Preserve]
	public class FeatureEntry : XUiListEntry<XUiC_PrefabFeatureEditorList.FeatureEntry>
	{
		// Token: 0x06006CCF RID: 27855 RVA: 0x002C7477 File Offset: 0x002C5677
		public FeatureEntry(XUiC_PrefabFeatureEditorList _parentList, string _name)
		{
			this.parentList = _parentList;
			this.Name = _name;
		}

		// Token: 0x06006CD0 RID: 27856 RVA: 0x002C748D File Offset: 0x002C568D
		public override int CompareTo(XUiC_PrefabFeatureEditorList.FeatureEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return 1;
			}
			return string.Compare(this.Name, _otherEntry.Name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06006CD1 RID: 27857 RVA: 0x002C74A8 File Offset: 0x002C56A8
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = this.Name;
				return true;
			}
			if (_bindingName == "selected")
			{
				bool flag = false;
				if (this.parentList.EditPrefab != null)
				{
					flag = this.parentList.FeatureEnabled(this.Name);
				}
				_value = (flag ? "true" : "false");
				return true;
			}
			if (!(_bindingName == "assigned"))
			{
				return false;
			}
			_value = "true";
			return true;
		}

		// Token: 0x06006CD2 RID: 27858 RVA: 0x002C7526 File Offset: 0x002C5726
		public override bool MatchesSearch(string _searchString)
		{
			return this.Name.IndexOf(_searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		// Token: 0x06006CD3 RID: 27859 RVA: 0x002C753C File Offset: 0x002C573C
		[Preserve]
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

		// Token: 0x040052C5 RID: 21189
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XUiC_PrefabFeatureEditorList parentList;

		// Token: 0x040052C6 RID: 21190
		public readonly string Name;
	}
}
