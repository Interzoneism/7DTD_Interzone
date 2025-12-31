using System;

// Token: 0x02001091 RID: 4241
public class UpdateLightOnAllMaterials : UpdateLight
{
	// Token: 0x060085E4 RID: 34276 RVA: 0x00366343 File Offset: 0x00364543
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEnable()
	{
		base.OnEnable();
		if (!GameManager.IsDedicatedServer && GameLightManager.Instance != null)
		{
			GameLightManager.Instance.AddUpdateLight(this);
		}
	}

	// Token: 0x060085E5 RID: 34277 RVA: 0x00366364 File Offset: 0x00364564
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnDisable()
	{
		base.OnDisable();
		if (!GameManager.IsDedicatedServer && GameLightManager.Instance != null)
		{
			GameLightManager.Instance.RemoveUpdateLight(this);
		}
	}
}
