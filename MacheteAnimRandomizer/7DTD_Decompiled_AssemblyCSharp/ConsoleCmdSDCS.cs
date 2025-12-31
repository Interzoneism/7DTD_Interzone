using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x0200023B RID: 571
[Preserve]
public class ConsoleCmdSDCS : ConsoleCmdAbstract
{
	// Token: 0x06001097 RID: 4247 RVA: 0x0006A6C5 File Offset: 0x000688C5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Change a player's sex, race, and variant for SDCS testing\nUsage:\n   sdcs sex male\n   sdcs race white\n   sdcs variant 4\n";
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x0006A6CC File Offset: 0x000688CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"sdcs"
		};
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x0006A6DC File Offset: 0x000688DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Control entity sex, race, and variant";
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x0006A6E4 File Offset: 0x000688E4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count < 2)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("sdcs requires a control type (sex, race, variant) and a value");
			return;
		}
		if (GameManager.Instance.World.GetLocalPlayers().Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No local players found");
			return;
		}
		EntityPlayer entityPlayer = GameManager.Instance.World.GetLocalPlayers()[0];
		ConsoleCmdSDCS.cTypes cTypes;
		if (!Enum.TryParse<ConsoleCmdSDCS.cTypes>(_params[0], true, out cTypes))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid control type");
			return;
		}
		string text = _params[1];
		EModelSDCS component = entityPlayer.GetComponent<EModelSDCS>();
		if (component == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No SDCS model found");
			return;
		}
		switch (cTypes)
		{
		case ConsoleCmdSDCS.cTypes.Sex:
		{
			ConsoleCmdSDCS.sTypes sTypes;
			if (!Enum.TryParse<ConsoleCmdSDCS.sTypes>(_params[1], true, out sTypes))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid race '" + _params[1] + "'");
				return;
			}
			bool sex = false;
			if (sTypes == ConsoleCmdSDCS.sTypes.male)
			{
				sex = true;
			}
			component.SetSex(sex);
			component.SetRace("White");
			component.SetVariant(1);
			return;
		}
		case ConsoleCmdSDCS.cTypes.Race:
		{
			ConsoleCmdSDCS.rTypes rTypes;
			if (!Enum.TryParse<ConsoleCmdSDCS.rTypes>(_params[1], true, out rTypes))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid race '" + _params[1] + "'");
				return;
			}
			component.SetRace(_params[1]);
			component.SetVariant(1);
			return;
		}
		case ConsoleCmdSDCS.cTypes.Variant:
		{
			int num;
			if (!StringParsers.TryParseSInt32(_params[1], out num, 0, -1, NumberStyles.Integer))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid variant number " + _params[1]);
			}
			if (num > 4)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Invalid variant number {0}", num));
			}
			component.SetVariant(num);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x0200023C RID: 572
	[PublicizedFrom(EAccessModifier.Private)]
	public enum cTypes
	{
		// Token: 0x04000B82 RID: 2946
		Sex,
		// Token: 0x04000B83 RID: 2947
		Race,
		// Token: 0x04000B84 RID: 2948
		Variant
	}

	// Token: 0x0200023D RID: 573
	[PublicizedFrom(EAccessModifier.Private)]
	public enum rTypes
	{
		// Token: 0x04000B86 RID: 2950
		White,
		// Token: 0x04000B87 RID: 2951
		Black,
		// Token: 0x04000B88 RID: 2952
		Asian,
		// Token: 0x04000B89 RID: 2953
		Hispanic,
		// Token: 0x04000B8A RID: 2954
		MiddleEastern
	}

	// Token: 0x0200023E RID: 574
	[PublicizedFrom(EAccessModifier.Private)]
	public enum sTypes
	{
		// Token: 0x04000B8C RID: 2956
		male,
		// Token: 0x04000B8D RID: 2957
		female
	}
}
