using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001335 RID: 4917
[RequireComponent(typeof(Rigidbody))]
public class vp_Grab : vp_Interactable
{
	// Token: 0x060098D1 RID: 39121 RVA: 0x003CC568 File Offset: 0x003CA768
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		if (!base.GetComponent<Rigidbody>() || !base.GetComponent<Collider>())
		{
			base.enabled = false;
		}
		if (base.GetComponent<Rigidbody>() != null)
		{
			this.m_DefaultGravity = base.GetComponent<Rigidbody>().useGravity;
			this.m_DefaultDrag = this.m_Transform.GetComponent<Rigidbody>().drag;
			this.m_DefaultAngularDrag = this.m_Transform.GetComponent<Rigidbody>().angularDrag;
		}
		this.InteractType = vp_Interactable.vp_InteractType.Normal;
		this.m_InteractManager = (UnityEngine.Object.FindObjectOfType(typeof(vp_FPInteractManager)) as vp_FPInteractManager);
	}

	// Token: 0x060098D2 RID: 39122 RVA: 0x003CC608 File Offset: 0x003CA808
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void FixedUpdate()
	{
		if (!this.m_IsGrabbed || this.m_Transform.parent == null)
		{
			return;
		}
		this.UpdateShake();
		this.UpdatePosition();
		this.UpdateRotation();
		this.UpdateBurden();
		this.DampenForces();
	}

	// Token: 0x060098D3 RID: 39123 RVA: 0x003CC644 File Offset: 0x003CA844
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		if (!this.m_IsGrabbed || this.m_Transform.parent == null)
		{
			return;
		}
		this.UpdateInput();
	}

	// Token: 0x060098D4 RID: 39124 RVA: 0x003CC668 File Offset: 0x003CA868
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateInput()
	{
		this.m_CurrentMouseMove.x = this.m_Player.InputRawLook.Get().x * Time.timeScale;
		this.m_CurrentMouseMove.y = this.m_Player.InputRawLook.Get().y * Time.timeScale;
		if (this.m_Player.InputGetButtonDown.Send("Attack"))
		{
			this.m_Player.Interact.TryStart(true);
			return;
		}
		if (this.m_Player.CurrentWeaponIndex.Get() != 0)
		{
			this.m_Player.SetWeapon.TryStart<int>(0);
			return;
		}
	}

	// Token: 0x060098D5 RID: 39125 RVA: 0x003CC724 File Offset: 0x003CA924
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateShake()
	{
		this.m_CurrentShake = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(this.ShakeSpeed), this.ShakeAmplitude);
		this.m_Transform.localEulerAngles += this.m_CurrentShake;
	}

	// Token: 0x060098D6 RID: 39126 RVA: 0x003CC760 File Offset: 0x003CA960
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdatePosition()
	{
		this.m_CurrentSwayForce += this.m_Player.Velocity.Get() * 0.005f;
		this.m_CurrentSwayForce.y = this.m_CurrentSwayForce.y + this.m_CurrentFootstepForce;
		this.m_CurrentSwayForce += this.m_Camera.Transform.TransformDirection(new Vector3(this.m_CurrentMouseMove.x * 0.05f, (this.m_Player.Rotation.Get().x > this.m_Camera.RotationPitchLimit.y) ? (this.m_CurrentMouseMove.y * 0.015f) : (this.m_CurrentMouseMove.y * 0.05f), 0f));
		this.m_TempCarryingOffset = (this.m_Player.IsFirstPerson.Get() ? this.CarryingOffset : (this.CarryingOffset - this.m_Camera.Position3rdPersonOffset));
		this.m_Transform.position = Vector3.Lerp(this.m_Transform.position, this.m_Camera.Transform.position - this.m_CurrentSwayForce + this.m_Camera.Transform.right * this.m_TempCarryingOffset.x + this.m_Camera.Transform.up * this.m_Transform.localScale.y * (this.m_TempCarryingOffset.y + this.m_CurrentShake.y * 0.5f) + this.m_Camera.Transform.forward * this.m_TempCarryingOffset.z, (this.m_FetchProgress < 1f) ? this.m_FetchProgress : (Time.deltaTime * (this.Stiffness * 60f)));
	}

	// Token: 0x060098D7 RID: 39127 RVA: 0x003CC96C File Offset: 0x003CAB6C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateRotation()
	{
		this.m_Camera.RotationPitchLimit = Vector2.Lerp(this.m_Camera.RotationPitchLimit, new Vector2(this.m_Camera.RotationPitchLimit.x, this.CameraPitchDownLimit), this.m_FetchProgress);
		if (this.m_DisableAngleSwayTimer.Active)
		{
			return;
		}
		this.m_CurrentSwayTorque += this.m_Player.Velocity.Get() * 0.005f;
		this.m_CurrentRotationSway = this.m_Camera.Transform.InverseTransformDirection(this.m_CurrentSwayTorque * 1.5f);
		this.m_CurrentRotationSway.y = this.m_CurrentRotationSway.z;
		this.m_CurrentRotationSway.z = this.m_CurrentRotationSway.x;
		this.m_CurrentRotationSway.x = -this.m_CurrentRotationSway.y * 0.5f;
		this.m_CurrentRotationSway.y = 0f;
		Quaternion localRotation = this.m_Transform.localRotation;
		this.m_Transform.Rotate(this.m_Camera.transform.forward, this.m_CurrentRotationSway.z * -0.5f * Time.timeScale);
		this.m_Transform.Rotate(this.m_Camera.transform.right, this.m_CurrentRotationSway.x * -0.5f * Time.timeScale);
		Quaternion localRotation2 = this.m_Transform.localRotation;
		this.m_Transform.localRotation = localRotation;
		this.m_Transform.localRotation = Quaternion.Slerp(localRotation2, Quaternion.Euler(this.m_CurrentHoldAngle + this.m_CurrentShake * 50f), Time.deltaTime * (this.Stiffness * 60f));
	}

	// Token: 0x060098D8 RID: 39128 RVA: 0x003CCB40 File Offset: 0x003CAD40
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateBurden()
	{
		if (this.Burden <= 0f)
		{
			return;
		}
		this.m_Player.MotorThrottle.Set(this.m_Player.MotorThrottle.Get() * (1f - Mathf.Clamp01(this.Burden)));
	}

	// Token: 0x060098D9 RID: 39129 RVA: 0x003CCB9B File Offset: 0x003CAD9B
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void DampenForces()
	{
		this.m_CurrentSwayForce *= 0.9f;
		this.m_CurrentSwayTorque *= 0.9f;
		this.m_CurrentFootstepForce *= 0.9f;
	}

	// Token: 0x060098DA RID: 39130 RVA: 0x003CCBDC File Offset: 0x003CADDC
	public override bool TryInteract(vp_FPPlayerEventHandler player)
	{
		if (this.m_Player == null)
		{
			this.m_Player = player;
		}
		if (player == null)
		{
			return false;
		}
		if (this.m_Controller == null)
		{
			this.m_Controller = this.m_Player.GetComponent<vp_FPController>();
		}
		if (this.m_Controller == null)
		{
			return false;
		}
		if (this.m_Camera == null)
		{
			this.m_Camera = this.m_Player.GetComponentInChildren<vp_FPCamera>();
		}
		if (this.m_Camera == null)
		{
			return false;
		}
		if (this.m_WeaponHandler == null)
		{
			this.m_WeaponHandler = this.m_Player.GetComponentInChildren<vp_WeaponHandler>();
		}
		if (this.m_Audio == null)
		{
			this.m_Audio = this.m_Player.GetComponent<AudioSource>();
		}
		this.m_Player.Register(this);
		if (!this.m_IsGrabbed)
		{
			this.StartGrab();
		}
		else
		{
			this.StopGrab();
		}
		this.m_Player.Interactable.Set(this);
		if (this.GrabStateCrosshair != null)
		{
			this.m_Player.Crosshair.Set(this.GrabStateCrosshair);
		}
		else
		{
			this.m_Player.Crosshair.Set(new Texture2D(0, 0));
		}
		return true;
	}

	// Token: 0x060098DB RID: 39131 RVA: 0x003CCD28 File Offset: 0x003CAF28
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void StartGrab()
	{
		vp_AudioUtility.PlayRandomSound(this.m_Audio, this.GrabSounds, this.SoundsPitch);
		if (!string.IsNullOrEmpty(this.OnGrabText))
		{
			this.m_Player.HUDText.Send(this.OnGrabText);
		}
		vp_FPCamera camera = this.m_Camera;
		camera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Combine(camera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.Footstep));
		this.m_LastWeaponEquipped = this.m_Player.CurrentWeaponIndex.Get();
		this.m_OriginalPitchDownLimit = this.m_Camera.RotationPitchLimit.y;
		this.m_FetchProgress = 0f;
		if (this.m_LastWeaponEquipped != 0)
		{
			this.m_Player.SetWeapon.TryStart<int>(0);
		}
		else if (!this.m_IsFetching)
		{
			base.StartCoroutine("Fetch");
		}
		if (this.m_Transform.GetComponent<Rigidbody>() != null)
		{
			this.m_Transform.GetComponent<Rigidbody>().useGravity = false;
			this.m_Transform.GetComponent<Rigidbody>().drag = this.Stiffness * 60f;
			this.m_Transform.GetComponent<Rigidbody>().angularDrag = this.Stiffness * 60f;
		}
		if (this.m_Controller.Transform.GetComponent<Collider>().enabled && this.m_Transform.GetComponent<Collider>().enabled)
		{
			Physics.IgnoreCollision(this.m_Controller.Transform.GetComponent<Collider>(), this.m_Transform.GetComponent<Collider>(), true);
		}
		this.m_Transform.parent = this.m_Camera.Transform;
		this.m_CurrentHoldAngle = this.m_Transform.localEulerAngles;
		this.m_IsGrabbed = true;
	}

	// Token: 0x060098DC RID: 39132 RVA: 0x003CCEDC File Offset: 0x003CB0DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void StopGrab()
	{
		this.m_IsGrabbed = false;
		this.m_FetchProgress = 1f;
		vp_FPCamera camera = this.m_Camera;
		camera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Remove(camera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.Footstep));
		this.m_Player.SetWeapon.TryStart<int>(this.m_LastWeaponEquipped);
		if (this.m_Transform.GetComponent<Rigidbody>() != null)
		{
			this.m_Transform.GetComponent<Rigidbody>().useGravity = this.m_DefaultGravity;
			this.m_Transform.GetComponent<Rigidbody>().drag = this.m_DefaultDrag;
			this.m_Transform.GetComponent<Rigidbody>().angularDrag = this.m_DefaultAngularDrag;
		}
		if (!this.m_Player.Dead.Active && vp_Utility.IsActive(this.m_Transform.gameObject) && this.m_Controller.Transform.GetComponent<Collider>().enabled && this.m_Transform.GetComponent<Collider>().enabled)
		{
			Physics.IgnoreCollision(this.m_Controller.Transform.GetComponent<Collider>(), this.m_Transform.GetComponent<Collider>(), false);
		}
		Vector3 eulerAngles = this.m_Transform.eulerAngles;
		this.m_Transform.parent = null;
		this.m_Transform.eulerAngles = eulerAngles;
		if (this.m_Transform.GetComponent<Rigidbody>() != null)
		{
			this.m_Transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
			this.m_Transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
		if (this.m_Player.InputGetButtonDown.Send("Attack"))
		{
			vp_AudioUtility.PlayRandomSound(this.m_Audio, this.ThrowSounds, this.SoundsPitch);
			if (this.m_Transform.GetComponent<Rigidbody>() != null)
			{
				this.m_Transform.GetComponent<Rigidbody>().AddForce(this.m_Player.Velocity.Get() + this.m_Player.CameraLookDirection.Get() * this.ThrowStrength, ForceMode.Impulse);
				if (this.AllowThrowRotation)
				{
					this.m_Transform.GetComponent<Rigidbody>().AddTorque(this.m_Camera.Transform.forward * ((UnityEngine.Random.value > 0.5f) ? 0.5f : -0.5f) + this.m_Camera.Transform.right * ((UnityEngine.Random.value > 0.5f) ? 0.5f : -0.5f), ForceMode.Impulse);
				}
			}
		}
		else
		{
			vp_AudioUtility.PlayRandomSound(this.m_Audio, this.DropSounds, this.SoundsPitch);
			if (this.m_Transform.GetComponent<Rigidbody>() != null)
			{
				this.m_Transform.GetComponent<Rigidbody>().AddForce(this.m_Player.Velocity.Get() + this.m_Player.CameraLookDirection.Get(), ForceMode.Impulse);
			}
		}
		if (this.m_InteractManager == null)
		{
			this.m_InteractManager = (UnityEngine.Object.FindObjectOfType(typeof(vp_FPInteractManager)) as vp_FPInteractManager);
		}
		this.m_InteractManager.CrosshairTimeoutTimer = Time.time + 0.5f;
		vp_Timer.In(0.1f, delegate()
		{
			this.m_Camera.RotationPitchLimit.y = this.m_OriginalPitchDownLimit;
		}, null);
	}

	// Token: 0x060098DD RID: 39133 RVA: 0x003CD231 File Offset: 0x003CB431
	public override void FinishInteraction()
	{
		if (this.m_IsGrabbed)
		{
			this.StopGrab();
		}
	}

	// Token: 0x060098DE RID: 39134 RVA: 0x003CD241 File Offset: 0x003CB441
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual IEnumerator Fetch()
	{
		this.m_IsFetching = true;
		this.m_CurrentSwayForce = Vector3.zero;
		this.m_CurrentSwayTorque = Vector3.zero;
		this.m_CurrentFootstepForce = 0f;
		this.m_FetchProgress = 0f;
		this.duration = Vector3.Distance(this.m_Camera.Transform.position, this.m_Transform.position) * 0.5f;
		vp_Timer.In(this.duration + 1f, delegate()
		{
		}, this.m_DisableAngleSwayTimer);
		while (this.m_FetchProgress < 1f)
		{
			this.m_FetchProgress += Time.deltaTime / this.duration;
			yield return new WaitForEndOfFrame();
		}
		this.m_IsFetching = false;
		yield break;
	}

	// Token: 0x060098DF RID: 39135 RVA: 0x003CD250 File Offset: 0x003CB450
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnCollisionEnter(Collision col)
	{
		if (!this.m_IsGrabbed)
		{
			return;
		}
		if (this.m_FetchProgress < 1f)
		{
			this.m_FetchProgress *= 1.2f;
		}
		vp_Timer.In(2f, delegate()
		{
		}, this.m_DisableAngleSwayTimer);
	}

	// Token: 0x060098E0 RID: 39136 RVA: 0x003CD2B4 File Offset: 0x003CB4B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnCollisionStay(Collision col)
	{
		if (!this.m_IsGrabbed || this.MaxCollisionCount == 0)
		{
			return;
		}
		if (col.collider != this.m_LastExternalCollider)
		{
			if (!col.collider.GetComponent<Rigidbody>() || col.collider.GetComponent<Rigidbody>().isKinematic)
			{
				this.m_LastExternalCollider = col.collider;
				this.m_CollisionCount = 1;
				return;
			}
		}
		else
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(this.m_Transform.position, this.m_Camera.Transform.forward, out raycastHit, 1f) && raycastHit.collider == this.m_LastExternalCollider && this.m_FetchProgress >= 1f)
			{
				this.m_CollisionCount = this.MaxCollisionCount;
			}
			this.m_CollisionCount++;
			if (this.m_CollisionCount > this.MaxCollisionCount && (!Physics.Raycast(col.contacts[0].point + Vector3.up * 0.1f, -Vector3.up, out raycastHit, 0.2f) || !(raycastHit.collider == this.m_LastExternalCollider)))
			{
				this.m_CollisionCount = 0;
				this.m_FetchProgress = 1f;
				this.m_LastExternalCollider = null;
				if (this.m_Player != null)
				{
					this.m_Player.Interact.TryStart(true);
				}
			}
		}
	}

	// Token: 0x060098E1 RID: 39137 RVA: 0x003CD41F File Offset: 0x003CB61F
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnCollisionExit()
	{
		this.m_CurrentHoldAngle = this.m_Transform.localEulerAngles;
	}

	// Token: 0x060098E2 RID: 39138 RVA: 0x003CD432 File Offset: 0x003CB632
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Footstep()
	{
		this.m_CurrentFootstepForce += this.FootstepForce;
	}

	// Token: 0x060098E3 RID: 39139 RVA: 0x003CD447 File Offset: 0x003CB647
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnMessage_FallImpact(float impact)
	{
		this.m_CurrentSwayForce.y = this.m_CurrentSwayForce.y + impact * this.Kneeling;
	}

	// Token: 0x060098E4 RID: 39140 RVA: 0x003CD460 File Offset: 0x003CB660
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStop_SetWeapon()
	{
		if (this.m_IsGrabbed && !this.m_IsFetching)
		{
			base.StartCoroutine("Fetch");
		}
	}

	// Token: 0x060098E5 RID: 39141 RVA: 0x003CD480 File Offset: 0x003CB680
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CanStart_SetWeapon()
	{
		int num = (int)this.m_Player.SetWeapon.Argument;
		return !this.m_IsGrabbed || num == 0;
	}

	// Token: 0x0400755B RID: 30043
	public string OnGrabText = "";

	// Token: 0x0400755C RID: 30044
	public Texture GrabStateCrosshair;

	// Token: 0x0400755D RID: 30045
	public float FootstepForce = 0.015f;

	// Token: 0x0400755E RID: 30046
	public float Kneeling = 5f;

	// Token: 0x0400755F RID: 30047
	public float Stiffness = 0.5f;

	// Token: 0x04007560 RID: 30048
	public float ShakeSpeed = 0.1f;

	// Token: 0x04007561 RID: 30049
	public Vector3 ShakeAmplitude = Vector3.one;

	// Token: 0x04007562 RID: 30050
	public float ThrowStrength = 6f;

	// Token: 0x04007563 RID: 30051
	public bool AllowThrowRotation = true;

	// Token: 0x04007564 RID: 30052
	public float Burden;

	// Token: 0x04007565 RID: 30053
	public int MaxCollisionCount = 20;

	// Token: 0x04007566 RID: 30054
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_CurrentShake = Vector3.zero;

	// Token: 0x04007567 RID: 30055
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_CurrentRotationSway = Vector3.zero;

	// Token: 0x04007568 RID: 30056
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector2 m_CurrentMouseMove;

	// Token: 0x04007569 RID: 30057
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_CurrentSwayForce;

	// Token: 0x0400756A RID: 30058
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_CurrentSwayTorque;

	// Token: 0x0400756B RID: 30059
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_CurrentFootstepForce;

	// Token: 0x0400756C RID: 30060
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_CurrentHoldAngle = Vector3.one;

	// Token: 0x0400756D RID: 30061
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Collider m_LastExternalCollider;

	// Token: 0x0400756E RID: 30062
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int m_CollisionCount;

	// Token: 0x0400756F RID: 30063
	public Vector3 CarryingOffset = new Vector3(0f, -0.5f, 1.5f);

	// Token: 0x04007570 RID: 30064
	public float CameraPitchDownLimit;

	// Token: 0x04007571 RID: 30065
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_TempCarryingOffset = Vector3.zero;

	// Token: 0x04007572 RID: 30066
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_IsFetching;

	// Token: 0x04007573 RID: 30067
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float duration;

	// Token: 0x04007574 RID: 30068
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_FetchProgress;

	// Token: 0x04007575 RID: 30069
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPInteractManager m_InteractManager;

	// Token: 0x04007576 RID: 30070
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioSource m_Audio;

	// Token: 0x04007577 RID: 30071
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int m_LastWeaponEquipped;

	// Token: 0x04007578 RID: 30072
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_IsGrabbed;

	// Token: 0x04007579 RID: 30073
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_OriginalPitchDownLimit;

	// Token: 0x0400757A RID: 30074
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_DefaultGravity;

	// Token: 0x0400757B RID: 30075
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_DefaultDrag;

	// Token: 0x0400757C RID: 30076
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_DefaultAngularDrag;

	// Token: 0x0400757D RID: 30077
	public Vector2 SoundsPitch = new Vector2(1f, 1.5f);

	// Token: 0x0400757E RID: 30078
	public List<AudioClip> GrabSounds = new List<AudioClip>();

	// Token: 0x0400757F RID: 30079
	public List<AudioClip> DropSounds = new List<AudioClip>();

	// Token: 0x04007580 RID: 30080
	public List<AudioClip> ThrowSounds = new List<AudioClip>();

	// Token: 0x04007581 RID: 30081
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_DisableAngleSwayTimer = new vp_Timer.Handle();
}
