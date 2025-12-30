using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

// Token: 0x02000680 RID: 1664
public static class ModEvents
{
	// Token: 0x040028B7 RID: 10423
	public static readonly ModEvents.ModEvent<ModEvents.SGameFocusData> GameFocus = new ModEvents.ModEvent<ModEvents.SGameFocusData>("GameFocus");

	// Token: 0x040028B8 RID: 10424
	public static readonly ModEvents.ModEvent<ModEvents.SGameAwakeData> GameAwake = new ModEvents.ModEvent<ModEvents.SGameAwakeData>("GameAwake");

	// Token: 0x040028B9 RID: 10425
	public static readonly ModEvents.ModEvent<ModEvents.SGameStartingData> GameStarting = new ModEvents.ModEvent<ModEvents.SGameStartingData>("GameStarting");

	// Token: 0x040028BA RID: 10426
	public static readonly ModEvents.ModEventInterruptible<ModEvents.SMainMenuOpeningData> MainMenuOpening = new ModEvents.ModEventInterruptible<ModEvents.SMainMenuOpeningData>("MainMenuOpening");

	// Token: 0x040028BB RID: 10427
	public static readonly ModEvents.ModEvent<ModEvents.SMainMenuOpenedData> MainMenuOpened = new ModEvents.ModEvent<ModEvents.SMainMenuOpenedData>("MainMenuOpened");

	// Token: 0x040028BC RID: 10428
	public static readonly ModEvents.ModEvent<ModEvents.SGameStartDoneData> GameStartDone = new ModEvents.ModEvent<ModEvents.SGameStartDoneData>("GameStartDone");

	// Token: 0x040028BD RID: 10429
	public static readonly ModEvents.ModEvent<ModEvents.SCreateWorldDoneData> CreateWorldDone = new ModEvents.ModEvent<ModEvents.SCreateWorldDoneData>("CreateWorldDone");

	// Token: 0x040028BE RID: 10430
	public static readonly ModEvents.ModEvent<ModEvents.SGameUpdateData> GameUpdate = new ModEvents.ModEvent<ModEvents.SGameUpdateData>("GameUpdate");

	// Token: 0x040028BF RID: 10431
	public static readonly ModEvents.ModEvent<ModEvents.SWorldShuttingDownData> WorldShuttingDown = new ModEvents.ModEvent<ModEvents.SWorldShuttingDownData>("WorldShuttingDown");

	// Token: 0x040028C0 RID: 10432
	public static readonly ModEvents.ModEvent<ModEvents.SGameShutdownData> GameShutdown = new ModEvents.ModEvent<ModEvents.SGameShutdownData>("GameShutdown");

	// Token: 0x040028C1 RID: 10433
	public static readonly ModEvents.ModEvent<ModEvents.SServerRegisteredData> ServerRegistered = new ModEvents.ModEvent<ModEvents.SServerRegisteredData>("ServerRegistered");

	// Token: 0x040028C2 RID: 10434
	public static readonly ModEvents.ModEvent<ModEvents.SUnityUpdateData> UnityUpdate = new ModEvents.ModEvent<ModEvents.SUnityUpdateData>("UnityUpdate");

	// Token: 0x040028C3 RID: 10435
	public static readonly ModEvents.ModEventInterruptible<ModEvents.SPlayerLoginData> PlayerLogin = new ModEvents.ModEventInterruptible<ModEvents.SPlayerLoginData>("PlayerLogin");

	// Token: 0x040028C4 RID: 10436
	public static readonly ModEvents.ModEvent<ModEvents.SPlayerJoinedGameData> PlayerJoinedGame = new ModEvents.ModEvent<ModEvents.SPlayerJoinedGameData>("PlayerJoinedGame");

	// Token: 0x040028C5 RID: 10437
	public static readonly ModEvents.ModEvent<ModEvents.SPlayerSpawningData> PlayerSpawning = new ModEvents.ModEvent<ModEvents.SPlayerSpawningData>("PlayerSpawning");

	// Token: 0x040028C6 RID: 10438
	public static readonly ModEvents.ModEvent<ModEvents.SPlayerSpawnedInWorldData> PlayerSpawnedInWorld = new ModEvents.ModEvent<ModEvents.SPlayerSpawnedInWorldData>("PlayerSpawnedInWorld");

	// Token: 0x040028C7 RID: 10439
	public static readonly ModEvents.ModEvent<ModEvents.SPlayerDisconnectedData> PlayerDisconnected = new ModEvents.ModEvent<ModEvents.SPlayerDisconnectedData>("PlayerDisconnected");

