using System;
using UnityEngine;

// Token: 0x0200055B RID: 1371
public class AnimationGunjointOffsetData
{
	// Token: 0x06002C32 RID: 11314 RVA: 0x001271D4 File Offset: 0x001253D4
	public static void InitStatic()
	{
		AnimationGunjointOffsetData.AnimationGunjointOffset = new AnimationGunjointOffsetData.AnimationGunjointOffsets[100];
		for (int i = 0; i < AnimationGunjointOffsetData.AnimationGunjointOffset.Length; i++)
		{
			AnimationGunjointOffsetData.AnimationGunjointOffset[i] = new AnimationGunjointOffsetData.AnimationGunjointOffsets(Vector3.zero, Vector3.zero);
		}
	}

	// Token: 0x06002C33 RID: 11315 RVA: 0x00127219 File Offset: 0x00125419
	public static void Cleanup()
	{
		AnimationGunjointOffsetData.InitStatic();
	}

	// Token: 0x04002277 RID: 8823
	public static AnimationGunjointOffsetData.AnimationGunjointOffsets[] AnimationGunjointOffset;

	// Token: 0x0200055C RID: 1372
	public struct AnimationGunjointOffsets
	{
		// Token: 0x06002C35 RID: 11317 RVA: 0x00127220 File Offset: 0x00125420
		public AnimationGunjointOffsets(Vector3 _position, Vector3 _rotation)
		{
			this.position = _position;
			this.rotation = _rotation;
		}

		// Token: 0x04002278 RID: 8824
		public Vector3 position;

		// Token: 0x04002279 RID: 8825
		public Vector3 rotation;
	}
}
