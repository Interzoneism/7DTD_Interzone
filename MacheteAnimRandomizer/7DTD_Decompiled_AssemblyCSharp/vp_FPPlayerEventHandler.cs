using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02001355 RID: 4949
[Preserve]
public class vp_FPPlayerEventHandler : vp_PlayerEventHandler
{
	// Token: 0x040076B6 RID: 30390
	public vp_Message<vp_DamageInfo> HUDDamageFlash;

	// Token: 0x040076B7 RID: 30391
	public vp_Message<string> HUDText;

	// Token: 0x040076B8 RID: 30392
	public vp_Value<Texture> Crosshair;

	// Token: 0x040076B9 RID: 30393
	public vp_Value<Texture2D> CurrentAmmoIcon;

	// Token: 0x040076BA RID: 30394
	public vp_Value<Vector2> InputSmoothLook;

	// Token: 0x040076BB RID: 30395
	public vp_Value<Vector2> InputRawLook;

	// Token: 0x040076BC RID: 30396
	public vp_Message<string, bool> InputGetButton;

	// Token: 0x040076BD RID: 30397
	public vp_Message<string, bool> InputGetButtonUp;

	// Token: 0x040076BE RID: 30398
	public vp_Message<string, bool> InputGetButtonDown;

	// Token: 0x040076BF RID: 30399
	public vp_Value<bool> InputAllowGameplay;

	// Token: 0x040076C0 RID: 30400
	public vp_Value<bool> Pause;

	// Token: 0x040076C1 RID: 30401
	public vp_Value<Vector3> CameraLookDirection;

	// Token: 0x040076C2 RID: 30402
	public vp_Message CameraToggle3rdPerson;

	// Token: 0x040076C3 RID: 30403
	public vp_Message<float> CameraGroundStomp;

	// Token: 0x040076C4 RID: 30404
	public vp_Message<float> CameraBombShake;

	// Token: 0x040076C5 RID: 30405
	public vp_Value<Vector3> CameraEarthQuakeForce;

	// Token: 0x040076C6 RID: 30406
	public vp_Activity<Vector3> CameraEarthQuake;

	// Token: 0x040076C7 RID: 30407
	public vp_Value<vp_Interactable> Interactable;

	// Token: 0x040076C8 RID: 30408
	public vp_Value<bool> CanInteract;

	// Token: 0x040076C9 RID: 30409
	public vp_Value<string> CurrentWeaponClipType;

	// Token: 0x040076CA RID: 30410
	public vp_Attempt<object> AddAmmo;

	// Token: 0x040076CB RID: 30411
	public vp_Attempt RemoveClip;
}
