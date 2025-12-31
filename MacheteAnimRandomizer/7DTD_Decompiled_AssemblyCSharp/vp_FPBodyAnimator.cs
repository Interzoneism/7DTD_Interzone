using System;
using System.Collections;
using UnityEngine;

// Token: 0x02001347 RID: 4935
public class vp_FPBodyAnimator : vp_BodyAnimator
{
	// Token: 0x17000FA5 RID: 4005
	// (get) Token: 0x06009939 RID: 39225 RVA: 0x003CEF4A File Offset: 0x003CD14A
	public vp_FPCamera FPCamera
	{
		get
		{
			if (this.m_FPCamera == null)
			{
				this.m_FPCamera = base.transform.root.GetComponentInChildren<vp_FPCamera>();
			}
			return this.m_FPCamera;
		}
	}

	// Token: 0x17000FA6 RID: 4006
	// (get) Token: 0x0600993A RID: 39226 RVA: 0x003CEF76 File Offset: 0x003CD176
	public vp_FPController FPController
	{
		get
		{
			if (this.m_FPController == null)
			{
				this.m_FPController = base.transform.root.GetComponent<vp_FPController>();
			}
			return this.m_FPController;
		}
	}

	// Token: 0x17000FA7 RID: 4007
	// (get) Token: 0x0600993B RID: 39227 RVA: 0x003CEFA4 File Offset: 0x003CD1A4
	public vp_FPWeaponShooter CurrentShooter
	{
		get
		{
			if ((this.m_CurrentShooter == null || (this.m_CurrentShooter != null && (!this.m_CurrentShooter.enabled || !vp_Utility.IsActive(this.m_CurrentShooter.gameObject)))) && base.WeaponHandler != null && base.WeaponHandler.CurrentWeapon != null)
			{
				this.m_CurrentShooter = base.WeaponHandler.CurrentWeapon.GetComponentInChildren<vp_FPWeaponShooter>();
			}
			return this.m_CurrentShooter;
		}
	}

