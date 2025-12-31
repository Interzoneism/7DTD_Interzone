using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000256 RID: 598
[Preserve]
public class ConsoleCmdSmoothPOI : ConsoleCmdAbstract
{
	// Token: 0x06001123 RID: 4387 RVA: 0x0006BF4F File Offset: 0x0006A14F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"smoothpoi"
		};
	}

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x06001124 RID: 4388 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AllowedInMainMenu
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06001125 RID: 4389 RVA: 0x0006BF60 File Offset: 0x0006A160
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!PrefabEditModeManager.Instance.IsActive())
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command can only be used in the prefab editor");
			return;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command can only be used on the host");
			return;
		}
		string text = "land";
		if (_params.Count >= 1)
		{
			text = _params[0];
		}
		int passes = 1;
		if (_params.Count == 2)
		{
			passes = int.Parse(_params[1]);
		}
		bool land = text.Equals("land");
		DateTime now = DateTime.Now;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Starting POI smoothing pass");
		PrefabHelpers.SmoothPOI(passes, land);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Finished POI smoothing at " + DateTime.Now.ToCultureInvariantString());
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Smoothing action took {0} ms", (DateTime.Now - now).TotalMilliseconds));
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x0006C047 File Offset: 0x0006A247
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Smoothens the POI";
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x0006C04E File Offset: 0x0006A24E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "\n\t\t\t|Usage:\n\t\t\t|  smoothpoi [mode] [passes]\n\t\t\t|Mode defaults to \"land\", also accepts \"air\".\n            |Passes defaults to 1.\n            |Smoothed area can be restricted with the blue selection.\n\t\t\t".Unindent(true);
	}
}
