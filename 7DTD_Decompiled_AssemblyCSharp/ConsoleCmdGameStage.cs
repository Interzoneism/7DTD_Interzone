using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020001E4 RID: 484
[Preserve]
public class ConsoleCmdGameStage : ConsoleCmdAbstract
{
	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000E6D RID: 3693 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000E6E RID: 3694 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000E6F RID: 3695 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x0005E321 File Offset: 0x0005C521
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"gamestage"
		};
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x0005E331 File Offset: 0x0005C531
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Shows the gamestage of the local player";
	}

	// Token: 0x06000E72 RID: 3698 RVA: 0x0005E338 File Offset: 0x0005C538
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer != null)
		{
			string text = string.Empty;
			BiomeDefinition biomeStandingOn = primaryPlayer.biomeStandingOn;
			if (biomeStandingOn != null)
			{
				text = biomeStandingOn.m_sBiomeName;
			}
			Log.Out("Player gamestage {0}, lootstage {1}, level {2}, days alive {3}, difficulty {4}, diff bonus {5}, biome {6}", new object[]
			{
				primaryPlayer.gameStage,
				primaryPlayer.GetLootStage(0f, 0f),
				primaryPlayer.Progression.Level,
				(long)((primaryPlayer.world.worldTime - primaryPlayer.gameStageBornAtWorldTime) / 24000UL),
				GameStats.GetInt(EnumGameStats.GameDifficulty),
				GameStats.GetFloat(EnumGameStats.GameDifficultyBonus).ToCultureInvariantString(),
				text
			});
		}
	}
}
