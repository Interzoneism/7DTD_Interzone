using System;
using UnityEngine;

// Token: 0x0200008F RID: 143
public class AnimatorStateHolstering : StateMachineBehaviour
{
	// Token: 0x0600029A RID: 666 RVA: 0x00014FC4 File Offset: 0x000131C4
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent != null && componentInParent.emodel != null && componentInParent.emodel.avatarController != null)
		{
			componentInParent.emodel.avatarController.CancelEvent("WeaponFire");
			componentInParent.emodel.avatarController.CancelEvent("PowerAttack");
			componentInParent.emodel.avatarController.CancelEvent("UseItem");
			componentInParent.emodel.avatarController.UpdateBool("ItemUse", false, true);
			componentInParent.emodel.avatarController.CancelEvent("Reload");
		}
	}

	// Token: 0x0600029B RID: 667 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
