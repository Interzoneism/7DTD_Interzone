using System;
using UnityEngine;

// Token: 0x02000091 RID: 145
[SharedBetweenAnimators]
public class AnimatorStateRaycastOnExit : StateMachineBehaviour
{
	// Token: 0x0600029F RID: 671 RVA: 0x00015112 File Offset: 0x00013312
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.ResetTrigger("WeaponFire");
		animator.ResetTrigger("PowerAttack");
	}
}
