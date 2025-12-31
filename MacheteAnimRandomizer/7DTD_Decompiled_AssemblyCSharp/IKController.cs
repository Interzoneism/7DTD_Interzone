using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// Token: 0x020000C8 RID: 200
public class IKController : MonoBehaviour
{
	// Token: 0x060004E1 RID: 1249 RVA: 0x00023384 File Offset: 0x00021584
	public unsafe void Start()
	{
		this.animator = base.GetComponent<Animator>();
		Transform transform = base.transform.Find("IKRig");
		if (transform)
		{
			this.rig = transform.GetComponent<Rig>();
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				TwoBoneIKConstraint component = transform.GetChild(i).GetComponent<TwoBoneIKConstraint>();
				if (component)
				{
					TwoBoneIKConstraintData twoBoneIKConstraintData = *component.data;
					Transform target = twoBoneIKConstraintData.target;
					if (target)
					{
						int num = this.NameToIndex(target.name);
						if (num >= 0)
						{
							IKController.Constraint constraint;
							constraint.tbConstraint = component;
							constraint.originalWeight = component.weight;
							constraint.originalTargetT = target;
							this.rigConstraints[num] = constraint;
						}
					}
				}
			}
			this.ModifyRig();
		}
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x00023454 File Offset: 0x00021654
	[PublicizedFrom(EAccessModifier.Private)]
	public int NameToIndex(string name)
	{
		for (int i = 0; i < IKController.IKNames.Length; i++)
		{
			if (name == IKController.IKNames[i])
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x00023485 File Offset: 0x00021685
	public void SetTargets(List<IKController.Target> _targets)
	{
		this.targets = _targets;
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x0002348E File Offset: 0x0002168E
	public void Cleanup()
	{
		this.targets = null;
		if (this.rig)
		{
			this.ModifyRig();
		}
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x000234AC File Offset: 0x000216AC
	public void ModifyRig()
	{
		if (this.targets == null)
		{
			for (int i = 0; i < 4; i++)
			{
				IKController.Constraint constraint = this.rigConstraints[i];
				if (constraint.originalTargetT)
				{
					TwoBoneIKConstraint tbConstraint = constraint.tbConstraint;
					tbConstraint.weight = constraint.originalWeight;
					tbConstraint.data.target = constraint.originalTargetT;
					Transform transform = tbConstraint.transform;
					transform.position = Vector3.zero;
					transform.rotation = Quaternion.identity;
				}
			}
		}
		else
		{
			Transform transform2 = base.transform;
			for (int j = 0; j < this.targets.Count; j++)
			{
				IKController.Target target = this.targets[j];
				int avatarGoal = (int)target.avatarGoal;
				TwoBoneIKConstraint tbConstraint2 = this.rigConstraints[avatarGoal].tbConstraint;
				if (tbConstraint2)
				{
					Transform transform3 = target.transform;
					if (!transform3)
					{
						transform3 = tbConstraint2.transform;
						Matrix4x4 localToWorldMatrix = transform2.localToWorldMatrix;
						Vector3 position = localToWorldMatrix.MultiplyPoint(target.position);
						transform3.position = position;
						Quaternion rotation = localToWorldMatrix.rotation * Quaternion.Euler(target.rotation);
						transform3.rotation = rotation;
					}
					tbConstraint2.weight = 1f;
					tbConstraint2.data.target = transform3;
				}
			}
		}
		base.GetComponent<RigBuilder>().Build();
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x00023608 File Offset: 0x00021808
	public void OnAnimatorIK()
	{
		if (!this.animator)
		{
			return;
		}
		if (this.targets == null)
		{
			for (int i = 0; i < 4; i++)
			{
				this.animator.SetIKPositionWeight((AvatarIKGoal)i, 0f);
				this.animator.SetIKRotationWeight((AvatarIKGoal)i, 0f);
			}
			return;
		}
		Transform transform = base.transform;
		for (int j = 0; j < this.targets.Count; j++)
		{
			IKController.Target target = this.targets[j];
			this.animator.SetIKPositionWeight(target.avatarGoal, 1f);
			this.animator.SetIKRotationWeight(target.avatarGoal, 1f);
			Transform transform2 = target.transform;
			if (!transform2)
			{
				Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
				Vector3 goalPosition = localToWorldMatrix.MultiplyPoint(target.position);
				this.animator.SetIKPosition(target.avatarGoal, goalPosition);
				Quaternion goalRotation = localToWorldMatrix.rotation * Quaternion.Euler(target.rotation);
				this.animator.SetIKRotation(target.avatarGoal, goalRotation);
			}
			else
			{
				this.animator.SetIKPosition(target.avatarGoal, transform2.position);
				this.animator.SetIKRotation(target.avatarGoal, transform2.rotation);
			}
		}
	}

	// Token: 0x0400059C RID: 1436
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cTargetTypeCount = 4;

	// Token: 0x0400059D RID: 1437
	public static string[] IKNames = new string[]
	{
		"IKFootL",
		"IKFootR",
		"IKHandL",
		"IKHandR"
	};

	// Token: 0x0400059E RID: 1438
	public List<IKController.Target> targets;

	// Token: 0x0400059F RID: 1439
	public IKController.Constraint[] rigConstraints = new IKController.Constraint[4];

	// Token: 0x040005A0 RID: 1440
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator animator;

	// Token: 0x040005A1 RID: 1441
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Rig rig;

	// Token: 0x020000C9 RID: 201
	[Serializable]
	public struct Target
	{
		// Token: 0x040005A2 RID: 1442
		public AvatarIKGoal avatarGoal;

		// Token: 0x040005A3 RID: 1443
		public Transform transform;

		// Token: 0x040005A4 RID: 1444
		public Vector3 position;

		// Token: 0x040005A5 RID: 1445
		public Vector3 rotation;
	}

	// Token: 0x020000CA RID: 202
	[Serializable]
	public struct Constraint
	{
		// Token: 0x040005A6 RID: 1446
		public TwoBoneIKConstraint tbConstraint;

		// Token: 0x040005A7 RID: 1447
		public float originalWeight;

		// Token: 0x040005A8 RID: 1448
		public Transform originalTargetT;
	}
}
