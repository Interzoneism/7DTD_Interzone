using System;
using UnityEngine;

// Token: 0x0200004F RID: 79
[AddComponentMenu("NGUI/Examples/Pan With Mouse")]
public class PanWithMouse : MonoBehaviour
{
	// Token: 0x06000188 RID: 392 RVA: 0x0000ECB3 File Offset: 0x0000CEB3
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.mTrans = base.transform;
		this.mStart = this.mTrans.localRotation;
	}

	// Token: 0x06000189 RID: 393 RVA: 0x0000ECD4 File Offset: 0x0000CED4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		float deltaTime = RealTime.deltaTime;
		Vector3 vector = UICamera.lastEventPosition;
		float num = (float)Screen.width * 0.5f;
		float num2 = (float)Screen.height * 0.5f;
		if (this.range < 0.1f)
		{
			this.range = 0.1f;
		}
		float x = Mathf.Clamp((vector.x - num) / num / this.range, -1f, 1f);
		float y = Mathf.Clamp((vector.y - num2) / num2 / this.range, -1f, 1f);
		this.mRot = Vector2.Lerp(this.mRot, new Vector2(x, y), deltaTime * 5f);
		this.mTrans.localRotation = this.mStart * Quaternion.Euler(-this.mRot.y * this.degrees.y, this.mRot.x * this.degrees.x, 0f);
	}

	// Token: 0x04000234 RID: 564
	public Vector2 degrees = new Vector2(5f, 3f);

	// Token: 0x04000235 RID: 565
	public float range = 1f;

	// Token: 0x04000236 RID: 566
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform mTrans;

	// Token: 0x04000237 RID: 567
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion mStart;

	// Token: 0x04000238 RID: 568
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 mRot = Vector2.zero;
}