	// Token: 0x040028C8 RID: 10440
	public static readonly ModEvents.ModEvent<ModEvents.SSavePlayerDataData> SavePlayerData = new ModEvents.ModEvent<ModEvents.SSavePlayerDataData>("SavePlayerData");

	// Token: 0x040028C9 RID: 10441
	public static readonly ModEvents.ModEventInterruptible<ModEvents.SGameMessageData> GameMessage = new ModEvents.ModEventInterruptible<ModEvents.SGameMessageData>("GameMessage");

	// Token: 0x040028CA RID: 10442
	public static readonly ModEvents.ModEventInterruptible<ModEvents.SChatMessageData> ChatMessage = new ModEvents.ModEventInterruptible<ModEvents.SChatMessageData>("ChatMessage");

	// Token: 0x040028CB RID: 10443
	public static readonly ModEvents.ModEvent<ModEvents.SCalcChunkColorsDoneData> CalcChunkColorsDone = new ModEvents.ModEvent<ModEvents.SCalcChunkColorsDoneData>("CalcChunkColorsDone");

	// Token: 0x040028CC RID: 10444
	public static readonly ModEvents.ModEvent<ModEvents.SEntityKilledData> EntityKilled = new ModEvents.ModEvent<ModEvents.SEntityKilledData>("EntityKilled");

	// Token: 0x02000681 RID: 1665
	public struct SGameFocusData
	{
		// Token: 0x060031EF RID: 12783 RVA: 0x001544DF File Offset: 0x001526DF
		public SGameFocusData(bool _isFocused)
		{
			this.IsFocused = _isFocused;
		}

		// Token: 0x040028CD RID: 10445
		public readonly bool IsFocused;
	}

	// Token: 0x02000682 RID: 1666
	public struct SGameAwakeData
	{
	}

	// Token: 0x02000683 RID: 1667
	public struct SGameStartingData
	{
		// Token: 0x060031F0 RID: 12784 RVA: 0x001544E8 File Offset: 0x001526E8
		public SGameStartingData(bool _asServer)
		{
			this.AsServer = _asServer;
		}

		// Token: 0x040028CE RID: 10446
		public readonly bool AsServer;
	}

	// Token: 0x02000684 RID: 1668
	public struct SMainMenuOpeningData
	{
		// Token: 0x060031F1 RID: 12785 RVA: 0x001544F1 File Offset: 0x001526F1
		public SMainMenuOpeningData(bool _openedBefore)
		{
			this.OpenedBefore = _openedBefore;
		}

		// Token: 0x040028CF RID: 10447
		public readonly bool OpenedBefore;
	}

	// Token: 0x02000685 RID: 1669
	public struct SMainMenuOpenedData
	{
		// Token: 0x060031F2 RID: 12786 RVA: 0x001544FA File Offset: 0x001526FA
		public SMainMenuOpenedData(bool _firstTimeOpen)
		{
			this.FirstTimeOpen = _firstTimeOpen;
		}

		// Token: 0x040028D0 RID: 10448
		public readonly bool FirstTimeOpen;
	}

	// Token: 0x02000686 RID: 1670
	public struct SGameStartDoneData
	{
	}

	// Token: 0x02000687 RID: 1671
	public struct SCreateWorldDoneData
	{
	}

	// Token: 0x02000688 RID: 1672
	public struct SGameUpdateData
	{
	}

	// Token: 0x02000689 RID: 1673
	public struct SWorldShuttingDownData
	{
	}

	// Token: 0x0200068A RID: 1674
	public struct SGameShutdownData
	{
	}

	// Token: 0x0200068B RID: 1675
	public struct SServerRegisteredData
	{
	}

	// Token: 0x0200068C RID: 1676
	public struct SUnityUpdateData
	{
	}

	// Token: 0x0200068D RID: 1677
	public struct SPlayerLoginData
	{
		// Token: 0x060031F3 RID: 12787 RVA: 0x00154503 File Offset: 0x00152703
		public SPlayerLoginData(ClientInfo _clientInfo, string _compatibilityVersion)
		{
			this.ClientInfo = _clientInfo;
			this.CompatibilityVersion = _compatibilityVersion;
			this.CustomMessage = null;
		}

		// Token: 0x040028D1 RID: 10449
		public readonly ClientInfo ClientInfo;

