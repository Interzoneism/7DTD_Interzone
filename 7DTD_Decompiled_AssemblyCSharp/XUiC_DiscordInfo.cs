using System;
using UnityEngine.Scripting;

// Token: 0x020002FC RID: 764
[Preserve]
public class XUiC_DiscordInfo : XUiController
{
	// Token: 0x060015A6 RID: 5542 RVA: 0x0007F844 File Offset: 0x0007DA44
	public override void Init()
	{
		base.Init();
		XUiC_DiscordInfo.ID = base.WindowGroup.ID;
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnFullAccount") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += delegate(XUiController _, int _)
			{
				this.closeAndOpenLoginWindow();
				DiscordManager.Instance.AuthManager.LoginDiscordUser();
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton2 = base.GetChildById("btnOk") as XUiC_SimpleButton;
		if (xuiC_SimpleButton2 != null)
		{
			xuiC_SimpleButton2.OnPressed += delegate(XUiController _, int _)
			{
				this.closeAndOpenMainMenu();
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton3 = base.GetChildById("btnNotNow") as XUiC_SimpleButton;
		if (xuiC_SimpleButton3 != null)
		{
			xuiC_SimpleButton3.OnPressed += delegate(XUiController _, int _)
			{
				this.closeAndOpenMainMenu();
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton4 = base.GetChildById("btnSettings") as XUiC_SimpleButton;
		if (xuiC_SimpleButton4 != null)
		{
			xuiC_SimpleButton4.OnPressed += delegate(XUiController _, int _)
			{
				base.xui.playerUI.windowManager.Close(XUiC_DiscordInfo.ID);
				XUiC_OptionsAudio childByType = base.xui.GetChildByType<XUiC_OptionsAudio>();
				if (childByType == null)
				{
					return;
				}
				childByType.OpenAtTab("xuiOptionsAudioDiscord");
			};
		}
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x0007F8FF File Offset: 0x0007DAFF
	[PublicizedFrom(EAccessModifier.Private)]
	public void closeAndOpenMainMenu()
	{
		base.xui.playerUI.windowManager.Close(XUiC_DiscordInfo.ID);
		base.xui.playerUI.windowManager.Open(XUiC_MainMenu.ID, true, false, true);
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x0007F938 File Offset: 0x0007DB38
	[PublicizedFrom(EAccessModifier.Private)]
	public void closeAndOpenLoginWindow()
	{
		base.xui.playerUI.windowManager.Close(XUiC_DiscordInfo.ID);
		XUiC_DiscordLogin.Open(null, true, false, false, true, false);
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x0007F95F File Offset: 0x0007DB5F
	public override void OnOpen()
	{
		base.OnOpen();
		this.windowGroup.isEscClosable = false;
		DiscordManager.Instance.Settings.DiscordFirstTimeInfoShown = true;
		DiscordManager.Instance.Settings.Save();
		base.RefreshBindings(false);
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x0007F99C File Offset: 0x0007DB9C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "supports_full_accounts")
		{
			_value = DiscordManager.SupportsFullAccounts.ToString();
			return true;
		}
		if (!(_bindingName == "supports_provisional_accounts"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = DiscordManager.SupportsProvisionalAccounts.ToString();
		return true;
	}

	// Token: 0x04000DD2 RID: 3538
	public static string ID = "";
}
