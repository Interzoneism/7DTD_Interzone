using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x0200006C RID: 108
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/Animation/Constraints/ParentConstraint.h")]
	[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
	[UsedByNativeCode]
	public sealed class ParentConstraint : Behaviour, IConstraint, IConstraintInternal
	{
		// Token: 0x0600062F RID: 1583 RVA: 0x0000843B File Offset: 0x0000663B
		private ParentConstraint()
		{
			ParentConstraint.Internal_Create(this);
		}

		// Token: 0x06000630 RID: 1584
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] ParentConstraint self);

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000631 RID: 1585
		// (set) Token: 0x06000632 RID: 1586
		public extern float weight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000633 RID: 1587
		// (set) Token: 0x06000634 RID: 1588
		public extern bool constraintActive { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000635 RID: 1589
		// (set) Token: 0x06000636 RID: 1590
		public extern bool locked { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000637 RID: 1591 RVA: 0x0000844C File Offset: 0x0000664C
		public int sourceCount
		{
			get
			{
				return ParentConstraint.GetSourceCountInternal(this);
			}
		}

		// Token: 0x06000638 RID: 1592
		[FreeFunction("ConstraintBindings::GetSourceCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSourceCountInternal([NotNull("ArgumentNullException")] ParentConstraint self);

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000639 RID: 1593 RVA: 0x00008464 File Offset: 0x00006664
		// (set) Token: 0x0600063A RID: 1594 RVA: 0x0000847A File Offset: 0x0000667A
		public Vector3 translationAtRest
		{
			get
			{
				Vector3 result;
				this.get_translationAtRest_Injected(out result);
				return result;
			}
			set
			{
				this.set_translationAtRest_Injected(ref value);
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x0600063B RID: 1595 RVA: 0x00008484 File Offset: 0x00006684
		// (set) Token: 0x0600063C RID: 1596 RVA: 0x0000849A File Offset: 0x0000669A
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

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x0600063D RID: 1597
		// (set) Token: 0x0600063E RID: 1598
		public extern Vector3[] translationOffsets { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x0600063F RID: 1599
		// (set) Token: 0x06000640 RID: 1600
		public extern Vector3[] rotationOffsets { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000641 RID: 1601
		// (set) Token: 0x06000642 RID: 1602
		public extern Axis translationAxis { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000643 RID: 1603
		// (set) Token: 0x06000644 RID: 1604
		public extern Axis rotationAxis { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x06000645 RID: 1605 RVA: 0x000084A4 File Offset: 0x000066A4
		public Vector3 GetTranslationOffset(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetTranslationOffsetInternal(index);
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x000084C5 File Offset: 0x000066C5
		public void SetTranslationOffset(int index, Vector3 value)
		{
			this.ValidateSourceIndex(index);
			this.SetTranslationOffsetInternal(index, value);
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x000084DC File Offset: 0x000066DC
		[NativeName("GetTranslationOffset")]
		private Vector3 GetTranslationOffsetInternal(int index)
		{
			Vector3 result;
			this.GetTranslationOffsetInternal_Injected(index, out result);
			return result;
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x000084F3 File Offset: 0x000066F3
		[NativeName("SetTranslationOffset")]
		private void SetTranslationOffsetInternal(int index, Vector3 value)
		{
			this.SetTranslationOffsetInternal_Injected(index, ref value);
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00008500 File Offset: 0x00006700
		public Vector3 GetRotationOffset(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetRotationOffsetInternal(index);
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00008521 File Offset: 0x00006721
		public void SetRotationOffset(int index, Vector3 value)
		{
			this.ValidateSourceIndex(index);
			this.SetRotationOffsetInternal(index, value);
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00008538 File Offset: 0x00006738
		[NativeName("GetRotationOffset")]
		private Vector3 GetRotationOffsetInternal(int index)
		{
			Vector3 result;
			this.GetRotationOffsetInternal_Injected(index, out result);
			return result;
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x0000854F File Offset: 0x0000674F
		[NativeName("SetRotationOffset")]
		private void SetRotationOffsetInternal(int index, Vector3 value)
		{
			this.SetRotationOffsetInternal_Injected(index, ref value);
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x0000855C File Offset: 0x0000675C
		private void ValidateSourceIndex(int index)
		{
			bool flag = this.sourceCount == 0;
			if (flag)
			{
				throw new InvalidOperationException("The ParentConstraint component has no sources.");
			}
			bool flag2 = index < 0 || index >= this.sourceCount;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Constraint source index {0} is out of bounds (0-{1}).", index, this.sourceCount));
			}
		}

		// Token: 0x0600064E RID: 1614
		[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetSources([NotNull("ArgumentNullException")] List<ConstraintSource> sources);

		// Token: 0x0600064F RID: 1615 RVA: 0x000085C4 File Offset: 0x000067C4
		public void SetSources(List<ConstraintSource> sources)
		{
			bool flag = sources == null;
			if (flag)
			{
				throw new ArgumentNullException("sources");
			}
			ParentConstraint.SetSourcesInternal(this, sources);
		}

		// Token: 0x06000650 RID: 1616
		[FreeFunction("ConstraintBindings::SetSources", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSourcesInternal([NotNull("ArgumentNullException")] ParentConstraint self, List<ConstraintSource> sources);

		// Token: 0x06000651 RID: 1617 RVA: 0x000085ED File Offset: 0x000067ED
		public int AddSource(ConstraintSource source)
		{
			return this.AddSource_Injected(ref source);
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x000085F7 File Offset: 0x000067F7
		public void RemoveSource(int index)
		{
			this.ValidateSourceIndex(index);
			this.RemoveSourceInternal(index);
		}

		// Token: 0x06000653 RID: 1619
		[NativeName("RemoveSource")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveSourceInternal(int index);

		// Token: 0x06000654 RID: 1620 RVA: 0x0000860C File Offset: 0x0000680C
		public ConstraintSource GetSource(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetSourceInternal(index);
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00008630 File Offset: 0x00006830
		[NativeName("GetSource")]
		private ConstraintSource GetSourceInternal(int index)
		{
			ConstraintSource result;
			this.GetSourceInternal_Injected(index, out result);
			return result;
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00008647 File Offset: 0x00006847
		public void SetSource(int index, ConstraintSource source)
		{
			this.ValidateSourceIndex(index);
			this.SetSourceInternal(index, source);
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x0000865B File Offset: 0x0000685B
		[NativeName("SetSource")]
		private void SetSourceInternal(int index, ConstraintSource source)
		{
			this.SetSourceInternal_Injected(index, ref source);
		}

		// Token: 0x06000658 RID: 1624
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_translationAtRest_Injected(out Vector3 ret);

		// Token: 0x06000659 RID: 1625
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_translationAtRest_Injected(ref Vector3 value);

		// Token: 0x0600065A RID: 1626
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rotationAtRest_Injected(out Vector3 ret);

		// Token: 0x0600065B RID: 1627
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_rotationAtRest_Injected(ref Vector3 value);

		// Token: 0x0600065C RID: 1628
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetTranslationOffsetInternal_Injected(int index, out Vector3 ret);

		// Token: 0x0600065D RID: 1629
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTranslationOffsetInternal_Injected(int index, ref Vector3 value);

		// Token: 0x0600065E RID: 1630
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetRotationOffsetInternal_Injected(int index, out Vector3 ret);

		// Token: 0x0600065F RID: 1631
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRotationOffsetInternal_Injected(int index, ref Vector3 value);

		// Token: 0x06000660 RID: 1632
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int AddSource_Injected(ref ConstraintSource source);

		// Token: 0x06000661 RID: 1633
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetSourceInternal_Injected(int index, out ConstraintSource ret);

		// Token: 0x06000662 RID: 1634
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSourceInternal_Injected(int index, ref ConstraintSource source);
	}
}
