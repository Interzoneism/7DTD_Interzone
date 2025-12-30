using System;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001602 RID: 5634
	[Preserve]
	public class ChallengeObjectiveWindowOpen : BaseChallengeObjective
	{
		// Token: 0x17001383 RID: 4995
		// (get) Token: 0x0600AD59 RID: 44377 RVA: 0x000DCA80 File Offset: 0x000DAC80
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.WindowOpen;
			}
		}

		// Token: 0x17001384 RID: 4996
		// (get) Token: 0x0600AD5A RID: 44378 RVA: 0x0043E1A4 File Offset: 0x0043C3A4
		public override string DescriptionText
		{
			get
			{
				return string.Format(Localization.Get("ObjectiveOpenWindow_keyword", false), string.Format("[DECEA3]{0}[-]", Localization.Get("xui" + this.WindowName, false)));
			}
		}

		// Token: 0x0600AD5B RID: 44379 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Init()
		{
		}

		// Token: 0x0600AD5C RID: 44380 RVA: 0x0043E1D6 File Offset: 0x0043C3D6
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.WindowChanged -= this.Current_WindowChanged;
			QuestEventManager.Current.WindowChanged += this.Current_WindowChanged;
		}

		// Token: 0x0600AD5D RID: 44381 RVA: 0x0043E204 File Offset: 0x0043C404
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_WindowChanged(string _windowName)
		{
			if (_windowName == "windowpaging")
			{
				return;
			}
			if (this.CheckBaseRequirements())
			{
				return;
			}
			this.currentOpenWindow = _windowName;
			this.HandleUpdatingCurrent();
			this.CheckObjectiveComplete(true);
			this.Parent.CheckPrerequisites();
		}

		// Token: 0x0600AD5E RID: 44382 RVA: 0x0043E23D File Offset: 0x0043C43D
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.WindowChanged -= this.Current_WindowChanged;
		}

		// Token: 0x0600AD5F RID: 44383 RVA: 0x0043E255 File Offset: 0x0043C455
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleUpdatingCurrent()
		{
			base.HandleUpdatingCurrent();
			if (this.Owner != null)
			{
				base.Current = ((this.currentOpenWindow == this.WindowName) ? 1 : 0);
			}
		}

		// Token: 0x0600AD60 RID: 44384 RVA: 0x0043E282 File Offset: 0x0043C482
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveWindowOpen
			{
				WindowName = this.WindowName
			};
		}

		// Token: 0x0600AD61 RID: 44385 RVA: 0x00439BCB File Offset: 0x00437DCB
		public override void CompleteObjective(bool handleComplete = true)
		{
			base.Current = this.MaxCount;
			base.Complete = true;
			if (handleComplete)
			{
				this.Owner.HandleComplete(true);
			}
		}

		// Token: 0x040086B3 RID: 34483
		public string WindowName = "";

		// Token: 0x040086B4 RID: 34484
		[PublicizedFrom(EAccessModifier.Private)]
		public string currentOpenWindow = "";

		// Token: 0x040086B5 RID: 34485
		public RequirementObjectiveGroupWindowOpen Parent;
	}
}
