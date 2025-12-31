using System;
using UnityEngine;

// Token: 0x02001365 RID: 4965
public class vp_SimpleFiring : MonoBehaviour
{
	// Token: 0x06009B6E RID: 39790 RVA: 0x003DD7B5 File Offset: 0x003DB9B5
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Player = (vp_PlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_PlayerEventHandler));
	}

	// Token: 0x06009B6F RID: 39791 RVA: 0x003DD7DC File Offset: 0x003DB9DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
	}

	// Token: 0x06009B70 RID: 39792 RVA: 0x003DD7F8 File Offset: 0x003DB9F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}

	// Token: 0x06009B71 RID: 39793 RVA: 0x003DD814 File Offset: 0x003DBA14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		if (this.m_Player.Attack.Active)
		{
			this.m_Player.Fire.Try();
		}
	}

	// Token: 0x04007837 RID: 30775
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_PlayerEventHandler m_Player;
}
