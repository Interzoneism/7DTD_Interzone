using System;
using UnityEngine;

// Token: 0x02000057 RID: 87
[AddComponentMenu("NGUI/Examples/Window Auto-Yaw")]
public class WindowAutoYaw : MonoBehaviour
{
	// Token: 0x0600019F RID: 415 RVA: 0x0000F42A File Offset: 0x0000D62A
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		this.mTrans.localRotation = Quaternion.identity;
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000F43C File Offset: 0x0000D63C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		if (this.uiCamera == null)
		{
			this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		this.mTrans = base.transform;
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x0000F470 File Offset: 0x0000D670
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.uiCamera != null)
		{
			Vector3 vector = this.uiCamera.WorldToViewportPoint(this.mTrans.position);
			this.mTrans.localRotation = Quaternion.Euler(0f, (vector.x * 2f - 1f) * this.yawAmount, 0f);
		}
	}

	// Token: 0x0400024B RID: 587
	public int updateOrder;

	// Token: 0x0400024C RID: 588
	public Camera uiCamera;

	// Token: 0x0400024D RID: 589
	public float yawAmount = 20f;

	// Token: 0x0400024E RID: 590
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform mTrans;
}
