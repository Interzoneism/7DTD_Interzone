using System;
using UnityEngine;

// Token: 0x020012CF RID: 4815
[RequireComponent(typeof(Rigidbody))]
public class vp_RigidbodyImpulse : MonoBehaviour
{
	// Token: 0x0600960A RID: 38410 RVA: 0x003BB497 File Offset: 0x003B9697
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600960B RID: 38411 RVA: 0x003BB4A8 File Offset: 0x003B96A8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.m_Rigidbody == null)
		{
			return;
		}
		if (this.RigidbodyForce != Vector3.zero)
		{
			this.m_Rigidbody.AddForce(this.RigidbodyForce, ForceMode.Impulse);
		}
		if (this.RigidbodySpin != 0f)
		{
			this.m_Rigidbody.AddTorque(UnityEngine.Random.rotation.eulerAngles * this.RigidbodySpin);
		}
	}

	// Token: 0x04007230 RID: 29232
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Rigidbody m_Rigidbody;

	// Token: 0x04007231 RID: 29233
	public Vector3 RigidbodyForce = new Vector3(0f, 5f, 0f);

	// Token: 0x04007232 RID: 29234
	public float RigidbodySpin = 0.2f;
}
