using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Challenges;
using Quests;
using UnityEngine;

// Token: 0x02000905 RID: 2309
public class QuestEventManager
{
	// Token: 0x1700072F RID: 1839
	// (get) Token: 0x0600442C RID: 17452 RVA: 0x001B5F9D File Offset: 0x001B419D
	public static QuestEventManager Current
	{
		get
		{
			if (QuestEventManager.instance == null)
			{
				QuestEventManager.instance = new QuestEventManager();
			}
			return QuestEventManager.instance;
		}
	}

	// Token: 0x17000730 RID: 1840
	// (get) Token: 0x0600442D RID: 17453 RVA: 0x001B5FB5 File Offset: 0x001B41B5
	public static bool HasInstance
	{
		get
		{
			return QuestEventManager.instance != null;
		}
	}

	// Token: 0x0600442E RID: 17454 RVA: 0x001B5FC0 File Offset: 0x001B41C0
	public void SetupTraderPrefabList(TraderArea area)
	{
		if (!this.TraderPrefabList.ContainsKey(area))
		{
			Vector3 a = area.Position.ToVector3();
			List<PrefabInstance> poiprefabs = GameManager.Instance.GetDynamicPrefabDecorator().GetPOIPrefabs();
			List<QuestEventManager.PrefabListData> list = new List<QuestEventManager.PrefabListData>();
			QuestEventManager.PrefabListData prefabListData = new QuestEventManager.PrefabListData();
			QuestEventManager.PrefabListData prefabListData2 = new QuestEventManager.PrefabListData();
			QuestEventManager.PrefabListData prefabListData3 = new QuestEventManager.PrefabListData();
			list.Add(prefabListData);
			list.Add(prefabListData2);
			list.Add(prefabListData3);
			for (int i = 0; i < poiprefabs.Count; i++)
			{
				float num = Vector3.Distance(a, poiprefabs[i].boundingBoxPosition);
				if (num <= 500f)
				{
					prefabListData.AddPOI(poiprefabs[i]);
				}
				else if (num <= 1500f)
				{
					prefabListData2.AddPOI(poiprefabs[i]);
				}
				else
				{
					prefabListData3.AddPOI(poiprefabs[i]);
				}
			}
			this.TraderPrefabList.Add(area, list);
		}
	}

	// Token: 0x0600442F RID: 17455 RVA: 0x001B60AC File Offset: 0x001B42AC
	public List<PrefabInstance> GetPrefabsForTrader(TraderArea traderArea, int difficulty, int index, GameRandom gameRandom)
	{
		if (traderArea == null)
		{
			return null;
		}
		if (!this.TraderPrefabList.ContainsKey(traderArea))
		{
			this.SetupTraderPrefabList(traderArea);
		}
		QuestEventManager.PrefabListData prefabListData = this.TraderPrefabList[traderArea][index];
		prefabListData.ShuffleDifficulty(difficulty, gameRandom);
		if (prefabListData.TierData.ContainsKey(difficulty))
		{
			return prefabListData.TierData[difficulty];
		}
		return null;
	}

	// Token: 0x06004430 RID: 17456 RVA: 0x001B610C File Offset: 0x001B430C
	public List<PrefabInstance> GetPrefabsByDifficultyTier(int difficulty)
	{
		if (this.tierPrefabList.Count == 0)
		{
			List<PrefabInstance> poiprefabs = GameManager.Instance.GetDynamicPrefabDecorator().GetPOIPrefabs();
			for (int i = 0; i < poiprefabs.Count; i++)
			{
				if (!this.tierPrefabList.ContainsKey((int)poiprefabs[i].prefab.DifficultyTier))
				{
					this.tierPrefabList.Add((int)poiprefabs[i].prefab.DifficultyTier, new List<PrefabInstance>());
				}
				this.tierPrefabList[(int)poiprefabs[i].prefab.DifficultyTier].Add(poiprefabs[i]);
			}
		}
		if (this.tierPrefabList.ContainsKey(difficulty))
		{
			return this.tierPrefabList[difficulty];
		}
		return null;
	}

	// Token: 0x06004431 RID: 17457 RVA: 0x001B61D0 File Offset: 0x001B43D0
	[PublicizedFrom(EAccessModifier.Private)]
	public QuestEventManager()
	{
	}

	// Token: 0x14000051 RID: 81
	// (add) Token: 0x06004432 RID: 17458 RVA: 0x001B6280 File Offset: 0x001B4480
	// (remove) Token: 0x06004433 RID: 17459 RVA: 0x001B62B8 File Offset: 0x001B44B8
	public event QuestEvent_BlockEvent BlockActivate;

	// Token: 0x06004434 RID: 17460 RVA: 0x001B62ED File Offset: 0x001B44ED
	public void BlockActivated(string blockName, Vector3i blockPos)
	{
		if (this.BlockActivate != null)
		{
			this.BlockActivate(blockName, blockPos);
		}
	}

	// Token: 0x14000052 RID: 82
	// (add) Token: 0x06004435 RID: 17461 RVA: 0x001B6304 File Offset: 0x001B4504
	// (remove) Token: 0x06004436 RID: 17462 RVA: 0x001B633C File Offset: 0x001B453C
	public event QuestEvent_BlockChangedEvent BlockChange;

	// Token: 0x06004437 RID: 17463 RVA: 0x001B6371 File Offset: 0x001B4571
	public void BlockChanged(Block blockOld, Block blockNew, Vector3i blockPos)
	{
		if (this.BlockChange != null)
		{
			this.BlockChange(blockOld, blockNew, blockPos);
		}
	}

	// Token: 0x14000053 RID: 83
	// (add) Token: 0x06004438 RID: 17464 RVA: 0x001B638C File Offset: 0x001B458C
	// (remove) Token: 0x06004439 RID: 17465 RVA: 0x001B63C4 File Offset: 0x001B45C4
	public event QuestEvent_BlockDestroyEvent BlockDestroy;

	// Token: 0x0600443A RID: 17466 RVA: 0x001B63FC File Offset: 0x001B45FC
	public void BlockDestroyed(Block block, Vector3i blockPos, Entity byEntity = null)
	{
		if (this.BlockDestroy != null)
		{
			this.BlockDestroy(block, blockPos);
		}
		if (block.AllowBlockTriggers && byEntity)
		{
			EntityPlayer entityPlayer = byEntity as EntityPlayer;
			if (!entityPlayer)
			{
				entityPlayer = byEntity.world.GetClosestPlayer(byEntity, 500f, false);
			}
			if (entityPlayer)
			{
				BlockValue blockValue = default(BlockValue);
				blockValue.type = block.blockID;
				block.HandleTrigger(entityPlayer, entityPlayer.world, 0, blockPos, blockValue);
			}
		}
	}

	// Token: 0x14000054 RID: 84
	// (add) Token: 0x0600443B RID: 17467 RVA: 0x001B6480 File Offset: 0x001B4680
	// (remove) Token: 0x0600443C RID: 17468 RVA: 0x001B64B8 File Offset: 0x001B46B8
	public event QuestEvent_BlockEvent BlockPickup;

	// Token: 0x0600443D RID: 17469 RVA: 0x001B64ED File Offset: 0x001B46ED
	public void BlockPickedUp(string blockName, Vector3i blockPos)
	{
		if (this.BlockPickup != null)
		{
			this.BlockPickup(blockName, blockPos);
		}
	}

	// Token: 0x14000055 RID: 85
	// (add) Token: 0x0600443E RID: 17470 RVA: 0x001B6504 File Offset: 0x001B4704
	// (remove) Token: 0x0600443F RID: 17471 RVA: 0x001B653C File Offset: 0x001B473C
	public event QuestEvent_BlockEvent BlockPlace;

	// Token: 0x06004440 RID: 17472 RVA: 0x001B6571 File Offset: 0x001B4771
	public void BlockPlaced(string blockName, Vector3i blockPos)
	{
		if (this.BlockPlace != null)
		{
			this.BlockPlace(blockName, blockPos);
		}
	}

	// Token: 0x14000056 RID: 86
	// (add) Token: 0x06004441 RID: 17473 RVA: 0x001B6588 File Offset: 0x001B4788
	// (remove) Token: 0x06004442 RID: 17474 RVA: 0x001B65C0 File Offset: 0x001B47C0
	public event QuestEvent_BlockEvent BlockUpgrade;

	// Token: 0x06004443 RID: 17475 RVA: 0x001B65F5 File Offset: 0x001B47F5
	public void BlockUpgraded(string blockName, Vector3i blockPos)
	{
		if (this.BlockUpgrade != null)
		{
			this.BlockUpgrade(blockName, blockPos);
		}
	}

