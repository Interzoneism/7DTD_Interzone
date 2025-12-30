using System;
using KinematicCharacterController;
using UnityEngine;

// Token: 0x02000486 RID: 1158
public class CharacterControllerKinematic : CharacterControllerAbstract
{
	// Token: 0x060025B3 RID: 9651 RVA: 0x000F3884 File Offset: 0x000F1A84
	public CharacterControllerKinematic(Entity _entity)
	{
		GameObject gameObject = _entity.PhysicsTransform.gameObject;
		KinematicCharacterSystem.EnsureCreation();
		this.cs = KinematicCharacterSystem.GetInstance();
		KinematicCharacterSystem.AutoSimulation = false;
		KinematicCharacterSystem.Interpolate = false;
		this.motor = gameObject.AddComponent<KinematicCharacterMotor>();
		this.motor.StepHandling = StepHandlingMethod.Extra;
		this.motor.AllowSteppingWithoutStableGrounding = true;
		this.motor.InteractiveRigidbodyHandling = false;
		this.motor.LedgeAndDenivelationHandling = false;
		this.motor.MaxStableSlopeAngle = 63.8f;
		this.cc = new CC();
		this.cc.entity = _entity;
		this.cc.motor = this.motor;
		this.motor.CharacterController = this.cc;
		this.motor.ForceUnground(0.1f);
	}

	// Token: 0x060025B4 RID: 9652 RVA: 0x000F3954 File Offset: 0x000F1B54
	public override void Enable(bool isEnabled)
	{
		this.motor.enabled = isEnabled;
	}

	// Token: 0x060025B5 RID: 9653 RVA: 0x000F3962 File Offset: 0x000F1B62
	public override void SetStepOffset(float _stepOffset)
	{
		this.motor.MaxStepHeight = _stepOffset + 0.01f;
	}

	// Token: 0x060025B6 RID: 9654 RVA: 0x000F3976 File Offset: 0x000F1B76
	public override float GetStepOffset()
	{
		return this.motor.MaxStepHeight;
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x000F3983 File Offset: 0x000F1B83
	public override void SetSize(Vector3 _center, float _height, float _radius)
	{
		this.motor.SetCapsuleDimensions(_radius, _height, _center.y);
	}

	// Token: 0x060025B8 RID: 9656 RVA: 0x000F3998 File Offset: 0x000F1B98
	public override void SetCenter(Vector3 _center)
	{
		this.motor.SetCapsuleDimensions(this.GetRadius(), this.GetHeight(), _center.y);
	}

	// Token: 0x060025B9 RID: 9657 RVA: 0x000F39B7 File Offset: 0x000F1BB7
	public override Vector3 GetCenter()
	{
		return this.motor.CharacterTransformToCapsuleCenter;
	}

	// Token: 0x060025BA RID: 9658 RVA: 0x000F39C4 File Offset: 0x000F1BC4
	public override void SetRadius(float _radius)
	{
		this.motor.SetCapsuleDimensions(_radius, this.GetHeight(), this.GetCenter().y);
	}

	// Token: 0x060025BB RID: 9659 RVA: 0x000F39E3 File Offset: 0x000F1BE3
	public override float GetRadius()
	{
		return this.motor.Capsule.radius;
	}

	// Token: 0x060025BC RID: 9660 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetSkinWidth(float _width)
	{
	}

	// Token: 0x060025BD RID: 9661 RVA: 0x000F39F5 File Offset: 0x000F1BF5
	public override float GetSkinWidth()
	{
		return 0.08f;
	}

	// Token: 0x060025BE RID: 9662 RVA: 0x000F39FC File Offset: 0x000F1BFC
	public override void SetHeight(float _height)
	{
		float radius = this.GetRadius();
		_height = Utils.FastMax(_height, radius * 2f);
		this.motor.SetCapsuleDimensions(radius, _height, _height * 0.5f);
	}

	// Token: 0x060025BF RID: 9663 RVA: 0x000F3A33 File Offset: 0x000F1C33
	public override float GetHeight()
	{
		return this.motor.Capsule.height;
	}

	// Token: 0x060025C0 RID: 9664 RVA: 0x000F3A45 File Offset: 0x000F1C45
	public override bool IsGrounded()
	{
		return (this.cc.collisionFlags & CollisionFlags.Below) > CollisionFlags.None;
	}

	// Token: 0x170003FE RID: 1022
	// (get) Token: 0x060025C1 RID: 9665 RVA: 0x000F3A57 File Offset: 0x000F1C57
	public override Vector3 GroundNormal
	{
		get
		{
			return this.motor.GroundingStatus.GroundNormal;
		}
	}

	// Token: 0x060025C2 RID: 9666 RVA: 0x000F3A69 File Offset: 0x000F1C69
	public override CollisionFlags Move(Vector3 _dir)
	{
		if (_dir.y >= 0.011f)
		{
			this.motor.ForceUnground(0.11f);
		}
		this.cc.vel = _dir / 0.05f;
		return this.Update();
	}

	// Token: 0x060025C3 RID: 9667 RVA: 0x000F3AA4 File Offset: 0x000F1CA4
	public override CollisionFlags Update()
	{
		this.cc.Move();
		if (this.motor.GroundingStatus.FoundAnyGround)
		{
			this.cc.collisionFlags |= CollisionFlags.Below;
		}
		return this.cc.collisionFlags;
	}

	// Token: 0x060025C4 RID: 9668 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Rotate(Quaternion _dir)
	{
	}

	// Token: 0x04001CAB RID: 7339
	[PublicizedFrom(EAccessModifier.Private)]
	public KinematicCharacterSystem cs;

	// Token: 0x04001CAC RID: 7340
	[PublicizedFrom(EAccessModifier.Private)]
	public KinematicCharacterMotor motor;

	// Token: 0x04001CAD RID: 7341
	[PublicizedFrom(EAccessModifier.Private)]
	public CC cc;
}
