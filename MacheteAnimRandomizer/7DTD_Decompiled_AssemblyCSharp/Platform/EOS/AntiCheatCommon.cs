using System;

namespace Platform.EOS
{
	// Token: 0x02001906 RID: 6406
	public static class AntiCheatCommon
	{
		// Token: 0x0600BD2E RID: 48430 RVA: 0x0047A71C File Offset: 0x0047891C
		public static void Init()
		{
			if (AntiCheatCommon.initialized)
			{
				return;
			}
			string launchArgument = GameUtils.GetLaunchArgument("debugeac");
			AntiCheatCommon.DebugEacVerbose = (launchArgument != null && launchArgument == "verbose");
			AntiCheatCommon.NoEacCmdLine = (GameUtils.GetLaunchArgument("noeac") != null);
			AntiCheatCommon.initialized = true;
		}

		// Token: 0x0600BD2F RID: 48431 RVA: 0x0047A76C File Offset: 0x0047896C
		public static ClientInfo IntPtrToClientInfo(IntPtr _ptr, string _messageIfNull = null)
		{
			int num = _ptr.ToInt32();
			ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForClientNumber(num);
			if (clientInfo == null && _messageIfNull != null)
			{
				Log.Error(_messageIfNull, new object[]
				{
					num
				});
			}
			return clientInfo;
		}

		// Token: 0x0600BD30 RID: 48432 RVA: 0x0047A7AE File Offset: 0x004789AE
		public static IntPtr ClientInfoToIntPtr(ClientInfo _clientInfo)
		{
			return new IntPtr(_clientInfo.ClientNumber);
		}

		// Token: 0x0400935C RID: 37724
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool initialized;

		// Token: 0x0400935D RID: 37725
		public static bool NoEacCmdLine;

		// Token: 0x0400935E RID: 37726
		public static bool DebugEacVerbose;

		// Token: 0x0400935F RID: 37727
		public static readonly object LockObject = new object();
	}
}
