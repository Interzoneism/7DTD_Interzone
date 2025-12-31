using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class LagPosition : MonoBehaviour
{
	// Token: 0x06000174 RID: 372 RVA: 0x0000E966 File Offset: 0x0000CB66
	public void OnRepositionEnd()
	{
		this.Interpolate(1000f);
	}

	// Token: 0x06000175 RID: 373 RVA: 0x0000E974 File Offset: 0x0000CB74
	[PublicizedFrom(EAccessModifier.Private)]
	public void Interpolate(float delta)
	{
		Transform parent = this.mTrans.parent;
		if (parent != null)
		{
			Vector3 vector = parent.position + parent.rotation * this.mRelative;
			this.mAbsolute.x = Mathf.Lerp(this.mAbsolute.x, vector.x, Mathf.Clamp01(delta * this.speed.x));
			this.mAbsolute.y = Mathf.Lerp(this.mAbsolute.y, vector.y, Mathf.Clamp01(delta * this.speed.y));
			this.mAbsolute.z = Mathf.Lerp(this.mAbsolute.z, vector.z, Mathf.Clamp01(delta * this.speed.z));
			this.mTrans.position = this.mAbsolute;
		}
	}

	// Token: 0x06000176 RID: 374 RVA: 0x0000EA60 File Offset: 0x0000CC60
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.mTrans = base.transform;
	}

	// Token: 0x06000177 RID: 375 RVA: 0x0000EA6E File Offset: 0x0000CC6E
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		if (this.mStarted)
		{
			this.ResetPosition();
		}
	}

	// Token: 0x06000178 RID: 376 RVA: 0x0000EA7E File Offset: 0x0000CC7E
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.mStarted = true;
		this.ResetPosition();
	}

	// Token: 0x06000179 RID: 377 RVA: 0x0000EA8D File Offset: 0x0000CC8D
	public void ResetPosition()
	{
		this.mAbsolute = this.mTrans.position;
		this.mRelative = this.mTrans.localPosition;
	}

	// Token: 0x0600017A RID: 378 RVA: 0x0000EAB1 File Offset: 0x0000CCB1
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		this.Interpolate(this.ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime);
	}

	// Token: 0x04000224 RID: 548
	public Vector3 speed = new Vector3(10f, 10f, 10f);

	// Token: 0x04000225 RID: 549
	public bool ignoreTimeScale;

	// Token: 0x04000226 RID: 550
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform mTrans;

	// Token: 0x04000227 RID: 551
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 mRelative;

	// Token: 0x04000228 RID: 552
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 mAbsolute;

	// Token: 0x04000229 RID: 553
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool mStarted;
}
