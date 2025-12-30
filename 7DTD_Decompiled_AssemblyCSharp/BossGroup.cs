using System;
using System.Collections.Generic;
using Audio;
using GameEvent.SequenceActions;
using UnityEngine;

// Token: 0x020004B6 RID: 1206
public class BossGroup
{
	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x0600278C RID: 10124 RVA: 0x000FFB3C File Offset: 0x000FDD3C
	public string GetBossNavClass
	{
		get
		{
			BossGroup.BossGroupTypes currentGroupType = this.CurrentGroupType;
			if (currentGroupType == BossGroup.BossGroupTypes.ImmortalBoss)
			{
				return "twitch_vote_boss_shield";
			}
			if (currentGroupType != BossGroup.BossGroupTypes.Specialized)
			{
				return "twitch_vote_boss";
			}
			return "";
		}
	}

	// Token: 0x17000412 RID: 1042
	// (get) Token: 0x0600278D RID: 10125 RVA: 0x000FFB6C File Offset: 0x000FDD6C
	public string GetMinionNavClass
	{
		get
		{
			BossGroup.BossGroupTypes currentGroupType = this.CurrentGroupType;
			if (currentGroupType == BossGroup.BossGroupTypes.ImmortalMinions)
			{
				return "twitch_vote_minion_shield";
			}
			if (currentGroupType != BossGroup.BossGroupTypes.Specialized)
			{
				return "twitch_vote_minion";
			}
			return "";
		}
	}

	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x0600278E RID: 10126 RVA: 0x000FFB9B File Offset: 0x000FDD9B
	public int MinionCount
	{
		get
		{
			if (this.MinionEntityIDs != null)
			{
				return this.MinionEntityIDs.Count;
			}
			return 0;
		}
	}

	// Token: 0x0600278F RID: 10127 RVA: 0x000FFBB4 File Offset: 0x000FDDB4
	public BossGroup(EntityPlayer target, EntityAlive boss, List<EntityAlive> minions, BossGroup.BossGroupTypes bossGroupType, string bossIcon)
	{
		this.CurrentGroupType = bossGroupType;
		this.TargetPlayer = target;
		this.BossEntity = boss;
		this.MinionEntities = minions;
		this.BossName = Localization.Get(EntityClass.list[this.BossEntity.entityClass].entityClassName, false);
		this.BossEntityID = boss.entityId;
		this.MinionEntityIDs = new List<int>();
		for (int i = 0; i < minions.Count; i++)
		{
			this.MinionEntityIDs.Add(minions[i].entityId);
		}
		this.BossIcon = bossIcon;
		this.BossGroupID = ++BossGroup.nextID;
		this.serverBounds.size = this.LeavingSize;
	}

	// Token: 0x06002790 RID: 10128 RVA: 0x000FFCF0 File Offset: 0x000FDEF0
	public BossGroup(int bossGroupID, BossGroup.BossGroupTypes bossGroupType, int bossEntityID, List<int> minionIDs, string bossIcon)
	{
		this.CurrentGroupType = bossGroupType;
		this.BossEntityID = bossEntityID;
		this.MinionEntityIDs = minionIDs;
		this.BossEntity = null;
		this.MinionEntities = null;
		this.BossIcon = bossIcon;
		this.BossGroupID = bossGroupID;
	}

