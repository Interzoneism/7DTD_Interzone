using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000C34 RID: 3124
[Preserve]
public class XUiC_Chat : XUiController
{
	// Token: 0x06006021 RID: 24609 RVA: 0x00270D18 File Offset: 0x0026EF18
	public override void Init()
	{
		XUiC_Chat.ID = this.windowGroup.ID;
		base.Init();
		this.cbxTarget = base.GetChildByType<XUiC_ComboBoxList<XUiC_Chat.ChatTarget>>();
		this.txtInput = base.GetChildByType<XUiC_TextInput>();
		this.txtInput.OnSubmitHandler += this.TextInput_OnSubmitHandler;
		this.txtInput.OnInputAbortedHandler += this.TextInput_OnInputAbortedHandler;
		this.txtInput.SupportBbCode = false;
		this.registerRegularChatMessageHandlers();
	}

	// Token: 0x06006022 RID: 24610 RVA: 0x00270D94 File Offset: 0x0026EF94
	[PublicizedFrom(EAccessModifier.Private)]
	public void registerRegularChatMessageHandlers()
	{
		XUiC_Chat.RegisterCustomMessagingHandler(EChatType.Global, new XUiC_Chat.IsValidTarget(XUiC_Chat.<registerRegularChatMessageHandlers>g__IsValidTargetRegular|9_4), new XUiC_Chat.GetTargetDisplayName(XUiC_Chat.<registerRegularChatMessageHandlers>g__GetTargetDisplayNameRegular|9_3), new XUiC_Chat.SendMessage(this.<registerRegularChatMessageHandlers>g__SendMessageRegular|9_2));
		XUiC_Chat.RegisterCustomMessagingHandler(EChatType.Friends, new XUiC_Chat.IsValidTarget(XUiC_Chat.<registerRegularChatMessageHandlers>g__IsValidTargetRegular|9_4), new XUiC_Chat.GetTargetDisplayName(XUiC_Chat.<registerRegularChatMessageHandlers>g__GetTargetDisplayNameRegular|9_3), new XUiC_Chat.SendMessage(this.<registerRegularChatMessageHandlers>g__SendMessageRegular|9_2));
		XUiC_Chat.RegisterCustomMessagingHandler(EChatType.Party, new XUiC_Chat.IsValidTarget(XUiC_Chat.<registerRegularChatMessageHandlers>g__IsValidTargetRegular|9_4), new XUiC_Chat.GetTargetDisplayName(XUiC_Chat.<registerRegularChatMessageHandlers>g__GetTargetDisplayNameRegular|9_3), new XUiC_Chat.SendMessage(this.<registerRegularChatMessageHandlers>g__SendMessageRegular|9_2));
		XUiC_Chat.RegisterCustomMessagingHandler(EChatType.Whisper, new XUiC_Chat.IsValidTarget(XUiC_Chat.<registerRegularChatMessageHandlers>g__IsValidTargetRegular|9_4), new XUiC_Chat.GetTargetDisplayName(XUiC_Chat.<registerRegularChatMessageHandlers>g__GetTargetDisplayNameRegular|9_3), new XUiC_Chat.SendMessage(this.<registerRegularChatMessageHandlers>g__SendMessageRegular|9_2));
		this.allTargets.Add(new XUiC_Chat.ChatTarget(EChatType.Global, null, null, true));
		this.allTargets.Add(new XUiC_Chat.ChatTarget(EChatType.Friends, null, (XUiC_Chat.ChatTarget _, XUi _) => this.entityIdsFriends.Count > 1, true));
		this.allTargets.Add(new XUiC_Chat.ChatTarget(EChatType.Party, null, (XUiC_Chat.ChatTarget _, XUi _) => this.entityIdsParty.Count > 1, true));
	}