	// Token: 0x17000FA8 RID: 4008
	// (get) Token: 0x0600993C RID: 39228 RVA: 0x003CF02C File Offset: 0x003CD22C
	public float DefaultCamHeight
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_DefaultCamHeight == 0f)
			{
				if (this.FPCamera != null && this.FPCamera.DefaultState != null && this.FPCamera.DefaultState.Preset != null)
				{
					this.m_DefaultCamHeight = ((Vector3)this.FPCamera.DefaultState.Preset.GetFieldValue("PositionOffset")).y;
				}
				else
				{
					this.m_DefaultCamHeight = 1.75f;
				}
			}
			return this.m_DefaultCamHeight;
		}
	}

	// Token: 0x0600993D RID: 39229 RVA: 0x003CF0B0 File Offset: 0x003CD2B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.InitMaterials();
		this.m_WasFirstPersonLastFrame = base.Player.IsFirstPerson.Get();
		this.FPCamera.HasCollision = true;
		base.Player.IsFirstPerson.Set(true);
	}

	// Token: 0x0600993E RID: 39230 RVA: 0x003CF106 File Offset: 0x003CD306
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEnable()
	{
		base.OnEnable();
		this.RefreshMaterials();
	}

	// Token: 0x0600993F RID: 39231 RVA: 0x003CF114 File Offset: 0x003CD314
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06009940 RID: 39232 RVA: 0x003CF11C File Offset: 0x003CD31C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (Time.timeScale == 0f)
		{
			return;
		}
		if (base.Player.IsFirstPerson.Get())
		{
			this.UpdatePosition();
			this.UpdateCameraPosition();
			this.UpdateCameraCollision();
		}
		else
		{
			this.FPCamera.DoCameraCollision();
		}
		this.UpdateFirePosition();
	}

	// Token: 0x06009941 RID: 39233 RVA: 0x003CF178 File Offset: 0x003CD378
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator RefreshMaterialsOnEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		this.RefreshMaterials();
		yield break;
	}

	// Token: 0x06009942 RID: 39234 RVA: 0x003CF188 File Offset: 0x003CD388
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void RefreshMaterials()
	{
		if (this.InvisibleMaterial == null)
		{
			Debug.LogWarning("Warning (" + ((this != null) ? this.ToString() : null) + ") No invisible material has been set. Head and arms will look buggy in first person.");
			return;
		}
		if (!base.Player.IsFirstPerson.Get())
		{
			if (this.m_ThirdPersonMaterials != null)
			{
				base.Renderer.materials = this.m_ThirdPersonMaterials;
				return;
			}
		}
		else if (!base.Player.Dead.Active)
		{
			if (this.ShowUnarmedArms && base.Player.CurrentWeaponIndex.Get() < 1 && !base.Player.Climb.Active)
			{
				if (this.m_FirstPersonWithArmsMaterials != null)
				{
					base.Renderer.materials = this.m_FirstPersonWithArmsMaterials;
					return;
				}
			}
			else if (this.m_FirstPersonMaterials != null)
			{
				base.Renderer.materials = this.m_FirstPersonMaterials;
				return;
			}
		}
		else if (this.m_InvisiblePersonMaterials != null)
		{
			base.Renderer.materials = this.m_InvisiblePersonMaterials;
		}
	}

	// Token: 0x06009943 RID: 39235 RVA: 0x003CF28C File Offset: 0x003CD48C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdatePosition()
	{
		base.Transform.position = this.FPController.SmoothPosition + this.FPController.SkinWidth * Vector3.down;
		if (base.Player.IsFirstPerson.Get() && !base.Player.Climb.Active)
		{
			if (this.m_HeadLookBones != null && this.m_HeadLookBones.Count > 0)
			{
				base.Transform.position = Vector3.Lerp(base.Transform.position, base.Transform.position + (this.FPCamera.Transform.position - this.m_HeadLookBones[0].transform.position), Mathf.Lerp(1f, 0f, Mathf.Max(0f, base.Player.Rotation.Get().x / 60f)));
			}
			else
			{
				Debug.LogWarning("Warning (" + ((this != null) ? this.ToString() : null) + ") No headlookbones have been assigned!");
			}
		}
		else
		{
			base.Transform.localPosition = Vector3.Scale(base.Transform.localPosition, Vector3.right + Vector3.up);
		}
		if (base.Player.Climb.Active)
		{
			base.Transform.localPosition += this.ClimbOffset;
		}
	}

	// Token: 0x06009944 RID: 39236 RVA: 0x003CF420 File Offset: 0x003CD620
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateCameraPosition()
	{
		this.FPCamera.transform.position = this.m_HeadLookBones[0].transform.position;
		float num = Mathf.Max(0f, (base.Player.Rotation.Get().x - 45f) / 45f);
		num = Mathf.SmoothStep(0f, 1f, num);
		this.FPCamera.transform.localPosition = new Vector3(this.FPCamera.transform.localPosition.x, this.FPCamera.transform.localPosition.y, this.FPCamera.transform.localPosition.z + num * (base.Player.Crouch.Active ? 0f : this.LookDownForwardOffset));
		this.FPCamera.Transform.localPosition -= this.EyeOffset;
		this.FPCamera.ZoomOffset = -this.LookDownZoomFactor * num;
		this.FPCamera.RefreshZoom();
	}

	// Token: 0x06009945 RID: 39237 RVA: 0x003CF54C File Offset: 0x003CD74C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateCameraCollision()
	{
		this.FPCamera.DoCameraCollision();
		if (this.FPCamera.CollisionVector != Vector3.zero)
		{
			base.Transform.position += this.FPCamera.CollisionVector;
		}
	}

	// Token: 0x06009946 RID: 39238 RVA: 0x003CF59C File Offset: 0x003CD79C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateGrounding()
	{
		this.m_Grounded = this.FPController.Grounded;
	}

	// Token: 0x06009947 RID: 39239 RVA: 0x003CF5B0 File Offset: 0x003CD7B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateDebugInfo()
	{
		if (this.ShowDebugObjects)
		{
			base.DebugLookTarget.transform.position = this.FPCamera.LookPoint;
			base.DebugLookArrow.transform.LookAt(base.DebugLookTarget.transform.position);
			if (!vp_Utility.IsActive(this.m_DebugLookTarget))
			{
				vp_Utility.Activate(this.m_DebugLookTarget, true);
			}
			if (!vp_Utility.IsActive(this.m_DebugLookArrow))
			{
				vp_Utility.Activate(this.m_DebugLookArrow, true);
				return;
			}
		}
		else
		{
			if (this.m_DebugLookTarget != null)
			{
				vp_Utility.Activate(this.m_DebugLookTarget, false);
			}
			if (this.m_DebugLookArrow != null)
			{
				vp_Utility.Activate(this.m_DebugLookArrow, false);
			}
		}
	}

	// Token: 0x06009948 RID: 39240 RVA: 0x003CF668 File Offset: 0x003CD868
	[PublicizedFrom(EAccessModifier.Protected)]
	public void UpdateFirePosition()
	{
		if (this.CurrentShooter == null)
		{
			return;
		}
		if (this.CurrentShooter.ProjectileSpawnPoint == null)
		{
			return;
		}
		this.CurrentShooter.FirePosition = this.CurrentShooter.ProjectileSpawnPoint.transform.position;
	}

	// Token: 0x06009949 RID: 39241 RVA: 0x003CF6B8 File Offset: 0x003CD8B8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void InitMaterials()
	{
		if (this.InvisibleMaterial == null)
		{
			Debug.LogWarning("Warning () No invisible material has been set.");
			return;
		}
		this.m_FirstPersonMaterials = new Material[base.Renderer.materials.Length];
		this.m_FirstPersonWithArmsMaterials = new Material[base.Renderer.materials.Length];
		this.m_ThirdPersonMaterials = new Material[base.Renderer.materials.Length];
		this.m_InvisiblePersonMaterials = new Material[base.Renderer.materials.Length];
		for (int i = 0; i < base.Renderer.materials.Length; i++)
		{
			this.m_ThirdPersonMaterials[i] = base.Renderer.materials[i];
			if (base.Renderer.materials[i].name.ToLower().Contains("head") || base.Renderer.materials[i].name.ToLower().Contains("arm"))
			{
				this.m_FirstPersonMaterials[i] = this.InvisibleMaterial;
			}
			else
			{
				this.m_FirstPersonMaterials[i] = base.Renderer.materials[i];
			}
			if (base.Renderer.materials[i].name.ToLower().Contains("head"))
			{
				this.m_FirstPersonWithArmsMaterials[i] = this.InvisibleMaterial;
			}
			else
			{
				this.m_FirstPersonWithArmsMaterials[i] = base.Renderer.materials[i];
			}
			this.m_InvisiblePersonMaterials[i] = this.InvisibleMaterial;
		}
		this.RefreshMaterials();
	}

	// Token: 0x0600994A RID: 39242 RVA: 0x003CF838 File Offset: 0x003CDA38
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetIsMoving()
	{
		return Vector3.Scale(base.Player.MotorThrottle.Get(), Vector3.right + Vector3.forward).magnitude > 0.01f;
	}

	// Token: 0x0600994B RID: 39243 RVA: 0x003CF87D File Offset: 0x003CDA7D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override Vector3 GetLookPoint()
	{
		return this.FPCamera.LookPoint;
	}

	// Token: 0x17000FA9 RID: 4009
	// (get) Token: 0x0600994C RID: 39244 RVA: 0x003CF88A File Offset: 0x003CDA8A
	public override Vector3 OnValue_LookPoint
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.GetLookPoint();
		}
	}

	// Token: 0x0600994D RID: 39245 RVA: 0x003CF892 File Offset: 0x003CDA92
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnMessage_CameraToggle3rdPerson()
	{
		base.OnMessage_CameraToggle3rdPerson();
		base.StartCoroutine(this.RefreshMaterialsOnEndOfFrame());
	}

	// Token: 0x0600994E RID: 39246 RVA: 0x003CF8A7 File Offset: 0x003CDAA7
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStop_SetWeapon()
	{
		this.RefreshMaterials();
	}

	// Token: 0x0600994F RID: 39247 RVA: 0x003CF8AF File Offset: 0x003CDAAF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnStart_Climb()
	{
		base.OnStart_Climb();
		this.RefreshMaterials();
	}

	// Token: 0x06009950 RID: 39248 RVA: 0x003CF8A7 File Offset: 0x003CDAA7
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStop_Climb()
	{
		this.RefreshMaterials();
	}

	// Token: 0x06009951 RID: 39249 RVA: 0x003CF8BD File Offset: 0x003CDABD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnStart_Dead()
	{
		base.OnStart_Dead();
		this.RefreshMaterials();
	}

	// Token: 0x040075D5 RID: 30165
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPController m_FPController;

	// Token: 0x040075D6 RID: 30166
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPCamera m_FPCamera;

	// Token: 0x040075D7 RID: 30167
	public Vector3 EyeOffset = new Vector3(0f, -0.08f, -0.1f);

	// Token: 0x040075D8 RID: 30168
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_WasFirstPersonLastFrame;

	// Token: 0x040075D9 RID: 30169
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_DefaultCamHeight;

	// Token: 0x040075DA RID: 30170
	public float LookDownZoomFactor = 15f;

	// Token: 0x040075DB RID: 30171
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float LookDownForwardOffset = 0.05f;

	// Token: 0x040075DC RID: 30172
	public bool ShowUnarmedArms = true;

	// Token: 0x040075DD RID: 30173
	public Material InvisibleMaterial;

	// Token: 0x040075DE RID: 30174
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Material[] m_FirstPersonMaterials;

	// Token: 0x040075DF RID: 30175
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Material[] m_FirstPersonWithArmsMaterials;

	// Token: 0x040075E0 RID: 30176
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Material[] m_ThirdPersonMaterials;

	// Token: 0x040075E1 RID: 30177
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Material[] m_InvisiblePersonMaterials;

	// Token: 0x040075E2 RID: 30178
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_FPWeaponShooter m_CurrentShooter;
}
