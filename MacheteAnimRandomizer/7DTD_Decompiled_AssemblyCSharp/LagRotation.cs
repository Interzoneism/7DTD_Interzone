using System;
using UnityEngine;

// Token: 0x0200004B RID: 75
[AddComponentMenu("NGUI/Examples/Lag Rotation")]
public class LagRotation : MonoBehaviour
{
	// Token: 0x0600017C RID: 380 RVA: 0x0000EAEF File Offset: 0x0000CCEF
	public void OnRepositionEnd()
	{
		this.Interpolate(1000f);
	}

	// Token: 0x0600017D RID: 381 RVA: 0x0000EAFC File Offset: 0x0000CCFC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Interpolate(float delta)
	{
		if (this.mTrans != null)
		{
			Transform parent = this.mTrans.parent;
			if (parent != null)
			{
				this.mAbsolute = Quaternion.Slerp(this.mAbsolute, parent.rotation * this.mRelative, delta * this.speed);
				this.mTrans.rotation = this.mAbsolute;
			}
		}
	}

	// Token: 0x0600017E RID: 382 RVA: 0x0000EB67 File Offset: 0x0000CD67
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.mTrans = base.transform;
		this.mRelative = this.mTrans.localRotation;
		this.mAbsolute = this.mTrans.rotation;
	}

	// Token: 0x0600017F RID: 383 RVA: 0x0000EB97 File Offset: 0x0000CD97
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		this.Interpolate(this.ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime);
	}

	// Token: 0x0400022A RID: 554
	public float speed = 10f;

	// Token: 0x0400022B RID: 555
	public bool ignoreTimeScale;

	// Token: 0x0400022C RID: 556
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform mTrans;

	// Token: 0x0400022D RID: 557
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion mRelative;

	// Token: 0x0400022E RID: 558
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion mAbsolute;
}
