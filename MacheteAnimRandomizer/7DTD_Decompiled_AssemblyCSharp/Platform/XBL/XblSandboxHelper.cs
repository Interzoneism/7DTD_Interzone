using System;
using System.Runtime.CompilerServices;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL
{
	// Token: 0x02001888 RID: 6280
	public class XblSandboxHelper
	{
		// Token: 0x17001524 RID: 5412
		// (get) Token: 0x0600B9A2 RID: 47522 RVA: 0x0046DFB8 File Offset: 0x0046C1B8
		public string SandboxId
		{
			get
			{
				string text = this.sandboxId;
				if (text == null)
				{
					Log.Error("[XBL] XblSandboxHelper SandboxId has not finished refreshing");
					return null;
				}
				return text;
			}
		}

		// Token: 0x0600B9A3 RID: 47523 RVA: 0x0046DFDC File Offset: 0x0046C1DC
		public void RefreshSandboxId()
		{
			this.sandboxId = null;
			ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(this.<RefreshSandboxId>g__GetSandboxTask|3_0), null, null, true);
		}

		// Token: 0x0600B9A4 RID: 47524 RVA: 0x0046DFFA File Offset: 0x0046C1FA
		public static EMatchmakingGroup SandboxIdToMatchmakingGroup(string sandboxId)
		{
			if (sandboxId == "CERT" || sandboxId == "CERT.DEBUG")
			{
				return EMatchmakingGroup.CertQA;
			}
			if (!(sandboxId == "RETAIL"))
			{
				return EMatchmakingGroup.Dev;
			}
			return EMatchmakingGroup.Retail;
		}

		// Token: 0x0600B9A5 RID: 47525 RVA: 0x0046E02A File Offset: 0x0046C22A
		public static DLCEnvironmentFlags SandboxIdToDLCEnvironment(string sandboxId)
		{
			if (sandboxId == "CERT" || sandboxId == "CERT.DEBUG")
			{
				return DLCEnvironmentFlags.Cert;
			}
			if (!(sandboxId == "RETAIL"))
			{
				return DLCEnvironmentFlags.Dev;
			}
			return DLCEnvironmentFlags.Retail;
		}

		// Token: 0x0600B9A7 RID: 47527 RVA: 0x0046E05C File Offset: 0x0046C25C
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <RefreshSandboxId>g__GetSandboxTask|3_0(ThreadManager.TaskInfo taskInfo)
		{
			string str;
			int hr = SDK.XSystemGetXboxLiveSandboxId(out str);
			XblHelpers.LogHR(hr, "XSystemGetXboxLiveSandboxId", false);
			if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
			{
				Log.Out("[XBL] retrieved sandbox id: " + str);
				this.sandboxId = str;
			}
		}

		// Token: 0x0400918B RID: 37259
		[PublicizedFrom(EAccessModifier.Private)]
		public string sandboxId;
	}
}