	// Token: 0x06006023 RID: 24611 RVA: 0x00269150 File Offset: 0x00267350
	[PublicizedFrom(EAccessModifier.Private)]
	public void TextInput_OnInputAbortedHandler(XUiController _sender)
	{
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x06006024 RID: 24612 RVA: 0x00270E9C File Offset: 0x0026F09C
	[PublicizedFrom(EAccessModifier.Private)]
	public void TextInput_OnSubmitHandler(XUiController _sender, string _text)
	{
		if (_text.Length > 0 && _text != " ")
		{
			_text = _text.Replace('\n', ' ');
			this.cbxTarget.Value.Send(_text);
			this.txtInput.Text = "";
		}
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x06006025 RID: 24613 RVA: 0x00270F0C File Offset: 0x0026F10C
	public override void OnOpen()
	{
		this.cbxTarget.Enabled = PermissionsManager.IsCommunicationAllowed();
		this.txtInput.Enabled = PermissionsManager.IsCommunicationAllowed();
		this.updateTargets();
		base.OnOpen();
	}

	// Token: 0x06006026 RID: 24614 RVA: 0x00270F3C File Offset: 0x0026F13C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateTargets()
	{
		this.entityIdsFriends.Clear();
		this.entityIdsFriends.Add(base.xui.playerUI.entityPlayer.entityId);
		foreach (EntityPlayer entityPlayer in GameManager.Instance.World.Players.list)
		{
			if (entityPlayer.IsFriendOfLocalPlayer)
			{
				this.entityIdsFriends.Add(entityPlayer.entityId);
			}
		}
		this.entityIdsParty.Clear();
		this.entityIdsParty.Add(base.xui.playerUI.entityPlayer.entityId);
		if (base.xui.playerUI.entityPlayer.Party != null)
		{
			foreach (EntityPlayer entityPlayer2 in base.xui.playerUI.entityPlayer.Party.MemberList)
			{
				if (entityPlayer2 != base.xui.playerUI.entityPlayer)
				{
					this.entityIdsParty.Add(entityPlayer2.entityId);
				}
			}
		}
		this.allTargets.Sort();
		XUiC_Chat.ChatTarget value = this.cbxTarget.Value;
		this.cbxTarget.Elements.Clear();
		for (int i = this.allTargets.Count - 1; i >= 0; i--)
		{
			XUiC_Chat.ChatTarget chatTarget = this.allTargets[i];
			if (!chatTarget.KeepForever && chatTarget.Age.TotalMinutes > this.keepWhisperTargetsForMinutes)
			{
				this.allTargets.RemoveAt(i);
			}
			else if (!chatTarget.IsValid(base.xui))
			{
				if (!chatTarget.KeepForever)
				{
					this.allTargets.RemoveAt(i);
				}
			}
			else
			{
				this.cbxTarget.Elements.Insert(0, chatTarget);
			}
		}
		if (value != null)
		{
			int num = this.cbxTarget.Elements.IndexOf(value);
			this.cbxTarget.SelectedIndex = ((num < 0) ? 0 : num);
		}
	}

	// Token: 0x06006027 RID: 24615 RVA: 0x00271184 File Offset: 0x0026F384
	[PublicizedFrom(EAccessModifier.Private)]
	public int enforceTarget(EChatType _chatType, string _targetId)
	{
		int num = this.cbxTarget.Elements.FindIndex((XUiC_Chat.ChatTarget _target) => _target.ChatType == _chatType && _targetId == _target.TargetId);
		if (num >= 0)
		{
			return num;
		}
		if (string.IsNullOrEmpty(_targetId))
		{
			return -1;
		}
		this.allTargets.Add(new XUiC_Chat.ChatTarget(_chatType, _targetId, null, false));
		this.updateTargets();
		return this.cbxTarget.Elements.FindIndex((XUiC_Chat.ChatTarget _target) => _target.ChatType == _chatType && _targetId == _target.TargetId);
	}

	// Token: 0x06006028 RID: 24616 RVA: 0x00271218 File Offset: 0x0026F418
	[PublicizedFrom(EAccessModifier.Private)]
	public void findOrAddTarget(EChatType _chatType, string _targetId)
	{
		int num = this.enforceTarget(_chatType, _targetId);
		if (num >= 0)
		{
			this.cbxTarget.SelectedIndex = num;
			this.txtInput.SetSelected(true, false);
		}
	}

	// Token: 0x06006029 RID: 24617 RVA: 0x0027124C File Offset: 0x0026F44C
	public override void UpdateInput()
	{
		base.UpdateInput();
		if (base.xui.playerUI.playerInput != null)
		{
			if (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard)
			{
				PlayerActionsGUI guiactions = base.xui.playerUI.playerInput.GUIActions;
				if (guiactions.Up.WasPressed)
				{
					this.cbxTarget.ChangeIndex(-1);
				}
				if (guiactions.Down.WasPressed)
				{
					this.cbxTarget.ChangeIndex(1);
				}
			}
			if (base.xui.playerUI.playerInput.PermanentActions.Cancel.WasPressed)
			{
				base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
			}
		}
	}

	// Token: 0x0600602A RID: 24618 RVA: 0x0027130D File Offset: 0x0026F50D
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "keep_whisper_targets_for_minutes")
		{
			this.keepWhisperTargetsForMinutes = StringParsers.ParseUInt32(_value, 0, -1, NumberStyles.Integer);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x0600602B RID: 24619 RVA: 0x00271338 File Offset: 0x0026F538
	public static void RegisterCustomMessagingHandler(EChatType _chatType, XUiC_Chat.IsValidTarget _isValidTargetDelegate, XUiC_Chat.GetTargetDisplayName _getTargetDisplayNameDelegate, XUiC_Chat.SendMessage _sendMessageDelegate)
	{
		if (_chatType >= (EChatType)XUiC_Chat.messagingHandlers.Length)
		{
			Log.Warning(string.Format("Can not register chat messaging handler for invalid chat type '{0}'", (int)_chatType));
			return;
		}
		XUiC_Chat.messagingHandlers[(int)_chatType] = new XUiC_Chat.ChatMessagingHandler(_isValidTargetDelegate, _getTargetDisplayNameDelegate, _sendMessageDelegate);
	}

	// Token: 0x0600602C RID: 24620 RVA: 0x00271378 File Offset: 0x0026F578
	public static void SetChatTarget(XUi _xuiInstance, EChatType _chatType, string _targetId)
	{
		XUiC_Chat windowByType = _xuiInstance.GetWindowByType<XUiC_Chat>();
		if (windowByType == null)
		{
			Log.Error("No chat window found!");
			return;
		}
		if (_chatType >= (EChatType)XUiC_Chat.messagingHandlers.Length || XUiC_Chat.messagingHandlers[(int)_chatType] == null)
		{
			Log.Warning(string.Format("Can not handle chat messaging, invalid chat type '{0}' or no handler defined", (int)_chatType));
			return;
		}
		if (!XUiC_Chat.messagingHandlers[(int)_chatType].IsValidTargetDelegate(_chatType, _targetId))
		{
			return;
		}
		windowByType.findOrAddTarget(_chatType, _targetId);
	}

	// Token: 0x0600602D RID: 24621 RVA: 0x002713E4 File Offset: 0x0026F5E4
	public static void EnforceTargetExists(XUi _xuiInstance, EChatType _chatType, string _targetId)
	{
		XUiC_Chat windowByType = _xuiInstance.GetWindowByType<XUiC_Chat>();
		if (windowByType == null)
		{
			Log.Error("No chat window found!");
			return;
		}
		if (_chatType >= (EChatType)XUiC_Chat.messagingHandlers.Length || XUiC_Chat.messagingHandlers[(int)_chatType] == null)
		{
			Log.Warning(string.Format("Can not handle chat messaging, invalid chat type '{0}' or no handler defined", (int)_chatType));
			return;
		}
		if (!XUiC_Chat.messagingHandlers[(int)_chatType].IsValidTargetDelegate(_chatType, _targetId))
		{
			return;
		}
		windowByType.enforceTarget(_chatType, _targetId);
	}

	// Token: 0x06006032 RID: 24626 RVA: 0x002714C0 File Offset: 0x0026F6C0
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <registerRegularChatMessageHandlers>g__SendMessageRegular|9_2(EChatType _chatType, string _targetId, string _message)
	{
		List<int> recipientEntityIds;
		if (_targetId == null)
		{
			List<int> list;
			switch (_chatType)
			{
			case EChatType.Global:
				list = null;
				break;
			case EChatType.Friends:
				list = this.entityIdsFriends;
				break;
			case EChatType.Party:
				list = this.entityIdsParty;
				break;
			case EChatType.Whisper:
				throw new ArgumentException("Whisper without ID not supported");
			default:
				throw new ArgumentException("Invalid chat type without ID");
			}
			recipientEntityIds = list;
		}
		else
		{
			int item;
			if (!int.TryParse(_targetId, out item))
			{
				throw new ArgumentException("Could not parse chat entity id '" + _targetId + "'");
			}
			_chatType = EChatType.Whisper;
			recipientEntityIds = new List<int>
			{
				item
			};
		}
		GameManager.Instance.ChatMessageServer(null, _chatType, base.xui.playerUI.entityPlayer.entityId, _message, recipientEntityIds, EMessageSender.SenderIdAsPlayer, GeneratedTextManager.BbCodeSupportMode.Supported);
	}

	// Token: 0x06006033 RID: 24627 RVA: 0x00271570 File Offset: 0x0026F770
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static string <registerRegularChatMessageHandlers>g__GetTargetDisplayNameRegular|9_3(EChatType _chatType, string _targetId)
	{
		if (_targetId == null)
		{
			return Localization.Get("xuiChatTarget" + _chatType.ToStringCached<EChatType>(), false);
		}
		int key;
		if (!int.TryParse(_targetId, out key))
		{
			throw new ArgumentException("Could not parse chat entity id '" + _targetId + "'");
		}
		EntityPlayer entityPlayer;
		if (!GameManager.Instance.World.Players.dict.TryGetValue(key, out entityPlayer))
		{
			return Localization.Get("xuiChatTarget" + _chatType.ToStringCached<EChatType>(), false);
		}
		return string.Format(Localization.Get("xuiChatTargetWhisper", false), entityPlayer.PlayerDisplayName);
	}

	// Token: 0x06006034 RID: 24628 RVA: 0x00271604 File Offset: 0x0026F804
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static bool <registerRegularChatMessageHandlers>g__IsValidTargetRegular|9_4(EChatType _chatType, string _targetId)
	{
		if (_targetId == null)
		{
			return true;
		}
		int num;
		if (!int.TryParse(_targetId, out num))
		{
			throw new ArgumentException("Could not parse chat entity id '" + _targetId + "' for target validation");
		}
		EntityPlayer entityPlayer;
		return GameManager.Instance.World.Players.dict.TryGetValue(num, out entityPlayer) && num != GameManager.Instance.World.GetPrimaryPlayer().entityId;
	}

	// Token: 0x0400488C RID: 18572
	public static string ID = "";

	// Token: 0x0400488D RID: 18573
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_Chat.ChatTarget> cbxTarget;

	// Token: 0x0400488E RID: 18574
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtInput;

	// Token: 0x0400488F RID: 18575
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<int> entityIdsFriends = new List<int>();

	// Token: 0x04004890 RID: 18576
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<int> entityIdsParty = new List<int>();

	// Token: 0x04004891 RID: 18577
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_Chat.ChatTarget> allTargets = new List<XUiC_Chat.ChatTarget>();

	// Token: 0x04004892 RID: 18578
	[PublicizedFrom(EAccessModifier.Private)]
	public uint keepWhisperTargetsForMinutes = 15U;

	// Token: 0x04004893 RID: 18579
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XUiC_Chat.ChatMessagingHandler[] messagingHandlers = new XUiC_Chat.ChatMessagingHandler[(int)(EnumUtils.MaxValue<EChatType>() + 1)];

	// Token: 0x02000C35 RID: 3125
	[UsedImplicitly]
	[PublicizedFrom(EAccessModifier.Private)]
	public class ChatTarget : IComparable<XUiC_Chat.ChatTarget>
	{
		// Token: 0x06006035 RID: 24629 RVA: 0x00271670 File Offset: 0x0026F870
		public ChatTarget(EChatType _chatType, string _targetId = null, Func<XUiC_Chat.ChatTarget, XUi, bool> _validCondition = null, bool _keepForever = false)
		{
			this.ChatType = _chatType;
			this.TargetId = _targetId;
			this.ValidCondition = _validCondition;
			this.messageHandler = XUiC_Chat.messagingHandlers[(int)_chatType];
			if (this.messageHandler == null)
			{
				throw new ArgumentException(string.Format("Missing handler for chat type {0}", _chatType));
			}
			this.displayText = this.messageHandler.GetTargetDisplayNameDelegate(_chatType, _targetId);
			this.KeepForever = _keepForever;
			this.lastUsed = DateTime.Now;
		}

		// Token: 0x06006036 RID: 24630 RVA: 0x002716EE File Offset: 0x0026F8EE
		public override string ToString()
		{
			return this.displayText;
		}

		// Token: 0x06006037 RID: 24631 RVA: 0x002716F6 File Offset: 0x0026F8F6
		public void Send(string _message)
		{
			this.messageHandler.SendMessageDelegate(this.ChatType, this.TargetId, _message);
			this.lastUsed = DateTime.Now;
		}

		// Token: 0x170009E8 RID: 2536
		// (get) Token: 0x06006038 RID: 24632 RVA: 0x00271720 File Offset: 0x0026F920
		public TimeSpan Age
		{
			get
			{
				return DateTime.Now - this.lastUsed;
			}
		}

		// Token: 0x06006039 RID: 24633 RVA: 0x00271732 File Offset: 0x0026F932
		public bool IsValid(XUi _xuiInstance)
		{
			Func<XUiC_Chat.ChatTarget, XUi, bool> validCondition = this.ValidCondition;
			if (validCondition == null)
			{
				XUiC_Chat.ChatMessagingHandler chatMessagingHandler = this.messageHandler;
				return chatMessagingHandler == null || chatMessagingHandler.IsValidTargetDelegate(this.ChatType, this.TargetId);
			}
			return validCondition(this, _xuiInstance);
		}

		// Token: 0x0600603A RID: 24634 RVA: 0x00271768 File Offset: 0x0026F968
		public int CompareTo(XUiC_Chat.ChatTarget _other)
		{
			if (this == _other)
			{
				return 0;
			}
			if (_other == null)
			{
				return -1;
			}
			int num = -this.KeepForever.CompareTo(_other.KeepForever);
			if (num != 0)
			{
				return num;
			}
			if (this.KeepForever)
			{
				num = this.ChatType.CompareTo(_other.ChatType);
				if (num != 0)
				{
					return num;
				}
			}
			return this.Age.CompareTo(_other.Age);
		}

		// Token: 0x04004894 RID: 18580
		public readonly EChatType ChatType;

		// Token: 0x04004895 RID: 18581
		public readonly string TargetId;

		// Token: 0x04004896 RID: 18582
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XUiC_Chat.ChatMessagingHandler messageHandler;

		// Token: 0x04004897 RID: 18583
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string displayText;

		// Token: 0x04004898 RID: 18584
		[PublicizedFrom(EAccessModifier.Private)]
		public DateTime lastUsed;

		// Token: 0x04004899 RID: 18585
		public readonly bool KeepForever;

		// Token: 0x0400489A RID: 18586
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Func<XUiC_Chat.ChatTarget, XUi, bool> ValidCondition;
	}

	// Token: 0x02000C36 RID: 3126
	// (Invoke) Token: 0x0600603C RID: 24636
	public delegate bool IsValidTarget(EChatType _chatType, string _targetId);

	// Token: 0x02000C37 RID: 3127
	// (Invoke) Token: 0x06006040 RID: 24640
	public delegate string GetTargetDisplayName(EChatType _chatType, string _targetId);

	// Token: 0x02000C38 RID: 3128
	// (Invoke) Token: 0x06006044 RID: 24644
	public delegate void SendMessage(EChatType _chatType, string _targetId, string _message);

	// Token: 0x02000C39 RID: 3129
	[PublicizedFrom(EAccessModifier.Private)]
	public class ChatMessagingHandler
	{
		// Token: 0x06006047 RID: 24647 RVA: 0x002717DB File Offset: 0x0026F9DB
		public ChatMessagingHandler(XUiC_Chat.IsValidTarget _isValidTargetDelegate, XUiC_Chat.GetTargetDisplayName _getTargetDisplayNameDelegate, XUiC_Chat.SendMessage _sendMessageDelegate)
		{
			this.IsValidTargetDelegate = _isValidTargetDelegate;
			this.GetTargetDisplayNameDelegate = _getTargetDisplayNameDelegate;
			this.SendMessageDelegate = _sendMessageDelegate;
		}

		// Token: 0x0400489B RID: 18587
		public readonly XUiC_Chat.IsValidTarget IsValidTargetDelegate;

		// Token: 0x0400489C RID: 18588
		public readonly XUiC_Chat.GetTargetDisplayName GetTargetDisplayNameDelegate;

		// Token: 0x0400489D RID: 18589
		public readonly XUiC_Chat.SendMessage SendMessageDelegate;
	}
}
