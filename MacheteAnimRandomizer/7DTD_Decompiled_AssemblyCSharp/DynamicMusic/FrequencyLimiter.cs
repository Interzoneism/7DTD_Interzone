using System;
using System.Collections.Generic;
using MusicUtils.Enums;
using UnityEngine;

namespace DynamicMusic
{
	// Token: 0x02001733 RID: 5939
	public class FrequencyLimiter : AbstractFilter, IMultiNotifiableFilter, INotifiable, INotifiableFilter<MusicActionType, SectionType>, INotifiable<MusicActionType>, IFilter<SectionType>, IGamePrefsChangedListener
	{
		// Token: 0x0600B2F0 RID: 45808 RVA: 0x004579F2 File Offset: 0x00455BF2
		public FrequencyLimiter()
		{
			this.rng = GameRandomManager.Instance.CreateGameRandom();
			GamePrefs.AddChangeListener(this);
			this.UpdateParameters();
			this.UpdateRollTime();
		}

		// Token: 0x0600B2F1 RID: 45809 RVA: 0x00457A1C File Offset: 0x00455C1C
		public override List<SectionType> Filter(List<SectionType> _sectionTypes)
		{
			if (AudioSettings.dspTime <= this.rollTime || this.rng.RandomRange(1f) > this.chanceOfPositiveRoll)
			{
				for (int i = _sectionTypes.Count - 1; i >= 0; i--)
				{
					if (_sectionTypes[i] != SectionType.None && _sectionTypes[i] != SectionType.Combat)
					{
						_sectionTypes.RemoveAt(i);
					}
				}
			}
			return _sectionTypes;
		}

		// Token: 0x0600B2F2 RID: 45810 RVA: 0x00457A7C File Offset: 0x00455C7C
		[PublicizedFrom(EAccessModifier.Private)]
		public void UpdateRollTime()
		{
			this.rollTime = AudioSettings.dspTime + (double)this.cooldown;
		}

		// Token: 0x0600B2F3 RID: 45811 RVA: 0x00457A91 File Offset: 0x00455C91
		public void Notify(MusicActionType _state)
		{
			if (_state.Equals(MusicActionType.Stop))
			{
				this.UpdateRollTime();
			}
		}

		// Token: 0x0600B2F4 RID: 45812 RVA: 0x00457AAE File Offset: 0x00455CAE
		public void Notify()
		{
			this.UpdateRollTime();
		}

		// Token: 0x0600B2F5 RID: 45813 RVA: 0x00457AB6 File Offset: 0x00455CB6
		public void OnGamePrefChanged(EnumGamePrefs _enum)
		{
			if (_enum.Equals(EnumGamePrefs.OptionsDynamicMusicDailyTime) || _enum.Equals(EnumGamePrefs.DayNightLength))
			{
				this.UpdateParameters();
			}
		}

		// Token: 0x0600B2F6 RID: 45814 RVA: 0x00457AF0 File Offset: 0x00455CF0
		[PublicizedFrom(EAccessModifier.Private)]
		public void UpdateParameters()
		{
			this.dayLengthInSeconds = (float)GamePrefs.GetInt(EnumGamePrefs.DayNightLength) * 60f;
			this.dailyAllottedPlaySeconds = GamePrefs.GetFloat(EnumGamePrefs.OptionsDynamicMusicDailyTime) * this.dayLengthInSeconds;
			this.rollsPerDay = Mathf.Ceil(this.dailyAllottedPlaySeconds / 168f);
			float num = this.dayLengthInSeconds / this.rollsPerDay;
			this.cooldown = Mathf.Max(num - 168f, 30f);
			this.chanceOfPositiveRoll = (float)Math.Pow(0.8999999761581421, 1.0 / (double)this.rollsPerDay);
		}

		// Token: 0x04008C1A RID: 35866
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cMinCooldown = 30f;

		// Token: 0x04008C1B RID: 35867
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cProbOfFailingToReachDailyAllotted = 0.1f;

		// Token: 0x04008C1C RID: 35868
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cPlayTime = 168f;

		// Token: 0x04008C1D RID: 35869
		[PublicizedFrom(EAccessModifier.Private)]
		public float dayLengthInSeconds;

		// Token: 0x04008C1E RID: 35870
		[PublicizedFrom(EAccessModifier.Private)]
		public float dailyAllottedPlaySeconds;

		// Token: 0x04008C1F RID: 35871
		[PublicizedFrom(EAccessModifier.Private)]
		public float rollsPerDay;

		// Token: 0x04008C20 RID: 35872
		[PublicizedFrom(EAccessModifier.Private)]
		public float cooldown;

		// Token: 0x04008C21 RID: 35873
		[PublicizedFrom(EAccessModifier.Private)]
		public float chanceOfPositiveRoll;

		// Token: 0x04008C22 RID: 35874
		[PublicizedFrom(EAccessModifier.Private)]
		public double rollTime;

		// Token: 0x04008C23 RID: 35875
		[PublicizedFrom(EAccessModifier.Private)]
		public GameRandom rng;
	}
}
