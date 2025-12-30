using System;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D6A RID: 3434
[Preserve]
public class XUiC_PartyEntry : XUiController
{
	// Token: 0x17000AC9 RID: 2761
	// (get) Token: 0x06006B51 RID: 27473 RVA: 0x002BDAEB File Offset: 0x002BBCEB
	// (set) Token: 0x06006B52 RID: 27474 RVA: 0x002BDAF3 File Offset: 0x002BBCF3
	public EntityPlayer Player { get; set; }

	// Token: 0x06006B53 RID: 27475 RVA: 0x002BDAFC File Offset: 0x002BBCFC
	public override void Init()
	{
		base.Init();
		this.IsDirty = true;
		XUiController[] childrenById = base.GetChildrenById("BarHealth", null);
		if (childrenById != null)
		{
			this.barHealth = new XUiV_Sprite[childrenById.Length];
			for (int i = 0; i < childrenById.Length; i++)
			{
				this.barHealth[i] = (XUiV_Sprite)childrenById[i].ViewComponent;
			}
		}
		childrenById = base.GetChildrenById("BarHealthModifiedMax", null);
		if (childrenById != null)
		{
			this.barHealthModifiedMax = new XUiV_Sprite[childrenById.Length];
			for (int j = 0; j < childrenById.Length; j++)
			{
				this.barHealthModifiedMax[j] = (XUiV_Sprite)childrenById[j].ViewComponent;
			}
		}
		childrenById = base.GetChildrenById("BarStamina", null);
		if (childrenById != null)
		{
			this.barStamina = new XUiV_Sprite[childrenById.Length];
			for (int k = 0; k < childrenById.Length; k++)
			{
				this.barStamina[k] = (XUiV_Sprite)childrenById[k].ViewComponent;
			}
		}
		childrenById = base.GetChildrenById("BarStaminaModifiedMax", null);
		if (childrenById != null)
		{
			this.barStaminaModifiedMax = new XUiV_Sprite[childrenById.Length];
			for (int l = 0; l < childrenById.Length; l++)
			{
				this.barStaminaModifiedMax[l] = (XUiV_Sprite)childrenById[l].ViewComponent;
			}
		}
		XUiController childById = base.GetChildById("arrowContent");
		if (childById != null)
		{
			this.arrowContent = (XUiV_Sprite)childById.ViewComponent;
		}
		XUiController childById2 = base.GetChildById("icon1");
		if (childById2 != null)
		{
			this.iconSprite1 = (XUiV_Sprite)childById2.ViewComponent;
			this.defaultIconColor = this.iconSprite1.Color;
			this.iconSpriteSize = new Vector2((float)this.iconSprite1.Sprite.width, (float)this.iconSprite1.Sprite.height);
		}
		childById2 = base.GetChildById("icon2");
		if (childById2 != null)
		{
			this.iconSprite2 = (XUiV_Sprite)childById2.ViewComponent;
		}
	}

