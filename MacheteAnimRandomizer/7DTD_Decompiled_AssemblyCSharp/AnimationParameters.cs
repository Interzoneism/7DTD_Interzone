using System;
using UnityEngine;

// Token: 0x0200007F RID: 127
public class AnimationParameters : MonoBehaviour
{
	// Token: 0x06000254 RID: 596 RVA: 0x00013238 File Offset: 0x00011438
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.anim = base.GetComponent<Animator>();
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00013248 File Offset: 0x00011448
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdate()
	{
		this.currentRotation = base.transform.rotation;
		Quaternion quaternion = this.currentRotation * Quaternion.Inverse(this.previousRotation);
		this.previousRotation = this.currentRotation;
		float num;
		Vector3 a;
		quaternion.ToAngleAxis(out num, out a);
		num *= 0.017453292f;
		this.angularVelocity = 1f / Time.deltaTime * num * a;
		this.deltaYaw = Mathf.SmoothDamp(this.deltaYaw, this.angularVelocity.y, ref this.turnVelocity, this.deltaYawSmoothTime);
		if (this.debugMode)
		{
			if (Mathf.Abs(this.deltaYaw) > 0.001f)
			{
				Debug.Log("DeltaYaw: " + this.deltaYaw.ToString());
			}
			if (this.deltaYaw < this.deltaYawMin)
			{
				this.deltaYawMin = this.deltaYaw;
			}
			if (this.deltaYaw > this.deltaYawMax)
			{
				this.deltaYawMax = this.deltaYaw;
			}
		}
		this.anim.SetFloat("deltaYaw", this.deltaYaw);
		this.anim.SetFloat("TurnPlayRate", this.deltaYaw);
		if (Mathf.Abs(this.angularVelocity.y) > 0.1f)
		{
			this.anim.SetBool("Turning", true);
			return;
		}
		this.anim.SetBool("Turning", false);
	}

	// Token: 0x040002EB RID: 747
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator anim;

	// Token: 0x040002EC RID: 748
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 currentEulerAngles;

	// Token: 0x040002ED RID: 749
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion currentRotation;

	// Token: 0x040002EE RID: 750
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion previousRotation;

	// Token: 0x040002EF RID: 751
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currentYaw;

	// Token: 0x040002F0 RID: 752
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastYaw;

	// Token: 0x040002F1 RID: 753
	public float turnPlayRateMultiplier;

	// Token: 0x040002F2 RID: 754
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float turnPlayRate;

	// Token: 0x040002F3 RID: 755
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float deltaYawTarget;

	// Token: 0x040002F4 RID: 756
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float deltaYaw;

	// Token: 0x040002F5 RID: 757
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float angle;

	// Token: 0x040002F6 RID: 758
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 angularVelocity;

	// Token: 0x040002F7 RID: 759
	public bool debugMode;

	// Token: 0x040002F8 RID: 760
	public float deltaYawMin;

	// Token: 0x040002F9 RID: 761
	public float deltaYawMax;

	// Token: 0x040002FA RID: 762
	public float deltaYawSmoothTime = 0.3f;

	// Token: 0x040002FB RID: 763
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float turnVelocity;
}
