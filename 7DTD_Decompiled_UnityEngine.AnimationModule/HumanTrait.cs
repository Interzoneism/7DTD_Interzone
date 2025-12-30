using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	// Token: 0x02000038 RID: 56
	[NativeHeader("Modules/Animation/HumanTrait.h")]
	public class HumanTrait
	{
		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000269 RID: 617
		public static extern int MuscleCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		// Token: 0x0600026A RID: 618
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetBoneIndexFromMono(int humanId);

		// Token: 0x0600026B RID: 619
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetBoneIndexToMono(int boneIndex);

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x0600026C RID: 620
		public static extern string[] MuscleName { [NativeMethod("GetMuscleNames")] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x0600026D RID: 621
		public static extern int BoneCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x0600026E RID: 622
		public static extern string[] BoneName { [NativeMethod("MonoBoneNames")] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		// Token: 0x0600026F RID: 623 RVA: 0x000042E0 File Offset: 0x000024E0
		public static int MuscleFromBone(int i, int dofIndex)
		{
			return HumanTrait.Internal_MuscleFromBone(HumanTrait.GetBoneIndexFromMono(i), dofIndex);
		}

		// Token: 0x06000270 RID: 624
		[NativeMethod("MuscleFromBone")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_MuscleFromBone(int i, int dofIndex);

		// Token: 0x06000271 RID: 625 RVA: 0x00004300 File Offset: 0x00002500
		public static int BoneFromMuscle(int i)
		{
			return HumanTrait.GetBoneIndexToMono(HumanTrait.Internal_BoneFromMuscle(i));
		}

		// Token: 0x06000272 RID: 626
		[NativeMethod("BoneFromMuscle")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_BoneFromMuscle(int i);

		// Token: 0x06000273 RID: 627 RVA: 0x00004320 File Offset: 0x00002520
		public static bool RequiredBone(int i)
		{
			return HumanTrait.Internal_RequiredBone(HumanTrait.GetBoneIndexFromMono(i));
		}

		// Token: 0x06000274 RID: 628
		[NativeMethod("RequiredBone")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_RequiredBone(int i);

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000275 RID: 629
		public static extern int RequiredBoneCount { [NativeMethod("RequiredBoneCount")] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		// Token: 0x06000276 RID: 630
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetMuscleDefaultMin(int i);

		// Token: 0x06000277 RID: 631
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetMuscleDefaultMax(int i);

		// Token: 0x06000278 RID: 632 RVA: 0x00004340 File Offset: 0x00002540
		public static float GetBoneDefaultHierarchyMass(int i)
		{
			return HumanTrait.Internal_GetBoneHierarchyMass(HumanTrait.GetBoneIndexFromMono(i));
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00004360 File Offset: 0x00002560
		public static int GetParentBone(int i)
		{
			int num = HumanTrait.Internal_GetParent(HumanTrait.GetBoneIndexFromMono(i));
			return (num != -1) ? HumanTrait.GetBoneIndexToMono(num) : -1;
		}

		// Token: 0x0600027A RID: 634
		[NativeMethod("GetBoneHierarchyMass")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float Internal_GetBoneHierarchyMass(int i);

		// Token: 0x0600027B RID: 635
		[NativeMethod("GetParent")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetParent(int i);
	}
}
