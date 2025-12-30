using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x02001600 RID: 5632
	[Preserve]
	public class ChallengeObjectiveUseItem : BaseChallengeObjective
	{
		// Token: 0x1700137F RID: 4991
		// (get) Token: 0x0600AD47 RID: 44359 RVA: 0x00237475 File Offset: 0x00235675
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Use;
			}
		}

		// Token: 0x17001380 RID: 4992
		// (get) Token: 0x0600AD48 RID: 44360 RVA: 0x0043DC70 File Offset: 0x0043BE70
		public override string DescriptionText
		{
			get
			{
				string str = (this.overrideText != "") ? this.overrideText : Localization.Get(this.itemName, false);
				return Localization.Get("challengeObjectiveUse", false) + " " + str + ":";
			}
		}

		// Token: 0x0600AD49 RID: 44361 RVA: 0x0043DCBF File Offset: 0x0043BEBF
		public override void Init()
		{
			if (this.itemName != null)
			{
				this.itemNames = this.itemName.Split(',', StringSplitOptions.None);
				if (this.itemNames.Length > 1)
				{
					this.itemName = this.itemNames[0];
				}
			}
		}

		// Token: 0x0600AD4A RID: 44362 RVA: 0x0043DCF6 File Offset: 0x0043BEF6
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.UseItem += this.Current_UseItem;
		}

		// Token: 0x0600AD4B RID: 44363 RVA: 0x0043DD0E File Offset: 0x0043BF0E
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.UseItem -= this.Current_UseItem;
		}

		// Token: 0x0600AD4C RID: 44364 RVA: 0x0043DD28 File Offset: 0x0043BF28
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_UseItem(ItemValue itemValue)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.itemNames.ContainsCaseInsensitive(itemValue.ItemClass.Name) || (!this.itemTags.IsEmpty && itemValue.ItemClass.ItemTags.Test_AnySet(this.itemTags)))
			{
				int num = base.Current;
				base.Current = num + 1;
				if (base.Current >= this.MaxCount)
				{
					base.Current = this.MaxCount;
					this.CheckObjectiveComplete(true);
				}
			}
		}

		// Token: 0x0600AD4D RID: 44365 RVA: 0x0043DDB0 File Offset: 0x0043BFB0
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("item"))
			{
				this.itemName = e.GetAttribute("item");
			}
			if (e.HasAttribute("item_tags"))
			{
				this.itemTags = FastTags<TagGroup.Global>.Parse(e.GetAttribute("item_tags"));
			}
			if (e.HasAttribute("override_text_key"))
			{
				this.overrideText = Localization.Get(e.GetAttribute("override_text_key"), false);
				return;
			}
			if (e.HasAttribute("override_text"))
			{
				this.overrideText = e.GetAttribute("override_text");
			}
		}

		// Token: 0x0600AD4E RID: 44366 RVA: 0x0043DE70 File Offset: 0x0043C070
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveUseItem
			{
				itemName = this.itemName,
				itemNames = this.itemNames,
				itemTags = this.itemTags,
				overrideText = this.overrideText
			};
		}

		// Token: 0x040086AA RID: 34474
		[PublicizedFrom(EAccessModifier.Private)]
		public string itemName = "";

		// Token: 0x040086AB RID: 34475
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] itemNames;

		// Token: 0x040086AC RID: 34476
		[PublicizedFrom(EAccessModifier.Private)]
		public string overrideText = "";

		// Token: 0x040086AD RID: 34477
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> itemTags = FastTags<TagGroup.Global>.none;
	}
}
