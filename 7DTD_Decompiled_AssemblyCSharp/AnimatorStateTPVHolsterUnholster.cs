using System;
using UnityEngine;

// Token: 0x02000093 RID: 147
[PublicizedFrom(EAccessModifier.Internal)]
public class AnimatorStateTPVHolsterUnholster : StateMachineBehaviour
{
	// Token: 0x060002A2 RID: 674 RVA: 0x0001512A File Offset: 0x0001332A
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponentInParent<EntityAlive>() != null;
	}
}
