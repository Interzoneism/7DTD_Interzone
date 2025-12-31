using System;
using UnityEngine.Scripting;

// Token: 0x02000559 RID: 1369
[Preserve]
public class AnimationDelayData
{
	// Token: 0x06002C2E RID: 11310 RVA: 0x00127154 File Offset: 0x00125354
	public static void InitStatic()
	{
		AnimationDelayData.AnimationDelay = new AnimationDelayData.AnimationDelays[100];
		for (int i = 0; i < AnimationDelayData.AnimationDelay.Length; i++)
		{
			AnimationDelayData.AnimationDelay[i] = new AnimationDelayData.AnimationDelays(0f, 0f, Constants.cMinHolsterTime, Constants.cMinUnHolsterTime, false);
		}
	}

	// Token: 0x06002C2F RID: 11311 RVA: 0x001271A4 File Offset: 0x001253A4
	public static void Cleanup()
	{
		AnimationDelayData.InitStatic();
	}

	// Token: 0x04002271 RID: 8817
	public static AnimationDelayData.AnimationDelays[] AnimationDelay;

	// Token: 0x0200055A RID: 1370
	public struct AnimationDelays
	{
		// Token: 0x06002C31 RID: 11313 RVA: 0x001271AB File Offset: 0x001253AB
		public AnimationDelays(float _rayCast, float _rayCastMoving, float _holster, float _unholster, bool _twoHanded)
		{
			this.RayCast = _rayCast;
			this.RayCastMoving = _rayCastMoving;
			this.Holster = _holster;
			this.Unholster = _unholster;
			this.TwoHanded = _twoHanded;
		}

		// Token: 0x04002272 RID: 8818
		public float RayCast;

		// Token: 0x04002273 RID: 8819
		public float RayCastMoving;

		// Token: 0x04002274 RID: 8820
		public float Holster;

		// Token: 0x04002275 RID: 8821
		public float Unholster;

		// Token: 0x04002276 RID: 8822
		public bool TwoHanded;
	}
}
