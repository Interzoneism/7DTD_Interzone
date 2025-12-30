using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015F3 RID: 5619
	[Preserve]
	public class ChallengeObjectiveHold : BaseChallengeObjective
	{
		// Token: 0x17001361 RID: 4961
		// (get) Token: 0x0600ACD1 RID: 44241 RVA: 0x000768A9 File Offset: 0x00074AA9
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Hold;
			}
		}

		// Token: 0x17001362 RID: 4962
		// (get) Token: 0x0600ACD2 RID: 44242 RVA: 0x0043C690 File Offset: 0x0043A890
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveHold", false) + " " + Localization.Get(this.itemClassList[0], false) + ":";
			}
		}

		// Token: 0x0600ACD3 RID: 44243 RVA: 0x0043C6BA File Offset: 0x0043A8BA
		public override void Init()
		{
			this.itemClassList = this.itemClassID.Split(',', StringSplitOptions.None);
			this.expectedItemClass = ItemClass.GetItemClass(this.itemClassList[0], false);
		}

		// Token: 0x0600ACD4 RID: 44244 RVA: 0x0043C6E4 File Offset: 0x0043A8E4
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.HoldItem -= this.Current_HoldItem;
			QuestEventManager.Current.HoldItem += this.Current_HoldItem;
		}

		// Token: 0x0600ACD5 RID: 44245 RVA: 0x0043C712 File Offset: 0x0043A912
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_HoldItem(ItemValue itemValue)
		{
			if (itemValue.ItemClass != null && this.itemClassList.ContainsCaseInsensitive(itemValue.ItemClass.Name))
			{
				base.Current = this.MaxCount;
			}
			else
			{
				base.Current = 0;
			}
			this.CheckObjectiveComplete(true);
		}

		// Token: 0x0600ACD6 RID: 44246 RVA: 0x0043C751 File Offset: 0x0043A951
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.HoldItem -= this.Current_HoldItem;
		}

		// Token: 0x0600ACD7 RID: 44247 RVA: 0x0043C76C File Offset: 0x0043A96C
		public override bool HandleCheckStatus()
		{
			ItemClass holdingItem = this.Owner.Owner.Player.inventory.holdingItem;
			if (holdingItem != null)
			{
				base.Current = (this.itemClassList.ContainsCaseInsensitive(holdingItem.Name) ? this.MaxCount : 0);
			}
			base.Complete = this.CheckObjectiveComplete(false);
			return base.Complete;
		}

		// Token: 0x0600ACD8 RID: 44248 RVA: 0x0043C7CC File Offset: 0x0043A9CC
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("item"))
			{
				this.itemClassID = e.GetAttribute("item");
			}
		}

		// Token: 0x0600ACD9 RID: 44249 RVA: 0x0043C7FD File Offset: 0x0043A9FD
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveHold
			{
				itemClassID = this.itemClassID,
				expectedItem = this.expectedItem,
				expectedItemClass = this.expectedItemClass
			};
		}

		// Token: 0x04008680 RID: 34432
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemValue expectedItem = ItemValue.None.Clone();

		// Token: 0x04008681 RID: 34433
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedItemClass;

		// Token: 0x04008682 RID: 34434
		public string itemClassID = "";

		// Token: 0x04008683 RID: 34435
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] itemClassList;
	}
}
