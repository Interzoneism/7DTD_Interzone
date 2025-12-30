using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x02000068 RID: 104
	[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
	[UsedByNativeCode]
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/Animation/Constraints/RotationConstraint.h")]
	public sealed class RotationConstraint : Behaviour, IConstraint, IConstraintInternal
	{
		// Token: 0x060005B6 RID: 1462 RVA: 0x00007EA9 File Offset: 0x000060A9
		private RotationConstraint()
		{
			RotationConstraint.Internal_Create(this);
		}

		// Token: 0x060005B7 RID: 1463
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] RotationConstraint self);

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x060005B8 RID: 1464
		// (set) Token: 0x060005B9 RID: 1465
		public extern float weight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x00007EBC File Offset: 0x000060BC
		// (set) Token: 0x060005BB RID: 1467 RVA: 0x00007ED2 File Offset: 0x000060D2
		public Vector3 rotationAtRest
		{
			get
			{
				Vector3 result;
				this.get_rotationAtRest_Injected(out result);
				return result;
			}
			set
			{
				this.set_rotationAtRest_Injected(ref value);
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x00007EDC File Offset: 0x000060DC
		// (set) Token: 0x060005BD RID: 1469 RVA: 0x00007EF2 File Offset: 0x000060F2
		public Vector3 rotationOffset
		{
			get
			{
				Vector3 result;
				this.get_rotationOffset_Injected(out result);
				return result;
			}
			set
			{
				this.set_rotationOffset_Injected(ref value);
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x060005BE RID: 1470
		// (set) Token: 0x060005BF RID: 1471
		public extern Axis rotationAxis { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x060005C0 RID: 1472
		// (set) Token: 0x060005C1 RID: 1473
		public extern bool constraintActive { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060005C2 RID: 1474
		// (set) Token: 0x060005C3 RID: 1475
		public extern bool locked { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060005C4 RID: 1476 RVA: 0x00007EFC File Offset: 0x000060FC
		public int sourceCount
		{
			get
			{
				return RotationConstraint.GetSourceCountInternal(this);
			}
		}

		// Token: 0x060005C5 RID: 1477
		[FreeFunction("ConstraintBindings::GetSourceCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSourceCountInternal([NotNull("ArgumentNullException")] RotationConstraint self);

		// Token: 0x060005C6 RID: 1478
		[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetSources([NotNull("ArgumentNullException")] List<ConstraintSource> sources);

		// Token: 0x060005C7 RID: 1479 RVA: 0x00007F14 File Offset: 0x00006114
		public void SetSources(List<ConstraintSource> sources)
		{
			bool flag = sources == null;
			if (flag)
			{
				throw new ArgumentNullException("sources");
			}
			RotationConstraint.SetSourcesInternal(this, sources);
		}

		// Token: 0x060005C8 RID: 1480
		[FreeFunction("ConstraintBindings::SetSources", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSourcesInternal([NotNull("ArgumentNullException")] RotationConstraint self, List<ConstraintSource> sources);

		// Token: 0x060005C9 RID: 1481 RVA: 0x00007F3D File Offset: 0x0000613D
		public int AddSource(ConstraintSource source)
		{
			return this.AddSource_Injected(ref source);
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00007F47 File Offset: 0x00006147
		public void RemoveSource(int index)
		{
			this.ValidateSourceIndex(index);
			this.RemoveSourceInternal(index);
		}

		// Token: 0x060005CB RID: 1483
		[NativeName("RemoveSource")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveSourceInternal(int index);

		// Token: 0x060005CC RID: 1484 RVA: 0x00007F5C File Offset: 0x0000615C
		public ConstraintSource GetSource(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetSourceInternal(index);
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x00007F80 File Offset: 0x00006180
		[NativeName("GetSource")]
		private ConstraintSource GetSourceInternal(int index)
		{
			ConstraintSource result;
			this.GetSourceInternal_Injected(index, out result);
			return result;
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x00007F97 File Offset: 0x00006197
		public void SetSource(int index, ConstraintSource source)
		{
			this.ValidateSourceIndex(index);
			this.SetSourceInternal(index, source);
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x00007FAB File Offset: 0x000061AB
		[NativeName("SetSource")]
		private void SetSourceInternal(int index, ConstraintSource source)
		{
			this.SetSourceInternal_Injected(index, ref source);
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x00007FB8 File Offset: 0x000061B8
		private void ValidateSourceIndex(int index)
		{
			bool flag = this.sourceCount == 0;
			if (flag)
			{
				throw new InvalidOperationException("The RotationConstraint component has no sources.");
			}
			bool flag2 = index < 0 || index >= this.sourceCount;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Constraint source index {0} is out of bounds (0-{1}).", index, this.sourceCount));
			}
		}

		// Token: 0x060005D1 RID: 1489
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rotationAtRest_Injected(out Vector3 ret);

		// Token: 0x060005D2 RID: 1490
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_rotationAtRest_Injected(ref Vector3 value);

		// Token: 0x060005D3 RID: 1491
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rotationOffset_Injected(out Vector3 ret);

		// Token: 0x060005D4 RID: 1492
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_rotationOffset_Injected(ref Vector3 value);

		// Token: 0x060005D5 RID: 1493
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int AddSource_Injected(ref ConstraintSource source);

		// Token: 0x060005D6 RID: 1494
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetSourceInternal_Injected(int index, out ConstraintSource ret);

		// Token: 0x060005D7 RID: 1495
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSourceInternal_Injected(int index, ref ConstraintSource source);
	}
}
