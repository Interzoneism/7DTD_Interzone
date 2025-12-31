using System;
using UnityEngine;

// Token: 0x02000053 RID: 83
[AddComponentMenu("NGUI/Examples/Spin")]
public class Spin : MonoBehaviour
{
	// Token: 0x06000192 RID: 402 RVA: 0x0000F0E1 File Offset: 0x0000D2E1
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.mTrans = base.transform;
		this.mRb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000193 RID: 403 RVA: 0x0000F0FB File Offset: 0x0000D2FB
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.mRb == null)
		{
			this.ApplyDelta(this.ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime);
		}
	}

	// Token: 0x06000194 RID: 404 RVA: 0x0000F125 File Offset: 0x0000D325
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdate()
	{
		if (this.mRb != null)
		{
			this.ApplyDelta(Time.deltaTime);
		}
	}

	// Token: 0x06000195 RID: 405 RVA: 0x0000F140 File Offset: 0x0000D340
	public void ApplyDelta(float delta)
	{
		delta *= 360f;
		Quaternion rhs = Quaternion.Euler(this.rotationsPerSecond * delta);
		if (this.mRb == null)
		{
			this.mTrans.rotation = this.mTrans.rotation * rhs;
			return;
		}
		this.mRb.MoveRotation(this.mRb.rotation * rhs);
	}

	// Token: 0x04000240 RID: 576
	public Vector3 rotationsPerSecond = new Vector3(0f, 0.1f, 0f);

	// Token: 0x04000241 RID: 577
	public bool ignoreTimeScale;

	// Token: 0x04000242 RID: 578
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Rigidbody mRb;

	// Token: 0x04000243 RID: 579
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform mTrans;
}