		// Token: 0x040028D2 RID: 10450
		public readonly string CompatibilityVersion;

		// Token: 0x040028D3 RID: 10451
		public string CustomMessage;
	}

	// Token: 0x0200068E RID: 1678
	public struct SPlayerJoinedGameData
	{
		// Token: 0x060031F4 RID: 12788 RVA: 0x0015451A File Offset: 0x0015271A
		public SPlayerJoinedGameData(ClientInfo _clientInfo)
		{
			this.ClientInfo = _clientInfo;
		}

		// Token: 0x040028D4 RID: 10452
		public readonly ClientInfo ClientInfo;
	}

	// Token: 0x0200068F RID: 1679
	public readonly struct SPlayerSpawningData
	{
		// Token: 0x060031F5 RID: 12789 RVA: 0x00154523 File Offset: 0x00152723
		public SPlayerSpawningData(ClientInfo _clientInfo, int _chunkViewDim, PlayerProfile _playerProfile)
		{
			this.ClientInfo = _clientInfo;
			this.ChunkViewDim = _chunkViewDim;
			this.PlayerProfile = _playerProfile;
		}

		// Token: 0x040028D5 RID: 10453
		public readonly ClientInfo ClientInfo;

		// Token: 0x040028D6 RID: 10454
		public readonly int ChunkViewDim;

		// Token: 0x040028D7 RID: 10455
		public readonly PlayerProfile PlayerProfile;
	}

	// Token: 0x02000690 RID: 1680
	public struct SPlayerSpawnedInWorldData
	{
		// Token: 0x060031F6 RID: 12790 RVA: 0x0015453A File Offset: 0x0015273A
		public SPlayerSpawnedInWorldData(ClientInfo _clientInfo, bool _isLocalPlayer, int _entityId, RespawnType _respawnType, Vector3i _position)
		{
			this.ClientInfo = _clientInfo;
			this.IsLocalPlayer = _isLocalPlayer;
			this.EntityId = _entityId;
			this.RespawnType = _respawnType;
			this.Position = _position;
		}

		// Token: 0x040028D8 RID: 10456
		public readonly ClientInfo ClientInfo;

		// Token: 0x040028D9 RID: 10457
		public readonly bool IsLocalPlayer;

		// Token: 0x040028DA RID: 10458
		public readonly int EntityId;

		// Token: 0x040028DB RID: 10459
		public readonly RespawnType RespawnType;

		// Token: 0x040028DC RID: 10460
		public readonly Vector3i Position;
	}

	// Token: 0x02000691 RID: 1681
	public struct SPlayerDisconnectedData
	{
		// Token: 0x060031F7 RID: 12791 RVA: 0x00154561 File Offset: 0x00152761
		public SPlayerDisconnectedData(ClientInfo _clientInfo, bool _gameShuttingDown)
		{
			this.ClientInfo = _clientInfo;
			this.GameShuttingDown = _gameShuttingDown;
		}

		// Token: 0x040028DD RID: 10461
		public readonly ClientInfo ClientInfo;

		// Token: 0x040028DE RID: 10462
		public readonly bool GameShuttingDown;
	}

	// Token: 0x02000692 RID: 1682
	public struct SSavePlayerDataData
	{
		// Token: 0x060031F8 RID: 12792 RVA: 0x00154571 File Offset: 0x00152771
		public SSavePlayerDataData(ClientInfo _clientInfo, PlayerDataFile _playerDataFile)
		{
			this.ClientInfo = _clientInfo;
			this.PlayerDataFile = _playerDataFile;
		}

		// Token: 0x040028DF RID: 10463
		public readonly ClientInfo ClientInfo;

		// Token: 0x040028E0 RID: 10464
		public readonly PlayerDataFile PlayerDataFile;
	}

	// Token: 0x02000693 RID: 1683
	public struct SGameMessageData
	{
		// Token: 0x060031F9 RID: 12793 RVA: 0x00154581 File Offset: 0x00152781
		public SGameMessageData(ClientInfo _clientInfo, EnumGameMessages _messageType, string _mainName, string _secondaryName)
		{
			this.ClientInfo = _clientInfo;
			this.MessageType = _messageType;
			this.MainName = _mainName;
			this.SecondaryName = _secondaryName;
		}

		// Token: 0x040028E1 RID: 10465
		public readonly ClientInfo ClientInfo;

		// Token: 0x040028E2 RID: 10466
		public readonly EnumGameMessages MessageType;

