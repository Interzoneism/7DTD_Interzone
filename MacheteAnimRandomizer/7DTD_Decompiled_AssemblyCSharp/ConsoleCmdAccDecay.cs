using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001AD RID: 429
[Preserve]
public class ConsoleCmdAccDecay : ConsoleCmdAbstract
{
	// Token: 0x06000D24 RID: 3364 RVA: 0x0005845E File Offset: 0x0005665E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"AccDecay",
			"SetAccDecay",
			"SetAccuracyDecay",
			"sad"
		};
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000D25 RID: 3365 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AllowedInMainMenu
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x00058486 File Offset: 0x00056686
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Accuracy Decay for guns, show/hide/reset/<Decimal value>";
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x0005848D File Offset: 0x0005668D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "reset - apply the default settings for this command\n<Decimal Value> - Sets the decay constant for accuracy calculations using ItemActionRanged";
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x00058494 File Offset: 0x00056694
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count != 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("ItemActionRanged.DecayConstant: {0}", ItemActionRanged.AccuracyUpdateDecayConstant));
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("ItemActionRanged.LogOldAccuracy: {0}", ItemActionRanged.LogOldAccuracy));
			return;
		}
		float num;
		if (float.TryParse(_params[0], out num))
		{
			ItemActionRanged.AccuracyUpdateDecayConstant = num;
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Set ItemActionRanged.DecayConstant to {0} and reset old accuracy for checking", num));
			return;
		}
		if (_params[0].ToLower() == "reset")
		{
			ItemActionRanged.LogOldAccuracy = false;
			ItemActionRanged.AccuracyUpdateDecayConstant = 9.1f;
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("ItemActionRanged.DecayConstant: {0}", ItemActionRanged.AccuracyUpdateDecayConstant));
	}
}
