using System;
using UnityEngine;

// Token: 0x02000096 RID: 150
public class AnimatorThrownWeaponHolsterState : StateMachineBehaviour
{
	// Token: 0x060002AB RID: 683 RVA: 0x00015160 File Offset: 0x00013360
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityPlayerLocal componentInParent = animator.GetComponentInParent<EntityPlayerLocal>();
		if (componentInParent != null)
		{
			componentInParent.HolsterWeapon(true);
		}
	}

	// Token: 0x060002AC RID: 684 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x060002AD RID: 685 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
