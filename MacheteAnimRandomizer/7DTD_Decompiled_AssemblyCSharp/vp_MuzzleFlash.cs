using System;
using UnityEngine;

// Token: 0x020012FC RID: 4860
public class vp_MuzzleFlash : MonoBehaviour
{
	// Token: 0x17000F78 RID: 3960
	// (get) Token: 0x0600976A RID: 38762 RVA: 0x003C4714 File Offset: 0x003C2914
	// (set) Token: 0x0600976B RID: 38763 RVA: 0x003C471C File Offset: 0x003C291C
	public float FadeSpeed
	{
		get
		{
			return this.m_FadeSpeed;
		}
		set
		{
			this.m_FadeSpeed = value;
		}
	}

	// Token: 0x17000F79 RID: 3961
	// (get) Token: 0x0600976C RID: 38764 RVA: 0x003C4725 File Offset: 0x003C2925
	// (set) Token: 0x0600976D RID: 38765 RVA: 0x003C472D File Offset: 0x003C292D
	public bool ForceShow
	{
		get
		{
			return this.m_ForceShow;
		}
		set
		{
			this.m_ForceShow = value;
		}
	}

	// Token: 0x0600976E RID: 38766 RVA: 0x003C4738 File Offset: 0x003C2938
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.m_Transform = base.transform;
		this.m_ForceShow = false;
		this.m_Light = base.GetComponent<Light>();
		if (this.m_Light != null)
		{
			this.m_LightIntensity = this.m_Light.intensity;
			this.m_Light.intensity = 0f;
		}
		this.m_Renderer = base.GetComponent<Renderer>();
		if (this.m_Renderer != null)
		{
			this.m_Material = base.GetComponent<Renderer>().material;
			if (this.m_Material != null)
			{
				this.m_Color = this.m_Material.GetColor("_TintColor");
				this.m_Color.a = 0f;
			}
		}
	}

	// Token: 0x0600976F RID: 38767 RVA: 0x003C47F4 File Offset: 0x003C29F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		GameObject gameObject = GameObject.Find("WeaponCamera");
		if (gameObject != null && gameObject.transform.parent == this.m_Transform.parent)
		{
			base.gameObject.layer = 10;
		}
	}

	// Token: 0x06009770 RID: 38768 RVA: 0x003C4840 File Offset: 0x003C2A40
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.m_ForceShow)
		{
			this.Show();
		}
		else if (this.m_Color.a > 0f)
		{
			this.m_Color.a = this.m_Color.a - this.m_FadeSpeed * (Time.deltaTime * 60f);
			if (this.m_Light != null)
			{
				this.m_Light.intensity = this.m_LightIntensity * (this.m_Color.a * 2f);
			}
		}
		if (this.m_Material != null)
		{
			this.m_Material.SetColor("_TintColor", this.m_Color);
		}
		if (this.m_Color.a < 0.01f)
		{
			this.m_Renderer.enabled = false;
			if (this.m_Light != null)
			{
				this.m_Light.enabled = false;
			}
		}
	}

	// Token: 0x06009771 RID: 38769 RVA: 0x003C4920 File Offset: 0x003C2B20
	public void Show()
	{
		this.m_Renderer.enabled = true;
		if (this.m_Light != null)
		{
			this.m_Light.enabled = true;
			this.m_Light.intensity = this.m_LightIntensity;
		}
		this.m_Color.a = 0.5f;
	}

	// Token: 0x06009772 RID: 38770 RVA: 0x003C4974 File Offset: 0x003C2B74
	public void Shoot()
	{
		this.ShootInternal(true);
	}

	// Token: 0x06009773 RID: 38771 RVA: 0x003C497D File Offset: 0x003C2B7D
	public void ShootLightOnly()
	{
		this.ShootInternal(false);
	}

	// Token: 0x06009774 RID: 38772 RVA: 0x003C4988 File Offset: 0x003C2B88
	public void ShootInternal(bool showMesh)
	{
		this.m_Color.a = 0.5f;
		if (showMesh)
		{
			this.m_Transform.Rotate(0f, 0f, (float)UnityEngine.Random.Range(0, 360));
			this.m_Renderer.enabled = true;
		}
		if (this.m_Light != null)
		{
			this.m_Light.enabled = true;
			this.m_Light.intensity = this.m_LightIntensity;
		}
	}

	// Token: 0x06009775 RID: 38773 RVA: 0x003C4A00 File Offset: 0x003C2C00
	public void SetFadeSpeed(float fadeSpeed)
	{
		this.FadeSpeed = fadeSpeed;
	}

	// Token: 0x040073D4 RID: 29652
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_FadeSpeed = 0.075f;

	// Token: 0x040073D5 RID: 29653
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_ForceShow;

	// Token: 0x040073D6 RID: 29654
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Color m_Color = new Color(1f, 1f, 1f, 0f);

	// Token: 0x040073D7 RID: 29655
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Transform;

	// Token: 0x040073D8 RID: 29656
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Light m_Light;

	// Token: 0x040073D9 RID: 29657
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_LightIntensity;

	// Token: 0x040073DA RID: 29658
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Renderer m_Renderer;

	// Token: 0x040073DB RID: 29659
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Material m_Material;
}
