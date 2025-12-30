using System;
using System.Globalization;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C3C RID: 3132
[Preserve]
public class XUiC_ChatOutput : XUiController
{
	// Token: 0x0600604B RID: 24651 RVA: 0x0027181C File Offset: 0x0026FA1C
	public override void Init()
	{
		XUiC_ChatOutput.ID = this.windowGroup.ID;
		base.Init();
		this.txtOutput = (XUiV_TextList)base.GetChildById("txtOutput").ViewComponent;
		this.collider = this.txtOutput.UiTransform.GetComponent<BoxCollider>();
	}

	// Token: 0x0600604C RID: 24652 RVA: 0x00271870 File Offset: 0x0026FA70
	[PublicizedFrom(EAccessModifier.Private)]
	public void addMessage(EnumGameMessages _messageType, EChatType _chatType, EChatDirection _chatDirection, string _message, string _senderDisplayName, string _senderHandlerId)
	{
		if (this.txtOutput == null)
		{
			return;
		}
		if (!ThreadManager.IsMainThread())
		{
			ThreadManager.AddSingleTaskMainThread("AddTextListLine", delegate(object _)
			{
				this.addMessage(_messageType, _chatType, _chatDirection, _message, _senderDisplayName, _senderHandlerId);
			}, null);
			return;
		}
		if (_messageType == EnumGameMessages.Chat)
		{
			if (_senderDisplayName == null)
			{
				_senderDisplayName = "";
			}
			if (_chatType == EChatType.Discord && !string.IsNullOrEmpty(_senderDisplayName))
			{
				_senderDisplayName += " [discord] ";
			}
			string text = string.IsNullOrEmpty(_senderDisplayName) ? "" : (XUiUtils.BuildUrlFunctionString("Chat", new ValueTuple<string, string>("ChatType", _chatType.ToStringCached<EChatType>()), new ValueTuple<string, string>("Sender", _senderHandlerId)) + _senderDisplayName + "[/url]: ");
			string text2 = this.useChatTypeColorForMessage ? "" : ("[-][" + this.textColorMessage + "]");
			string text3 = Localization.Get((_chatDirection == EChatDirection.Outbound) ? "xuiChatDirectionTo" : "xuiChatDirectionFrom", false);
			string message;
			switch (_chatType)
			{
			case EChatType.Global:
				message = string.Concat(new string[]
				{
					"[",
					this.textColorGlobal,
					"]",
					text,
					text2,
					XUiUtils.BuildUrlFunctionString("Chat", new ValueTuple<string, string>("ChatType", _chatType.ToStringCached<EChatType>())),
					_message,
					"[/url]"
				});
				break;
			case EChatType.Friends:
				message = string.Concat(new string[]
				{
					"[",
					this.textColorFriends,
					"]",
					XUiUtils.BuildUrlFunctionString("Chat", new ValueTuple<string, string>("ChatType", _chatType.ToStringCached<EChatType>())),
					"(Friends)[/url] ",
					text,
					text2,
					_message
				});
				break;
			case EChatType.Party:
				message = string.Concat(new string[]
				{
					"[",
					this.textColorParty,
					"]",
					XUiUtils.BuildUrlFunctionString("Chat", new ValueTuple<string, string>("ChatType", _chatType.ToStringCached<EChatType>())),
					"(Party)[/url] ",
					text,
					text2,
					_message
				});
				break;
			case EChatType.Whisper:
				message = string.Concat(new string[]
				{
					"[",
					this.textColorWhisper,
					"]",
					text3,
					" ",
					text,
					text2,
					_message
				});
				break;
			case EChatType.Discord:
				message = string.Concat(new string[]
				{
					"[5865f2]",
					text3,
					" ",
					text,
					text2,
					_message
				});
				break;
			default:
				throw new ArgumentOutOfRangeException("_chatType", _chatType, null);
			}
			_message = message;
		}
		_message = _message.Replace('\n', ' ');
		this.txtOutput.AddLine(_message);
		this.txtOutput.Label.alpha = 1f;
		this.currentWaitTime = this.fadeoutWaitTime + this.fadeoutDuration;
		this.txtOutput.TextList.scrollValue = 1f;
	}

	// Token: 0x0600604D RID: 24653 RVA: 0x00271C14 File Offset: 0x0026FE14
	public static void AddMessage(XUi _xuiInstance, EnumGameMessages _messageType, string _message, EChatType _chatType = EChatType.Global, EChatDirection _chatDirection = EChatDirection.Inbound, int _senderId = -1, string _senderDisplayName = null, string _senderHandlerId = null, EMessageSender _messageSenderType = EMessageSender.None, GeneratedTextManager.TextFilteringMode _filteringMode = GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode _bbMode = GeneratedTextManager.BbCodeSupportMode.Supported)
	{
		if (_messageType == EnumGameMessages.Chat && !PermissionsManager.IsCommunicationAllowed())
		{
			return;
		}
		if (_senderId != -1 || _messageSenderType == EMessageSender.SenderIdAsPlayer)
		{
			PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(_senderId);
			if (playerDataFromEntityID == null)
			{
				Log.Warning(string.Format("Could not find player name corresponding to entity id {0}, discarding message", _senderId));
				return;
			}
			if (playerDataFromEntityID.PlatformData.Blocked[EBlockType.TextChat].IsBlocked())
			{
				return;
			}
			_senderDisplayName = playerDataFromEntityID.PlayerName.DisplayName;
		}
		if (_messageType == EnumGameMessages.Chat && _messageSenderType == EMessageSender.SenderIdAsPlayer)
		{
			_bbMode = GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes;
		}
		GeneratedTextManager.GetDisplayText(_message, null, delegate(string _filteredMessage)
		{
			XUiController xuiController = _xuiInstance.FindWindowGroupByName(XUiC_ChatOutput.ID);
			if (xuiController != null)
			{
				_filteredMessage += "[ffffffff][/url][/b][/i][/u][/s][/sub][/sup]";
				XUiC_ChatOutput childByType = xuiController.GetChildByType<XUiC_ChatOutput>();
				string senderDisplayName;
				if (_messageSenderType == EMessageSender.Server)
				{
					senderDisplayName = Localization.Get("xuiChatServer", false);
				}
				else
				{
					senderDisplayName = _senderDisplayName;
				}
				_senderDisplayName = senderDisplayName;
				childByType.addMessage(_messageType, _chatType, _chatDirection, _filteredMessage, _senderDisplayName, _senderHandlerId);
			}
		}, false, _filteringMode, _bbMode);
	}

