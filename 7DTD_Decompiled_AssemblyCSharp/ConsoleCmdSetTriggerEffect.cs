using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000246 RID: 582
[Preserve]
public class ConsoleCmdSetTriggerEffect : ConsoleCmdAbstract
{
	// Token: 0x060010C6 RID: 4294 RVA: 0x0006AFD9 File Offset: 0x000691D9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"sette"
		};
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x060010C7 RID: 4295 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x060010C8 RID: 4296 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x0006AFE9 File Offset: 0x000691E9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Sets the UseTriggerEffects flag, if true controller trigger effects are to be used";
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x0006AFF0 File Offset: 0x000691F0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (GameManager.IsDedicatedServer)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Client only setting, please execute as a client");
			return;
		}
		if (_params.Count > 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: sette <on/true/y/1/off/false/n/0>");
			return;
		}
		bool flag = false;
		if (_params.Count != 0)
		{
			flag = GamePrefs.GetBool(EnumGamePrefs.OptionsControllerTriggerEffects);
		}
		if (_params.Count == 1)
		{
			try
			{
				flag = ConsoleHelper.ParseParamBool(_params[0], false);
			}
			catch
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Usage: sette <on/true/y/1/off/false/n/0>");
				return;
			}
		}
		GamePrefs.Set(EnumGamePrefs.OptionsControllerTriggerEffects, flag);
		GamePrefs.Instance.Save();
		foreach (EntityPlayerLocal entityPlayerLocal in GameManager.Instance.World.GetLocalPlayers())
		{
			GameManager.Instance.triggerEffectManager.PollSetting();
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("UseTriggerEffects now set to: {0}", flag));
	}
}
