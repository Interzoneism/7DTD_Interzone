using System;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic.Factories
{
	// Token: 0x0200178D RID: 6029
	public static class Factory
	{
		// Token: 0x0600B4A3 RID: 46243 RVA: 0x0045C20A File Offset: 0x0045A40A
		[Preserve]
		public static Conductor CreateConductor()
		{
			return new Conductor();
		}

		// Token: 0x0600B4A4 RID: 46244 RVA: 0x0045C211 File Offset: 0x0045A411
		[Preserve]
		public static ISectionSelector CreateSectionSelector()
		{
			return new SectionSelector();
		}

		// Token: 0x0600B4A5 RID: 46245 RVA: 0x0045C218 File Offset: 0x0045A418
		[Preserve]
		public static IThreatLevel CreateThreatLevel()
		{
			return default(ThreatLevel);
		}

		// Token: 0x0600B4A6 RID: 46246 RVA: 0x0045C233 File Offset: 0x0045A433
		[Preserve]
		public static IFilter<SectionType> CreatePlayerTracker()
		{
			return new PlayerTracker();
		}

		// Token: 0x0600B4A7 RID: 46247 RVA: 0x0045C23A File Offset: 0x0045A43A
		[Preserve]
		public static DayTimeTracker CreateDayTimeTracker()
		{
			return new DayTimeTracker();
		}

		// Token: 0x0600B4A8 RID: 46248 RVA: 0x0045C241 File Offset: 0x0045A441
		[Preserve]
		public static IMultiNotifiableFilter CreateMusicTimeTracker()
		{
			return new MusicTimeTracker();
		}

		// Token: 0x0600B4A9 RID: 46249 RVA: 0x0045C248 File Offset: 0x0045A448
		[Preserve]
		public static ISection CreateSection<T>(SectionType _sectionType) where T : ISection, new()
		{
			T t = Activator.CreateInstance<T>();
			t.Sect = _sectionType;
			t.Init();
			return t;
		}

		// Token: 0x0600B4AA RID: 46250 RVA: 0x0045C27C File Offset: 0x0045A47C
		[Preserve]
		public static IConfiguration CreateConfiguration()
		{
			return new Configuration();
		}
	}
}
