using System;
using UnityEngine;

// Token: 0x0200008D RID: 141
public class AnimatorRangedSpecialAttackState : StateMachineBehaviour
{
	// Token: 0x06000294 RID: 660 RVA: 0x00014F8C File Offset: 0x0001318C
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent == null)
		{
			return;
		}
		componentInParent.emodel.avatarController.UpdateInt("CurrentAnim", 1, true);
	}

	// Token: 0x06000295 RID: 661 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x06000296 RID: 662 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
