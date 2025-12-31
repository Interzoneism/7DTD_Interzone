using System;
using UnityEngine;

// Token: 0x0200135C RID: 4956
public class vp_3rdPersonWeaponAim : MonoBehaviour
{
	// Token: 0x17000FFC RID: 4092
	// (get) Token: 0x06009AD7 RID: 39639 RVA: 0x003D9B78 File Offset: 0x003D7D78
	public Transform Transform
	{
		get
		{
			if (this.m_Transform == null)
			{
				this.m_Transform = base.transform;
			}
			return this.m_Transform;
		}
	}

	// Token: 0x17000FFD RID: 4093
	// (get) Token: 0x06009AD8 RID: 39640 RVA: 0x003D9B9A File Offset: 0x003D7D9A
	public vp_PlayerEventHandler Player
	{
		get
		{
			if (this.m_Player == null)
			{
				this.m_Player = (vp_PlayerEventHandler)this.Root.GetComponentInChildren(typeof(vp_PlayerEventHandler));
			}
			return this.m_Player;
		}
	}

	// Token: 0x17000FFE RID: 4094
	// (get) Token: 0x06009AD9 RID: 39641 RVA: 0x003D9BD0 File Offset: 0x003D7DD0
	public vp_WeaponHandler WeaponHandler
	{
		get
		{
			if (this.m_WeaponHandler == null)
			{
				this.m_WeaponHandler = (vp_WeaponHandler)this.Root.GetComponentInChildren(typeof(vp_WeaponHandler));
			}
			return this.m_WeaponHandler;
		}
	}

