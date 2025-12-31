using System;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class AnimationStateRandomBlend : StateMachineBehaviour
{
	// Token: 0x0600025C RID: 604 RVA: 0x0001347C File Offset: 0x0001167C
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int integer = animator.GetInteger("RandomSelector");
		if (this.ChoiceCount > 0)
		{
			animator.SetFloat("RandomVariationQuantized", (float)(integer % this.ChoiceCount));
			return;
		}
		animator.SetFloat("RandomVariationQuantized", 0f);
	}

	// Token: 0x04000300 RID: 768
	[Tooltip("The number of options to randomly select from")]
	public int ChoiceCount = 1;
}
