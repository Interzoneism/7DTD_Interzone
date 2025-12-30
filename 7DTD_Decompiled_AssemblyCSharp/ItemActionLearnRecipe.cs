using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000525 RID: 1317
[Preserve]
public class ItemActionLearnRecipe : ItemAction
{
	// Token: 0x06002AB6 RID: 10934 RVA: 0x00118C93 File Offset: 0x00116E93
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionLearnRecipe.MyInventoryData(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002AB7 RID: 10935 RVA: 0x00118C9C File Offset: 0x00116E9C
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		if (!_props.Values.ContainsKey("Recipes_to_learn"))
		{
			this.RecipesToLearn = new string[0];
		}
		else
		{
			this.RecipesToLearn = _props.Values["Recipes_to_learn"].Replace(" ", "").Split(',', StringSplitOptions.None);
		}
		if (!_props.Values.ContainsKey("Title"))
		{
			this.Title = "The title is impossible to read.";
		}
		else
		{
			this.Title = _props.Values["Title"];
		}
		if (!_props.Values.ContainsKey("Description"))
		{
			this.Description = "The description is impossible to read.";
		}
		else
		{
			this.Description = _props.Values["Description"];
		}
		for (int i = 0; i < this.RecipesToLearn.Length; i++)
		{
			CraftingManager.LockRecipe(this.RecipesToLearn[i], CraftingManager.RecipeLockTypes.Item);
		}
	}

	// Token: 0x06002AB8 RID: 10936 RVA: 0x00118D88 File Offset: 0x00116F88
	public override void StopHolding(ItemActionData _data)
	{
		base.StopHolding(_data);
		((ItemActionLearnRecipe.MyInventoryData)_data).bReadingStarted = false;
	}

	// Token: 0x06002AB9 RID: 10937 RVA: 0x00118DA0 File Offset: 0x00116FA0
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		if (!_bReleased)
		{
			return;
		}
		if (Time.time - _actionData.lastUseTime < this.Delay)
		{
			return;
		}
		_actionData.lastUseTime = Time.time;
		_actionData.invData.holdingEntity.RightArmAnimationUse = true;
		((ItemActionLearnRecipe.MyInventoryData)_actionData).bReadingStarted = true;
	}

	// Token: 0x06002ABA RID: 10938 RVA: 0x00118DF0 File Offset: 0x00116FF0
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		ItemActionLearnRecipe.MyInventoryData myInventoryData = (ItemActionLearnRecipe.MyInventoryData)_actionData;
		return myInventoryData.bReadingStarted && Time.time - myInventoryData.lastUseTime < 2f * AnimationDelayData.AnimationDelay[myInventoryData.invData.item.HoldType.Value].RayCast;
	}

	// Token: 0x06002ABB RID: 10939 RVA: 0x00118E48 File Offset: 0x00117048
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		EntityPlayerLocal player = _actionData.invData.holdingEntity as EntityPlayerLocal;
		ItemActionLearnRecipe.MyInventoryData myInventoryData = (ItemActionLearnRecipe.MyInventoryData)_actionData;
		if (!myInventoryData.bReadingStarted || Time.time - myInventoryData.lastUseTime < AnimationDelayData.AnimationDelay[myInventoryData.invData.item.HoldType.Value].RayCast)
		{
			return;
		}
		myInventoryData.bReadingStarted = false;
		bool flag = false;
		for (int i = 0; i < this.RecipesToLearn.Length; i++)
		{
			if (CraftingManager.GetRecipe(this.RecipesToLearn[i]).tags.Equals(FastTags<TagGroup.Global>.Parse("learnable")) && myInventoryData.invData.holdingEntity.GetCVar(this.RecipesToLearn[i]) == 0f)
			{
				flag = true;
				myInventoryData.invData.holdingEntity.SetCVar(this.RecipesToLearn[i], 1f);
				GameManager.ShowTooltip(player, string.Format(Localization.Get("ttRecipeUnlocked", false), Localization.Get(this.RecipesToLearn[i], false)), false, false, 0f);
			}
		}
		if (flag)
		{
			_actionData.invData.holdingEntity.inventory.DecHoldingItem(1);
			if (this.soundStart != null)
			{
				Manager.PlayInsidePlayerHead(this.soundStart, -1, 0f, false, false);
				return;
			}
		}
		else
		{
			GameManager.ShowTooltip(player, Localization.Get("alreadyKnown", false), false, false, 0f);
		}
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x00118FA4 File Offset: 0x001171A4
	public override void GetItemValueActionInfo(ref List<string> _infoList, ItemValue _itemValue, XUi _xui, int _actionIndex = 0)
	{
		for (int i = 0; i < this.RecipesToLearn.Length; i++)
		{
			if (!XUiM_Recipes.GetRecipeIsUnlocked(_xui, this.RecipesToLearn[i]))
			{
				_infoList.Add(ItemAction.StringFormatHandler(Localization.Get("lblAttributeRecipe", false), Localization.Get(this.RecipesToLearn[i], false)));
			}
			else
			{
				_infoList.Add(ItemAction.StringFormatHandler(Localization.Get("lblAttributeRecipe", false), Localization.Get("lblKnown", false)));
			}
		}
	}

	// Token: 0x0400213E RID: 8510
	public new string[] RecipesToLearn;

	// Token: 0x0400213F RID: 8511
	public string[] SkillsToGain;

	// Token: 0x04002140 RID: 8512
	public new string Title;

	// Token: 0x04002141 RID: 8513
	public new string Description;

	// Token: 0x02000526 RID: 1318
	[PublicizedFrom(EAccessModifier.Private)]
	public class MyInventoryData : ItemActionAttackData
	{
		// Token: 0x06002ABE RID: 10942 RVA: 0x00112618 File Offset: 0x00110818
		public MyInventoryData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x04002142 RID: 8514
		public bool bReadingStarted;
	}
}
