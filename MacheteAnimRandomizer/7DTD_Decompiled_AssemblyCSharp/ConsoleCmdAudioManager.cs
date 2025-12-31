using System;
using System.Collections.Generic;
using System.Globalization;
using Audio;
using UnityEngine.Scripting;

// Token: 0x020001B6 RID: 438
[Preserve]
public class ConsoleCmdAudioManager : ConsoleCmdAbstract
{
	// Token: 0x06000D57 RID: 3415 RVA: 0x0005967B File Offset: 0x0005787B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"audio"
		};
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000D58 RID: 3416 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000D59 RID: 3417 RVA: 0x0005968B File Offset: 0x0005788B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Watch audio stats";
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x00059692 File Offset: 0x00057892
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Just type audio and hit enter for the info.\n";
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x00059699 File Offset: 0x00057899
	[PublicizedFrom(EAccessModifier.Private)]
	public void DisplayHelp()
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No help yet");
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x000596AC File Offset: 0x000578AC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			this.DisplayHelp();
			return;
		}
		if (_params.Count == 2)
		{
			if (_params[0].EqualsCaseInsensitive("occlusion"))
			{
				if (_params[1].EqualsCaseInsensitive("on"))
				{
					Manager.occlusionsOn = true;
					return;
				}
				if (_params[1].EqualsCaseInsensitive("off"))
				{
					Manager.occlusionsOn = false;
					return;
				}
			}
			else
			{
				if (_params[0].EqualsCaseInsensitive("hitdelay"))
				{
					int num = 0;
					int.TryParse(_params[1], out num);
					EntityAlive.HitDelay = (ulong)((long)num);
					return;
				}
				if (_params[0].EqualsCaseInsensitive("hitdis"))
				{
					float hitSoundDistance = 0f;
					StringParsers.TryParseFloat(_params[1], out hitSoundDistance, 0, -1, NumberStyles.Any);
					EntityAlive.HitSoundDistance = hitSoundDistance;
					return;
				}
				if (_params[0].EqualsCaseInsensitive("play"))
				{
					Manager.Play(GameManager.Instance.World.GetPrimaryPlayer(), _params[1], 1f, false);
					return;
				}
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid Input");
			this.DisplayHelp();
		}
	}
}
