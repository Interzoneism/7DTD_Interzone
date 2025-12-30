using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x0200006A RID: 106
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
	[NativeHeader("Modules/Animation/Constraints/LookAtConstraint.h")]
	[UsedByNativeCode]
	public sealed class LookAtConstraint : Behaviour, IConstraint, IConstraintInternal
	{
		// Token: 0x060005FA RID: 1530 RVA: 0x00008191 File Offset: 0x00006391
		private LookAtConstraint()
		{
			LookAtConstraint.Internal_Create(this);
		}

		// Token: 0x060005FB RID: 1531
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] LookAtConstraint self);

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x060005FC RID: 1532
		// (set) Token: 0x060005FD RID: 1533
		public extern float weight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x060005FE RID: 1534
		// (set) Token: 0x060005FF RID: 1535
		public extern float roll { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000600 RID: 1536
		// (set) Token: 0x06000601 RID: 1537
		public extern bool constraintActive { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000602 RID: 1538
		// (set) Token: 0x06000603 RID: 1539
		public extern bool locked { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000604 RID: 1540 RVA: 0x000081A4 File Offset: 0x000063A4
		// (set) Token: 0x06000605 RID: 1541 RVA: 0x000081BA File Offset: 0x000063BA
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

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000606 RID: 1542 RVA: 0x000081C4 File Offset: 0x000063C4
		// (set) Token: 0x06000607 RID: 1543 RVA: 0x000081DA File Offset: 0x000063DA
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

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000608 RID: 1544
		// (set) Token: 0x06000609 RID: 1545
		public extern Transform worldUpObject { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x0600060A RID: 1546
		// (set) Token: 0x0600060B RID: 1547
		public extern bool useUpObject { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x000081E4 File Offset: 0x000063E4
		public int sourceCount
		{
			get
			{
				return LookAtConstraint.GetSourceCountInternal(this);
			}
		}

		// Token: 0x0600060D RID: 1549
		[FreeFunction("ConstraintBindings::GetSourceCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSourceCountInternal([NotNull("ArgumentNullException")] LookAtConstraint self);

		// Token: 0x0600060E RID: 1550
		[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetSources([NotNull("ArgumentNullException")] List<ConstraintSource> sources);

		// Token: 0x0600060F RID: 1551 RVA: 0x000081FC File Offset: 0x000063FC
		public void SetSources(List<ConstraintSource> sources)
		{
			bool flag = sources == null;
			if (flag)
			{
				throw new ArgumentNullException("sources");
			}
			LookAtConstraint.SetSourcesInternal(this, sources);
		}

		// Token: 0x06000610 RID: 1552
		[FreeFunction("ConstraintBindings::SetSources", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSourcesInternal([NotNull("ArgumentNullException")] LookAtConstraint self, List<ConstraintSource> sources);

		// Token: 0x06000611 RID: 1553 RVA: 0x00008225 File Offset: 0x00006425
		public int AddSource(ConstraintSource source)
		{
			return this.AddSource_Injected(ref source);
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x0000822F File Offset: 0x0000642F
		public void RemoveSource(int index)
		{
			this.ValidateSourceIndex(index);
			this.RemoveSourceInternal(index);
		}

		// Token: 0x06000613 RID: 1555
		[NativeName("RemoveSource")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveSourceInternal(int index);

		// Token: 0x06000614 RID: 1556 RVA: 0x00008244 File Offset: 0x00006444
		public ConstraintSource GetSource(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetSourceInternal(index);
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x00008268 File Offset: 0x00006468
		[NativeName("GetSource")]
		private ConstraintSource GetSourceInternal(int index)
		{
			ConstraintSource result;
			this.GetSourceInternal_Injected(index, out result);
			return result;
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x0000827F File Offset: 0x0000647F
		public void SetSource(int index, ConstraintSource source)
		{
			this.ValidateSourceIndex(index);
			this.SetSourceInternal(index, source);
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x00008293 File Offset: 0x00006493
		[NativeName("SetSource")]
		private void SetSourceInternal(int index, ConstraintSource source)
		{
			this.SetSourceInternal_Injected(index, ref source);
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x000082A0 File Offset: 0x000064A0
		private void ValidateSourceIndex(int index)
		{
			bool flag = this.sourceCount == 0;
			if (flag)
			{
				throw new InvalidOperationException("The LookAtConstraint component has no sources.");
			}
			bool flag2 = index < 0 || index >= this.sourceCount;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Constraint source index {0} is out of bounds (0-{1}).", index, this.sourceCount));
			}
		}

		// Token: 0x06000619 RID: 1561
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rotationAtRest_Injected(out Vector3 ret);

		// Token: 0x0600061A RID: 1562
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_rotationAtRest_Injected(ref Vector3 value);

		// Token: 0x0600061B RID: 1563
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rotationOffset_Injected(out Vector3 ret);

		// Token: 0x0600061C RID: 1564
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_rotationOffset_Injected(ref Vector3 value);

		// Token: 0x0600061D RID: 1565
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int AddSource_Injected(ref ConstraintSource source);

		// Token: 0x0600061E RID: 1566
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetSourceInternal_Injected(int index, out ConstraintSource ret);

		// Token: 0x0600061F RID: 1567
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSourceInternal_Injected(int index, ref ConstraintSource source);
	}
}
