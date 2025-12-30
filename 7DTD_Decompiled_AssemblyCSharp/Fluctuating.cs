using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000575 RID: 1397
[Preserve]
public class Fluctuating : LightState
{
	// Token: 0x17000477 RID: 1143
	// (get) Token: 0x06002D2A RID: 11562 RVA: 0x0012DE4B File Offset: 0x0012C04B
	public override float LODThreshold
	{
		get
		{
			return 0.2f;
		}
	}

	// Token: 0x17000478 RID: 1144
	// (get) Token: 0x06002D2B RID: 11563 RVA: 0x0012DE52 File Offset: 0x0012C052
	public override float Intensity
	{
		get
		{
			return this.currentIntensity;
		}
	}

	// Token: 0x17000479 RID: 1145
	// (get) Token: 0x06002D2C RID: 11564 RVA: 0x0012DE5A File Offset: 0x0012C05A
	public override float Emissive
	{
		get
		{
			return this.currentEmissive;
		}
	}

	// Token: 0x06002D2D RID: 11565 RVA: 0x0012DE62 File Offset: 0x0012C062
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.startIntensity = 1f;
		this.fixedFrameRate = 1f / Time.fixedDeltaTime;
		this.currentIntensity = this.startIntensity;
		this.currentEmissive = this.startIntensity;
	}

	// Token: 0x06002D2E RID: 11566 RVA: 0x0012DEA0 File Offset: 0x0012C0A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (GameManager.Instance.IsPaused())
		{
			return;
		}
		if (this.canSwitchProcess)
		{
			this.ChangeProcess();
			this.currentFrame = 0;
			int num = (int)(this.lightLOD.FluxDelay * this.fixedFrameRate);
			if (this.process == 0)
			{
				this.numOfFrames = UnityEngine.Random.Range(num / 2, num);
				this.t = 0f;
				this.preSlideIntenisty = this.currentIntensity;
				this.up = (UnityEngine.Random.Range(0f, 1f) > this.slideProbability(this.preSlideIntenisty));
				if (this.up)
				{
					this.slideTo = UnityEngine.Random.Range(this.preSlideIntenisty, 1f);
				}
				else
				{
					this.slideTo = UnityEngine.Random.Range(0.2f, this.preSlideIntenisty);
				}
				this.increment = (this.slideTo - this.preSlideIntenisty) / (float)this.numOfFrames;
			}
			else if (this.process == 1)
			{
				this.numOfFrames = UnityEngine.Random.Range(90, 181);
			}
			else
			{
				this.numOfFrames = UnityEngine.Random.Range(num / 2, num);
			}
		}
		if (this.process == 0)
		{
			this.Slide();
		}
		else if (this.process == 1)
		{
			this.Flutter();
		}
		else if (this.canSwitchProcess)
		{
			this.currentIntensity = this.startIntensity;
			this.currentEmissive = this.startIntensity / 1f;
		}
		int num2 = this.currentFrame + 1;
		this.currentFrame = num2;
		this.canSwitchProcess = (num2 >= this.numOfFrames);
	}

	// Token: 0x06002D2F RID: 11567 RVA: 0x0012E024 File Offset: 0x0012C224
	[PublicizedFrom(EAccessModifier.Private)]
	public void Slide()
	{
		if (this.up)
		{
			this.currentIntensity = Mathf.Lerp(this.preSlideIntenisty, this.slideTo, this.t);
			this.currentEmissive = this.currentIntensity / 1f;
			this.t += this.increment;
			return;
		}
		this.currentIntensity = Mathf.Lerp(this.slideTo, this.preSlideIntenisty, this.t);
		this.currentEmissive = this.currentIntensity / 1f;
		this.t -= this.increment;
	}

	// Token: 0x06002D30 RID: 11568 RVA: 0x0012E0C0 File Offset: 0x0012C2C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Flutter()
	{
		int num = UnityEngine.Random.Range(0, 3);
		if (num == 0)
		{
			this.currentIntensity = Mathf.Clamp(this.currentIntensity + 0.0625f, 0.2f, 1f);
			this.currentEmissive = this.currentIntensity / 1f;
			return;
		}
		if (num != 1)
		{
			return;
		}
		this.currentIntensity = Mathf.Clamp(this.currentIntensity - 0.0625f, 0.2f, 1f);
		this.currentEmissive = this.currentIntensity / 1f;
	}

	// Token: 0x06002D31 RID: 11569 RVA: 0x0012E144 File Offset: 0x0012C344
	[PublicizedFrom(EAccessModifier.Private)]
	public float slideProbability(float intensity)
	{
		return (intensity - 0.2f) / 0.8f;
	}

	// Token: 0x06002D32 RID: 11570 RVA: 0x0012E154 File Offset: 0x0012C354
	[PublicizedFrom(EAccessModifier.Private)]
	public void ChangeProcess()
	{
		int num = UnityEngine.Random.Range(1, 3);
		if (num != this.process)
		{
			this.process = num;
			return;
		}
		if (this.process > 0)
		{
			this.process = (this.process + 1) % 3;
			return;
		}
		this.process++;
	}

	// Token: 0x040023CB RID: 9163
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float unityLightIntensityMax = 8f;

	// Token: 0x040023CC RID: 9164
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float hiRange = 1f;

	// Token: 0x040023CD RID: 9165
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float loRange = 0.2f;

	// Token: 0x040023CE RID: 9166
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float flutterVariance = 0.0625f;

	// Token: 0x040023CF RID: 9167
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float increment;

	// Token: 0x040023D0 RID: 9168
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float startIntensity;

	// Token: 0x040023D1 RID: 9169
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fixedFrameRate;

	// Token: 0x040023D2 RID: 9170
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currentIntensity;

	// Token: 0x040023D3 RID: 9171
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currentEmissive;

	// Token: 0x040023D4 RID: 9172
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool canSwitchProcess = true;

	// Token: 0x040023D5 RID: 9173
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int process;

	// Token: 0x040023D6 RID: 9174
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int numOfFrames;

	// Token: 0x040023D7 RID: 9175
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int currentFrame;

	// Token: 0x040023D8 RID: 9176
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float t;

	// Token: 0x040023D9 RID: 9177
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float preSlideIntenisty;

	// Token: 0x040023DA RID: 9178
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float slideTo;

	// Token: 0x040023DB RID: 9179
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool up;
}
