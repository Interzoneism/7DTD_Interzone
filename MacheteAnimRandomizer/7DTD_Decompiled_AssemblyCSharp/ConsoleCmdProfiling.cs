using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Profiling;
using UnityEngine.Scripting;

// Token: 0x02000232 RID: 562
[Preserve]
public class ConsoleCmdProfiling : ConsoleCmdAbstract
{
	// Token: 0x17000198 RID: 408
	// (get) Token: 0x06001063 RID: 4195 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x00069D12 File Offset: 0x00067F12
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"profiling"
		};
	}

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x06001065 RID: 4197 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x00069D24 File Offset: 0x00067F24
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (this.cmdNetwork == null)
		{
			this.cmdNetwork = (SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand("profilenetwork", true) as ConsoleCmdProfileNetwork);
		}
		if (_params.Count != 1 || !_params[0].EqualsCaseInsensitive("stop"))
		{
			if (!Profiler.enabled)
			{
				this.profileNetwork = true;
				int num = 300;
				if (_params.Count > 0 && !int.TryParse(_params[0], out num))
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Not a valid integer for number of frames (\"{0}\")", num));
					return;
				}
				if (num < 10 || num > 3000)
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Number of frames needs to be within {0} and {1}", 10, 3000));
					return;
				}
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Enabled profiling for {0} frames (typically 5 - 10 seconds)", num));
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Profiler mem: {0}", Profiler.maxUsedMemory));
				Profiler.logFile = string.Format("{0}/profiling_{1:yyyy-MM-dd_HH-mm-ss}_unity.log", GameIO.GetApplicationPath(), DateTime.Now);
				Profiler.enableBinaryLog = true;
				Profiler.enabled = true;
				ThreadManager.StartCoroutine(this.stopProfilingLater(num));
				if (this.profileNetwork)
				{
					ConsoleCmdProfileNetwork consoleCmdProfileNetwork = this.cmdNetwork;
					if (consoleCmdProfileNetwork == null)
					{
						return;
					}
					consoleCmdProfileNetwork.resetData();
				}
			}
			return;
		}
		if (!Profiler.enabled)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Profiling not running.");
			return;
		}
		this.stopProfiling();
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Profiling stopped.");
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x00069EA2 File Offset: 0x000680A2
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator stopProfilingLater(int _frames)
	{
		int i = 0;
		while (i < _frames && Profiler.enabled)
		{
			yield return null;
			int num = i;
			i = num + 1;
		}
		if (Profiler.enabled)
		{
			this.stopProfiling();
			Log.Out("Profiling done");
		}
		yield break;
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x00069EB8 File Offset: 0x000680B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void stopProfiling()
	{
		Profiler.enabled = false;
		Profiler.logFile = null;
		if (this.profileNetwork)
		{
			ConsoleCmdProfileNetwork consoleCmdProfileNetwork = this.cmdNetwork;
			if (consoleCmdProfileNetwork == null)
			{
				return;
			}
			consoleCmdProfileNetwork.doProfileNetwork();
		}
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x00069EDE File Offset: 0x000680DE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Enable Unity profiling for 300 frames";
	}

	// Token: 0x04000B77 RID: 2935
	[PublicizedFrom(EAccessModifier.Private)]
	public ConsoleCmdProfileNetwork cmdNetwork;

	// Token: 0x04000B78 RID: 2936
	[PublicizedFrom(EAccessModifier.Private)]
	public bool profileNetwork;

	// Token: 0x04000B79 RID: 2937
	[PublicizedFrom(EAccessModifier.Private)]
	public const int FramesDefault = 300;

	// Token: 0x04000B7A RID: 2938
	[PublicizedFrom(EAccessModifier.Private)]
	public const int FramesMin = 10;

	// Token: 0x04000B7B RID: 2939
	[PublicizedFrom(EAccessModifier.Private)]
	public const int FramesMax = 3000;
}
