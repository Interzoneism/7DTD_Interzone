using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Platform
{
	// Token: 0x02001841 RID: 6209
	public static class PlatformApplicationManager
	{
		// Token: 0x170014DE RID: 5342
		// (get) Token: 0x0600B838 RID: 47160 RVA: 0x00468E49 File Offset: 0x00467049
		// (set) Token: 0x0600B839 RID: 47161 RVA: 0x00468E50 File Offset: 0x00467050
		public static IPlatformApplication Application { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x170014DF RID: 5343
		// (get) Token: 0x0600B83A RID: 47162 RVA: 0x00468E58 File Offset: 0x00467058
		public static bool IsRestartRequired
		{
			get
			{
				return PlatformApplicationManager.isRestartRequired;
			}
		}

		// Token: 0x0600B83B RID: 47163 RVA: 0x00468E5F File Offset: 0x0046705F
		public static bool Init()
		{
			PlatformApplicationManager.Application = IPlatformApplication.Create();
			return true;
		}

		// Token: 0x0600B83C RID: 47164 RVA: 0x00468E6C File Offset: 0x0046706C
		public static void SetRestartRequired()
		{
			PlatformApplicationManager.isRestartRequired = PlatformOptimizations.RestartProcessSupported;
			Log.Out(string.Format("[PlatformApplication] restart required = {0}", PlatformApplicationManager.isRestartRequired));
		}

		// Token: 0x0600B83D RID: 47165 RVA: 0x00468E91 File Offset: 0x00467091
		public static bool CheckRestartCoroutineReady()
		{
			return PlatformApplicationManager.isRestartRequired && !PlatformApplicationManager.isRestarting && !InviteManager.Instance.IsConnectingToInvite();
		}

		// Token: 0x0600B83E RID: 47166 RVA: 0x00468EB0 File Offset: 0x004670B0
		public static IEnumerator CheckRestartCoroutine(bool loadSaveGame = false)
		{
			if (!PlatformApplicationManager.CheckRestartCoroutineReady())
			{
				yield break;
			}
			PlatformApplicationManager.isRestartRequired = false;
			PlatformApplicationManager.isRestarting = true;
			try
			{
				yield return GameManager.Instance.ShowExitingGameUICoroutine();
				PlatformApplicationManager.RestartProcess(loadSaveGame);
			}
			finally
			{
				Log.Error("[PlatformApplication] failed to restart process.");
				PlatformApplicationManager.isRestarting = false;
			}
			yield break;
			yield break;
		}

		// Token: 0x0600B83F RID: 47167 RVA: 0x00468EBF File Offset: 0x004670BF
		[PublicizedFrom(EAccessModifier.Private)]
		public static string[] RemoveFirstRunArguments(string[] argv)
		{
			return (from arg in argv
			where !arg.StartsWith("-LoadSaveGame=", StringComparison.OrdinalIgnoreCase)
			select arg).ToArray<string>();
		}

		// Token: 0x0600B840 RID: 47168 RVA: 0x00468EEC File Offset: 0x004670EC
		[PublicizedFrom(EAccessModifier.Private)]
		public static void RestartProcess(bool loadSaveGame)
		{
			List<string> list = new List<string>();
			list.AddRange(PlatformApplicationManager.RemoveFirstRunArguments(GameStartupHelper.RemoveTemporaryArguments(GameStartupHelper.GetCommandLineArgs())));
			list.Add("[REMOVE_ON_RESTART]");
			list.Add("-skipintro");
			list.Add(LaunchPrefs.SkipNewsScreen.ToCommandLine(true));
			if (PlatformOptimizations.RestartAfterRwg && loadSaveGame)
			{
				Log.Out("[LoadSaveGame] After restart should load: worldName=" + GamePrefs.GetString(EnumGamePrefs.GameWorld) + " saveName=" + GamePrefs.GetString(EnumGamePrefs.GameName));
				list.Add(LaunchPrefs.LoadSaveGame.ToCommandLine(true));
			}
			list.AddRange(InviteManager.Instance.GetCommandLineArguments());
			list.AddRange(PlatformManager.NativePlatform.GetArgumentsForRelaunch());
			try
			{
				GamePrefs.Instance.Save();
				SaveDataUtils.Destroy();
				PlatformManager.Destroy();
			}
			catch (Exception e)
			{
				Log.Error("Exception thrown while preparing for process restart. This may cause errors in the next run");
				Log.Exception(e);
			}
			PlatformApplicationManager.Application.RestartProcess(list.ToArray());
		}

		// Token: 0x0600B841 RID: 47169 RVA: 0x00468FE0 File Offset: 0x004671E0
		public static EPlatformLoadSaveGameState GetLoadSaveGameState()
		{
			if (!LaunchPrefs.LoadSaveGame.Value)
			{
				return EPlatformLoadSaveGameState.Done;
			}
			if (PlatformApplicationManager.loadSaveGameState != EPlatformLoadSaveGameState.Init)
			{
				return PlatformApplicationManager.loadSaveGameState;
			}
			string worldName = GamePrefs.GetString(EnumGamePrefs.GameWorld);
			if (!GameIO.DoesWorldExist(worldName))
			{
				Log.Warning("[LoadSaveGame] World does not exist: " + worldName);
				return PlatformApplicationManager.loadSaveGameState = EPlatformLoadSaveGameState.Done;
			}
			string gameName = GamePrefs.GetString(EnumGamePrefs.GameName);
			bool found = false;
			bool isArchived = false;
			GameIO.GetPlayerSaves(delegate(string foundSaveName, string foundWorldName, DateTime _, WorldState _, bool foundIsArchived)
			{
				if (foundSaveName.EqualsCaseInsensitive(gameName) && foundWorldName.EqualsCaseInsensitive(worldName))
				{
					found = true;
					isArchived = foundIsArchived;
				}
			}, true);
			if (!found)
			{
				Log.Out(string.Concat(new string[]
				{
					"[LoadSaveGame] Creating new save game '",
					gameName,
					"' from the world '",
					worldName,
					"'."
				}));
				return PlatformApplicationManager.loadSaveGameState = EPlatformLoadSaveGameState.NewGameOpen;
			}
			if (isArchived)
			{
				Log.Warning(string.Concat(new string[]
				{
					"[LoadSaveGame] Can not load archived save '",
					gameName,
					"' (world '",
					worldName,
					"')."
				}));
				return PlatformApplicationManager.loadSaveGameState = EPlatformLoadSaveGameState.Done;
			}
			Log.Out(string.Concat(new string[]
			{
				"[LoadSaveGame] Loading existing save game '",
				gameName,
				"' (world '",
				worldName,
				"')."
			}));
			return PlatformApplicationManager.loadSaveGameState = EPlatformLoadSaveGameState.ContinueGameOpen;
		}

		// Token: 0x0600B842 RID: 47170 RVA: 0x0046914C File Offset: 0x0046734C
		public static void AdvanceLoadSaveGameStateFrom(EPlatformLoadSaveGameState previousState)
		{
			if (PlatformApplicationManager.loadSaveGameState != previousState)
			{
				Log.Error(string.Format("[LoadSaveGame] Expected advance from {0} but was {1}", PlatformApplicationManager.loadSaveGameState, previousState));
				PlatformApplicationManager.loadSaveGameState = EPlatformLoadSaveGameState.Done;
			}
			EPlatformLoadSaveGameState eplatformLoadSaveGameState;
			switch (previousState)
			{
			case EPlatformLoadSaveGameState.Init:
				throw new NotSupportedException("Init state should be manually advanced from because it branches.");
			case EPlatformLoadSaveGameState.NewGameOpen:
				eplatformLoadSaveGameState = EPlatformLoadSaveGameState.NewGameSelect;
				break;
			case EPlatformLoadSaveGameState.NewGameSelect:
				eplatformLoadSaveGameState = EPlatformLoadSaveGameState.NewGamePlay;
				break;
			case EPlatformLoadSaveGameState.NewGamePlay:
				eplatformLoadSaveGameState = EPlatformLoadSaveGameState.Done;
				break;
			case EPlatformLoadSaveGameState.ContinueGameOpen:
				eplatformLoadSaveGameState = EPlatformLoadSaveGameState.ContinueGameSelect;
				break;
			case EPlatformLoadSaveGameState.ContinueGameSelect:
				eplatformLoadSaveGameState = EPlatformLoadSaveGameState.ContinueGamePlay;
				break;
			case EPlatformLoadSaveGameState.ContinueGamePlay:
				eplatformLoadSaveGameState = EPlatformLoadSaveGameState.Done;
				break;
			case EPlatformLoadSaveGameState.Done:
				throw new NotSupportedException("Can't advance from the final state.");
			default:
				throw new ArgumentOutOfRangeException();
			}
			PlatformApplicationManager.loadSaveGameState = eplatformLoadSaveGameState;
			Log.Out(string.Format("[LoadSaveGame] Advanced to state {0} (was {1})", PlatformApplicationManager.loadSaveGameState, previousState));
		}

		// Token: 0x0600B843 RID: 47171 RVA: 0x00469207 File Offset: 0x00467407
		public static void SetFailedLoadSaveGame()
		{
			if (PlatformApplicationManager.loadSaveGameState == EPlatformLoadSaveGameState.Done)
			{
				return;
			}
			Log.Warning(string.Format("[LoadSaveGame] Failed to automate creating or loading the save game. State: {0}", PlatformApplicationManager.loadSaveGameState));
			PlatformApplicationManager.loadSaveGameState = EPlatformLoadSaveGameState.Done;
		}

		// Token: 0x04009034 RID: 36916
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool isRestartRequired;

		// Token: 0x04009035 RID: 36917
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool isRestarting;

		// Token: 0x04009036 RID: 36918
		[PublicizedFrom(EAccessModifier.Private)]
		public static EPlatformLoadSaveGameState loadSaveGameState;
	}
}
