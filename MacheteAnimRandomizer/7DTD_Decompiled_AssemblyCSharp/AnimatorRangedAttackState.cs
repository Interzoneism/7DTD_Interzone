using System;
using UnityEngine;

// Token: 0x0200008A RID: 138
public class AnimatorRangedAttackState : StateMachineBehaviour
{
	// Token: 0x06000286 RID: 646 RVA: 0x00014580 File Offset: 0x00012780
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent == null)
		{
			return;
		}
		componentInParent.emodel.avatarController.UpdateInt("CurrentAnim", 2, true);
	}

	// Token: 0x06000287 RID: 647 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x06000288 RID: 648 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
