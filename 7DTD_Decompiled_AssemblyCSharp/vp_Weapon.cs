using System;
using UnityEngine;

// Token: 0x02001366 RID: 4966
public class vp_Weapon : vp_Component
{
	// Token: 0x17001028 RID: 4136
	// (get) Token: 0x06009B73 RID: 39795 RVA: 0x003DD83E File Offset: 0x003DBA3E
	// (set) Token: 0x06009B74 RID: 39796 RVA: 0x003DD850 File Offset: 0x003DBA50
	public bool Wielded
	{
		get
		{
			return this.m_Wielded && base.Rendering;
		}
		set
		{
			this.m_Wielded = value;
		}
	}

	// Token: 0x17001029 RID: 4137
	// (get) Token: 0x06009B75 RID: 39797 RVA: 0x003DD859 File Offset: 0x003DBA59
	public vp_PlayerEventHandler Player
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_Player == null && base.EventHandler != null)
			{
				this.m_Player = (vp_PlayerEventHandler)base.EventHandler;
			}
			return this.m_Player;
		}
	}

	// Token: 0x06009B76 RID: 39798 RVA: 0x003DD890 File Offset: 0x003DBA90
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.RotationOffset = base.transform.localEulerAngles;
		this.PositionOffset = base.transform.position;
		base.Transform.localEulerAngles = this.RotationOffset;
		if (base.transform.parent == null)
		{
			Debug.LogError("Error (" + ((this != null) ? this.ToString() : null) + ") Must not be placed in scene root. Disabling self.");
			vp_Utility.Activate(base.gameObject, false);
			return;
		}
		if (base.GetComponent<Collider>() != null)
		{
			base.GetComponent<Collider>().enabled = false;
		}
	}

	// Token: 0x06009B77 RID: 39799 RVA: 0x003DD931 File Offset: 0x003DBB31
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEnable()
	{
		this.RefreshWeaponModel();
		base.OnEnable();
	}

	// Token: 0x06009B78 RID: 39800 RVA: 0x003DD93F File Offset: 0x003DBB3F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnDisable()
	{
		this.RefreshWeaponModel();
		this.Activate3rdPersonModel(false);
		base.OnDisable();
	}

	// Token: 0x1700102A RID: 4138
	// (get) Token: 0x06009B79 RID: 39801 RVA: 0x003DD954 File Offset: 0x003DBB54
	// (set) Token: 0x06009B7A RID: 39802 RVA: 0x003DD95C File Offset: 0x003DBB5C
	public Vector3 RotationSpring2DefaultRotation
	{
		get
		{
			return this.m_RotationSpring2DefaultRotation;
		}
		set
		{
			this.m_RotationSpring2DefaultRotation = value;
		}
	}

	// Token: 0x06009B7B RID: 39803 RVA: 0x003DD968 File Offset: 0x003DBB68
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		this.m_PositionSpring2 = new vp_Spring(base.transform, vp_Spring.UpdateMode.PositionAdditiveSelf, true);
		this.m_PositionSpring2.MinVelocity = 1E-05f;
		this.m_RotationSpring2 = new vp_Spring(base.transform, vp_Spring.UpdateMode.RotationAdditiveGlobal, true);
		this.m_RotationSpring2.MinVelocity = 1E-05f;
		this.SnapSprings();
		this.Refresh();
		base.CacheRenderers();
	}

	// Token: 0x1700102B RID: 4139
	// (get) Token: 0x06009B7C RID: 39804 RVA: 0x003DD9D3 File Offset: 0x003DBBD3
	public Vector3 Recoil
	{
		get
		{
			return this.m_RotationSpring2.State;
		}
	}

	// Token: 0x06009B7D RID: 39805 RVA: 0x003DD9E0 File Offset: 0x003DBBE0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.UpdateSprings();
	}

	// Token: 0x06009B7E RID: 39806 RVA: 0x003DD9FB File Offset: 0x003DBBFB
	public virtual void AddForce2(Vector3 positional, Vector3 angular)
	{
		this.m_PositionSpring2.AddForce(positional);
		this.m_RotationSpring2.AddForce(angular);
	}

	// Token: 0x06009B7F RID: 39807 RVA: 0x003DDA15 File Offset: 0x003DBC15
	public virtual void AddForce2(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
	{
		this.AddForce2(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot));
	}

	// Token: 0x06009B80 RID: 39808 RVA: 0x003DDA30 File Offset: 0x003DBC30
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateSprings()
	{
		base.Transform.localPosition = Vector3.up;
		base.Transform.localRotation = Quaternion.identity;
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring2.FixedUpdate();
	}

	// Token: 0x06009B81 RID: 39809 RVA: 0x003DDA68 File Offset: 0x003DBC68
	public override void Refresh()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stiffness = new Vector3(this.PositionSpring2Stiffness, this.PositionSpring2Stiffness, this.PositionSpring2Stiffness);
			this.m_PositionSpring2.Damping = Vector3.one - new Vector3(this.PositionSpring2Damping, this.PositionSpring2Damping, this.PositionSpring2Damping);
			this.m_PositionSpring2.RestState = Vector3.zero;
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.Stiffness = new Vector3(this.RotationSpring2Stiffness, this.RotationSpring2Stiffness, this.RotationSpring2Stiffness);
			this.m_RotationSpring2.Damping = Vector3.one - new Vector3(this.RotationSpring2Damping, this.RotationSpring2Damping, this.RotationSpring2Damping);
			this.m_RotationSpring2.RestState = this.m_RotationSpring2DefaultRotation;
		}
	}

	// Token: 0x06009B82 RID: 39810 RVA: 0x003DDB4A File Offset: 0x003DBD4A
	public override void Activate()
	{
		base.Activate();
		this.m_Wielded = true;
		base.Rendering = true;
	}

	// Token: 0x06009B83 RID: 39811 RVA: 0x003DDB60 File Offset: 0x003DBD60
	public virtual void SnapSprings()
	{
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.RestState = Vector3.zero;
			this.m_PositionSpring2.State = Vector3.zero;
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.RestState = this.m_RotationSpring2DefaultRotation;
			this.m_RotationSpring2.State = this.m_RotationSpring2DefaultRotation;
			this.m_RotationSpring2.Stop(true);
		}
	}

	// Token: 0x06009B84 RID: 39812 RVA: 0x003DDBD7 File Offset: 0x003DBDD7
	public virtual void StopSprings()
	{
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.Stop(true);
		}
	}

	// Token: 0x06009B85 RID: 39813 RVA: 0x003DDC01 File Offset: 0x003DBE01
	public virtual void Wield(bool isWielding = true)
	{
		this.m_Wielded = isWielding;
		this.Refresh();
		base.StateManager.CombineStates();
	}

	// Token: 0x06009B86 RID: 39814 RVA: 0x003DDC1B File Offset: 0x003DBE1B
	public virtual void RefreshWeaponModel()
	{
		if (this.Player == null)
		{
			return;
		}
		vp_Value<bool> isFirstPerson = this.Player.IsFirstPerson;
	}

	// Token: 0x06009B87 RID: 39815 RVA: 0x003DDC38 File Offset: 0x003DBE38
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Activate3rdPersonModel(bool active = true)
	{
		if (this.Weapon3rdPersonModel == null)
		{
			return;
		}
		if (active)
		{
			this.Weapon3rdPersonModel.GetComponent<Renderer>().enabled = true;
			vp_Utility.Activate(this.Weapon3rdPersonModel, true);
			return;
		}
		this.Weapon3rdPersonModel.GetComponent<Renderer>().enabled = false;
		vp_Timer.In(0.1f, delegate()
		{
			if (this.Weapon3rdPersonModel != null)
			{
				vp_Utility.Activate(this.Weapon3rdPersonModel, false);
			}
		}, this.m_Weapon3rdPersonModelWakeUpTimer);
	}

	// Token: 0x06009B88 RID: 39816 RVA: 0x003DDCA2 File Offset: 0x003DBEA2
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStart_Dead()
	{
		if (this.Player.IsFirstPerson.Get())
		{
			return;
		}
		base.Rendering = false;
	}

	// Token: 0x06009B89 RID: 39817 RVA: 0x003DDCC3 File Offset: 0x003DBEC3
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStop_Dead()
	{
		if (this.Player.IsFirstPerson.Get())
		{
			return;
		}
		base.Rendering = true;
	}

	// Token: 0x1700102C RID: 4140
	// (get) Token: 0x06009B8A RID: 39818 RVA: 0x003DDCE4 File Offset: 0x003DBEE4
	public virtual Vector3 OnValue_AimDirection
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return (this.Weapon3rdPersonModel.transform.position - this.Player.LookPoint.Get()).normalized;
		}
	}

	// Token: 0x04007838 RID: 30776
	public GameObject Weapon3rdPersonModel;

	// Token: 0x04007839 RID: 30777
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public GameObject m_WeaponModel;

	// Token: 0x0400783A RID: 30778
	public Vector3 PositionOffset = new Vector3(0.15f, -0.15f, -0.15f);

	// Token: 0x0400783B RID: 30779
	public Vector3 AimingPositionOffset = new Vector3(0f, 0f, 0f);

	// Token: 0x0400783C RID: 30780
	public float PositionSpring2Stiffness = 0.95f;

	// Token: 0x0400783D RID: 30781
	public float PositionSpring2Damping = 0.25f;

	// Token: 0x0400783E RID: 30782
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Spring m_PositionSpring2;

	// Token: 0x0400783F RID: 30783
	public Vector3 RotationOffset = Vector3.zero;

	// Token: 0x04007840 RID: 30784
	public float RotationSpring2Stiffness = 0.95f;

	// Token: 0x04007841 RID: 30785
	public float RotationSpring2Damping = 0.25f;

	// Token: 0x04007842 RID: 30786
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Spring m_RotationSpring2;

	// Token: 0x04007843 RID: 30787
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_Wielded = true;

	// Token: 0x04007844 RID: 30788
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_Weapon3rdPersonModelWakeUpTimer = new vp_Timer.Handle();

	// Token: 0x04007845 RID: 30789
	public int AnimationType = 1;

	// Token: 0x04007846 RID: 30790
	public int AnimationGrip = 1;

	// Token: 0x04007847 RID: 30791
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_PlayerEventHandler m_Player;

	// Token: 0x04007848 RID: 30792
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_RotationSpring2DefaultRotation = Vector3.zero;

	// Token: 0x02001367 RID: 4967
	public new enum Type
	{
		// Token: 0x0400784A RID: 30794
		Custom,
		// Token: 0x0400784B RID: 30795
		Firearm,
		// Token: 0x0400784C RID: 30796
		Melee,
		// Token: 0x0400784D RID: 30797
		Thrown
	}

	// Token: 0x02001368 RID: 4968
	public enum Grip
	{
		// Token: 0x0400784F RID: 30799
		Custom,
		// Token: 0x04007850 RID: 30800
		OneHanded,
		// Token: 0x04007851 RID: 30801
		TwoHanded
	}
}
