using System;
using UnityEngine;

// Token: 0x020012D0 RID: 4816
[RequireComponent(typeof(Rigidbody))]
public class vp_RigidbodyImpulse_random : MonoBehaviour
{
	// Token: 0x0600960D RID: 38413 RVA: 0x003BB545 File Offset: 0x003B9745
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600960E RID: 38414 RVA: 0x003BB554 File Offset: 0x003B9754
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.m_Rigidbody == null)
		{
			return;
		}
		Vector3 vector = new Vector3(UnityEngine.Random.Range(this.minForce.x, this.maxForce.x), UnityEngine.Random.Range(this.minForce.y, this.maxForce.y), UnityEngine.Random.Range(this.minForce.z, this.maxForce.z));
		float num = UnityEngine.Random.Range(this.minRigidBodySpin, this.maxRigidBodySpin);
		if (vector != Vector3.zero)
		{
			this.m_Rigidbody.AddForce(vector, ForceMode.Impulse);
		}
		if (num != 0f)
		{
			this.m_Rigidbody.AddTorque(UnityEngine.Random.rotation.eulerAngles * num);
		}
	}

	// Token: 0x04007233 RID: 29235
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Rigidbody m_Rigidbody;

	// Token: 0x04007234 RID: 29236
	public float minRigidBodySpin = 0.2f;

	// Token: 0x04007235 RID: 29237
	public float maxRigidBodySpin = 0.2f;

	// Token: 0x04007236 RID: 29238
	public Vector3 minForce = new Vector3(0f, 0f, 0f);

	// Token: 0x04007237 RID: 29239
	public Vector3 maxForce = new Vector3(0f, 0f, 0f);
}
