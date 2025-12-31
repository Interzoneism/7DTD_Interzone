using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D6D RID: 3437
[Preserve]
public class XUiC_PartyWindow : XUiController
{
	// Token: 0x06006B6A RID: 27498 RVA: 0x002BEFC8 File Offset: 0x002BD1C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "partyvisible")
		{
			_value = ((this.player != null) ? (this.player.Party != null && !this.playerDead).ToString() : "false");
			return true;
		}
		if (_bindingName == "isleader")
		{
			_value = ((this.player != null) ? (this.player.Party != null && this.player.Party.Leader == this.player).ToString() : "false");
			return true;
		}
		if (_bindingName == "voicevisible")
		{
			_value = VoiceHelpers.InAnyVoiceChat.ToString();
			return true;
		}
		if (!(_bindingName == "voiceactive"))
		{
			return false;
		}
		_value = this.voiceActive.ToString();
		return true;
	}

	// Token: 0x06006B6B RID: 27499 RVA: 0x002BF0BC File Offset: 0x002BD2BC
	public override void OnOpen()
	{
		base.OnOpen();
		this.player = base.xui.playerUI.entityPlayer;
		base.RefreshBindings(true);
		this.player.PartyJoined += this.Player_PartyChanged;
		this.player.PartyChanged += this.Player_PartyChanged;
		this.player.PartyLeave += this.Player_PartyChanged;
		this.playerDead = base.xui.playerUI.entityPlayer.IsDead();
	}

	// Token: 0x06006B6C RID: 27500 RVA: 0x002BF14C File Offset: 0x002BD34C
	public override void OnClose()
	{
		base.OnClose();
		this.player.PartyJoined -= this.Player_PartyChanged;
		this.player.PartyChanged -= this.Player_PartyChanged;
		this.player.PartyLeave -= this.Player_PartyChanged;
	}

	// Token: 0x06006B6D RID: 27501 RVA: 0x002BF1A4 File Offset: 0x002BD3A4
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.player == null)
		{
			return;
		}
		float time = Time.time;
		bool flag = time > this.updateTime;
		bool flag2 = this.player.IsDead();
		bool flag3 = flag2 != this.playerDead;
		this.playerDead = flag2;
		if (flag || flag3 || this.updateVoiceState())
		{
			this.updateTime = time + 1f;
			base.RefreshBindings(true);
		}
	}

	// Token: 0x06006B6E RID: 27502 RVA: 0x002BF216 File Offset: 0x002BD416
	[PublicizedFrom(EAccessModifier.Private)]
	public void Player_PartyChanged(Party _affectedParty, EntityPlayer _player)
	{
		base.RefreshBindings(true);
	}

	// Token: 0x06006B6F RID: 27503 RVA: 0x002BF220 File Offset: 0x002BD420
	[PublicizedFrom(EAccessModifier.Private)]
	public bool updateVoiceState()
	{
		if (GameManager.IsDedicatedServer)
		{
			return false;
		}
		if (GameStats.GetInt(EnumGameStats.GameState) != 1)
		{
			return false;
		}
		if (this.player == null)
		{
			return false;
		}
		bool flag = VoiceHelpers.LocalPlayerTalking();
		if (flag == this.voiceActive)
		{
			return false;
		}
		this.voiceActive = flag;
		return true;
	}

	// Token: 0x040051A3 RID: 20899
	[PublicizedFrom(EAccessModifier.Private)]
	public const float ForcedRefreshTime = 1f;

	// Token: 0x040051A4 RID: 20900
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x040051A5 RID: 20901
	[PublicizedFrom(EAccessModifier.Private)]
	public bool voiceActive;

	// Token: 0x040051A6 RID: 20902
	[PublicizedFrom(EAccessModifier.Private)]
	public bool playerDead;

	// Token: 0x040051A7 RID: 20903
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateTime;
}
