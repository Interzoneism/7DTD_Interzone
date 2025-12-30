using System;
using UnityEngine;

// Token: 0x020006AA RID: 1706
public class NavObjectSettings
{
	// Token: 0x06003263 RID: 12899 RVA: 0x00155DC0 File Offset: 0x00153FC0
	public virtual void Init()
	{
		this.Properties.ParseString("sprite_name", ref this.SpriteName);
		this.Properties.ParseFloat("min_distance", ref this.MinDistance);
		this.Properties.ParseFloat("max_distance", ref this.MaxDistance);
		this.Properties.ParseVec("offset", ref this.Offset);
		this.Properties.ParseColor("color", ref this.Color);
		this.Properties.ParseBool("has_pulse", ref this.HasPulse);
	}

	// Token: 0x0400294F RID: 10575
	public DynamicProperties Properties;

	// Token: 0x04002950 RID: 10576
	public string SpriteName = "";

	// Token: 0x04002951 RID: 10577
	public float MinDistance;

	// Token: 0x04002952 RID: 10578
	public float MaxDistance = -1f;

	// Token: 0x04002953 RID: 10579
	public Vector3 Offset;

	// Token: 0x04002954 RID: 10580
	public Color Color = Color.white;

	// Token: 0x04002955 RID: 10581
	public bool HasPulse;
}
