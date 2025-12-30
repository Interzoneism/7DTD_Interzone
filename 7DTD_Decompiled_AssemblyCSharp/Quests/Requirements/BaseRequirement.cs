using System;

namespace Quests.Requirements
{
	// Token: 0x020015B1 RID: 5553
	public abstract class BaseRequirement
	{
		// Token: 0x17001307 RID: 4871
		// (get) Token: 0x0600AA9A RID: 43674 RVA: 0x00433BEC File Offset: 0x00431DEC
		// (set) Token: 0x0600AA9B RID: 43675 RVA: 0x00433BF4 File Offset: 0x00431DF4
		public string ID { get; set; }

		// Token: 0x17001308 RID: 4872
		// (get) Token: 0x0600AA9C RID: 43676 RVA: 0x00433BFD File Offset: 0x00431DFD
		// (set) Token: 0x0600AA9D RID: 43677 RVA: 0x00433C05 File Offset: 0x00431E05
		public string Value { get; set; }

		// Token: 0x17001309 RID: 4873
		// (get) Token: 0x0600AA9E RID: 43678 RVA: 0x00433C0E File Offset: 0x00431E0E
		// (set) Token: 0x0600AA9F RID: 43679 RVA: 0x00433C16 File Offset: 0x00431E16
		public bool Complete { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700130A RID: 4874
		// (get) Token: 0x0600AAA0 RID: 43680 RVA: 0x00433C1F File Offset: 0x00431E1F
		// (set) Token: 0x0600AAA1 RID: 43681 RVA: 0x00433C27 File Offset: 0x00431E27
		public Quest OwnerQuest { get; set; }

		// Token: 0x1700130B RID: 4875
		// (get) Token: 0x0600AAA2 RID: 43682 RVA: 0x00433C30 File Offset: 0x00431E30
		// (set) Token: 0x0600AAA3 RID: 43683 RVA: 0x00433C38 File Offset: 0x00431E38
		public QuestClass Owner { get; set; }

		// Token: 0x1700130C RID: 4876
		// (get) Token: 0x0600AAA4 RID: 43684 RVA: 0x00433C41 File Offset: 0x00431E41
		// (set) Token: 0x0600AAA5 RID: 43685 RVA: 0x00433C49 File Offset: 0x00431E49
		public string Description { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700130D RID: 4877
		// (get) Token: 0x0600AAA6 RID: 43686 RVA: 0x00433C52 File Offset: 0x00431E52
		// (set) Token: 0x0600AAA7 RID: 43687 RVA: 0x00433C5A File Offset: 0x00431E5A
		public string StatusText { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x1700130E RID: 4878
		// (get) Token: 0x0600AAA8 RID: 43688 RVA: 0x00433C63 File Offset: 0x00431E63
		// (set) Token: 0x0600AAA9 RID: 43689 RVA: 0x00433C6B File Offset: 0x00431E6B
		public int Phase { get; set; }

		// Token: 0x0600AAAA RID: 43690 RVA: 0x00433C74 File Offset: 0x00431E74
		public virtual void HandleVariables()
		{
			this.ID = this.OwnerQuest.ParseVariable(this.ID);
			this.Value = this.OwnerQuest.ParseVariable(this.Value);
		}

		// Token: 0x0600AAAB RID: 43691 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void SetupRequirement()
		{
		}

		// Token: 0x0600AAAC RID: 43692 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public virtual bool CheckRequirement()
		{
			return false;
		}

		// Token: 0x0600AAAD RID: 43693 RVA: 0x00019766 File Offset: 0x00017966
		public virtual BaseRequirement Clone()
		{
			return null;
		}

		// Token: 0x0600AAAE RID: 43694 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Protected)]
		public BaseRequirement()
		{
		}

		// Token: 0x04008546 RID: 34118
		public DynamicProperties Properties;

		// Token: 0x020015B2 RID: 5554
		public enum RequirementTypes
		{
			// Token: 0x04008548 RID: 34120
			Buff,
			// Token: 0x04008549 RID: 34121
			Holding,
			// Token: 0x0400854A RID: 34122
			Level,
			// Token: 0x0400854B RID: 34123
			Wearing
		}
	}
}