	// Token: 0x14000057 RID: 87
	// (add) Token: 0x06004444 RID: 17476 RVA: 0x001B660C File Offset: 0x001B480C
	// (remove) Token: 0x06004445 RID: 17477 RVA: 0x001B6644 File Offset: 0x001B4844
	public event QuestEvent_ItemStackActionEvent AddItem;

	// Token: 0x06004446 RID: 17478 RVA: 0x001B6679 File Offset: 0x001B4879
	public void ItemAdded(ItemStack newStack)
	{
		if (this.AddItem != null)
		{
			this.AddItem(newStack);
		}
	}

	// Token: 0x14000058 RID: 88
	// (add) Token: 0x06004447 RID: 17479 RVA: 0x001B6690 File Offset: 0x001B4890
	// (remove) Token: 0x06004448 RID: 17480 RVA: 0x001B66C8 File Offset: 0x001B48C8
	public event QuestEvent_HarvestStackActionEvent HarvestItem;

	// Token: 0x06004449 RID: 17481 RVA: 0x001B66FD File Offset: 0x001B48FD
	public void HarvestedItem(ItemValue heldItem, ItemStack newStack, BlockValue bv)
	{
		if (this.HarvestItem != null)
		{
			this.HarvestItem(heldItem, newStack, bv);
		}
	}

	// Token: 0x14000059 RID: 89
	// (add) Token: 0x0600444A RID: 17482 RVA: 0x001B6718 File Offset: 0x001B4918
	// (remove) Token: 0x0600444B RID: 17483 RVA: 0x001B6750 File Offset: 0x001B4950
	public event QuestEvent_ItemStackActionEvent AssembleItem;

	// Token: 0x0600444C RID: 17484 RVA: 0x001B6785 File Offset: 0x001B4985
	public void AssembledItem(ItemStack newStack)
	{
		if (this.AssembleItem != null)
		{
			this.AssembleItem(newStack);
		}
	}

	// Token: 0x1400005A RID: 90
	// (add) Token: 0x0600444D RID: 17485 RVA: 0x001B679C File Offset: 0x001B499C
	// (remove) Token: 0x0600444E RID: 17486 RVA: 0x001B67D4 File Offset: 0x001B49D4
	public event QuestEvent_ItemStackActionEvent CraftItem;

	// Token: 0x0600444F RID: 17487 RVA: 0x001B6809 File Offset: 0x001B4A09
	public void CraftedItem(ItemStack newStack)
	{
		if (this.CraftItem != null)
		{
			this.CraftItem(newStack);
		}
	}

	// Token: 0x1400005B RID: 91
	// (add) Token: 0x06004450 RID: 17488 RVA: 0x001B6820 File Offset: 0x001B4A20
	// (remove) Token: 0x06004451 RID: 17489 RVA: 0x001B6858 File Offset: 0x001B4A58
	public event QuestEvent_ItemStackActionEvent ExchangeFromItem;

	// Token: 0x06004452 RID: 17490 RVA: 0x001B688D File Offset: 0x001B4A8D
	public void ExchangedFromItem(ItemStack newStack)
	{
		if (this.ExchangeFromItem != null)
		{
			this.ExchangeFromItem(newStack);
		}
	}

	// Token: 0x1400005C RID: 92
	// (add) Token: 0x06004453 RID: 17491 RVA: 0x001B68A4 File Offset: 0x001B4AA4
	// (remove) Token: 0x06004454 RID: 17492 RVA: 0x001B68DC File Offset: 0x001B4ADC
	public event QuestEvent_ItemStackActionEvent ScrapItem;

	// Token: 0x06004455 RID: 17493 RVA: 0x001B6911 File Offset: 0x001B4B11
	public void ScrappedItem(ItemStack newStack)
	{
		if (this.ScrapItem != null)
		{
			this.ScrapItem(newStack);
		}
	}

	// Token: 0x1400005D RID: 93
	// (add) Token: 0x06004456 RID: 17494 RVA: 0x001B6928 File Offset: 0x001B4B28
	// (remove) Token: 0x06004457 RID: 17495 RVA: 0x001B6960 File Offset: 0x001B4B60
	public event QuestEvent_ItemValueActionEvent RepairItem;

	// Token: 0x06004458 RID: 17496 RVA: 0x001B6995 File Offset: 0x001B4B95
	public void RepairedItem(ItemValue newValue)
	{
		if (this.RepairItem != null)
		{
			this.RepairItem(newValue);
		}
	}

	// Token: 0x1400005E RID: 94
	// (add) Token: 0x06004459 RID: 17497 RVA: 0x001B69AC File Offset: 0x001B4BAC
	// (remove) Token: 0x0600445A RID: 17498 RVA: 0x001B69E4 File Offset: 0x001B4BE4
	public event QuestEvent_SkillPointSpent SkillPointSpent;

	// Token: 0x1400005F RID: 95
	// (add) Token: 0x0600445B RID: 17499 RVA: 0x001B6A1C File Offset: 0x001B4C1C
	// (remove) Token: 0x0600445C RID: 17500 RVA: 0x001B6A54 File Offset: 0x001B4C54
	public event QuestEvent_ItemValueActionEvent HoldItem;

	// Token: 0x0600445D RID: 17501 RVA: 0x001B6A89 File Offset: 0x001B4C89
	public void HeldItem(ItemValue newValue)
	{
		if (this.HoldItem != null)
		{
			this.HoldItem(newValue);
		}
	}

	// Token: 0x14000060 RID: 96
	// (add) Token: 0x0600445E RID: 17502 RVA: 0x001B6AA0 File Offset: 0x001B4CA0
	// (remove) Token: 0x0600445F RID: 17503 RVA: 0x001B6AD8 File Offset: 0x001B4CD8
	public event QuestEvent_ItemValueActionEvent WearItem;

	// Token: 0x06004460 RID: 17504 RVA: 0x001B6B0D File Offset: 0x001B4D0D
	public void WoreItem(ItemValue newValue)
	{
		if (this.WearItem != null)
		{
			this.WearItem(newValue);
		}
	}

	// Token: 0x06004461 RID: 17505 RVA: 0x001B6B23 File Offset: 0x001B4D23
	public void SpendSkillPoint(ProgressionValue skill)
	{
		if (this.SkillPointSpent != null)
		{
			this.SkillPointSpent(skill.ProgressionClass.Name);
		}
	}

	// Token: 0x14000061 RID: 97
	// (add) Token: 0x06004462 RID: 17506 RVA: 0x001B6B44 File Offset: 0x001B4D44
	// (remove) Token: 0x06004463 RID: 17507 RVA: 0x001B6B7C File Offset: 0x001B4D7C
	public event QuestEvent_WindowChanged WindowChanged;

	// Token: 0x06004464 RID: 17508 RVA: 0x001B6BB1 File Offset: 0x001B4DB1
	public void ChangedWindow(string windowName)
	{
		if (this.WindowChanged != null)
		{
			this.WindowChanged(windowName);
		}
	}

	// Token: 0x14000062 RID: 98
	// (add) Token: 0x06004465 RID: 17509 RVA: 0x001B6BC8 File Offset: 0x001B4DC8
	// (remove) Token: 0x06004466 RID: 17510 RVA: 0x001B6C00 File Offset: 0x001B4E00
	public event QuestEvent_OpenContainer ContainerOpened;

	// Token: 0x06004467 RID: 17511 RVA: 0x001B6C35 File Offset: 0x001B4E35
	public void OpenedContainer(int entityId, Vector3i containerLocation, ITileEntityLootable tileEntity)
	{
		if (this.ContainerOpened != null)
		{
			this.ContainerOpened(entityId, containerLocation, tileEntity);
		}
	}

	// Token: 0x14000063 RID: 99
	// (add) Token: 0x06004468 RID: 17512 RVA: 0x001B6C50 File Offset: 0x001B4E50
	// (remove) Token: 0x06004469 RID: 17513 RVA: 0x001B6C88 File Offset: 0x001B4E88
	public event QuestEvent_OpenContainer ContainerClosed;

	// Token: 0x0600446A RID: 17514 RVA: 0x001B6CBD File Offset: 0x001B4EBD
	public void ClosedContainer(int entityId, Vector3i containerLocation, ITileEntityLootable tileEntity)
	{
		if (this.ContainerClosed != null)
		{
			this.ContainerClosed(entityId, containerLocation, tileEntity);
		}
	}

	// Token: 0x14000064 RID: 100
	// (add) Token: 0x0600446B RID: 17515 RVA: 0x001B6CD8 File Offset: 0x001B4ED8
	// (remove) Token: 0x0600446C RID: 17516 RVA: 0x001B6D10 File Offset: 0x001B4F10
	public event QuestEvent_EntityKillEvent EntityKill;

