using System;
using System.Collections.Generic;
using System.Text;
using Audio;
using UnityEngine;
using XMLData.Item;

// Token: 0x020004FE RID: 1278
public abstract class ItemAction : XMLData.Item.ItemActionData
{
	// Token: 0x060029AD RID: 10669 RVA: 0x0010FC6F File Offset: 0x0010DE6F
	public string GetSoundStart()
	{
		return this.soundStart;
	}

	// Token: 0x060029AE RID: 10670 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual ItemClass.EnumCrosshairType GetCrosshairType(global::ItemActionData _actionData)
	{
		return ItemClass.EnumCrosshairType.Plus;
	}

	// Token: 0x060029AF RID: 10671 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsEndDelayed()
	{
		return false;
	}

	// Token: 0x060029B0 RID: 10672 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnHUD(global::ItemActionData _actionData, int _x, int _y)
	{
	}

	// Token: 0x060029B1 RID: 10673 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnScreenOverlay(global::ItemActionData _data)
	{
	}

	// Token: 0x060029B2 RID: 10674 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool ConsumeScrollWheel(global::ItemActionData _actionData, float _scrollWheelInput, PlayerActionsLocal _playerInput)
	{
		return false;
	}

	// Token: 0x060029B3 RID: 10675 RVA: 0x0010FC77 File Offset: 0x0010DE77
	public virtual global::ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new global::ItemActionData(_invData, _indexInEntityOfAction);
	}

	// Token: 0x060029B4 RID: 10676 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual RenderCubeType GetFocusType(global::ItemActionData _actionData)
	{
		return RenderCubeType.None;
	}

	// Token: 0x060029B5 RID: 10677 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsFocusBlockInside()
	{
		return true;
	}

	// Token: 0x060029B6 RID: 10678 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsHUDDisabled(global::ItemActionData _data)
	{
		return false;
	}

	// Token: 0x060029B7 RID: 10679 RVA: 0x0010FC80 File Offset: 0x0010DE80
	public virtual void StartHolding(global::ItemActionData _data)
	{
		this.OnModificationsChanged(_data);
	}

	// Token: 0x060029B8 RID: 10680 RVA: 0x0010FC89 File Offset: 0x0010DE89
	public virtual void StopHolding(global::ItemActionData _data)
	{
		_data.invData.holdingEntity.StopAnimatorAudio(Entity.StopAnimatorAudioType.StopOnStopHolding);
	}

	// Token: 0x060029B9 RID: 10681 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnHoldingUpdate(global::ItemActionData _actionData)
	{
	}

	// Token: 0x060029BA RID: 10682 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ShowItem(global::ItemActionData _actionData, bool _show)
	{
	}

	// Token: 0x060029BB RID: 10683 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnModificationsChanged(global::ItemActionData _data)
	{
	}

	// Token: 0x060029BC RID: 10684 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool CanCancel(global::ItemActionData _data)
	{
		return false;
	}

	// Token: 0x060029BD RID: 10685 RVA: 0x0010FC9C File Offset: 0x0010DE9C
	public virtual void CancelReload(global::ItemActionData _actionData, bool holsterWeapon)
	{
		EntityPlayerLocal entityPlayerLocal = _actionData.invData.holdingEntity as EntityPlayerLocal;
		if (entityPlayerLocal != null && holsterWeapon)
		{
			entityPlayerLocal.HolsterWeapon(true);
		}
	}

	// Token: 0x060029BE RID: 10686 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool AllowConcurrentActions()
	{
		return false;
	}

	// Token: 0x060029BF RID: 10687 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Cleanup(global::ItemActionData _data)
	{
	}

	// Token: 0x060029C0 RID: 10688 RVA: 0x0010FCD0 File Offset: 0x0010DED0
	public virtual void ReadFrom(DynamicProperties _props)
	{
		this.Delay = 0f;
		_props.ParseFloat("Delay", ref this.Delay);
		_props.ParseString("Sound_start", ref this.soundStart);
		_props.ParseBool("Sound_in_head", ref this.Sound_in_head);
		if (_props.Values.ContainsKey("Particle_harvesting"))
		{
			this.bUseParticleHarvesting = StringParsers.ParseBool(_props.Values["Particle_harvesting"], 0, -1, true);
			this.particleHarvestingCategory = _props.Params1["Particle_harvesting"];
		}
		this.ActionExp = 2;
		_props.ParseInt("ActionExp", ref this.ActionExp);
		this.ActionExpBonusMultiplier = 10f;
		_props.ParseFloat("ActionExpBonusMultiplier", ref this.ActionExpBonusMultiplier);
		_props.ParseBool("UseAnimation", ref this.UseAnimation);
		this.BuffActions = new List<string>();
		if (_props.Values.ContainsKey("Buff"))
		{
			if (_props.Values["Buff"].Contains(","))
			{
				string[] collection = _props.Values["Buff"].Replace(" ", "").Split(',', StringSplitOptions.None);
				this.BuffActions.AddRange(collection);
			}
			else
			{
				this.BuffActions.Add(_props.Values["Buff"].Trim());
			}
		}
		else
		{
			this.ActionExpBonusMultiplier = 10f;
		}
		this.Properties = _props;
	}

	// Token: 0x060029C1 RID: 10689 RVA: 0x0010FE49 File Offset: 0x0010E049
	public string GetDescription()
	{
		return this.Properties.GetString("Description");
	}

	// Token: 0x060029C2 RID: 10690 RVA: 0x00019766 File Offset: 0x00017966
	public virtual string CanInteract(global::ItemActionData _actionData)
	{
		return null;
	}

	// Token: 0x060029C3 RID: 10691 RVA: 0x0010FE5C File Offset: 0x0010E05C
	public static void ExecuteBuffActions(List<string> actions, int instigatorId, EntityAlive target, bool isCritical, EnumBodyPartHit hitLocation, string context)
	{
		if (target == null)
		{
			return;
		}
		EntityAlive entityAlive = GameManager.Instance.World.GetEntity(instigatorId) as EntityAlive;
		if (entityAlive == null)
		{
			return;
		}
		if (actions != null)
		{
			for (int i = 0; i < actions.Count; i++)
			{
				BuffClass buff = BuffManager.GetBuff(actions[i]);
				if (buff != null)
				{
					float num = 1f;
					num = EffectManager.GetValue(PassiveEffects.BuffProcChance, null, num, entityAlive, null, FastTags<TagGroup.Global>.Parse(buff.Name), true, true, true, true, true, 1, true, false);
					if (target.rand.RandomFloat <= num)
					{
						target.Buffs.AddBuff(actions[i], entityAlive.entityId, true, false, -1f);
					}
				}
			}
		}
	}

	// Token: 0x060029C4 RID: 10692 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsActionRunning(global::ItemActionData _actionData)
	{
		return false;
	}

	// Token: 0x060029C5 RID: 10693 RVA: 0x0010FF10 File Offset: 0x0010E110
	public virtual bool CanExecute(global::ItemActionData _actionData)
	{
		if (this.ExecutionRequirements == null)
		{
			return true;
		}
		_actionData.invData.holdingEntity.MinEventContext.ItemValue = _actionData.invData.itemValue;
		for (int i = 0; i < this.ExecutionRequirements.Count; i++)
		{
			if (!this.ExecutionRequirements[i].IsValid(_actionData.invData.holdingEntity.MinEventContext))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060029C6 RID: 10694
	public abstract void ExecuteAction(global::ItemActionData _actionData, bool _bReleased);

	// Token: 0x060029C7 RID: 10695 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool ExecuteInstantAction(EntityAlive ent, ItemStack stack, bool isHeldItem, XUiC_ItemStack stackController)
	{
		return false;
	}

	// Token: 0x060029C8 RID: 10696 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void CancelAction(global::ItemActionData _actionData)
	{
	}

	// Token: 0x060029C9 RID: 10697 RVA: 0x00019766 File Offset: 0x00017966
	public virtual WorldRayHitInfo GetExecuteActionTarget(global::ItemActionData _actionData)
	{
		return null;
	}

	// Token: 0x060029CA RID: 10698 RVA: 0x0010FF83 File Offset: 0x0010E183
	public virtual void GetIronSights(global::ItemActionData _actionData, out float _fov)
	{
		_fov = 0f;
	}

	// Token: 0x060029CB RID: 10699 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual EnumCameraShake GetCameraShakeType(global::ItemActionData _actionData)
	{
		return EnumCameraShake.None;
	}

	// Token: 0x060029CC RID: 10700 RVA: 0x0010FF8C File Offset: 0x0010E18C
	public virtual TriggerEffectManager.ControllerTriggerEffect GetControllerTriggerEffectPull()
	{
		return TriggerEffectManager.NoneEffect;
	}

	// Token: 0x060029CD RID: 10701 RVA: 0x0010FF8C File Offset: 0x0010E18C
	public virtual TriggerEffectManager.ControllerTriggerEffect GetControllerTriggerEffectShoot()
	{
		return TriggerEffectManager.NoneEffect;
	}

	// Token: 0x060029CE RID: 10702 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool AllowItemLoopingSound(global::ItemActionData _actionData)
	{
		return true;
	}

	// Token: 0x060029CF RID: 10703 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsAimingGunPossible(global::ItemActionData _actionData)
	{
		return true;
	}

	// Token: 0x060029D0 RID: 10704 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void AimingSet(global::ItemActionData _actionData, bool _isAiming, bool _wasAiming)
	{
	}

	// Token: 0x060029D1 RID: 10705 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ItemActionEffects(GameManager _gameManager, global::ItemActionData _actionData, int _firingState, Vector3 _startPos, Vector3 _direction, int _userData = 0)
	{
	}

	// Token: 0x060029D2 RID: 10706 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void UpdateNozzleParticlesPosAndRot(global::ItemActionData _actionData)
	{
	}

	// Token: 0x060029D3 RID: 10707 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual int GetInitialMeta(ItemValue _itemValue)
	{
		return 0;
	}

	// Token: 0x060029D4 RID: 10708 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SwapAmmoType(EntityAlive _entity, int _ammoItemId = -1)
	{
	}

	// Token: 0x060029D5 RID: 10709 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool canShowOverlay(global::ItemActionData actionData)
	{
		return false;
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool isShowOverlay(global::ItemActionData actionData)
	{
		return false;
	}

	// Token: 0x060029D7 RID: 10711 RVA: 0x0010FF93 File Offset: 0x0010E193
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void getOverlayData(global::ItemActionData actionData, out float _perc, out string _text)
	{
		_perc = 0f;
		_text = "";
	}

	// Token: 0x060029D8 RID: 10712 RVA: 0x0010FFA4 File Offset: 0x0010E1A4
	public static float GetDismemberChance(global::ItemActionData _actionData, WorldRayHitInfo hitInfo)
	{
		FastTags<TagGroup.Global> a = _actionData.ActionTags;
		if (hitInfo.tag == "E_BP_Head")
		{
			a |= ItemAction.headTag;
		}
		else if (hitInfo.tag.ContainsCaseInsensitive("arm"))
		{
			a |= ItemAction.armTag;
		}
		else if (hitInfo.tag.ContainsCaseInsensitive("leg"))
		{
			a |= ItemAction.legTag;
		}
		return EffectManager.GetValue(PassiveEffects.DismemberChance, _actionData.invData.holdingEntity.inventory.holdingItemItemValue, 0f, _actionData.invData.holdingEntity, null, a | _actionData.invData.item.ItemTags, true, true, true, true, true, 1, true, false);
	}

	// Token: 0x060029D9 RID: 10713 RVA: 0x00110064 File Offset: 0x0010E264
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual List<string> getBuffActions(global::ItemActionData _actionData)
	{
		if (this.BuffActions == null)
		{
			return new List<string>();
		}
		return this.BuffActions;
	}

	// Token: 0x060029DA RID: 10714 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void GetItemValueActionInfo(ref List<string> _infoList, ItemValue _itemValue, XUi _xui, int _actionIndex = 0)
	{
	}

	// Token: 0x060029DB RID: 10715 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsEditingTool()
	{
		return false;
	}

	// Token: 0x060029DC RID: 10716 RVA: 0x00047178 File Offset: 0x00045378
	public virtual string GetStat(global::ItemActionData _data)
	{
		return string.Empty;
	}

	// Token: 0x060029DD RID: 10717 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsStatChanged()
	{
		return false;
	}

	// Token: 0x060029DE RID: 10718 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool HasRadial()
	{
		return false;
	}

	// Token: 0x060029DF RID: 10719 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetupRadial(XUiC_Radial _xuiRadialWindow, EntityPlayerLocal _epl)
	{
	}

	// Token: 0x060029E0 RID: 10720 RVA: 0x0011007A File Offset: 0x0010E27A
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string StringFormatHandler(string title, object value)
	{
		return string.Format("{0}: [REPLACE_COLOR]{1}[-]\n", title, value);
	}

	// Token: 0x060029E1 RID: 10721 RVA: 0x00110088 File Offset: 0x0010E288
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string BuffActionStrings(ItemAction itemAction, List<string> stringList)
	{
		if (itemAction.BuffActions == null || itemAction.BuffActions.Count == 0)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < itemAction.BuffActions.Count; i++)
		{
			BuffClass buff = BuffManager.GetBuff(itemAction.BuffActions[i]);
			if (buff != null && !string.IsNullOrEmpty(buff.Name))
			{
				stringList.Add(ItemAction.StringFormatHandler(Localization.Get("lblEffect", false), string.Format("{0}", buff.Name)));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060029E2 RID: 10722 RVA: 0x0011011A File Offset: 0x0010E31A
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string getColoredItemStat(string _title, float _value)
	{
		if (_value > 0f)
		{
			return string.Format("{0}: [00ff00]+{1}[-]", _title, _value.ToCultureInvariantString());
		}
		if (_value < 0f)
		{
			return string.Format("{0}: [ff0000]{1}[-]", _title, _value.ToCultureInvariantString());
		}
		return "";
	}

	// Token: 0x060029E3 RID: 10723 RVA: 0x00110158 File Offset: 0x0010E358
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string getColoredItemStatPercentage(string _title, float _value)
	{
		if (_value > 0f)
		{
			return string.Format("{0}: [00ff00]+{1}%[-]", _title, _value.ToCultureInvariantString("0.0"));
		}
		if (_value < 0f)
		{
			return string.Format("{0}: [ff0000]{1}%[-]", _title, _value.ToCultureInvariantString("0.0"));
		}
		return "";
	}

	// Token: 0x060029E4 RID: 10724 RVA: 0x001101A8 File Offset: 0x0010E3A8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleItemBreak(global::ItemActionData _actionData)
	{
		if (_actionData.invData.itemValue.MaxUseTimes > 0 && _actionData.invData.itemValue.UseTimes >= (float)_actionData.invData.itemValue.MaxUseTimes)
		{
			Manager.BroadcastPlay(_actionData.invData.holdingEntity, "itembreak", false);
		}
	}

	// Token: 0x060029E5 RID: 10725 RVA: 0x00110201 File Offset: 0x0010E401
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemAction()
	{
	}

	// Token: 0x04002073 RID: 8307
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> headTag = FastTags<TagGroup.Global>.Parse("head");

	// Token: 0x04002074 RID: 8308
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> armTag = FastTags<TagGroup.Global>.Parse("arm");

	// Token: 0x04002075 RID: 8309
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> legTag = FastTags<TagGroup.Global>.Parse("leg");

	// Token: 0x04002076 RID: 8310
	public static bool ShowDebugDisplayHit;

	// Token: 0x04002077 RID: 8311
	public static float DebugDisplayHitSize = 0.005f;

	// Token: 0x04002078 RID: 8312
	public static float DebugDisplayHitTime = 10f;

	// Token: 0x04002079 RID: 8313
	public static bool ShowDistanceDebugInfo = false;

	// Token: 0x0400207A RID: 8314
	public ItemClass item;

	// Token: 0x0400207B RID: 8315
	public List<string> BuffActions;

	// Token: 0x0400207C RID: 8316
	public new float Delay;

	// Token: 0x0400207D RID: 8317
	public new float Range;

	// Token: 0x0400207E RID: 8318
	public float SphereRadius;

	// Token: 0x0400207F RID: 8319
	public DynamicProperties Properties = new DynamicProperties();

	// Token: 0x04002080 RID: 8320
	public List<IRequirement> ExecutionRequirements;

	// Token: 0x04002081 RID: 8321
	public bool UseAnimation = true;

	// Token: 0x04002082 RID: 8322
	public int ActionIndex;

	// Token: 0x04002083 RID: 8323
	[PublicizedFrom(EAccessModifier.Protected)]
	public string soundStart;

	// Token: 0x04002084 RID: 8324
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool Sound_in_head;

	// Token: 0x04002085 RID: 8325
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bUseParticleHarvesting;

	// Token: 0x04002086 RID: 8326
	[PublicizedFrom(EAccessModifier.Protected)]
	public string particleHarvestingCategory;

	// Token: 0x04002087 RID: 8327
	public int ActionExp;

	// Token: 0x04002088 RID: 8328
	public float ActionExpBonusMultiplier;
}
