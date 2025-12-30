using System;
using System.Collections.Generic;

// Token: 0x02000588 RID: 1416
public class LootManager
{
	// Token: 0x06002DB5 RID: 11701 RVA: 0x00130EF2 File Offset: 0x0012F0F2
	public LootManager(WorldBase _world)
	{
		this.world = _world;
		this.Random = _world.GetGameRandom();
	}

	// Token: 0x06002DB6 RID: 11702 RVA: 0x00130F10 File Offset: 0x0012F110
	public void LootContainerOpened(ITileEntityLootable _tileEntity, int _entityIdThatOpenedIt, FastTags<TagGroup.Global> _containerTags)
	{
		if (this.world.IsEditor())
		{
			return;
		}
		if (_tileEntity.bTouched)
		{
			return;
		}
		_tileEntity.bTouched = true;
		_tileEntity.worldTimeTouched = this.world.GetWorldTime();
		LootContainer lootContainer = LootContainer.GetLootContainer(_tileEntity.lootListName, true);
		if (lootContainer == null)
		{
			return;
		}
		bool flag = _tileEntity.IsEmpty();
		_tileEntity.bTouched = true;
		_tileEntity.worldTimeTouched = this.world.GetWorldTime();
		if (!flag)
		{
			return;
		}
		EntityPlayer entityPlayer = (EntityPlayer)this.world.GetEntity(_entityIdThatOpenedIt);
		if (entityPlayer == null)
		{
			return;
		}
		entityPlayer.MinEventContext.TileEntity = _tileEntity;
		if (_tileEntity.EntityId == -1)
		{
			entityPlayer.MinEventContext.BlockValue = _tileEntity.blockValue;
		}
		if (entityPlayer.isEntityRemote)
		{
			if (_tileEntity.EntityId == -1)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageMinEventFire>().Setup(entityPlayer.entityId, -1, MinEventTypes.onSelfOpenLootContainer, _tileEntity.blockValue), false, entityPlayer.entityId, -1, -1, null, 192, false);
			}
		}
		else
		{
			entityPlayer.FireEvent(MinEventTypes.onSelfOpenLootContainer, true);
		}
		float containerMod = 0f;
		float containerBonus = 0f;
		if (_tileEntity.EntityId == -1)
		{
			containerMod = _tileEntity.LootStageMod;
			containerBonus = _tileEntity.LootStageBonus;
		}
		int num = lootContainer.useUnmodifiedLootstage ? entityPlayer.unModifiedGameStage : entityPlayer.GetHighestPartyLootStage(containerMod, containerBonus);
		IList<ItemStack> list = lootContainer.Spawn(this.Random, _tileEntity.items.Length, (float)num, 0f, entityPlayer, _containerTags, lootContainer.UniqueItems, lootContainer.IgnoreLootProb);
		for (int i = 0; i < list.Count; i++)
		{
			_tileEntity.items[i] = list[i].Clone();
		}
		entityPlayer.FireEvent(MinEventTypes.onSelfLootContainer, true);
	}

	// Token: 0x06002DB7 RID: 11703 RVA: 0x001310B8 File Offset: 0x0012F2B8
	public void LootContainerOpenedClient(ITileEntityLootable _tileEntity, int _entityIdThatOpenedIt, FastTags<TagGroup.Global> _containerTags)
	{
		if (this.world.IsEditor())
		{
			return;
		}
		if (_tileEntity.bTouched)
		{
			return;
		}
		EntityPlayer entityPlayer = (EntityPlayer)this.world.GetEntity(_entityIdThatOpenedIt);
		if (entityPlayer == null)
		{
			return;
		}
		entityPlayer.MinEventContext.TileEntity = _tileEntity;
		if (_tileEntity.EntityId == -1)
		{
			entityPlayer.MinEventContext.BlockValue = _tileEntity.blockValue;
		}
		entityPlayer.FireEvent(MinEventTypes.onSelfOpenLootContainer, true);
	}

	// Token: 0x04002454 RID: 9300
	public GameRandom Random;

	// Token: 0x04002455 RID: 9301
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldBase world;

	// Token: 0x04002456 RID: 9302
	public static float[] POITierMod;

	// Token: 0x04002457 RID: 9303
	public static float[] POITierBonus;
}
