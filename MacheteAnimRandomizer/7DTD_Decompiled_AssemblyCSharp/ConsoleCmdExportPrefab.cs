using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x020001E0 RID: 480
[Preserve]
public class ConsoleCmdExportPrefab : ConsoleCmdAbstract
{
	// Token: 0x06000E52 RID: 3666 RVA: 0x0005DDBC File Offset: 0x0005BFBC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			ConsoleCmdExportPrefab.CommandName
		};
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000E53 RID: 3667 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AllowedInMainMenu
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x0005DDCC File Offset: 0x0005BFCC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Exports a prefab from a world area";
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x0005DDD3 File Offset: 0x0005BFD3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n  exportprefab <name> <x1> <y1> <z1> <x2> <y2> <z2> [part]\nExports a prefab with the given name from a box defined by the coordinate pair of two corners\nIf the optional parameter 'part' is 'true' it will export into the 'Parts' subfolder of the prefabs folder";
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x0005DDDC File Offset: 0x0005BFDC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count < 7 || _params.Count > 8)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Illegal number of parameters");
			return;
		}
		bool flag = false;
		if (_params.Count == 8)
		{
			flag = ConsoleHelper.ParseParamBool(_params[7], true);
		}
		string text = _params[0];
		int x;
		if (!StringParsers.TryParseSInt32(_params[1], out x, 0, -1, NumberStyles.Integer))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("x1 coordinate is not a valid integer");
			return;
		}
		int y;
		if (!StringParsers.TryParseSInt32(_params[2], out y, 0, -1, NumberStyles.Integer))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("y1 coordinate is not a valid integer");
			return;
		}
		int z;
		if (!StringParsers.TryParseSInt32(_params[3], out z, 0, -1, NumberStyles.Integer))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("z1 coordinate is not a valid integer");
			return;
		}
		Vector3i posStart = new Vector3i(x, y, z);
		if (!StringParsers.TryParseSInt32(_params[4], out x, 0, -1, NumberStyles.Integer))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("x2 coordinate is not a valid integer");
			return;
		}
		if (!StringParsers.TryParseSInt32(_params[5], out y, 0, -1, NumberStyles.Integer))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("y2 coordinate is not a valid integer");
			return;
		}
		if (!StringParsers.TryParseSInt32(_params[6], out z, 0, -1, NumberStyles.Integer))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("z2 coordinate is not a valid integer");
			return;
		}
		Vector3i posEnd = new Vector3i(x, y, z);
		if (Prefab.PrefabExists(text))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("A prefab with the name \"" + text + "\" already exists.");
			return;
		}
		Prefab prefab = new Prefab();
		prefab.location = Prefab.LocationForNewPrefab(text, flag ? "Parts" : null);
		prefab.copyFromWorld(GameManager.Instance.World, posStart, posEnd);
		if (prefab.Save(prefab.location, true))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Prefab saved to " + prefab.location.ToString());
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Prefab could not be saved");
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x0005DFB4 File Offset: 0x0005C1B4
	public static string BuildCommandString(string _prefabName, Vector3i _startPos, Vector3i _endPos, bool _asPart)
	{
		return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", new object[]
		{
			ConsoleCmdExportPrefab.CommandName,
			_prefabName,
			_startPos.x,
			_startPos.y,
			_startPos.z,
			_endPos.x,
			_endPos.y,
			_endPos.z,
			_asPart
		});
	}

	// Token: 0x04000AF9 RID: 2809
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string CommandName = "exportprefab";
}