	// Token: 0x0600604E RID: 24654 RVA: 0x00271D00 File Offset: 0x0026FF00
	public override void Update(float _dt)
	{
		base.Update(_dt);
		bool flag = base.xui.playerUI.windowManager.IsWindowOpen(XUiC_Chat.ID);
		this.collider.enabled = flag;
		if (flag)
		{
			this.currentWaitTime = this.fadeoutWaitTime + this.fadeoutDuration;
		}
		this.txtOutput.Label.alpha = Mathf.Lerp(0f, 1f, this.currentWaitTime / this.fadeoutDuration);
		this.currentWaitTime -= Time.deltaTime;
		if (GameManager.Instance == null || GameManager.Instance.World == null || base.xui.playerUI.entityPlayer == null || base.xui.playerUI.entityPlayer.IsDead())
		{
			this.txtOutput.Label.alpha = 0f;
		}
	}

	// Token: 0x0600604F RID: 24655 RVA: 0x00271DEC File Offset: 0x0026FFEC
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_name);
		if (num <= 1054300425U)
		{
			if (num <= 484394924U)
			{
				if (num != 395109698U)
				{
					if (num == 484394924U)
					{
						if (_name == "text_color_global")
						{
							this.textColorGlobal = _value;
							return true;
						}
					}
				}
				else if (_name == "fadeout_duration")
				{
					this.fadeoutDuration = StringParsers.ParseFloat(_value, 0, -1, NumberStyles.Any);
					return true;
				}
			}
			else if (num != 750825111U)
			{
				if (num == 1054300425U)
				{
					if (_name == "text_color_party")
					{
						this.textColorParty = _value;
						return true;
					}
				}
			}
			else if (_name == "text_color_whisper")
			{
				this.textColorWhisper = _value;
				return true;
			}
		}
		else if (num <= 3890498550U)
		{
			if (num != 3023599480U)
			{
				if (num == 3890498550U)
				{
					if (_name == "text_color_message")
					{
						this.textColorMessage = _value;
						return true;
					}
				}
			}
			else if (_name == "text_color_friends")
			{
				this.textColorFriends = _value;
				return true;
			}
		}
		else if (num != 4020806997U)
		{
			if (num == 4076460375U)
			{
				if (_name == "use_chattype_color_for_message")
				{
					this.useChatTypeColorForMessage = StringParsers.ParseBool(_value, 0, -1, true);
					return true;
				}
			}
		}
		else if (_name == "fadeout_wait_time")
		{
			this.fadeoutWaitTime = StringParsers.ParseFloat(_value, 0, -1, NumberStyles.Any);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x040048A4 RID: 18596
	public static string ID = "";

	// Token: 0x040048A5 RID: 18597
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_TextList txtOutput;

	// Token: 0x040048A6 RID: 18598
	[PublicizedFrom(EAccessModifier.Private)]
	public BoxCollider collider;

	// Token: 0x040048A7 RID: 18599
	[PublicizedFrom(EAccessModifier.Private)]
	public float fadeoutWaitTime = 5f;

	// Token: 0x040048A8 RID: 18600
	[PublicizedFrom(EAccessModifier.Private)]
	public float fadeoutDuration = 2f;

	// Token: 0x040048A9 RID: 18601
	[PublicizedFrom(EAccessModifier.Private)]
	public float currentWaitTime;

	// Token: 0x040048AA RID: 18602
	[PublicizedFrom(EAccessModifier.Private)]
	public bool useChatTypeColorForMessage;

	// Token: 0x040048AB RID: 18603
	[PublicizedFrom(EAccessModifier.Private)]
	public string textColorMessage = Color.white.ToHexCode(false);

	// Token: 0x040048AC RID: 18604
	[PublicizedFrom(EAccessModifier.Private)]
	public string textColorGlobal = Color.white.ToHexCode(false);

	// Token: 0x040048AD RID: 18605
	[PublicizedFrom(EAccessModifier.Private)]
	public string textColorFriends = new Color(0f, 0.75f, 0f).ToHexCode(false);

	// Token: 0x040048AE RID: 18606
	[PublicizedFrom(EAccessModifier.Private)]
	public string textColorParty = new Color(1f, 0.8f, 0f).ToHexCode(false);

	// Token: 0x040048AF RID: 18607
	[PublicizedFrom(EAccessModifier.Private)]
	public string textColorWhisper = new Color(0.8f, 0f, 0f).ToHexCode(false);

	// Token: 0x040048B0 RID: 18608
	[PublicizedFrom(EAccessModifier.Private)]
	public const string TextColorDiscord = "5865f2";
}
