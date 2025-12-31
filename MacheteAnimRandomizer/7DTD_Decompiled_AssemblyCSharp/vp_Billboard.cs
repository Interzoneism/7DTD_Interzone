using System;
using UnityEngine;

// Token: 0x020012CA RID: 4810
public class vp_Billboard : MonoBehaviour
{
	// Token: 0x060095ED RID: 38381 RVA: 0x003BA5D0 File Offset: 0x003B87D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
		this.m_Transform = base.transform;
		if (this.m_CameraTransform == null)
		{
			this.m_CameraTransform = Camera.main.transform;
		}
	}

	// Token: 0x060095EE RID: 38382 RVA: 0x003BA5FC File Offset: 0x003B87FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		if (this.m_CameraTransform != null)
		{
			this.m_Transform.localEulerAngles = this.m_CameraTransform.eulerAngles;
		}
		this.m_Transform.localEulerAngles = this.m_Transform.localEulerAngles;
	}

	// Token: 0x04007218 RID: 29208
	public Transform m_CameraTransform;

	// Token: 0x04007219 RID: 29209
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform m_Transform;
}
