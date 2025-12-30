using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000B37 RID: 2871
[Preserve]
public class VPHeadlight : VehiclePart
{
	// Token: 0x06005934 RID: 22836 RVA: 0x0023F424 File Offset: 0x0023D624
	public override void InitPrefabConnections()
	{
		this.headlightT = base.GetTransform();
		if (this.headlightT)
		{
			GameObject gameObject = this.headlightT.gameObject;
			this.lights = new List<Light>();
			gameObject.GetComponentsInChildren<Light>(true, this.lights);
			for (int i = this.lights.Count - 1; i >= 0; i--)
			{
				if (this.lights[i].type != LightType.Spot)
				{
					this.lights.RemoveAt(i);
				}
			}
		}
		Transform transform = base.GetTransform("matT");
		if (transform)
		{
			MeshRenderer componentInChildren = transform.GetComponentInChildren<MeshRenderer>();
			if (componentInChildren)
			{
				this.headlightMat = componentInChildren.material;
			}
		}
		this.modT = base.GetTransform("modT");
		if (this.modT)
		{
			List<MeshRenderer> list = new List<MeshRenderer>();
			this.modT.GetComponentsInChildren<MeshRenderer>(list);
			for (int j = 0; j < list.Count; j++)
			{
				MeshRenderer meshRenderer = list[j];
				if (meshRenderer.gameObject.CompareTag("LOD"))
				{
					if (!this.modMat)
					{
						this.modMat = meshRenderer.material;
					}
					else
					{
						meshRenderer.material = this.modMat;
					}
				}
			}
		}
		this.modOnT = base.GetTransform("modOnT");
		if (this.modOnT)
		{
			GameObject gameObject2 = this.modOnT.gameObject;
			this.modLights = new List<Light>();
			gameObject2.GetComponentsInChildren<Light>(true, this.modLights);
			for (int k = this.modLights.Count - 1; k >= 0; k--)
			{
				if (this.modLights[k].type != LightType.Spot)
				{
					this.modLights.RemoveAt(k);
				}
			}
		}
		this.curIntensity = -1f;
	}

	// Token: 0x06005935 RID: 22837 RVA: 0x0023F5E8 File Offset: 0x0023D7E8
	public override void SetProperties(DynamicProperties _properties)
	{
		base.SetProperties(_properties);
		StringParsers.TryParseFloat(base.GetProperty("bright"), out this.bright, 0, -1, NumberStyles.Any);
		this.properties.ParseColorHex("matEmissive", ref this.headLightEmissive);
		this.properties.ParseColorHex("modMatEmissive", ref this.modMatEmissive);
		this.properties.ParseColorHex("tailEmissive", ref this.tailLightEmissive);
	}

	// Token: 0x06005936 RID: 22838 RVA: 0x0023F65C File Offset: 0x0023D85C
	public override void SetMods()
	{
		base.SetMods();
		this.UpdateOn();
	}

	// Token: 0x06005937 RID: 22839 RVA: 0x0023F66A File Offset: 0x0023D86A
	public override void HandleEvent(VehiclePart.Event _event, VehiclePart _part, float _arg)
	{
		if (_event == VehiclePart.Event.LightsOn)
		{
			this.SetOn(_arg != 0f);
			this.PlaySound();
		}
	}

	// Token: 0x06005938 RID: 22840 RVA: 0x0023F688 File Offset: 0x0023D888
	public override void Update(float _dt)
	{
		if (this.IsBroken())
		{
			this.SetOn(false);
			return;
		}
		if (this.lights != null)
		{
			float num = (this.vehicle.EffectLightIntensity - 1f) * 0.5f + 1f;
			num = Utils.FastClamp(num, 0f, 10f);
			if (num != this.curIntensity)
			{
				this.curIntensity = num;
				float num2 = this.bright * num;
				float range = 50f * num;
				if (this.modInstalled && this.modLights != null)
				{
					num2 *= 0.58f;
					for (int i = this.modLights.Count - 1; i >= 0; i--)
					{
						Light light = this.modLights[i];
						light.intensity = num2;
						light.range = range;
					}
				}
				for (int j = this.lights.Count - 1; j >= 0; j--)
				{
					Light light2 = this.lights[j];
					light2.intensity = num2;
					light2.range = range;
				}
			}
		}
		this.SetTailLights();
	}

