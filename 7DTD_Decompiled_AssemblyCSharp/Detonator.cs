using System;
using UnityEngine;

// Token: 0x0200116D RID: 4461
public class Detonator : MonoBehaviour
{
	// Token: 0x06008B7D RID: 35709 RVA: 0x0038490B File Offset: 0x00382B0B
	public void StartCountdown()
	{
		if (base.isActiveAndEnabled)
		{
			return;
		}
		base.enabled = true;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06008B7E RID: 35710 RVA: 0x00384929 File Offset: 0x00382B29
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this._animTime = 0f;
		this._animTimeDetonator = 0f;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06008B7F RID: 35711 RVA: 0x0038494D File Offset: 0x00382B4D
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06008B80 RID: 35712 RVA: 0x0038495C File Offset: 0x00382B5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		float num = Time.deltaTime;
		num *= this.PulseRateScale;
		this._animTime += num;
		this._animTimeDetonator += num * ((this._timeRate != null) ? this._timeRate.Evaluate(this._animTime) : 1f);
		if (this._light != null && this._lightIntensity != null)
		{
			this._light.intensity = this._lightIntensity.Evaluate(this._animTimeDetonator);
		}
	}

	// Token: 0x04006CFE RID: 27902
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public Light _light;

	// Token: 0x04006CFF RID: 27903
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public AnimationCurve _timeRate;

	// Token: 0x04006D00 RID: 27904
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public AnimationCurve _lightIntensity;

	// Token: 0x04006D01 RID: 27905
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float _animTime;

	// Token: 0x04006D02 RID: 27906
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float _animTimeDetonator;

	// Token: 0x04006D03 RID: 27907
	public float PulseRateScale = 1f;
}
