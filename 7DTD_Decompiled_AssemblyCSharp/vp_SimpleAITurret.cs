using System;
using UnityEngine;

// Token: 0x02001301 RID: 4865
[RequireComponent(typeof(vp_Shooter))]
public class vp_SimpleAITurret : MonoBehaviour
{
	// Token: 0x0600978A RID: 38794 RVA: 0x003C505B File Offset: 0x003C325B
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
		this.m_Shooter = base.GetComponent<vp_Shooter>();
		this.m_Transform = base.transform;
	}

	// Token: 0x0600978B RID: 38795 RVA: 0x003C5075 File Offset: 0x003C3275
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		if (!this.m_Timer.Active)
		{
			vp_Timer.In(this.WakeInterval, delegate()
			{
				if (this.m_Target == null)
				{
					this.m_Target = this.ScanForLocalPlayer();
					return;
				}
				this.m_Target = null;
			}, this.m_Timer);
		}
		if (this.m_Target != null)
		{
			this.AttackTarget();
		}
	}

	// Token: 0x0600978C RID: 38796 RVA: 0x003C50B8 File Offset: 0x003C32B8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual Transform ScanForLocalPlayer()
	{
		foreach (Collider collider in Physics.OverlapSphere(this.m_Transform.position, this.ViewRange, 1073741824))
		{
			RaycastHit raycastHit;
			Physics.Linecast(this.m_Transform.position, collider.transform.position + Vector3.up, out raycastHit);
			if (!(raycastHit.collider != null) || !(raycastHit.collider != collider))
			{
				return collider.transform;
			}
		}
		return null;
	}

	// Token: 0x0600978D RID: 38797 RVA: 0x003C5144 File Offset: 0x003C3344
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void AttackTarget()
	{
		Quaternion to = Quaternion.LookRotation(this.m_Target.GetComponent<Collider>().bounds.center - this.m_Transform.position);
		this.m_Transform.rotation = Quaternion.RotateTowards(this.m_Transform.rotation, to, Time.deltaTime * this.AimSpeed);
		if (Mathf.Abs(vp_3DUtility.LookAtAngleHorizontal(this.m_Transform.position, this.m_Transform.forward, this.m_Target.position)) < this.FireAngle)
		{
			this.m_Shooter.TryFire();
		}
	}

	// Token: 0x040073F0 RID: 29680
	public float ViewRange = 10f;

	// Token: 0x040073F1 RID: 29681
	public float AimSpeed = 50f;

	// Token: 0x040073F2 RID: 29682
	public float WakeInterval = 2f;

	// Token: 0x040073F3 RID: 29683
	public float FireAngle = 10f;

	// Token: 0x040073F4 RID: 29684
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Shooter m_Shooter;

	// Token: 0x040073F5 RID: 29685
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Transform;

	// Token: 0x040073F6 RID: 29686
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Target;

	// Token: 0x040073F7 RID: 29687
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_Timer = new vp_Timer.Handle();
}
