using System;
using UnityEngine;

// Token: 0x02000081 RID: 129
public class AnimationStateRagdoll : StateMachineBehaviour
{
	// Token: 0x06000259 RID: 601 RVA: 0x00013404 File Offset: 0x00011604
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent != null)
		{
			componentInParent.BeginDynamicRagdoll(this.RagdollFlags, this.StunTime);
		}
	}

	// Token: 0x0600025A RID: 602 RVA: 0x00013434 File Offset: 0x00011634
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent != null)
		{
			componentInParent.ActivateDynamicRagdoll();
		}
	}

	// Token: 0x040002FE RID: 766
	public DynamicRagdollFlags RagdollFlags = DynamicRagdollFlags.Active | DynamicRagdollFlags.RagdollOnFall | DynamicRagdollFlags.UseBoneVelocities;

	// Token: 0x040002FF RID: 767
	[Tooltip("Time period to stun")]
	public FloatRange StunTime = new FloatRange(1f, 1f);
}
