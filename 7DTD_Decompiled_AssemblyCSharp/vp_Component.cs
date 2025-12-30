using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200128D RID: 4749
[Preserve]
public class vp_Component : MonoBehaviour
{
	// Token: 0x17000F36 RID: 3894
	// (get) Token: 0x06009475 RID: 38005 RVA: 0x003B36F8 File Offset: 0x003B18F8
	public vp_EventHandler EventHandler
	{
		get
		{
			if (this.m_EventHandler == null)
			{
				this.m_EventHandler = (vp_EventHandler)this.Transform.GetComponentInChildren(typeof(vp_EventHandler));
			}
			if (this.m_EventHandler == null && this.Transform.parent != null)
			{
				this.m_EventHandler = (vp_EventHandler)this.Transform.parent.GetComponentInChildren(typeof(vp_EventHandler));
			}
			if (this.m_EventHandler == null && this.Transform.parent != null && this.Transform.parent.parent != null)
			{
				this.m_EventHandler = (vp_EventHandler)this.Transform.parent.parent.GetComponentInChildren(typeof(vp_EventHandler));
			}
			return this.m_EventHandler;
		}
	}

	// Token: 0x17000F37 RID: 3895
	// (get) Token: 0x06009476 RID: 38006 RVA: 0x003B37E2 File Offset: 0x003B19E2
	public Type Type
	{
		get
		{
			if (this.m_Type == null)
			{
				this.m_Type = base.GetType();
			}
			return this.m_Type;
		}
	}

	// Token: 0x17000F38 RID: 3896
	// (get) Token: 0x06009477 RID: 38007 RVA: 0x003B3804 File Offset: 0x003B1A04
	public FieldInfo[] Fields
	{
		get
		{
			if (this.m_Fields == null)
			{
				this.m_Fields = this.Type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			}
			return this.m_Fields;
		}
	}

	// Token: 0x17000F39 RID: 3897
	// (get) Token: 0x06009478 RID: 38008 RVA: 0x003B3827 File Offset: 0x003B1A27
	public vp_StateManager StateManager
	{
		get
		{
			if (this.m_StateManager == null)
			{
				this.m_StateManager = new vp_StateManager(this, this.States);
			}
			return this.m_StateManager;
		}
	}

	// Token: 0x17000F3A RID: 3898
	// (get) Token: 0x06009479 RID: 38009 RVA: 0x003B3849 File Offset: 0x003B1A49
	public vp_State DefaultState
	{
		get
		{
			return this.m_DefaultState;
		}
	}

	// Token: 0x17000F3B RID: 3899
	// (get) Token: 0x0600947A RID: 38010 RVA: 0x003B3851 File Offset: 0x003B1A51
	public float Delta
	{
		get
		{
			return Time.deltaTime * 60f;
		}
	}

	// Token: 0x17000F3C RID: 3900
	// (get) Token: 0x0600947B RID: 38011 RVA: 0x003B385E File Offset: 0x003B1A5E
	public float SDelta
	{
		get
		{
			return Time.smoothDeltaTime * 60f;
		}
	}

	// Token: 0x17000F3D RID: 3901
	// (get) Token: 0x0600947C RID: 38012 RVA: 0x003B386B File Offset: 0x003B1A6B
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

	// Token: 0x17000F3E RID: 3902
	// (get) Token: 0x0600947D RID: 38013 RVA: 0x003B388D File Offset: 0x003B1A8D
	public Transform Parent
	{
		get
		{
			if (this.m_Parent == null)
			{
				this.m_Parent = base.transform.parent;
			}
			return this.m_Parent;
		}
	}

	// Token: 0x17000F3F RID: 3903
	// (get) Token: 0x0600947E RID: 38014 RVA: 0x003B38B4 File Offset: 0x003B1AB4
	public Transform Root
	{
		get
		{
			if (this.m_Root == null)
			{
				this.m_Root = base.transform.root;
			}
			return this.m_Root;
		}
	}

