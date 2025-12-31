using System;
using System.Collections.Generic;
using System.Threading;

namespace Platform.MultiPlatform
{
	// Token: 0x020018E4 RID: 6372
	public class ServerListAnnouncer : IMasterServerAnnouncer
	{
		// Token: 0x0600BC53 RID: 48211 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600BC54 RID: 48212 RVA: 0x00002914 File Offset: 0x00000B14
		public void Update()
		{
		}

		// Token: 0x1700159A RID: 5530
		// (get) Token: 0x0600BC55 RID: 48213 RVA: 0x00477884 File Offset: 0x00475A84
		public bool GameServerInitialized
		{
			get
			{
				IMasterServerAnnouncer serverListAnnouncer = PlatformManager.NativePlatform.ServerListAnnouncer;
				if (serverListAnnouncer == null || serverListAnnouncer.GameServerInitialized)
				{
					IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
					bool? flag;
					if (crossplatformPlatform == null)
					{
						flag = null;
					}
					else
					{
						IMasterServerAnnouncer serverListAnnouncer2 = crossplatformPlatform.ServerListAnnouncer;
						flag = ((serverListAnnouncer2 != null) ? new bool?(serverListAnnouncer2.GameServerInitialized) : null);
					}
					return flag ?? true;
				}
				return false;
			}
		}

		// Token: 0x0600BC56 RID: 48214 RVA: 0x004778F0 File Offset: 0x00475AF0
		public string GetServerPorts()
		{
			string text = "";
			IMasterServerAnnouncer serverListAnnouncer = PlatformManager.NativePlatform.ServerListAnnouncer;
			string text2 = (serverListAnnouncer != null) ? serverListAnnouncer.GetServerPorts() : null;
			if (!string.IsNullOrEmpty(text2))
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += ", ";
				}
				text += text2;
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			string text3;
			if (crossplatformPlatform == null)
			{
				text3 = null;
			}
			else
			{
				IMasterServerAnnouncer serverListAnnouncer2 = crossplatformPlatform.ServerListAnnouncer;
				text3 = ((serverListAnnouncer2 != null) ? serverListAnnouncer2.GetServerPorts() : null);
			}
			string text4 = text3;
			if (!string.IsNullOrEmpty(text4))
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += ", ";
				}
				text += text4;
			}
			foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair in PlatformManager.ServerPlatforms)
			{
				if (keyValuePair.Value.AsServerOnly)
				{
					IMasterServerAnnouncer serverListAnnouncer3 = keyValuePair.Value.ServerListAnnouncer;
					string text5 = (serverListAnnouncer3 != null) ? serverListAnnouncer3.GetServerPorts() : null;
					if (!string.IsNullOrEmpty(text5))
					{
						if (!string.IsNullOrEmpty(text))
						{
							text += ", ";
						}
						text += text5;
					}
				}
			}
			return text;
		}

		// Token: 0x0600BC57 RID: 48215 RVA: 0x00477A08 File Offset: 0x00475C08
		public void AdvertiseServer(Action _onServerRegistered)
		{
			if (PlatformManager.NativePlatform.ServerListAnnouncer != null)
			{
				Interlocked.Increment(ref this.platformsAdvertising);
				PlatformManager.NativePlatform.ServerListAnnouncer.AdvertiseServer(new Action(this.serverRegisteredCallback));
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (((crossplatformPlatform != null) ? crossplatformPlatform.ServerListAnnouncer : null) != null)
			{
				Interlocked.Increment(ref this.platformsAdvertising);
				IPlatform crossplatformPlatform2 = PlatformManager.CrossplatformPlatform;
				if (crossplatformPlatform2 != null)
				{
					IMasterServerAnnouncer serverListAnnouncer = crossplatformPlatform2.ServerListAnnouncer;
					if (serverListAnnouncer != null)
					{
						serverListAnnouncer.AdvertiseServer(new Action(this.serverRegisteredCallback));
					}
				}
			}
			foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair in PlatformManager.ServerPlatforms)
			{
				if (keyValuePair.Value.AsServerOnly && keyValuePair.Value.ServerListAnnouncer != null)
				{
					Interlocked.Increment(ref this.platformsAdvertising);
					keyValuePair.Value.ServerListAnnouncer.AdvertiseServer(new Action(this.serverRegisteredCallback));
				}
			}
			if (this.platformsAdvertising == 0)
			{
				_onServerRegistered();
				this.onServerRegistered = null;
				return;
			}
			this.onServerRegistered = _onServerRegistered;
		}

		// Token: 0x0600BC58 RID: 48216 RVA: 0x00477B28 File Offset: 0x00475D28
		[PublicizedFrom(EAccessModifier.Private)]
		public void serverRegisteredCallback()
		{
			if (Interlocked.Decrement(ref this.platformsAdvertising) == 0)
			{
				Action action = this.onServerRegistered;
				if (action != null)
				{
					action();
				}
				this.onServerRegistered = null;
			}
		}

		// Token: 0x0600BC59 RID: 48217 RVA: 0x00477B50 File Offset: 0x00475D50
		public void StopServer()
		{
			foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair in PlatformManager.ServerPlatforms)
			{
				if (keyValuePair.Value.AsServerOnly)
				{
					IMasterServerAnnouncer serverListAnnouncer = keyValuePair.Value.ServerListAnnouncer;
					if (serverListAnnouncer != null)
					{
						serverListAnnouncer.StopServer();
					}
				}
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform != null)
			{
				IMasterServerAnnouncer serverListAnnouncer2 = crossplatformPlatform.ServerListAnnouncer;
				if (serverListAnnouncer2 != null)
				{
					serverListAnnouncer2.StopServer();
				}
			}
			IMasterServerAnnouncer serverListAnnouncer3 = PlatformManager.NativePlatform.ServerListAnnouncer;
			if (serverListAnnouncer3 == null)
			{
				return;
			}
			serverListAnnouncer3.StopServer();
		}

		// Token: 0x040092EC RID: 37612
		[PublicizedFrom(EAccessModifier.Private)]
		public int platformsAdvertising;

		// Token: 0x040092ED RID: 37613
		[PublicizedFrom(EAccessModifier.Private)]
		public Action onServerRegistered;
	}
}
