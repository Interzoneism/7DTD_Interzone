using System;
using System.Collections.Generic;
using Platform.LAN;
using Twitch;
using UnityEngine;
using UnityEngine.Scripting;

namespace Platform.XBL
{
	// Token: 0x0200189D RID: 6301
	[Preserve]
	[PlatformFactory(EPlatformIdentifier.XBL)]
	public class Factory : AbsPlatform
	{
		// Token: 0x0600B9FE RID: 47614 RVA: 0x0046FCCC File Offset: 0x0046DECC
		public override void CreateInstances()
		{
			if (!base.AsServerOnly)
			{
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				if (crossplatformPlatform == null || crossplatformPlatform.PlatformIdentifier != EPlatformIdentifier.EOS)
				{
					Application.Quit(1);
					throw new Exception("[XBL] This platform requires EOS as cross platform provider");
				}
			}
			base.Api = new Api();
			base.Utils = new Utils();
			if (!base.AsServerOnly)
			{
				XblXuidMapper.Enable();
				base.User = new User();
				base.AchievementManager = new AchievementManager();
				base.Input = new PlayerInputManager();
				base.TextCensor = new TextCensor();
				base.InviteListeners = new List<IJoinSessionGameInviteListener>
				{
					new JoinSessionGameInviteListener(),
					DiscordInviteListener.ListenerInstance
				};
				base.EntitlementValidators = new List<IEntitlementValidator>
				{
					new DownloadableContentValidator(),
					new TwitchEntitlementManager()
				};
				base.ServerListInterfaces = new List<IServerListInterface>
				{
					new ServerListFriendsMultiplayerActivity(),
					new LANServerList()
				};
			}
		}
	}
}
