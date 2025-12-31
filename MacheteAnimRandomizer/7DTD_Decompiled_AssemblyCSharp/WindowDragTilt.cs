using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
[AddComponentMenu("NGUI/Examples/Window Drag Tilt")]
public class WindowDragTilt : MonoBehaviour
{
	// Token: 0x060001A3 RID: 419 RVA: 0x0000F4E8 File Offset: 0x0000D6E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.mTrans = base.transform;
		this.mLastPos = this.mTrans.position;
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000F508 File Offset: 0x0000D708
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		Vector3 vector = this.mTrans.position - this.mLastPos;
		this.mLastPos = this.mTrans.position;
		this.mAngle += vector.x * this.degrees;
		this.mAngle = NGUIMath.SpringLerp(this.mAngle, 0f, 20f, Time.deltaTime);
		this.mTrans.localRotation = Quaternion.Euler(0f, 0f, -this.mAngle);
	}

	// Token: 0x0400024F RID: 591
	public int updateOrder;

	// Token: 0x04000250 RID: 592
	public float degrees = 30f;

	// Token: 0x04000251 RID: 593
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 mLastPos;

	// Token: 0x04000252 RID: 594
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform mTrans;

	// Token: 0x04000253 RID: 595
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float mAngle;
}
