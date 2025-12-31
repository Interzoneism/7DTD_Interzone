using System;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class AnimatorSpeedSetter : StateMachineBehaviour
{
	// Token: 0x06000019 RID: 25 RVA: 0x000028E8 File Offset: 0x00000AE8
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.applyOnStateEnter)
		{
			animator.speed = this.animatorSpeed;
		}
	}

	// Token: 0x0600001A RID: 26 RVA: 0x000028FE File Offset: 0x00000AFE
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.applyOnStateExit)
		{
			animator.speed = this.animatorSpeed;
		}
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x0400001F RID: 31
	public bool applyOnStateEnter = true;

	// Token: 0x04000020 RID: 32
	public bool applyOnStateExit;

	// Token: 0x04000021 RID: 33
	public float animatorSpeed = 1f;
}