	// Token: 0x0600446D RID: 17517 RVA: 0x001B6D45 File Offset: 0x001B4F45
	public void EntityKilled(EntityAlive killedBy, EntityAlive killedEntity)
	{
		if (this.EntityKill != null && killedBy != null && killedEntity != null)
		{
			this.EntityKill(killedBy, killedEntity);
		}
	}

	// Token: 0x14000065 RID: 101
	// (add) Token: 0x0600446E RID: 17518 RVA: 0x001B6D70 File Offset: 0x001B4F70
	// (remove) Token: 0x0600446F RID: 17519 RVA: 0x001B6DA8 File Offset: 0x001B4FA8
	public event QuestEvent_NPCInteracted NPCInteract;

	// Token: 0x06004470 RID: 17520 RVA: 0x001B6DDD File Offset: 0x001B4FDD
	public void NPCInteracted(EntityNPC entityNPC)
	{
		if (this.NPCInteract != null)
		{
			this.NPCInteract(entityNPC);
		}
	}

	// Token: 0x14000066 RID: 102
	// (add) Token: 0x06004471 RID: 17521 RVA: 0x001B6DF4 File Offset: 0x001B4FF4
	// (remove) Token: 0x06004472 RID: 17522 RVA: 0x001B6E2C File Offset: 0x001B502C
	public event QuestEvent_NPCInteracted NPCMeet;

	// Token: 0x06004473 RID: 17523 RVA: 0x001B6E61 File Offset: 0x001B5061
	public void NPCMet(EntityNPC entityNPC)
	{
		if (this.NPCMeet != null)
		{
			this.NPCMeet(entityNPC);
		}
	}

	// Token: 0x14000067 RID: 103
	// (add) Token: 0x06004474 RID: 17524 RVA: 0x001B6E78 File Offset: 0x001B5078
	// (remove) Token: 0x06004475 RID: 17525 RVA: 0x001B6EB0 File Offset: 0x001B50B0
	public event QuestEvent_SleepersCleared SleepersCleared;

	// Token: 0x06004476 RID: 17526 RVA: 0x001B6EE5 File Offset: 0x001B50E5
	public void ClearedSleepers(Vector3 prefabPos)
	{
		if (this.SleepersCleared != null)
		{
			this.SleepersCleared(prefabPos);
		}
	}

	// Token: 0x14000068 RID: 104
	// (add) Token: 0x06004477 RID: 17527 RVA: 0x001B6EFC File Offset: 0x001B50FC
	// (remove) Token: 0x06004478 RID: 17528 RVA: 0x001B6F34 File Offset: 0x001B5134
	public event QuestEvent_Explosion ExplosionDetected;

	// Token: 0x06004479 RID: 17529 RVA: 0x001B6F69 File Offset: 0x001B5169
	public void DetectedExplosion(Vector3 explosionPos, int entityID, float blockDamage)
	{
		if (this.ExplosionDetected != null)
		{
			this.ExplosionDetected(explosionPos, entityID, blockDamage);
		}
	}

	// Token: 0x14000069 RID: 105
	// (add) Token: 0x0600447A RID: 17530 RVA: 0x001B6F84 File Offset: 0x001B5184
	// (remove) Token: 0x0600447B RID: 17531 RVA: 0x001B6FBC File Offset: 0x001B51BC
	public event QuestEvent_PurchaseEvent BuyItems;

	// Token: 0x1400006A RID: 106
	// (add) Token: 0x0600447C RID: 17532 RVA: 0x001B6FF4 File Offset: 0x001B51F4
	// (remove) Token: 0x0600447D RID: 17533 RVA: 0x001B702C File Offset: 0x001B522C
	public event QuestEvent_PurchaseEvent SellItems;

	// Token: 0x0600447E RID: 17534 RVA: 0x001B7061 File Offset: 0x001B5261
	public void BoughtItems(string traderName, int itemCount)
	{
		if (this.BuyItems != null)
		{
			this.BuyItems(traderName, itemCount);
		}
	}

	// Token: 0x0600447F RID: 17535 RVA: 0x001B7078 File Offset: 0x001B5278
	public void SoldItems(string traderName, int itemCount)
	{
		if (this.SellItems != null)
		{
			this.SellItems(traderName, itemCount);
		}
	}

	// Token: 0x1400006B RID: 107
	// (add) Token: 0x06004480 RID: 17536 RVA: 0x001B7090 File Offset: 0x001B5290
	// (remove) Token: 0x06004481 RID: 17537 RVA: 0x001B70C8 File Offset: 0x001B52C8
	public event QuestEvent_ChallengeCompleteEvent ChallengeComplete;

	// Token: 0x06004482 RID: 17538 RVA: 0x001B70FD File Offset: 0x001B52FD
	public void ChallengeCompleted(ChallengeClass challenge, bool isRedeemed)
	{
		if (this.ChallengeComplete != null)
		{
			this.ChallengeComplete(challenge, isRedeemed);
		}
	}

	// Token: 0x1400006C RID: 108
	// (add) Token: 0x06004483 RID: 17539 RVA: 0x001B7114 File Offset: 0x001B5314
	// (remove) Token: 0x06004484 RID: 17540 RVA: 0x001B714C File Offset: 0x001B534C
	public event QuestEvent_TwitchEvent TwitchEventReceive;

	// Token: 0x06004485 RID: 17541 RVA: 0x001B7181 File Offset: 0x001B5381
	public void TwitchEventReceived(TwitchObjectiveTypes actionType, string param)
	{
		if (this.TwitchEventReceive != null)
		{
			this.TwitchEventReceive(actionType, param);
		}
	}

	// Token: 0x1400006D RID: 109
	// (add) Token: 0x06004486 RID: 17542 RVA: 0x001B7198 File Offset: 0x001B5398
	// (remove) Token: 0x06004487 RID: 17543 RVA: 0x001B71D0 File Offset: 0x001B53D0
	public event QuestEvent_QuestCompleteEvent QuestComplete;

	// Token: 0x06004488 RID: 17544 RVA: 0x001B7205 File Offset: 0x001B5405
	public void QuestCompleted(FastTags<TagGroup.Global> questTags, QuestClass questClass)
	{
		if (this.QuestComplete != null)
		{
			this.QuestComplete(questTags, questClass);
		}
	}

	// Token: 0x1400006E RID: 110
	// (add) Token: 0x06004489 RID: 17545 RVA: 0x001B721C File Offset: 0x001B541C
	// (remove) Token: 0x0600448A RID: 17546 RVA: 0x001B7254 File Offset: 0x001B5454
	public event QuestEvent_ChallengeAwardCredit ChallengeAwardCredit;

	// Token: 0x0600448B RID: 17547 RVA: 0x001B7289 File Offset: 0x001B5489
	public void ChallengeAwardCredited(string challengeStat, int creditAmount)
	{
		if (this.ChallengeAwardCredit != null)
		{
			this.ChallengeAwardCredit(challengeStat, creditAmount);
		}
	}

	// Token: 0x1400006F RID: 111
	// (add) Token: 0x0600448C RID: 17548 RVA: 0x001B72A0 File Offset: 0x001B54A0
	// (remove) Token: 0x0600448D RID: 17549 RVA: 0x001B72D8 File Offset: 0x001B54D8
	public event QuestEvent_ChallengeAwardCredit QuestAwardCredit;

	// Token: 0x0600448E RID: 17550 RVA: 0x001B730D File Offset: 0x001B550D
	public void QuestAwardCredited(string stat, int creditAmount)
	{
		if (this.QuestAwardCredit != null)
		{
			this.QuestAwardCredit(stat, creditAmount);
		}
	}

	// Token: 0x14000070 RID: 112
	// (add) Token: 0x0600448F RID: 17551 RVA: 0x001B7324 File Offset: 0x001B5524
	// (remove) Token: 0x06004490 RID: 17552 RVA: 0x001B735C File Offset: 0x001B555C
	public event QuestEvent_BiomeEvent BiomeEnter;

	// Token: 0x06004491 RID: 17553 RVA: 0x001B7391 File Offset: 0x001B5591
	public void BiomeEntered(BiomeDefinition biomeDef)
	{
		if (this.BiomeEnter != null)
		{
			this.BiomeEnter(biomeDef);
		}
	}

	// Token: 0x14000071 RID: 113
	// (add) Token: 0x06004492 RID: 17554 RVA: 0x001B73A8 File Offset: 0x001B55A8
	// (remove) Token: 0x06004493 RID: 17555 RVA: 0x001B73E0 File Offset: 0x001B55E0
	public event QuestEvent_ItemValueActionEvent UseItem;

	// Token: 0x06004494 RID: 17556 RVA: 0x001B7415 File Offset: 0x001B5615
	public void UsedItem(ItemValue newValue)
	{
		if (this.UseItem != null)
		{
			this.UseItem(newValue);
		}
	}

