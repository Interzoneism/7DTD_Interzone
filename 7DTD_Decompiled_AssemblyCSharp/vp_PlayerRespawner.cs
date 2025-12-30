using System;
using UnityEngine;

// Token: 0x02001363 RID: 4963
public class vp_PlayerRespawner : vp_Respawner
{
	// Token: 0x1700101F RID: 4127
	// (get) Token: 0x06009B53 RID: 39763 RVA: 0x003DD009 File Offset: 0x003DB209
	public vp_PlayerEventHandler Player
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_Player == null)
			{
				this.m_Player = base.transform.GetComponent<vp_PlayerEventHandler>();
			}
			return this.m_Player;
		}
	}

	// Token: 0x06009B54 RID: 39764 RVA: 0x003DD030 File Offset: 0x003DB230
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06009B55 RID: 39765 RVA: 0x003DD038 File Offset: 0x003DB238
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEnable()
	{
		if (this.Player != null)
		{
			this.Player.Register(this);
		}
		base.OnEnable();
	}

	// Token: 0x06009B56 RID: 39766 RVA: 0x003DD05A File Offset: 0x003DB25A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnDisable()
	{
		if (this.Player != null)
		{
			this.Player.Unregister(this);
		}
	}

	// Token: 0x06009B57 RID: 39767 RVA: 0x003DD078 File Offset: 0x003DB278
	public override void Reset()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.Player == null)
		{
			return;
		}
		this.Player.Position.Set(this.Placement.Position);
		this.Player.Rotation.Set(this.Placement.Rotation.eulerAngles);
		this.Player.Stop.Send();
	}

	// Token: 0x04007823 RID: 30755
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_PlayerEventHandler m_Player;
}
