using System;
using UnityEngine;

// Token: 0x02000488 RID: 1160
public class CharacterControllerUnity : CharacterControllerAbstract
{
	// Token: 0x060025D3 RID: 9683 RVA: 0x000F3DA3 File Offset: 0x000F1FA3
	public CharacterControllerUnity(CharacterController _cc)
	{
		this.cc = _cc;
	}

	// Token: 0x060025D4 RID: 9684 RVA: 0x000F3DB2 File Offset: 0x000F1FB2
	public override void Enable(bool isEnabled)
	{
		this.cc.enabled = isEnabled;
	}

	// Token: 0x060025D5 RID: 9685 RVA: 0x000F3DC0 File Offset: 0x000F1FC0
	public override void SetStepOffset(float _stepOffset)
	{
		this.cc.stepOffset = _stepOffset;
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x000F3DCE File Offset: 0x000F1FCE
	public override float GetStepOffset()
	{
		return this.cc.stepOffset;
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x000F3DDB File Offset: 0x000F1FDB
	public override void SetSize(Vector3 _center, float _height, float _radius)
	{
		this.cc.center = _center;
		this.cc.height = _height;
		this.cc.radius = _radius;
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x000F3E01 File Offset: 0x000F2001
	public override void SetCenter(Vector3 _center)
	{
		this.cc.center = _center;
	}

	// Token: 0x060025D9 RID: 9689 RVA: 0x000F3E0F File Offset: 0x000F200F
	public override Vector3 GetCenter()
	{
		return this.cc.center;
	}

	// Token: 0x060025DA RID: 9690 RVA: 0x000F3E1C File Offset: 0x000F201C
	public override void SetRadius(float _radius)
	{
		this.cc.radius = _radius;
	}

	// Token: 0x060025DB RID: 9691 RVA: 0x000F3E2A File Offset: 0x000F202A
	public override float GetRadius()
	{
		return this.cc.radius;
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x000F3E37 File Offset: 0x000F2037
	public override void SetSkinWidth(float _width)
	{
		this.cc.skinWidth = _width;
	}

	// Token: 0x060025DD RID: 9693 RVA: 0x000F3E45 File Offset: 0x000F2045
	public override float GetSkinWidth()
	{
		return this.cc.skinWidth;
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x000F3E52 File Offset: 0x000F2052
	public override void SetHeight(float _height)
	{
		this.cc.height = _height;
	}

	// Token: 0x060025DF RID: 9695 RVA: 0x000F3E60 File Offset: 0x000F2060
	public override float GetHeight()
	{
		return this.cc.height;
	}

	// Token: 0x060025E0 RID: 9696 RVA: 0x000F3E6D File Offset: 0x000F206D
	public override bool IsGrounded()
	{
		return this.cc.isGrounded;
	}

	// Token: 0x060025E1 RID: 9697 RVA: 0x000F3E7A File Offset: 0x000F207A
	public override CollisionFlags Move(Vector3 _dir)
	{
		return this.cc.Move(_dir);
	}

	// Token: 0x060025E2 RID: 9698 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Rotate(Quaternion _dir)
	{
	}

	// Token: 0x04001CB4 RID: 7348
	[PublicizedFrom(EAccessModifier.Private)]
	public CharacterController cc;
}
