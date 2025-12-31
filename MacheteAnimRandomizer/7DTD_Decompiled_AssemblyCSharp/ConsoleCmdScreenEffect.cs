using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x020001D0 RID: 464
[Preserve]
public class ConsoleCmdScreenEffect : ConsoleCmdAbstract
{
	// Token: 0x06000DF4 RID: 3572 RVA: 0x0005CEFE File Offset: 0x0005B0FE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"ScreenEffect"
		};
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x0005CF0E File Offset: 0x0005B10E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Sets a screen effect";
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x0005CF15 File Offset: 0x0005B115
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "ScreenEffect [name] [intensity] [fade time]\nScreenEffect clear\nScreenEffect reload";
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x0005CF1C File Offset: 0x0005B11C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		List<EntityPlayerLocal> localPlayers = GameManager.Instance.World.GetLocalPlayers();
		if (_params[0] == "clear")
		{
			for (int i = 0; i < localPlayers.Count; i++)
			{
				localPlayers[i].ScreenEffectManager.DisableScreenEffects();
			}
			return;
		}
		if (_params[0] == "reload")
		{
			for (int j = 0; j < localPlayers.Count; j++)
			{
				localPlayers[j].ScreenEffectManager.ResetEffects();
			}
			return;
		}
		float intensity = 0f;
		float fadeTime = 4f;
		if (_params.Count >= 2)
		{
			StringParsers.TryParseFloat(_params[1], out intensity, 0, -1, NumberStyles.Any);
		}
		if (_params.Count >= 3)
		{
			StringParsers.TryParseFloat(_params[2], out fadeTime, 0, -1, NumberStyles.Any);
		}
		for (int k = 0; k < localPlayers.Count; k++)
		{
			localPlayers[k].ScreenEffectManager.SetScreenEffect(_params[0], intensity, fadeTime);
		}
	}
}
