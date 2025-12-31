using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000645 RID: 1605
[Preserve]
public class MinEventActionResetProgression : MinEventActionTargetedBase
{
	// Token: 0x06003102 RID: 12546 RVA: 0x0014E944 File Offset: 0x0014CB44
	public override void Execute(MinEventParams _params)
	{
		EntityPlayerLocal entityPlayerLocal = this.targets[0] as EntityPlayerLocal;
		if (entityPlayerLocal != null)
		{
			entityPlayerLocal.Progression.ResetProgression(this.resetLevels || this.resetSkills, this.removeBooks, this.removeCrafting);
			if (this.resetLevels)
			{
				entityPlayerLocal.Progression.Level = 1;
				entityPlayerLocal.Progression.ExpToNextLevel = entityPlayerLocal.Progression.GetExpForNextLevel();
				entityPlayerLocal.Progression.SkillPoints = entityPlayerLocal.QuestJournal.GetRewardedSkillPoints();
				entityPlayerLocal.Progression.ExpDeficit = 0;
			}
			if (this.removeCrafting)
			{
				List<Recipe> recipes = CraftingManager.GetRecipes();
				for (int i = 0; i < recipes.Count; i++)
				{
					if (recipes[i].IsLearnable)
					{
						entityPlayerLocal.Buffs.RemoveCustomVar(recipes[i].GetName());
					}
				}
			}
			entityPlayerLocal.Progression.ResetProgression(this.removeBooks, false, false);
			entityPlayerLocal.Progression.bProgressionStatsChanged = true;
			entityPlayerLocal.bPlayerStatsChanged = true;
		}
	}

	// Token: 0x06003103 RID: 12547 RVA: 0x0014EA50 File Offset: 0x0014CC50
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (!(localName == "reset_books"))
			{
				if (!(localName == "reset_levels"))
				{
					if (!(localName == "reset_skills"))
					{
						if (localName == "reset_crafting")
						{
							this.removeCrafting = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
						}
					}
					else
					{
						this.resetSkills = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
					}
				}
				else
				{
					this.resetLevels = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				}
			}
			else
			{
				this.removeBooks = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
			}
		}
		return flag;
	}

	// Token: 0x0400276C RID: 10092
	[PublicizedFrom(EAccessModifier.Private)]
	public bool resetLevels;

	// Token: 0x0400276D RID: 10093
	[PublicizedFrom(EAccessModifier.Private)]
	public bool resetSkills;

	// Token: 0x0400276E RID: 10094
	[PublicizedFrom(EAccessModifier.Private)]
	public bool removeBooks;

	// Token: 0x0400276F RID: 10095
	[PublicizedFrom(EAccessModifier.Private)]
	public bool removeCrafting;
}
