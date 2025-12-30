using System;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class EyeAdv_AutoDilation : MonoBehaviour
{
	// Token: 0x06000026 RID: 38 RVA: 0x00002AB3 File Offset: 0x00000CB3
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.eyeRenderer = base.gameObject.GetComponent<Renderer>();
		if (this.sceneLightObject != null)
		{
			this.sceneLight = this.sceneLightObject.GetComponent<Light>();
		}
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002AE8 File Offset: 0x00000CE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.sceneLight != null)
		{
			this.lightIntensity = this.sceneLight.intensity;
			if (this.enableAutoDilation)
			{
				if (this.currTargetDilation != this.targetDilation || this.currLightSensitivity != this.lightSensitivity)
				{
					this.dilateTime = 0f;
					this.currTargetDilation = this.targetDilation;
					this.currLightSensitivity = this.lightSensitivity;
				}
				this.lightAngle = Vector3.Angle(this.sceneLightObject.transform.forward, base.transform.forward) / 180f;
				this.targetDilation = Mathf.Lerp(1f, 0f, this.lightAngle * this.lightIntensity * this.lightSensitivity);
				this.dilateTime += Time.deltaTime * this.dilationSpeed;
				this.pupilDilation = Mathf.Clamp(this.pupilDilation, 0f, this.maxDilation);
				this.pupilDilation = Mathf.Lerp(this.pupilDilation, this.targetDilation, this.dilateTime);
				this.eyeRenderer.sharedMaterial.SetFloat("_pupilSize", this.pupilDilation);
			}
		}
	}

	// Token: 0x0400002B RID: 43
	public bool enableAutoDilation = true;

	// Token: 0x0400002C RID: 44
	public Transform sceneLightObject;

	// Token: 0x0400002D RID: 45
	public float lightSensitivity = 1f;

	// Token: 0x0400002E RID: 46
	public float dilationSpeed = 0.1f;

	// Token: 0x0400002F RID: 47
	public float maxDilation = 1f;

	// Token: 0x04000030 RID: 48
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Light sceneLight;

	// Token: 0x04000031 RID: 49
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lightIntensity;

	// Token: 0x04000032 RID: 50
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lightAngle;

	// Token: 0x04000033 RID: 51
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float dilateTime;

	// Token: 0x04000034 RID: 52
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float pupilDilation = 0.5f;

	// Token: 0x04000035 RID: 53
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currTargetDilation = -1f;

	// Token: 0x04000036 RID: 54
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float targetDilation;

	// Token: 0x04000037 RID: 55
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currLightSensitivity = -1f;

	// Token: 0x04000038 RID: 56
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Renderer eyeRenderer;
}
