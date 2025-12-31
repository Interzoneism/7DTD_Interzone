using System;
using System.Globalization;
using UnityEngine;

// Token: 0x020006AB RID: 1707
public class NavObjectMapSettings : NavObjectSettings
{
	// Token: 0x06003265 RID: 12901 RVA: 0x00155E7C File Offset: 0x0015407C
	public override void Init()
	{
		base.Init();
		this.Properties.ParseInt("layer", ref this.Layer);
		if (this.Properties.Values.ContainsKey("icon_scale"))
		{
			this.IconScale = StringParsers.ParseFloat(this.Properties.Values["icon_scale"], 0, -1, NumberStyles.Any);
			this.IconScaleVector = new Vector3(this.IconScale, this.IconScale, this.IconScale);
		}
		this.Properties.ParseBool("adjust_center", ref this.AdjustCenter);
		this.Properties.ParseBool("use_rotation", ref this.UseRotation);
	}

	// Token: 0x04002956 RID: 10582
	public int Layer;

	// Token: 0x04002957 RID: 10583
	public float IconScale = 1f;

	// Token: 0x04002958 RID: 10584
	public Vector3 IconScaleVector = Vector3.one;

	// Token: 0x04002959 RID: 10585
	public bool UseRotation;

	// Token: 0x0400295A RID: 10586
	public bool AdjustCenter;
}