	// Token: 0x14000072 RID: 114
	// (add) Token: 0x06004495 RID: 17557 RVA: 0x001B742C File Offset: 0x001B562C
	// (remove) Token: 0x06004496 RID: 17558 RVA: 0x001B7464 File Offset: 0x001B5664
	public event QuestEvent_FloatEvent TimeSurvive;

	// Token: 0x06004497 RID: 17559 RVA: 0x001B7499 File Offset: 0x001B5699
	public void TimeSurvived(float time)
	{
		if (this.TimeSurvive != null)
		{
			this.TimeSurvive(time);
		}
	}

	// Token: 0x14000073 RID: 115
	// (add) Token: 0x06004498 RID: 17560 RVA: 0x001B74B0 File Offset: 0x001B56B0
	// (remove) Token: 0x06004499 RID: 17561 RVA: 0x001B74E8 File Offset: 0x001B56E8
	public event QuestEvent_Event BloodMoonSurvive;

	// Token: 0x0600449A RID: 17562 RVA: 0x001B751D File Offset: 0x001B571D
	public void BloodMoonSurvived()
	{
		if (this.BloodMoonSurvive != null)
		{
			this.BloodMoonSurvive();
		}
	}

	// Token: 0x0600449B RID: 17563 RVA: 0x001B7534 File Offset: 0x001B5734
	public void Update()
	{
		ObjectiveRallyPoint.SetupFlags(this.objectivesToUpdate);
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < this.objectivesToUpdate.Count; i++)
		{
			this.objectivesToUpdate[i].HandleUpdate(deltaTime);
		}
		for (int j = 0; j < this.challengeObjectivesToUpdate.Count; j++)
		{
			this.challengeObjectivesToUpdate[j].HandleUpdate(deltaTime);
		}
		for (int k = this.questTrackersToUpdate.Count - 1; k >= 0; k--)
		{
			if (!this.questTrackersToUpdate[k].Update(deltaTime))
			{
				this.questTrackersToUpdate.RemoveAt(k);
			}
		}
		if (this.challengeTrackerToUpdate != null && !this.challengeTrackerToUpdate.Update(deltaTime))
		{
			this.challengeTrackerToUpdate = null;
		}
		foreach (KeyValuePair<Vector3, SleeperEventData> keyValuePair in this.SleeperVolumeUpdateDictionary)
		{
			if (keyValuePair.Value.Update())
			{
				this.removeSleeperDataList.Add(keyValuePair.Value.position);
			}
		}
		for (int l = 0; l < this.removeSleeperDataList.Count; l++)
		{
			this.SleeperVolumeUpdateDictionary.Remove(this.removeSleeperDataList[l]);
		}
		this.removeSleeperDataList.Clear();
	}

	// Token: 0x0600449C RID: 17564 RVA: 0x001B769C File Offset: 0x001B589C
	public void HandlePlayerDisconnect(EntityPlayer player)
	{
		for (int i = 0; i < player.QuestJournal.quests.Count; i++)
		{
			Quest quest = player.QuestJournal.quests[i];
			if (quest.CurrentState == Quest.QuestState.InProgress)
			{
				quest.HandleUnlockPOI(player);
				this.FinishTreasureQuest(quest.QuestCode, player);
			}
		}
	}

	// Token: 0x0600449D RID: 17565 RVA: 0x001B76F4 File Offset: 0x001B58F4
	public void HandleAllPlayersDisconnect()
	{
		foreach (int key in this.TreasureQuestDictionary.Keys)
		{
			this.TreasureQuestDictionary[key].Remove();
		}
		this.TreasureQuestDictionary.Clear();
	}

	// Token: 0x0600449E RID: 17566 RVA: 0x001B7764 File Offset: 0x001B5964
	[PublicizedFrom(EAccessModifier.Internal)]
	public void AddTraderResetQuestsForPlayer(int playerID, int traderID)
	{
		if (!this.ForceResetQuestTrader.ContainsKey(playerID))
		{
			this.ForceResetQuestTrader.Add(playerID, traderID);
			return;
		}
		this.ForceResetQuestTrader[playerID] = traderID;
	}

	// Token: 0x0600449F RID: 17567 RVA: 0x001B778F File Offset: 0x001B598F
	public void ClearTraderResetQuestsForPlayer(int playerID)
	{
		if (this.ForceResetQuestTrader.ContainsKey(playerID))
		{
			this.ForceResetQuestTrader.Remove(playerID);
		}
	}

	// Token: 0x060044A0 RID: 17568 RVA: 0x001B77AC File Offset: 0x001B59AC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CheckResetQuestTrader(int playerEntityID, int npcEntityID)
	{
		if (!this.ForceResetQuestTrader.ContainsKey(playerEntityID))
		{
			return false;
		}
		Log.Out(string.Format("CheckResetQuestTrader {0}", this.ForceResetQuestTrader[playerEntityID] == npcEntityID));
		return this.ForceResetQuestTrader[playerEntityID] == npcEntityID;
	}

	// Token: 0x060044A1 RID: 17569 RVA: 0x001B77FB File Offset: 0x001B59FB
	public void AddObjectiveToBeUpdated(BaseObjective obj)
	{
		if (!this.objectivesToUpdate.Contains(obj))
		{
			this.objectivesToUpdate.Add(obj);
		}
	}

	// Token: 0x060044A2 RID: 17570 RVA: 0x001B7817 File Offset: 0x001B5A17
	public void RemoveObjectiveToBeUpdated(BaseObjective obj)
	{
		if (this.objectivesToUpdate.Contains(obj))
		{
			this.objectivesToUpdate.Remove(obj);
		}
	}

	// Token: 0x060044A3 RID: 17571 RVA: 0x001B7834 File Offset: 0x001B5A34
	public void AddObjectiveToBeUpdated(BaseChallengeObjective obj)
	{
		if (!this.challengeObjectivesToUpdate.Contains(obj))
		{
			this.challengeObjectivesToUpdate.Add(obj);
		}
	}

	// Token: 0x060044A4 RID: 17572 RVA: 0x001B7850 File Offset: 0x001B5A50
	public void RemoveObjectiveToBeUpdated(BaseChallengeObjective obj)
	{
		if (this.challengeObjectivesToUpdate.Contains(obj))
		{
			this.challengeObjectivesToUpdate.Remove(obj);
		}
	}

	// Token: 0x060044A5 RID: 17573 RVA: 0x001B786D File Offset: 0x001B5A6D
	public void AddTrackerToBeUpdated(TrackingHandler track)
	{
		if (!this.questTrackersToUpdate.Contains(track))
		{
			this.questTrackersToUpdate.Add(track);
		}
	}

	// Token: 0x060044A6 RID: 17574 RVA: 0x001B7889 File Offset: 0x001B5A89
	public void RemoveTrackerToBeUpdated(TrackingHandler track)
	{
		if (this.questTrackersToUpdate.Contains(track))
		{
			this.questTrackersToUpdate.Remove(track);
		}
	}

	// Token: 0x060044A7 RID: 17575 RVA: 0x001B78A6 File Offset: 0x001B5AA6
	public void AddTrackerToBeUpdated(ChallengeTrackingHandler track)
	{
		this.challengeTrackerToUpdate = track;
	}

	// Token: 0x060044A8 RID: 17576 RVA: 0x001B78AF File Offset: 0x001B5AAF
	public void RemoveTrackerToBeUpdated(ChallengeTrackingHandler track)
	{
		this.challengeTrackerToUpdate = null;
	}

	// Token: 0x14000074 RID: 116
	// (add) Token: 0x060044A9 RID: 17577 RVA: 0x001B78B8 File Offset: 0x001B5AB8
	// (remove) Token: 0x060044AA RID: 17578 RVA: 0x001B78F0 File Offset: 0x001B5AF0
	public event QuestEvent_SleeperVolumePositionChanged SleeperVolumePositionAdd;

	// Token: 0x14000075 RID: 117
	// (add) Token: 0x060044AB RID: 17579 RVA: 0x001B7928 File Offset: 0x001B5B28
	// (remove) Token: 0x060044AC RID: 17580 RVA: 0x001B7960 File Offset: 0x001B5B60
	public event QuestEvent_SleeperVolumePositionChanged SleeperVolumePositionRemove;

	// Token: 0x060044AD RID: 17581 RVA: 0x001B7995 File Offset: 0x001B5B95
	public void SleeperVolumePositionAdded(Vector3 pos)
	{
		if (this.SleeperVolumePositionAdd != null)
		{
			this.SleeperVolumePositionAdd(pos);
		}
	}

	// Token: 0x060044AE RID: 17582 RVA: 0x001B79AB File Offset: 0x001B5BAB
	public void SleeperVolumePositionRemoved(Vector3 pos)
	{
		if (this.SleeperVolumePositionRemove != null)
		{
			this.SleeperVolumePositionRemove(pos);
		}
	}

	// Token: 0x060044AF RID: 17583 RVA: 0x001B79C1 File Offset: 0x001B5BC1
	public void AddSleeperVolumeLocation(Vector3 newLocation)
	{
		this.SleeperVolumeLocationList.Add(newLocation);
	}

	// Token: 0x060044B0 RID: 17584 RVA: 0x001B79D0 File Offset: 0x001B5BD0
	public void SubscribeToUpdateEvent(int entityID, Vector3 prefabPos)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (!this.SleeperVolumeUpdateDictionary.ContainsKey(prefabPos))
			{
				SleeperEventData sleeperEventData = new SleeperEventData();
				sleeperEventData.SetupData(prefabPos);
				this.SleeperVolumeUpdateDictionary.Add(prefabPos, sleeperEventData);
			}
			SleeperEventData sleeperEventData2 = this.SleeperVolumeUpdateDictionary[prefabPos];
			this.removeSleeperDataList.Remove(prefabPos);
			if (!sleeperEventData2.EntityList.Contains(entityID))
			{
				sleeperEventData2.EntityList.Add(entityID);
				return;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.ClearSleeper, entityID, prefabPos, true), false);
		}
	}

	// Token: 0x060044B1 RID: 17585 RVA: 0x001B7A60 File Offset: 0x001B5C60
	public void UnSubscribeToUpdateEvent(int entityID, Vector3 prefabPos)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (!this.SleeperVolumeUpdateDictionary.ContainsKey(prefabPos))
			{
				return;
			}
			SleeperEventData sleeperEventData = this.SleeperVolumeUpdateDictionary[prefabPos];
			if (!sleeperEventData.EntityList.Contains(entityID))
			{
				return;
			}
			sleeperEventData.EntityList.Remove(entityID);
			if (sleeperEventData.EntityList.Count == 0)
			{
				this.removeSleeperDataList.Add(prefabPos);
			}
			using (List<SleeperVolume>.Enumerator enumerator = sleeperEventData.SleeperVolumes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SleeperVolume sleeperVolume = enumerator.Current;
					QuestEventManager.Current.SleeperVolumePositionRemoved(sleeperVolume.Center);
				}
				return;
			}
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.ClearSleeper, entityID, prefabPos, false), false);
	}

	// Token: 0x060044B2 RID: 17586 RVA: 0x001B7B38 File Offset: 0x001B5D38
	public IEnumerator QuestLockPOI(int entityID, QuestClass questClass, Vector3 prefabPos, FastTags<TagGroup.Global> questTags, int[] sharedWithList, Action completionCallback)
	{
		List<PrefabInstance> prefabsFromWorldPosInside = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabsFromWorldPosInside(prefabPos, questTags);
		yield return GameManager.Instance.World.ResetPOIS(prefabsFromWorldPosInside, questTags, entityID, sharedWithList, questClass);
		if (completionCallback != null)
		{
			completionCallback();
		}
		yield break;
	}

	// Token: 0x060044B3 RID: 17587 RVA: 0x001B7B70 File Offset: 0x001B5D70
	public void QuestUnlockPOI(int entityID, Vector3 prefabPos)
	{
		PrefabInstance prefabFromWorldPos = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabFromWorldPos((int)prefabPos.x, (int)prefabPos.z);
		if (prefabFromWorldPos.lockInstance != null)
		{
			prefabFromWorldPos.lockInstance.RemoveQuester(entityID);
		}
	}

	// Token: 0x060044B4 RID: 17588 RVA: 0x001B7BB0 File Offset: 0x001B5DB0
	public QuestEventManager.POILockoutReasonTypes CheckForPOILockouts(int entityId, Vector2 prefabPos, out ulong extraData)
	{
		World world = GameManager.Instance.World;
		PrefabInstance prefabFromWorldPos = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabFromWorldPos((int)prefabPos.x, (int)prefabPos.y);
		Rect rect = new Rect((float)prefabFromWorldPos.boundingBoxPosition.x, (float)prefabFromWorldPos.boundingBoxPosition.z, (float)prefabFromWorldPos.boundingBoxSize.x, (float)prefabFromWorldPos.boundingBoxSize.z);
		if (prefabFromWorldPos.lockInstance != null && prefabFromWorldPos.lockInstance.CheckQuestLock())
		{
			prefabFromWorldPos.lockInstance = null;
		}
		if (prefabFromWorldPos.lockInstance != null)
		{
			extraData = prefabFromWorldPos.lockInstance.LockedOutUntil;
			return QuestEventManager.POILockoutReasonTypes.QuestLock;
		}
		extraData = 0UL;
		EntityPlayer entityPlayer = (EntityPlayer)world.GetEntity(entityId);
		if (entityPlayer != null)
		{
			for (int i = 0; i < world.Players.list.Count; i++)
			{
				Vector3 position = world.Players.list[i].GetPosition();
				EntityPlayer entityPlayer2 = world.Players.list[i];
				if (entityPlayer != entityPlayer2 && (!entityPlayer.IsInParty() || !entityPlayer.Party.MemberList.Contains(entityPlayer2)) && rect.Contains(new Vector2(position.x, position.z)))
				{
					return QuestEventManager.POILockoutReasonTypes.PlayerInside;
				}
			}
		}
		GameUtils.EPlayerHomeType eplayerHomeType = prefabFromWorldPos.CheckForAnyPlayerHome(world);
		if (eplayerHomeType == GameUtils.EPlayerHomeType.Landclaim)
		{
			return QuestEventManager.POILockoutReasonTypes.LandClaim;
		}
		if (eplayerHomeType == GameUtils.EPlayerHomeType.Bedroll)
		{
			return QuestEventManager.POILockoutReasonTypes.Bedroll;
		}
		return QuestEventManager.POILockoutReasonTypes.None;
	}

	// Token: 0x060044B5 RID: 17589 RVA: 0x001B7D18 File Offset: 0x001B5F18
	public void SetupRepairForMP(List<Vector3i> repairBlockList, List<bool> repairStates, World _world, Vector3 prefabPos)
	{
		PrefabInstance prefabFromWorldPos = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabFromWorldPos((int)prefabPos.x, (int)prefabPos.z);
		Vector3i vector3i = new Vector3i(prefabPos);
		Vector3i size = prefabFromWorldPos.prefab.size;
		int num = World.toChunkXZ(vector3i.x - 1);
		int num2 = World.toChunkXZ(vector3i.x + size.x + 1);
		int num3 = World.toChunkXZ(vector3i.z - 1);
		int num4 = World.toChunkXZ(vector3i.z + size.z + 1);
		repairBlockList.Clear();
		repairStates.Clear();
		Rect rect = new Rect((float)vector3i.x, (float)vector3i.z, (float)size.x, (float)size.z);
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				Chunk chunk = _world.GetChunkSync(i, j) as Chunk;
				if (chunk != null)
				{
					List<Vector3i> list = chunk.IndexedBlocks[Constants.cQuestRestorePowerIndexName];
					if (list != null)
					{
						for (int k = 0; k < list.Count; k++)
						{
							BlockValue block = chunk.GetBlock(list[k]);
							if (!block.ischild)
							{
								Vector3i vector3i2 = chunk.ToWorldPos(list[k]);
								if (rect.Contains(new Vector2((float)vector3i2.x, (float)vector3i2.z)))
								{
									repairStates.Add(!block.Block.UpgradeBlock.isair);
									repairBlockList.Add(vector3i2);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060044B6 RID: 17590 RVA: 0x001B7EB0 File Offset: 0x001B60B0
	public void SetupActivateForMP(int entityID, int questCode, string completeEvent, List<Vector3i> activateBlockList, World _world, Vector3 prefabPos, string indexName, int[] sharedWithList)
	{
		PrefabInstance prefabFromWorldPos = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabFromWorldPos((int)prefabPos.x, (int)prefabPos.z);
		Vector3i vector3i = new Vector3i(prefabPos);
		Vector3i size = prefabFromWorldPos.prefab.size;
		EntityPlayer entityPlayer = _world.GetEntity(entityID) as EntityPlayer;
		int num = World.toChunkXZ(vector3i.x - 1);
		int num2 = World.toChunkXZ(vector3i.x + size.x + 1);
		int num3 = World.toChunkXZ(vector3i.z - 1);
		int num4 = World.toChunkXZ(vector3i.z + size.z + 1);
		activateBlockList.Clear();
		Rect rect = new Rect((float)vector3i.x, (float)vector3i.z, (float)size.x, (float)size.z);
		new BlockChangeInfo();
		List<BlockChangeInfo> list = new List<BlockChangeInfo>();
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				Chunk chunk = _world.GetChunkSync(i, j) as Chunk;
				if (chunk != null)
				{
					List<Vector3i> list2 = chunk.IndexedBlocks[indexName];
					if (list2 != null)
					{
						for (int k = 0; k < list2.Count; k++)
						{
							BlockValue block = chunk.GetBlock(list2[k]);
							if (!block.ischild)
							{
								Vector3i vector3i2 = chunk.ToWorldPos(list2[k]);
								if (rect.Contains(new Vector2((float)vector3i2.x, (float)vector3i2.z)))
								{
									activateBlockList.Add(vector3i2);
									if (block.Block is BlockQuestActivate)
									{
										(block.Block as BlockQuestActivate).SetupForQuest(_world, chunk, vector3i2, block, list);
									}
								}
							}
						}
					}
				}
			}
		}
		if (entityPlayer is EntityPlayerLocal)
		{
			entityPlayer.QuestJournal.HandleRestorePowerReceived(prefabPos, activateBlockList);
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.SetupRestorePower, entityPlayer.entityId, questCode, completeEvent, prefabPos, activateBlockList), false, entityPlayer.entityId, -1, -1, null, 192, false);
		}
		QuestEventManager.Current.AddRestorePowerQuest(questCode, entityID, new Vector3i(prefabPos), completeEvent);
		if (entityPlayer.IsInParty() && sharedWithList != null)
		{
			Party party = entityPlayer.Party;
			for (int l = 0; l < sharedWithList.Length; l++)
			{
				EntityPlayer entityPlayer2 = _world.GetEntity(sharedWithList[l]) as EntityPlayer;
				if (entityPlayer2 is EntityPlayerLocal)
				{
					entityPlayer2.QuestJournal.HandleRestorePowerReceived(prefabPos, activateBlockList);
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.SetupRestorePower, entityPlayer2.entityId, questCode, completeEvent, prefabPos, activateBlockList), false, entityPlayer2.entityId, -1, -1, null, 192, false);
				}
				QuestEventManager.Current.AddRestorePowerQuest(questCode, sharedWithList[l], new Vector3i(prefabPos), completeEvent);
			}
		}
		if (list.Count > 0)
		{
			GameManager.Instance.StartCoroutine(this.UpdateBlocks(list));
		}
		GameEventManager.Current.HandleAction("quest_poi_lights_off", null, entityPlayer, false, vector3i, "", "", false, true, "", null);
	}

	// Token: 0x060044B7 RID: 17591 RVA: 0x001B81D7 File Offset: 0x001B63D7
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator UpdateBlocks(List<BlockChangeInfo> blockChanges)
	{
		yield return new WaitForSeconds(1f);
		if (GameManager.Instance != null && GameManager.Instance.World != null)
		{
			GameManager.Instance.World.SetBlocksRPC(blockChanges);
		}
		yield break;
	}

	// Token: 0x060044B8 RID: 17592 RVA: 0x001B81E8 File Offset: 0x001B63E8
	public void SetupFetchForMP(int entityID, Vector3 prefabPos, ObjectiveFetchFromContainer.FetchModeTypes fetchMode, int[] sharedWithList)
	{
		PrefabInstance prefabFromWorldPos = GameManager.Instance.GetDynamicPrefabDecorator().GetPrefabFromWorldPos((int)prefabPos.x, (int)prefabPos.z);
		this.HandleContainerPositions(GameManager.Instance.World, entityID, new Vector3i(prefabPos), prefabFromWorldPos.prefab.size, fetchMode, sharedWithList);
	}

	// Token: 0x060044B9 RID: 17593 RVA: 0x001B8238 File Offset: 0x001B6438
	public void HandleContainerPositions(World _world, int _entityID, Vector3i _prefabPosition, Vector3i _prefabSize, ObjectiveFetchFromContainer.FetchModeTypes fetchMode, int[] sharedWithList)
	{
		int num = World.toChunkXZ(_prefabPosition.x - 1);
		int num2 = World.toChunkXZ(_prefabPosition.x + _prefabSize.x + 1);
		int num3 = World.toChunkXZ(_prefabPosition.z - 1);
		int num4 = World.toChunkXZ(_prefabPosition.z + _prefabSize.z + 1);
		List<Vector3i> list = new List<Vector3i>();
		Rect rect = new Rect((float)_prefabPosition.x, (float)_prefabPosition.z, (float)_prefabSize.x, (float)_prefabSize.z);
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				Chunk chunk = _world.GetChunkSync(i, j) as Chunk;
				if (chunk != null)
				{
					List<Vector3i> list2 = chunk.IndexedBlocks[Constants.cQuestLootFetchContainerIndexName];
					if (list2 != null)
					{
						for (int k = 0; k < list2.Count; k++)
						{
							if (!chunk.GetBlock(list2[k]).ischild)
							{
								Vector3i vector3i = chunk.ToWorldPos(list2[k]);
								if (rect.Contains(new Vector2((float)vector3i.x, (float)vector3i.z)))
								{
									list.Add(vector3i);
								}
							}
						}
					}
				}
			}
		}
		if (list.Count == 0)
		{
			Log.Error("Valid container not found for fetch loot.");
			return;
		}
		List<int> list3 = new List<int>();
		EntityPlayer entityPlayer = _world.GetEntity(_entityID) as EntityPlayer;
		Quest.PositionDataTypes dataType = (fetchMode == ObjectiveFetchFromContainer.FetchModeTypes.Standard) ? Quest.PositionDataTypes.FetchContainer : Quest.PositionDataTypes.HiddenCache;
		int num5 = _world.GetGameRandom().RandomRange(list.Count);
		if (entityPlayer is EntityPlayerLocal)
		{
			entityPlayer.QuestJournal.SetActivePositionData(dataType, list[num5]);
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.SetupFetch, _entityID, list[num5].ToVector3(), fetchMode), false, -1, -1, -1, null, 192, false);
		}
		list3.Add(num5);
		if (entityPlayer.IsInParty() && sharedWithList != null)
		{
			Party party = entityPlayer.Party;
			for (int l = 0; l < sharedWithList.Length; l++)
			{
				entityPlayer = (_world.GetEntity(sharedWithList[l]) as EntityPlayer);
				num5 = _world.GetGameRandom().RandomRange(list.Count);
				if (entityPlayer is EntityPlayerLocal)
				{
					entityPlayer.QuestJournal.SetActivePositionData(dataType, list[num5]);
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.SetupFetch, entityPlayer.entityId, list[num5].ToVector3(), fetchMode), false, -1, -1, -1, null, 192, false);
				}
				if (!list3.Contains(num5))
				{
					list3.Add(num5);
				}
			}
		}
		List<BlockChangeInfo> list4 = new List<BlockChangeInfo>();
		GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
		for (int m = 0; m < list.Count; m++)
		{
			if (!list3.Contains(m))
			{
				Chunk chunk2 = (Chunk)_world.GetChunkFromWorldPos(list[m]);
				BlockValue blockValue = BlockPlaceholderMap.Instance.Replace(Block.GetBlockValue("cntQuestRandomLootHelper", false), gameRandom, chunk2, list[m].x, 0, list[m].z, FastTags<TagGroup.Global>.none, false, true);
				list4.Add(new BlockChangeInfo(chunk2.ClrIdx, list[m], blockValue));
			}
		}
		if (list4.Count > 0)
		{
			GameManager.Instance.StartCoroutine(this.UpdateBlocks(list4));
		}
	}

	// Token: 0x060044BA RID: 17594 RVA: 0x001B85C4 File Offset: 0x001B67C4
	public void Cleanup()
	{
		this.BlockPickup = null;
		this.BlockPlace = null;
		this.BlockUpgrade = null;
		this.AddItem = null;
		this.AssembleItem = null;
		this.CraftItem = null;
		this.ExchangeFromItem = null;
		this.ScrapItem = null;
		this.RepairItem = null;
		this.SkillPointSpent = null;
		this.WearItem = null;
		this.WindowChanged = null;
		this.ContainerOpened = null;
		this.EntityKill = null;
		this.HarvestItem = null;
		this.SellItems = null;
		this.BuyItems = null;
		this.ExplosionDetected = null;
		this.ChallengeComplete = null;
		this.BiomeEnter = null;
		this.UseItem = null;
		this.TimeSurvive = null;
		this.BloodMoonSurvive = null;
		this.objectivesToUpdate = null;
		this.npcQuestData.Clear();
		this.npcQuestData = null;
		this.questTierRewards.Clear();
		this.questTierRewards = null;
		QuestEventManager.instance = null;
	}

	// Token: 0x060044BB RID: 17595 RVA: 0x001B86A4 File Offset: 0x001B68A4
	public void SetupQuestList(EntityTrader npc, int playerEntityID, List<Quest> questList)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			return;
		}
		if (!this.npcQuestData.ContainsKey(npc.entityId))
		{
			this.npcQuestData.Add(npc.entityId, new NPCQuestData());
		}
		if (!this.npcQuestData[npc.entityId].PlayerQuestList.ContainsKey(playerEntityID))
		{
			this.npcQuestData[npc.entityId].PlayerQuestList.Add(playerEntityID, new NPCQuestData.PlayerQuestData(questList));
		}
		else
		{
			this.npcQuestData[npc.entityId].PlayerQuestList[playerEntityID].QuestList = questList;
		}
		if (!(GameManager.Instance.World.GetEntity(playerEntityID) is EntityPlayerLocal))
		{
			NetPackageNPCQuestList.SendQuestPacketsToPlayer(npc, playerEntityID);
		}
	}

	// Token: 0x060044BC RID: 17596 RVA: 0x001B876C File Offset: 0x001B696C
	public List<Quest> GetQuestList(World world, int npcEntityID, int playerEntityID)
	{
		if (this.npcQuestData.ContainsKey(npcEntityID))
		{
			NPCQuestData npcquestData = this.npcQuestData[npcEntityID];
			if (npcquestData.PlayerQuestList.ContainsKey(playerEntityID))
			{
				NPCQuestData.PlayerQuestData playerQuestData = npcquestData.PlayerQuestList[playerEntityID];
				if (QuestEventManager.Current.CheckResetQuestTrader(playerEntityID, npcEntityID))
				{
					playerQuestData.QuestList.Clear();
					playerQuestData.QuestList = null;
					QuestEventManager.Current.ClearTraderResetQuestsForPlayer(playerEntityID);
				}
				else if ((int)(world.GetWorldTime() - playerQuestData.LastUpdate) > 24000)
				{
					playerQuestData.QuestList.Clear();
					playerQuestData.QuestList = null;
				}
				return playerQuestData.QuestList;
			}
		}
		return null;
	}

	// Token: 0x060044BD RID: 17597 RVA: 0x001B880E File Offset: 0x001B6A0E
	public void ClearQuestList(int npcEntityID)
	{
		if (this.npcQuestData.ContainsKey(npcEntityID))
		{
			this.npcQuestData[npcEntityID].PlayerQuestList.Clear();
		}
	}

	// Token: 0x060044BE RID: 17598 RVA: 0x001B8834 File Offset: 0x001B6A34
	public void ClearQuestListForPlayer(int npcEntityID, int playerID)
	{
		if (this.npcQuestData.ContainsKey(npcEntityID))
		{
			NPCQuestData npcquestData = this.npcQuestData[npcEntityID];
			if (npcquestData.PlayerQuestList.ContainsKey(playerID))
			{
				npcquestData.PlayerQuestList.Remove(playerID);
			}
		}
	}

	// Token: 0x060044BF RID: 17599 RVA: 0x001B8877 File Offset: 0x001B6A77
	public void AddQuestTierReward(QuestTierReward reward)
	{
		if (this.questTierRewards == null)
		{
			this.questTierRewards = new List<QuestTierReward>();
		}
		this.questTierRewards.Add(reward);
	}

	// Token: 0x060044C0 RID: 17600 RVA: 0x001B8898 File Offset: 0x001B6A98
	public void HandleNewCompletedQuest(EntityPlayer player, byte questFaction, int completedQuestTier, bool addsToTierComplete)
	{
		if (addsToTierComplete)
		{
			int currentFactionTier = player.QuestJournal.GetCurrentFactionTier(questFaction, 0, true);
			int currentFactionTier2 = player.QuestJournal.GetCurrentFactionTier(questFaction, completedQuestTier, true);
			if (currentFactionTier != currentFactionTier2)
			{
				for (int i = 0; i < this.questTierRewards.Count; i++)
				{
					if (this.questTierRewards[i].Tier == currentFactionTier2)
					{
						this.questTierRewards[i].GiveRewards(player);
					}
				}
			}
		}
	}

	// Token: 0x060044C1 RID: 17601 RVA: 0x001B8908 File Offset: 0x001B6B08
	public void HandleRallyMarkerActivate(EntityPlayerLocal _player, Vector3i blockPos, BlockValue blockValue)
	{
		Quest quest = _player.QuestJournal.HasQuestAtRallyPosition(blockPos.ToVector3(), true);
		if (quest != null)
		{
			Action action = delegate()
			{
				QuestEventManager.Current.BlockActivated(blockValue.Block.GetBlockName(), blockPos);
			};
			if (_player.IsInParty())
			{
				List<EntityPlayer> sharedWithListNotInRange = quest.GetSharedWithListNotInRange();
				if (sharedWithListNotInRange != null && sharedWithListNotInRange.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < sharedWithListNotInRange.Count; i++)
					{
						stringBuilder.Append(sharedWithListNotInRange[i].PlayerDisplayName);
						if (i < sharedWithListNotInRange.Count - 1)
						{
							stringBuilder.Append(", ");
						}
					}
					XUiC_MessageBoxWindowGroup.ShowMessageBox(_player.PlayerUI.xui, "Rally Activate", string.Format(Localization.Get("xuiQuestRallyOutOfRange", false), stringBuilder.ToString().Trim(',')), XUiC_MessageBoxWindowGroup.MessageBoxTypes.OkCancel, action, null, true, true, true);
					return;
				}
				action();
				return;
			}
			else
			{
				action();
			}
		}
	}

	// Token: 0x060044C2 RID: 17602 RVA: 0x001B8A0C File Offset: 0x001B6C0C
	public void AddTreasureQuest(int _questCode, int _entityID, int _blocksPerReduction, Vector3i _position, Vector3 _treasureOffset)
	{
		if (!this.TreasureQuestDictionary.ContainsKey(_questCode))
		{
			TreasureQuestData value = new TreasureQuestData(_questCode, _entityID, _blocksPerReduction, _position, _treasureOffset);
			this.TreasureQuestDictionary.Add(_questCode, value);
		}
	}

	// Token: 0x060044C3 RID: 17603 RVA: 0x001B8A41 File Offset: 0x001B6C41
	public void SetTreasureContainerPosition(int _questCode, Vector3i _updatedPosition)
	{
		if (this.TreasureQuestDictionary.ContainsKey(_questCode))
		{
			this.TreasureQuestDictionary[_questCode].UpdatePosition(_updatedPosition);
		}
	}

	// Token: 0x060044C4 RID: 17604 RVA: 0x001B8A64 File Offset: 0x001B6C64
	public bool GetTreasureContainerPosition(int _questCode, float _distance, int _offset, float _treasureRadius, Vector3 _startPosition, int _entityID, bool _useNearby, int _currentBlocksPerReduction, out int _blocksPerReduction, out Vector3i _position, out Vector3 _treasureOffset)
	{
		_position = Vector3i.zero;
		_treasureOffset = Vector3.zero;
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestTreasurePoint>().Setup(_questCode, _distance, _offset, _treasureRadius, _startPosition, _entityID, _useNearby, _currentBlocksPerReduction), false);
			_position = Vector3i.zero;
			_treasureOffset = Vector3.zero;
			_blocksPerReduction = _currentBlocksPerReduction;
			return true;
		}
		if (this.TreasureQuestDictionary.ContainsKey(_questCode))
		{
			_position = this.TreasureQuestDictionary[_questCode].Position;
			_treasureOffset = this.TreasureQuestDictionary[_questCode].TreasureOffset;
			this.TreasureQuestDictionary[_questCode].AddSharedQuester(_entityID, _currentBlocksPerReduction);
			_blocksPerReduction = this.TreasureQuestDictionary[_questCode].BlocksPerReduction;
			return true;
		}
		_blocksPerReduction = _currentBlocksPerReduction;
		float num = _distance + 500f;
		for (float num2 = _distance; num2 < num; num2 += 50f)
		{
			for (int i = 0; i < 5; i++)
			{
				if (ObjectiveTreasureChest.CalculateTreasurePoint(_startPosition, num2, _offset, _treasureRadius - 1f, _useNearby, out _position, out _treasureOffset))
				{
					this.AddTreasureQuest(_questCode, _entityID, _blocksPerReduction, _position, _treasureOffset);
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060044C5 RID: 17605 RVA: 0x001B8BA8 File Offset: 0x001B6DA8
	public void UpdateTreasureBlocksPerReduction(int _questCode, int _newBlocksPerReduction)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (this.TreasureQuestDictionary.ContainsKey(_questCode))
			{
				this.TreasureQuestDictionary[_questCode].SendBlocksPerReductionUpdate(_newBlocksPerReduction);
				return;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestTreasurePoint>().Setup(_questCode, _newBlocksPerReduction), false);
		}
	}

	// Token: 0x060044C6 RID: 17606 RVA: 0x001B8BFC File Offset: 0x001B6DFC
	public void FinishTreasureQuest(int _questCode, EntityPlayer _player)
	{
		TreasureQuestData treasureQuestData;
		if (this.TreasureQuestDictionary.TryGetValue(_questCode, out treasureQuestData))
		{
			treasureQuestData.RemoveSharedQuester(_player);
			ChunkProviderGenerateWorld chunkProviderGenerateWorld = GameManager.Instance.World.ChunkCache.ChunkProvider as ChunkProviderGenerateWorld;
			if (chunkProviderGenerateWorld != null)
			{
				Debug.Log(string.Format("[FinishTreasureQuest] Requesting reset at world position: {0}", treasureQuestData.Position));
				Vector2i vector2i = World.toChunkXZ(treasureQuestData.Position);
				for (int i = vector2i.x - 1; i <= vector2i.x + 1; i++)
				{
					for (int j = vector2i.y - 1; j <= vector2i.y + 1; j++)
					{
						long chunkKey = WorldChunkCache.MakeChunkKey(i, j);
						chunkProviderGenerateWorld.RequestChunkReset(chunkKey);
					}
				}
			}
		}
	}

	// Token: 0x060044C7 RID: 17607 RVA: 0x001B8CB0 File Offset: 0x001B6EB0
	public void AddRestorePowerQuest(int _questCode, int _entityID, Vector3i _position, string _completeEvent)
	{
		if (!this.BlockActivateQuestDictionary.ContainsKey(_questCode))
		{
			RestorePowerQuestData value = new RestorePowerQuestData(_questCode, _entityID, _position, _completeEvent);
			this.BlockActivateQuestDictionary.Add(_questCode, value);
			return;
		}
		this.BlockActivateQuestDictionary[_questCode].AddSharedQuester(_entityID);
	}

	// Token: 0x060044C8 RID: 17608 RVA: 0x001B8CF6 File Offset: 0x001B6EF6
	public void FinishManagedQuest(int _questCode, EntityPlayer _player)
	{
		if (this.BlockActivateQuestDictionary.ContainsKey(_questCode))
		{
			this.BlockActivateQuestDictionary[_questCode].RemoveSharedQuester(_player);
		}
	}

	// Token: 0x040035BB RID: 13755
	[PublicizedFrom(EAccessModifier.Private)]
	public static QuestEventManager instance = null;

	// Token: 0x040035BC RID: 13756
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BaseObjective> objectivesToUpdate = new List<BaseObjective>();

	// Token: 0x040035BD RID: 13757
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BaseChallengeObjective> challengeObjectivesToUpdate = new List<BaseChallengeObjective>();

	// Token: 0x040035BE RID: 13758
	[PublicizedFrom(EAccessModifier.Private)]
	public List<TrackingHandler> questTrackersToUpdate = new List<TrackingHandler>();

	// Token: 0x040035BF RID: 13759
	[PublicizedFrom(EAccessModifier.Private)]
	public ChallengeTrackingHandler challengeTrackerToUpdate;

	// Token: 0x040035C0 RID: 13760
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3> removeSleeperDataList = new List<Vector3>();

	// Token: 0x040035C1 RID: 13761
	public Dictionary<int, NPCQuestData> npcQuestData = new Dictionary<int, NPCQuestData>();

	// Token: 0x040035C2 RID: 13762
	public List<QuestTierReward> questTierRewards = new List<QuestTierReward>();

	// Token: 0x040035C3 RID: 13763
	public Dictionary<Vector3, SleeperEventData> SleeperVolumeUpdateDictionary = new Dictionary<Vector3, SleeperEventData>();

	// Token: 0x040035C4 RID: 13764
	public List<Vector3> SleeperVolumeLocationList = new List<Vector3>();

	// Token: 0x040035C5 RID: 13765
	public Dictionary<int, TreasureQuestData> TreasureQuestDictionary = new Dictionary<int, TreasureQuestData>();

	// Token: 0x040035C6 RID: 13766
	public Dictionary<int, RestorePowerQuestData> BlockActivateQuestDictionary = new Dictionary<int, RestorePowerQuestData>();

	// Token: 0x040035C7 RID: 13767
	public Dictionary<int, List<PrefabInstance>> tierPrefabList = new Dictionary<int, List<PrefabInstance>>();

	// Token: 0x040035C8 RID: 13768
	public Dictionary<TraderArea, List<QuestEventManager.PrefabListData>> TraderPrefabList = new Dictionary<TraderArea, List<QuestEventManager.PrefabListData>>();

	// Token: 0x040035C9 RID: 13769
	public Rect QuestBounds;

	// Token: 0x040035CA RID: 13770
	public List<Vector3i> ActiveQuestBlocks = new List<Vector3i>();

	// Token: 0x040035CB RID: 13771
	public Dictionary<int, int> ForceResetQuestTrader = new Dictionary<int, int>();

	// Token: 0x040035CC RID: 13772
	public static FastTags<TagGroup.Global> manualResetTag = FastTags<TagGroup.Global>.Parse("manual");

	// Token: 0x040035CD RID: 13773
	public static FastTags<TagGroup.Global> traderTag = FastTags<TagGroup.Global>.Parse("trader");

	// Token: 0x040035CE RID: 13774
	public static FastTags<TagGroup.Global> clearTag = FastTags<TagGroup.Global>.Parse("clear");

	// Token: 0x040035CF RID: 13775
	public static FastTags<TagGroup.Global> treasureTag = FastTags<TagGroup.Global>.Parse("treasure");

	// Token: 0x040035D0 RID: 13776
	public static FastTags<TagGroup.Global> fetchTag = FastTags<TagGroup.Global>.Parse("fetch");

	// Token: 0x040035D1 RID: 13777
	public static FastTags<TagGroup.Global> craftingTag = FastTags<TagGroup.Global>.Parse("crafting");

	// Token: 0x040035D2 RID: 13778
	public static FastTags<TagGroup.Global> restorePowerTag = FastTags<TagGroup.Global>.Parse("restore_power");

	// Token: 0x040035D3 RID: 13779
	public static FastTags<TagGroup.Global> infestedTag = FastTags<TagGroup.Global>.Parse("infested");

	// Token: 0x040035D4 RID: 13780
	public static FastTags<TagGroup.Global> banditTag = FastTags<TagGroup.Global>.Parse("bandit");

	// Token: 0x040035D5 RID: 13781
	public static FastTags<TagGroup.Global> allQuestTags = FastTags<TagGroup.Global>.CombineTags(FastTags<TagGroup.Global>.CombineTags(QuestEventManager.traderTag, QuestEventManager.clearTag, QuestEventManager.treasureTag, QuestEventManager.fetchTag), FastTags<TagGroup.Global>.CombineTags(QuestEventManager.craftingTag, QuestEventManager.restorePowerTag), FastTags<TagGroup.Global>.CombineTags(QuestEventManager.infestedTag, QuestEventManager.banditTag));

	// Token: 0x040035D6 RID: 13782
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cTreasurePointAttempts = 5;

	// Token: 0x040035D7 RID: 13783
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cTreasurePointDistanceAdd = 50f;

	// Token: 0x040035D8 RID: 13784
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cTreasurePointMaxDistanceAdd = 500f;

	// Token: 0x02000906 RID: 2310
	public enum POILockoutReasonTypes
	{
		// Token: 0x040035FF RID: 13823
		None,
		// Token: 0x04003600 RID: 13824
		PlayerInside,
		// Token: 0x04003601 RID: 13825
		Bedroll,
		// Token: 0x04003602 RID: 13826
		LandClaim,
		// Token: 0x04003603 RID: 13827
		QuestLock
	}

	// Token: 0x02000907 RID: 2311
	public class PrefabListData
	{
		// Token: 0x060044CA RID: 17610 RVA: 0x001B8DF4 File Offset: 0x001B6FF4
		public void AddPOI(PrefabInstance poi)
		{
			int difficultyTier = (int)poi.prefab.DifficultyTier;
			if (!this.TierData.ContainsKey(difficultyTier))
			{
				this.TierData.Add(difficultyTier, new List<PrefabInstance>());
			}
			this.TierData[difficultyTier].Add(poi);
		}

		// Token: 0x060044CB RID: 17611 RVA: 0x001B8E40 File Offset: 0x001B7040
		public void ShuffleDifficulty(int difficulty, GameRandom gameRandom)
		{
			if (this.TierData.ContainsKey(difficulty))
			{
				List<PrefabInstance> list = this.TierData[difficulty];
				for (int i = 0; i < list.Count * 2; i++)
				{
					int index = gameRandom.RandomRange(list.Count);
					int index2 = gameRandom.RandomRange(list.Count);
					PrefabInstance value = list[index2];
					list[index2] = list[index];
					list[index] = value;
				}
			}
		}

		// Token: 0x04003604 RID: 13828
		public Dictionary<int, List<PrefabInstance>> TierData = new Dictionary<int, List<PrefabInstance>>();
	}
}
