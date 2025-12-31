using System;
using UnityEngine;

// Token: 0x020012F6 RID: 4854
public class vp_FPSDemoManager : vp_DemoManager
{
	// Token: 0x17000F72 RID: 3954
	// (get) Token: 0x06009740 RID: 38720 RVA: 0x003C38E4 File Offset: 0x003C1AE4
	public vp_Shooter CurrentShooter
	{
		get
		{
			if (this.m_CurrentShooter == null || (this.m_CurrentShooter != null && (!this.m_CurrentShooter.enabled || !vp_Utility.IsActive(this.m_CurrentShooter.gameObject))))
			{
				this.m_CurrentShooter = this.Player.GetComponentInChildren<vp_Shooter>();
			}
			return this.m_CurrentShooter;
		}
	}

	// Token: 0x17000F73 RID: 3955
	// (get) Token: 0x06009741 RID: 38721 RVA: 0x003C3944 File Offset: 0x003C1B44
	// (set) Token: 0x06009742 RID: 38722 RVA: 0x003C3980 File Offset: 0x003C1B80
	public bool DrawCrosshair
	{
		get
		{
			vp_SimpleCrosshair vp_SimpleCrosshair = (vp_SimpleCrosshair)this.Player.GetComponent(typeof(vp_SimpleCrosshair));
			return !(vp_SimpleCrosshair == null) && vp_SimpleCrosshair.enabled;
		}
		set
		{
			vp_SimpleCrosshair vp_SimpleCrosshair = (vp_SimpleCrosshair)this.Player.GetComponent(typeof(vp_SimpleCrosshair));
			if (vp_SimpleCrosshair != null)
			{
				vp_SimpleCrosshair.enabled = value;
			}
		}
	}

	// Token: 0x06009743 RID: 38723 RVA: 0x003C39B8 File Offset: 0x003C1BB8
	public vp_FPSDemoManager(GameObject player)
	{
		this.Player = player;
		this.Controller = this.Player.GetComponent<vp_FPController>();
		this.Camera = this.Player.GetComponentInChildren<vp_FPCamera>();
		this.WeaponHandler = this.Player.GetComponentInChildren<vp_WeaponHandler>();
		this.PlayerEventHandler = (vp_FPPlayerEventHandler)this.Player.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
		this.Input = this.Player.GetComponent<vp_FPInput>();
		this.Earthquake = (vp_FPEarthquake)UnityEngine.Object.FindObjectOfType(typeof(vp_FPEarthquake));
		if (Screen.width < 1024)
		{
			this.EditorPreviewSectionExpanded = false;
		}
	}

	// Token: 0x06009744 RID: 38724 RVA: 0x003C3A8F File Offset: 0x003C1C8F
	public void Teleport(Vector3 pos, Vector2 startAngle)
	{
		this.Controller.SetPosition(pos);
		this.Camera.SetRotation(startAngle);
	}

	// Token: 0x06009745 RID: 38725 RVA: 0x003C3AAC File Offset: 0x003C1CAC
	public void SmoothLookAt(Vector3 lookPoint)
	{
		this.m_CurrentLookPoint = Vector3.SmoothDamp(this.m_CurrentLookPoint, lookPoint, ref this.m_LookVelocity, this.LookDamping);
		this.Camera.transform.LookAt(this.m_CurrentLookPoint);
		this.Camera.Angle = new Vector2(this.Camera.transform.eulerAngles.x, this.Camera.transform.eulerAngles.y);
	}

	// Token: 0x06009746 RID: 38726 RVA: 0x003C3B28 File Offset: 0x003C1D28
	public void SnapLookAt(Vector3 lookPoint)
	{
		this.m_CurrentLookPoint = lookPoint;
		this.Camera.transform.LookAt(this.m_CurrentLookPoint);
		this.Camera.Angle = new Vector2(this.Camera.transform.eulerAngles.x, this.Camera.transform.eulerAngles.y);
	}

	// Token: 0x06009747 RID: 38727 RVA: 0x003C3B8C File Offset: 0x003C1D8C
	public void FreezePlayer(Vector3 pos, Vector2 startAngle, bool freezeCamera)
	{
		this.m_UnFreezePosition = this.Controller.transform.position;
		this.Teleport(pos, startAngle);
		this.Controller.SetState("Freeze", true, false, false);
		this.Controller.Stop();
		if (freezeCamera)
		{
			this.Camera.SetState("Freeze", true, false, false);
			this.Input.SetState("Freeze", true, false, false);
		}
	}

	// Token: 0x06009748 RID: 38728 RVA: 0x003C3BFE File Offset: 0x003C1DFE
	public void FreezePlayer(Vector3 pos, Vector2 startAngle)
	{
		this.FreezePlayer(pos, startAngle, false);
	}

	// Token: 0x06009749 RID: 38729 RVA: 0x003C3C0C File Offset: 0x003C1E0C
	public void UnFreezePlayer()
	{
		this.Controller.transform.position = this.m_UnFreezePosition;
		this.m_UnFreezePosition = Vector3.zero;
		this.Controller.SetState("Freeze", false, false, false);
		this.Camera.SetState("Freeze", false, false, false);
		this.Input.SetState("Freeze", false, false, false);
		this.Input.Refresh();
	}