		// Token: 0x040028E3 RID: 10467
		public readonly string MainName;

		// Token: 0x040028E4 RID: 10468
		public readonly string SecondaryName;
	}

	// Token: 0x02000694 RID: 1684
	public struct SChatMessageData
	{
		// Token: 0x060031FA RID: 12794 RVA: 0x001545A0 File Offset: 0x001527A0
		public SChatMessageData(ClientInfo _clientInfo, EChatType _chatType, int _senderEntityId, string _message, string _mainName, List<int> _recipientEntityIds)
		{
			this.ClientInfo = _clientInfo;
			this.ChatType = _chatType;
			this.SenderEntityId = _senderEntityId;
			this.Message = _message;
			this.MainName = _mainName;
			this.RecipientEntityIds = _recipientEntityIds;
		}

		// Token: 0x040028E5 RID: 10469
		public readonly ClientInfo ClientInfo;

		// Token: 0x040028E6 RID: 10470
		public readonly EChatType ChatType;

		// Token: 0x040028E7 RID: 10471
		public readonly int SenderEntityId;

		// Token: 0x040028E8 RID: 10472
		public readonly string Message;

		// Token: 0x040028E9 RID: 10473
		public readonly string MainName;

		// Token: 0x040028EA RID: 10474
		public readonly List<int> RecipientEntityIds;
	}

	// Token: 0x02000695 RID: 1685
	public struct SCalcChunkColorsDoneData
	{
		// Token: 0x060031FB RID: 12795 RVA: 0x001545CF File Offset: 0x001527CF
		public SCalcChunkColorsDoneData(Chunk _chunk)
		{
			this.Chunk = _chunk;
		}

		// Token: 0x040028EB RID: 10475
		public readonly Chunk Chunk;
	}

	// Token: 0x02000696 RID: 1686
	public struct SEntityKilledData
	{
		// Token: 0x060031FC RID: 12796 RVA: 0x001545D8 File Offset: 0x001527D8
		public SEntityKilledData(Entity _killedEntitiy, Entity _killingEntity)
		{
			this.KilledEntitiy = _killedEntitiy;
			this.KillingEntity = _killingEntity;
		}

		// Token: 0x040028EC RID: 10476
		public readonly Entity KilledEntitiy;

		// Token: 0x040028ED RID: 10477
		public readonly Entity KillingEntity;
	}

	// Token: 0x02000697 RID: 1687
	public enum EModEventResult
	{
		// Token: 0x040028EF RID: 10479
		Continue,
		// Token: 0x040028F0 RID: 10480
		StopHandlersRunVanilla,
		// Token: 0x040028F1 RID: 10481
		StopHandlersAndVanilla
	}

	// Token: 0x02000698 RID: 1688
	// (Invoke) Token: 0x060031FE RID: 12798
	public delegate void ModEventHandlerDelegate<TData>(ref TData _data) where TData : struct;

	// Token: 0x02000699 RID: 1689
	// (Invoke) Token: 0x06003202 RID: 12802
	public delegate ModEvents.EModEventResult ModEventInterruptibleHandlerDelegate<TData>(ref TData _data) where TData : struct;

	// Token: 0x0200069A RID: 1690
	public abstract class ModEventAbs<TDelegate> where TDelegate : Delegate
	{
		// Token: 0x06003205 RID: 12805 RVA: 0x001545E8 File Offset: 0x001527E8
		[PublicizedFrom(EAccessModifier.Protected)]
		public ModEventAbs(string _eventName)
		{
			this.eventName = _eventName;
		}

		// Token: 0x06003206 RID: 12806 RVA: 0x00154604 File Offset: 0x00152804
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void RegisterHandler(TDelegate _handlerFunc)
		{
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			Assembly assembly = typeof(ModEvents).Assembly;
			bool coreGame = false;
			Mod mod = null;
			if (callingAssembly.Equals(assembly))
			{
				coreGame = true;
			}
			else
			{
				mod = ModManager.GetModForAssembly(callingAssembly);
				if (mod == null)
				{
					Log.Warning("[MODS] Could not find mod that tries to register a handler for event " + this.eventName);
				}
			}
			this.receivers.Add(new ModEvents.ModEventAbs<TDelegate>.Receiver(mod, _handlerFunc, coreGame));
		}

