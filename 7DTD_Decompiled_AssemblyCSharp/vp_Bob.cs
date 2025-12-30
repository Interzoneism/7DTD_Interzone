using System;
using UnityEngine;

// Token: 0x020012CB RID: 4811
public class vp_Bob : MonoBehaviour
{
	// Token: 0x060095F0 RID: 38384 RVA: 0x003BA64D File Offset: 0x003B884D
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Transform = base.transform;
		this.m_InitialPosition = this.m_Transform.position;
	}

	// Token: 0x060095F1 RID: 38385 RVA: 0x003BA66C File Offset: 0x003B886C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		this.m_Transform.position = this.m_InitialPosition;
		if (this.RandomizeBobOffset)
		{
			this.BobOffset = UnityEngine.Random.value;
		}
	}

	// Token: 0x060095F2 RID: 38386 RVA: 0x003BA694 File Offset: 0x003B8894
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		if (this.BobRate.x != 0f && this.BobAmp.x != 0f)
		{
			this.m_Offset.x = vp_MathUtility.Sinus(this.BobRate.x, this.BobAmp.x, this.BobOffset);
		}
		if (this.BobRate.y != 0f && this.BobAmp.y != 0f)
		{
			this.m_Offset.y = vp_MathUtility.Sinus(this.BobRate.y, this.BobAmp.y, this.BobOffset);
		}
		if (this.BobRate.z != 0f && this.BobAmp.z != 0f)
		{
			this.m_Offset.z = vp_MathUtility.Sinus(this.BobRate.z, this.BobAmp.z, this.BobOffset);
		}
		if (!this.LocalMotion)
		{
			this.m_Transform.position = this.m_InitialPosition + this.m_Offset + Vector3.up * this.GroundOffset;
			return;
		}
		this.m_Transform.position = this.m_InitialPosition + Vector3.up * this.GroundOffset;
		this.m_Transform.localPosition += this.m_Transform.TransformDirection(this.m_Offset);
	}

	// Token: 0x0400721A RID: 29210
	public Vector3 BobAmp = new Vector3(0f, 0.1f, 0f);

	// Token: 0x0400721B RID: 29211
	public Vector3 BobRate = new Vector3(0f, 4f, 0f);

	// Token: 0x0400721C RID: 29212
	public float BobOffset;

	// Token: 0x0400721D RID: 29213
	public float GroundOffset;

	// Token: 0x0400721E RID: 29214
	public bool RandomizeBobOffset;

	// Token: 0x0400721F RID: 29215
	public bool LocalMotion;

	// Token: 0x04007220 RID: 29216
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Transform;

	// Token: 0x04007221 RID: 29217
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_InitialPosition;

	// Token: 0x04007222 RID: 29218
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_Offset;
}
