using System;
using Challenges;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200165E RID: 5726
	[Preserve]
	public class ActionCompleteChallenge : ActionBaseClientAction
	{
		// Token: 0x0600AF2D RID: 44845 RVA: 0x004465E0 File Offset: 0x004447E0
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				string[] array = this.ChallengeID.ToLower().Split(',', StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					Challenge challenge = entityPlayerLocal.challengeJournal.ChallengeDictionary[array[i]];
					if (challenge != null)
					{
						challenge.CompleteChallenge(this.ForceRedeem);
					}
				}
				entityPlayerLocal.PlayerUI.xui.QuestTracker.TrackedChallenge = null;
			}
		}

		// Token: 0x0600AF2E RID: 44846 RVA: 0x00446652 File Offset: 0x00444852
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionCompleteChallenge.PropChallengeID, ref this.ChallengeID);
			properties.ParseBool(ActionCompleteChallenge.PropForceRedeem, ref this.ForceRedeem);
		}

		// Token: 0x0600AF2F RID: 44847 RVA: 0x0044667D File Offset: 0x0044487D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionCompleteChallenge
			{
				ChallengeID = this.ChallengeID,
				ForceRedeem = this.ForceRedeem
			};
		}

		// Token: 0x04008848 RID: 34888
		public string ChallengeID = "";

		// Token: 0x04008849 RID: 34889
		public bool ForceRedeem;

		// Token: 0x0400884A RID: 34890
		public static string PropChallengeID = "challenges";

		// Token: 0x0400884B RID: 34891
		public static string PropForceRedeem = "force_redeem";
	}
}