	// Token: 0x17000F40 RID: 3904
	// (get) Token: 0x0600947F RID: 38015 RVA: 0x003B38DB File Offset: 0x003B1ADB
	public AudioSource Audio
	{
		get
		{
			if (this.m_Audio == null)
			{
				this.m_Audio = base.GetComponent<AudioSource>();
			}
			return this.m_Audio;
		}
	}

	// Token: 0x17000F41 RID: 3905
	// (get) Token: 0x06009480 RID: 38016 RVA: 0x003B38FD File Offset: 0x003B1AFD
	public Collider Collider
	{
		get
		{
			if (this.m_Collider == null)
			{
				this.m_Collider = base.GetComponent<Collider>();
			}
			return this.m_Collider;
		}
	}

	// Token: 0x17000F42 RID: 3906
	// (get) Token: 0x06009481 RID: 38017 RVA: 0x003B3920 File Offset: 0x003B1B20
	// (set) Token: 0x06009482 RID: 38018 RVA: 0x003B398C File Offset: 0x003B1B8C
	public bool Rendering
	{
		get
		{
			if (this.Renderers != null)
			{
				foreach (Renderer renderer in this.Renderers)
				{
					if (renderer != null && renderer.enabled)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
		set
		{
			foreach (Renderer renderer in this.Renderers)
			{
				if (!(renderer == null))
				{
					renderer.enabled = value;
				}
			}
		}
	}

	// Token: 0x06009483 RID: 38019 RVA: 0x003B39E8 File Offset: 0x003B1BE8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.CacheChildren();
		this.CacheSiblings();
		this.CacheFamily();
		this.CacheRenderers();
		this.CacheAudioSources();
		this.StateManager.SetState("Default", base.enabled);
	}

	// Token: 0x06009484 RID: 38020 RVA: 0x003B3A1E File Offset: 0x003B1C1E
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
		this.ResetState();
	}

	// Token: 0x06009485 RID: 38021 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Init()
	{
	}

	// Token: 0x06009486 RID: 38022 RVA: 0x003B3A26 File Offset: 0x003B1C26
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.EventHandler != null)
		{
			this.EventHandler.Register(this);
		}
	}

	// Token: 0x06009487 RID: 38023 RVA: 0x003B3A42 File Offset: 0x003B1C42
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		if (this.EventHandler != null)
		{
			this.EventHandler.Unregister(this);
		}
	}

	// Token: 0x06009488 RID: 38024 RVA: 0x003B3A5E File Offset: 0x003B1C5E
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		if (!this.m_Initialized)
		{
			this.Init();
			this.m_Initialized = true;
		}
	}

	// Token: 0x06009489 RID: 38025 RVA: 0x003B3A78 File Offset: 0x003B1C78
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void FixedUpdate()
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		if (GameManager.Instance.World == null)
		{
			return;
		}
		if (GameManager.Instance.World.GetPrimaryPlayer() == null)
		{
			return;
		}
		Color color = Color.white;
		color.a = 1f;
		if (color == Color.black)
		{
			color.a = 1f;
			if (color == Color.black)
			{
				color = Color.gray;
			}
		}
		if (this.prevSkinColor == color)
		{
			return;
		}
		this.prevSkinColor = color;
		foreach (Renderer renderer in this.Renderers)
		{
			if (!(renderer == null) && renderer.enabled)
			{
				renderer.material.SetColor("Tint", color);
			}
		}
	}

	// Token: 0x0600948A RID: 38026 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void LateUpdate()
	{
	}

	// Token: 0x0600948B RID: 38027 RVA: 0x003B3B70 File Offset: 0x003B1D70
	public void SetState(string state, bool enabled = true, bool recursive = false, bool includeDisabled = false)
	{
		this.StateManager.SetState(state, enabled);
		if (recursive)
		{
			foreach (vp_Component vp_Component in this.Children)
			{
				if (includeDisabled || (vp_Utility.IsActive(vp_Component.gameObject) && vp_Component.enabled))
				{
					vp_Component.SetState(state, enabled, true, includeDisabled);
				}
			}
		}
	}

	// Token: 0x0600948C RID: 38028 RVA: 0x003B3BF0 File Offset: 0x003B1DF0
	public void ActivateGameObject(bool setActive = true)
	{
		if (setActive)
		{
			this.Activate();
			foreach (vp_Component vp_Component in this.Siblings)
			{
				vp_Component.Activate();
			}
			this.VerifyRenderers();
			return;
		}
		this.DeactivateWhenSilent();
		foreach (vp_Component vp_Component2 in this.Siblings)
		{
			vp_Component2.DeactivateWhenSilent();
		}
	}

	// Token: 0x0600948D RID: 38029 RVA: 0x003B3C98 File Offset: 0x003B1E98
	public void ResetState()
	{
		this.StateManager.Reset();
		this.Refresh();
	}

	// Token: 0x0600948E RID: 38030 RVA: 0x003B3CAB File Offset: 0x003B1EAB
	public bool StateEnabled(string stateName)
	{
		return this.StateManager.IsEnabled(stateName);
	}

	// Token: 0x0600948F RID: 38031 RVA: 0x003B3CBC File Offset: 0x003B1EBC
	public void RefreshDefaultState()
	{
		vp_State vp_State = null;
		if (this.States.Count == 0)
		{
			vp_State = new vp_State(this.Type.Name, "Default", null, null);
			this.States.Add(vp_State);
		}
		else
		{
			for (int i = this.States.Count - 1; i > -1; i--)
			{
				if (this.States[i].Name == "Default")
				{
					vp_State = this.States[i];
					this.States.Remove(vp_State);
					this.States.Add(vp_State);
				}
			}
			if (vp_State == null)
			{
				vp_State = new vp_State(this.Type.Name, "Default", null, null);
				this.States.Add(vp_State);
			}
		}
		if (vp_State.Preset == null || vp_State.Preset.ComponentType == null)
		{
			vp_State.Preset = new vp_ComponentPreset();
		}
		if (vp_State.TextAsset == null)
		{
			vp_State.Preset.InitFromComponent(this);
		}
		vp_State.Enabled = true;
		this.m_DefaultState = vp_State;
	}

	// Token: 0x06009490 RID: 38032 RVA: 0x003B3DD1 File Offset: 0x003B1FD1
	public void ApplyPreset(vp_ComponentPreset preset)
	{
		vp_ComponentPreset.Apply(this, preset);
		this.RefreshDefaultState();
		this.Refresh();
	}

	// Token: 0x06009491 RID: 38033 RVA: 0x003B3DE7 File Offset: 0x003B1FE7
	public vp_ComponentPreset Load(string path)
	{
		vp_ComponentPreset result = vp_ComponentPreset.LoadFromResources(this, path);
		this.RefreshDefaultState();
		this.Refresh();
		return result;
	}

	// Token: 0x06009492 RID: 38034 RVA: 0x003B3DFC File Offset: 0x003B1FFC
	public vp_ComponentPreset Load(TextAsset asset)
	{
		vp_ComponentPreset result = vp_ComponentPreset.LoadFromTextAsset(this, asset);
		this.RefreshDefaultState();
		this.Refresh();
		return result;
	}

	// Token: 0x06009493 RID: 38035 RVA: 0x003B3E14 File Offset: 0x003B2014
	public void CacheChildren()
	{
		this.Children.Clear();
		foreach (vp_Component vp_Component in base.GetComponentsInChildren<vp_Component>(true))
		{
			if (vp_Component.transform.parent == base.transform)
			{
				this.Children.Add(vp_Component);
			}
		}
	}

	// Token: 0x06009494 RID: 38036 RVA: 0x003B3E6C File Offset: 0x003B206C
	public void CacheSiblings()
	{
		this.Siblings.Clear();
		foreach (vp_Component vp_Component in base.GetComponents<vp_Component>())
		{
			if (vp_Component != this)
			{
				this.Siblings.Add(vp_Component);
			}
		}
	}

	// Token: 0x06009495 RID: 38037 RVA: 0x003B3EB4 File Offset: 0x003B20B4
	public void CacheFamily()
	{
		this.Family.Clear();
		foreach (vp_Component vp_Component in base.transform.root.GetComponentsInChildren<vp_Component>(true))
		{
			if (vp_Component != this)
			{
				this.Family.Add(vp_Component);
			}
		}
	}

	// Token: 0x06009496 RID: 38038 RVA: 0x003B3F08 File Offset: 0x003B2108
	public void CacheRenderers()
	{
		this.Renderers.Clear();
		foreach (Renderer item in base.GetComponentsInChildren<Renderer>(true))
		{
			this.Renderers.Add(item);
		}
	}

	// Token: 0x06009497 RID: 38039 RVA: 0x003B3F48 File Offset: 0x003B2148
	[PublicizedFrom(EAccessModifier.Protected)]
	public void VerifyRenderers()
	{
		if (this.Renderers.Count == 0)
		{
			return;
		}
		if (this.Renderers[0] == null || !vp_Utility.IsDescendant(this.Renderers[0].transform, this.Transform))
		{
			this.Renderers.Clear();
			this.CacheRenderers();
		}
	}

	// Token: 0x06009498 RID: 38040 RVA: 0x003B3FA8 File Offset: 0x003B21A8
	public void CacheAudioSources()
	{
		this.AudioSources.Clear();
		foreach (AudioSource item in base.GetComponentsInChildren<AudioSource>(true))
		{
			this.AudioSources.Add(item);
		}
	}

	// Token: 0x06009499 RID: 38041 RVA: 0x003B3FE6 File Offset: 0x003B21E6
	public virtual void Activate()
	{
		this.m_DeactivationTimer.Cancel();
		vp_Utility.Activate(base.gameObject, true);
	}

	// Token: 0x0600949A RID: 38042 RVA: 0x003B3FFF File Offset: 0x003B21FF
	public virtual void Deactivate()
	{
		vp_Utility.Activate(base.gameObject, false);
	}

	// Token: 0x0600949B RID: 38043 RVA: 0x003B4010 File Offset: 0x003B2210
	public void DeactivateWhenSilent()
	{
		if (this == null)
		{
			return;
		}
		if (vp_Utility.IsActive(base.gameObject))
		{
			foreach (AudioSource audioSource in this.AudioSources)
			{
				if (audioSource.isPlaying && !audioSource.loop)
				{
					this.Rendering = false;
					vp_Timer.In(0.1f, delegate()
					{
						this.DeactivateWhenSilent();
					}, this.m_DeactivationTimer);
					return;
				}
			}
		}
		this.Deactivate();
	}

	// Token: 0x0600949C RID: 38044 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Refresh()
	{
	}

	// Token: 0x040071A5 RID: 29093
	public bool Persist;

	// Token: 0x040071A6 RID: 29094
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_StateManager m_StateManager;

	// Token: 0x040071A7 RID: 29095
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_EventHandler m_EventHandler;

	// Token: 0x040071A8 RID: 29096
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_State m_DefaultState;

	// Token: 0x040071A9 RID: 29097
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_Initialized;

	// Token: 0x040071AA RID: 29098
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Transform;

	// Token: 0x040071AB RID: 29099
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Parent;

	// Token: 0x040071AC RID: 29100
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Root;

	// Token: 0x040071AD RID: 29101
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioSource m_Audio;

	// Token: 0x040071AE RID: 29102
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Collider m_Collider;

	// Token: 0x040071AF RID: 29103
	public List<vp_State> States = new List<vp_State>();

	// Token: 0x040071B0 RID: 29104
	public List<vp_Component> Children = new List<vp_Component>();

	// Token: 0x040071B1 RID: 29105
	public List<vp_Component> Siblings = new List<vp_Component>();

	// Token: 0x040071B2 RID: 29106
	public List<vp_Component> Family = new List<vp_Component>();

	// Token: 0x040071B3 RID: 29107
	public List<Renderer> Renderers = new List<Renderer>();

	// Token: 0x040071B4 RID: 29108
	public List<AudioSource> AudioSources = new List<AudioSource>();

	// Token: 0x040071B5 RID: 29109
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Type m_Type;

	// Token: 0x040071B6 RID: 29110
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public FieldInfo[] m_Fields;

	// Token: 0x040071B7 RID: 29111
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_DeactivationTimer = new vp_Timer.Handle();

	// Token: 0x040071B8 RID: 29112
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Color prevSkinColor = Color.white;
}
