using System;
using UnityEngine.Scripting;

// Token: 0x02000DA0 RID: 3488
[Preserve]
public class XUiC_PrefabTagList : XUiC_PrefabFeatureEditorList
{
	// Token: 0x06006D2D RID: 27949 RVA: 0x002C91FC File Offset: 0x002C73FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool FeatureEnabled(string _featureName)
	{
		return this.EditPrefab.Tags.Test_AllSet(FastTags<TagGroup.Poi>.GetTag(_featureName));
	}

	// Token: 0x06006D2E RID: 27950 RVA: 0x002C9222 File Offset: 0x002C7422
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AddNewFeature(string _featureName)
	{
		this.EditPrefab.Tags |= FastTags<TagGroup.Poi>.GetTag(_featureName);
	}

	// Token: 0x06006D2F RID: 27951 RVA: 0x002C9240 File Offset: 0x002C7440
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ToggleFeature(string _featureName)
	{
		FastTags<TagGroup.Poi> tag = FastTags<TagGroup.Poi>.GetTag(_featureName);
		if (this.EditPrefab.Tags.Test_AnySet(tag))
		{
			this.EditPrefab.Tags = this.EditPrefab.Tags.Remove(tag);
		}
		else
		{
			this.EditPrefab.Tags |= tag;
		}
		this.RebuildList(false);
	}

	// Token: 0x06006D30 RID: 27952 RVA: 0x002C92A9 File Offset: 0x002C74A9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void GetSupportedFeatures()
	{
		PrefabEditModeManager.Instance.GetAllTags(this.groupsResult, this.EditPrefab);
	}
}
