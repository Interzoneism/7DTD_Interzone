using System;
using UnityEngine;

namespace DynamicMusic.Legacy
{
	// Token: 0x0200177B RID: 6011
	public class FrequencyManager
	{
		// Token: 0x1700142A RID: 5162
		// (get) Token: 0x0600B41F RID: 46111 RVA: 0x0045A62C File Offset: 0x0045882C
		public float DailyTimePercentage
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return GamePrefs.GetFloat(EnumGamePrefs.OptionsDynamicMusicDailyTime);
			}
		}

		// Token: 0x1700142B RID: 5163
		// (get) Token: 0x0600B420 RID: 46112 RVA: 0x0045A638 File Offset: 0x00458838
		public int MinutesPerDay
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return GamePrefs.GetInt(EnumGamePrefs.DayNightLength);
			}
		}

		// Token: 0x1700142C RID: 5164
		// (get) Token: 0x0600B421 RID: 46113 RVA: 0x0045A641 File Offset: 0x00458841
		public bool IsMusicPlayingThisTick
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.dynamicMusicManager.IsMusicPlayingThisTick;
			}
		}

		// Token: 0x1700142D RID: 5165
		// (get) Token: 0x0600B422 RID: 46114 RVA: 0x0045A64E File Offset: 0x0045884E
		public bool MusicStarted
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.dynamicMusicManager.MusicStarted;
			}
		}

		// Token: 0x1700142E RID: 5166
		// (get) Token: 0x0600B423 RID: 46115 RVA: 0x0045A65B File Offset: 0x0045885B
		public bool MusicStopped
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.dynamicMusicManager.MusicStopped;
			}
		}

		// Token: 0x1700142F RID: 5167
		// (get) Token: 0x0600B424 RID: 46116 RVA: 0x0045A668 File Offset: 0x00458868
		public bool IsMusicScheduled
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.CanScheduleTrack && !this.IsInCoolDown;
			}
		}

		// Token: 0x17001430 RID: 5168
		// (get) Token: 0x0600B425 RID: 46117 RVA: 0x0045A67D File Offset: 0x0045887D
		public bool DidDayChange
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.currentDay != GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime);
			}
		}

		// Token: 0x17001431 RID: 5169
		// (get) Token: 0x0600B426 RID: 46118 RVA: 0x0045A69E File Offset: 0x0045889E
		public float DailyTimeAllotted
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.DailyTimePercentage * (float)this.MinutesPerDay;
			}
		}

		// Token: 0x17001432 RID: 5170
		// (get) Token: 0x0600B427 RID: 46119 RVA: 0x0045A6AE File Offset: 0x004588AE
		public bool HasExceededDailyAllotted
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.DailyPlayTimeUsed >= this.DailyTimeAllotted;
			}
		}

		// Token: 0x17001433 RID: 5171
		// (get) Token: 0x0600B428 RID: 46120 RVA: 0x0045A6C1 File Offset: 0x004588C1
		public float RealTimeInMinutes
		{
			get
			{
				return (float)(AudioSettings.dspTime / 60.0);
			}
		}

		// Token: 0x17001434 RID: 5172
		// (get) Token: 0x0600B429 RID: 46121 RVA: 0x0045A6D3 File Offset: 0x004588D3
		public bool IsInCoolDown
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.RealTimeInMinutes < this.NextScheduleChance;
			}
		}

		// Token: 0x17001435 RID: 5173
		// (get) Token: 0x0600B42A RID: 46122 RVA: 0x0045A6E3 File Offset: 0x004588E3
		public float CoolDownTime
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return DynamicMusicManager.Random.RandomRange(GamePrefs.GetFloat(EnumGamePrefs.OptionsPlayChanceFrequency) - 1f, GamePrefs.GetFloat(EnumGamePrefs.OptionsPlayChanceFrequency) + 1f);
			}
		}

		// Token: 0x17001436 RID: 5174
		// (get) Token: 0x0600B42B RID: 46123 RVA: 0x0045A70F File Offset: 0x0045890F
		// (set) Token: 0x0600B42C RID: 46124 RVA: 0x0045A717 File Offset: 0x00458917
		public bool CanScheduleTrack { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600B42D RID: 46125 RVA: 0x0045A720 File Offset: 0x00458920
		public static void Init(DynamicMusicManager _dynamicMusicManager)
		{
			_dynamicMusicManager.FrequencyManager = new FrequencyManager();
			_dynamicMusicManager.FrequencyManager.dynamicMusicManager = _dynamicMusicManager;
			_dynamicMusicManager.FrequencyManager.CanScheduleTrack = false;
			FrequencyManager.PlayChance = GamePrefs.GetFloat(EnumGamePrefs.OptionsPlayChanceProbability);
		}

		// Token: 0x0600B42E RID: 46126 RVA: 0x0045A754 File Offset: 0x00458954
		public void Tick()
		{
			this.CanScheduleTrack = (!this.IsMusicPlayingThisTick && !this.IsInCoolDown && this.RollIsSuccessful);
			if (this.IsMusicPlayingThisTick)
			{
				this.RollIsSuccessful = false;
			}
			if (this.DidDayChange)
			{
				this.currentDay = GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime);
				this.DailyPlayTimeUsed = 0f;
				if (this.IsMusicPlayingThisTick)
				{
					this.musicStartTime = this.RealTimeInMinutes;
				}
				else if (!this.CanScheduleTrack)
				{
					this.StartCoolDown();
				}
			}
			if (this.dynamicMusicManager.MusicStarted)
			{
				this.OnMusicStarted();
				return;
			}
			if (this.dynamicMusicManager.MusicStopped)
			{
				this.OnMusicStopped();
				return;
			}
			if (this.HasExceededDailyAllotted || this.IsInCoolDown || this.IsMusicPlayingThisTick || this.dynamicMusicManager.IsAfterDusk || !this.dynamicMusicManager.IsAfterDawn)
			{
				return;
			}
			if (!(this.RollIsSuccessful = (DynamicMusicManager.Random.RandomRange(1f) < FrequencyManager.PlayChance)))
			{
				this.StartCoolDown();
			}
		}

		// Token: 0x0600B42F RID: 46127 RVA: 0x0045A862 File Offset: 0x00458A62
		public void OnPlayerFirstSpawned()
		{
			this.StartCoolDown();
		}

		// Token: 0x0600B430 RID: 46128 RVA: 0x0045A86A File Offset: 0x00458A6A
		public void StartCoolDown()
		{
			this.CanScheduleTrack = false;
			this.NextScheduleChance = this.RealTimeInMinutes + this.CoolDownTime;
		}

		// Token: 0x0600B431 RID: 46129 RVA: 0x0045A886 File Offset: 0x00458A86
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnMusicStarted()
		{
			this.musicStartTime = this.RealTimeInMinutes;
		}

		// Token: 0x0600B432 RID: 46130 RVA: 0x0045A894 File Offset: 0x00458A94
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnMusicStopped()
		{
			this.DailyPlayTimeUsed += this.RealTimeInMinutes - this.musicStartTime - this.PauseTime;
			this.StartCoolDown();
			this.PauseTime = 0f;
		}

		// Token: 0x0600B433 RID: 46131 RVA: 0x0045A8C8 File Offset: 0x00458AC8
		public void OnPause()
		{
			this.pauseStart = this.RealTimeInMinutes;
		}

		// Token: 0x0600B434 RID: 46132 RVA: 0x0045A8D8 File Offset: 0x00458AD8
		public void OnUnPause()
		{
			this.pauseEnd = this.RealTimeInMinutes;
			this.PauseTime += this.pauseEnd - this.pauseStart;
			this.pauseEnd = (this.pauseStart = 0f);
		}

		// Token: 0x04008CA4 RID: 36004
		[PublicizedFrom(EAccessModifier.Private)]
		public DynamicMusicManager dynamicMusicManager;

		// Token: 0x04008CA5 RID: 36005
		[PublicizedFrom(EAccessModifier.Private)]
		public static float PlayChance;

		// Token: 0x04008CA6 RID: 36006
		[PublicizedFrom(EAccessModifier.Private)]
		public int currentDay;

		// Token: 0x04008CA7 RID: 36007
		[PublicizedFrom(EAccessModifier.Private)]
		public float musicStartTime;

		// Token: 0x04008CA8 RID: 36008
		public float DailyPlayTimeUsed;

		// Token: 0x04008CA9 RID: 36009
		public float NextScheduleChance;

		// Token: 0x04008CAA RID: 36010
		public float PauseTime;

		// Token: 0x04008CAB RID: 36011
		[PublicizedFrom(EAccessModifier.Private)]
		public float pauseStart;

		// Token: 0x04008CAC RID: 36012
		[PublicizedFrom(EAccessModifier.Private)]
		public float pauseEnd;

		// Token: 0x04008CAD RID: 36013
		[PublicizedFrom(EAccessModifier.Private)]
		public bool RollIsSuccessful;
	}
}
