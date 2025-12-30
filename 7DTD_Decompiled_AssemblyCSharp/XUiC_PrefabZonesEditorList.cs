using System;
using UnityEngine.Scripting;

// Token: 0x02000DA4 RID: 3492
[Preserve]
public class XUiC_PrefabZonesEditorList : XUiC_PrefabFeatureEditorList
{
	// Token: 0x06006D40 RID: 27968 RVA: 0x002C96AA File Offset: 0x002C78AA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool FeatureEnabled(string _featureName)
	{
		return this.EditPrefab.IsAllowedZone(_featureName);
	}

	// Token: 0x06006D41 RID: 27969 RVA: 0x002C96B8 File Offset: 0x002C78B8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AddNewFeature(string _featureName)
	{
		this.EditPrefab.AddAllowedZone(_featureName);
	}

	// Token: 0x06006D42 RID: 27970 RVA: 0x002C96C6 File Offset: 0x002C78C6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ToggleFeature(string _featureName)
	{
		if (this.EditPrefab.IsAllowedZone(_featureName))
		{
			this.EditPrefab.RemoveAllowedZone(_featureName);
			return;
		}
		this.EditPrefab.AddAllowedZone(_featureName);
	}

	// Token: 0x06006D43 RID: 27971 RVA: 0x002C96EF File Offset: 0x002C78EF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void GetSupportedFeatures()
	{
		PrefabEditModeManager.Instance.GetAllZones(this.groupsResult, this.EditPrefab);
	}
}
