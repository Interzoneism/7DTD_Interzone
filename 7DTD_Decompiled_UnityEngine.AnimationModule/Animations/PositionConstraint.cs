using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x02000067 RID: 103
	[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
	[NativeHeader("Modules/Animation/Constraints/PositionConstraint.h")]
	[RequireComponent(typeof(Transform))]
	[UsedByNativeCode]
	public sealed class PositionConstraint : Behaviour, IConstraint, IConstraintInternal
	{
		// Token: 0x06000594 RID: 1428 RVA: 0x00007D36 File Offset: 0x00005F36
		private PositionConstraint()
		{
			PositionConstraint.Internal_Create(this);
		}

		// Token: 0x06000595 RID: 1429
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] PositionConstraint self);

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000596 RID: 1430
		// (set) Token: 0x06000597 RID: 1431
		public extern float weight { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000598 RID: 1432 RVA: 0x00007D48 File Offset: 0x00005F48
		// (set) Token: 0x06000599 RID: 1433 RVA: 0x00007D5E File Offset: 0x00005F5E
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

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x0600059A RID: 1434 RVA: 0x00007D68 File Offset: 0x00005F68
		// (set) Token: 0x0600059B RID: 1435 RVA: 0x00007D7E File Offset: 0x00005F7E
		public Vector3 translationOffset
		{
			get
			{
				Vector3 result;
				this.get_translationOffset_Injected(out result);
				return result;
			}
			set
			{
				this.set_translationOffset_Injected(ref value);
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x0600059C RID: 1436
		// (set) Token: 0x0600059D RID: 1437
		public extern Axis translationAxis { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x0600059E RID: 1438
		// (set) Token: 0x0600059F RID: 1439
		public extern bool constraintActive { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060005A0 RID: 1440
		// (set) Token: 0x060005A1 RID: 1441
		public extern bool locked { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x060005A2 RID: 1442 RVA: 0x00007D88 File Offset: 0x00005F88
		public int sourceCount
		{
			get
			{
				return PositionConstraint.GetSourceCountInternal(this);
			}
		}

		// Token: 0x060005A3 RID: 1443
		[FreeFunction("ConstraintBindings::GetSourceCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSourceCountInternal([NotNull("ArgumentNullException")] PositionConstraint self);

		// Token: 0x060005A4 RID: 1444
		[FreeFunction(Name = "ConstraintBindings::GetSources", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetSources([NotNull("ArgumentNullException")] List<ConstraintSource> sources);

		// Token: 0x060005A5 RID: 1445 RVA: 0x00007DA0 File Offset: 0x00005FA0
		public void SetSources(List<ConstraintSource> sources)
		{
			bool flag = sources == null;
			if (flag)
			{
				throw new ArgumentNullException("sources");
			}
			PositionConstraint.SetSourcesInternal(this, sources);
		}

		// Token: 0x060005A6 RID: 1446
		[FreeFunction("ConstraintBindings::SetSources", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSourcesInternal([NotNull("ArgumentNullException")] PositionConstraint self, List<ConstraintSource> sources);

		// Token: 0x060005A7 RID: 1447 RVA: 0x00007DC9 File Offset: 0x00005FC9
		public int AddSource(ConstraintSource source)
		{
			return this.AddSource_Injected(ref source);
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x00007DD3 File Offset: 0x00005FD3
		public void RemoveSource(int index)
		{
			this.ValidateSourceIndex(index);
			this.RemoveSourceInternal(index);
		}

		// Token: 0x060005A9 RID: 1449
		[NativeName("RemoveSource")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveSourceInternal(int index);

		// Token: 0x060005AA RID: 1450 RVA: 0x00007DE8 File Offset: 0x00005FE8
		public ConstraintSource GetSource(int index)
		{
			this.ValidateSourceIndex(index);
			return this.GetSourceInternal(index);
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x00007E0C File Offset: 0x0000600C
		[NativeName("GetSource")]
		private ConstraintSource GetSourceInternal(int index)
		{
			ConstraintSource result;
			this.GetSourceInternal_Injected(index, out result);
			return result;
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x00007E23 File Offset: 0x00006023
		public void SetSource(int index, ConstraintSource source)
		{
			this.ValidateSourceIndex(index);
			this.SetSourceInternal(index, source);
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x00007E37 File Offset: 0x00006037
		[NativeName("SetSource")]
		private void SetSourceInternal(int index, ConstraintSource source)
		{
			this.SetSourceInternal_Injected(index, ref source);
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x00007E44 File Offset: 0x00006044
		private void ValidateSourceIndex(int index)
		{
			bool flag = this.sourceCount == 0;
			if (flag)
			{
				throw new InvalidOperationException("The PositionConstraint component has no sources.");
			}
			bool flag2 = index < 0 || index >= this.sourceCount;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("index", string.Format("Constraint source index {0} is out of bounds (0-{1}).", index, this.sourceCount));
			}
		}

		// Token: 0x060005AF RID: 1455
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_translationAtRest_Injected(out Vector3 ret);

		// Token: 0x060005B0 RID: 1456
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_translationAtRest_Injected(ref Vector3 value);

		// Token: 0x060005B1 RID: 1457
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_translationOffset_Injected(out Vector3 ret);

		// Token: 0x060005B2 RID: 1458
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_translationOffset_Injected(ref Vector3 value);

		// Token: 0x060005B3 RID: 1459
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int AddSource_Injected(ref ConstraintSource source);

		// Token: 0x060005B4 RID: 1460
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetSourceInternal_Injected(int index, out ConstraintSource ret);

		// Token: 0x060005B5 RID: 1461
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSourceInternal_Injected(int index, ref ConstraintSource source);
	}
}
