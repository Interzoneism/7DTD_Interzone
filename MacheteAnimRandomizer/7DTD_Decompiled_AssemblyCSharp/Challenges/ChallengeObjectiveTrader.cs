using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015FD RID: 5629
	[Preserve]
	public class ChallengeObjectiveTrader : BaseChallengeObjective
	{
		// Token: 0x1700137A RID: 4986
		// (get) Token: 0x0600AD34 RID: 44340 RVA: 0x00198AC2 File Offset: 0x00196CC2
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Trader;
			}
		}

		// Token: 0x1700137B RID: 4987
		// (get) Token: 0x0600AD35 RID: 44341 RVA: 0x0043D7F0 File Offset: 0x0043B9F0
		public override string DescriptionText
		{
			get
			{
				if (string.IsNullOrEmpty(this.TraderName))
				{
					if (!this.BuyItems)
					{
						return Localization.Get("challengeObjectiveSellItems", false);
					}
					return Localization.Get("challengeObjectiveBuyItems", false);
				}
				else
				{
					if (!this.BuyItems)
					{
						return string.Format(Localization.Get("challengeObjectiveSellItemsTo", false), Localization.Get(this.TraderName, false));
					}
					return string.Format(Localization.Get("challengeObjectiveBuyItemsFrom", false), Localization.Get(this.TraderName, false));
				}
			}
		}

		// Token: 0x0600AD36 RID: 44342 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Init()
		{
		}

		// Token: 0x0600AD37 RID: 44343 RVA: 0x0043D86B File Offset: 0x0043BA6B
		public override void HandleAddHooks()
		{
			if (this.BuyItems)
			{
				QuestEventManager.Current.BuyItems += this.Current_BuyItems;
				return;
			}
			QuestEventManager.Current.SellItems += this.Current_SellItems;
		}

		// Token: 0x0600AD38 RID: 44344 RVA: 0x0043D8A2 File Offset: 0x0043BAA2
		public override void HandleRemoveHooks()
		{
			if (this.BuyItems)
			{
				QuestEventManager.Current.BuyItems -= this.Current_BuyItems;
				return;
			}
			QuestEventManager.Current.SellItems -= this.Current_SellItems;
		}

		// Token: 0x0600AD39 RID: 44345 RVA: 0x0043D8DC File Offset: 0x0043BADC
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_BuyItems(string traderName, int itemCounts)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.TraderName == "" || traderName == this.TraderName)
			{
				base.Current += itemCounts;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600AD3A RID: 44346 RVA: 0x0043D944 File Offset: 0x0043BB44
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_SellItems(string traderName, int itemCounts)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.TraderName == "" || traderName == this.TraderName)
			{
				base.Current += itemCounts;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600AD3B RID: 44347 RVA: 0x0043D9AC File Offset: 0x0043BBAC
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("is_buy"))
			{
				this.BuyItems = StringParsers.ParseBool(e.GetAttribute("is_buy"), 0, -1, true);
			}
			if (e.HasAttribute("trader_name"))
			{
				this.TraderName = e.GetAttribute("trader_name");
			}
		}

		// Token: 0x0600AD3C RID: 44348 RVA: 0x0043DA18 File Offset: 0x0043BC18
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveTrader
			{
				BuyItems = this.BuyItems,
				TraderName = this.TraderName
			};
		}

		// Token: 0x0400869B RID: 34459
		[PublicizedFrom(EAccessModifier.Private)]
		public bool BuyItems;

		// Token: 0x0400869C RID: 34460
		[PublicizedFrom(EAccessModifier.Private)]
		public string TraderName = "";
	}
}