	// Token: 0x06006B54 RID: 27476 RVA: 0x002BDCCC File Offset: 0x002BBECC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.Player == null || !XUi.IsGameRunning())
		{
			return;
		}
		this.RefreshFill();
		this.updateVoiceState();
		if (Time.time > this.updateTime)
		{
			this.updateTime = Time.time + 0.5f;
			if (this.HasChanged() || this.IsDirty)
			{
				if (this.IsDirty)
				{
					base.RefreshBindings(true);
					this.IsDirty = false;
				}
				else
				{
					base.RefreshBindings(false);
				}
			}
		}
		if (this.Player != null && this.arrowContent != null)
		{
			this.arrowRotation = this.ReturnRotation(base.xui.playerUI.entityPlayer, this.Player);
			if (this.lastArrowRotation < 15f && this.arrowRotation > 345f)
			{
				this.lastArrowRotation = this.arrowRotation;
			}
			else if (this.lastArrowRotation > 345f && this.arrowRotation < 15f)
			{
				this.lastArrowRotation = this.arrowRotation;
			}
			else
			{
				this.lastArrowRotation = Mathf.Lerp(this.lastArrowRotation, this.arrowRotation, _dt * 3f);
			}
			this.arrowContent.UiTransform.localEulerAngles = new Vector3(0f, 0f, this.lastArrowRotation - 180f);
		}
		if (this.Player != null)
		{
			if (this.Player.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.TempDisabledEnding)
			{
				float num = Mathf.PingPong(Time.time, 0.5f);
				float num2 = 1f;
				if (num > 0.25f)
				{
					num2 = 1f + num - 0.25f;
				}
				if (this.Player.Party.Leader == this.Player)
				{
					this.iconSprite2.Color = Color.Lerp(this.defaultIconColor, this.iconBlinkColor, num * 4f);
					this.iconSprite2.Sprite.SetDimensions((int)(this.iconSpriteSize.x * num2), (int)(this.iconSpriteSize.y * num2));
					return;
				}
				this.iconSprite1.Color = Color.Lerp(this.defaultIconColor, this.iconBlinkColor, num * 4f);
				this.iconSprite1.Sprite.SetDimensions((int)(this.iconSpriteSize.x * num2), (int)(this.iconSpriteSize.y * num2));
				return;
			}
			else
			{
				this.iconSprite1.Color = this.defaultIconColor;
				this.iconSprite2.Color = this.defaultIconColor;
			}
		}
	}

	// Token: 0x06006B55 RID: 27477 RVA: 0x002BDF54 File Offset: 0x002BC154
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateVoiceState()
	{
		IPartyVoice.EVoiceMemberState playerVoiceState = VoiceHelpers.GetPlayerVoiceState(this.Player, true);
		if (playerVoiceState != this.voiceState)
		{
			this.voiceState = playerVoiceState;
			this.IsDirty = true;
		}
	}

	// Token: 0x06006B56 RID: 27478 RVA: 0x002BDF85 File Offset: 0x002BC185
	[PublicizedFrom(EAccessModifier.Internal)]
	public void SetPlayer(EntityPlayer player)
	{
		this.Player = player;
		if (this.Player == null)
		{
			base.RefreshBindings(true);
			return;
		}
		this.IsDirty = true;
	}

	// Token: 0x06006B57 RID: 27479 RVA: 0x002BDFAC File Offset: 0x002BC1AC
	public bool HasChanged()
	{
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		float magnitude = (this.Player.GetPosition() - entityPlayer.GetPosition()).magnitude;
		bool result = this.oldValue != magnitude || this.Player.TwitchEnabled != this.oldTwitch || this.Player.TwitchSafe != this.oldSafe || this.Player.TwitchActionsEnabled != this.oldTwitchActions;
		this.oldValue = magnitude;
		this.distance = magnitude;
		this.oldTwitch = this.Player.TwitchEnabled;
		this.oldSafe = this.Player.TwitchSafe;
		this.oldTwitchActions = this.Player.TwitchActionsEnabled;
		return result;
	}

	// Token: 0x06006B58 RID: 27480 RVA: 0x002BE074 File Offset: 0x002BC274
	public void RefreshFill()
	{
		if (this.Player == null)
		{
			return;
		}
		float t = Time.deltaTime * 3f;
		if (this.barHealth != null)
		{
			float valuePercentUI = this.Player.Stats.Health.ValuePercentUI;
			float fill = Math.Max(this.lastHealthValue, 0f) * 1.01f;
			this.lastHealthValue = Mathf.Lerp(this.lastHealthValue, valuePercentUI, t);
			for (int i = 0; i < this.barHealth.Length; i++)
			{
				this.barHealth[i].Fill = fill;
			}
		}
		if (this.barHealthModifiedMax != null)
		{
			for (int j = 0; j < this.barHealthModifiedMax.Length; j++)
			{
				this.barHealthModifiedMax[j].Fill = this.Player.Stats.Health.ModifiedMax / this.Player.Stats.Health.Max;
			}
		}
		if (this.barStamina != null)
		{
			float valuePercentUI2 = this.Player.Stats.Stamina.ValuePercentUI;
			float fill2 = Math.Max(this.lastStaminaValue, 0f) * 1.01f;
			this.lastStaminaValue = Mathf.Lerp(this.lastStaminaValue, valuePercentUI2, t);
			for (int k = 0; k < this.barStamina.Length; k++)
			{
				this.barStamina[k].Fill = fill2;
			}
		}
		if (this.barStaminaModifiedMax != null)
		{
			for (int l = 0; l < this.barStaminaModifiedMax.Length; l++)
			{
				this.barStaminaModifiedMax[l].Fill = this.Player.Stats.Stamina.ModifiedMax / this.Player.Stats.Stamina.Max;
			}
		}
	}

	// Token: 0x06006B59 RID: 27481 RVA: 0x002BE22C File Offset: 0x002BC42C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 1810125418U)
		{
			if (num <= 833193568U)
			{
				if (num <= 679423209U)
				{
					if (num != 365230134U)
					{
						if (num == 679423209U)
						{
							if (bindingName == "arrowcolor")
							{
								if (this.Player == null)
								{
									value = "";
									return true;
								}
								int num2 = this.Player.Party.MemberList.IndexOf(this.Player);
								Color32 v = Constants.TrackedFriendColors[num2 % Constants.TrackedFriendColors.Length];
								value = this.arrowcolorFormatter.Format(v);
								return true;
							}
						}
					}
					else if (bindingName == "healthfill")
					{
						if (this.Player == null)
						{
							value = "0";
							return true;
						}
						float valuePercentUI = this.Player.Stats.Health.ValuePercentUI;
						value = this.healthfillFormatter.Format(valuePercentUI);
						return true;
					}
				}
				else if (num != 783488098U)
				{
					if (num == 833193568U)
					{
						if (bindingName == "healthcurrent")
						{
							if (this.Player == null)
							{
								value = "";
								return true;
							}
							value = this.healthcurrentFormatter.Format(this.Player.Health);
							return true;
						}
					}
				}
				else if (bindingName == "distance")
				{
					if (this.Player == null)
					{
						value = "";
						return true;
					}
					value = this.distanceFormatter.Format(this.distance);
					return true;
				}
			}
			else if (num <= 954351718U)
			{
				if (num != 937574099U)
				{
					if (num == 954351718U)
					{
						if (bindingName == "icon2")
						{
							if (this.Player == null || GameStats.GetBool(EnumGameStats.AutoParty))
							{
								value = "";
								return true;
							}
							if (!this.Player.IsDead() && !this.Player.IsPartyLead())
							{
								value = "";
							}
							else if (this.Player.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled)
							{
								value = this.twitchDisabledIcon;
							}
							else if (this.Player.TwitchSafe)
							{
								value = this.twitchSafeIcon;
							}
							else
							{
								value = "";
							}
							return true;
						}
					}
				}
				else if (bindingName == "icon1")
				{
					if (this.Player == null || GameStats.GetBool(EnumGameStats.AutoParty))
					{
						value = "";
						return true;
					}
					if (this.Player.IsDead())
					{
						value = this.deathIcon;
					}
					else if (this.Player.IsPartyLead())
					{
						value = this.leaderIcon;
					}
					else if (this.Player.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled)
					{
						value = this.twitchDisabledIcon;
					}
					else if (this.Player.TwitchSafe)
					{
						value = this.twitchSafeIcon;
					}
					else
					{
						value = "";
					}
					return true;
				}
			}
			else if (num != 1339791005U)
			{
				if (num != 1766231739U)
				{
					if (num == 1810125418U)
					{
						if (bindingName == "healthmodifiedmax")
						{
							if (this.Player == null || base.xui.playerUI.entityPlayer.IsDead())
							{
								value = "0";
								return true;
							}
							value = (this.Player.Stats.Health.ModifiedMax / this.Player.Stats.Health.Max).ToCultureInvariantString();
							return true;
						}
					}
				}
				else if (bindingName == "voiceactive")
				{
					value = (this.voiceState == IPartyVoice.EVoiceMemberState.VoiceActive).ToString();
					return true;
				}
			}
			else if (bindingName == "showarrow")
			{
				if (this.Player == null)
				{
					value = "false";
					return true;
				}
				value = this.Player.IsAlive().ToString();
				return true;
			}
		}
		else if (num <= 2535347189U)
		{
			if (num <= 2369371622U)
			{
				if (num != 1926871326U)
				{
					if (num == 2369371622U)
					{
						if (bindingName == "name")
						{
							if (this.Player == null)
							{
								value = "";
								return true;
							}
							value = GameUtils.SafeStringFormat(this.Player.PlayerDisplayName);
							return true;
						}
					}
				}
				else if (bindingName == "healthcolor")
				{
					if (this.Player == null)
					{
						value = "";
						return true;
					}
					value = (this.Player.TwitchEnabled ? this.twitchHealthColor : this.defaultHealthColor);
					return true;
				}
			}
			else if (num != 2529325253U)
			{
				if (num == 2535347189U)
				{
					if (bindingName == "distancecolor")
					{
						Color32 v2 = Color.white;
						if (this.Player == null)
						{
							value = "";
							return true;
						}
						if (this.distance > 100f)
						{
							v2 = Color.grey;
						}
						value = this.itemicontintcolorFormatter.Format(v2);
						return true;
					}
				}
			}
			else if (bindingName == "voicevisible")
			{
				value = (this.voiceState > IPartyVoice.EVoiceMemberState.Disabled).ToString();
				return true;
			}
		}
		else if (num <= 2966955437U)
		{
			if (num != 2916622580U)
			{
				if (num == 2966955437U)
				{
					if (bindingName == "showicon2")
					{
						if (this.Player == null)
						{
							value = "false";
							return true;
						}
						value = ((this.Player.IsPartyLead() || this.Player.IsDead()) && (this.Player.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled || this.Player.TwitchSafe) && this.Player.HasTwitchMember()).ToString();
						return true;
					}
				}
			}
			else if (bindingName == "showicon1")
			{
				if (this.Player == null)
				{
					value = "false";
					return true;
				}
				value = (this.Player.IsPartyLead() || this.Player.IsDead() || ((this.Player.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled || this.Player.TwitchSafe) && this.Player.HasTwitchMember())).ToString();
				return true;
			}
		}
		else if (num != 3004043574U)
		{
			if (num != 3388762708U)
			{
				if (num == 3910386971U)
				{
					if (bindingName == "partyvisible")
					{
						if (this.Player == null || base.xui.playerUI.entityPlayer.IsDead())
						{
							value = "false";
							return true;
						}
						value = "true";
						return true;
					}
				}
			}
			else if (bindingName == "voicemuted")
			{
				value = (this.voiceState == IPartyVoice.EVoiceMemberState.Muted).ToString();
				return true;
			}
		}
		else if (bindingName == "healthcurrentwithmax")
		{
			if (this.Player == null)
			{
				value = "";
				return true;
			}
			value = this.healthcurrentWMaxFormatter.Format(this.Player.Health, this.Player.GetMaxHealth());
			return true;
		}
		return false;
	}

	// Token: 0x06006B5A RID: 27482 RVA: 0x002BE9B8 File Offset: 0x002BCBB8
	public XUiC_PartyEntry()
	{
		this.oldTwitch = false;
	}

	// Token: 0x06006B5B RID: 27483 RVA: 0x002BEAB0 File Offset: 0x002BCCB0
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
		if (num <= 1003795756U)
		{
			if (num != 547217411U)
			{
				if (num != 933825086U)
				{
					if (num == 1003795756U)
					{
						if (name == "leader_icon")
						{
							this.leaderIcon = value;
							return true;
						}
					}
				}
				else if (name == "twitch_icon")
				{
					this.twitchActiveIcon = value;
					return true;
				}
			}
			else if (name == "death_icon")
			{
				this.deathIcon = value;
				return true;
			}
		}
		else if (num <= 3319615829U)
		{
			if (num != 2710568369U)
			{
				if (num == 3319615829U)
				{
					if (name == "twitch_health_color")
					{
						this.twitchHealthColor = value;
						return true;
					}
				}
			}
			else if (name == "default_health_color")
			{
				this.defaultHealthColor = value;
				return true;
			}
		}
		else if (num != 3463996801U)
		{
			if (num == 4202079582U)
			{
				if (name == "twitch_safe_icon")
				{
					this.twitchSafeIcon = value;
					return true;
				}
			}
		}
		else if (name == "twitch_disabled_icon")
		{
			this.twitchDisabledIcon = value;
			return true;
		}
		return base.ParseAttribute(name, value, _parent);
	}

	// Token: 0x06006B5C RID: 27484 RVA: 0x00277AB3 File Offset: 0x00275CB3
	public override void OnOpen()
	{
		base.OnOpen();
		this.IsDirty = true;
		base.RefreshBindings(true);
	}

	// Token: 0x06006B5D RID: 27485 RVA: 0x002BEBD4 File Offset: 0x002BCDD4
	public override void OnClose()
	{
		base.OnClose();
		this.SetPlayer(null);
	}

	// Token: 0x06006B5E RID: 27486 RVA: 0x002BEBE4 File Offset: 0x002BCDE4
	[PublicizedFrom(EAccessModifier.Private)]
	public float ReturnRotation(EntityAlive _self, EntityAlive _other)
	{
		Transform transform = _self.transform;
		Vector3 forward = transform.forward;
		Vector2 vector = new Vector2(forward.x, forward.z);
		Vector3 normalized = (transform.position - _other.transform.position).normalized;
		Vector2 vector2 = new Vector2(normalized.x, normalized.z);
		Vector3 vector3 = Vector3.Cross(vector, vector2);
		float num = Vector2.Angle(vector, vector2);
		if (vector3.z < 0f)
		{
			num = 360f - num;
		}
		return num;
	}

	// Token: 0x0400517C RID: 20860
	[PublicizedFrom(EAccessModifier.Private)]
	public const float ForcedRefreshTime = 0.5f;

	// Token: 0x0400517D RID: 20861
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastHealthValue;

	// Token: 0x0400517E RID: 20862
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastStaminaValue;

	// Token: 0x04005180 RID: 20864
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite arrowContent;

	// Token: 0x04005181 RID: 20865
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite[] barHealth;

	// Token: 0x04005182 RID: 20866
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite[] barHealthModifiedMax;

	// Token: 0x04005183 RID: 20867
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite[] barStamina;

	// Token: 0x04005184 RID: 20868
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite[] barStaminaModifiedMax;

	// Token: 0x04005185 RID: 20869
	[PublicizedFrom(EAccessModifier.Private)]
	public float distance;

	// Token: 0x04005186 RID: 20870
	public string defaultHealthColor = "255,0,0,128";

	// Token: 0x04005187 RID: 20871
	public string twitchHealthColor = "100,65,165,128";

	// Token: 0x04005188 RID: 20872
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite iconSprite1;

	// Token: 0x04005189 RID: 20873
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite iconSprite2;

	// Token: 0x0400518A RID: 20874
	[PublicizedFrom(EAccessModifier.Private)]
	public Color defaultIconColor;

	// Token: 0x0400518B RID: 20875
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2 iconSpriteSize;

	// Token: 0x0400518C RID: 20876
	[PublicizedFrom(EAccessModifier.Private)]
	public Color iconBlinkColor = new Color32(byte.MaxValue, 180, 0, byte.MaxValue);

	// Token: 0x0400518D RID: 20877
	[PublicizedFrom(EAccessModifier.Protected)]
	public float updateTime;

	// Token: 0x0400518E RID: 20878
	[PublicizedFrom(EAccessModifier.Private)]
	public float arrowRotation;

	// Token: 0x0400518F RID: 20879
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastArrowRotation;

	// Token: 0x04005190 RID: 20880
	[PublicizedFrom(EAccessModifier.Private)]
	public IPartyVoice.EVoiceMemberState voiceState;

	// Token: 0x04005191 RID: 20881
	[PublicizedFrom(EAccessModifier.Private)]
	public float oldValue;

	// Token: 0x04005192 RID: 20882
	[PublicizedFrom(EAccessModifier.Private)]
	public bool oldTwitch;

	// Token: 0x04005193 RID: 20883
	[PublicizedFrom(EAccessModifier.Private)]
	public bool oldSafe;

	// Token: 0x04005194 RID: 20884
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer.TwitchActionsStates oldTwitchActions;

	// Token: 0x04005195 RID: 20885
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt healthcurrentFormatter = new CachedStringFormatterInt();

	// Token: 0x04005196 RID: 20886
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int, int> healthcurrentWMaxFormatter = new CachedStringFormatter<int, int>((int _i, int _i2) => string.Format("{0}/{1}", _i, _i2));

	// Token: 0x04005197 RID: 20887
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat healthfillFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x04005198 RID: 20888
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<float> distanceFormatter = new CachedStringFormatter<float>(new Func<float, string>(ValueDisplayFormatters.Distance));

	// Token: 0x04005199 RID: 20889
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor itemicontintcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x0400519A RID: 20890
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor arrowcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x0400519B RID: 20891
	public string deathIcon = "ui_game_symbol_death";

	// Token: 0x0400519C RID: 20892
	public string leaderIcon = "server_favorite";

	// Token: 0x0400519D RID: 20893
	public string twitchActiveIcon = "ui_game_symbol_twitch_actions";

	// Token: 0x0400519E RID: 20894
	public string twitchDisabledIcon = "ui_game_symbol_twitch_action_disabled";

	// Token: 0x0400519F RID: 20895
	public string twitchSafeIcon = "ui_game_symbol_brick";
}
