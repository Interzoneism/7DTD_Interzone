using System;
using UnityEngine;

// Token: 0x02001300 RID: 4864
public class vp_SecurityCamTurret : vp_SimpleAITurret
{
	// Token: 0x06009786 RID: 38790 RVA: 0x003C4EC8 File Offset: 0x003C30C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		this.m_Transform = base.transform;
		this.m_AngleBob = base.gameObject.AddComponent<vp_AngleBob>();
		this.m_AngleBob.BobAmp.y = this.SwivelAmp;
		this.m_AngleBob.BobRate.y = this.SwivelRate;
		this.m_AngleBob.YOffset = this.SwivelOffset;
		this.m_AngleBob.FadeToTarget = true;
		this.SwivelRotation = this.Swivel.transform.eulerAngles;
	}

	// Token: 0x06009787 RID: 38791 RVA: 0x003C4F58 File Offset: 0x003C3158
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		if (this.m_Target != null && this.m_AngleBob.enabled)
		{
			this.m_AngleBob.enabled = false;
			this.vp_ResumeSwivelTimer.Cancel();
		}
		if (this.m_Target == null && !this.m_AngleBob.enabled && !this.vp_ResumeSwivelTimer.Active)
		{
			vp_Timer.In(this.WakeInterval * 2f, delegate()
			{
				this.m_AngleBob.enabled = true;
			}, this.vp_ResumeSwivelTimer);
		}
		this.SwivelRotation.y = this.m_Transform.eulerAngles.y;
		this.Swivel.transform.eulerAngles = this.SwivelRotation;
	}

	// Token: 0x040073E9 RID: 29673
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_AngleBob m_AngleBob;

	// Token: 0x040073EA RID: 29674
	public GameObject Swivel;

	// Token: 0x040073EB RID: 29675
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 SwivelRotation = Vector3.zero;

	// Token: 0x040073EC RID: 29676
	public float SwivelAmp = 100f;

	// Token: 0x040073ED RID: 29677
	public float SwivelRate = 0.5f;

	// Token: 0x040073EE RID: 29678
	public float SwivelOffset;

	// Token: 0x040073EF RID: 29679
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_Timer.Handle vp_ResumeSwivelTimer = new vp_Timer.Handle();
}
