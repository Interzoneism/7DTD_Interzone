using System;
using UnityEngine;

// Token: 0x020012D1 RID: 4817
public class vp_Spin : MonoBehaviour
{
	// Token: 0x06009610 RID: 38416 RVA: 0x003BB679 File Offset: 0x003B9879
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
		this.m_Transform = base.transform;
	}

	// Token: 0x06009611 RID: 38417 RVA: 0x003BB687 File Offset: 0x003B9887
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		this.m_Transform.Rotate(this.RotationSpeed * Time.deltaTime);
	}

	// Token: 0x04007238 RID: 29240
	public Vector3 RotationSpeed = new Vector3(0f, 90f, 0f);

	// Token: 0x04007239 RID: 29241
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_Transform;
}
