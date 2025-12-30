using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015EC RID: 5612
	[Preserve]
	public class ChallengeObjectiveCureDebuff : BaseChallengeObjective
	{
		// Token: 0x17001352 RID: 4946
		// (get) Token: 0x0600AC7A RID: 44154 RVA: 0x00075E2B File Offset: 0x0007402B
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.CureDebuff;
			}
		}

		// Token: 0x17001353 RID: 4947
		// (get) Token: 0x0600AC7B RID: 44155 RVA: 0x0043ABAD File Offset: 0x00438DAD
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("challengeObjectiveCure", false) + " " + BuffManager.GetBuff(this.buffName).LocalizedName + ":";
			}
		}

		// Token: 0x0600AC7C RID: 44156 RVA: 0x0043ABDC File Offset: 0x00438DDC
		public override void Init()
		{
			if (this.buffName != null)
			{
				this.buffNames = this.buffName.Split(',', StringSplitOptions.None);
				if (this.buffNames.Length > 1)
				{
					this.buffName = this.buffNames[0];
				}
			}
			if (this.itemName != null)
			{
				this.itemNames = this.itemName.Split(',', StringSplitOptions.None);
				if (this.itemNames.Length > 1)
				{
					this.itemName = this.itemNames[0];
				}
			}
		}

		// Token: 0x0600AC7D RID: 44157 RVA: 0x0043AC53 File Offset: 0x00438E53
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.UseItem += this.Current_UseItem;
		}

		// Token: 0x0600AC7E RID: 44158 RVA: 0x0043AC6B File Offset: 0x00438E6B
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.UseItem -= this.Current_UseItem;
		}

		// Token: 0x0600AC7F RID: 44159 RVA: 0x0043AC84 File Offset: 0x00438E84
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_UseItem(ItemValue itemValue)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.itemNames.ContainsCaseInsensitive(itemValue.ItemClass.Name) && this.PlayerHasBuff())
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

		// Token: 0x0600AC80 RID: 44160 RVA: 0x0043ACEC File Offset: 0x00438EEC
		[PublicizedFrom(EAccessModifier.Private)]
		public bool PlayerHasBuff()
		{
			EntityBuffs buffs = this.Owner.Owner.Player.Buffs;
			for (int i = 0; i < this.buffNames.Length; i++)
			{
				if (buffs.HasBuff(this.buffNames[i]))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600AC81 RID: 44161 RVA: 0x0043AD38 File Offset: 0x00438F38
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("debuff"))
			{
				this.buffName = e.GetAttribute("debuff");
			}
			if (e.HasAttribute("item"))
			{
				this.itemName = e.GetAttribute("item");
			}
		}

		// Token: 0x0600AC82 RID: 44162 RVA: 0x0043AD9C File Offset: 0x00438F9C
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveCureDebuff
			{
				buffName = this.buffName,
				buffNames = this.buffNames,
				itemName = this.itemName,
				itemNames = this.itemNames
			};
		}

		// Token: 0x04008662 RID: 34402
		[PublicizedFrom(EAccessModifier.Private)]
		public string buffName = "";

		// Token: 0x04008663 RID: 34403
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] buffNames;

		// Token: 0x04008664 RID: 34404
		[PublicizedFrom(EAccessModifier.Private)]
		public string itemName = "";

		// Token: 0x04008665 RID: 34405
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] itemNames;
	}
}
