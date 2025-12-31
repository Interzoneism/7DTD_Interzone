using System;
using UnityEngine;

// Token: 0x020012DA RID: 4826
public sealed class vp_Layer
{
	// Token: 0x0600965B RID: 38491 RVA: 0x003BCC34 File Offset: 0x003BAE34
	[PublicizedFrom(EAccessModifier.Private)]
	static vp_Layer()
	{
		Physics.IgnoreLayerCollision(30, 29);
		Physics.IgnoreLayerCollision(29, 29);
		Physics.IgnoreLayerCollision(22, 23);
	}

	// Token: 0x0600965C RID: 38492 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Private)]
	public vp_Layer()
	{
	}

	// Token: 0x0600965D RID: 38493 RVA: 0x003BCC5C File Offset: 0x003BAE5C
	public static void Set(GameObject obj, int layer, bool recursive = false)
	{
		if (layer < 0 || layer > 31)
		{
			Debug.LogError("vp_Layer: Attempted to set layer id out of range [0-31].");
			return;
		}
		obj.layer = layer;
		if (recursive)
		{
			foreach (object obj2 in obj.transform)
			{
				vp_Layer.Set(((Transform)obj2).gameObject, layer, true);
			}
		}
	}

	// Token: 0x0600965E RID: 38494 RVA: 0x003BCCD8 File Offset: 0x003BAED8
	public static bool IsInMask(int layer, int layerMask)
	{
		return (layerMask & 1 << layer) == 0;
	}

	// Token: 0x04007267 RID: 29287
	public static readonly vp_Layer instance = new vp_Layer();

	// Token: 0x04007268 RID: 29288
	public const int Default = 0;

	// Token: 0x04007269 RID: 29289
	public const int TransparentFX = 1;

	// Token: 0x0400726A RID: 29290
	public const int IgnoreRaycast = 2;

	// Token: 0x0400726B RID: 29291
	public const int Water = 4;

	// Token: 0x0400726C RID: 29292
	public const int Ragdoll = 22;

	// Token: 0x0400726D RID: 29293
	public const int PlayerDamageCollider = 23;

	// Token: 0x0400726E RID: 29294
	public const int IgnoreBullets = 24;

	// Token: 0x0400726F RID: 29295
	public const int Enemy = 25;

	// Token: 0x04007270 RID: 29296
	public const int Pickup = 26;

	// Token: 0x04007271 RID: 29297
	public const int Trigger = 27;

	// Token: 0x04007272 RID: 29298
	public const int MovableObject = 28;

	// Token: 0x04007273 RID: 29299
	public const int Debris = 29;

	// Token: 0x04007274 RID: 29300
	public const int LocalPlayer = 30;

	// Token: 0x04007275 RID: 29301
	public const int Weapon = 10;

	// Token: 0x020012DB RID: 4827
	public static class Mask
	{
		// Token: 0x04007276 RID: 29302
		public const int BulletBlockers = -538750981;

		// Token: 0x04007277 RID: 29303
		public const int ExternalBlockers = 1084850176;

		// Token: 0x04007278 RID: 29304
		public const int CameraBlockers = 1082195968;

		// Token: 0x04007279 RID: 29305
		public const int PhysicsBlockers = 2260992;

		// Token: 0x0400727A RID: 29306
		public const int IgnoreWalkThru = -738197525;
	}
}
