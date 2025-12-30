using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020001BA RID: 442
[Preserve]
public class ConsoleCmdBuff : ConsoleCmdAbstract
{
	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000D78 RID: 3448 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000D79 RID: 3449 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x0005A72F File Offset: 0x0005892F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"buff"
		};
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x0005A740 File Offset: 0x00058940
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!_senderInfo.IsLocalGame)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command can only be used on clients, use \"buffplayer\" instead for other players / remote clients");
			return;
		}
		if (_params.Count == 1)
		{
			EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			if (primaryPlayer != null)
			{
				EntityBuffs.BuffStatus buffStatus = primaryPlayer.Buffs.AddBuff(_params[0], -1, true, false, -1f);
				if (buffStatus != EntityBuffs.BuffStatus.Added)
				{
					switch (buffStatus)
					{
					case EntityBuffs.BuffStatus.FailedInvalidName:
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Buff failed: buff \"" + _params[0] + "\" unknown");
						ConsoleCmdBuff.PrintAvailableBuffNames();
						return;
					case EntityBuffs.BuffStatus.FailedImmune:
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Buff failed: entity is immune to \"" + _params[0] + "\"");
						return;
					case EntityBuffs.BuffStatus.FailedFriendlyFire:
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Buff failed: entity is friendly");
						return;
					case EntityBuffs.BuffStatus.FailedEditor:
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Buff failed: buff " + _params[0] + " not allowed in editor");
						return;
					case EntityBuffs.BuffStatus.FailedGameStat:
						SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Buff failed: missing required game stat.");
						return;
					default:
						return;
					}
				}
			}
		}
		else
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("buff requires a buff name as the only argument!");
			ConsoleCmdBuff.PrintAvailableBuffNames();
		}
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x0005A867 File Offset: 0x00058A67
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Applies a buff to the local player";
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x0005A870 File Offset: 0x00058A70
	public static void PrintAvailableBuffNames()
	{
		SortedDictionary<string, BuffClass> sortedDictionary = new SortedDictionary<string, BuffClass>(BuffManager.Buffs, StringComparer.OrdinalIgnoreCase);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Available buffs:");
		foreach (KeyValuePair<string, BuffClass> keyValuePair in sortedDictionary)
		{
			if (keyValuePair.Key.Equals(keyValuePair.Value.LocalizedName))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(" - " + keyValuePair.Key);
			}
			else
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Concat(new string[]
				{
					" - ",
					keyValuePair.Key,
					" (",
					keyValuePair.Value.LocalizedName,
					")"
				}));
			}
		}
	}
}
