using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015E6 RID: 5606
	[Preserve]
	public class ChallengeObjectiveBlockPlace : BaseChallengeObjective
	{
		// Token: 0x17001345 RID: 4933
		// (get) Token: 0x0600AC33 RID: 44083 RVA: 0x000197A5 File Offset: 0x000179A5
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.BlockPlace;
			}
		}

		// Token: 0x17001346 RID: 4934
		// (get) Token: 0x0600AC34 RID: 44084 RVA: 0x00439C1D File Offset: 0x00437E1D
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("xuiWorldPrefabsPlace", false) + " " + Localization.Get(this.expectedBlock, false) + ":";
			}
		}

		// Token: 0x0600AC35 RID: 44085 RVA: 0x00439C45 File Offset: 0x00437E45
		public override void HandleOnCreated()
		{
			base.HandleOnCreated();
			this.CreateRequirements();
		}

		// Token: 0x0600AC36 RID: 44086 RVA: 0x00439C53 File Offset: 0x00437E53
		[PublicizedFrom(EAccessModifier.Private)]
		public void CreateRequirements()
		{
			if (!this.ShowRequirements)
			{
				return;
			}
			this.Owner.SetRequirementGroup(new RequirementObjectiveGroupPlace((this.alternateItem != "") ? this.alternateItem : this.expectedBlock));
		}

		// Token: 0x0600AC37 RID: 44087 RVA: 0x00439C8E File Offset: 0x00437E8E
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.BlockPlace += this.Current_BlockPlace;
		}

		// Token: 0x0600AC38 RID: 44088 RVA: 0x00439CA6 File Offset: 0x00437EA6
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.BlockPlace -= this.Current_BlockPlace;
		}

		// Token: 0x0600AC39 RID: 44089 RVA: 0x00439CC0 File Offset: 0x00437EC0
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_BlockPlace(string blockName, Vector3i blockPos)
		{
			bool flag = false;
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.expectedBlock == null || this.expectedBlock == "" || this.expectedBlock.EqualsCaseInsensitive(blockName))
			{
				flag = true;
			}
			if (!flag && this.expectedBlock != null && this.expectedBlock != "")
			{
				Block blockByName = Block.GetBlockByName(this.expectedBlock, true);
				if (blockByName != null && blockByName.SelectAlternates && blockByName.ContainsAlternateBlock(blockName))
				{
					flag = true;
				}
			}
			if (flag)
			{
				int num = base.Current;
				base.Current = num + 1;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600AC3A RID: 44090 RVA: 0x00439D60 File Offset: 0x00437F60
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("block"))
			{
				this.expectedBlock = e.GetAttribute("block");
			}
			if (e.HasAttribute("alternate_item"))
			{
				this.alternateItem = e.GetAttribute("alternate_item");
			}
		}

		// Token: 0x0600AC3B RID: 44091 RVA: 0x00439DC4 File Offset: 0x00437FC4
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveBlockPlace
			{
				expectedBlock = this.expectedBlock,
				alternateItem = this.alternateItem
			};
		}

		// Token: 0x0400864E RID: 34382
		public string expectedBlock = "";

		// Token: 0x0400864F RID: 34383
		public string alternateItem = "";
	}
}