	// Token: 0x06005939 RID: 22841 RVA: 0x0023F788 File Offset: 0x0023D988
	public void SetTailLights()
	{
		float num = 0f;
		if (this.isOn)
		{
			num = 0.5f;
		}
		if (this.vehicle.CurrentIsBreak)
		{
			num = 1f;
		}
		if (num == this.tailLightIntensity)
		{
			return;
		}
		this.tailLightIntensity = num;
		if (this.vehicle.mainEmissiveMat && this.tailLightEmissive.a > 0f)
		{
			Color value = this.tailLightEmissive;
			value.r *= num;
			value.g *= num;
			value.b *= num;
			this.vehicle.mainEmissiveMat.SetColor("_EmissionColor", value);
		}
	}

	// Token: 0x0600593A RID: 22842 RVA: 0x0023F832 File Offset: 0x0023DA32
	public bool IsOn()
	{
		return this.isOn;
	}

	// Token: 0x0600593B RID: 22843 RVA: 0x0023F83A File Offset: 0x0023DA3A
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetOn(bool _isOn)
	{
		if (_isOn == this.isOn)
		{
			return;
		}
		this.isOn = _isOn;
		this.UpdateOn();
		this.SetTailLights();
	}

	// Token: 0x0600593C RID: 22844 RVA: 0x0023F85C File Offset: 0x0023DA5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateOn()
	{
		this.curIntensity = -1f;
		if (this.headlightT)
		{
			this.headlightT.gameObject.SetActive(this.isOn);
		}
		if (this.headlightMat)
		{
			Color value = this.isOn ? this.headLightEmissive : Color.black;
			this.headlightMat.SetColor("_EmissionColor", value);
		}
		if (this.modInstalled)
		{
			if (this.modOnT)
			{
				this.modOnT.gameObject.SetActive(this.isOn);
			}
			if (this.modMat)
			{
				Color value2 = this.isOn ? this.modMatEmissive : Color.black;
				this.modMat.SetColor("_EmissionColor", value2);
			}
		}
	}

	// Token: 0x0600593D RID: 22845 RVA: 0x0023F92A File Offset: 0x0023DB2A
	public float GetLightLevel()
	{
		if (!this.isOn)
		{
			return 0f;
		}
		return this.bright * 3f;
	}

	// Token: 0x0600593E RID: 22846 RVA: 0x0023F948 File Offset: 0x0023DB48
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlaySound()
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (!primaryPlayer || !primaryPlayer.IsSpawned())
		{
			return;
		}
		if (this.vehicle.entity != null && !this.vehicle.entity.isEntityRemote)
		{
			this.vehicle.entity.PlayOneShot("UseActions/flashlight_toggle", false, false, false, null);
		}
	}

	// Token: 0x04004429 RID: 17449
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cRange = 50f;

	// Token: 0x0400442A RID: 17450
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cModBrightPer = 0.58f;

	// Token: 0x0400442B RID: 17451
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform headlightT;

	// Token: 0x0400442C RID: 17452
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Light> lights;

	// Token: 0x0400442D RID: 17453
	[PublicizedFrom(EAccessModifier.Private)]
	public Material headlightMat;

	// Token: 0x0400442E RID: 17454
	[PublicizedFrom(EAccessModifier.Private)]
	public Color headLightEmissive;

	// Token: 0x0400442F RID: 17455
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform modT;

	// Token: 0x04004430 RID: 17456
	[PublicizedFrom(EAccessModifier.Private)]
	public Material modMat;

	// Token: 0x04004431 RID: 17457
	[PublicizedFrom(EAccessModifier.Private)]
	public Color modMatEmissive;

	// Token: 0x04004432 RID: 17458
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform modOnT;

	// Token: 0x04004433 RID: 17459
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Light> modLights;

	// Token: 0x04004434 RID: 17460
	[PublicizedFrom(EAccessModifier.Private)]
	public float bright;

	// Token: 0x04004435 RID: 17461
	[PublicizedFrom(EAccessModifier.Private)]
	public Color tailLightEmissive;

	// Token: 0x04004436 RID: 17462
	[PublicizedFrom(EAccessModifier.Private)]
	public float tailLightIntensity = -1f;

	// Token: 0x04004437 RID: 17463
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOn;

	// Token: 0x04004438 RID: 17464
	[PublicizedFrom(EAccessModifier.Private)]
	public float curIntensity;
}
