using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000223 RID: 547
[Preserve]
public class ConsoleCmdPois : ConsoleCmdAbstract
{
	// Token: 0x06000FFD RID: 4093 RVA: 0x00067458 File Offset: 0x00065658
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"pois"
		};
	}

	// Token: 0x06000FFE RID: 4094 RVA: 0x00067468 File Offset: 0x00065668
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Switches distant POIs on/off";
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x0006746F File Offset: 0x0006566F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Use on or off or only the command to toggle";
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x00067478 File Offset: 0x00065678
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		GameObject x = GameObject.Find("/PrefabsLOD");
		if (x != null)
		{
			ConsoleCmdPois.parentGO = x;
		}
		if (ConsoleCmdPois.parentGO == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Distant POIs not active!");
			return;
		}
		if (_params.Count == 0)
		{
			ConsoleCmdPois.parentGO.SetActive(!ConsoleCmdPois.parentGO.activeSelf);
		}
		else if (_params[0] == "on")
		{
			ConsoleCmdPois.parentGO.SetActive(true);
		}
		else if (_params[0] == "off")
		{
			ConsoleCmdPois.parentGO.SetActive(true);
		}
		else
		{
			int num;
			if (int.TryParse(_params[0], out num))
			{
				GameManager.Instance.prefabLODManager.SetPOIDistance(128 * num);
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Concat(new string[]
				{
					"Setting to POI chunk distance ",
					num.ToString(),
					" =",
					(128 * num).ToString(),
					"m"
				}));
				return;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown parameter");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("POIs set to " + (ConsoleCmdPois.parentGO.activeSelf ? "on" : "off"));
	}

	// Token: 0x04000B53 RID: 2899
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject parentGO;
}
