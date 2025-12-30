using System;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000552 RID: 1362
[Preserve]
public class ItemActionUseOther : ItemAction
{
	// Token: 0x06002BFA RID: 11258 RVA: 0x001256EE File Offset: 0x001238EE
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionUseOther.FeedInventoryData(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002BFB RID: 11259 RVA: 0x001256F8 File Offset: 0x001238F8
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		if (_props.Values.ContainsKey("Consume"))
		{
			this.Consume = StringParsers.ParseBool(_props.Values["Consume"], 0, -1, true);
		}
		else
		{
			this.Consume = true;
		}
		if (!_props.Values.ContainsKey("Create_item"))
		{
			this.CreateItem = null;
			this.CreateItemCount = 0;
			return;
		}
		this.CreateItem = _props.Values["Create_item"];
		if (_props.Values.ContainsKey("Create_item_count"))
		{
			this.CreateItemCount = int.Parse(_props.Values["Create_item_count"]);
			return;
		}
		this.CreateItemCount = 1;
	}

	// Token: 0x06002BFC RID: 11260 RVA: 0x001257B4 File Offset: 0x001239B4
	public override void StopHolding(ItemActionData _data)
	{
		base.StopHolding(_data);
		ItemActionUseOther.FeedInventoryData feedInventoryData = (ItemActionUseOther.FeedInventoryData)_data;
		feedInventoryData.bFeedingStarted = false;
		feedInventoryData.TargetEntity = null;
		if (_data.invData.holdingEntity is EntityPlayerLocal)
		{
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_data.invData.holdingEntity as EntityPlayerLocal);
			NGUIWindowManager nguiWindowManager = uiforPlayer.nguiWindowManager;
			XUiC_FocusedBlockHealth.SetData(uiforPlayer, null, 0f);
		}
	}

	// Token: 0x06002BFD RID: 11261 RVA: 0x00125814 File Offset: 0x00123A14
	public override bool CanExecute(ItemActionData _actionData)
	{
		if (Time.time - _actionData.lastUseTime < this.Delay)
		{
			return false;
		}
		ItemActionUseOther.FeedInventoryData feedInventoryData = (ItemActionUseOther.FeedInventoryData)_actionData;
		EntityAlive holdingEntity = feedInventoryData.invData.holdingEntity;
		int modelLayer = holdingEntity.GetModelLayer();
		holdingEntity.SetModelLayer(2, false, null);
		float distance = 4f;
		feedInventoryData.ray = holdingEntity.GetLookRay();
		EntityAlive entityAlive = null;
		if (Voxel.Raycast(feedInventoryData.invData.world, feedInventoryData.ray, distance, -538750981, 128, this.SphereRadius))
		{
			entityAlive = (ItemActionUseOther.GetEntityFromHit(Voxel.voxelRayHitInfo) as EntityAlive);
		}
		if (entityAlive == null || !entityAlive.IsAlive() || !(entityAlive is EntityPlayer))
		{
			Voxel.Raycast(feedInventoryData.invData.world, feedInventoryData.ray, distance, -538488837, 128, this.SphereRadius);
		}
		holdingEntity.SetModelLayer(modelLayer, false, null);
		if (feedInventoryData.TargetEntity == null)
		{
			feedInventoryData.TargetEntity = entityAlive;
		}
		_actionData.invData.holdingEntity.MinEventContext.Other = feedInventoryData.TargetEntity;
		_actionData.invData.holdingEntity.MinEventContext.ItemValue = _actionData.invData.itemValue;
		return base.CanExecute(_actionData);
	}

	// Token: 0x06002BFE RID: 11262 RVA: 0x00125950 File Offset: 0x00123B50
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		if (!_bReleased)
		{
			return;
		}
		EntityAlive holdingEntity = _actionData.invData.holdingEntity;
		if (EffectManager.GetValue(PassiveEffects.DisableItem, holdingEntity.inventory.holdingItemItemValue, 0f, holdingEntity, null, _actionData.invData.item.ItemTags, true, true, true, true, true, 1, true, false) > 0f)
		{
			_actionData.lastUseTime = Time.time + 1f;
			Manager.PlayInsidePlayerHead("twitch_no_attack", -1, 0f, false, false);
			return;
		}
		ItemActionUseOther.FeedInventoryData feedInventoryData = (ItemActionUseOther.FeedInventoryData)_actionData;
		_actionData.lastUseTime = Time.time;
		feedInventoryData.bFeedingStarted = true;
		if (feedInventoryData.TargetEntity == null)
		{
			return;
		}
		if (feedInventoryData.invData.item.HasAnyTags(this.medicalItemTag) && feedInventoryData.TargetEntity as EntityPlayer == null)
		{
			feedInventoryData.bFeedingStarted = false;
			feedInventoryData.TargetEntity = null;
			return;
		}
		if (feedInventoryData.invData.item.HasAnyTags(this.medicalItemTag) && feedInventoryData.TargetEntity.HasAnyTags(this.noMedBuffsTag))
		{
			feedInventoryData.bFeedingStarted = false;
			feedInventoryData.TargetEntity = null;
			return;
		}
		holdingEntity.RightArmAnimationUse = true;
		if (this.soundStart != null)
		{
			holdingEntity.PlayOneShot(this.soundStart, false, false, false, null);
		}
		holdingEntity.MinEventContext.Other = feedInventoryData.TargetEntity;
		holdingEntity.MinEventContext.ItemValue = _actionData.invData.itemValue;
		holdingEntity.FireEvent(MinEventTypes.onSelfHealedOther, true);
		holdingEntity.FireEvent((_actionData.indexInEntityOfAction == 0) ? MinEventTypes.onSelfPrimaryActionEnd : MinEventTypes.onSelfSecondaryActionEnd, true);
		if (_actionData.invData.itemValue.ItemClass.HasAnyTags(this.stopBleed) && feedInventoryData.TargetEntity.entityType == EntityType.Player && feedInventoryData.TargetEntity.Buffs.HasBuff("buffInjuryBleeding"))
		{
			IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
			if (achievementManager != null)
			{
				achievementManager.SetAchievementStat(EnumAchievementDataStat.BleedOutStopped, 1);
			}
		}
		ItemAction.ExecuteBuffActions(this.getBuffActions(_actionData), feedInventoryData.TargetEntity.entityId, feedInventoryData.TargetEntity, false, EnumBodyPartHit.None, null);
		EntityPlayer entityPlayer = holdingEntity as EntityPlayer;
		if (this.Consume)
		{
			if (_actionData.invData.itemValue.MaxUseTimes > 0 && _actionData.invData.itemValue.UseTimes + 1f < (float)_actionData.invData.itemValue.MaxUseTimes)
			{
				ItemValue itemValue = _actionData.invData.itemValue;
				itemValue.UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, feedInventoryData.invData.itemValue, 1f, holdingEntity, null, _actionData.invData.itemValue.ItemClass.ItemTags, true, true, true, true, true, 1, true, false);
				feedInventoryData.invData.itemValue = itemValue;
				return;
			}
			holdingEntity.inventory.DecHoldingItem(1);
		}
		if (this.CreateItem != null && this.CreateItemCount > 0)
		{
			ItemStack itemStack = new ItemStack(ItemClass.GetItem(this.CreateItem, false), this.CreateItemCount);
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayer as EntityPlayerLocal);
			if (null != uiforPlayer && !uiforPlayer.xui.PlayerInventory.AddItem(itemStack))
			{
				holdingEntity.world.gameManager.ItemDropServer(itemStack, holdingEntity.GetPosition(), Vector3.zero, -1, 60f, false);
			}
		}
		feedInventoryData.bFeedingStarted = false;
		feedInventoryData.TargetEntity = null;
	}

	// Token: 0x06002BFF RID: 11263 RVA: 0x00125C88 File Offset: 0x00123E88
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		ItemActionUseOther.FeedInventoryData feedInventoryData = (ItemActionUseOther.FeedInventoryData)_actionData;
		return feedInventoryData.bFeedingStarted && Time.time - feedInventoryData.lastUseTime < this.Delay;
	}

	// Token: 0x06002C00 RID: 11264 RVA: 0x00125CBB File Offset: 0x00123EBB
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		ItemActionUseOther.FeedInventoryData feedInventoryData = (ItemActionUseOther.FeedInventoryData)_actionData;
	}

	// Token: 0x06002C01 RID: 11265 RVA: 0x00125CC4 File Offset: 0x00123EC4
	public static Entity GetEntityFromHit(WorldRayHitInfo hitInfo)
	{
		Transform hitRootTransform = GameUtils.GetHitRootTransform(hitInfo.tag, hitInfo.transform);
		if (hitRootTransform != null)
		{
			return hitRootTransform.GetComponent<Entity>();
		}
		return null;
	}

	// Token: 0x06002C02 RID: 11266 RVA: 0x00125CF4 File Offset: 0x00123EF4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool canShowOverlay(ItemActionData _actionData)
	{
		return this.isValidEntityToHeal((ItemActionUseOther.FeedInventoryData)_actionData);
	}

	// Token: 0x06002C03 RID: 11267 RVA: 0x00125D02 File Offset: 0x00123F02
	public override ItemClass.EnumCrosshairType GetCrosshairType(ItemActionData _actionData)
	{
		if (this.isValidEntityToHeal((ItemActionUseOther.FeedInventoryData)_actionData))
		{
			return ItemClass.EnumCrosshairType.Heal;
		}
		return ItemClass.EnumCrosshairType.Plus;
	}

	// Token: 0x06002C04 RID: 11268 RVA: 0x00125CF4 File Offset: 0x00123EF4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isShowOverlay(ItemActionData _actionData)
	{
		return this.isValidEntityToHeal((ItemActionUseOther.FeedInventoryData)_actionData);
	}

	// Token: 0x06002C05 RID: 11269 RVA: 0x00125D18 File Offset: 0x00123F18
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void getOverlayData(ItemActionData _actionData, out float _perc, out string _text)
	{
		ItemActionUseOther.FeedInventoryData feedInventoryData = (ItemActionUseOther.FeedInventoryData)_actionData;
		if (!this.isValidEntityToHeal(feedInventoryData))
		{
			base.getOverlayData(_actionData, out _perc, out _text);
			return;
		}
		_perc = feedInventoryData.TargetEntity.Stats.Health.Value / feedInventoryData.TargetEntity.Stats.Health.Max;
		_text = string.Format("{0}/{1}", feedInventoryData.TargetEntity.Stats.Health.Value.ToCultureInvariantString(), feedInventoryData.TargetEntity.Stats.Health.Max.ToCultureInvariantString());
	}

	// Token: 0x06002C06 RID: 11270 RVA: 0x00125DAC File Offset: 0x00123FAC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isValidEntityToHeal(ItemActionUseOther.FeedInventoryData _actionData)
	{
		return _actionData.TargetEntity != null;
	}

	// Token: 0x06002C07 RID: 11271 RVA: 0x00125DBC File Offset: 0x00123FBC
	public override void OnHUD(ItemActionData _actionData, int _x, int _y)
	{
		ItemActionUseOther.FeedInventoryData feedInventoryData = (ItemActionUseOther.FeedInventoryData)_actionData;
		if (feedInventoryData == null)
		{
			return;
		}
		if (!this.canShowOverlay(feedInventoryData))
		{
			XUiC_FocusedBlockHealth.SetData(LocalPlayerUI.GetUIForPrimaryPlayer(), null, 0f);
			return;
		}
		if (!(feedInventoryData.invData.holdingEntity is EntityPlayerLocal))
		{
			return;
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer((EntityPlayerLocal)feedInventoryData.invData.holdingEntity);
		if (!this.isShowOverlay(feedInventoryData))
		{
			if (feedInventoryData.uiOpenedByMe && XUiC_FocusedBlockHealth.IsWindowOpen(uiforPlayer))
			{
				XUiC_FocusedBlockHealth.SetData(uiforPlayer, null, 0f);
				feedInventoryData.uiOpenedByMe = false;
				return;
			}
		}
		else
		{
			if (!XUiC_FocusedBlockHealth.IsWindowOpen(uiforPlayer))
			{
				feedInventoryData.uiOpenedByMe = true;
			}
			float fill;
			string text;
			this.getOverlayData(feedInventoryData, out fill, out text);
			XUiC_FocusedBlockHealth.SetData(uiforPlayer, text, fill);
		}
	}

	// Token: 0x04002248 RID: 8776
	public new string CreateItem;

	// Token: 0x04002249 RID: 8777
	public int CreateItemCount;

	// Token: 0x0400224A RID: 8778
	public new bool Consume;

	// Token: 0x0400224B RID: 8779
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> noMedBuffsTag = FastTags<TagGroup.Global>.Parse("noMedBuffs");

	// Token: 0x0400224C RID: 8780
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> medicalItemTag = FastTags<TagGroup.Global>.Parse("medical");

	// Token: 0x0400224D RID: 8781
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> stopBleed = FastTags<TagGroup.Global>.Parse("stopsBleeding");

	// Token: 0x02000553 RID: 1363
	public class FeedInventoryData : ItemActionAttackData
	{
		// Token: 0x06002C09 RID: 11273 RVA: 0x00112618 File Offset: 0x00110818
		public FeedInventoryData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x0400224E RID: 8782
		public bool bFeedingStarted;

		// Token: 0x0400224F RID: 8783
		public EntityAlive TargetEntity;

		// Token: 0x04002250 RID: 8784
		public Ray ray;
	}
}
