using System;
using UnityEngine;

// Token: 0x020012C9 RID: 4809
public class vp_AngleBob : MonoBehaviour
{
	// Token: 0x060095E9 RID: 38377 RVA: 0x003BA377 File Offset: 0x003B8577
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Transform = base.transform;
		this.m_InitialRotation = this.m_Transform.eulerAngles;
	}

	// Token: 0x060095EA RID: 38378 RVA: 0x003BA396 File Offset: 0x003B8596
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		this.m_Transform.eulerAngles = this.m_InitialRotation;
		if (this.RandomizeBobOffset)
		{
			this.YOffset = UnityEngine.Random.value;
		}
	}

	// Token: 0x060095EB RID: 38379 RVA: 0x003BA3BC File Offset: 0x003B85BC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		if (this.BobRate.x != 0f && this.BobAmp.x != 0f)
		{
			this.m_Offset.x = vp_MathUtility.Sinus(this.BobRate.x, this.BobAmp.x, 0f);
		}
		if (this.BobRate.y != 0f && this.BobAmp.y != 0f)
		{
			this.m_Offset.y = vp_MathUtility.Sinus(this.BobRate.y, this.BobAmp.y, 0f);
		}
		if (this.BobRate.z != 0f && this.BobAmp.z != 0f)
		{
			this.m_Offset.z = vp_MathUtility.Sinus(this.BobRate.z, this.BobAmp.z, 0f);
		}
		if (this.LocalMotion)
		{
			this.m_Transform.eulerAngles = this.m_InitialRotation + Vector3.up * this.YOffset;
			this.m_Transform.localEulerAngles += this.m_Transform.TransformDirection(this.m_Offset);
			return;
		}
		if (this.FadeToTarget)
		{
			this.m_Transform.rotation = Quaternion.Lerp(this.m_Transform.rotation, Quaternion.Euler(this.m_InitialRotation + this.m_Offset + Vector3.up * this.YOffset), Time.deltaTime);
			return;
		}
		this.m_Transform.eulerAngles = this.m_InitialRotation + this.m_Offset + Vector3.up * this.YOffset;
	}

	// Token: 0x0400720F RID: 29199
	public Vector3 BobAmp = new Vector3(0f, 0.1f, 0f);

	// Token: 0x04007210 RID: 29200
	public Vector3 BobRate = new Vector3(0f, 4f, 0f);

	// Token: 0x04007211 RID: 29201
	public float YOffset;

	// Token: 0x04007212 RID: 29202
	public bool RandomizeBobOffset;

	// Token: 0x04007213 RID: 29203
	public bool LocalMotion;

	// Token: 0x04007214 RID: 29204
	public bool FadeToTarget;

	// Token: 0x04007215 RID: 29205
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Transform;

	// Token: 0x04007216 RID: 29206
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_InitialRotation;

	// Token: 0x04007217 RID: 29207
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_Offset;
}
