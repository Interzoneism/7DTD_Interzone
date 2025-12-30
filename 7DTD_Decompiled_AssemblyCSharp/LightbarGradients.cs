using System;
using UnityEngine;

// Token: 0x020011B4 RID: 4532
[CreateAssetMenu(menuName = "Gradients/Lightbar Gradient Data")]
[Serializable]
public class LightbarGradients : ScriptableObject
{
	// Token: 0x04006DEF RID: 28143
	public Gradient timeOfDayGradient;

	// Token: 0x04006DF0 RID: 28144
	public Gradient dayGradient;

	// Token: 0x04006DF1 RID: 28145
	public Gradient nightGradient;

	// Token: 0x04006DF2 RID: 28146
	public Gradient cloudDayGradient;

	// Token: 0x04006DF3 RID: 28147
	public Gradient cloudNightGradient;

	// Token: 0x04006DF4 RID: 28148
	public Gradient bloodmoonGradient;

	// Token: 0x04006DF5 RID: 28149
	public Color mainMenuColor;
}
