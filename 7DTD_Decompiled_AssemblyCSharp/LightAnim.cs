using System;
using UnityEngine;

// Token: 0x02000FF0 RID: 4080
public class LightAnim : MonoBehaviour
{
	// Token: 0x06008175 RID: 33141 RVA: 0x003471A4 File Offset: 0x003453A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.Duration = Utils.FastMax(this.Duration, 0.001f);
		this.LightRef = base.GetComponent<Light>();
		this.intensityStart = this.LightRef.intensity;
		int length = this.IntensityCurve.length;
		if (length > 0)
		{
			this.timeEnd = this.IntensityCurve[length - 1].time;
		}
	}

	// Token: 0x06008176 RID: 33142 RVA: 0x00347210 File Offset: 0x00345410
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		float num = Time.deltaTime / this.Duration;
		this.time += num;
		if (this.time < this.timeEnd)
		{
			this.LightRef.intensity = this.intensityStart * this.IntensityCurve.Evaluate(this.time);
			return;
		}
		if (this.DestroyAtEnd)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x040063FB RID: 25595
	public float Duration = 0.5f;

	// Token: 0x040063FC RID: 25596
	public AnimationCurve IntensityCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.1f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x040063FD RID: 25597
	public bool DestroyAtEnd;

	// Token: 0x040063FE RID: 25598
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Light LightRef;

	// Token: 0x040063FF RID: 25599
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float intensityStart;

	// Token: 0x04006400 RID: 25600
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float time;

	// Token: 0x04006401 RID: 25601
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeEnd;
}
