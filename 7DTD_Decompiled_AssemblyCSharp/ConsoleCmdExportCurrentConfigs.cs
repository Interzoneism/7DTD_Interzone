using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine.Scripting;

// Token: 0x020001DE RID: 478
[Preserve]
public class ConsoleCmdExportCurrentConfigs : ConsoleCmdAbstract
{
	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000E4A RID: 3658 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x0005DC0E File Offset: 0x0005BE0E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Exports the current game config XMLs";
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x0005DC15 File Offset: 0x0005BE15
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Exports all game config XMLs as they are currently used (including applied\npatches from mods) to the folder \"Configs\" in the save folder of the game.\nIf run from the main menu it exports the XUi configs for the menu, if run\nfrom a game session will export all others.";
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x0005DC1C File Offset: 0x0005BE1C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"exportcurrentconfigs"
		};
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x0005DC2C File Offset: 0x0005BE2C
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		string text;
		if (GameManager.Instance.World == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No game started, exporting XUi menu and rwgmixer configs");
			text = GameIO.GetUserGameDataDir() + "/ExportedConfigs";
			if (SdDirectory.Exists(text))
			{
				SdDirectory.Delete(text, true);
			}
			Thread.Sleep(50);
			SdDirectory.CreateDirectory(text);
			string[] array = new string[]
			{
				"rwgmixer",
				"loadingscreen",
				"XUi_Menu/styles",
				"XUi_Menu/controls",
				"XUi_Menu/windows",
				"XUi_Menu/xui"
			};
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				XmlFile xml = null;
				ThreadManager.RunCoroutineSync(XmlPatcher.LoadAndPatchConfig(text2, delegate(XmlFile _file)
				{
					xml = _file;
				}));
				string path = text + "/" + text2 + ".xml";
				if (text2.IndexOf('/') >= 0)
				{
					string directoryName = Path.GetDirectoryName(path);
					if (!SdDirectory.Exists(directoryName))
					{
						SdDirectory.CreateDirectory(directoryName);
					}
				}
				xml.SerializeToFile(path, false, null);
			}
		}
		else
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				string text3 = GameIO.GetSaveGameDir() + "/ConfigsDump";
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Patched XMLs are automatically dumped on game start to a ConfigsDump subdirectory of the save game.");
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("In this case you can find the folder at: " + text3);
				GameIO.OpenExplorer(text3);
				return;
			}
			text = GameIO.GetSaveGameLocalDir() + "/ConfigsDump";
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Configs exported to " + text);
		GameIO.OpenExplorer(text);
	}
}
