using System;
using UnityEngine.Scripting;

// Token: 0x02000DA1 RID: 3489
[Preserve]
public class XUiC_PrefabThemeTagList : XUiC_PrefabFeatureEditorList
{
	// Token: 0x06006D32 RID: 27954 RVA: 0x002C92C4 File Offset: 0x002C74C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool FeatureEnabled(string _featureName)
	{
		return this.EditPrefab.ThemeTags.Test_AllSet(FastTags<TagGroup.Poi>.GetTag(_featureName));
	}

	// Token: 0x06006D33 RID: 27955 RVA: 0x002C92EA File Offset: 0x002C74EA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AddNewFeature(string _featureName)
	{
		this.EditPrefab.ThemeTags |= FastTags<TagGroup.Poi>.GetTag(_featureName);
	}

	// Token: 0x06006D34 RID: 27956 RVA: 0x002C9308 File Offset: 0x002C7508
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ToggleFeature(string _featureName)
	{
		FastTags<TagGroup.Poi> tag = FastTags<TagGroup.Poi>.GetTag(_featureName);
		if (this.EditPrefab.ThemeTags.Test_AnySet(tag))
		{
			this.EditPrefab.ThemeTags = this.EditPrefab.ThemeTags.Remove(tag);
		}
		else
		{
			this.EditPrefab.ThemeTags |= tag;
		}
		this.RebuildList(false);
	}

	// Token: 0x06006D35 RID: 27957 RVA: 0x002C9371 File Offset: 0x002C7571
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void GetSupportedFeatures()
	{
		PrefabEditModeManager.Instance.GetAllThemeTags(this.groupsResult, this.EditPrefab);
	}
}
