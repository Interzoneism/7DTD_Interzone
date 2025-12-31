using System;
using System.Collections.Generic;
using Twitch;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016DF RID: 5855
	[Preserve]
	public class ActionTwitchAddActionCooldown : ActionBaseClientAction
	{
		// Token: 0x0600B17C RID: 45436 RVA: 0x0045355C File Offset: 0x0045175C
		public override bool CanPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null && entityPlayer.TwitchEnabled && entityPlayer.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Disabled)
			{
				return this.searchType != ActionTwitchAddActionCooldown.SearchTypes.Name || !(this.twitchActions == "");
			}
			return base.CanPerform(target);
		}

		// Token: 0x0600B17D RID: 45437 RVA: 0x004535A8 File Offset: 0x004517A8
		public override void OnClientPerform(Entity target)
		{
			TwitchManager twitchManager = TwitchManager.Current;
			if (!twitchManager.TwitchActive)
			{
				return;
			}
			float currentUnityTime = twitchManager.CurrentUnityTime;
			switch (this.searchType)
			{
			case ActionTwitchAddActionCooldown.SearchTypes.Name:
			{
				string[] array = this.twitchActions.Split(',', StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					if (TwitchActionManager.TwitchActions.ContainsKey(array[i]))
					{
						TwitchAction twitchAction = TwitchActionManager.TwitchActions[array[i]];
						twitchAction.tempCooldown = this.cooldownTime;
						twitchAction.tempCooldownSet = currentUnityTime;
					}
				}
				return;
			}
			case ActionTwitchAddActionCooldown.SearchTypes.Positive:
				using (Dictionary<string, TwitchAction>.ValueCollection.Enumerator enumerator = twitchManager.AvailableCommands.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TwitchAction twitchAction2 = enumerator.Current;
						if (twitchAction2.IsPositive)
						{
							twitchAction2.tempCooldown = this.cooldownTime;
							twitchAction2.tempCooldownSet = currentUnityTime;
						}
					}
					return;
				}
				break;
			case ActionTwitchAddActionCooldown.SearchTypes.Negative:
				break;
			default:
				return;
			}
			foreach (TwitchAction twitchAction3 in twitchManager.AvailableCommands.Values)
			{
				if (!twitchAction3.IsPositive)
				{
					twitchAction3.tempCooldown = this.cooldownTime;
					twitchAction3.tempCooldownSet = currentUnityTime;
				}
			}
		}

		// Token: 0x0600B17E RID: 45438 RVA: 0x004536FC File Offset: 0x004518FC
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseFloat(ActionTwitchAddActionCooldown.PropTime, ref this.cooldownTime);
			properties.ParseString(ActionTwitchAddActionCooldown.PropTwitchActions, ref this.twitchActions);
			properties.ParseEnum<ActionTwitchAddActionCooldown.SearchTypes>(ActionTwitchAddActionCooldown.PropSearchType, ref this.searchType);
		}

		// Token: 0x0600B17F RID: 45439 RVA: 0x00453738 File Offset: 0x00451938
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTwitchAddActionCooldown
			{
				twitchActions = this.twitchActions,
				cooldownTime = this.cooldownTime,
				searchType = this.searchType
			};
		}

		// Token: 0x04008AEA RID: 35562
		[PublicizedFrom(EAccessModifier.Protected)]
		public float cooldownTime = 5f;

		// Token: 0x04008AEB RID: 35563
		[PublicizedFrom(EAccessModifier.Protected)]
		public string twitchActions = "";

		// Token: 0x04008AEC RID: 35564
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionTwitchAddActionCooldown.SearchTypes searchType;

		// Token: 0x04008AED RID: 35565
		public static string PropTime = "time";

		// Token: 0x04008AEE RID: 35566
		public static string PropTwitchActions = "action_name";

		// Token: 0x04008AEF RID: 35567
		public static string PropSearchType = "search_type";

		// Token: 0x020016E0 RID: 5856
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum SearchTypes
		{
			// Token: 0x04008AF1 RID: 35569
			Name,
			// Token: 0x04008AF2 RID: 35570
			Positive,
			// Token: 0x04008AF3 RID: 35571
			Negative
		}
	}
}
