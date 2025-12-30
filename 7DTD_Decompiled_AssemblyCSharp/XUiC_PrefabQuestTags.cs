using System;
using UnityEngine.Scripting;

// Token: 0x02000D9F RID: 3487
[Preserve]
public class XUiC_PrefabQuestTags : XUiC_PrefabFeatureEditorList
{
	// Token: 0x06006D28 RID: 27944 RVA: 0x002C91A1 File Offset: 0x002C73A1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool FeatureEnabled(string _featureName)
	{
		return this.EditPrefab.GetQuestTag(FastTags<TagGroup.Global>.Parse(_featureName));
	}

	// Token: 0x06006D29 RID: 27945 RVA: 0x002C91B4 File Offset: 0x002C73B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AddNewFeature(string _featureName)
	{
		this.EditPrefab.ToggleQuestTag(FastTags<TagGroup.Global>.Parse(_featureName));
	}

	// Token: 0x06006D2A RID: 27946 RVA: 0x002C91C7 File Offset: 0x002C73C7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ToggleFeature(string _featureName)
	{
		this.EditPrefab.ToggleQuestTag(FastTags<TagGroup.Global>.GetTag(_featureName));
		this.RebuildList(false);
	}

	// Token: 0x06006D2B RID: 27947 RVA: 0x002C91E1 File Offset: 0x002C73E1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void GetSupportedFeatures()
	{
		PrefabEditModeManager.Instance.GetAllQuestTags(this.groupsResult, this.EditPrefab);
	}
}
