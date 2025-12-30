using System;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x020008B2 RID: 2226
[Preserve]
public class ObjectiveBaseFetchContainer : BaseObjective
{
	// Token: 0x170006BF RID: 1727
	// (get) Token: 0x060040FC RID: 16636 RVA: 0x001A5F17 File Offset: 0x001A4117
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			if (base.CurrentValue != 3)
			{
				return BaseObjective.ObjectiveValueTypes.Number;
			}
			return BaseObjective.ObjectiveValueTypes.Boolean;
		}
	}

	// Token: 0x060040FD RID: 16637 RVA: 0x001A5F28 File Offset: 0x001A4128
	[PublicizedFrom(EAccessModifier.Protected)]
	public void RemoveFetchItems()
	{
		if (base.CurrentValue != 3)
		{
			return;
		}
		if (this.expectedItemClass == null)
		{
			this.SetupExpectedItem();
		}
		XUi xui = LocalPlayerUI.GetUIForPlayer(base.OwnerQuest.OwnerJournal.OwnerPlayer).xui;
		XUiM_PlayerInventory playerInventory = xui.PlayerInventory;
		int num = 1;
		int num2 = 1;
		num2 -= playerInventory.Backpack.DecItem(this.expectedItem, num2, false, null);
		if (num2 > 0)
		{
			playerInventory.Toolbelt.DecItem(this.expectedItem, num2, false, null);
		}
		if (num != num2)
		{
			ItemStack stack = new ItemStack(this.expectedItem.Clone(), num - num2);
			xui.CollectedItemList.AddRemoveItemQueueEntry(stack);
		}
	}

	// Token: 0x060040FE RID: 16638 RVA: 0x001A5FC9 File Offset: 0x001A41C9
	public override void SetupQuestTag()
	{
		base.OwnerQuest.AddQuestTag(QuestEventManager.fetchTag);
	}

	// Token: 0x060040FF RID: 16639 RVA: 0x001A5FDB File Offset: 0x001A41DB
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveFetchContainer_keyword", false);
		if (this.expectedItemClass == null)
		{
			this.SetupExpectedItem();
		}
		this.SetupQuestTag();
	}

	// Token: 0x06004100 RID: 16640 RVA: 0x001A6004 File Offset: 0x001A4204
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetupExpectedItem()
	{
		if (base.OwnerQuest.QuestCode == 0)
		{
			base.OwnerQuest.SetupQuestCode();
		}
		this.expectedItemClass = ItemClass.GetItemClass(this.questItemClassID, false);
		this.expectedItem = new ItemValue(this.expectedItemClass.Id, false);
		if (this.expectedItemClass is ItemClassQuest)
		{
			ushort num = StringParsers.ParseUInt16(this.ID, 0, -1, NumberStyles.Integer);
			this.expectedItemClass = ItemClassQuest.GetItemQuestById(num);
			this.expectedItem.Seed = num;
		}
		this.expectedItem.Meta = base.OwnerQuest.QuestCode;
	}

	// Token: 0x06004101 RID: 16641 RVA: 0x001A609C File Offset: 0x001A429C
	public override void HandleCompleted()
	{
		base.HandleCompleted();
		this.RemoveFetchItems();
	}

	// Token: 0x06004102 RID: 16642 RVA: 0x001A60AA File Offset: 0x001A42AA
	public override void HandleFailed()
	{
		base.HandleFailed();
		this.RemoveFetchItems();
	}

	// Token: 0x06004103 RID: 16643 RVA: 0x001A60B8 File Offset: 0x001A42B8
	public override void ResetObjective()
	{
		this.RemoveFetchItems();
	}

	// Token: 0x06004104 RID: 16644 RVA: 0x001A60C0 File Offset: 0x001A42C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public int GetItemCount(int _expectedMeta = -2)
	{
		XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(base.OwnerQuest.OwnerJournal.OwnerPlayer).xui.PlayerInventory;
		if (_expectedMeta == -2)
		{
			_expectedMeta = base.OwnerQuest.QuestCode;
		}
		this.expectedItem.Meta = _expectedMeta;
		return playerInventory.Backpack.GetItemCount(this.expectedItem, -1, _expectedMeta, true) + playerInventory.Toolbelt.GetItemCount(this.expectedItem, false, -1, _expectedMeta, true);
	}

	// Token: 0x06004105 RID: 16645 RVA: 0x001A6135 File Offset: 0x001A4335
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CopyValues(BaseObjective objective)
	{
		base.CopyValues(objective);
		((ObjectiveBaseFetchContainer)objective).questItemClassID = this.questItemClassID;
	}

	// Token: 0x06004106 RID: 16646 RVA: 0x001A6150 File Offset: 0x001A4350
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(ObjectiveBaseFetchContainer.PropQuestItemClass))
		{
			this.questItemClassID = properties.Values[ObjectiveBaseFetchContainer.PropQuestItemClass];
		}
		if (properties.Values.ContainsKey(ObjectiveBaseFetchContainer.PropQuestItemID))
		{
			this.ID = properties.Values[ObjectiveBaseFetchContainer.PropQuestItemID];
		}
		if (properties.Values.ContainsKey(ObjectiveBaseFetchContainer.PropItemCount))
		{
			this.Value = properties.Values[ObjectiveBaseFetchContainer.PropItemCount];
		}
		if (properties.Values.ContainsKey(ObjectiveBaseFetchContainer.PropDefaultContainer))
		{
			this.defaultContainer = properties.Values[ObjectiveBaseFetchContainer.PropDefaultContainer];
		}
	}

	// Token: 0x04003408 RID: 13320
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemValue expectedItem = ItemValue.None.Clone();

	// Token: 0x04003409 RID: 13321
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemClass expectedItemClass;

	// Token: 0x0400340A RID: 13322
	[PublicizedFrom(EAccessModifier.Protected)]
	public float distance;

	// Token: 0x0400340B RID: 13323
	[PublicizedFrom(EAccessModifier.Protected)]
	public int currentCount;

	// Token: 0x0400340C RID: 13324
	[PublicizedFrom(EAccessModifier.Protected)]
	public string defaultContainer = "";

	// Token: 0x0400340D RID: 13325
	public string questItemClassID = "questItem";

	// Token: 0x0400340E RID: 13326
	public static string PropQuestItemClass = "quest_item";

	// Token: 0x0400340F RID: 13327
	public static string PropQuestItemID = "quest_item_ID";

	// Token: 0x04003410 RID: 13328
	public static string PropItemCount = "item_count";

	// Token: 0x04003411 RID: 13329
	public static string PropDefaultContainer = "default_container";

	// Token: 0x04003412 RID: 13330
	[PublicizedFrom(EAccessModifier.Protected)]
	public World world;
}
