using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200056C RID: 1388
[Preserve]
public class ItemClassWaterContainer : ItemClass
{
	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x06002CEC RID: 11500 RVA: 0x0012B9B4 File Offset: 0x00129BB4
	// (set) Token: 0x06002CED RID: 11501 RVA: 0x0012B9BC File Offset: 0x00129BBC
	public int MaxMass { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06002CEE RID: 11502 RVA: 0x0012B9C8 File Offset: 0x00129BC8
	public override void Init()
	{
		base.Init();
		float num = 0f;
		this.Properties.ParseFloat("WaterCapacity", ref num);
		this.MaxMass = Mathf.Clamp((int)(num * 19500f), 0, 65535);
		this.Properties.ParseFloat("InitialFillRatio", ref this.initialFillRatio);
		this.initialFillRatio = Mathf.Clamp(this.initialFillRatio, 0f, 1f);
	}

	// Token: 0x06002CEF RID: 11503 RVA: 0x0012BA3D File Offset: 0x00129C3D
	public override int GetInitialMetadata(ItemValue _itemValue)
	{
		return (int)((float)this.MaxMass * this.initialFillRatio);
	}

	// Token: 0x04002380 RID: 9088
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PropWaterCapacity = "WaterCapacity";

	// Token: 0x04002381 RID: 9089
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PropInitFillRatio = "InitialFillRatio";

	// Token: 0x04002383 RID: 9091
	[PublicizedFrom(EAccessModifier.Private)]
	public float initialFillRatio;
}
