using System;
using UnityEngine.Scripting;

// Token: 0x020008BB RID: 2235
[Preserve]
public class ObjectiveCraft : BaseObjective
{
	// Token: 0x170006C7 RID: 1735
	// (get) Token: 0x06004152 RID: 16722 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Number;
		}
	}

	// Token: 0x06004153 RID: 16723 RVA: 0x001A730B File Offset: 0x001A550B
	public override void SetupQuestTag()
	{
		base.OwnerQuest.AddQuestTag(QuestEventManager.craftingTag);
	}

	// Token: 0x06004154 RID: 16724 RVA: 0x001A7320 File Offset: 0x001A5520
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveCraft_keyword", false);
		this.expectedItem = ItemClass.GetItem(this.ID, false);
		this.expectedItemClass = ItemClass.GetItemClass(this.ID, false);
		this.itemCount = Convert.ToInt32(this.Value);
	}

	// Token: 0x06004155 RID: 16725 RVA: 0x001A7374 File Offset: 0x001A5574
	public override void SetupDisplay()
	{
		string arg = (this.ID != "" && this.ID != null) ? Localization.Get(this.ID, false) : "Any Item";
		base.Description = string.Format(this.keyword, arg);
		this.StatusText = string.Format("{0}/{1}", base.CurrentValue, this.itemCount);
	}

	// Token: 0x06004156 RID: 16726 RVA: 0x001A73E7 File Offset: 0x001A55E7
	public override void AddHooks()
	{
		QuestEventManager.Current.CraftItem -= this.Current_CraftItem;
		QuestEventManager.Current.CraftItem += this.Current_CraftItem;
	}

	// Token: 0x06004157 RID: 16727 RVA: 0x001A7415 File Offset: 0x001A5615
	public override void RemoveHooks()
	{
		QuestEventManager.Current.CraftItem -= this.Current_CraftItem;
	}

	// Token: 0x06004158 RID: 16728 RVA: 0x001A7430 File Offset: 0x001A5630
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_CraftItem(ItemStack stack)
	{
		if (base.Complete)
		{
			return;
		}
		if (stack.itemValue.type == this.expectedItem.type && base.OwnerQuest.CheckRequirements())
		{
			base.CurrentValue += (byte)stack.count;
			this.Refresh();
		}
	}

	// Token: 0x06004159 RID: 16729 RVA: 0x001A7486 File Offset: 0x001A5686
	public override void Refresh()
	{
		if (base.Complete)
		{
			return;
		}
		base.Complete = ((int)base.CurrentValue >= this.itemCount);
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x0600415A RID: 16730 RVA: 0x001A74C0 File Offset: 0x001A56C0
	public override BaseObjective Clone()
	{
		ObjectiveCraft objectiveCraft = new ObjectiveCraft();
		this.CopyValues(objectiveCraft);
		return objectiveCraft;
	}

	// Token: 0x0600415B RID: 16731 RVA: 0x001A74DC File Offset: 0x001A56DC
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(ObjectiveCraft.PropItem))
		{
			this.ID = properties.Values[ObjectiveCraft.PropItem];
		}
		if (properties.Values.ContainsKey(ObjectiveCraft.PropCount))
		{
			this.Value = properties.Values[ObjectiveCraft.PropCount];
		}
	}

	// Token: 0x04003426 RID: 13350
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue expectedItem = ItemValue.None.Clone();

	// Token: 0x04003427 RID: 13351
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass expectedItemClass;

	// Token: 0x04003428 RID: 13352
	[PublicizedFrom(EAccessModifier.Private)]
	public int itemCount;

	// Token: 0x04003429 RID: 13353
	public static string PropItem = "item";

	// Token: 0x0400342A RID: 13354
	public static string PropCount = "count";
}
