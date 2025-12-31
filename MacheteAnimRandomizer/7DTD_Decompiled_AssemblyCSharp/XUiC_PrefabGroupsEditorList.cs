using System;
using UnityEngine.Scripting;

// Token: 0x02000D99 RID: 3481
[Preserve]
public class XUiC_PrefabGroupsEditorList : XUiC_PrefabFeatureEditorList
{
	// Token: 0x06006CEF RID: 27887 RVA: 0x002C7B60 File Offset: 0x002C5D60
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool FeatureEnabled(string _featureName)
	{
		return this.EditPrefab.editorGroups.Contains(_featureName);
	}

	// Token: 0x06006CF0 RID: 27888 RVA: 0x002C7B73 File Offset: 0x002C5D73
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AddNewFeature(string _featureName)
	{
		this.EditPrefab.editorGroups.Add(_featureName);
	}

	// Token: 0x06006CF1 RID: 27889 RVA: 0x002C7B86 File Offset: 0x002C5D86
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ToggleFeature(string _featureName)
	{
		if (this.EditPrefab.editorGroups.Contains(_featureName))
		{
			this.EditPrefab.editorGroups.Remove(_featureName);
			return;
		}
		this.EditPrefab.editorGroups.Add(_featureName);
	}

	// Token: 0x06006CF2 RID: 27890 RVA: 0x002C7BBF File Offset: 0x002C5DBF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void GetSupportedFeatures()
	{
		PrefabEditModeManager.Instance.GetAllGroups(this.groupsResult, this.EditPrefab);
	}
}
