using System;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015F6 RID: 5622
	[Preserve]
	public class ChallengeObjectiveLootContainer : BaseChallengeObjective
	{
		// Token: 0x17001367 RID: 4967
		// (get) Token: 0x0600ACED RID: 44269 RVA: 0x0043CD9B File Offset: 0x0043AF9B
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.LootContainer;
			}
		}

		// Token: 0x17001368 RID: 4968
		// (get) Token: 0x0600ACEE RID: 44270 RVA: 0x0043CD9F File Offset: 0x0043AF9F
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("ObjectiveLootContainer_keyword", false);
			}
		}

		// Token: 0x0600ACEF RID: 44271 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Init()
		{
		}

		// Token: 0x0600ACF0 RID: 44272 RVA: 0x0043CDAC File Offset: 0x0043AFAC
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.ContainerOpened -= this.Current_ContainerOpened;
			QuestEventManager.Current.ContainerOpened += this.Current_ContainerOpened;
		}

		// Token: 0x0600ACF1 RID: 44273 RVA: 0x0043CDDC File Offset: 0x0043AFDC
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_ContainerOpened(int entityId, Vector3i containerLocation, ITileEntityLootable tileEntity)
		{
			if (containerLocation == this.lastLocation)
			{
				return;
			}
			this.lastLocation = containerLocation;
			if (tileEntity.bWasTouched)
			{
				return;
			}
			if (this.CheckBaseRequirements())
			{
				return;
			}
			int num = base.Current;
			base.Current = num + 1;
			this.CheckObjectiveComplete(true);
		}

		// Token: 0x0600ACF2 RID: 44274 RVA: 0x0043CE29 File Offset: 0x0043B029
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.ContainerOpened -= this.Current_ContainerOpened;
		}

		// Token: 0x0600ACF3 RID: 44275 RVA: 0x0043CE41 File Offset: 0x0043B041
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveLootContainer();
		}

		// Token: 0x0400868C RID: 34444
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3i lastLocation = Vector3i.zero;
	}
}
