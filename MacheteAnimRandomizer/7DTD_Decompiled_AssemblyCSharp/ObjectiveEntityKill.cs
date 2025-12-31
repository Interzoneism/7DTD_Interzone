using System;
using UnityEngine.Scripting;

// Token: 0x020008BC RID: 2236
[Preserve]
public class ObjectiveEntityKill : BaseObjective
{
	// Token: 0x170006C8 RID: 1736
	// (get) Token: 0x0600415E RID: 16734 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Number;
		}
	}

	// Token: 0x170006C9 RID: 1737
	// (get) Token: 0x0600415F RID: 16735 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool RequiresZombies
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06004160 RID: 16736 RVA: 0x001A7570 File Offset: 0x001A5770
	public override void SetupObjective()
	{
		this.neededKillCount = Convert.ToInt32(this.Value);
		if (this.ID != null)
		{
			string[] array = this.ID.Split(',', StringSplitOptions.None);
			if (array.Length > 1)
			{
				this.ID = array[0];
				this.entityNames = new string[array.Length - 1];
				for (int i = 1; i < array.Length; i++)
				{
					this.entityNames[i - 1] = array[i];
				}
			}
		}
	}

	// Token: 0x06004161 RID: 16737 RVA: 0x001A75E0 File Offset: 0x001A57E0
	public override void SetupDisplay()
	{
		this.keyword = Localization.Get("ObjectiveZombieKill_keyword", false);
		if (this.localizedName == "")
		{
			this.localizedName = ((this.ID != null && this.ID != "") ? Localization.Get(this.ID, false) : "Any Zombie");
		}
		base.Description = string.Format(this.keyword, this.localizedName);
		this.StatusText = string.Format("{0}/{1}", base.CurrentValue, this.neededKillCount);
	}

	// Token: 0x06004162 RID: 16738 RVA: 0x001A7680 File Offset: 0x001A5880
	public override void AddHooks()
	{
		QuestEventManager.Current.EntityKill += this.Current_EntityKill;
	}

	// Token: 0x06004163 RID: 16739 RVA: 0x001A7698 File Offset: 0x001A5898
	public override void RemoveHooks()
	{
		QuestEventManager.Current.EntityKill -= this.Current_EntityKill;
	}

	// Token: 0x06004164 RID: 16740 RVA: 0x001A76B0 File Offset: 0x001A58B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_EntityKill(EntityAlive killedBy, EntityAlive killedEntity)
	{
		if (base.Complete)
		{
			return;
		}
		bool flag = false;
		string entityClassName = killedEntity.EntityClass.entityClassName;
		if (this.targetTags.IsEmpty)
		{
			if (this.ID == null || entityClassName.EqualsCaseInsensitive(this.ID))
			{
				flag = true;
			}
			if (!flag && this.entityNames != null)
			{
				for (int i = 0; i < this.entityNames.Length; i++)
				{
					if (this.entityNames[i].EqualsCaseInsensitive(entityClassName))
					{
						flag = true;
						break;
					}
				}
			}
		}
		else
		{
			flag = killedEntity.EntityClass.Tags.Test_AnySet(this.targetTags);
		}
		if (flag && base.OwnerQuest.CheckRequirements())
		{
			byte currentValue = base.CurrentValue;
			base.CurrentValue = currentValue + 1;
			this.Refresh();
		}
	}

	// Token: 0x06004165 RID: 16741 RVA: 0x001A776B File Offset: 0x001A596B
	public override void Refresh()
	{
		if (base.Complete)
		{
			return;
		}
		base.Complete = ((int)base.CurrentValue >= this.neededKillCount);
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x06004166 RID: 16742 RVA: 0x001A77A4 File Offset: 0x001A59A4
	public override BaseObjective Clone()
	{
		ObjectiveEntityKill objectiveEntityKill = new ObjectiveEntityKill();
		this.CopyValues(objectiveEntityKill);
		objectiveEntityKill.localizedName = this.localizedName;
		objectiveEntityKill.targetTags = this.targetTags;
		return objectiveEntityKill;
	}

	// Token: 0x06004167 RID: 16743 RVA: 0x001A77D8 File Offset: 0x001A59D8
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(ObjectiveEntityKill.PropObjectiveKey))
		{
			this.localizedName = Localization.Get(properties.Values[ObjectiveEntityKill.PropObjectiveKey], false);
		}
		properties.ParseString(ObjectiveEntityKill.PropEntityNames, ref this.ID);
		properties.ParseString(ObjectiveEntityKill.PropNeededCount, ref this.Value);
		string str = "";
		properties.ParseString(ObjectiveEntityKill.PropTargetTags, ref str);
		this.targetTags = FastTags<TagGroup.Global>.Parse(str);
	}

	// Token: 0x06004168 RID: 16744 RVA: 0x001A785C File Offset: 0x001A5A5C
	public override string ParseBinding(string bindingName)
	{
		string id = this.ID;
		string value = this.Value;
		if (this.localizedName == "")
		{
			this.localizedName = ((id != null && id != "") ? Localization.Get(id, false) : "Any Zombie");
		}
		if (bindingName == "target")
		{
			return this.localizedName;
		}
		if (!(bindingName == "targetwithcount"))
		{
			return "";
		}
		return Convert.ToInt32(value).ToString() + " " + this.localizedName;
	}

	// Token: 0x0400342B RID: 13355
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedName = "";

	// Token: 0x0400342C RID: 13356
	[PublicizedFrom(EAccessModifier.Private)]
	public int neededKillCount;

	// Token: 0x0400342D RID: 13357
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] entityNames;

	// Token: 0x0400342E RID: 13358
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> targetTags = FastTags<TagGroup.Global>.none;

	// Token: 0x0400342F RID: 13359
	public static string PropObjectiveKey = "objective_name_key";

	// Token: 0x04003430 RID: 13360
	public static string PropNeededCount = "needed_count";

	// Token: 0x04003431 RID: 13361
	public static string PropEntityNames = "entity_names";

	// Token: 0x04003432 RID: 13362
	public static string PropTargetTags = "target_tags";
}
