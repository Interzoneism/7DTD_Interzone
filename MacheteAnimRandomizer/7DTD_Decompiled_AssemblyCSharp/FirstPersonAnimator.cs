using System;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class FirstPersonAnimator : BodyAnimator
{
	// Token: 0x0600049B RID: 1179 RVA: 0x00020D88 File Offset: 0x0001EF88
	public FirstPersonAnimator(EntityAlive _entity, AvatarCharacterController.AnimationStates _animStates, Transform _bodyTransform, BodyAnimator.EnumState _defaultState)
	{
		BodyAnimator.BodyParts bodyParts = new BodyAnimator.BodyParts(_bodyTransform, _bodyTransform.FindInChilds((_entity.emodel is EModelSDCS) ? "RightWeapon" : "Gunjoint", false));
		base.initBodyAnimator(_entity, bodyParts, _defaultState);
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x00020DCC File Offset: 0x0001EFCC
	public override void SetDrunk(float _numBeers)
	{
		Animator animator = base.Animator;
		if (animator)
		{
			animator.SetFloat("drunk", _numBeers);
		}
	}
}