	// Token: 0x0600974A RID: 38730 RVA: 0x003C3C80 File Offset: 0x003C1E80
	public void LockControls()
	{
		this.Input.AllowGameplayInput = false;
		this.Input.MouseLookSensitivity = Vector2.zero;
		if (this.WeaponHandler.CurrentWeapon != null)
		{
			((vp_FPWeapon)this.WeaponHandler.CurrentWeapon).RotationLookSway = Vector2.zero;
		}
	}

	// Token: 0x0600974B RID: 38731 RVA: 0x003C3CDC File Offset: 0x003C1EDC
	public void SetWeaponPreset(TextAsset weaponPreset, TextAsset shooterPreset = null, bool smoothFade = true)
	{
		if (this.WeaponHandler.CurrentWeapon == null)
		{
			return;
		}
		this.WeaponHandler.CurrentWeapon.Load(weaponPreset);
		if (!smoothFade)
		{
			((vp_FPWeapon)this.WeaponHandler.CurrentWeapon).SnapSprings();
			((vp_FPWeapon)this.WeaponHandler.CurrentWeapon).SnapPivot();
			((vp_FPWeapon)this.WeaponHandler.CurrentWeapon).SnapZoom();
		}
		this.WeaponHandler.CurrentWeapon.Refresh();
		if (shooterPreset != null && this.CurrentShooter != null)
		{
			this.CurrentShooter.Load(shooterPreset);
		}
		this.CurrentShooter.Refresh();
	}

	// Token: 0x0600974C RID: 38732 RVA: 0x003C3D90 File Offset: 0x003C1F90
	public void RefreshDefaultState()
	{
		if (this.Controller != null)
		{
			this.Controller.RefreshDefaultState();
		}
		if (this.Camera != null)
		{
			this.Camera.RefreshDefaultState();
			if (this.WeaponHandler.CurrentWeapon != null)
			{
				this.WeaponHandler.CurrentWeapon.RefreshDefaultState();
			}
			if (this.CurrentShooter != null)
			{
				this.CurrentShooter.RefreshDefaultState();
			}
		}
		if (this.Input != null)
		{
			this.Input.RefreshDefaultState();
		}
	}

	// Token: 0x0600974D RID: 38733 RVA: 0x003C3E24 File Offset: 0x003C2024
	public void ResetState()
	{
		if (this.Controller != null)
		{
			this.Controller.ResetState();
		}
		if (this.Camera != null)
		{
			this.Camera.ResetState();
			if (this.WeaponHandler.CurrentWeapon != null)
			{
				this.WeaponHandler.CurrentWeapon.ResetState();
			}
			if (this.CurrentShooter != null)
			{
				this.CurrentShooter.ResetState();
			}
		}
		if (this.Input != null)
		{
			this.Input.ResetState();
		}
	}

	// Token: 0x0600974E RID: 38734 RVA: 0x003C3EB8 File Offset: 0x003C20B8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Reset()
	{
		base.Reset();
		this.PlayerEventHandler.RefreshActivityStates();
		this.WeaponHandler.SetWeapon(0);
		this.PlayerEventHandler.CameraEarthQuake.Stop(0f);
		this.Camera.BobStepCallback = null;
		this.Camera.SnapSprings();
		if (this.WeaponHandler.CurrentWeapon != null)
		{
			((vp_FPWeapon)this.WeaponHandler.CurrentWeapon).SetPivotVisible(false);
			this.WeaponHandler.CurrentWeapon.SnapSprings();
			vp_Layer.Set(this.WeaponHandler.CurrentWeapon.gameObject, 10, true);
		}
		if (Screen.width < 1024)
		{
			this.EditorPreviewSectionExpanded = false;
		}
		else
		{
			this.EditorPreviewSectionExpanded = true;
		}
		if (this.m_UnFreezePosition != Vector3.zero)
		{
			this.UnFreezePlayer();
		}
	}

	// Token: 0x0600974F RID: 38735 RVA: 0x003C3F93 File Offset: 0x003C2193
	public void ForceCameraShake(float speed, Vector3 amplitude)
	{
		this.Camera.ShakeSpeed = speed;
		this.Camera.ShakeAmplitude = amplitude;
	}

	// Token: 0x06009750 RID: 38736 RVA: 0x003C3FAD File Offset: 0x003C21AD
	public void ForceCameraShake()
	{
		this.ForceCameraShake(0.0727273f, new Vector3(-10f, 10f, 0f));
	}

	// Token: 0x040073B3 RID: 29619
	public GameObject Player;

	// Token: 0x040073B4 RID: 29620
	public vp_FPController Controller;

	// Token: 0x040073B5 RID: 29621
	public vp_FPCamera Camera;

	// Token: 0x040073B6 RID: 29622
	public vp_WeaponHandler WeaponHandler;

	// Token: 0x040073B7 RID: 29623
	public vp_FPInput Input;

	// Token: 0x040073B8 RID: 29624
	public vp_FPEarthquake Earthquake;

	// Token: 0x040073B9 RID: 29625
	public vp_FPPlayerEventHandler PlayerEventHandler;

	// Token: 0x040073BA RID: 29626
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 m_UnFreezePosition = Vector3.zero;

	// Token: 0x040073BB RID: 29627
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 m_CurrentLookPoint = Vector3.zero;

	// Token: 0x040073BC RID: 29628
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 m_LookVelocity = Vector3.zero;

	// Token: 0x040073BD RID: 29629
	public float LookDamping = 0.3f;

	// Token: 0x040073BE RID: 29630
	[PublicizedFrom(EAccessModifier.Private)]
	public vp_Shooter m_CurrentShooter;
}
