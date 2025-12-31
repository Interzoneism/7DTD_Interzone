using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001338 RID: 4920
[RequireComponent(typeof(Collider))]
public abstract class vp_Interactable : MonoBehaviour
{
	// Token: 0x060098F2 RID: 39154 RVA: 0x003CD6F8 File Offset: 0x003CB8F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
		this.m_Transform = base.transform;
		if (this.RecipientTags.Count == 0)
		{
			this.RecipientTags.Add("Player");
		}
		if (this.InteractType == vp_Interactable.vp_InteractType.Trigger && base.GetComponent<Collider>() != null)
		{
			base.GetComponent<Collider>().isTrigger = true;
		}
	}

	// Token: 0x060098F3 RID: 39155 RVA: 0x003CD751 File Offset: 0x003CB951
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
	}

	// Token: 0x060098F4 RID: 39156 RVA: 0x003CD76D File Offset: 0x003CB96D
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}

	// Token: 0x060098F5 RID: 39157 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool TryInteract(vp_FPPlayerEventHandler player)
	{
		return false;
	}

	// Token: 0x060098F6 RID: 39158 RVA: 0x003CD78C File Offset: 0x003CB98C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnTriggerEnter(Collider col)
	{
		if (this.InteractType != vp_Interactable.vp_InteractType.Trigger)
		{
			return;
		}
		foreach (string b in this.RecipientTags)
		{
			if (col.gameObject.tag == b)
			{
				goto IL_4F;
			}
		}
		return;
		IL_4F:
		this.m_Player = col.gameObject.GetComponent<vp_FPPlayerEventHandler>();
		if (this.m_Player == null)
		{
			return;
		}
		this.TryInteract(this.m_Player);
	}

	// Token: 0x060098F7 RID: 39159 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void FinishInteraction()
	{
	}

	// Token: 0x060098F8 RID: 39160 RVA: 0x003CD828 File Offset: 0x003CBA28
	[PublicizedFrom(EAccessModifier.Protected)]
	public vp_Interactable()
	{
	}

	// Token: 0x04007588 RID: 30088
	public vp_Interactable.vp_InteractType InteractType;

	// Token: 0x04007589 RID: 30089
	public List<string> RecipientTags = new List<string>();

	// Token: 0x0400758A RID: 30090
	public float InteractDistance;

	// Token: 0x0400758B RID: 30091
	public Texture m_InteractCrosshair;

	// Token: 0x0400758C RID: 30092
	public string InteractText = "";

	// Token: 0x0400758D RID: 30093
	public float DelayShowingText = 2f;

	// Token: 0x0400758E RID: 30094
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Transform;

	// Token: 0x0400758F RID: 30095
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPController m_Controller;

	// Token: 0x04007590 RID: 30096
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPCamera m_Camera;

	// Token: 0x04007591 RID: 30097
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_WeaponHandler m_WeaponHandler;

	// Token: 0x04007592 RID: 30098
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPPlayerEventHandler m_Player;

	// Token: 0x02001339 RID: 4921
	public enum vp_InteractType
	{
		// Token: 0x04007594 RID: 30100
		Normal,
		// Token: 0x04007595 RID: 30101
		Trigger,
		// Token: 0x04007596 RID: 30102
		CollisionTrigger
	}
}
