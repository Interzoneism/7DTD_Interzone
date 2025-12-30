using System;
using UnityEngine;

// Token: 0x02000095 RID: 149
public class AnimatorThrownWeaponHoldState : StateMachineBehaviour
{
	// Token: 0x060002A7 RID: 679 RVA: 0x0001513C File Offset: 0x0001333C
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityPlayerLocal componentInParent = animator.GetComponentInParent<EntityPlayerLocal>();
		if (componentInParent != null)
		{
			componentInParent.HolsterWeapon(false);
		}
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
