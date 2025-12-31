using System;
using UnityEngine;

// Token: 0x02001372 RID: 4978
public class vp_SimpleCrosshair : MonoBehaviour
{
	// Token: 0x06009BFA RID: 39930 RVA: 0x003DFD7F File Offset: 0x003DDF7F
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Player = (UnityEngine.Object.FindObjectOfType(typeof(vp_FPPlayerEventHandler)) as vp_FPPlayerEventHandler);
	}

	// Token: 0x06009BFB RID: 39931 RVA: 0x003DFD9B File Offset: 0x003DDF9B
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
	}

	// Token: 0x06009BFC RID: 39932 RVA: 0x003DFDB7 File Offset: 0x003DDFB7
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}

	// Token: 0x06009BFD RID: 39933 RVA: 0x003DFDD4 File Offset: 0x003DDFD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGUI()
	{
		if (this.m_ImageCrosshair == null)
		{
			return;
		}
		if (this.HideOnFirstPersonZoom && this.m_Player.Zoom.Active && this.m_Player.IsFirstPerson.Get())
		{
			return;
		}
		if (this.HideOnDeath && this.m_Player.Dead.Active)
		{
			return;
		}
		GUI.color = new Color(1f, 1f, 1f, 0.8f);
		GUI.DrawTexture(new Rect((float)Screen.width * 0.5f - (float)this.m_ImageCrosshair.width * 0.5f, (float)Screen.height * 0.5f - (float)this.m_ImageCrosshair.height * 0.5f, (float)this.m_ImageCrosshair.width, (float)this.m_ImageCrosshair.height), this.m_ImageCrosshair);
		GUI.color = Color.white;
	}

	// Token: 0x17001045 RID: 4165
	// (get) Token: 0x06009BFE RID: 39934 RVA: 0x003DFECB File Offset: 0x003DE0CB
	// (set) Token: 0x06009BFF RID: 39935 RVA: 0x003DFED3 File Offset: 0x003DE0D3
	public virtual Texture OnValue_Crosshair
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.m_ImageCrosshair;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			this.m_ImageCrosshair = value;
		}
	}

	// Token: 0x0400789A RID: 30874
	public Texture m_ImageCrosshair;

	// Token: 0x0400789B RID: 30875
	public bool HideOnFirstPersonZoom = true;

	// Token: 0x0400789C RID: 30876
	public bool HideOnDeath = true;

	// Token: 0x0400789D RID: 30877
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPPlayerEventHandler m_Player;
}
