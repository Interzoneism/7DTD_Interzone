using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015F9 RID: 5625
	[Preserve]
	public class ChallengeObjectiveScrap : BaseChallengeObjective
	{
		// Token: 0x1700136D RID: 4973
		// (get) Token: 0x0600AD07 RID: 44295 RVA: 0x000E74AA File Offset: 0x000E56AA
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Scrap;
			}
		}

		// Token: 0x1700136E RID: 4974
		// (get) Token: 0x0600AD08 RID: 44296 RVA: 0x0043D184 File Offset: 0x0043B384
		public override string DescriptionText
		{
			get
			{
				string str = (this.expectedItemClass != null) ? this.expectedItemClass.GetLocalizedItemName() : Localization.Get("xuiItems", false);
				return Localization.Get("challengeObjectiveScrap", false) + " " + str + ":";
			}
		}

		// Token: 0x0600AD09 RID: 44297 RVA: 0x0043D1CD File Offset: 0x0043B3CD
		public override void Init()
		{
			this.expectedItem = ItemClass.GetItem(this.itemClassID, false);
			this.expectedItemClass = ItemClass.GetItemClass(this.itemClassID, false);
		}

		// Token: 0x0600AD0A RID: 44298 RVA: 0x0043D1F3 File Offset: 0x0043B3F3
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.ScrapItem += this.Current_ScrapItem;
		}

		// Token: 0x0600AD0B RID: 44299 RVA: 0x0043D20C File Offset: 0x0043B40C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_ScrapItem(ItemStack stack)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.expectedItemClass == null || stack.itemValue.type == this.expectedItem.type)
			{
				base.Current += stack.count;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600AD0C RID: 44300 RVA: 0x0043D25D File Offset: 0x0043B45D
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.ScrapItem -= this.Current_ScrapItem;
		}

		// Token: 0x0600AD0D RID: 44301 RVA: 0x0043D275 File Offset: 0x0043B475
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("item"))
			{
				this.itemClassID = e.GetAttribute("item");
			}
		}

		// Token: 0x0600AD0E RID: 44302 RVA: 0x0043D2A6 File Offset: 0x0043B4A6
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveScrap
			{
				itemClassID = this.itemClassID,
				expectedItem = this.expectedItem,
				expectedItemClass = this.expectedItemClass
			};
		}

		// Token: 0x04008692 RID: 34450
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemValue expectedItem = ItemValue.None.Clone();

		// Token: 0x04008693 RID: 34451
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedItemClass;

		// Token: 0x04008694 RID: 34452
		public string itemClassID = "";
	}
}
