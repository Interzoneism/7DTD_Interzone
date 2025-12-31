using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x02000069 RID: 105
	[UsedByNativeCode]
	[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
	[NativeHeader("Modules/Animation/Constraints/ScaleConstraint.h")]
	[RequireComponent(typeof(Transform))]
	public sealed class ScaleConstraint : Behaviour, IConstraint, IConstraintInternal
	{
		// Token: 0x060005D8 RID: 1496 RVA: 0x0000801D File Offset: 0x0000621D
		private ScaleConstraint()
		{
			ScaleConstraint.Internal_Create(this);
		}

		// Token: 0x060005D9 RID: 1497
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] ScaleConstraint self);

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060005DA RID: 1498
		// (set) Token: 0x060005DB RID: 1499
		public extern float weight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060005DC RID: 1500 RVA: 0x00008030 File Offset: 0x00006230
		// (set) Token: 0x060005DD RID: 1501 RVA: 0x00008046 File Offset: 0x00006246
		public Vector3 scaleAtRest
		{
			get
			{
				Vector3 result;
				this.get_scaleAtRest_Injected(out result);
				return result;
			}
			set
			{
				this.set_scaleAtRest_Injected(ref value);
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060005DE RID: 1502 RVA: 0x00008050 File Offset: 0x00006250
		// (set) Token: 0x060005DF RID: 1503 RVA: 0x00008066 File Offset: 0x00006266
		public Vector3 scaleOffset
		{
			get
			{
				Vector3 result;
				this.get_scaleOffset_Injected(out result);
				return result;
			}
			set
			{
				this.set_scaleOffset_Injected(ref value);
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060005E0 RID: 1504
		// (set) Token: 0x060005E1 RID: 1505
		public extern Axis scalingAxis { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x060005E2 RID: 1506
		// (set) Token: 0x060005E3 RID: 1507
		public extern bool constraintActive { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x060005E4 RID: 1508
		// (set) Token: 0x060005E5 RID: 1509
		public extern bool locked { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x060005E6 RID: 1510 RVA: 0x00008070 File Offset: 0x00006270
		public int sourceCount
		{
			get
			{
				return ScaleConstraint.GetSourceCountInternal(this);
			}
		}

		// Token: 0x060005E7 RID: 1511
		[FreeFunction("ConstraintBindings::GetSourceCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSourceCountInternal([NotNull("ArgumentNullException")] ScaleConstraint self);

		// Token: 0x060005E8 RID: 1512
		[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetSources([NotNull("ArgumentNullException")] List<ConstraintSource> sources);

		// Token: 0x060005E9 RID: 1513 RVA: 0x00008088 File Offset: 0x00006288
		public void SetSources(List<ConstraintSource> sources)
		{
			bool flag = sources == null;
			if (flag)
			{
				throw new ArgumentNullException("sources");
			}
			ScaleConstraint.SetSourcesInternal(this, sources);
		}

		// Token: 0x060005EA RID: 1514
		[FreeFunction("ConstraintBindings::SetSources", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSourcesInternal([NotNull("ArgumentNullException")] ScaleConstraint self, List<ConstraintSource> sources);

		// Token: 0x060005EB RID: 1515 RVA: 0x000080B1 File Offset: 0x000062B1
		public int AddSource(ConstraintSource source)
		{
			return this.AddSource_Injected(ref source);
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x000080BB File Offset: 0x000062BB
		public void RemoveSource(int index)
		{
			this.ValidateSourceIndex(index);
			this.RemoveSourceInternal(index);
		}

		// Token: 0x060005ED RID: 1517
		[NativeName("RemoveSource")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveSourceInternal(int index);

		// Token: 0x060005EE RID: 1518 RVA: 0x000080D0 File Offset: 0x000062D0
		public ConstraintSource GetSource(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetSourceInternal(index);
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x000080F4 File Offset: 0x000062F4
		[NativeName("GetSource")]
		private ConstraintSource GetSourceInternal(int index)
		{
			ConstraintSource result;
			this.GetSourceInternal_Injected(index, out result);
			return result;
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x0000810B File Offset: 0x0000630B
		public void SetSource(int index, ConstraintSource source)
		{
			this.ValidateSourceIndex(index);
			this.SetSourceInternal(index, source);
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x0000811F File Offset: 0x0000631F
		[NativeName("SetSource")]
		private void SetSourceInternal(int index, ConstraintSource source)
		{
			this.SetSourceInternal_Injected(index, ref source);
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x0000812C File Offset: 0x0000632C
		private void ValidateSourceIndex(int index)
		{
			bool flag = this.sourceCount == 0;
			if (flag)
			{
				throw new InvalidOperationException("The ScaleConstraint component has no sources.");
			}
			bool flag2 = index < 0 || index >= this.sourceCount;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Constraint source index {0} is out of bounds (0-{1}).", index, this.sourceCount));
			}
		}

		// Token: 0x060005F3 RID: 1523
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_scaleAtRest_Injected(out Vector3 ret);

		// Token: 0x060005F4 RID: 1524
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_scaleAtRest_Injected(ref Vector3 value);

		// Token: 0x060005F5 RID: 1525
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_scaleOffset_Injected(out Vector3 ret);

		// Token: 0x060005F6 RID: 1526
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_scaleOffset_Injected(ref Vector3 value);

		// Token: 0x060005F7 RID: 1527
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int AddSource_Injected(ref ConstraintSource source);

		// Token: 0x060005F8 RID: 1528
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetSourceInternal_Injected(int index, out ConstraintSource ret);

		// Token: 0x060005F9 RID: 1529
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSourceInternal_Injected(int index, ref ConstraintSource source);
	}
}
