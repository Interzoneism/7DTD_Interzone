using System;
using DynamicMusic.Legacy;
using DynamicMusic.Legacy.ObjectModel;

// Token: 0x02000378 RID: 888
public class DynamicMusicManager : IGamePrefsChangedListener
{
	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06001A38 RID: 6712 RVA: 0x000A38F5 File Offset: 0x000A1AF5
	public bool MusicStarted
	{
		get
		{
			return this.IsMusicPlayingThisTick && !this.WasMusicPlayingLastTick;
		}
	}

	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06001A39 RID: 6713 RVA: 0x000A390A File Offset: 0x000A1B0A
	public bool MusicStopped
	{
		get
		{
			return !this.IsMusicPlayingThisTick && this.WasMusicPlayingLastTick;
		}
	}

	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x06001A3A RID: 6714 RVA: 0x000A391C File Offset: 0x000A1B1C
	public bool IsDynamicMusicPlaying
	{
		get
		{
			return this.IsMusicPlayingThisTick;
		}
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x06001A3B RID: 6715 RVA: 0x000A3924 File Offset: 0x000A1B24
	public bool IsBeforeDuskPlayBan
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return SkyManager.GetTimeOfDayAsMinutes() < SkyManager.GetDuskTimeAsMinutes() - DynamicMusicManager.PlayBanThreshold;
		}
	}

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x06001A3C RID: 6716 RVA: 0x000A3938 File Offset: 0x000A1B38
	public bool IsAfterDusk
	{
		get
		{
			return SkyManager.TimeOfDay() > SkyManager.GetDuskTime();
		}
	}

	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x06001A3D RID: 6717 RVA: 0x000A3946 File Offset: 0x000A1B46
	public bool IsAfterDuskWindow
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return SkyManager.GetTimeOfDayAsMinutes() > SkyManager.GetDuskTimeAsMinutes() + DynamicMusicManager.deadWindow;
		}
	}

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x06001A3E RID: 6718 RVA: 0x000A395A File Offset: 0x000A1B5A
	public bool IsBeforeDawnPlayBan
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return SkyManager.GetTimeOfDayAsMinutes() < SkyManager.GetDawnTimeAsMinutes() - DynamicMusicManager.PlayBanThreshold;
		}
	}

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06001A3F RID: 6719 RVA: 0x000A396E File Offset: 0x000A1B6E
	public bool IsAfterDawn
	{
		get
		{
			return SkyManager.TimeOfDay() > SkyManager.GetDawnTime();
		}
	}

	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06001A40 RID: 6720 RVA: 0x000A397C File Offset: 0x000A1B7C
	public bool IsAfterDawnWindow
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return SkyManager.GetTimeOfDayAsMinutes() > SkyManager.GetDawnTimeAsMinutes() + DynamicMusicManager.deadWindow;
		}
	}

	// Token: 0x170002FC RID: 764
	// (get) Token: 0x06001A41 RID: 6721 RVA: 0x000A3990 File Offset: 0x000A1B90
	// (set) Token: 0x06001A42 RID: 6722 RVA: 0x000A3998 File Offset: 0x000A1B98
	public bool IsInDeadWindow { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170002FD RID: 765
	// (get) Token: 0x06001A43 RID: 6723 RVA: 0x000A39A1 File Offset: 0x000A1BA1
	// (set) Token: 0x06001A44 RID: 6724 RVA: 0x000A39A9 File Offset: 0x000A1BA9
	public bool IsPlayAllowed { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06001A45 RID: 6725 RVA: 0x000A39B2 File Offset: 0x000A1BB2
	// (set) Token: 0x06001A46 RID: 6726 RVA: 0x000A39BA File Offset: 0x000A1BBA
	public float DistanceFromDeadWindow { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170002FF RID: 767
	// (get) Token: 0x06001A47 RID: 6727 RVA: 0x000A39C3 File Offset: 0x000A1BC3
	// (set) Token: 0x06001A48 RID: 6728 RVA: 0x000A39CB File Offset: 0x000A1BCB
	public bool IsPlayerInTraderStation { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06001A49 RID: 6729 RVA: 0x000A39D4 File Offset: 0x000A1BD4
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicMusicManager()
	{
		this.UpdateConditions = default(DMSUpdateConditions);
		this.UpdateConditions.IsDMSEnabled = GamePrefs.GetBool(EnumGamePrefs.OptionsDynamicMusicEnabled);
		this.UpdateConditions.IsGameUnPaused = true;
	}

	// Token: 0x06001A4A RID: 6730 RVA: 0x000A3A0C File Offset: 0x000A1C0C
	public static void Init(EntityPlayerLocal _epLocal)
	{
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Initializing Dynamic Music System");
		_epLocal.DynamicMusicManager = new DynamicMusicManager();
		GamePrefs.AddChangeListener(_epLocal.DynamicMusicManager);
		_epLocal.DynamicMusicManager.PrimaryLocalPlayer = _epLocal;
		DynamicMusicManager.Random = GameRandomManager.Instance.CreateGameRandom();
		ThreatLevelTracker.Init(_epLocal.DynamicMusicManager);
		FrequencyManager.Init(_epLocal.DynamicMusicManager);
		StreamerMaster.Init(_epLocal.DynamicMusicManager);
		TransitionManager.Init(_epLocal.DynamicMusicManager);
		_epLocal.DynamicMusicManager.UpdateConditions.IsDMSInitialized = true;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Finished initializing Dynamic Music System");
	}

	// Token: 0x06001A4B RID: 6731 RVA: 0x000A3AA8 File Offset: 0x000A1CA8
	public void Tick()
	{
		if (this.UpdateConditions.CanUpdate)
		{
			if (StreamerMaster.currentStreamer != null)
			{
				this.IsMusicPlayingThisTick = StreamerMaster.currentStreamer.IsPlaying;
			}
			if (this.IsAfterDusk)
			{
				this.IsInDeadWindow = !(this.IsPlayAllowed = this.IsAfterDuskWindow);
				this.DistanceFromDeadWindow = (float)GamePrefs.GetInt(EnumGamePrefs.DayNightLength) - SkyManager.GetTimeOfDayAsMinutes() + SkyManager.GetDawnTimeAsMinutes();
			}
			else if (this.IsAfterDawn)
			{
				if (this.IsAfterDawnWindow)
				{
					this.DistanceFromDeadWindow = Utils.FastMax(SkyManager.GetDuskTimeAsMinutes() - DynamicMusicManager.deadWindow - SkyManager.GetTimeOfDayAsMinutes(), 0f);
					if (this.IsBeforeDuskPlayBan)
					{
						this.IsInDeadWindow = false;
						this.IsPlayAllowed = true;
					}
					else
					{
						this.IsPlayAllowed = false;
						this.IsInDeadWindow = (this.DistanceFromDeadWindow == 0f);
					}
				}
			}
			else
			{
				this.DistanceFromDeadWindow = Utils.FastMax(SkyManager.GetDawnTimeAsMinutes() - DynamicMusicManager.deadWindow - SkyManager.GetTimeOfDayAsMinutes(), 0f);
				if (this.IsBeforeDawnPlayBan)
				{
					this.IsPlayAllowed = true;
					this.IsInDeadWindow = false;
				}
				else
				{
					this.IsPlayAllowed = false;
					this.IsInDeadWindow = (this.DistanceFromDeadWindow == 0f);
				}
			}
			this.IsPlayerInTraderStation = this.IsPrimaryPlayerInTraderStation();
			this.ThreatLevelTracker.Tick();
			this.FrequencyManager.Tick();
			this.TransitionManager.Tick();
			this.StreamerMaster.Tick();
			this.WasMusicPlayingLastTick = this.IsMusicPlayingThisTick;
		}
	}

	// Token: 0x06001A4C RID: 6732 RVA: 0x000A3C1A File Offset: 0x000A1E1A
	public void CleanUpDynamicMembers()
	{
		if (this.StreamerMaster != null)
		{
			this.StreamerMaster.Cleanup();
		}
		this.UpdateConditions.IsDMSInitialized = false;
	}

	// Token: 0x06001A4D RID: 6733 RVA: 0x000A3C3B File Offset: 0x000A1E3B
	public static void Cleanup()
	{
		MusicGroup.Cleanup();
		ConfigSet.Cleanup();
	}

	// Token: 0x06001A4E RID: 6734 RVA: 0x000A3C48 File Offset: 0x000A1E48
	public void Event(MinEventTypes _eventType, MinEventParams _eventParms)
	{
		if (_eventType <= MinEventTypes.onSelfDied)
		{
			switch (_eventType)
			{
			case MinEventTypes.onOtherDamagedSelf:
				this.ThreatLevelTracker.Event(_eventType, _eventParms);
				return;
			case MinEventTypes.onOtherAttackedSelf:
				this.ThreatLevelTracker.Event(_eventType, _eventParms);
				return;
			case MinEventTypes.onOtherHealedSelf:
				break;
			case MinEventTypes.onSelfDamagedOther:
				this.ThreatLevelTracker.Event(_eventType, _eventParms);
				return;
			case MinEventTypes.onSelfAttackedOther:
				this.ThreatLevelTracker.Event(_eventType, _eventParms);
				return;
			default:
				if (_eventType != MinEventTypes.onSelfDied)
				{
					return;
				}
				Log.Out("DMS Died!");
				this.UpdateConditions.DoesPlayerExist = false;
				return;
			}
		}
		else
		{
			switch (_eventType)
			{
			case MinEventTypes.onSelfRespawn:
				Log.Out("DMS Respawn!");
				this.UpdateConditions.DoesPlayerExist = true;
				return;
			case MinEventTypes.onSelfLeaveGame:
				Log.Out("DMS Left Game!");
				return;
			case MinEventTypes.onSelfEnteredGame:
				Log.Out("DMS Entered Game!");
				this.UpdateConditions.DoesPlayerExist = true;
				break;
			default:
				if (_eventType != MinEventTypes.onSelfEnteredBiome)
				{
					return;
				}
				break;
			}
		}
	}

	// Token: 0x06001A4F RID: 6735 RVA: 0x000A3D26 File Offset: 0x000A1F26
	public void OnPlayerDeath()
	{
		this.StreamerMaster.Stop();
	}

	// Token: 0x06001A50 RID: 6736 RVA: 0x000A3D33 File Offset: 0x000A1F33
	public void OnPlayerFirstSpawned()
	{
		this.FrequencyManager.OnPlayerFirstSpawned();
	}

	// Token: 0x06001A51 RID: 6737 RVA: 0x000A3D40 File Offset: 0x000A1F40
	public void Pause()
	{
		this.UpdateConditions.IsGameUnPaused = false;
		this.StreamerMaster.Pause();
		this.FrequencyManager.OnPause();
	}

	// Token: 0x06001A52 RID: 6738 RVA: 0x000A3D64 File Offset: 0x000A1F64
	public void UnPause()
	{
		this.UpdateConditions.IsGameUnPaused = true;
		this.StreamerMaster.UnPause();
		this.FrequencyManager.OnUnPause();
	}

	// Token: 0x06001A53 RID: 6739 RVA: 0x000A3D88 File Offset: 0x000A1F88
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsPrimaryPlayerInTraderStation()
	{
		return GameManager.Instance.World.IsWithinTraderArea(this.PrimaryLocalPlayer.GetBlockPosition());
	}

	// Token: 0x06001A54 RID: 6740 RVA: 0x000A3DA4 File Offset: 0x000A1FA4
	public bool IsInDawnOrDuskRange(float _dawnOrDuskTime, float _currentTime)
	{
		return this.DistanceFromDawnOrDusk(_dawnOrDuskTime, _currentTime) <= DynamicMusicManager.deadWindow;
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x000A3DB8 File Offset: 0x000A1FB8
	public float DistanceFromDawnOrDusk(float _dawnOrDuskTime, float _currentTime)
	{
		return Math.Abs(_dawnOrDuskTime - _currentTime);
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x000A3DC4 File Offset: 0x000A1FC4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGamePrefChanged(EnumGamePrefs _enum)
	{
		if (_enum == EnumGamePrefs.OptionsDynamicMusicEnabled && !(this.UpdateConditions.IsDMSEnabled = GamePrefs.GetBool(EnumGamePrefs.OptionsDynamicMusicEnabled)))
		{
			this.StreamerMaster.Stop();
		}
	}

	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06001A57 RID: 6743 RVA: 0x000A3DFE File Offset: 0x000A1FFE
	public float TimeToNextDayEvent
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return (SkyManager.IsDark() ? SkyManager.GetDawnTimeAsMinutes() : SkyManager.GetDuskTimeAsMinutes()) - SkyManager.GetTimeOfDayAsMinutes();
		}
	}

	// Token: 0x04001106 RID: 4358
	public EntityPlayerLocal PrimaryLocalPlayer;

	// Token: 0x04001107 RID: 4359
	public ThreatLevelTracker ThreatLevelTracker;

	// Token: 0x04001108 RID: 4360
	public FrequencyManager FrequencyManager;

	// Token: 0x04001109 RID: 4361
	public TransitionManager TransitionManager;

	// Token: 0x0400110A RID: 4362
	public StreamerMaster StreamerMaster;

	// Token: 0x0400110B RID: 4363
	public bool IsMusicPlayingThisTick;

	// Token: 0x0400110C RID: 4364
	public bool WasMusicPlayingLastTick;

	// Token: 0x0400110D RID: 4365
	public static readonly float PlayBanThreshold = 1f;

	// Token: 0x0400110E RID: 4366
	public static readonly float deadWindow = 0.16666667f;

	// Token: 0x0400110F RID: 4367
	public static GameRandom Random;

	// Token: 0x04001110 RID: 4368
	public DMSUpdateConditions UpdateConditions;
}
