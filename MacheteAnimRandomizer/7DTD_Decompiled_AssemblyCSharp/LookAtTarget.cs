using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
[AddComponentMenu("NGUI/Examples/Look At Target")]
public class LookAtTarget : MonoBehaviour
{
	// Token: 0x06000183 RID: 387 RVA: 0x0000EBE0 File Offset: 0x0000CDE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.mTrans = base.transform;
	}

	// Token: 0x06000184 RID: 388 RVA: 0x0000EBF0 File Offset: 0x0000CDF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.target != null)
		{
			Vector3 forward = this.target.position - this.mTrans.position;
			if (forward.magnitude > 0.001f)
			{
				Quaternion b = Quaternion.LookRotation(forward);
				this.mTrans.rotation = Quaternion.Slerp(this.mTrans.rotation, b, Mathf.Clamp01(this.speed * Time.deltaTime));
			}
		}
	}

	// Token: 0x04000230 RID: 560
	public int level;

	// Token: 0x04000231 RID: 561
	public Transform target;

	// Token: 0x04000232 RID: 562
	public float speed = 8f;

	// Token: 0x04000233 RID: 563
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform mTrans;
}
