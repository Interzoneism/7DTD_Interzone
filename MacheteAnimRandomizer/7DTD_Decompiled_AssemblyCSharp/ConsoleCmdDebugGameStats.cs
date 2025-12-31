using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020001D3 RID: 467
[Preserve]
public class ConsoleCmdDebugGameStats : ConsoleCmdAbstract
{
	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000E05 RID: 3589 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x0005D2A5 File Offset: 0x0005B4A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"debuggamestats"
		};
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x0005D2B5 File Offset: 0x0005B4B5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "GameStats commands";
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x0005D2BC File Offset: 0x0005B4BC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count >= 1)
		{
			if (_params[0].ToUpper() == "LOG")
			{
				bool flag;
				if (_params.Count >= 2 && bool.TryParse(_params[1], out flag))
				{
					if (flag)
					{
						this.logFile = Path.Join(PlatformApplicationManager.Application.temporaryCachePath, "GameStats.tsv");
						this.stringBuilder = new StringBuilder();
						this.stringBuilder.AppendLine(DebugGameStats.GetHeader('\t'));
						File.AppendAllText(this.logFile, this.stringBuilder.ToString());
						DebugGameStats.StartStatisticsUpdate(new DebugGameStats.StatisticsUpdatedCallback(this.logDebugGameStats));
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Started logging debug stats to " + this.logFile + ".");
						return;
					}
					DebugGameStats.StopStatisticsUpdate();
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Stopped logging debug stats to " + this.logFile + ".");
					return;
				}
			}
			else if (_params[0].ToUpper() == "PRINT")
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(DebugGameStats.GetHeader(','));
				stringBuilder.AppendLine(DebugGameStats.GetCurrentStatsString(','));
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(stringBuilder.ToString());
				return;
			}
		}
		Log.Out("Incorrect params, expected 'log [true|false]' or 'print'");
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x0005D418 File Offset: 0x0005B618
	[PublicizedFrom(EAccessModifier.Private)]
	public void logDebugGameStats(Dictionary<string, string> statisticsDictionary)
	{
		this.stringBuilder.Clear();
		foreach (KeyValuePair<string, string> keyValuePair in statisticsDictionary)
		{
			this.stringBuilder.Append(keyValuePair.Value);
			this.stringBuilder.Append('\t');
		}
		this.stringBuilder.AppendLine();
		File.AppendAllText(this.logFile, this.stringBuilder.ToString());
	}

	// Token: 0x04000AF3 RID: 2803
	[PublicizedFrom(EAccessModifier.Private)]
	public string logFile;

	// Token: 0x04000AF4 RID: 2804
	[PublicizedFrom(EAccessModifier.Private)]
	public StringBuilder stringBuilder;
}
