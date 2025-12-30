using System;
using UnityEngine;

// Token: 0x02001305 RID: 4869
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(vp_DamageHandler))]
public class vp_Grenade : MonoBehaviour
{
	// Token: 0x060097C4 RID: 38852 RVA: 0x003C621E File Offset: 0x003C441E
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060097C5 RID: 38853 RVA: 0x003C622C File Offset: 0x003C442C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		vp_Timer.In(this.LifeTime, delegate()
		{
			base.transform.SendMessage("DieBySources", new Transform[]
			{
				this.m_Source,
				this.m_OriginalSource
			}, SendMessageOptions.DontRequireReceiver);
		}, null);
	}

	// Token: 0x060097C6 RID: 38854 RVA: 0x003C6248 File Offset: 0x003C4448
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.m_Rigidbody == null)
		{
			return;
		}
		if (this.RigidbodyForce != 0f)
		{
			this.m_Rigidbody.AddForce(base.transform.forward * this.RigidbodyForce, ForceMode.Impulse);
		}
		if (this.RigidbodySpin != 0f)
		{
			this.m_Rigidbody.AddTorque(UnityEngine.Random.rotation.eulerAngles * this.RigidbodySpin);
		}
	}

	// Token: 0x060097C7 RID: 38855 RVA: 0x003C62C3 File Offset: 0x003C44C3
	public void SetSource(Transform source)
	{
		this.m_Source = base.transform;
		this.m_OriginalSource = source;
	}

	// Token: 0x0400742C RID: 29740
	public float LifeTime = 3f;

	// Token: 0x0400742D RID: 29741
	public float RigidbodyForce = 10f;

	// Token: 0x0400742E RID: 29742
	public float RigidbodySpin;

	// Token: 0x0400742F RID: 29743
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Rigidbody m_Rigidbody;

	// Token: 0x04007430 RID: 29744
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Source;

	// Token: 0x04007431 RID: 29745
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_OriginalSource;
}