	// Token: 0x17000FFF RID: 4095
	// (get) Token: 0x06009ADA RID: 39642 RVA: 0x003D9C06 File Offset: 0x003D7E06
	public Animator Animator
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_Animator == null)
			{
				this.m_Animator = this.Root.GetComponentInChildren<Animator>();
			}
			return this.m_Animator;
		}
	}

	// Token: 0x17001000 RID: 4096
	// (get) Token: 0x06009ADB RID: 39643 RVA: 0x003D9C2D File Offset: 0x003D7E2D
	public Transform Root
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (this.m_Root == null)
			{
				this.m_Root = this.Transform.root;
			}
			return this.m_Root;
		}
	}

	// Token: 0x17001001 RID: 4097
	// (get) Token: 0x06009ADC RID: 39644 RVA: 0x003D9C54 File Offset: 0x003D7E54
	public Transform LowerArmObj
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (this.m_LowerArmObj == null)
			{
				this.m_LowerArmObj = this.HandObj.parent;
			}
			return this.m_LowerArmObj;
		}
	}

	// Token: 0x17001002 RID: 4098
	// (get) Token: 0x06009ADD RID: 39645 RVA: 0x003D9C7C File Offset: 0x003D7E7C
	public Transform HandObj
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (this.m_HandObj == null)
			{
				if (this.Hand != null)
				{
					this.m_HandObj = this.Hand.transform;
				}
				else
				{
					this.m_HandObj = vp_Utility.GetTransformByNameInAncestors(this.Transform, "hand", true, true);
					if (this.m_HandObj == null && this.Transform.parent != null)
					{
						this.m_HandObj = this.Transform.parent;
					}
					if (this.m_HandObj != null)
					{
						this.Hand = this.m_HandObj.gameObject;
					}
				}
			}
			return this.m_HandObj;
		}
	}

	// Token: 0x06009ADE RID: 39646 RVA: 0x003D9D2A File Offset: 0x003D7F2A
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.Player != null)
		{
			this.Player.Register(this);
		}
	}

	// Token: 0x06009ADF RID: 39647 RVA: 0x003D9D46 File Offset: 0x003D7F46
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		if (this.Player != null)
		{
			this.Player.Unregister(this);
		}
	}

	// Token: 0x06009AE0 RID: 39648 RVA: 0x003D9D64 File Offset: 0x003D7F64
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_DefaultRotation = this.Transform.localRotation;
		if (this.LowerArmObj == null || this.HandObj == null)
		{
			Debug.LogError("Hierarchy Error (" + ((this != null) ? this.ToString() : null) + ") This script should be placed on a 3rd person weapon gameobject childed to a hand bone in a rigged character.");
			base.enabled = false;
			return;
		}
		Quaternion lhs = Quaternion.Inverse(this.LowerArmObj.rotation);
		this.m_ReferenceLookDir = lhs * this.Root.rotation * Vector3.forward;
		this.m_ReferenceUpDir = lhs * this.Root.rotation * Vector3.up;
		Quaternion rotation = this.HandObj.rotation;
		this.HandObj.rotation = this.Root.rotation;
		Quaternion rotation2 = this.HandObj.rotation;
		this.HandObj.rotation = rotation;
		this.m_HandBoneRotDif = Quaternion.Inverse(rotation2) * rotation;
	}

	// Token: 0x06009AE1 RID: 39649 RVA: 0x003D9E66 File Offset: 0x003D8066
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void LateUpdate()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateAiming();
	}

	// Token: 0x06009AE2 RID: 39650 RVA: 0x003D9E7C File Offset: 0x003D807C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateAiming()
	{
		if (this.Animator == null)
		{
			return;
		}
		if ((!this.Animator.GetBool("IsAttacking") && !this.Animator.GetBool("IsZooming")) || this.Animator.GetBool("IsReloading") || this.Animator.GetBool("IsOutOfControl") || this.Player.CurrentWeaponIndex.Get() == 0)
		{
			this.Transform.localRotation = this.m_DefaultRotation;
			return;
		}
		Quaternion rotation = this.Transform.rotation;
		this.Transform.rotation = Quaternion.LookRotation(this.Player.AimDirection.Get());
		this.m_WorldDir = this.Transform.forward;
		this.Transform.rotation = rotation;
		this.HandObj.rotation = vp_3DUtility.GetBoneLookRotationInWorldSpace(this.HandObj.rotation, this.LowerArmObj.rotation, this.m_WorldDir, 1f, this.m_ReferenceUpDir, this.m_ReferenceLookDir, this.m_HandBoneRotDif);
		this.HandObj.Rotate(this.Transform.forward, this.AngleAdjustZ + this.WeaponHandler.CurrentWeapon.Recoil.z * this.RecoilFactorZ, Space.World);
		this.HandObj.Rotate(this.Transform.up, this.AngleAdjustY + this.WeaponHandler.CurrentWeapon.Recoil.y * this.RecoilFactorY, Space.World);
		this.HandObj.Rotate(this.Transform.right, this.AngleAdjustX + this.WeaponHandler.CurrentWeapon.Recoil.x * this.RecoilFactorX, Space.World);
	}

	// Token: 0x04007766 RID: 30566
	public GameObject Hand;

	// Token: 0x04007767 RID: 30567
	[Range(0f, 360f)]
	public float AngleAdjustX;

	// Token: 0x04007768 RID: 30568
	[Range(0f, 360f)]
	public float AngleAdjustY;

	// Token: 0x04007769 RID: 30569
	[Range(0f, 360f)]
	public float AngleAdjustZ;

	// Token: 0x0400776A RID: 30570
	[Range(0f, 5f)]
	public float RecoilFactorX = 1f;

	// Token: 0x0400776B RID: 30571
	[Range(0f, 5f)]
	public float RecoilFactorY = 1f;

	// Token: 0x0400776C RID: 30572
	[Range(0f, 5f)]
	public float RecoilFactorZ = 1f;

	// Token: 0x0400776D RID: 30573
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Quaternion m_DefaultRotation;

	// Token: 0x0400776E RID: 30574
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_ReferenceUpDir;

	// Token: 0x0400776F RID: 30575
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_ReferenceLookDir;

	// Token: 0x04007770 RID: 30576
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Quaternion m_HandBoneRotDif;

	// Token: 0x04007771 RID: 30577
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_WorldDir = Vector3.zero;

	// Token: 0x04007772 RID: 30578
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Transform;

	// Token: 0x04007773 RID: 30579
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_PlayerEventHandler m_Player;

	// Token: 0x04007774 RID: 30580
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_WeaponHandler m_WeaponHandler;

	// Token: 0x04007775 RID: 30581
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Animator m_Animator;

	// Token: 0x04007776 RID: 30582
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_Root;

	// Token: 0x04007777 RID: 30583
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_LowerArmObj;

	// Token: 0x04007778 RID: 30584
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_HandObj;
}
