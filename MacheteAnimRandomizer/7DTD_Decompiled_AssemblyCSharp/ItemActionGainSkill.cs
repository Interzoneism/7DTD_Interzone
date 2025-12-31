using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000521 RID: 1313
[Preserve]
public class ItemActionGainSkill : ItemAction
{
	// Token: 0x06002A9D RID: 10909 RVA: 0x001183E2 File Offset: 0x001165E2
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionGainSkill.MyInventoryData(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002A9E RID: 10910 RVA: 0x001183EC File Offset: 0x001165EC
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		if (!_props.Values.ContainsKey("Skills_to_gain"))
		{
			this.SkillsToGain = new string[0];
		}
		else
		{
			this.SkillsToGain = _props.Values["Skills_to_gain"].Split(',', StringSplitOptions.None);
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
			return;
		}
		this.Description = _props.Values["Description"];
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x001184A7 File Offset: 0x001166A7
	public override void StopHolding(ItemActionData _data)
	{
		base.StopHolding(_data);
		((ItemActionGainSkill.MyInventoryData)_data).bReadingStarted = false;
	}

	// Token: 0x06002AA0 RID: 10912 RVA: 0x001184BC File Offset: 0x001166BC
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
		((ItemActionGainSkill.MyInventoryData)_actionData).bReadingStarted = true;
	}

	// Token: 0x06002AA1 RID: 10913 RVA: 0x0011850C File Offset: 0x0011670C
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		ItemActionGainSkill.MyInventoryData myInventoryData = (ItemActionGainSkill.MyInventoryData)_actionData;
		return myInventoryData.bReadingStarted && Time.time - myInventoryData.lastUseTime < 2f * AnimationDelayData.AnimationDelay[myInventoryData.invData.item.HoldType.Value].RayCast;
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x00118564 File Offset: 0x00116764
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		ItemActionGainSkill.MyInventoryData myInventoryData = (ItemActionGainSkill.MyInventoryData)_actionData;
		if (!myInventoryData.bReadingStarted || Time.time - myInventoryData.lastUseTime < AnimationDelayData.AnimationDelay[myInventoryData.invData.item.HoldType.Value].RayCast)
		{
			return;
		}
		myInventoryData.bReadingStarted = false;
		bool flag = false;
		for (int i = 0; i < this.SkillsToGain.Length; i++)
		{
			if (myInventoryData.invData.holdingEntity is EntityPlayer)
			{
				EntityPlayerLocal entityPlayerLocal = myInventoryData.invData.holdingEntity as EntityPlayerLocal;
				ProgressionValue progressionValue = myInventoryData.invData.holdingEntity.Progression.GetProgressionValue(this.SkillsToGain[i]);
				if (progressionValue != null)
				{
					if (progressionValue.Level + 1 <= progressionValue.ProgressionClass.MaxLevel)
					{
						ProgressionValue progressionValue2 = progressionValue;
						int level = progressionValue2.Level;
						progressionValue2.Level = level + 1;
						entityPlayerLocal.MinEventContext.ProgressionValue = progressionValue;
						entityPlayerLocal.FireEvent(MinEventTypes.onPerkLevelChanged, true);
						string arg = Localization.Get(progressionValue.ProgressionClass.NameKey, false);
						GameManager.ShowTooltip(entityPlayerLocal, string.Format(Localization.Get("ttSkillLevelUp", false), arg, progressionValue.Level), false, false, 0f);
						(myInventoryData.invData.holdingEntity as EntityPlayer).bPlayerStatsChanged = true;
						flag = true;
					}
					else
					{
						GameManager.ShowTooltip(entityPlayerLocal, Localization.Get("ttSkillMaxLevel", false), false, false, 0f);
					}
				}
			}
		}
		if (flag)
		{
			_actionData.invData.holdingEntity.inventory.DecHoldingItem(1);
			if (this.soundStart != null)
			{
				Manager.PlayInsidePlayerHead(this.soundStart, -1, 0f, false, false);
			}
		}
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x0011870C File Offset: 0x0011690C
	public override void GetItemValueActionInfo(ref List<string> _infoList, ItemValue _itemValue, XUi _xui, int _actionIndex = 0)
	{
		for (int i = 0; i < this.SkillsToGain.Length; i++)
		{
			_infoList.Add(ItemAction.StringFormatHandler(Localization.Get(_xui.playerUI.entityPlayer.Progression.GetProgressionValue(this.SkillsToGain[i]).ProgressionClass.NameKey, false), "+1"));
		}
	}

	// Token: 0x04002135 RID: 8501
	public string[] SkillsToGain;

	// Token: 0x04002136 RID: 8502
	public new string Title;

	// Token: 0x04002137 RID: 8503
	public new string Description;

	// Token: 0x02000522 RID: 1314
	[PublicizedFrom(EAccessModifier.Private)]
	public class MyInventoryData : ItemActionAttackData
	{
		// Token: 0x06002AA5 RID: 10917 RVA: 0x00112618 File Offset: 0x00110818
		public MyInventoryData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x04002138 RID: 8504
		public bool bReadingStarted;
	}
}