		// Token: 0x06003207 RID: 12807 RVA: 0x0015466C File Offset: 0x0015286C
		public void UnregisterHandler(TDelegate _handlerFunc)
		{
			for (int i = this.receivers.Count - 1; i >= 0; i--)
			{
				if (this.receivers[i].DelegateFunc.Equals(_handlerFunc))
				{
					this.receivers.RemoveAt(i);
				}
			}
		}

		// Token: 0x06003208 RID: 12808 RVA: 0x001546C0 File Offset: 0x001528C0
		[PublicizedFrom(EAccessModifier.Protected)]
		public void LogError(Exception _e, ModEvents.ModEventAbs<TDelegate>.Receiver _currentMod)
		{
			Log.Error(string.Concat(new string[]
			{
				"[MODS] Error while executing ModEvent \"",
				this.eventName,
				"\" on mod \"",
				_currentMod.ModName,
				"\""
			}));
			Log.Exception(_e);
		}

		// Token: 0x040028F2 RID: 10482
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly string eventName;

		// Token: 0x040028F3 RID: 10483
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly List<ModEvents.ModEventAbs<TDelegate>.Receiver> receivers = new List<ModEvents.ModEventAbs<TDelegate>.Receiver>();

		// Token: 0x0200069B RID: 1691
		[PublicizedFrom(EAccessModifier.Protected)]
		public class Receiver
		{
			// Token: 0x170004C7 RID: 1223
			// (get) Token: 0x06003209 RID: 12809 RVA: 0x0015470D File Offset: 0x0015290D
			public string ModName
			{
				get
				{
					if (this.Mod != null)
					{
						return this.Mod.Name;
					}
					if (this.coreGame)
					{
						return "-GameCore-";
					}
					return "-UnknownMod-";
				}
			}

			// Token: 0x0600320A RID: 12810 RVA: 0x00154736 File Offset: 0x00152936
			public Receiver(Mod _mod, TDelegate _handler, bool _coreGame = false)
			{
				this.Mod = _mod;
				this.DelegateFunc = _handler;
				this.coreGame = _coreGame;
			}

			// Token: 0x040028F4 RID: 10484
			public readonly Mod Mod;

			// Token: 0x040028F5 RID: 10485
			public readonly TDelegate DelegateFunc;

			// Token: 0x040028F6 RID: 10486
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly bool coreGame;
		}
	}

	// Token: 0x0200069C RID: 1692
	public class ModEvent<TData> : ModEvents.ModEventAbs<ModEvents.ModEventHandlerDelegate<TData>> where TData : struct
	{
		// Token: 0x0600320B RID: 12811 RVA: 0x00154753 File Offset: 0x00152953
		public ModEvent([CallerMemberName] string _eventName = null) : base(_eventName)
		{
		}

		// Token: 0x0600320C RID: 12812 RVA: 0x0015475C File Offset: 0x0015295C
		public void Invoke(ref TData _data)
		{
			foreach (ModEvents.ModEventAbs<ModEvents.ModEventHandlerDelegate<TData>>.Receiver receiver in this.receivers)
			{
				try
				{
					receiver.DelegateFunc(ref _data);
				}
				catch (Exception e)
				{
					base.LogError(e, receiver);
				}
			}
		}
	}

	// Token: 0x0200069D RID: 1693
	public class ModEventInterruptible<TData> : ModEvents.ModEventAbs<ModEvents.ModEventInterruptibleHandlerDelegate<TData>> where TData : struct
	{
		// Token: 0x0600320D RID: 12813 RVA: 0x001547D0 File Offset: 0x001529D0
		public ModEventInterruptible([CallerMemberName] string _eventName = null) : base(_eventName)
		{
		}

		// Token: 0x0600320E RID: 12814 RVA: 0x001547DC File Offset: 0x001529DC
		public ValueTuple<ModEvents.EModEventResult, Mod> Invoke(ref TData _data)
		{
			foreach (ModEvents.ModEventAbs<ModEvents.ModEventInterruptibleHandlerDelegate<TData>>.Receiver receiver in this.receivers)
			{
				try
				{
					ModEvents.EModEventResult emodEventResult = receiver.DelegateFunc(ref _data);
					if (emodEventResult != ModEvents.EModEventResult.Continue)
					{
						return new ValueTuple<ModEvents.EModEventResult, Mod>(emodEventResult, receiver.Mod);
					}
				}
				catch (Exception e)
				{
					base.LogError(e, receiver);
				}
			}
			return new ValueTuple<ModEvents.EModEventResult, Mod>(ModEvents.EModEventResult.Continue, null);
		}
	}
}
