using System;
using System.Globalization;
using UnityEngine;

// Token: 0x02000382 RID: 898
public class AutoTurretYawLerp : MonoBehaviour
{
	// Token: 0x17000313 RID: 787
	// (get) Token: 0x06001AB0 RID: 6832 RVA: 0x000A6194 File Offset: 0x000A4394
	// (set) Token: 0x06001AB1 RID: 6833 RVA: 0x000A619C File Offset: 0x000A439C
	public float Yaw { get; set; }

	// Token: 0x17000314 RID: 788
	// (get) Token: 0x06001AB2 RID: 6834 RVA: 0x000A61A8 File Offset: 0x000A43A8
	public float CurrentYaw
	{
		get
		{
			return this.myTransform.localRotation.eulerAngles.y - this.BaseRotation.y;
		}
	}

	// Token: 0x17000315 RID: 789
	// (get) Token: 0x06001AB3 RID: 6835 RVA: 0x000A61D9 File Offset: 0x000A43D9
	// (set) Token: 0x06001AB4 RID: 6836 RVA: 0x000A61E1 File Offset: 0x000A43E1
	public bool IsTurning { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06001AB5 RID: 6837 RVA: 0x000A61EC File Offset: 0x000A43EC
	public void Init(DynamicProperties _properties)
	{
		if (_properties.Values.ContainsKey("TurnSpeed"))
		{
			this.degreesPerSecond = StringParsers.ParseFloat(_properties.Values["TurnSpeed"], 0, -1, NumberStyles.Any);
		}
		if (_properties.Values.ContainsKey("TurnSpeedIdle"))
		{
			this.idleDegreesPerSecond = StringParsers.ParseFloat(_properties.Values["TurnSpeedIdle"], 0, -1, NumberStyles.Any);
		}
	}

	// Token: 0x06001AB6 RID: 6838 RVA: 0x000A6261 File Offset: 0x000A4461
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.myTransform = base.transform;
	}

	// Token: 0x06001AB7 RID: 6839 RVA: 0x000A626F File Offset: 0x000A446F
	public void SetYaw()
	{
		this.myTransform.localRotation = Quaternion.Euler(this.BaseRotation.x, this.BaseRotation.y + this.Yaw, this.BaseRotation.z);
	}

	// Token: 0x06001AB8 RID: 6840 RVA: 0x000A62AC File Offset: 0x000A44AC
	public void UpdateYaw()
	{
		float num = Mathf.LerpAngle(this.myTransform.localRotation.eulerAngles.y, this.BaseRotation.y + this.Yaw, Time.deltaTime * (this.IdleScan ? this.idleDegreesPerSecond : this.degreesPerSecond));
		this.myTransform.localRotation = Quaternion.Euler(this.BaseRotation.x, num, this.BaseRotation.z);
		this.IsTurning = ((int)num != (int)((this.BaseRotation.y + this.Yaw) * 100f));
	}

	// Token: 0x0400117A RID: 4474
	public Vector3 BaseRotation = Vector3.zero;

	// Token: 0x0400117C RID: 4476
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float degreesPerSecond = 11.25f;

	// Token: 0x0400117D RID: 4477
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float idleDegreesPerSecond = 0.5f;

	// Token: 0x0400117E RID: 4478
	public bool IdleScan;

	// Token: 0x0400117F RID: 4479
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform myTransform;
}
