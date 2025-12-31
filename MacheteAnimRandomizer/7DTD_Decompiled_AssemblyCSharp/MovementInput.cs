using System;
using UnityEngine;

// Token: 0x02001016 RID: 4118
public class MovementInput
{
	// Token: 0x0600829F RID: 33439 RVA: 0x0034C8D6 File Offset: 0x0034AAD6
	public bool IsMoving()
	{
		return Mathf.Abs(this.moveStrafe) > 0.05f || Mathf.Abs(this.moveForward) > 0.05f;
	}

	// Token: 0x060082A0 RID: 33440 RVA: 0x0034C900 File Offset: 0x0034AB00
	public void Clear()
	{
		this.moveStrafe = 0f;
		this.moveForward = 0f;
		this.rotation = Vector3.zero;
		this.cameraRotation = Vector3.zero;
		this.jump = false;
		this.sneak = false;
		this.useItemOnBackAction = false;
		this.down = false;
		this.downToggle = false;
	}

	// Token: 0x060082A1 RID: 33441 RVA: 0x0034C95C File Offset: 0x0034AB5C
	public void Copy(MovementInput _other)
	{
		_other.moveStrafe = this.moveStrafe;
		_other.moveForward = this.moveForward;
		_other.rotation = this.rotation;
		_other.cameraRotation = this.cameraRotation;
		_other.cameraDistance = this.cameraDistance;
		_other.running = this.running;
		_other.jump = this.jump;
		_other.sneak = this.sneak;
		_other.useItemOnBackAction = this.useItemOnBackAction;
		_other.down = this.down;
		_other.downToggle = this.downToggle;
		_other.bDetachedCameraMove = this.bDetachedCameraMove;
		_other.bCameraPositionLocked = this.bCameraPositionLocked;
	}

	// Token: 0x060082A2 RID: 33442 RVA: 0x0034CA08 File Offset: 0x0034AC08
	public bool Equals(MovementInput _other)
	{
		return this.moveStrafe == _other.moveStrafe && this.moveForward == _other.moveForward && this.rotation.Equals(_other.rotation) && this.cameraRotation.Equals(_other.cameraRotation) && this.jump == _other.jump && this.sneak == _other.sneak && this.down == _other.down && this.downToggle == _other.downToggle && this.useItemOnBackAction == _other.useItemOnBackAction && this.running == _other.running;
	}

	// Token: 0x040064CF RID: 25807
	public float moveStrafe;

	// Token: 0x040064D0 RID: 25808
	public float moveForward;

	// Token: 0x040064D1 RID: 25809
	public Vector3 rotation;

	// Token: 0x040064D2 RID: 25810
	public Vector3 cameraRotation;

	// Token: 0x040064D3 RID: 25811
	public float cameraDistance;

	// Token: 0x040064D4 RID: 25812
	public bool running;

	// Token: 0x040064D5 RID: 25813
	public bool jump;

	// Token: 0x040064D6 RID: 25814
	public bool sneak;

	// Token: 0x040064D7 RID: 25815
	public bool useItemOnBackAction;

	// Token: 0x040064D8 RID: 25816
	public bool down;

	// Token: 0x040064D9 RID: 25817
	public bool downToggle;

	// Token: 0x040064DA RID: 25818
	public bool bDetachedCameraMove;

	// Token: 0x040064DB RID: 25819
	public bool bCameraChange;

	// Token: 0x040064DC RID: 25820
	public bool bCameraPositionLocked;

	// Token: 0x040064DD RID: 25821
	public bool lastInputController;
}
