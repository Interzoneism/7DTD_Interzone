using System;
using UnityEngine;

// Token: 0x02000485 RID: 1157
public abstract class CharacterControllerAbstract
{
	// Token: 0x060025A1 RID: 9633
	public abstract void Enable(bool isEnabled);

	// Token: 0x060025A2 RID: 9634
	public abstract void SetStepOffset(float _stepOffset);

	// Token: 0x060025A3 RID: 9635
	public abstract float GetStepOffset();

	// Token: 0x060025A4 RID: 9636
	public abstract void SetSize(Vector3 _center, float _height, float _radius);

	// Token: 0x060025A5 RID: 9637
	public abstract void SetCenter(Vector3 _center);

	// Token: 0x060025A6 RID: 9638
	public abstract Vector3 GetCenter();

	// Token: 0x060025A7 RID: 9639
	public abstract void SetRadius(float _radius);

	// Token: 0x060025A8 RID: 9640
	public abstract float GetRadius();

	// Token: 0x060025A9 RID: 9641
	public abstract void SetSkinWidth(float _width);

	// Token: 0x060025AA RID: 9642
	public abstract float GetSkinWidth();

	// Token: 0x060025AB RID: 9643
	public abstract void SetHeight(float _height);

	// Token: 0x060025AC RID: 9644
	public abstract float GetHeight();

	// Token: 0x060025AD RID: 9645
	public abstract bool IsGrounded();

	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x060025AE RID: 9646 RVA: 0x000F387A File Offset: 0x000F1A7A
	public virtual Vector3 GroundNormal
	{
		get
		{
			return Vector3.up;
		}
	}

	// Token: 0x060025AF RID: 9647
	public abstract CollisionFlags Move(Vector3 _dir);

	// Token: 0x060025B0 RID: 9648 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual CollisionFlags Update()
	{
		return CollisionFlags.None;
	}

	// Token: 0x060025B1 RID: 9649
	public abstract void Rotate(Quaternion _dir);

	// Token: 0x060025B2 RID: 9650 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public CharacterControllerAbstract()
	{
	}
}
