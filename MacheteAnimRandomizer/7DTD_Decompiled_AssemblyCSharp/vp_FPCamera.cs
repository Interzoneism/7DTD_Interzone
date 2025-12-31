using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02001349 RID: 4937
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))]
[Preserve]
public class vp_FPCamera : vp_Component
{
	// Token: 0x17000FAC RID: 4012
	// (get) Token: 0x06009959 RID: 39257 RVA: 0x003CF971 File Offset: 0x003CDB71
	// (set) Token: 0x0600995A RID: 39258 RVA: 0x003CF979 File Offset: 0x003CDB79
	public bool DrawCameraCollisionDebugLine
	{
		get
		{
			return this.m_DrawCameraCollisionDebugLine;
		}
		set
		{
			this.m_DrawCameraCollisionDebugLine = value;
		}
	}

	// Token: 0x17000FAD RID: 4013
	// (get) Token: 0x0600995B RID: 39259 RVA: 0x003CF982 File Offset: 0x003CDB82
	public Vector3 CollisionVector
	{
		get
		{
			return this.m_CollisionVector;
		}
	}

	// Token: 0x17000FAE RID: 4014
	// (get) Token: 0x0600995C RID: 39260 RVA: 0x003CF98A File Offset: 0x003CDB8A
	// (set) Token: 0x0600995D RID: 39261 RVA: 0x003CF992 File Offset: 0x003CDB92
	public bool HasOverheadSpace { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000FAF RID: 4015
	// (get) Token: 0x0600995E RID: 39262 RVA: 0x003CF99B File Offset: 0x003CDB9B
	public vp_FPPlayerEventHandler Player
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (this.m_Player == null && base.EventHandler != null)
			{
				this.m_Player = (vp_FPPlayerEventHandler)base.EventHandler;
			}
			return this.m_Player;
		}
	}

	// Token: 0x17000FB0 RID: 4016
	// (get) Token: 0x0600995F RID: 39263 RVA: 0x003CF9D0 File Offset: 0x003CDBD0
	public Rigidbody FirstRigidBody
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (this.m_FirstRigidbody == null)
			{
				this.m_FirstRigidbody = base.Transform.root.GetComponentInChildren<Rigidbody>();
			}
			return this.m_FirstRigidbody;
		}
	}

	// Token: 0x17000FB1 RID: 4017
	// (get) Token: 0x06009960 RID: 39264 RVA: 0x003CF9FC File Offset: 0x003CDBFC
	// (set) Token: 0x06009961 RID: 39265 RVA: 0x003CFA0F File Offset: 0x003CDC0F
	public Vector2 Angle
	{
		get
		{
			return new Vector2(this.m_Pitch, this.m_Yaw);
		}
		set
		{
			this.Pitch = value.x;
			this.Yaw = value.y;
		}
	}

	// Token: 0x17000FB2 RID: 4018
	// (get) Token: 0x06009962 RID: 39266 RVA: 0x003CFA29 File Offset: 0x003CDC29
	public Vector3 Forward
	{
		get
		{
			return this.m_Transform.forward;
		}
	}

	// Token: 0x17000FB3 RID: 4019
	// (get) Token: 0x06009963 RID: 39267 RVA: 0x003CFA36 File Offset: 0x003CDC36
	// (set) Token: 0x06009964 RID: 39268 RVA: 0x003CFA3E File Offset: 0x003CDC3E
	public float Pitch
	{
		get
		{
			return this.m_Pitch;
		}
		set
		{
			if (value > 90f)
			{
				value -= 360f;
			}
			this.m_Pitch = value;
		}
	}

	// Token: 0x17000FB4 RID: 4020
	// (get) Token: 0x06009965 RID: 39269 RVA: 0x003CFA58 File Offset: 0x003CDC58
	// (set) Token: 0x06009966 RID: 39270 RVA: 0x003CFA60 File Offset: 0x003CDC60
	public float Yaw
	{
		get
		{
			return this.m_Yaw;
		}
		set
		{
			this.m_Yaw = value;
		}
	}

	// Token: 0x17000FB5 RID: 4021
	// (get) Token: 0x06009967 RID: 39271 RVA: 0x003CFA69 File Offset: 0x003CDC69
	// (set) Token: 0x06009968 RID: 39272 RVA: 0x003CFA71 File Offset: 0x003CDC71
	public virtual bool OnValue_IsFirstPerson
	{
		get
		{
			return this.m_IsFirstPerson;
		}
		set
		{
			this.m_IsFirstPerson = value;
		}
	}

	// Token: 0x17000FB6 RID: 4022
	// (get) Token: 0x06009969 RID: 39273 RVA: 0x000F888E File Offset: 0x000F6A8E
	public virtual Transform OnValue_CameraTransform
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return base.transform;
		}
	}

	// Token: 0x0600996A RID: 39274 RVA: 0x003CFA7C File Offset: 0x003CDC7C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.FPController = base.Transform.parent.GetComponent<vp_FPController>();
		this.SetRotation(new Vector2(base.Transform.eulerAngles.x, base.Transform.eulerAngles.y));
		base.GetComponent<Camera>().fieldOfView = (float)Constants.cDefaultCameraFieldOfView;
		this.m_PositionSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.Position, false);
		this.m_PositionSpring.MinVelocity = 1E-05f;
		this.m_PositionSpring.RestState = this.PositionOffset + this.AimingPositionOffset;
		this.m_PositionSpring2 = new vp_Spring(base.Transform, vp_Spring.UpdateMode.PositionAdditiveLocal, false);
		this.m_PositionSpring2.MinVelocity = 1E-05f;
		this.m_RotationSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.RotationAdditiveLocal, false);
		this.m_RotationSpring.MinVelocity = 1E-05f;
		this.m_RotationSpring.SdtdStopping = true;
		this.HasOverheadSpace = true;
	}

	// Token: 0x0600996B RID: 39275 RVA: 0x003CFB7C File Offset: 0x003CDD7C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEnable()
	{
		base.OnEnable();
		vp_TargetEvent<float>.Register(base.Parent, "CameraBombShake", new Action<float>(this.OnMessage_CameraBombShake));
		vp_TargetEvent<float>.Register(base.Parent, "CameraGroundStomp", new Action<float>(this.OnMessage_CameraGroundStomp));
	}

	// Token: 0x0600996C RID: 39276 RVA: 0x003CFBCC File Offset: 0x003CDDCC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnDisable()
	{
		base.OnDisable();
		vp_TargetEvent<float>.Unregister(base.Parent, "CameraBombShake", new Action<float>(this.OnMessage_CameraBombShake));
		vp_TargetEvent<float>.Unregister(base.Parent, "CameraGroundStomp", new Action<float>(this.OnMessage_CameraGroundStomp));
	}

	// Token: 0x0600996D RID: 39277 RVA: 0x003CFC19 File Offset: 0x003CDE19
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		this.Refresh();
		this.SnapSprings();
		this.SnapZoom();
	}

	// Token: 0x0600996E RID: 39278 RVA: 0x003CFC33 File Offset: 0x003CDE33
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Init()
	{
		base.Init();
	}

	// Token: 0x0600996F RID: 39279 RVA: 0x003CFC3B File Offset: 0x003CDE3B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateInput();
	}

	// Token: 0x06009970 RID: 39280 RVA: 0x003CFC58 File Offset: 0x003CDE58
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void FixedUpdate()
	{
		if (!this.hasLateUpdateRan)
		{
			return;
		}
		this.hasLateUpdateRan = false;
		base.FixedUpdate();
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (this.Locked3rdPerson)
		{
			return;
		}
		this.UpdateZoom();
		this.UpdateSwaying();
		this.UpdateBob();
		this.UpdateEarthQuake();
		this.UpdateShakes();
		Vector3 position = base.Transform.position;
		this.UpdateSprings();
		base.Transform.position = position;
	}

	// Token: 0x06009971 RID: 39281 RVA: 0x003CFCD0 File Offset: 0x003CDED0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void LateUpdate()
	{
		this.hasLateUpdateRan = true;
		base.LateUpdate();
		if (this.Locked3rdPerson)
		{
			if (!this.Player.Driving.Active)
			{
				Quaternion lhs = Quaternion.AngleAxis(this.m_Yaw, Vector3.up);
				Quaternion rhs = Quaternion.AngleAxis(0f, Vector3.left);
				base.Parent.rotation = vp_MathUtility.NaNSafeQuaternion(lhs * rhs, base.Parent.rotation);
			}
			return;
		}
		if (this.FPController.enabled)
		{
			if (this.Player.Driving.Active)
			{
				this.m_Transform.position = this.DrivingPosition;
			}
			else if (this.Player.IsFirstPerson.Get())
			{
				this.m_Transform.position = this.FPController.SmoothPosition;
			}
			else
			{
				this.m_Transform.position = this.FPController.transform.position;
			}
			if (this.Player.IsFirstPerson.Get())
			{
				this.m_Transform.localPosition += this.m_PositionSpring.State + this.m_PositionSpring2.State;
			}
			else if (!this.Player.Driving.Active)
			{
				this.m_Transform.localPosition += this.m_PositionSpring.State + Vector3.Scale(this.m_PositionSpring2.State, Vector3.up);
			}
			if (this.HasCollision)
			{
				this.DoCameraCollision();
			}
		}
		Quaternion lhs2 = Quaternion.AngleAxis(this.m_Yaw, Vector3.up);
		if (this.Player.Driving.Active)
		{
			Quaternion rhs2 = Quaternion.AngleAxis(-this.m_Pitch, Vector3.left);
			base.Transform.rotation = vp_MathUtility.NaNSafeQuaternion(lhs2 * rhs2, base.Transform.rotation);
		}
		else
		{
			Quaternion rhs3 = Quaternion.AngleAxis(0f, Vector3.left);
			base.Parent.rotation = vp_MathUtility.NaNSafeQuaternion(lhs2 * rhs3, base.Parent.rotation);
			rhs3 = Quaternion.AngleAxis(-this.m_Pitch, Vector3.left);
			base.Transform.rotation = vp_MathUtility.NaNSafeQuaternion(lhs2 * rhs3, base.Transform.rotation);
		}
		base.Transform.localEulerAngles += vp_MathUtility.NaNSafeVector3(Vector3.forward * this.m_RotationSpring.State.z, default(Vector3));
		this.Update3rdPerson();
	}

	// Token: 0x06009972 RID: 39282 RVA: 0x003CFF74 File Offset: 0x003CE174
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update3rdPerson()
	{
		if (this.Position3rdPersonOffset == Vector3.zero)
		{
			return;
		}
		if (this.PositionOnDeath != Vector3.zero)
		{
			base.Transform.position = this.PositionOnDeath;
			if (this.FirstRigidBody != null)
			{
				base.Transform.LookAt(this.FirstRigidBody.transform.position + Vector3.up);
				return;
			}
			base.Transform.LookAt(base.Root.position + Vector3.up);
			return;
		}
		else
		{
			if (this.Player.IsFirstPerson.Get() || !this.FPController.enabled)
			{
				this.m_Final3rdPersonCameraOffset = Vector3.zero;
				this.m_Current3rdPersonBlend = 0f;
				this.LookPoint = this.GetLookPoint();
				return;
			}
			this.m_Current3rdPersonBlend = Mathf.Lerp(this.m_Current3rdPersonBlend, 1f, Time.deltaTime);
			this.m_Final3rdPersonCameraOffset = base.Transform.position;
			if (!this.Player.Driving.Active && base.Transform.localPosition.z > -0.2f)
			{
				base.Transform.localPosition = new Vector3(base.Transform.localPosition.x, base.Transform.localPosition.y, -0.2f);
			}
			Vector3 vector = base.Transform.position;
			vector += this.m_Transform.right * this.Position3rdPersonOffset.x;
			vector += this.m_Transform.up * this.Position3rdPersonOffset.y;
			vector += this.m_Transform.forward * this.Position3rdPersonOffset.z;
			base.Transform.position = Vector3.Lerp(base.Transform.position, vector, this.m_Current3rdPersonBlend);
			this.m_Final3rdPersonCameraOffset -= base.Transform.position;
			this.DoCameraCollision();
			this.LookPoint = this.GetLookPoint();
			return;
		}
	}

	// Token: 0x06009973 RID: 39283 RVA: 0x003D01A0 File Offset: 0x003CE3A0
	public virtual void DoCameraCollision()
	{
		this.HasOverheadSpace = true;
		this.m_CameraCollisionStartPos = this.FPController.Transform.TransformPoint(0f, this.PositionOffset.y + this.AimingPositionOffset.y, 0f) - (this.m_Player.IsFirstPerson.Get() ? Vector3.zero : (this.FPController.Transform.position - this.FPController.SmoothPosition));
		if (this.m_Player.IsFirstPerson.Get())
		{
			Vector3 vector = this.m_CameraCollisionStartPos - Vector3.up * (this.FPController.CharacterController.height * 0.5f + 0.05f);
			this.m_CameraCollisionEndPos = vector + Vector3.up * this.FPController.CharacterController.radius * 2.1f;
			if (Physics.SphereCast(vector, this.FPController.CharacterController.radius, Vector3.up, out this.m_CameraHit, this.FPController.CharacterController.radius * 2.1f, 1082195968) && !this.m_CameraHit.collider.isTrigger)
			{
				this.m_CollisionVector = this.m_CameraCollisionEndPos - this.m_CameraHit.point;
				base.Transform.position = vector + Vector3.up * this.FPController.CharacterController.radius + Vector3.down * this.m_CollisionVector.y;
				this.HasOverheadSpace = false;
			}
			return;
		}
		this.m_CameraCollisionEndPos = base.Transform.position + (base.Transform.position - this.m_CameraCollisionStartPos).normalized * this.FPController.CharacterController.radius;
		this.m_CollisionVector = Vector3.zero;
		if (Physics.Linecast(this.m_CameraCollisionStartPos, this.m_CameraCollisionEndPos, out this.m_CameraHit, 1082195968) && !this.m_CameraHit.collider.isTrigger)
		{
			base.Transform.position = this.m_CameraHit.point - (this.m_CameraHit.point - this.m_CameraCollisionStartPos).normalized * this.FPController.CharacterController.radius;
			this.m_CollisionVector = this.m_CameraHit.point - this.m_CameraCollisionEndPos;
			return;
		}
		Camera playerCamera = GameManager.Instance.World.GetPrimaryPlayer().playerCamera;
		Vector3[] array = new Vector3[4];
		playerCamera.CalculateFrustumCorners(new Rect(0f, 0f, 1f, 1f), playerCamera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, array);
		this.cameraCollisionEndPosList.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 direction = array[i] * 1.5f;
			this.m_CameraCollisionEndPos = base.Transform.position + base.Transform.TransformDirection(direction);
			this.m_CollisionVector = Vector3.zero;
			this.cameraCollisionEndPosList.Add(this.m_CameraCollisionEndPos);
			if (Physics.Linecast(this.m_CameraCollisionStartPos, this.m_CameraCollisionEndPos, out this.m_CameraHit, 1082195968) && !this.m_CameraHit.collider.isTrigger)
			{
				base.Transform.position = this.m_CameraHit.point - (this.m_CameraHit.point - this.m_CameraCollisionStartPos).normalized * this.FPController.CharacterController.radius;
				this.m_CollisionVector = this.m_CameraHit.point - this.m_CameraCollisionEndPos;
				return;
			}
		}
	}

	// Token: 0x06009974 RID: 39284 RVA: 0x003D05A5 File Offset: 0x003CE7A5
	public virtual void AddForce(Vector3 force)
	{
		this.m_PositionSpring.AddForce(force);
	}

	// Token: 0x06009975 RID: 39285 RVA: 0x003D05B3 File Offset: 0x003CE7B3
	public virtual void AddForce(float x, float y, float z)
	{
		this.AddForce(new Vector3(x, y, z));
	}

	// Token: 0x06009976 RID: 39286 RVA: 0x003D05C3 File Offset: 0x003CE7C3
	public virtual void AddForce2(Vector3 force)
	{
		this.m_PositionSpring2.AddForce(force);
	}

	// Token: 0x06009977 RID: 39287 RVA: 0x003D05D1 File Offset: 0x003CE7D1
	public void AddForce2(float x, float y, float z)
	{
		this.AddForce2(new Vector3(x, y, z));
	}

	// Token: 0x06009978 RID: 39288 RVA: 0x003D05E1 File Offset: 0x003CE7E1
	public virtual void AddRollForce(float force)
	{
		this.m_RotationSpring.AddForce(Vector3.forward * force);
	}

	// Token: 0x06009979 RID: 39289 RVA: 0x003D05FC File Offset: 0x003CE7FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateInput()
	{
		if (this.Player.Dead.Active)
		{
			return;
		}
		if (this.Player.InputSmoothLook.Get() == Vector2.zero)
		{
			return;
		}
		this.m_Yaw += this.Player.InputSmoothLook.Get().x;
		this.m_Pitch += this.Player.InputSmoothLook.Get().y;
		this.m_Yaw = ((this.m_Yaw < -360f) ? (this.m_Yaw += 360f) : this.m_Yaw);
		this.m_Yaw = ((this.m_Yaw > 360f) ? (this.m_Yaw -= 360f) : this.m_Yaw);
		this.m_Yaw = Mathf.Clamp(this.m_Yaw, this.RotationYawLimit.x, this.RotationYawLimit.y);
		this.m_Pitch = ((this.m_Pitch < -360f) ? (this.m_Pitch += 360f) : this.m_Pitch);
		this.m_Pitch = ((this.m_Pitch > 360f) ? (this.m_Pitch -= 360f) : this.m_Pitch);
		this.m_Pitch = Mathf.Clamp(this.m_Pitch, -this.RotationPitchLimit.x, -this.RotationPitchLimit.y);
	}

	// Token: 0x0600997A RID: 39290 RVA: 0x003D079C File Offset: 0x003CE99C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateZoom()
	{
		if (this.m_FinalZoomTime <= Time.time)
		{
			return;
		}
		this.RenderingZoomDamping = Mathf.Max(this.RenderingZoomDamping, 0.01f);
		float t = 1f - (this.m_FinalZoomTime - Time.time) / this.RenderingZoomDamping;
		base.gameObject.GetComponent<Camera>().fieldOfView = Mathf.SmoothStep(base.gameObject.GetComponent<Camera>().fieldOfView, (float)Constants.cDefaultCameraFieldOfView + this.ZoomOffset, t);
	}

	// Token: 0x0600997B RID: 39291 RVA: 0x003D081C File Offset: 0x003CEA1C
	public void RefreshZoom()
	{
		float t = 1f - (this.m_FinalZoomTime - Time.time) / this.RenderingZoomDamping;
		base.gameObject.GetComponent<Camera>().fieldOfView = Mathf.SmoothStep(base.gameObject.GetComponent<Camera>().fieldOfView, (float)Constants.cDefaultCameraFieldOfView + this.ZoomOffset, t);
	}

	// Token: 0x0600997C RID: 39292 RVA: 0x003D0876 File Offset: 0x003CEA76
	public virtual void Zoom()
	{
		this.m_FinalZoomTime = Time.time + this.RenderingZoomDamping;
	}

	// Token: 0x0600997D RID: 39293 RVA: 0x003D088A File Offset: 0x003CEA8A
	public virtual void SnapZoom()
	{
		base.gameObject.GetComponent<Camera>().fieldOfView = (float)Constants.cDefaultCameraFieldOfView + this.ZoomOffset;
	}

	// Token: 0x0600997E RID: 39294 RVA: 0x003D08AC File Offset: 0x003CEAAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateShakes()
	{
		if (this.ShakeSpeed != 0f)
		{
			this.m_Yaw -= this.m_Shake.y;
			this.m_Pitch -= this.m_Shake.x;
			this.m_Shake = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(this.ShakeSpeed), this.ShakeAmplitude);
			if (float.IsNaN(this.m_Shake.x) || float.IsNaN(this.m_Shake.y) || float.IsNaN(this.m_Shake.z))
			{
				Log.Warning("Shake NaN {0}, time {1}, speed {2}, amp {3}", new object[]
				{
					this.m_Shake,
					Time.time,
					this.ShakeSpeed,
					this.ShakeAmplitude
				});
				this.ShakeSpeed = 0f;
				this.m_Shake = Vector3.zero;
				this.m_Pitch += -1f;
			}
			this.m_Yaw += this.m_Shake.y;
			this.m_Pitch += this.m_Shake.x;
			this.m_RotationSpring.AddForce(Vector3.forward * this.m_Shake.z * Time.timeScale);
		}
		if (this.ShakeSpeed2 != 0f)
		{
			this.m_Yaw -= this.m_Shake2.y;
			this.m_Pitch -= this.m_Shake2.x;
			this.m_Shake2 = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(this.ShakeSpeed2), this.ShakeAmplitude2);
			if (float.IsNaN(this.m_Shake2.x) || float.IsNaN(this.m_Shake2.y) || float.IsNaN(this.m_Shake2.z))
			{
				Log.Warning("Shake2 NaN {0}, time {1}, speed {2}, amp {3}", new object[]
				{
					this.m_Shake2,
					Time.time,
					this.ShakeSpeed2,
					this.ShakeAmplitude2
				});
				this.ShakeSpeed2 = 0f;
				this.m_Shake2 = Vector3.zero;
				this.m_Pitch += -1f;
			}
			this.m_Yaw += this.m_Shake2.y;
			this.m_Pitch += this.m_Shake2.x;
			this.m_RotationSpring.AddForce(Vector3.forward * this.m_Shake2.z * Time.timeScale);
		}
	}

	// Token: 0x0600997F RID: 39295 RVA: 0x003D0B70 File Offset: 0x003CED70
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateBob()
	{
		if (this.BobAmplitude == Vector4.zero || this.BobRate == Vector4.zero)
		{
			return;
		}
		if (!this.Player.IsFirstPerson.Get())
		{
			return;
		}
		this.m_BobSpeed = ((this.BobRequireGroundContact && !this.FPController.Grounded) ? 0f : this.FPController.CharacterController.velocity.sqrMagnitude);
		this.m_BobSpeed = Mathf.Min(this.m_BobSpeed * this.BobInputVelocityScale, this.BobMaxInputVelocity);
		this.m_BobSpeed = Mathf.Round(this.m_BobSpeed * 1000f) / 1000f;
		if (this.m_BobSpeed == 0f)
		{
			this.m_BobSpeed = Mathf.Min(this.m_LastBobSpeed * 0.93f, this.BobMaxInputVelocity);
		}
		this.m_CurrentBobAmp.y = this.m_BobSpeed * (this.BobAmplitude.y * -0.0001f);
		this.m_CurrentBobVal.y = Mathf.Cos(Time.time * (this.BobRate.y * 10f)) * this.m_CurrentBobAmp.y;
		this.m_CurrentBobAmp.x = this.m_BobSpeed * (this.BobAmplitude.x * 0.0001f);
		this.m_CurrentBobVal.x = Mathf.Cos(Time.time * (this.BobRate.x * 10f)) * this.m_CurrentBobAmp.x;
		this.m_CurrentBobAmp.z = this.m_BobSpeed * (this.BobAmplitude.z * 0.0001f);
		this.m_CurrentBobVal.z = Mathf.Cos(Time.time * (this.BobRate.z * 10f)) * this.m_CurrentBobAmp.z;
		this.m_CurrentBobAmp.w = this.m_BobSpeed * (this.BobAmplitude.w * 0.0001f);
		this.m_CurrentBobVal.w = Mathf.Cos(Time.time * (this.BobRate.w * 10f)) * this.m_CurrentBobAmp.w;
		this.m_PositionSpring.AddForce(this.m_CurrentBobVal * Time.timeScale);
		this.AddRollForce(this.m_CurrentBobVal.w * Time.timeScale);
		this.m_LastBobSpeed = this.m_BobSpeed;
		this.DetectBobStep(this.m_BobSpeed, this.m_CurrentBobVal.y);
	}

	// Token: 0x06009980 RID: 39296 RVA: 0x003D0E0C File Offset: 0x003CF00C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void DetectBobStep(float speed, float upBob)
	{
		if (this.BobStepCallback == null)
		{
			return;
		}
		if (speed < this.BobStepThreshold)
		{
			return;
		}
		bool flag = this.m_LastUpBob < upBob;
		this.m_LastUpBob = upBob;
		if (flag && !this.m_BobWasElevating)
		{
			this.BobStepCallback();
		}
		this.m_BobWasElevating = flag;
	}

	// Token: 0x06009981 RID: 39297 RVA: 0x003D0E60 File Offset: 0x003CF060
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateSwaying()
	{
		Vector3 vector = base.Transform.InverseTransformDirection(this.FPController.CharacterController.velocity * 0.016f) * Time.timeScale;
		this.AddRollForce(vector.x * this.RotationStrafeRoll);
	}

	// Token: 0x06009982 RID: 39298 RVA: 0x003D0EB0 File Offset: 0x003CF0B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateEarthQuake()
	{
		if (this.Player == null)
		{
			return;
		}
		if (!this.Player.CameraEarthQuake.Active)
		{
			return;
		}
		if (this.m_PositionSpring.State.y >= this.m_PositionSpring.RestState.y)
		{
			Vector3 vector = this.Player.CameraEarthQuakeForce.Get();
			vector.y = -vector.y;
			this.Player.CameraEarthQuakeForce.Set(vector);
		}
		this.m_PositionSpring.AddForce(this.Player.CameraEarthQuakeForce.Get() * this.PositionEarthQuakeFactor);
		this.m_RotationSpring.AddForce(Vector3.forward * (-this.Player.CameraEarthQuakeForce.Get().x * 2f) * this.RotationEarthQuakeFactor);
	}

	// Token: 0x06009983 RID: 39299 RVA: 0x003D0FA7 File Offset: 0x003CF1A7
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateSprings()
	{
		this.m_PositionSpring.FixedUpdate();
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring.FixedUpdate();
	}

	// Token: 0x06009984 RID: 39300 RVA: 0x003D0FCC File Offset: 0x003CF1CC
	public virtual void DoBomb(Vector3 positionForce, float minRollForce, float maxRollForce)
	{
		this.AddForce2(positionForce);
		float num = UnityEngine.Random.Range(minRollForce, maxRollForce);
		if (UnityEngine.Random.value > 0.5f)
		{
			num = -num;
		}
		this.AddRollForce(num);
	}

	// Token: 0x06009985 RID: 39301 RVA: 0x003D1000 File Offset: 0x003CF200
	public override void Refresh()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stiffness = new Vector3(this.PositionSpringStiffness, this.PositionSpringStiffness, this.PositionSpringStiffness);
			this.m_PositionSpring.Damping = Vector3.one - new Vector3(this.PositionSpringDamping, this.PositionSpringDamping, this.PositionSpringDamping);
			this.m_PositionSpring.MinState.y = this.PositionGroundLimit;
			this.m_PositionSpring.RestState = this.PositionOffset + this.AimingPositionOffset;
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stiffness = new Vector3(this.PositionSpring2Stiffness, this.PositionSpring2Stiffness, this.PositionSpring2Stiffness);
			this.m_PositionSpring2.Damping = Vector3.one - new Vector3(this.PositionSpring2Damping, this.PositionSpring2Damping, this.PositionSpring2Damping);
			this.m_PositionSpring2.MinState.y = -this.PositionOffset.y - this.AimingPositionOffset.y + this.PositionGroundLimit;
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stiffness = new Vector3(this.RotationSpringStiffness, this.RotationSpringStiffness, this.RotationSpringStiffness);
			this.m_RotationSpring.Damping = Vector3.one - new Vector3(this.RotationSpringDamping, this.RotationSpringDamping, this.RotationSpringDamping);
		}
	}

	// Token: 0x06009986 RID: 39302 RVA: 0x003D117C File Offset: 0x003CF37C
	public virtual void SnapSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.RestState = this.PositionOffset + this.AimingPositionOffset;
			this.m_PositionSpring.State = this.PositionOffset + this.AimingPositionOffset;
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.RestState = Vector3.zero;
			this.m_PositionSpring2.State = Vector3.zero;
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.RestState = Vector3.zero;
			this.m_RotationSpring.State = Vector3.zero;
			this.m_RotationSpring.Stop(true);
		}
	}

	// Token: 0x06009987 RID: 39303 RVA: 0x003D1240 File Offset: 0x003CF440
	public virtual void StopSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stop(true);
		}
		this.m_BobSpeed = 0f;
		this.m_LastBobSpeed = 0f;
	}

	// Token: 0x06009988 RID: 39304 RVA: 0x003D129F File Offset: 0x003CF49F
	public virtual void Stop()
	{
		this.SnapSprings();
		this.SnapZoom();
		this.Refresh();
	}

	// Token: 0x06009989 RID: 39305 RVA: 0x003D12B3 File Offset: 0x003CF4B3
	public virtual void SetRotation(Vector2 eulerAngles, bool stopZoomAndSprings)
	{
		this.Angle = eulerAngles;
		if (stopZoomAndSprings)
		{
			this.Stop();
		}
	}

	// Token: 0x0600998A RID: 39306 RVA: 0x003D12C5 File Offset: 0x003CF4C5
	public virtual void SetRotation(Vector2 eulerAngles)
	{
		this.Angle = eulerAngles;
		this.Stop();
	}

	// Token: 0x0600998B RID: 39307 RVA: 0x003D12D4 File Offset: 0x003CF4D4
	public virtual void SetRotation(Vector2 eulerAngles, bool stopZoomAndSprings, bool obsolete)
	{
		this.SetRotation(eulerAngles, stopZoomAndSprings);
	}

	// Token: 0x0600998C RID: 39308 RVA: 0x003D12E0 File Offset: 0x003CF4E0
	public Vector3 GetLookPoint()
	{
		if (!this.Player.IsFirstPerson.Get() && Physics.Linecast(base.Transform.position, base.Transform.position + base.Transform.forward * 1000f, out this.m_LookPointHit, 1082195968) && !this.m_LookPointHit.collider.isTrigger && base.Root.InverseTransformPoint(this.m_LookPointHit.point).z > 0f)
		{
			return this.m_LookPointHit.point;
		}
		return base.Transform.position + base.Transform.forward * 1000f;
	}

	// Token: 0x17000FB7 RID: 4023
	// (get) Token: 0x0600998D RID: 39309 RVA: 0x003D13AE File Offset: 0x003CF5AE
	public virtual Vector3 OnValue_LookPoint
	{
		get
		{
			return this.LookPoint;
		}
	}

	// Token: 0x17000FB8 RID: 4024
	// (get) Token: 0x0600998E RID: 39310 RVA: 0x003D13B8 File Offset: 0x003CF5B8
	public virtual Vector3 OnValue_CameraLookDirection
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return (this.Player.LookPoint.Get() - base.Transform.position).normalized;
		}
	}

	// Token: 0x0600998F RID: 39311 RVA: 0x003D13F4 File Offset: 0x003CF5F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnMessage_FallImpact(float impact)
	{
		impact = Mathf.Abs(impact * 55f);
		float num = impact * this.PositionKneeling;
		float num2 = impact * this.RotationKneeling;
		num = Mathf.SmoothStep(0f, 1f, num);
		num2 = Mathf.SmoothStep(0f, 1f, num2);
		num2 = Mathf.SmoothStep(0f, 1f, num2);
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.AddSoftForce(Vector3.down * num, (float)this.PositionKneelingSoftness);
		}
		if (this.m_RotationSpring != null)
		{
			float d = (UnityEngine.Random.value > 0.5f) ? (num2 * 2f) : (-(num2 * 2f));
			this.m_RotationSpring.AddSoftForce(Vector3.forward * d, (float)this.RotationKneelingSoftness);
		}
	}

	// Token: 0x06009990 RID: 39312 RVA: 0x003D14C0 File Offset: 0x003CF6C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnMessage_HeadImpact(float impact)
	{
		if (this.m_RotationSpring != null && Mathf.Abs(this.m_RotationSpring.State.z) < 30f)
		{
			this.m_RotationSpring.AddForce(Vector3.forward * (impact * 20f) * Time.timeScale);
		}
	}

	// Token: 0x06009991 RID: 39313 RVA: 0x003D1517 File Offset: 0x003CF717
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnMessage_CameraGroundStomp(float impact)
	{
		this.AddForce2(new Vector3(0f, -1f, 0f) * impact);
	}

	// Token: 0x06009992 RID: 39314 RVA: 0x003D1539 File Offset: 0x003CF739
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnMessage_CameraBombShake(float impact)
	{
		this.DoBomb(new Vector3(1f, -10f, 1f) * impact, 1f, 2f);
	}

	// Token: 0x06009993 RID: 39315 RVA: 0x003D1565 File Offset: 0x003CF765
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStart_Zoom()
	{
		if (this.Player == null)
		{
			return;
		}
		this.Player.Run.Stop(0f);
	}

	// Token: 0x06009994 RID: 39316 RVA: 0x003D158B File Offset: 0x003CF78B
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CanStart_Run()
	{
		return this.Player == null || !this.Player.Zoom.Active;
	}

	// Token: 0x17000FB9 RID: 4025
	// (get) Token: 0x06009995 RID: 39317 RVA: 0x003D15B2 File Offset: 0x003CF7B2
	// (set) Token: 0x06009996 RID: 39318 RVA: 0x003D15BA File Offset: 0x003CF7BA
	public virtual Vector2 OnValue_Rotation
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.Angle;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.Angle = value;
		}
	}

	// Token: 0x06009997 RID: 39319 RVA: 0x003D15C3 File Offset: 0x003CF7C3
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnMessage_Stop()
	{
		this.Stop();
	}

	// Token: 0x06009998 RID: 39320 RVA: 0x003D15CB File Offset: 0x003CF7CB
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStart_Dead()
	{
		if (this.Player.IsFirstPerson.Get())
		{
			return;
		}
		this.PositionOnDeath = base.Transform.position - this.m_Final3rdPersonCameraOffset;
	}

	// Token: 0x06009999 RID: 39321 RVA: 0x003D1601 File Offset: 0x003CF801
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStop_Dead()
	{
		this.PositionOnDeath = Vector3.zero;
		this.m_Current3rdPersonBlend = 0f;
	}

	// Token: 0x17000FBA RID: 4026
	// (get) Token: 0x0600999A RID: 39322 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool OnValue_IsLocal
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return true;
		}
	}

	// Token: 0x0600999B RID: 39323 RVA: 0x003D1619 File Offset: 0x003CF819
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnMessage_CameraToggle3rdPerson()
	{
		this.m_Player.IsFirstPerson.Set(!this.m_Player.IsFirstPerson.Get());
	}

	// Token: 0x040075E6 RID: 30182
	public Vector3 DrivingPosition;

	// Token: 0x040075E7 RID: 30183
	public vp_FPController FPController;

	// Token: 0x040075E8 RID: 30184
	public float RenderingFieldOfView = 60f;

	// Token: 0x040075E9 RID: 30185
	public float RenderingZoomDamping = 0.2f;

	// Token: 0x040075EA RID: 30186
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_FinalZoomTime;

	// Token: 0x040075EB RID: 30187
	public float ZoomOffset;

	// Token: 0x040075EC RID: 30188
	public Vector3 PositionOffset = new Vector3(0f, 1.75f, 0.1f);

	// Token: 0x040075ED RID: 30189
	public Vector3 AimingPositionOffset = Vector3.zero;

	// Token: 0x040075EE RID: 30190
	public float PositionGroundLimit = 0.1f;

	// Token: 0x040075EF RID: 30191
	public float PositionSpringStiffness = 0.01f;

	// Token: 0x040075F0 RID: 30192
	public float PositionSpringDamping = 0.25f;

	// Token: 0x040075F1 RID: 30193
	public float PositionSpring2Stiffness = 0.95f;

	// Token: 0x040075F2 RID: 30194
	public float PositionSpring2Damping = 0.25f;

	// Token: 0x040075F3 RID: 30195
	public float PositionKneeling = 0.025f;

	// Token: 0x040075F4 RID: 30196
	public int PositionKneelingSoftness = 1;

	// Token: 0x040075F5 RID: 30197
	public float PositionEarthQuakeFactor = 1f;

	// Token: 0x040075F6 RID: 30198
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Spring m_PositionSpring;

	// Token: 0x040075F7 RID: 30199
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Spring m_PositionSpring2;

	// Token: 0x040075F8 RID: 30200
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_DrawCameraCollisionDebugLine;

	// Token: 0x040075F9 RID: 30201
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 PositionOnDeath = Vector3.zero;

	// Token: 0x040075FA RID: 30202
	public Vector2 RotationPitchLimit = new Vector2(90f, -90f);

	// Token: 0x040075FB RID: 30203
	public Vector2 RotationYawLimit = new Vector2(-360f, 360f);

	// Token: 0x040075FC RID: 30204
	public float RotationSpringStiffness = 0.01f;

	// Token: 0x040075FD RID: 30205
	public float RotationSpringDamping = 0.25f;

	// Token: 0x040075FE RID: 30206
	public float RotationKneeling = 0.025f;

	// Token: 0x040075FF RID: 30207
	public int RotationKneelingSoftness = 1;

	// Token: 0x04007600 RID: 30208
	public float RotationStrafeRoll = 0.01f;

	// Token: 0x04007601 RID: 30209
	public float RotationEarthQuakeFactor;

	// Token: 0x04007602 RID: 30210
	public Vector3 LookPoint = Vector3.zero;

	// Token: 0x04007603 RID: 30211
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_Pitch;

	// Token: 0x04007604 RID: 30212
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_Yaw;

	// Token: 0x04007605 RID: 30213
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Spring m_RotationSpring;

	// Token: 0x04007606 RID: 30214
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public RaycastHit m_LookPointHit;

	// Token: 0x04007607 RID: 30215
	public Vector3 Position3rdPersonOffset = new Vector3(0.5f, 0.1f, 0.75f);

	// Token: 0x04007608 RID: 30216
	public bool Locked3rdPerson;

	// Token: 0x04007609 RID: 30217
	public float m_Current3rdPersonBlend;

	// Token: 0x0400760A RID: 30218
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_Final3rdPersonCameraOffset = Vector3.zero;

	// Token: 0x0400760B RID: 30219
	public float ShakeSpeed;

	// Token: 0x0400760C RID: 30220
	public Vector3 ShakeAmplitude = new Vector3(10f, 10f, 0f);

	// Token: 0x0400760D RID: 30221
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_Shake = Vector3.zero;

	// Token: 0x0400760E RID: 30222
	public float ShakeSpeed2;

	// Token: 0x0400760F RID: 30223
	public Vector3 ShakeAmplitude2 = new Vector3(10f, 10f, 0f);

	// Token: 0x04007610 RID: 30224
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_Shake2 = Vector3.zero;

	// Token: 0x04007611 RID: 30225
	public Vector4 BobRate = new Vector4(0f, 1.4f, 0f, 0.7f);

	// Token: 0x04007612 RID: 30226
	public Vector4 BobAmplitude = new Vector4(0f, 0.25f, 0f, 0.5f);

	// Token: 0x04007613 RID: 30227
	public float BobInputVelocityScale = 1f;

	// Token: 0x04007614 RID: 30228
	public float BobMaxInputVelocity = 100f;

	// Token: 0x04007615 RID: 30229
	public bool BobRequireGroundContact = true;

	// Token: 0x04007616 RID: 30230
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_LastBobSpeed;

	// Token: 0x04007617 RID: 30231
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector4 m_CurrentBobAmp = Vector4.zero;

	// Token: 0x04007618 RID: 30232
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector4 m_CurrentBobVal = Vector4.zero;

	// Token: 0x04007619 RID: 30233
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_BobSpeed;

	// Token: 0x0400761A RID: 30234
	public vp_FPCamera.BobStepDelegate BobStepCallback;

	// Token: 0x0400761B RID: 30235
	public float BobStepThreshold = 10f;

	// Token: 0x0400761C RID: 30236
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_LastUpBob;

	// Token: 0x0400761D RID: 30237
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_BobWasElevating;

	// Token: 0x0400761E RID: 30238
	public bool HasCollision = true;

	// Token: 0x0400761F RID: 30239
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_CollisionVector = Vector3.zero;

	// Token: 0x04007620 RID: 30240
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_CameraCollisionStartPos = Vector3.zero;

	// Token: 0x04007621 RID: 30241
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_CameraCollisionEndPos = Vector3.zero;

	// Token: 0x04007622 RID: 30242
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public RaycastHit m_CameraHit;

	// Token: 0x04007623 RID: 30243
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3> cameraCollisionEndPosList = new List<Vector3>();

	// Token: 0x04007625 RID: 30245
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_FPPlayerEventHandler m_Player;

	// Token: 0x04007626 RID: 30246
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Rigidbody m_FirstRigidbody;

	// Token: 0x04007627 RID: 30247
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_IsFirstPerson = true;

	// Token: 0x04007628 RID: 30248
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasLateUpdateRan;

	// Token: 0x04007629 RID: 30249
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Vector3 lastPos;

	// Token: 0x0200134A RID: 4938
	// (Invoke) Token: 0x0600999E RID: 39326
	public delegate void BobStepDelegate();
}
