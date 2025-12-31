using System;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class AnimationRandomTransition : StateMachineBehaviour
{
	// Token: 0x06000257 RID: 599 RVA: 0x000133C0 File Offset: 0x000115C0
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.numberOfAnimations > 0)
		{
			int value = UnityEngine.Random.Range(0, this.numberOfAnimations);
			animator.SetInteger(this.animationParameter, value);
		}
	}

	// Token: 0x040002FC RID: 764
	public string animationParameter = "RandomIndex";

	// Token: 0x040002FD RID: 765
	public int numberOfAnimations;
}
