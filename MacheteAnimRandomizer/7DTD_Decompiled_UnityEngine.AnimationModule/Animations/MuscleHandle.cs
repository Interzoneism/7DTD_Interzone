using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	// Token: 0x0200006B RID: 107
	[NativeHeader("Modules/Animation/MuscleHandle.h")]
	[MovedFrom("UnityEngine.Experimental.Animations")]
	[NativeHeader("Modules/Animation/Animator.h")]
	public struct MuscleHandle
	{
		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000620 RID: 1568 RVA: 0x00008305 File Offset: 0x00006505
		// (set) Token: 0x06000621 RID: 1569 RVA: 0x0000830D File Offset: 0x0000650D
		public HumanPartDof humanPartDof { readonly get; private set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000622 RID: 1570 RVA: 0x00008316 File Offset: 0x00006516
		// (set) Token: 0x06000623 RID: 1571 RVA: 0x0000831E File Offset: 0x0000651E
		public int dof { readonly get; private set; }

		// Token: 0x06000624 RID: 1572 RVA: 0x00008327 File Offset: 0x00006527
		public MuscleHandle(BodyDof bodyDof)
		{
			this.humanPartDof = HumanPartDof.Body;
			this.dof = (int)bodyDof;
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0000833A File Offset: 0x0000653A
		public MuscleHandle(HeadDof headDof)
		{
			this.humanPartDof = HumanPartDof.Head;
			this.dof = (int)headDof;
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x00008350 File Offset: 0x00006550
		public MuscleHandle(HumanPartDof partDof, LegDof legDof)
		{
			bool flag = partDof != HumanPartDof.LeftLeg && partDof != HumanPartDof.RightLeg;
			if (flag)
			{
				throw new InvalidOperationException("Invalid HumanPartDof for a leg, please use either HumanPartDof.LeftLeg or HumanPartDof.RightLeg.");
			}
			this.humanPartDof = partDof;
			this.dof = (int)legDof;
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x0000838C File Offset: 0x0000658C
		public MuscleHandle(HumanPartDof partDof, ArmDof armDof)
		{
			bool flag = partDof != HumanPartDof.LeftArm && partDof != HumanPartDof.RightArm;
			if (flag)
			{
				throw new InvalidOperationException("Invalid HumanPartDof for an arm, please use either HumanPartDof.LeftArm or HumanPartDof.RightArm.");
			}
			this.humanPartDof = partDof;
			this.dof = (int)armDof;
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x000083C8 File Offset: 0x000065C8
		public MuscleHandle(HumanPartDof partDof, FingerDof fingerDof)
		{
			bool flag = partDof < HumanPartDof.LeftThumb || partDof > HumanPartDof.RightLittle;
			if (flag)
			{
				throw new InvalidOperationException("Invalid HumanPartDof for a finger.");
			}
			this.humanPartDof = partDof;
			this.dof = (int)fingerDof;
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000629 RID: 1577 RVA: 0x00008404 File Offset: 0x00006604
		public string name
		{
			get
			{
				return this.GetName();
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x0600062A RID: 1578 RVA: 0x0000841C File Offset: 0x0000661C
		public static int muscleHandleCount
		{
			get
			{
				return MuscleHandle.GetMuscleHandleCount();
			}
		}

		// Token: 0x0600062B RID: 1579
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetMuscleHandles([NotNull("ArgumentNullException")] [Out] MuscleHandle[] muscleHandles);

		// Token: 0x0600062C RID: 1580 RVA: 0x00008433 File Offset: 0x00006633
		private string GetName()
		{
			return MuscleHandle.GetName_Injected(ref this);
		}

		// Token: 0x0600062D RID: 1581
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMuscleHandleCount();

		// Token: 0x0600062E RID: 1582
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetName_Injected(ref MuscleHandle _unity_self);
	}
}
