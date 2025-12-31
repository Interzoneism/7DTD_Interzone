using System;
using System.Collections;
using System.Collections.Generic;
using DynamicMusic.Factories;
using MusicUtils.Enums;
using UnityEngine;

namespace DynamicMusic
{
	// Token: 0x02001773 RID: 6003
	public static class TrackerTests
	{
		// Token: 0x0600B3ED RID: 46061 RVA: 0x0045A0AC File Offset: 0x004582AC
		public static void Run(int num)
		{
			switch (num)
			{
			case 0:
				GameManager.Instance.StartCoroutine(TrackerTests.MusicTimeTrackerTest());
				return;
			case 1:
				GameManager.Instance.StartCoroutine(TrackerTests.DayTimeTrackerTest());
				return;
			case 2:
				GameManager.Instance.StartCoroutine(TrackerTests.PlayerTrackerTest());
				return;
			case 3:
				GameManager.Instance.StartCoroutine(TrackerTests.SelectorTest());
				return;
			case 4:
				GameManager.Instance.StartCoroutine(TrackerTests.ConductorTest());
				return;
			case 5:
				GameManager.Instance.StartCoroutine(TrackerTests.RealTimeConductorTest());
				return;
			default:
				return;
			}
		}

		// Token: 0x0600B3EE RID: 46062 RVA: 0x0045A13D File Offset: 0x0045833D
		[PublicizedFrom(EAccessModifier.Private)]
		public static IEnumerator MusicTimeTrackerTest()
		{
			TrackerTests.isFinished = false;
			IMultiNotifiableFilter musicTimeTracker = Factory.CreateMusicTimeTracker();
			int num;
			for (int i = 0; i < 2; i = num + 1)
			{
				musicTimeTracker.Notify(MusicActionType.Play);
				yield return new WaitForSeconds(30f);
				musicTimeTracker.Notify(MusicActionType.Pause);
				yield return new WaitForSeconds(30f);
				musicTimeTracker.Notify(MusicActionType.UnPause);
				yield return new WaitForSeconds(30f);
				musicTimeTracker.Notify(MusicActionType.Stop);
				yield return new WaitForSeconds(30f);
				num = i;
			}
			musicTimeTracker.Notify();
			yield break;
		}

		// Token: 0x0600B3EF RID: 46063 RVA: 0x0045A145 File Offset: 0x00458345
		[PublicizedFrom(EAccessModifier.Private)]
		public static IEnumerator DayTimeTrackerTest()
		{
			TrackerTests.isFinished = false;
			List<SectionType> sectionTypes = new List<SectionType>
			{
				SectionType.Exploration
			};
			DayTimeTracker dtt = Factory.CreateDayTimeTracker();
			do
			{
				yield return new WaitUntil(() => TrackerTests.continueTest);
				TrackerTests.continueTest = false;
				dtt.Filter(sectionTypes);
			}
			while (!TrackerTests.isFinished);
			yield break;
		}

		// Token: 0x0600B3F0 RID: 46064 RVA: 0x0045A14D File Offset: 0x0045834D
		[PublicizedFrom(EAccessModifier.Private)]
		public static IEnumerator PlayerTrackerTest()
		{
			TrackerTests.isFinished = false;
			IFilter<SectionType> playerTracker = Factory.CreatePlayerTracker();
			while (!TrackerTests.isFinished)
			{
				List<SectionType> list = new List<SectionType>(TrackerTests.sections);
				playerTracker.Filter(list);
				yield return new WaitUntil(() => TrackerTests.continueTest);
				TrackerTests.continueTest = false;
			}
			yield break;
		}

		// Token: 0x0600B3F1 RID: 46065 RVA: 0x0045A155 File Offset: 0x00458355
		[PublicizedFrom(EAccessModifier.Private)]
		public static IEnumerator SelectorTest()
		{
			TrackerTests.isFinished = false;
			ISectionSelector sectionSelector = Factory.CreateSectionSelector();
			while (!TrackerTests.isFinished)
			{
				SectionType sectionType = sectionSelector.Select();
				Log.Out(string.Format("Selected Section: {0}", sectionType));
				yield return new WaitUntil(() => TrackerTests.continueTest);
				TrackerTests.continueTest = false;
			}
			yield break;
		}

		// Token: 0x0600B3F2 RID: 46066 RVA: 0x0045A15D File Offset: 0x0045835D
		[PublicizedFrom(EAccessModifier.Private)]
		public static IEnumerator ConductorTest()
		{
			TrackerTests.isFinished = false;
			Conductor c = Factory.CreateConductor();
			while (!TrackerTests.isFinished)
			{
				yield return new WaitUntil(() => TrackerTests.continueTest);
				c.Update();
				TrackerTests.continueTest = false;
			}
			yield break;
		}

		// Token: 0x0600B3F3 RID: 46067 RVA: 0x0045A165 File Offset: 0x00458365
		[PublicizedFrom(EAccessModifier.Private)]
		public static IEnumerator RealTimeConductorTest()
		{
			TrackerTests.isFinished = false;
			Conductor c = Factory.CreateConductor();
			while (!TrackerTests.isFinished)
			{
				c.Update();
				yield return null;
			}
			yield break;
		}

		// Token: 0x04008C88 RID: 35976
		public static bool continueTest = false;

		// Token: 0x04008C89 RID: 35977
		public static bool isFinished = false;

		// Token: 0x04008C8A RID: 35978
		[PublicizedFrom(EAccessModifier.Private)]
		public static List<SectionType> sections = new List<SectionType>
		{
			SectionType.Exploration,
			SectionType.Suspense,
			SectionType.Combat
		};
	}
}
