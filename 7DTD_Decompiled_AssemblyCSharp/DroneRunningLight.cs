using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000397 RID: 919
public class DroneRunningLight : MonoBehaviour
{
	// Token: 0x06001B77 RID: 7031 RVA: 0x000AC392 File Offset: 0x000AA592
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.runningLight = base.GetComponent<Light>();
		this.particles = base.transform.GetComponentInChildren<ParticleSystem>();
		this._initLights();
		this.setLightsActive(true);
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x000AC3BE File Offset: 0x000AA5BE
	[PublicizedFrom(EAccessModifier.Private)]
	public void _initLights()
	{
		this.lightBlinkTimer = this.LightBlinkInterval;
		this.startIntensity = this.MinLightIntensity;
		this.runningLight.intensity = this.startIntensity;
		this.setLightColor(this.LightColor);
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x000AC3F8 File Offset: 0x000AA5F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void setLightsActive(bool value)
	{
		this.runningLight.enabled = value;
		if (this.connectedLights != null)
		{
			for (int i = 0; i < this.connectedLights.Count; i++)
			{
				this.connectedLights[i].enabled = value;
			}
		}
		if (!this.dayTimeVisibility)
		{
			this.particles.gameObject.SetActive(value);
		}
		this.lightsActive = value;
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x000AC464 File Offset: 0x000AA664
	[PublicizedFrom(EAccessModifier.Private)]
	public void setLightColor(Color color)
	{
		this.runningLight.color = color;
		this.particles.main.startColor = color;
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x000AC498 File Offset: 0x000AA698
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		float num = (float)GameUtils.WorldTimeToHours(world.worldTime);
		if (num > 4f && num < 22f)
		{
			if (this.runningLight.intensity > this.startIntensity)
			{
				this._initLights();
			}
			if (this.lightsActive)
			{
				this.setLightsActive(!this.lightsActive);
			}
			return;
		}
		if (!this.lightsActive)
		{
			this.setLightsActive(!this.lightsActive);
		}
		if (this.runningLight.color != this.LightColor || this.particles.main.startColor.color != this.LightColor)
		{
			this.setLightColor(this.LightColor);
		}
		if (this.startIntensity != this.MinLightIntensity)
		{
			this.startIntensity = this.MinLightIntensity;
		}
		if (this.lightBlinkTimer > 0f)
		{
			this.lightBlinkTimer -= Time.deltaTime;
			if (this.lightBlinkTimer < 0.2f && this.lightBlinkTimer > 0.15f && this.particles.gameObject.activeSelf)
			{
				this.particles.gameObject.SetActive(false);
			}
			if (this.lightBlinkTimer < 0.15f && !this.particles.gameObject.activeSelf)
			{
				this.particles.gameObject.SetActive(true);
			}
			if (this.lightBlinkTimer <= 0f)
			{
				base.StartCoroutine(this.blink());
			}
		}
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x000AC62A File Offset: 0x000AA82A
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator blink()
	{
		this.transitionTimer = this.transitionTime;
		while (this.runningLight.intensity < this.MaxLightIntensity)
		{
			this.transitionTimer += Time.deltaTime;
			this.runningLight.intensity = Mathf.Lerp(this.startIntensity, this.MaxLightIntensity, this.transitionTimer / this.transitionTime);
			yield return null;
		}
		this.transitionTimer = this.transitionTime;
		while (this.runningLight.intensity > this.startIntensity)
		{
			this.transitionTimer += Time.deltaTime;
			this.runningLight.intensity = Mathf.Lerp(this.MaxLightIntensity, this.startIntensity, this.transitionTimer / this.transitionTime);
			yield return null;
		}
		this.runningLight.intensity = this.startIntensity;
		this.lightBlinkTimer = this.LightBlinkInterval;
		yield return null;
		yield break;
	}

	// Token: 0x04001249 RID: 4681
	public float MinLightIntensity;

	// Token: 0x0400124A RID: 4682
	public float MaxLightIntensity;

	// Token: 0x0400124B RID: 4683
	public float LightBlinkInterval;

	// Token: 0x0400124C RID: 4684
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Light runningLight;

	// Token: 0x0400124D RID: 4685
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float startIntensity;

	// Token: 0x0400124E RID: 4686
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ParticleSystem particles;

	// Token: 0x0400124F RID: 4687
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lightBlinkTimer;

	// Token: 0x04001250 RID: 4688
	public Color LightColor;

	// Token: 0x04001251 RID: 4689
	public List<Light> connectedLights;

	// Token: 0x04001252 RID: 4690
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool lightsActive;

	// Token: 0x04001253 RID: 4691
	public bool dayTimeVisibility;

	// Token: 0x04001254 RID: 4692
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float transitionTime = 0.2f;

	// Token: 0x04001255 RID: 4693
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float transitionTimer;
}
