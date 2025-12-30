using System;
using System.Globalization;
using UnityEngine;

// Token: 0x02000381 RID: 897
public class AutoTurretPitchLerp : MonoBehaviour
{
	// Token: 0x17000310 RID: 784
	// (get) Token: 0x06001AA6 RID: 6822 RVA: 0x000A5FD6 File Offset: 0x000A41D6
	// (set) Token: 0x06001AA7 RID: 6823 RVA: 0x000A5FDE File Offset: 0x000A41DE
	public float Pitch { get; set; }

	// Token: 0x17000311 RID: 785
	// (get) Token: 0x06001AA8 RID: 6824 RVA: 0x000A5FE8 File Offset: 0x000A41E8
	public float CurrentPitch
	{
		get
		{
			return this.myTransform.localRotation.eulerAngles.x - this.BaseRotation.x;
		}
	}

	// Token: 0x17000312 RID: 786
	// (get) Token: 0x06001AA9 RID: 6825 RVA: 0x000A6019 File Offset: 0x000A4219
	// (set) Token: 0x06001AAA RID: 6826 RVA: 0x000A6021 File Offset: 0x000A4221
	public bool IsTurning { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06001AAB RID: 6827 RVA: 0x000A602A File Offset: 0x000A422A
	public void Init(DynamicProperties _properties)
	{
		if (_properties.Values.ContainsKey("TurnSpeed"))
		{
			this.degreesPerSecond = StringParsers.ParseFloat(_properties.Values["TurnSpeed"], 0, -1, NumberStyles.Any);
		}
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x000A6060 File Offset: 0x000A4260
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.myTransform = base.transform;
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x000A606E File Offset: 0x000A426E
	public void SetPitch()
	{
		this.myTransform.localRotation = Quaternion.Euler(this.BaseRotation.x + this.Pitch, this.BaseRotation.y, this.BaseRotation.z);
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x000A60A8 File Offset: 0x000A42A8
	public void UpdatePitch()
	{
		int num = (int)(this.myTransform.localRotation.eulerAngles.x * 1000f);
		this.myTransform.localRotation = Quaternion.Euler(Mathf.LerpAngle(this.myTransform.localRotation.eulerAngles.x, this.BaseRotation.x + this.Pitch, Time.deltaTime * ((this.IdleScan ? 0.25f : 1f) * this.degreesPerSecond)), this.BaseRotation.y, this.BaseRotation.z);
		this.IsTurning = ((int)(this.myTransform.localRotation.eulerAngles.x * 1000f) != num);
	}

	// Token: 0x04001174 RID: 4468
	public Vector3 BaseRotation = Vector3.zero;

	// Token: 0x04001176 RID: 4470
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float degreesPerSecond = 11.25f;

	// Token: 0x04001177 RID: 4471
	public bool IdleScan;

	// Token: 0x04001178 RID: 4472
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform myTransform;
}
