using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x02000049 RID: 73
	[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
	[NativeHeader("Modules/Animation/Constraints/AimConstraint.h")]
	[RequireComponent(typeof(Transform))]
	[UsedByNativeCode]
	public sealed class AimConstraint : Behaviour, IConstraint, IConstraintInternal
	{
		// Token: 0x060002B0 RID: 688 RVA: 0x00004722 File Offset: 0x00002922
		private AimConstraint()
		{
			AimConstraint.Internal_Create(this);
		}

		// Token: 0x060002B1 RID: 689
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] AimConstraint self);

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060002B2 RID: 690
		// (set) Token: 0x060002B3 RID: 691
		public extern float weight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060002B4 RID: 692
		// (set) Token: 0x060002B5 RID: 693
		public extern bool constraintActive { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060002B6 RID: 694
		// (set) Token: 0x060002B7 RID: 695
		public extern bool locked { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x00004734 File Offset: 0x00002934
		// (set) Token: 0x060002B9 RID: 697 RVA: 0x0000474A File Offset: 0x0000294A
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

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060002BA RID: 698 RVA: 0x00004754 File Offset: 0x00002954
		// (set) Token: 0x060002BB RID: 699 RVA: 0x0000476A File Offset: 0x0000296A
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

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060002BC RID: 700
		// (set) Token: 0x060002BD RID: 701
		public extern Axis rotationAxis { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060002BE RID: 702 RVA: 0x00004774 File Offset: 0x00002974
		// (set) Token: 0x060002BF RID: 703 RVA: 0x0000478A File Offset: 0x0000298A
		public Vector3 aimVector
		{
			get
			{
				Vector3 result;
				this.get_aimVector_Injected(out result);
				return result;
			}
			set
			{
				this.set_aimVector_Injected(ref value);
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x00004794 File Offset: 0x00002994
		// (set) Token: 0x060002C1 RID: 705 RVA: 0x000047AA File Offset: 0x000029AA
		public Vector3 upVector
		{
			get
			{
				Vector3 result;
				this.get_upVector_Injected(out result);
				return result;
			}
			set
			{
				this.set_upVector_Injected(ref value);
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x000047B4 File Offset: 0x000029B4
		// (set) Token: 0x060002C3 RID: 707 RVA: 0x000047CA File Offset: 0x000029CA
		public Vector3 worldUpVector
		{
			get
			{
				Vector3 result;
				this.get_worldUpVector_Injected(out result);
				return result;
			}
			set
			{
				this.set_worldUpVector_Injected(ref value);
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060002C4 RID: 708
		// (set) Token: 0x060002C5 RID: 709
		public extern Transform worldUpObject { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060002C6 RID: 710
		// (set) Token: 0x060002C7 RID: 711
		public extern AimConstraint.WorldUpType worldUpType { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x000047D4 File Offset: 0x000029D4
		public int sourceCount
		{
			get
			{
				return AimConstraint.GetSourceCountInternal(this);
			}
		}

		// Token: 0x060002C9 RID: 713
		[FreeFunction("ConstraintBindings::GetSourceCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSourceCountInternal([NotNull("ArgumentNullException")] AimConstraint self);

		// Token: 0x060002CA RID: 714
		[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetSources([NotNull("ArgumentNullException")] List<ConstraintSource> sources);

		// Token: 0x060002CB RID: 715 RVA: 0x000047EC File Offset: 0x000029EC
		public void SetSources(List<ConstraintSource> sources)
		{
			bool flag = sources == null;
			if (flag)
			{
				throw new ArgumentNullException("sources");
			}
			AimConstraint.SetSourcesInternal(this, sources);
		}

		// Token: 0x060002CC RID: 716
		[FreeFunction("ConstraintBindings::SetSources", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSourcesInternal([NotNull("ArgumentNullException")] AimConstraint self, List<ConstraintSource> sources);

		// Token: 0x060002CD RID: 717 RVA: 0x00004815 File Offset: 0x00002A15
		public int AddSource(ConstraintSource source)
		{
			return this.AddSource_Injected(ref source);
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0000481F File Offset: 0x00002A1F
		public void RemoveSource(int index)
		{
			this.ValidateSourceIndex(index);
			this.RemoveSourceInternal(index);
		}

		// Token: 0x060002CF RID: 719
		[NativeName("RemoveSource")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveSourceInternal(int index);

		// Token: 0x060002D0 RID: 720 RVA: 0x00004834 File Offset: 0x00002A34
		public ConstraintSource GetSource(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetSourceInternal(index);
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00004858 File Offset: 0x00002A58
		[NativeName("GetSource")]
		private ConstraintSource GetSourceInternal(int index)
		{
			ConstraintSource result;
			this.GetSourceInternal_Injected(index, out result);
			return result;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0000486F File Offset: 0x00002A6F
		public void SetSource(int index, ConstraintSource source)
		{
			this.ValidateSourceIndex(index);
			this.SetSourceInternal(index, source);
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00004883 File Offset: 0x00002A83
		[NativeName("SetSource")]
		private void SetSourceInternal(int index, ConstraintSource source)
		{
			this.SetSourceInternal_Injected(index, ref source);
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00004890 File Offset: 0x00002A90
		private void ValidateSourceIndex(int index)
		{
			bool flag = this.sourceCount == 0;
			if (flag)
			{
				throw new InvalidOperationException("The AimConstraint component has no sources.");
			}
			bool flag2 = index < 0 || index >= this.sourceCount;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Constraint source index {0} is out of bounds (0-{1}).", index, this.sourceCount));
			}
		}

		// Token: 0x060002D5 RID: 725
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rotationAtRest_Injected(out Vector3 ret);

		// Token: 0x060002D6 RID: 726
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_rotationAtRest_Injected(ref Vector3 value);

		// Token: 0x060002D7 RID: 727
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rotationOffset_Injected(out Vector3 ret);

		// Token: 0x060002D8 RID: 728
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_rotationOffset_Injected(ref Vector3 value);

		// Token: 0x060002D9 RID: 729
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_aimVector_Injected(out Vector3 ret);

		// Token: 0x060002DA RID: 730
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_aimVector_Injected(ref Vector3 value);

		// Token: 0x060002DB RID: 731
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_upVector_Injected(out Vector3 ret);

		// Token: 0x060002DC RID: 732
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_upVector_Injected(ref Vector3 value);

		// Token: 0x060002DD RID: 733
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_worldUpVector_Injected(out Vector3 ret);

		// Token: 0x060002DE RID: 734
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_worldUpVector_Injected(ref Vector3 value);

		// Token: 0x060002DF RID: 735
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int AddSource_Injected(ref ConstraintSource source);

		// Token: 0x060002E0 RID: 736
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetSourceInternal_Injected(int index, out ConstraintSource ret);

		// Token: 0x060002E1 RID: 737
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSourceInternal_Injected(int index, ref ConstraintSource source);

		// Token: 0x0200004A RID: 74
		public enum WorldUpType
		{
			// Token: 0x04000141 RID: 321
			SceneUp,
			// Token: 0x04000142 RID: 322
			ObjectUp,
			// Token: 0x04000143 RID: 323
			ObjectRotationUp,
			// Token: 0x04000144 RID: 324
			Vector,
			// Token: 0x04000145 RID: 325
			None
		}
	}
}
