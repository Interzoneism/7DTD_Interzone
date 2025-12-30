using System;
using Twitch;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016E2 RID: 5858
	[Preserve]
	public class ActionTwitchAddPoints : ActionBaseClientAction
	{
		// Token: 0x0600B185 RID: 45445 RVA: 0x00453910 File Offset: 0x00451B10
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null && base.Owner.Requester == entityPlayerLocal)
			{
				this.amount = GameEventManager.GetIntValue(entityPlayerLocal, this.amountText, 0);
				if (this.recipientType == ActionTwitchAddPoints.RecipientTypes.All)
				{
					switch (this.pointType)
					{
					case TwitchAction.PointTypes.PP:
						TwitchManager.Current.ViewerData.AddPointsAll(this.amount, 0, true);
						break;
					case TwitchAction.PointTypes.SP:
						TwitchManager.Current.ViewerData.AddPointsAll(0, this.amount, true);
						break;
					case TwitchAction.PointTypes.Bits:
						Debug.LogWarning("TwitchAddPoints: Cannot add Bit Credit to all.");
						break;
					}
					TwitchManager.Current.SendChannelMessage(Localization.Get(this.awardText, false), true);
					return;
				}
				this.viewer = ((this.recipientType == ActionTwitchAddPoints.RecipientTypes.Requester) ? base.Owner.ExtraData : TwitchManager.Current.ViewerData.GetRandomActiveViewer());
				if (this.viewer == "")
				{
					return;
				}
				switch (this.pointType)
				{
				case TwitchAction.PointTypes.PP:
					TwitchManager.Current.ViewerData.AddPoints(this.viewer, this.amount, false, false);
					break;
				case TwitchAction.PointTypes.SP:
					TwitchManager.Current.ViewerData.AddPoints(this.viewer, this.amount, true, false);
					break;
				case TwitchAction.PointTypes.Bits:
					TwitchManager.Current.ViewerData.AddCredit(this.viewer, this.amount, false);
					break;
				}
				if (this.awardText != "")
				{
					TwitchManager.Current.SendChannelMessage(base.GetTextWithElements(Localization.Get(this.awardText, false)), true);
				}
			}
		}

		// Token: 0x0600B186 RID: 45446 RVA: 0x00453AAD File Offset: 0x00451CAD
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string ParseTextElement(string element)
		{
			if (element == "amount")
			{
				return this.amount.ToString();
			}
			if (!(element == "viewer"))
			{
				return base.ParseTextElement(element);
			}
			return this.viewer;
		}

		// Token: 0x0600B187 RID: 45447 RVA: 0x00453AE8 File Offset: 0x00451CE8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionTwitchAddPoints.PropAmount, ref this.amountText);
			properties.ParseEnum<TwitchAction.PointTypes>(ActionTwitchAddPoints.PropPointType, ref this.pointType);
			properties.ParseEnum<ActionTwitchAddPoints.RecipientTypes>(ActionTwitchAddPoints.PropRecipientType, ref this.recipientType);
			properties.ParseBool(ActionTwitchAddPoints.PropRequesterOnly, ref this.requesterOnly);
			properties.ParseString(ActionTwitchAddPoints.PropAwardText, ref this.awardText);
		}

		// Token: 0x0600B188 RID: 45448 RVA: 0x00453B54 File Offset: 0x00451D54
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionTwitchAddPoints
			{
				amountText = this.amountText,
				pointType = this.pointType,
				recipientType = this.recipientType,
				requesterOnly = this.requesterOnly,
				awardText = this.awardText
			};
		}

		// Token: 0x04008AF4 RID: 35572
		[PublicizedFrom(EAccessModifier.Protected)]
		public string amountText;

		// Token: 0x04008AF5 RID: 35573
		[PublicizedFrom(EAccessModifier.Protected)]
		public string viewer;

		// Token: 0x04008AF6 RID: 35574
		[PublicizedFrom(EAccessModifier.Protected)]
		public int amount;

		// Token: 0x04008AF7 RID: 35575
		[PublicizedFrom(EAccessModifier.Protected)]
		public TwitchAction.PointTypes pointType;

		// Token: 0x04008AF8 RID: 35576
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool requesterOnly = true;

		// Token: 0x04008AF9 RID: 35577
		[PublicizedFrom(EAccessModifier.Protected)]
		public string awardText = "";

		// Token: 0x04008AFA RID: 35578
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionTwitchAddPoints.RecipientTypes recipientType;

		// Token: 0x04008AFB RID: 35579
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAmount = "amount";

		// Token: 0x04008AFC RID: 35580
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPointType = "point_type";

		// Token: 0x04008AFD RID: 35581
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRecipientType = "recipient_type";

		// Token: 0x04008AFE RID: 35582
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRequesterOnly = "requester_only";

		// Token: 0x04008AFF RID: 35583
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAwardText = "award_text";

		// Token: 0x020016E3 RID: 5859
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum RecipientTypes
		{
			// Token: 0x04008B01 RID: 35585
			Requester,
			// Token: 0x04008B02 RID: 35586
			All,
			// Token: 0x04008B03 RID: 35587
			Random
		}
	}
}