	// Token: 0x06002791 RID: 10129 RVA: 0x000FFDB0 File Offset: 0x000FDFB0
	public void Update(EntityPlayerLocal player)
	{
		float num = -1f;
		EntityAlive entityAlive = null;
		if (this.BossEntity == null)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				this.BossEntity = (GameManager.Instance.World.GetEntity(this.BossEntityID) as EntityAlive);
				if (this.BossEntity != null)
				{
					if (this.BossName == "")
					{
						this.BossName = Localization.Get(EntityClass.list[this.BossEntity.entityClass].entityClassName, false);
					}
					if (this.BossEntity.IsAlive())
					{
						entityAlive = this.BossEntity;
					}
				}
			}
		}
		else if (this.BossEntity.IsAlive())
		{
			entityAlive = this.BossEntity;
		}
		if (entityAlive != null)
		{
			num = entityAlive.GetDistance(player);
		}
		if (this.MinionEntities != null)
		{
			for (int i = 0; i < this.MinionEntities.Count; i++)
			{
				if (this.MinionEntities[i] != null && this.MinionEntities[i].IsAlive())
				{
					float distance = this.MinionEntities[i].GetDistance(player);
					if (num == -1f || distance < num)
					{
						entityAlive = this.MinionEntities[i];
						num = distance;
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < this.MinionEntityIDs.Count; j++)
			{
				EntityAlive entityAlive2 = GameManager.Instance.World.GetEntity(this.MinionEntityIDs[j]) as EntityAlive;
				if (entityAlive2 != null && entityAlive2.IsAlive())
				{
					float distance2 = entityAlive2.GetDistance(player);
					if (num == -1f || distance2 < num)
					{
						entityAlive = entityAlive2;
						num = distance2;
					}
				}
			}
		}
		if (entityAlive == null)
		{
			this.ReadyForRemove = true;
			return;
		}
		this.ReadyForRemove = false;
		this.bounds.center = entityAlive.position;
		this.bounds.size = (this.IsCurrent ? this.LeavingSize : this.EnteringSize);
	}

	// Token: 0x06002792 RID: 10130 RVA: 0x000FFFB2 File Offset: 0x000FE1B2
	public bool IsPlayerWithinRange(EntityPlayer player)
	{
		return this.bounds.Contains(player.position);
	}

	// Token: 0x06002793 RID: 10131 RVA: 0x000FFFC5 File Offset: 0x000FE1C5
	public bool IsPlayerWithinServerRange(EntityPlayer player)
	{
		return this.serverBounds.Contains(player.position);
	}

	// Token: 0x06002794 RID: 10132 RVA: 0x000FFFD8 File Offset: 0x000FE1D8
	public void RemoveMinion(int entityID)
	{
		if (this.MinionEntityIDs != null)
		{
			this.MinionEntityIDs.Remove(entityID);
		}
		if (this.MinionEntities != null)
		{
			for (int i = this.MinionEntities.Count - 1; i >= 0; i--)
			{
				if (this.MinionEntities[i] != null && this.MinionEntities[i].entityId == entityID)
				{
					this.MinionEntities.RemoveAt(i);
				}
			}
		}
	}

	// Token: 0x06002795 RID: 10133 RVA: 0x00100050 File Offset: 0x000FE250
	public void AddNavObjects()
	{
		if (this.MinionEntities == null)
		{
			this.MinionEntities = new List<EntityAlive>();
			for (int i = 0; i < this.MinionEntityIDs.Count; i++)
			{
				this.MinionEntities.Add(GameManager.Instance.World.GetEntity(this.MinionEntityIDs[i]) as EntityAlive);
			}
		}
		if (this.BossEntity != null)
		{
			this.BossEntity.Buffs.AddBuff("twitch_give_navobject", -1, true, false, -1f);
		}
		if (this.MinionEntities != null)
		{
			for (int j = 0; j < this.MinionEntities.Count; j++)
			{
				if (this.MinionEntities[j] != null)
				{
					this.MinionEntities[j].Buffs.AddBuff("twitch_give_navobject", -1, true, false, -1f);
				}
			}
		}
	}

	// Token: 0x06002796 RID: 10134 RVA: 0x00100133 File Offset: 0x000FE333
	public void RequestStatRefresh()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageBossEvent>().Setup(NetPackageBossEvent.BossEventTypes.RequestStats, this.BossGroupID), false);
		}
	}

	// Token: 0x06002797 RID: 10135 RVA: 0x00100160 File Offset: 0x000FE360
	public void RefreshStats(int playerID)
	{
		if (this.MinionEntities == null)
		{
			this.MinionEntities = new List<EntityAlive>();
			for (int i = 0; i < this.MinionEntityIDs.Count; i++)
			{
				this.MinionEntities.Add(GameManager.Instance.World.GetEntity(this.MinionEntityIDs[i]) as EntityAlive);
			}
		}
		if (this.BossEntity != null)
		{
			this.BossEntity.bPlayerStatsChanged = true;
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityStatChanged>().Setup(this.BossEntity, playerID, NetPackageEntityStatChanged.EnumStat.Health), false, -1, -1, -1, null, 192, false);
		}
		if (this.MinionEntities != null)
		{
			for (int j = 0; j < this.MinionEntities.Count; j++)
			{
				if (this.MinionEntities[j] != null)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityStatChanged>().Setup(this.MinionEntities[j], playerID, NetPackageEntityStatChanged.EnumStat.Health), false, -1, -1, -1, null, 192, false);
				}
			}
		}
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x00100274 File Offset: 0x000FE474
	public void RemoveNavObjects()
	{
		if (this.BossEntity != null)
		{
			this.BossEntity.RemoveNavObject("twitch_vote_boss");
			this.BossEntity.RemoveNavObject("twitch_vote_boss_shield");
		}
		if (this.MinionEntities != null)
		{
			for (int i = 0; i < this.MinionEntities.Count; i++)
			{
				if (this.MinionEntities[i] != null)
				{
					this.MinionEntities[i].RemoveNavObject("twitch_vote_minion");
					this.MinionEntities[i].RemoveNavObject("twitch_vote_minion_shield");
				}
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				this.MinionEntities = null;
			}
		}
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x00100320 File Offset: 0x000FE520
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupTeleportList()
	{
		this.ClosestEnemy = this.GetClosestEntity(this.TargetPlayer);
		if (this.ClosestEnemy == null)
		{
			return;
		}
		this.serverBounds.center = this.ClosestEnemy.position;
		for (int i = 0; i < this.MinionEntities.Count; i++)
		{
			if (!(this.MinionEntities[i] == null) && this.MinionEntities[i].IsAlive() && !(this.MinionEntities[i] == this.ClosestEnemy))
			{
				EntityAlive entityAlive = this.MinionEntities[i];
				if (Vector3.Distance(this.ClosestEnemy.position, this.MinionEntities[i].position) > BossGroup.autoPullDistance)
				{
					if (!this.TeleportList.Contains(this.MinionEntities[i]))
					{
						this.TeleportList.Add(this.MinionEntities[i]);
					}
				}
				else if (this.TeleportList.Contains(this.MinionEntities[i]))
				{
					this.TeleportList.Remove(this.MinionEntities[i]);
				}
			}
		}
		if (this.ClosestEnemy == this.BossEntity || this.BossEntity == null || !this.BossEntity.IsAlive())
		{
			return;
		}
		if (Vector3.Distance(this.ClosestEnemy.position, this.BossEntity.position) > BossGroup.autoPullDistance)
		{
			if (!this.TeleportList.Contains(this.BossEntity))
			{
				this.TeleportList.Add(this.BossEntity);
				return;
			}
		}
		else if (this.TeleportList.Contains(this.BossEntity))
		{
			this.TeleportList.Remove(this.BossEntity);
		}
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x001004FC File Offset: 0x000FE6FC
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive GetClosestEntity(EntityPlayer player)
	{
		EntityAlive result = null;
		float num = -1f;
		for (int i = this.MinionEntities.Count - 1; i >= 0; i--)
		{
			if (this.MinionEntities[i] != null && this.MinionEntities[i].IsAlive())
			{
				float num2 = Vector3.Distance(this.TargetPlayer.position, this.MinionEntities[i].position);
				if (num > num2 || num == -1f)
				{
					num = num2;
					result = this.MinionEntities[i];
				}
			}
		}
		if (this.BossEntity != null && this.BossEntity.IsAlive())
		{
			float num2 = Vector3.Distance(this.TargetPlayer.position, this.BossEntity.position);
			if (num > num2 || num == -1f)
			{
				result = this.BossEntity;
			}
		}
		return result;
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x001005E0 File Offset: 0x000FE7E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleTeleportList()
	{
		for (int i = this.TeleportList.Count - 1; i >= 0; i--)
		{
			EntityAlive entityAlive = this.TeleportList[i];
			Vector3 zero = Vector3.zero;
			if (ActionBaseSpawn.FindValidPosition(out zero, this.ClosestEnemy.position, 3f, 6f, true, 0f, false, 0f))
			{
				if (this.pullSound != "")
				{
					Manager.BroadcastPlayByLocalPlayer(entityAlive.position, this.pullSound);
				}
				entityAlive.SetPosition(zero, true);
				entityAlive.SetAttackTarget(this.TargetPlayer, 12000);
				this.TeleportList.RemoveAt(i);
				if (this.pullSound != "")
				{
					Manager.BroadcastPlayByLocalPlayer(zero, this.pullSound);
				}
			}
		}
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x001006AE File Offset: 0x000FE8AE
	public void HandleAutoPull()
	{
		if (this.TeleportList.Count > 0)
		{
			this.HandleTeleportList();
		}
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x001006C4 File Offset: 0x000FE8C4
	public void HandleLiveHandling()
	{
		this.liveTime += Time.deltaTime;
		this.attackTime -= Time.deltaTime;
		if (this.liveTime > 5f && !this.IsPlayerWithinServerRange(this.TargetPlayer))
		{
			this.RemoveNavObjects();
			this.DespawnAll();
		}
		if (this.attackTime <= 0f)
		{
			this.HandleAttackTrigger();
			this.attackTime = 5f;
		}
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x0010073C File Offset: 0x000FE93C
	public bool ServerUpdate()
	{
		bool flag = false;
		this.SetupTeleportList();
		for (int i = this.MinionEntities.Count - 1; i >= 0; i--)
		{
			if (this.MinionEntities[i] == null || !this.MinionEntities[i].IsAlive())
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageBossEvent>().Setup(NetPackageBossEvent.BossEventTypes.RemoveMinion, this.BossGroupID, this.MinionEntities[i].entityId), false, -1, -1, -1, null, 192, false);
				this.MinionEntityIDs.Remove(this.MinionEntities[i].entityId);
				this.MinionEntities.RemoveAt(i);
			}
			else
			{
				flag = true;
			}
		}
		if (this.BossEntity != null && this.BossEntity.IsAlive())
		{
			flag = true;
		}
		if (!flag)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageBossEvent>().Setup(NetPackageBossEvent.BossEventTypes.RemoveGroup, this.BossGroupID), false, -1, -1, -1, null, 192, false);
		}
		return !flag;
	}

	// Token: 0x0600279F RID: 10143 RVA: 0x00100854 File Offset: 0x000FEA54
	public void DespawnAll()
	{
		for (int i = this.MinionEntities.Count - 1; i >= 0; i--)
		{
			if (this.MinionEntities[i] != null && this.MinionEntities[i].IsAlive())
			{
				this.MinionEntities[i].ForceDespawn();
			}
		}
		if (this.BossEntity != null && this.BossEntity.IsAlive())
		{
			this.BossEntity.ForceDespawn();
		}
	}

	// Token: 0x060027A0 RID: 10144 RVA: 0x001008D8 File Offset: 0x000FEAD8
	public void HandleAttackTrigger()
	{
		for (int i = this.MinionEntities.Count - 1; i >= 0; i--)
		{
			if (this.MinionEntities[i] != null && this.MinionEntities[i].IsAlive() && this.MinionEntities[i].GetAttackTarget() == null)
			{
				this.MinionEntities[i].SetAttackTarget(this.TargetPlayer, 60000);
			}
		}
		if (this.BossEntity != null && this.BossEntity.IsAlive() && this.BossEntity.GetAttackTarget() == null)
		{
			this.BossEntity.SetAttackTarget(this.TargetPlayer, 60000);
		}
	}

	// Token: 0x04001E1C RID: 7708
	public int BossGroupID = -1;

	// Token: 0x04001E1D RID: 7709
	public int BossEntityID = -1;

	// Token: 0x04001E1E RID: 7710
	public EntityAlive BossEntity;

	// Token: 0x04001E1F RID: 7711
	public List<int> MinionEntityIDs;

	// Token: 0x04001E20 RID: 7712
	public List<EntityAlive> MinionEntities;

	// Token: 0x04001E21 RID: 7713
	public EntityPlayer TargetPlayer;

	// Token: 0x04001E22 RID: 7714
	public BossGroup.BossGroupTypes CurrentGroupType;

	// Token: 0x04001E23 RID: 7715
	public string BossIcon = "";

	// Token: 0x04001E24 RID: 7716
	public string BossName = "";

	// Token: 0x04001E25 RID: 7717
	[PublicizedFrom(EAccessModifier.Private)]
	public Bounds serverBounds;

	// Token: 0x04001E26 RID: 7718
	[PublicizedFrom(EAccessModifier.Private)]
	public Bounds bounds;

	// Token: 0x04001E27 RID: 7719
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 EnteringSize = new Vector3(32f, 32f, 32f);

	// Token: 0x04001E28 RID: 7720
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 LeavingSize = new Vector3(200f, 200f, 200f);

	// Token: 0x04001E29 RID: 7721
	[PublicizedFrom(EAccessModifier.Private)]
	public static float autoPullDistance = 32f;

	// Token: 0x04001E2A RID: 7722
	[PublicizedFrom(EAccessModifier.Private)]
	public static int nextID = -1;

	// Token: 0x04001E2B RID: 7723
	public bool IsCurrent;

	// Token: 0x04001E2C RID: 7724
	public bool ReadyForRemove;

	// Token: 0x04001E2D RID: 7725
	public string pullSound = "twitch_pull";

	// Token: 0x04001E2E RID: 7726
	[PublicizedFrom(EAccessModifier.Private)]
	public float liveTime;

	// Token: 0x04001E2F RID: 7727
	[PublicizedFrom(EAccessModifier.Private)]
	public float attackTime = 5f;

	// Token: 0x04001E30 RID: 7728
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive ClosestEnemy;

	// Token: 0x04001E31 RID: 7729
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityAlive> TeleportList = new List<EntityAlive>();

	// Token: 0x020004B7 RID: 1207
	public enum BossGroupTypes
	{
		// Token: 0x04001E33 RID: 7731
		Standard,
		// Token: 0x04001E34 RID: 7732
		ImmortalBoss,
		// Token: 0x04001E35 RID: 7733
		ImmortalMinions,
		// Token: 0x04001E36 RID: 7734
		Specialized
	}
}
