using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000DFC RID: 3580
[Preserve]
public class XUiC_ServerBrowserDirectConnect : XUiController
{
	// Token: 0x06007038 RID: 28728 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		return false;
	}

	// Token: 0x06007039 RID: 28729 RVA: 0x002DCA04 File Offset: 0x002DAC04
	public override void Init()
	{
		base.Init();
		this.txtIp = (XUiC_TextInput)base.GetChildById("txtIp");
		this.txtPort = (XUiC_TextInput)base.GetChildById("txtPort");
		this.txtIp.OnClipboardHandler += this.TxtIp_OnClipboardHandler;
		this.txtIp.OnSubmitHandler += this.Txt_OnSubmitHandler;
		this.txtPort.OnSubmitHandler += this.Txt_OnSubmitHandler;
		this.txtIp.OnChangeHandler += this.validateIpPort;
		this.txtPort.OnChangeHandler += this.validateIpPort;
		this.txtIp.SelectOnTab = this.txtPort;
		this.txtPort.SelectOnTab = this.txtIp;
		this.btnCancel = (XUiC_SimpleButton)base.GetChildById("btnCancel");
		this.btnCancel.OnPressed += this.BtnCancel_OnPressed;
		this.btnDirectConnectConnect = (XUiC_SimpleButton)base.GetChildById("btnDirectConnectConnect");
		this.btnDirectConnectConnect.OnPressed += this.BtnConnect_OnPressed;
		this.btnDirectConnectConnect.Enabled = false;
	}

	// Token: 0x0600703A RID: 28730 RVA: 0x002DCB3E File Offset: 0x002DAD3E
	public override void OnClose()
	{
		base.OnClose();
		XUiC_MultiplayerPrivilegeNotification.Close();
	}

	// Token: 0x0600703B RID: 28731 RVA: 0x002DCB4C File Offset: 0x002DAD4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void TxtIp_OnClipboardHandler(UIInput.ClipboardAction _actiontype, string _oldtext, int _selstart, int _selend, string _actionresulttext)
	{
		if (_actiontype != UIInput.ClipboardAction.Paste)
		{
			return;
		}
		if (_selend - _selstart != _oldtext.Length)
		{
			return;
		}
		Match match = XUiC_ServerBrowserDirectConnect.ipPortMatcher.Match(_actionresulttext);
		if (!match.Success)
		{
			return;
		}
		string value = match.Groups[1].Value;
		int num;
		if (!StringParsers.TryParseSInt32(match.Groups[2].Value, out num, 0, -1, NumberStyles.Integer))
		{
			return;
		}
		this.txtIp.Text = value;
		this.txtPort.Text = num.ToString();
	}

	// Token: 0x0600703C RID: 28732 RVA: 0x002DCBCF File Offset: 0x002DADCF
	[PublicizedFrom(EAccessModifier.Private)]
	public void Txt_OnSubmitHandler(XUiController _sender, string _text)
	{
		this.BtnConnect_OnPressed(_sender, -1);
	}

	// Token: 0x0600703D RID: 28733 RVA: 0x002DCBDC File Offset: 0x002DADDC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnConnect_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.ViewComponent.IsVisible = false;
		this.saveIpPort();
		if (this.wdwMultiplayerPrivileges == null)
		{
			this.wdwMultiplayerPrivileges = XUiC_MultiplayerPrivilegeNotification.GetWindow();
		}
		XUiC_MultiplayerPrivilegeNotification xuiC_MultiplayerPrivilegeNotification = this.wdwMultiplayerPrivileges;
		if (xuiC_MultiplayerPrivilegeNotification == null)
		{
			return;
		}
		xuiC_MultiplayerPrivilegeNotification.ResolvePrivilegesWithDialog(EUserPerms.Multiplayer, delegate(bool result)
		{
			if (!result)
			{
				return;
			}
			string text = this.txtIp.Text.Trim();
			long num;
			if (!long.TryParse(text.Replace(".", ""), out num))
			{
				try
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(text);
					if (hostEntry.AddressList.Length == 0)
					{
						Log.Out("No valid IP for server found");
						return;
					}
					text = hostEntry.AddressList[0].ToString();
					if (hostEntry.AddressList[0].AddressFamily != AddressFamily.InterNetwork)
					{
						for (int i = 1; i < hostEntry.AddressList.Length; i++)
						{
							if (hostEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
							{
								text = hostEntry.AddressList[i].ToString();
								break;
							}
						}
					}
				}
				catch (SocketException ex)
				{
					string str = "No such hostname: \"";
					string str2 = text;
					string str3 = "\": ";
					SocketException ex2 = ex;
					Log.Out(str + str2 + str3 + ((ex2 != null) ? ex2.ToString() : null));
					return;
				}
			}
			Log.Out("Connect by IP");
			GameServerInfo gameServerInfo = new GameServerInfo();
			gameServerInfo.SetValue(GameInfoString.IP, text);
			gameServerInfo.SetValue(GameInfoInt.Port, int.Parse(this.txtPort.Text));
			GameManager.Instance.showOpenerMovieOnLoad = false;
			SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo = gameServerInfo;
			base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
			SingletonMonoBehaviour<ConnectionManager>.Instance.Connect(SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo);
		}, -1f, false, null);
	}

	// Token: 0x0600703E RID: 28734 RVA: 0x002DCC33 File Offset: 0x002DAE33
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnCancel_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.Hide();
	}

	// Token: 0x0600703F RID: 28735 RVA: 0x002DCC3C File Offset: 0x002DAE3C
	public void Show(XUiController _cancelTarget)
	{
		this.viewComponent.IsVisible = true;
		if (this.txtIp != null)
		{
			this.txtIp.Text = GamePrefs.GetString(EnumGamePrefs.ConnectToServerIP);
			this.txtPort.Text = GamePrefs.GetInt(EnumGamePrefs.ConnectToServerPort).ToString();
			this.txtIp.SetSelected(true, false);
			this.validateIpPort(null, null, true);
		}
		this.cancelTarget = _cancelTarget;
		base.xui.playerUI.CursorController.SetNavigationLockView(this.viewComponent, null);
		if (this.btnCancel != null)
		{
			this.btnCancel.SelectCursorElement(true, false);
		}
	}

	// Token: 0x06007040 RID: 28736 RVA: 0x002DCCD8 File Offset: 0x002DAED8
	public void Hide()
	{
		this.viewComponent.IsVisible = false;
		this.saveIpPort();
		base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
		if (this.cancelTarget != null)
		{
			this.cancelTarget.SelectCursorElement(true, false);
		}
	}

	// Token: 0x06007041 RID: 28737 RVA: 0x002DCD24 File Offset: 0x002DAF24
	[PublicizedFrom(EAccessModifier.Private)]
	public void validateIpPort(XUiController _sender, string _newText, bool _changeFromCode)
	{
		int num;
		bool flag = int.TryParse(this.txtPort.Text, out num);
		flag = (flag && this.txtIp.Text.Length > 0 && num > 0 && num < 65533);
		this.btnDirectConnectConnect.Enabled = flag;
	}

	// Token: 0x06007042 RID: 28738 RVA: 0x002DCD78 File Offset: 0x002DAF78
	[PublicizedFrom(EAccessModifier.Private)]
	public void saveIpPort()
	{
		if (this.txtIp != null && this.btnDirectConnectConnect.Enabled)
		{
			GamePrefs.Set(EnumGamePrefs.ConnectToServerIP, this.txtIp.Text);
			GamePrefs.Set(EnumGamePrefs.ConnectToServerPort, StringParsers.ParseSInt32(this.txtPort.Text, 0, -1, NumberStyles.Integer));
		}
	}

	// Token: 0x04005541 RID: 21825
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_MultiplayerPrivilegeNotification wdwMultiplayerPrivileges;

	// Token: 0x04005542 RID: 21826
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtIp;

	// Token: 0x04005543 RID: 21827
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtPort;

	// Token: 0x04005544 RID: 21828
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnDirectConnectConnect;

	// Token: 0x04005545 RID: 21829
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnCancel;

	// Token: 0x04005546 RID: 21830
	public XUiController cancelTarget;

	// Token: 0x04005547 RID: 21831
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex ipPortMatcher = new Regex("^(.*):(\\d+)$");
}
