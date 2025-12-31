using System;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class ArchetypePreviewIKController : MonoBehaviour
{
	// Token: 0x0600030B RID: 779 RVA: 0x000178B8 File Offset: 0x00015AB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.animator = base.GetComponent<Animator>();
		Transform transform = base.transform.FindInChilds("LeftFoot", false);
		Transform transform2 = base.transform.FindInChilds("RightFoot", false);
		this.leftFootPos = transform.position;
		this.leftFootRot = transform.rotation;
		this.rightFootPos = transform2.position;
		this.rightFootRot = transform2.rotation;
	}

	// Token: 0x0600030C RID: 780 RVA: 0x00017928 File Offset: 0x00015B28
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnAnimatorIK()
	{
		if (this.animator)
		{
			this.animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
			this.animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
			this.animator.SetIKPosition(AvatarIKGoal.LeftFoot, this.leftFootPos);
			this.animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.Euler(this.leftFootRot.eulerAngles.x + this.FootRotationModifier.x, this.leftFootRot.eulerAngles.y - this.FootRotationModifier.y, this.leftFootRot.eulerAngles.z + this.FootRotationModifier.z));
			this.animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
			this.animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
			this.animator.SetIKPosition(AvatarIKGoal.RightFoot, this.rightFootPos);
			this.animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.Euler(this.rightFootRot.eulerAngles.x + this.FootRotationModifier.x, this.rightFootRot.eulerAngles.y + this.FootRotationModifier.y, this.rightFootRot.eulerAngles.z + this.FootRotationModifier.z));
		}
	}

	// Token: 0x040003AE RID: 942
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Animator animator;

	// Token: 0x040003AF RID: 943
	public bool ikActive;

	// Token: 0x040003B0 RID: 944
	public Transform rightHandObj;

	// Token: 0x040003B1 RID: 945
	public Transform lookObj;

	// Token: 0x040003B2 RID: 946
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 leftFootPos;

	// Token: 0x040003B3 RID: 947
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 rightFootPos;

	// Token: 0x040003B4 RID: 948
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion leftFootRot;

	// Token: 0x040003B5 RID: 949
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion rightFootRot;

	// Token: 0x040003B6 RID: 950
	public Vector3 FootRotationModifier = new Vector3(-62f, -198f, -93.5f);
}
