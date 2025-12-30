using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200135F RID: 4959
[Preserve]
public class vp_PlayerEventHandler : vp_StateEventHandler
{
	// Token: 0x06009B21 RID: 39713 RVA: 0x003DBF34 File Offset: 0x003DA134
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		base.BindStateToActivity(this.Run);
		base.BindStateToActivity(this.Walk);
		base.BindStateToActivity(this.Jump);
		base.BindStateToActivity(this.Crouch);
		base.BindStateToActivity(this.CrouchWalk);
		base.BindStateToActivity(this.CrouchRun);
		base.BindStateToActivity(this.Zoom);
		base.BindStateToActivity(this.Reload);
		base.BindStateToActivity(this.Dead);
		base.BindStateToActivity(this.Climb);
		base.BindStateToActivity(this.OutOfControl);
		base.BindStateToActivity(this.Driving);
		base.BindStateToActivityOnStart(this.Attack);
		this.SetWeapon.AutoDuration = 1f;
		this.Reload.AutoDuration = 1f;
		this.Zoom.MinDuration = 0.2f;
		this.Crouch.MinDuration = 0.1f;
		this.Jump.MinPause = 0f;
		this.SetWeapon.MinPause = 0.2f;
	}

	// Token: 0x040077CD RID: 30669
	public vp_Value<bool> IsFirstPerson;

	// Token: 0x040077CE RID: 30670
	public vp_Value<bool> IsLocal;

	// Token: 0x040077CF RID: 30671
	public vp_Value<bool> IsAI;

	// Token: 0x040077D0 RID: 30672
	public vp_Value<float> Health;

	// Token: 0x040077D1 RID: 30673
	public vp_Value<float> MaxHealth;

	// Token: 0x040077D2 RID: 30674
	public vp_Value<Vector3> Position;

	// Token: 0x040077D3 RID: 30675
	public vp_Value<Vector2> Rotation;

	// Token: 0x040077D4 RID: 30676
	public vp_Value<float> BodyYaw;

	// Token: 0x040077D5 RID: 30677
	public vp_Value<Vector3> LookPoint;

	// Token: 0x040077D6 RID: 30678
	public vp_Value<Vector3> HeadLookDirection;

	// Token: 0x040077D7 RID: 30679
	public vp_Value<Vector3> AimDirection;

	// Token: 0x040077D8 RID: 30680
	public vp_Value<Vector3> MotorThrottle;

	// Token: 0x040077D9 RID: 30681
	public vp_Value<bool> MotorJumpDone;

	// Token: 0x040077DA RID: 30682
	public vp_Value<Vector2> InputMoveVector;

	// Token: 0x040077DB RID: 30683
	public vp_Value<float> InputClimbVector;

	// Token: 0x040077DC RID: 30684
	public vp_Activity Dead;

	// Token: 0x040077DD RID: 30685
	public vp_Activity Run;

	// Token: 0x040077DE RID: 30686
	public vp_Activity Walk;

	// Token: 0x040077DF RID: 30687
	public vp_Activity Jump;

	// Token: 0x040077E0 RID: 30688
	public vp_Activity Crouch;

	// Token: 0x040077E1 RID: 30689
	public vp_Activity CrouchWalk;

	// Token: 0x040077E2 RID: 30690
	public vp_Activity CrouchRun;

	// Token: 0x040077E3 RID: 30691
	public vp_Activity Zoom;

	// Token: 0x040077E4 RID: 30692
	public vp_Activity Attack;

	// Token: 0x040077E5 RID: 30693
	public vp_Activity Reload;

	// Token: 0x040077E6 RID: 30694
	public vp_Activity Climb;

	// Token: 0x040077E7 RID: 30695
	public vp_Activity Interact;

	// Token: 0x040077E8 RID: 30696
	public vp_Activity<int> SetWeapon;

	// Token: 0x040077E9 RID: 30697
	public vp_Activity OutOfControl;

	// Token: 0x040077EA RID: 30698
	public vp_Activity Driving;

	// Token: 0x040077EB RID: 30699
	public vp_Message<int> Wield;

	// Token: 0x040077EC RID: 30700
	public vp_Message Unwield;

	// Token: 0x040077ED RID: 30701
	public vp_Attempt Fire;

	// Token: 0x040077EE RID: 30702
	public vp_Message DryFire;

	// Token: 0x040077EF RID: 30703
	public vp_Attempt SetPrevWeapon;

	// Token: 0x040077F0 RID: 30704
	public vp_Attempt SetNextWeapon;

	// Token: 0x040077F1 RID: 30705
	public vp_Attempt<string> SetWeaponByName;

	// Token: 0x040077F2 RID: 30706
	[Obsolete("Please use the 'CurrentWeaponIndex' vp_Value instead.")]
	public vp_Value<int> CurrentWeaponID;

	// Token: 0x040077F3 RID: 30707
	public vp_Value<int> CurrentWeaponIndex;

	// Token: 0x040077F4 RID: 30708
	public vp_Value<string> CurrentWeaponName;

	// Token: 0x040077F5 RID: 30709
	public vp_Value<bool> CurrentWeaponWielded;

	// Token: 0x040077F6 RID: 30710
	public vp_Attempt AutoReload;

	// Token: 0x040077F7 RID: 30711
	public vp_Value<float> CurrentWeaponReloadDuration;

	// Token: 0x040077F8 RID: 30712
	public vp_Message<string, int> GetItemCount;

	// Token: 0x040077F9 RID: 30713
	public vp_Attempt RefillCurrentWeapon;

	// Token: 0x040077FA RID: 30714
	public vp_Value<int> CurrentWeaponAmmoCount;

	// Token: 0x040077FB RID: 30715
	public vp_Value<int> CurrentWeaponMaxAmmoCount;

	// Token: 0x040077FC RID: 30716
	public vp_Value<int> CurrentWeaponClipCount;

	// Token: 0x040077FD RID: 30717
	public vp_Value<int> CurrentWeaponType;

	// Token: 0x040077FE RID: 30718
	public vp_Value<int> CurrentWeaponGrip;

	// Token: 0x040077FF RID: 30719
	public vp_Attempt<object> AddItem;

	// Token: 0x04007800 RID: 30720
	public vp_Attempt<object> RemoveItem;

	// Token: 0x04007801 RID: 30721
	public vp_Attempt DepleteAmmo;

	// Token: 0x04007802 RID: 30722
	public vp_Message<Vector3> Move;

	// Token: 0x04007803 RID: 30723
	public vp_Value<Vector3> Velocity;

	// Token: 0x04007804 RID: 30724
	public vp_Value<float> SlopeLimit;

	// Token: 0x04007805 RID: 30725
	public vp_Value<float> StepOffset;

	// Token: 0x04007806 RID: 30726
	public vp_Value<float> Radius;

	// Token: 0x04007807 RID: 30727
	public vp_Value<float> Height;

	// Token: 0x04007808 RID: 30728
	public vp_Value<float> FallSpeed;

	// Token: 0x04007809 RID: 30729
	public vp_Message<float> FallImpact;

	// Token: 0x0400780A RID: 30730
	public vp_Message<float> FallImpact2;

	// Token: 0x0400780B RID: 30731
	public vp_Message<float> HeadImpact;

	// Token: 0x0400780C RID: 30732
	public vp_Message<Vector3> ForceImpact;

	// Token: 0x0400780D RID: 30733
	public vp_Message Stop;

	// Token: 0x0400780E RID: 30734
	public vp_Value<Transform> Platform;

	// Token: 0x0400780F RID: 30735
	public vp_Value<Texture> GroundTexture;

	// Token: 0x04007810 RID: 30736
	public vp_Value<vp_SurfaceIdentifier> SurfaceType;
}
