using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform;
using Twitch;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000455 RID: 1109
[Preserve]
public class EntityPlayer : EntityAlive
{
	// Token: 0x170003BE RID: 958
	// (get) Token: 0x060022CE RID: 8910 RVA: 0x000DB6CC File Offset: 0x000D98CC
	public string PlayerDisplayName
	{
		get
		{
			if (this.cachedPlayerName != null)
			{
				return this.cachedPlayerName.DisplayName;
			}
			PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.entityId);
			if (playerDataFromEntityID == null)
			{
				return null;
			}
			this.cachedPlayerName = playerDataFromEntityID.PlayerName;
			return this.cachedPlayerName.DisplayName;
		}
	}

	// Token: 0x170003BF RID: 959
	// (get) Token: 0x060022CF RID: 8911 RVA: 0x000DB71F File Offset: 0x000D991F
	// (set) Token: 0x060022D0 RID: 8912 RVA: 0x000DB728 File Offset: 0x000D9928
	public bool TwitchEnabled
	{
		get
		{
			return this.twitchEnabled;
		}
		set
		{
			if (value != this.twitchEnabled)
			{
				this.twitchEnabled = value;
				this.bPlayerTwitchChanged |= !this.isEntityRemote;
				if (TwitchManager.HasInstance && TwitchManager.Current.extensionManager != null)
				{
					TwitchManager.Current.extensionManager.TwitchEnabledChanged(this);
				}
			}
		}
	}

	// Token: 0x170003C0 RID: 960
	// (get) Token: 0x060022D1 RID: 8913 RVA: 0x000DB77E File Offset: 0x000D997E
	// (set) Token: 0x060022D2 RID: 8914 RVA: 0x000DB788 File Offset: 0x000D9988
	public bool TwitchSafe
	{
		get
		{
			return this.twitchSafe;
		}
		set
		{
			if (value != this.twitchSafe)
			{
				this.twitchSafe = value;
				this.bPlayerTwitchChanged |= !this.isEntityRemote;
				if (this.twitchSafe)
				{
					this.Buffs.AddBuff("twitch_safe", -1, true, false, -1f);
					return;
				}
				this.Buffs.RemoveBuff("twitch_safe", true);
			}
		}
	}

	// Token: 0x170003C1 RID: 961
	// (get) Token: 0x060022D3 RID: 8915 RVA: 0x000DB7EE File Offset: 0x000D99EE
	// (set) Token: 0x060022D4 RID: 8916 RVA: 0x000DB7F6 File Offset: 0x000D99F6
	public TwitchVoteLockTypes TwitchVoteLock
	{
		get
		{
			return this.twitchVoteLock;
		}
		set
		{
			if (value != this.twitchVoteLock)
			{
				this.twitchVoteLock = value;
				this.bPlayerTwitchChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x170003C2 RID: 962
	// (get) Token: 0x060022D5 RID: 8917 RVA: 0x000DB81E File Offset: 0x000D9A1E
	// (set) Token: 0x060022D6 RID: 8918 RVA: 0x000DB826 File Offset: 0x000D9A26
	public bool TwitchVisionDisabled
	{
		get
		{
			return this.twitchVisionDisabled;
		}
		set
		{
			if (value != this.twitchVisionDisabled)
			{
				this.twitchVisionDisabled = value;
				this.bPlayerTwitchChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x060022D7 RID: 8919 RVA: 0x000DB84E File Offset: 0x000D9A4E
	// (set) Token: 0x060022D8 RID: 8920 RVA: 0x000DB856 File Offset: 0x000D9A56
	public EntityPlayer.TwitchActionsStates TwitchActionsEnabled
	{
		get
		{
			return this.twitchActionsEnabled;
		}
		set
		{
			if (value != this.twitchActionsEnabled)
			{
				this.twitchActionsEnabled = value;
				this.bPlayerTwitchChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x060022D9 RID: 8921 RVA: 0x000DB87E File Offset: 0x000D9A7E
	// (set) Token: 0x060022DA RID: 8922 RVA: 0x000DB886 File Offset: 0x000D9A86
	public bool IsSpectator
	{
		get
		{
			return this.isSpectator;
		}
		set
		{
			this.isSpectator = value;
			this.isIgnoredByAI = this.isSpectator;
			this.SetVisible(this.bModelVisible);
			this.bPlayerStatsChanged |= !this.isEntityRemote;
		}
	}

	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x060022DB RID: 8923 RVA: 0x000DB8BD File Offset: 0x000D9ABD
	// (set) Token: 0x060022DC RID: 8924 RVA: 0x000DB8C8 File Offset: 0x000D9AC8
	public Vector3i markerPosition
	{
		get
		{
			return this.m_MarkerPosition;
		}
		set
		{
			if (!this.isEntityRemote)
			{
				if (value.Equals(Vector3i.zero))
				{
					if (this.navMarker != null)
					{
						NavObjectManager.Instance.UnRegisterNavObject(this.navMarker);
						this.navMarker = null;
					}
				}
				else if (this.navMarker == null)
				{
					this.navMarker = NavObjectManager.Instance.RegisterNavObject("quick_waypoint", value.ToVector3(), "", this.navMarkerHidden, -1, null);
				}
				else
				{
					this.navMarker.TrackedPosition = value.ToVector3();
					this.navMarker.hiddenOnCompass = this.navMarkerHidden;
				}
				this.m_MarkerPosition = value;
			}
		}
	}

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x060022DD RID: 8925 RVA: 0x000DB96C File Offset: 0x000D9B6C
	// (remove) Token: 0x060022DE RID: 8926 RVA: 0x000DB9A4 File Offset: 0x000D9BA4
	public event QuestJournal_QuestEvent QuestAccepted;

	// Token: 0x1400001F RID: 31
	// (add) Token: 0x060022DF RID: 8927 RVA: 0x000DB9DC File Offset: 0x000D9BDC
	// (remove) Token: 0x060022E0 RID: 8928 RVA: 0x000DBA14 File Offset: 0x000D9C14
	public event QuestJournal_QuestEvent QuestChanged;

	// Token: 0x14000020 RID: 32
	// (add) Token: 0x060022E1 RID: 8929 RVA: 0x000DBA4C File Offset: 0x000D9C4C
	// (remove) Token: 0x060022E2 RID: 8930 RVA: 0x000DBA84 File Offset: 0x000D9C84
	public event QuestJournal_QuestEvent QuestRemoved;

	// Token: 0x14000021 RID: 33
	// (add) Token: 0x060022E3 RID: 8931 RVA: 0x000DBABC File Offset: 0x000D9CBC
	// (remove) Token: 0x060022E4 RID: 8932 RVA: 0x000DBAF4 File Offset: 0x000D9CF4
	public event QuestJournal_QuestSharedEvent SharedQuestAdded;

	// Token: 0x14000022 RID: 34
	// (add) Token: 0x060022E5 RID: 8933 RVA: 0x000DBB2C File Offset: 0x000D9D2C
	// (remove) Token: 0x060022E6 RID: 8934 RVA: 0x000DBB64 File Offset: 0x000D9D64
	public event QuestJournal_QuestSharedEvent SharedQuestRemoved;

	// Token: 0x060022E7 RID: 8935 RVA: 0x000DBB99 File Offset: 0x000D9D99
	public void TriggerQuestAddedEvent(Quest _q)
	{
		QuestJournal_QuestEvent questAccepted = this.QuestAccepted;
		if (questAccepted == null)
		{
			return;
		}
		questAccepted(_q);
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x000DBBAC File Offset: 0x000D9DAC
	public void TriggerQuestChangedEvent(Quest _q)
	{
		QuestJournal_QuestEvent questChanged = this.QuestChanged;
		if (questChanged == null)
		{
			return;
		}
		questChanged(_q);
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x000DBBBF File Offset: 0x000D9DBF
	public void TriggerQuestRemovedEvent(Quest _q)
	{
		QuestJournal_QuestEvent questRemoved = this.QuestRemoved;
		if (questRemoved == null)
		{
			return;
		}
		questRemoved(_q);
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000DBBD2 File Offset: 0x000D9DD2
	public void TriggerSharedQuestAddedEvent(SharedQuestEntry _entry)
	{
		if (this.SharedQuestAdded != null)
		{
			this.SharedQuestAdded(_entry);
			return;
		}
		Log.Warning(string.Format("No SharedQuestAdded listeners! Player: {0}", this));
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x000DBBF9 File Offset: 0x000D9DF9
	public void TriggerSharedQuestRemovedEvent(SharedQuestEntry _entry)
	{
		QuestJournal_QuestSharedEvent sharedQuestRemoved = this.SharedQuestRemoved;
		if (sharedQuestRemoved == null)
		{
			return;
		}
		sharedQuestRemoved(_entry);
	}

	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x060022EC RID: 8940 RVA: 0x000DBC0C File Offset: 0x000D9E0C
	// (set) Token: 0x060022ED RID: 8941 RVA: 0x000DBC14 File Offset: 0x000D9E14
	public Vector3i RentedVMPosition
	{
		get
		{
			return this.m_rentedVMPosition;
		}
		set
		{
			if (!this.isEntityRemote)
			{
				if (value.Equals(Vector3i.zero))
				{
					if (this.navVending != null)
					{
						NavObjectManager.Instance.UnRegisterNavObject(this.navVending);
						this.navVending = null;
					}
				}
				else if (this.navVending == null)
				{
					this.navVending = NavObjectManager.Instance.RegisterNavObject("vending_machine", value.ToVector3(), "", false, -1, null);
				}
				else
				{
					this.navVending.TrackedPosition = value.ToVector3();
				}
				this.m_rentedVMPosition = value;
			}
		}
	}

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x060022EE RID: 8942 RVA: 0x000DBCA0 File Offset: 0x000D9EA0
	// (set) Token: 0x060022EF RID: 8943 RVA: 0x000DBD03 File Offset: 0x000D9F03
	public bool IsAdmin
	{
		get
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return this.isAdmin;
			}
			if (!this.isEntityRemote)
			{
				return true;
			}
			ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(this.entityId);
			AdminTools adminTools = GameManager.Instance.adminTools;
			return ((adminTools != null) ? adminTools.Users.GetUserPermissionLevel(clientInfo) : 1000) == 0;
		}
		set
		{
			if (value != this.isAdmin)
			{
				this.isAdmin = value;
			}
		}
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x000DBD15 File Offset: 0x000D9F15
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.Progression = new Progression(this);
		this.bWillRespawn = true;
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x000DBD30 File Offset: 0x000D9F30
	public override void Init(int _entityClass)
	{
		this.gameStageBornAtWorldTime = ulong.MaxValue;
		if (this.playerProfile == null)
		{
			this.playerProfile = PlayerProfile.LoadLocalProfile();
		}
		this.Stealth.Init(this);
		this.alertEnabled = false;
		base.Init(_entityClass);
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000DBD67 File Offset: 0x000D9F67
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitStats()
	{
		this.entityStats = new PlayerEntityStats(this);
	}

	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x060022F3 RID: 8947 RVA: 0x000DBD75 File Offset: 0x000D9F75
	public PlayerEntityStats PlayerStats
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (PlayerEntityStats)this.entityStats;
		}
	}

	// Token: 0x060022F4 RID: 8948 RVA: 0x000DBD82 File Offset: 0x000D9F82
	public override void CopyPropertiesFromEntityClass()
	{
		base.CopyPropertiesFromEntityClass();
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000DBD8A File Offset: 0x000D9F8A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		this.SetVisible(this.Spawned);
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000DBDA0 File Offset: 0x000D9FA0
	public override void SetAlive()
	{
		bool flag = this.IsDead();
		base.SetAlive();
		if (flag)
		{
			long num = GameStageDefinition.DaysAliveChangeWhenKilled * 24000L;
			if (this.world.worldTime - this.gameStageBornAtWorldTime < (ulong)num)
			{
				this.gameStageBornAtWorldTime = this.world.worldTime;
				return;
			}
			this.gameStageBornAtWorldTime += (ulong)num;
		}
	}

	// Token: 0x060022F7 RID: 8951 RVA: 0x000DBDFD File Offset: 0x000D9FFD
	public override void SetDead()
	{
		base.SetDead();
		if (this.world.aiDirector != null)
		{
			this.IsBloodMoonDead = this.world.aiDirector.BloodMoonComponent.BloodMoonActive;
		}
	}

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x060022F8 RID: 8952 RVA: 0x000DBE30 File Offset: 0x000DA030
	public int unModifiedGameStage
	{
		get
		{
			float num = Mathf.Clamp((float)((this.world.worldTime - this.gameStageBornAtWorldTime) / 24000UL), 0f, (float)this.Progression.Level);
			float @float = GameStats.GetFloat(EnumGameStats.GameDifficultyBonus);
			return Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.GameStage, null, ((float)this.Progression.Level + num) * @float, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		}
	}

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x060022F9 RID: 8953 RVA: 0x000DBEAC File Offset: 0x000DA0AC
	public int gameStage
	{
		get
		{
			float num = Mathf.Clamp((float)((this.world.worldTime - this.gameStageBornAtWorldTime) / 24000UL), 0f, (float)this.Progression.Level);
			float @float = GameStats.GetFloat(EnumGameStats.GameDifficultyBonus);
			if (this.biomeStandingOn != null)
			{
				float num2 = 0f;
				float num3 = 0f;
				if (this.QuestJournal.ActiveQuest != null)
				{
					num2 = this.QuestJournal.ActiveQuest.QuestClass.GameStageMod;
					num3 = this.QuestJournal.ActiveQuest.QuestClass.GameStageBonus;
				}
				return Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.GameStage, null, ((float)this.Progression.Level * (1f + this.biomeStandingOn.GameStageMod + num2) + num + this.biomeStandingOn.GameStageBonus + num3) * @float, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
			}
			return Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.GameStage, null, ((float)this.Progression.Level + num) * @float, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		}
	}

	// Token: 0x060022FA RID: 8954 RVA: 0x000DBFD0 File Offset: 0x000DA1D0
	public int GetTraderStage(int tier)
	{
		GameStats.GetFloat(EnumGameStats.GameDifficultyBonus);
		int a = Mathf.Max(0, tier - 1);
		float num = TraderManager.QuestTierMod[Mathf.Min(a, TraderManager.QuestTierMod.Length - 1)];
		return Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.TraderStage, null, (float)this.Progression.Level * (1f + num), this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x000DC03C File Offset: 0x000DA23C
	public int GetLootStage(float containerMod, float containerBonus)
	{
		float num = 0f;
		float num2 = 0f;
		if (this.prefab != null && this.prefab.prefab.DifficultyTier > 0)
		{
			int a = Mathf.Max(0, (int)(this.prefab.prefab.DifficultyTier - 1));
			num = LootManager.POITierMod[Mathf.Min(a, LootManager.POITierMod.Length - 1)];
			num2 = LootManager.POITierBonus[Mathf.Min(a, LootManager.POITierBonus.Length - 1)];
		}
		if (this.biomeStandingOn != null)
		{
			int num3 = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.LootStage, null, (float)this.Progression.Level * (1f + num + this.biomeStandingOn.LootStageMod + containerMod) + (num2 + this.biomeStandingOn.LootStageBonus + containerBonus), this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
			if (this.biomeStandingOn.LootStageMin != -1)
			{
				num3 = Utils.FastMax(num3, this.biomeStandingOn.LootStageMin);
			}
			if (GameStats.GetBool(EnumGameStats.BiomeProgression) && this.biomeStandingOn.LootStageMax != -1)
			{
				num3 = Utils.FastMin(num3, Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.LootStageMax, null, (float)this.biomeStandingOn.LootStageMax, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)));
			}
			return num3;
		}
		return Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.LootStage, null, (float)this.Progression.Level * (1f + num + containerMod) + (num2 + containerBonus), this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x000DC1C5 File Offset: 0x000DA3C5
	public int GetHighestPartyLootStage(float containerMod, float containerBonus)
	{
		if (this.Party != null)
		{
			return this.Party.GetHighestLootStage(containerMod, containerBonus);
		}
		return this.GetLootStage(containerMod, containerBonus);
	}

	// Token: 0x170003CB RID: 971
	// (get) Token: 0x060022FD RID: 8957 RVA: 0x000DC1E5 File Offset: 0x000DA3E5
	public int HighestPartyGameStage
	{
		get
		{
			if (this.Party != null)
			{
				return this.Party.HighestGameStage;
			}
			return this.gameStage;
		}
	}

	// Token: 0x170003CC RID: 972
	// (get) Token: 0x060022FE RID: 8958 RVA: 0x000DC201 File Offset: 0x000DA401
	public int PartyGameStage
	{
		get
		{
			if (this.Party != null)
			{
				return this.Party.GameStage;
			}
			return this.gameStage;
		}
	}

	// Token: 0x060022FF RID: 8959 RVA: 0x000DC21D File Offset: 0x000DA41D
	public void TurnOffLightFlares()
	{
		this.inventory.TurnOffLightFlares();
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x000DB5F8 File Offset: 0x000D97F8
	public override float GetSeeDistance()
	{
		return 80f;
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x000DC22C File Offset: 0x000DA42C
	public float DetectUsScale(EntityAlive _entity)
	{
		if (this.prefab != null && this.prefab.prefab.DifficultyTier >= 1 && Time.time - this.prefabTimeIn > 60f && _entity.GetSpawnerSource() == EnumSpawnerSource.Biome && _entity is EntityEnemy)
		{
			return 0.3f;
		}
		return 1f;
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000DC284 File Offset: 0x000DA484
	public override Vector3 getHeadPosition()
	{
		if (!(this.emodel != null) || !(this.emodel.GetHeadTransform() != null))
		{
			return base.transform.position + new Vector3(0f, base.height - 0.15f, 0f) + Origin.position;
		}
		return this.emodel.GetHeadTransform().position + Origin.position;
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x000DC304 File Offset: 0x000DA504
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		this.generalTags = this.MinEventContext.Tags;
		if (!GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		float num = this.totalTimePlayed + Time.unscaledDeltaTime / 60f;
		if (this is EntityPlayerLocal)
		{
			int num2 = (int)this.totalTimePlayed;
			int num3 = (int)num;
			if (num2 != num3 && num3 % 60 == 0)
			{
				int num4 = num3 / 60;
				if (num4 < 301)
				{
					GameSparksCollector.SetValue(GameSparksCollector.GSDataKey.PlayerLevelAtHour, num4.ToString(), this.Progression.Level, true, GameSparksCollector.GSDataCollection.SessionUpdates);
				}
			}
		}
		this.totalTimePlayed = num;
		if (this.ChunkObserver != null)
		{
			this.ChunkObserver.SetPosition(base.GetPosition());
			if (this.ChunkObserver.mapDatabase != null && this.IsSpawned() && this.chunkPosAddedEntityTo != this.lastChunkPos)
			{
				this.lastChunkPos = this.chunkPosAddedEntityTo;
				this.ChunkObserver.mapDatabase.Add(this.chunkPosAddedEntityTo, this.world);
			}
		}
		if (this.emodel.avatarController != null)
		{
			this.emodel.avatarController.SetHeadAngles(this.rotation.x, 0f);
			if (this.inventory.holdingItem != null && this.inventory.holdingItem.CanHold())
			{
				this.emodel.avatarController.SetArmsAngles(this.rotation.x + 90f, 0f);
			}
			else
			{
				this.emodel.avatarController.SetArmsAngles(0f, 0f);
			}
		}
		if (!this.IsDead())
		{
			this.currentLife += Time.deltaTime / 60f;
			if (this.currentLife > this.longestLife)
			{
				this.longestLife = this.currentLife;
				if ((int)this.longestLife > this.longestLifeLived)
				{
					this.longestLifeLived = (int)this.longestLife;
					if (this is EntityPlayerLocal)
					{
						QuestEventManager.Current.TimeSurvived((float)this.longestLifeLived);
						IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
						if (achievementManager != null)
						{
							achievementManager.SetAchievementStat(EnumAchievementDataStat.LongestLifeLived, this.longestLifeLived);
						}
					}
				}
			}
		}
		this.HasUpdated = true;
	}

	// Token: 0x06002304 RID: 8964 RVA: 0x000DC52C File Offset: 0x000DA72C
	public override float GetSpeedModifier()
	{
		float num;
		float num2;
		if (base.IsCrouching)
		{
			if (this.MovementRunning)
			{
				num = Constants.cPlayerSpeedModifierWalking;
				num2 = EffectManager.GetValue(PassiveEffects.WalkSpeed, null, num, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
			else
			{
				num = Constants.cPlayerSpeedModifierCrouching;
				num2 = EffectManager.GetValue(PassiveEffects.CrouchSpeed, null, num, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
		}
		else if (this.MovementRunning)
		{
			num = Constants.cPlayerSpeedModifierRunning;
			num2 = EffectManager.GetValue(PassiveEffects.RunSpeed, null, num, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		else
		{
			num = Constants.cPlayerSpeedModifierWalking;
			num2 = EffectManager.GetValue(PassiveEffects.WalkSpeed, null, num, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		num *= 0.35f;
		if (num2 < num)
		{
			num2 = num;
		}
		return num2;
	}

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x06002305 RID: 8965 RVA: 0x000DC5FE File Offset: 0x000DA7FE
	public override float MaxVelocity
	{
		get
		{
			if (this.MovementRunning)
			{
				return 0.35f;
			}
			return 0.17999f;
		}
	}

	// Token: 0x06002306 RID: 8966 RVA: 0x000DC613 File Offset: 0x000DA813
	public override Vector3 GetVelocityPerSecond()
	{
		if (this.AttachedToEntity)
		{
			return this.AttachedToEntity.GetVelocityPerSecond();
		}
		return this.averageVel * 20f;
	}

	// Token: 0x06002307 RID: 8967 RVA: 0x000DC63E File Offset: 0x000DA83E
	public Color GetTeamColor()
	{
		return Constants.cTeamColors[this.TeamNumber];
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x000DC650 File Offset: 0x000DA850
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void StartJumpMotion()
	{
		base.StartJumpMotion();
		this.motion.y = EffectManager.GetValue(PassiveEffects.JumpStrength, null, this.jumpStrength, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * base.Stats.Stamina.ValuePercent;
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x000DC6A3 File Offset: 0x000DA8A3
	public override void OnUpdateLive()
	{
		base.Stats.Stamina.RegenerationAmount = 0f;
		base.OnUpdateLive();
		base.GetEntitySenses().Clear();
		this.CheckSleeperTriggers();
	}

	// Token: 0x0600230A RID: 8970 RVA: 0x000DC6D1 File Offset: 0x000DA8D1
	[PublicizedFrom(EAccessModifier.Protected)]
	public void CheckSleeperTriggers()
	{
		if (!this.world.IsRemote() && base.IsAlive())
		{
			this.world.CheckSleeperVolumeTouching(this);
			this.world.CheckTriggerVolumeTrigger(this);
		}
	}

	// Token: 0x0600230B RID: 8971 RVA: 0x000DC700 File Offset: 0x000DA900
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float _impulseScale = 1f)
	{
		if (GameStats.GetBool(EnumGameStats.IsPlayerDamageEnabled))
		{
			return base.DamageEntity(_damageSource, _strength, _criticalHit, _impulseScale);
		}
		return 0;
	}

	// Token: 0x0600230C RID: 8972 RVA: 0x00002914 File Offset: 0x00000B14
	public override void CheckDismember(ref DamageResponse _dmResponse, float damagePer)
	{
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x000DC718 File Offset: 0x000DA918
	public override void PlayOneShot(string clipName, bool sound_in_head = false, bool serverSignalOnly = false, bool isUnique = false, AnimationEvent _animEvent = null)
	{
		if (!this.isSpectator || sound_in_head)
		{
			base.PlayOneShot(clipName, sound_in_head, serverSignalOnly, isUnique, _animEvent);
		}
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x000DC734 File Offset: 0x000DA934
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string GetSoundHurt(DamageSource _damageSource, int _damageStrength)
	{
		string soundDrownPain;
		if (_damageSource.GetDamageType() == EnumDamageTypes.Suffocation && (soundDrownPain = base.GetSoundDrownPain()) != null)
		{
			return soundDrownPain;
		}
		if (_damageStrength > 15 || base.GetSoundHurtSmall() == null)
		{
			return base.GetSoundHurt();
		}
		return base.GetSoundHurtSmall();
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000DC771 File Offset: 0x000DA971
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string GetSoundDeath(DamageSource _damageSource)
	{
		if (this.soundDrownDeath == null || _damageSource.GetDamageType() != EnumDamageTypes.Suffocation)
		{
			return base.GetSoundDeath(_damageSource);
		}
		return this.soundDrownDeath;
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x000DC793 File Offset: 0x000DA993
	public bool CanHeal()
	{
		return this.Health > 0 && this.Health < this.GetMaxHealth();
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsSavedToFile()
	{
		return false;
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsSavedToNetwork()
	{
		return false;
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void EnableCamera(bool _b)
	{
	}

	// Token: 0x06002314 RID: 8980 RVA: 0x000DC7AE File Offset: 0x000DA9AE
	public virtual void Respawn(RespawnType _reason)
	{
		this.lastRespawnReason = _reason;
		this.emodel.DisableRagdoll(true);
		this.InitBreadcrumbs();
	}

	// Token: 0x06002315 RID: 8981 RVA: 0x000DC7CC File Offset: 0x000DA9CC
	public virtual void Teleport(Vector3 _pos, float _dir = -3.4028235E+38f)
	{
		if (this.AttachedToEntity)
		{
			this.AttachedToEntity.SetPosition(_pos, true);
		}
		else
		{
			this.SetPosition(_pos, true);
			if (_dir > -999999f)
			{
				this.SetRotation(new Vector3(0f, _dir, 0f));
			}
		}
		GameEventManager.Current.HandleForceBossDespawn(this);
		this.Respawn(RespawnType.Teleport);
	}

	// Token: 0x06002316 RID: 8982 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void BeforePlayerRespawn(RespawnType _type)
	{
	}

	// Token: 0x06002317 RID: 8983 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void AfterPlayerRespawn(RespawnType _type)
	{
	}

	// Token: 0x06002318 RID: 8984 RVA: 0x000DC830 File Offset: 0x000DAA30
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void onSpawnStateChanged()
	{
		base.onSpawnStateChanged();
		this.SetVisible(this.Spawned);
		if (this.Spawned)
		{
			this.SpawnedTicks = 0;
			switch (this.lastRespawnReason)
			{
			case RespawnType.NewGame:
			case RespawnType.Died:
			case RespawnType.EnterMultiplayer:
				if (!this.world.IsRemote() && !this.world.IsEditor() && this.IsSafeZoneActive())
				{
					this.world.LockAreaMasterChunksAround(World.worldToBlockPos(base.GetPosition()), this.world.worldTime + (ulong)((long)(GamePrefs.GetInt(EnumGamePrefs.PlayerSafeZoneHours) * 1000)));
				}
				break;
			}
			if (this.lastRespawnReason != RespawnType.Teleport)
			{
				this.lastRespawnReason = RespawnType.Unknown;
			}
		}
	}

	// Token: 0x06002319 RID: 8985 RVA: 0x000DC8E8 File Offset: 0x000DAAE8
	public override int AttachToEntity(Entity _other, int slot = -1)
	{
		slot = base.AttachToEntity(_other, slot);
		if (slot >= 0)
		{
			Transform modelTransformParent = this.emodel.GetModelTransformParent();
			this.attachedModelPos = modelTransformParent.localPosition;
			modelTransformParent.localPosition = Vector3.zero;
		}
		return slot;
	}

	// Token: 0x0600231A RID: 8986 RVA: 0x000DC927 File Offset: 0x000DAB27
	public override void Detach()
	{
		base.Detach();
		this.emodel.GetModelTransformParent().localPosition = this.attachedModelPos;
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x000DC948 File Offset: 0x000DAB48
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void onNewPrefabEntered(PrefabInstance _prefabInstance)
	{
		if (_prefabInstance == null)
		{
			return;
		}
		if (_prefabInstance.prefab.bTraderArea)
		{
			EntityPlayerLocal entityPlayerLocal = this as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				Waypoint waypoint = new Waypoint();
				waypoint.pos = World.worldToBlockPos(_prefabInstance.boundingBoxPosition + _prefabInstance.boundingBoxSize / 2);
				waypoint.icon = "ui_game_symbol_map_trader";
				waypoint.name.Update(_prefabInstance.prefab.PrefabName, PlatformManager.MultiPlatform.User.PlatformUserId);
				waypoint.ownerId = null;
				waypoint.lastKnownPositionEntityId = -1;
				waypoint.bIsAutoWaypoint = true;
				waypoint.bUsingLocalizationId = true;
				if (!entityPlayerLocal.Waypoints.ContainsWaypoint(waypoint))
				{
					NavObject navObject = NavObjectManager.Instance.RegisterNavObject("waypoint", waypoint.pos, waypoint.icon, true, -1, null);
					navObject.UseOverrideColor = true;
					navObject.OverrideColor = Color.white;
					navObject.IsActive = false;
					navObject.name = waypoint.name.Text;
					navObject.usingLocalizationId = true;
					waypoint.navObject = navObject;
					entityPlayerLocal.Waypoints.Collection.Add(waypoint);
				}
			}
		}
	}

	// Token: 0x0600231C RID: 8988 RVA: 0x000DCA6A File Offset: 0x000DAC6A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void StartJumpSwimMotion()
	{
		this.motion.y = this.motion.y + 0.04f;
	}

	// Token: 0x170003CE RID: 974
	// (get) Token: 0x0600231D RID: 8989 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsImmuneToLegDamage
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600231E RID: 8990 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDetailedHeadBodyColliders()
	{
		return true;
	}

	// Token: 0x0600231F RID: 8991 RVA: 0x000DCA80 File Offset: 0x000DAC80
	public override int GetLayerForMapIcon()
	{
		return 19;
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x000DCA84 File Offset: 0x000DAC84
	public override bool CanMapIconBeSelected()
	{
		return GameStats.GetBool(EnumGameStats.IsSpawnNearOtherPlayer);
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x000DCA8D File Offset: 0x000DAC8D
	public override bool IsDrawMapIcon()
	{
		return base.IsSpawned() && ((this.IsFriendOfLocalPlayer && GameStats.GetBool(EnumGameStats.ShowFriendPlayerOnMap)) || GameStats.GetBool(EnumGameStats.ShowAllPlayersOnMap) || this.IsInPartyOfLocalPlayer);
	}

	// Token: 0x06002322 RID: 8994 RVA: 0x000B195B File Offset: 0x000AFB5B
	public override Color GetMapIconColor()
	{
		return Color.white;
	}

	// Token: 0x06002323 RID: 8995 RVA: 0x000DCABB File Offset: 0x000DACBB
	public override Vector3 GetMapIconScale()
	{
		return new Vector3(1.5f, 1.5f, 1.5f);
	}

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x06002324 RID: 8996 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsAlert
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsClientControlled()
	{
		return true;
	}

	// Token: 0x06002326 RID: 8998 RVA: 0x000DCAD4 File Offset: 0x000DACD4
	public bool IsFriendsWith(EntityPlayer _other)
	{
		PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.entityId);
		PersistentPlayerData playerDataFromEntityID2 = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(_other.entityId);
		return playerDataFromEntityID != null && playerDataFromEntityID2 != null && playerDataFromEntityID2.ACL != null && playerDataFromEntityID2.ACL.Contains(playerDataFromEntityID.PrimaryId);
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x000DCB31 File Offset: 0x000DAD31
	public bool IsSafeZoneActive()
	{
		return this.Progression.Level <= GamePrefs.GetInt(EnumGamePrefs.PlayerSafeZoneLevel) && this.spawnPoints.Count == 0;
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x000DCB57 File Offset: 0x000DAD57
	public override void OnEntityUnload()
	{
		if (!this.world.IsEditor() && this.prefab != null)
		{
			this.world.triggerManager.RemovePlayer(this.prefab, this.entityId);
		}
		base.OnEntityUnload();
		this.ChunkObserver = null;
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x000DCB98 File Offset: 0x000DAD98
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		this.SpawnedTicks++;
		Vector3 a = this.position - this.averagVelLastPos;
		this.averagVelLastPos = this.position;
		if (a.sqrMagnitude < 25f)
		{
			this.averageVel = this.averageVel * 0.7f + a * 0.3f;
		}
		if (this.Health <= 0)
		{
			this.lastRespawnReason = RespawnType.Died;
			List<Transform> list = new List<Transform>();
			GameUtils.FindDeepChildWithPartialName(base.transform, "temp_Projectile", ref list);
			for (int i = 0; i < list.Count; i++)
			{
				UnityEngine.Object.Destroy(list[i].gameObject);
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Vector3 position = this.position;
			if ((position - this.breadcrumbLastPos).sqrMagnitude >= 0.9025f)
			{
				this.breadcrumbLastPos = position;
				this.breadcrumbIndex = (this.breadcrumbIndex + 1 & 31);
				this.breadcrumbs[this.breadcrumbIndex] = position;
			}
			this.Stealth.Tick();
		}
		else
		{
			this.Stealth.ProcNoiseCleanup();
		}
		this.PrefabTick();
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x000DCCCC File Offset: 0x000DAECC
	[PublicizedFrom(EAccessModifier.Private)]
	public void PrefabTick()
	{
		if (Time.time - this.lastTimePrefabChecked > 1f)
		{
			this.lastTimePrefabChecked = Time.time;
			PrefabInstance poiatPosition = this.world.GetPOIAtPosition(this.position, true);
			if (poiatPosition != this.prefab)
			{
				if (!this.world.IsEditor())
				{
					if (this.prefab != null)
					{
						this.world.triggerManager.RemovePlayer(this.prefab, this.entityId);
					}
					if (poiatPosition != null)
					{
						this.world.triggerManager.AddPrefabData(poiatPosition, this.entityId);
					}
				}
				this.prefab = poiatPosition;
				this.prefabTimeIn = Time.time;
				this.prefabInfoEntered = false;
				this.onNewPrefabEntered(this.prefab);
			}
			if (this.prefab != null && !this.prefabInfoEntered && !this.world.IsEditor())
			{
				if (this is EntityPlayerLocal)
				{
					if (this.prefab.IsWithinInfoArea(this.position))
					{
						if (this.prefab.prefab.InfoVolumes.Count > 0 || this.prefab.prefab.DifficultyTier >= 0)
						{
							this.enteredPrefab = this.prefab;
						}
						this.prefabInfoEntered = true;
					}
				}
				else
				{
					this.prefabInfoEntered = true;
				}
			}
			Vector3i blockPosition = base.GetBlockPosition();
			this.IsInTrader = (this.world.GetTraderAreaAt(blockPosition) != null);
			if (this.TwitchEnabled || this.HasTwitchMember())
			{
				this.TwitchSafe = (!this.world.CanPlaceBlockAt(blockPosition, null, false) || this.IsInTrader);
				return;
			}
			if (this.twitchSafe)
			{
				this.TwitchSafe = false;
			}
		}
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x000DCE60 File Offset: 0x000DB060
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitBreadcrumbs()
	{
		this.breadcrumbs.Fill(this.position);
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x000DCE74 File Offset: 0x000DB074
	public Vector3 GetBreadcrumbPos(float distance)
	{
		int num = (int)(distance + 0.5f);
		int num2 = this.breadcrumbIndex;
		if (num >= 31)
		{
			num2++;
		}
		else
		{
			num2 -= num;
		}
		return this.breadcrumbs[num2 & 31];
	}

	// Token: 0x0600232D RID: 9005 RVA: 0x000DCEAF File Offset: 0x000DB0AF
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateStepSound(float _distX, float _distZ, float _rotYDelta)
	{
		if (this is EntityPlayerLocal && !this.isSpectator)
		{
			base.updateStepSound(_distX, _distZ, _rotYDelta);
		}
	}

	// Token: 0x0600232E RID: 9006 RVA: 0x000DCECA File Offset: 0x000DB0CA
	public override float GetBlockDamageScale()
	{
		return (float)GameStats.GetInt(EnumGameStats.BlockDamagePlayer) * 0.01f;
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x000DCEDA File Offset: 0x000DB0DA
	public override void SetDamagedTarget(EntityAlive _attackTarget)
	{
		base.SetDamagedTarget(_attackTarget);
		if (_attackTarget is EntityEnemy)
		{
			this.LastZombieAttackTime = this.world.worldTime;
		}
		this.IsBloodMoonDead = false;
	}

	// Token: 0x06002330 RID: 9008 RVA: 0x000DCF04 File Offset: 0x000DB104
	public override void VisiblityCheck(float _distanceSqr, bool _masterIsZooming)
	{
		if (!this.Spawned)
		{
			return;
		}
		int num = this.visiblityCheckTicks - 1;
		this.visiblityCheckTicks = num;
		if (num > 0)
		{
			return;
		}
		this.visiblityCheckTicks = 5;
		int num2 = Utils.FastMin(12, GameUtils.GetViewDistance()) * 16;
		num2--;
		this.bModelVisible = (_distanceSqr < (float)(num2 * num2));
		if (!this.IsDead() && base.GetDeathTime() == 0)
		{
			this.SetVisible(this.bModelVisible);
		}
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x000DCF73 File Offset: 0x000DB173
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetVisible(bool _isVisible)
	{
		if (this.isSpectator)
		{
			this.emodel.SetVisible(false, false);
			return;
		}
		this.emodel.SetVisible(_isVisible, !this.world.IsRemote());
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x000DCFA5 File Offset: 0x000DB1A5
	public override void Kill(DamageResponse _dmResponse)
	{
		base.Kill(_dmResponse);
		this.currentLife = 0f;
	}

	// Token: 0x06002333 RID: 9011 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnHUD()
	{
	}

	// Token: 0x06002334 RID: 9012 RVA: 0x000DCFBC File Offset: 0x000DB1BC
	public void ServerNetSendRangeCheckedDamage(Vector3 _origin, float _maxRange, DamageSourceEntity _damageSource, int _strength, bool _isCritical, List<string> _buffActions, string _buffActionsContext, ParticleEffect particleEffect)
	{
		NetPackageRangeCheckDamageEntity package = NetPackageManager.GetPackage<NetPackageRangeCheckDamageEntity>().Setup(this.entityId, _origin, _maxRange, _damageSource, _strength, _isCritical, _buffActions, _buffActionsContext, particleEffect);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
	}

	// Token: 0x06002335 RID: 9013 RVA: 0x000DD00A File Offset: 0x000DB20A
	public override AttachedToEntitySlotExit FindValidExitPosition(List<AttachedToEntitySlotExit> candidatePositions)
	{
		this.lastVehiclePositionOnDismount = this.position;
		this.timeOfVehicleDismount = Time.time;
		this.forcedDetach = false;
		return base.FindValidExitPosition(candidatePositions);
	}

	// Token: 0x06002336 RID: 9014 RVA: 0x000DD034 File Offset: 0x000DB234
	public override void CheckPosition()
	{
		base.CheckPosition();
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (this.IsFlyMode.Value || !this.Spawned)
		{
			return;
		}
		if (this.position.y >= 0f)
		{
			return;
		}
		if (this.AttachedToEntity != null)
		{
			this.Detach();
			this.forcedDetach = true;
			return;
		}
		Log.Out(string.Format("[FELLTHROUGHWORLD] Player is under the world, starting teleport respawn from {0}", this.position));
		Vector3 fallingSavePosition = this.GetFallingSavePosition();
		if (this.isEntityRemote)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(fallingSavePosition, null, true), false, this.entityId, -1, -1, null, 192, false);
			return;
		}
		Log.Out(string.Format("[FELLTHROUGHWORLD] Attempting teleport to {0}", fallingSavePosition));
		this.Teleport(fallingSavePosition, float.MinValue);
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x000DD11C File Offset: 0x000DB31C
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 GetFallingSavePosition()
	{
		if (!this.forcedDetach && Time.time - this.timeOfVehicleDismount < this.vehicleTeleportThresholdSeconds)
		{
			return this.lastVehiclePositionOnDismount;
		}
		Vector3 position = this.position;
		IChunk chunkFromWorldPos = this.world.GetChunkFromWorldPos((int)position.x, (int)position.z);
		if (chunkFromWorldPos == null || chunkFromWorldPos.IsEmpty())
		{
			Log.Out(string.Format("[FELLTHROUGHWORLD] GetFallingSavePosition - CurrentChunk {0}", chunkFromWorldPos));
			IChunk chunk = null;
			Vector2 b = new Vector2(position.x, position.z);
			float num = float.PositiveInfinity;
			foreach (long key in this.ChunkObserver.chunksAround.list)
			{
				IChunk chunkSync = this.world.GetChunkSync(key);
				if (chunkSync != null && !chunkSync.IsEmpty())
				{
					Vector3i worldPos = chunkSync.GetWorldPos();
					float sqrMagnitude = (new Vector2((float)worldPos.x + 8f, (float)worldPos.z + 8f) - b).sqrMagnitude;
					if (chunk == null || sqrMagnitude < num)
					{
						chunk = chunkSync;
						num = sqrMagnitude;
					}
				}
			}
			Log.Out(string.Format("[FELLTHROUGHWORLD] GetFallingSavePosition - closestChunk {0}", chunk));
			if (chunk != null)
			{
				Vector3i worldPos2 = chunk.GetWorldPos();
				position.x = Math.Clamp(position.x, (float)worldPos2.x + 0.5f, (float)(worldPos2.x + 16) - 1f);
				position.z = Math.Clamp(position.z, (float)worldPos2.z + 0.5f, (float)(worldPos2.z + 16) - 1f);
			}
		}
		position.y = (float)GameManager.Instance.World.GetTerrainHeight((int)position.x, (int)position.z) + 0.5f;
		return position;
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x000DD304 File Offset: 0x000DB504
	public override bool FriendlyFireCheck(EntityAlive other)
	{
		bool result = true;
		try
		{
			EntityPlayer entityPlayer = other as EntityPlayer;
			if (entityPlayer != null)
			{
				if (this.entityId == entityPlayer.entityId)
				{
					return true;
				}
				int @int = GameStats.GetInt(EnumGameStats.PlayerKillingMode);
				if (@int != 0)
				{
					if (@int - 1 <= 1)
					{
						PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.entityId);
						PersistentPlayerData playerDataFromEntityID2 = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(entityPlayer.entityId);
						if (playerDataFromEntityID != null && playerDataFromEntityID2 != null)
						{
							bool flag = playerDataFromEntityID2.ACL != null && playerDataFromEntityID2.ACL.Contains(playerDataFromEntityID.PrimaryId);
							bool flag2 = this.Party != null && this.Party.MemberList.Contains(entityPlayer);
							result = ((flag || flag2) ^ @int == 2);
						}
					}
				}
				else
				{
					result = false;
				}
			}
		}
		catch
		{
			result = true;
		}
		return result;
	}

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x06002339 RID: 9017 RVA: 0x000DD3EC File Offset: 0x000DB5EC
	// (set) Token: 0x0600233A RID: 9018 RVA: 0x000DD3F4 File Offset: 0x000DB5F4
	public Party Party
	{
		get
		{
			return this.party;
		}
		set
		{
			if (this.party != null && value == null && this is EntityPlayerLocal)
			{
				this.party.ClearAllNavObjectColors();
			}
			this.party = value;
			if (this.party == null && this is EntityPlayerLocal)
			{
				this.QuestJournal.RemoveAllSharedQuests();
			}
		}
	}

	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x0600233B RID: 9019 RVA: 0x000DD441 File Offset: 0x000DB641
	public CompanionGroup Companions
	{
		get
		{
			if (this.companions == null)
			{
				this.companions = new CompanionGroup();
			}
			return this.companions;
		}
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x000DD45C File Offset: 0x000DB65C
	public bool IsInParty()
	{
		return this.Party != null;
	}

	// Token: 0x0600233D RID: 9021 RVA: 0x000DD467 File Offset: 0x000DB667
	public bool IsPartyLead()
	{
		return this.Party != null && this.Party.Leader == this;
	}

	// Token: 0x0600233E RID: 9022 RVA: 0x000DD484 File Offset: 0x000DB684
	public bool HasTwitchMember()
	{
		return this.Party != null && this.Party.HasTwitchMember;
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x000DD49B File Offset: 0x000DB69B
	public TwitchVoteLockTypes HasTwitchVoteLockMember()
	{
		if (this.Party != null)
		{
			return this.Party.HasTwitchVoteLock;
		}
		return TwitchVoteLockTypes.None;
	}

	// Token: 0x06002340 RID: 9024 RVA: 0x000DD4B2 File Offset: 0x000DB6B2
	public void CreateParty()
	{
		this.Party = new Party();
		this.Party.AddPlayer(this);
		this.Party.LeaderIndex = 0;
		this.HandleOnPartyJoined();
	}

	// Token: 0x06002341 RID: 9025 RVA: 0x000DD4E0 File Offset: 0x000DB6E0
	public void LeaveParty()
	{
		Party oldParty = this.Party;
		if (this.Party != null)
		{
			this.Party.MemberList.Remove(this);
			if (this is EntityPlayerLocal)
			{
				for (int i = 0; i < this.Party.MemberList.Count; i++)
				{
					if (this.Party.MemberList[i].NavObject != null)
					{
						this.Party.MemberList[i].NavObject.UseOverrideColor = false;
					}
				}
			}
		}
		this.Party = null;
		this.HandleOnPartyLeave(oldParty);
	}

	// Token: 0x14000023 RID: 35
	// (add) Token: 0x06002342 RID: 9026 RVA: 0x000DD574 File Offset: 0x000DB774
	// (remove) Token: 0x06002343 RID: 9027 RVA: 0x000DD5AC File Offset: 0x000DB7AC
	public event OnPartyChanged PartyJoined;

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x06002344 RID: 9028 RVA: 0x000DD5E4 File Offset: 0x000DB7E4
	// (remove) Token: 0x06002345 RID: 9029 RVA: 0x000DD61C File Offset: 0x000DB81C
	public event OnPartyChanged PartyChanged;

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06002346 RID: 9030 RVA: 0x000DD654 File Offset: 0x000DB854
	// (remove) Token: 0x06002347 RID: 9031 RVA: 0x000DD68C File Offset: 0x000DB88C
	public event OnPartyChanged PartyLeave;

	// Token: 0x14000026 RID: 38
	// (add) Token: 0x06002348 RID: 9032 RVA: 0x000DD6C4 File Offset: 0x000DB8C4
	// (remove) Token: 0x06002349 RID: 9033 RVA: 0x000DD6FC File Offset: 0x000DB8FC
	public event OnPartyChanged InvitedToParty;

	// Token: 0x0600234A RID: 9034 RVA: 0x000DD734 File Offset: 0x000DB934
	public void RemovePartyInvite(int playerEntityID)
	{
		EntityPlayer item = GameManager.Instance.World.GetEntity(playerEntityID) as EntityPlayer;
		if (this.partyInvites.Contains(item))
		{
			this.partyInvites.Remove(item);
		}
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x000DD772 File Offset: 0x000DB972
	public void RemoveAllPartyInvites()
	{
		this.partyInvites.Clear();
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x000DD780 File Offset: 0x000DB980
	public void AddPartyInvite(int playerEntityID)
	{
		EntityPlayer item = GameManager.Instance.World.GetEntity(playerEntityID) as EntityPlayer;
		if (!this.partyInvites.Contains(item))
		{
			this.partyInvites.Add(item);
			if (this.InvitedToParty != null)
			{
				this.InvitedToParty(null, this);
			}
		}
	}

	// Token: 0x0600234D RID: 9037 RVA: 0x000DD7D4 File Offset: 0x000DB9D4
	public bool HasPendingPartyInvite(int playerEntityID)
	{
		EntityPlayer item = GameManager.Instance.World.GetEntity(playerEntityID) as EntityPlayer;
		return this.partyInvites.Contains(item);
	}

	// Token: 0x0600234E RID: 9038 RVA: 0x000DD803 File Offset: 0x000DBA03
	public void HandleOnPartyJoined()
	{
		OnPartyChanged partyJoined = this.PartyJoined;
		if (partyJoined == null)
		{
			return;
		}
		partyJoined(this.party, this);
	}

	// Token: 0x0600234F RID: 9039 RVA: 0x000DD81C File Offset: 0x000DBA1C
	public void HandleOnPartyChanged()
	{
		OnPartyChanged partyChanged = this.PartyChanged;
		if (partyChanged == null)
		{
			return;
		}
		partyChanged(this.party, this);
	}

	// Token: 0x06002350 RID: 9040 RVA: 0x000DD835 File Offset: 0x000DBA35
	public void HandleOnPartyLeave(Party _oldParty)
	{
		OnPartyChanged partyLeave = this.PartyLeave;
		if (partyLeave == null)
		{
			return;
		}
		partyLeave(_oldParty, this);
	}

	// Token: 0x06002351 RID: 9041 RVA: 0x000DD849 File Offset: 0x000DBA49
	public void PartyDisconnect()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Party.ServerHandleDisconnectParty(this);
		}
	}

	// Token: 0x06002352 RID: 9042 RVA: 0x000DD860 File Offset: 0x000DBA60
	public void SetPrefabsAroundNear(Dictionary<int, PrefabInstance> _prefabsAround)
	{
		this.prefabsAroundNear.Clear();
		foreach (KeyValuePair<int, PrefabInstance> keyValuePair in _prefabsAround)
		{
			this.prefabsAroundNear.Add(keyValuePair.Key, keyValuePair.Value);
		}
	}

	// Token: 0x06002353 RID: 9043 RVA: 0x000DD8CC File Offset: 0x000DBACC
	public Dictionary<int, PrefabInstance> GetPrefabsAroundNear()
	{
		return this.prefabsAroundNear;
	}

	// Token: 0x06002354 RID: 9044 RVA: 0x000DD8D4 File Offset: 0x000DBAD4
	public void AddKillXP(EntityAlive killedEntity, float xpModifier = 1f)
	{
		int num = EntityClass.list[killedEntity.entityClass].ExperienceValue;
		num = (int)EffectManager.GetValue(PassiveEffects.ExperienceGain, killedEntity.inventory.holdingItemItemValue, (float)num, killedEntity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		if (xpModifier != 1f)
		{
			num = (int)((float)num * xpModifier + 0.5f);
		}
		if (this.IsInParty())
		{
			num = this.Party.GetPartyXP(this, num);
		}
		if (!this.isEntityRemote)
		{
			this.Progression.AddLevelExp(num, "_xpFromKill", Progression.XPTypes.Kill, true, true);
			this.bPlayerStatsChanged = true;
		}
		else
		{
			NetPackageEntityAddExpClient package = NetPackageManager.GetPackage<NetPackageEntityAddExpClient>().Setup(this.entityId, num, Progression.XPTypes.Kill);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
		}
		if (xpModifier == 1f)
		{
			GameManager.Instance.SharedKillServer(killedEntity.entityId, this.entityId, xpModifier);
		}
	}

	// Token: 0x06002355 RID: 9045 RVA: 0x000DD9C8 File Offset: 0x000DBBC8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleClientDeath(Vector3i attackPos)
	{
		base.HandleClientDeath(attackPos);
		TwitchManager.Current.CheckKiller(this, this.entityThatKilledMe, attackPos);
		switch (GameStats.GetInt(EnumGameStats.DeathPenalty))
		{
		case 0:
			GameEventManager.Current.HandleAction("game_on_death_none", this, this, false, "", "", false, true, "", null);
			return;
		case 1:
			GameEventManager.Current.HandleAction("game_on_death_default", this, this, false, "", "", false, true, "", null);
			return;
		case 2:
			GameEventManager.Current.HandleAction("game_on_death_injured", this, this, false, "", "", false, true, "", null);
			return;
		case 3:
			GameEventManager.Current.HandleAction("game_on_death_permanent", this, this, false, "", "", false, true, "", null);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002356 RID: 9046 RVA: 0x000DDAA4 File Offset: 0x000DBCA4
	public void HandleTwitchActionsTempEnabled(EntityPlayer.TwitchActionsStates newState)
	{
		if (this.twitchActionsEnabled == EntityPlayer.TwitchActionsStates.Disabled)
		{
			return;
		}
		this.TwitchActionsEnabled = newState;
	}

	// Token: 0x06002357 RID: 9047 RVA: 0x000DDAB8 File Offset: 0x000DBCB8
	public bool IsReloadCancelled()
	{
		if (this.inventory.holdingItemData.actionData != null)
		{
			foreach (ItemActionData itemActionData in this.inventory.holdingItemData.actionData)
			{
				ItemActionRanged.ItemActionDataRanged itemActionDataRanged = itemActionData as ItemActionRanged.ItemActionDataRanged;
				if (itemActionDataRanged != null && itemActionDataRanged.isReloadCancelled)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x000DDB38 File Offset: 0x000DBD38
	public void SetLaserSightData(bool _laserSightActive, Vector3 _laserSightPosition)
	{
		if (_laserSightActive == this.laserSightActive && (this.laserSightPosition - _laserSightPosition).sqrMagnitude < 0.0016f)
		{
			return;
		}
		this.laserSightPosition = _laserSightPosition;
		this.laserSightActive = _laserSightActive;
		if (this.world.entityDistributer != null)
		{
			this.world.entityDistributer.SendPacketToTrackedPlayers(this.entityId, (this.world.GetPrimaryPlayer() != null) ? this.world.GetPrimaryPlayer().entityId : -1, NetPackageManager.GetPackage<NetPackagePlayerLaserSight>().Setup(this.entityId, _laserSightActive, _laserSightPosition), true);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePlayerLaserSight>().Setup(this.entityId, _laserSightActive, _laserSightPosition), false);
	}

	// Token: 0x040019F7 RID: 6647
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly FastTags<TagGroup.Global> STAMINA_LOSS_TAGS = FastTags<TagGroup.Global>.GetTag("Athletics");

	// Token: 0x040019F8 RID: 6648
	public float jumpStrength = 0.451f;

	// Token: 0x040019F9 RID: 6649
	public SpawnPosition lastSpawnPosition = SpawnPosition.Undef;

	// Token: 0x040019FA RID: 6650
	public PlayerProfile playerProfile;

	// Token: 0x040019FB RID: 6651
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public PersistentPlayerName cachedPlayerName;

	// Token: 0x040019FC RID: 6652
	public List<EntityAlive> aiClosest = new List<EntityAlive>();

	// Token: 0x040019FD RID: 6653
	public AIDirectorBloodMoonParty bloodMoonParty;

	// Token: 0x040019FE RID: 6654
	public bool IsBloodMoonDead;

	// Token: 0x040019FF RID: 6655
	public PlayerStealth Stealth;

	// Token: 0x04001A00 RID: 6656
	public const long cSpawnPointKeyInvalid = -1L;

	// Token: 0x04001A01 RID: 6657
	public long selectedSpawnPointKey = -1L;

	// Token: 0x04001A02 RID: 6658
	public ulong LastZombieAttackTime;

	// Token: 0x04001A03 RID: 6659
	public bool IsFriendOfLocalPlayer;

	// Token: 0x04001A04 RID: 6660
	public bool IsInPartyOfLocalPlayer;

	// Token: 0x04001A05 RID: 6661
	public uint totalItemsCrafted;

	// Token: 0x04001A06 RID: 6662
	public float longestLife;

	// Token: 0x04001A07 RID: 6663
	public float currentLife;

	// Token: 0x04001A08 RID: 6664
	public float totalTimePlayed;

	// Token: 0x04001A09 RID: 6665
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int longestLifeLived;

	// Token: 0x04001A0A RID: 6666
	public ChunkManager.ChunkObserver ChunkObserver;

	// Token: 0x04001A0B RID: 6667
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public RespawnType lastRespawnReason = RespawnType.Unknown;

	// Token: 0x04001A0C RID: 6668
	public int SpawnedTicks;

	// Token: 0x04001A0D RID: 6669
	public ulong gameStageBornAtWorldTime;

	// Token: 0x04001A0E RID: 6670
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i m_MarkerPosition = Vector3i.zero;

	// Token: 0x04001A0F RID: 6671
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NavObject navMarker;

	// Token: 0x04001A10 RID: 6672
	public bool navMarkerHidden;

	// Token: 0x04001A11 RID: 6673
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NavObject navVending;

	// Token: 0x04001A12 RID: 6674
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float lastTimePrefabChecked;

	// Token: 0x04001A13 RID: 6675
	public PrefabInstance prefab;

	// Token: 0x04001A14 RID: 6676
	public PrefabInstance enteredPrefab;

	// Token: 0x04001A15 RID: 6677
	public bool prefabInfoEntered;

	// Token: 0x04001A16 RID: 6678
	public float prefabTimeIn;

	// Token: 0x04001A17 RID: 6679
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bModelVisible = true;

	// Token: 0x04001A18 RID: 6680
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool twitchEnabled;

	// Token: 0x04001A19 RID: 6681
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool twitchSafe;

	// Token: 0x04001A1A RID: 6682
	public bool IsInTrader;

	// Token: 0x04001A1B RID: 6683
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public TwitchVoteLockTypes twitchVoteLock;

	// Token: 0x04001A1C RID: 6684
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool twitchVisionDisabled;

	// Token: 0x04001A1D RID: 6685
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityPlayer.TwitchActionsStates twitchActionsEnabled = EntityPlayer.TwitchActionsStates.Enabled;

	// Token: 0x04001A1E RID: 6686
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isSpectator;

	// Token: 0x04001A1F RID: 6687
	public WaypointCollection Waypoints = new WaypointCollection();

	// Token: 0x04001A20 RID: 6688
	public List<Waypoint> WaypointInvites = new List<Waypoint>();

	// Token: 0x04001A21 RID: 6689
	public QuestJournal QuestJournal = new QuestJournal();

	// Token: 0x04001A27 RID: 6695
	public List<ushort> favoriteCreativeStacks = new List<ushort>();

	// Token: 0x04001A28 RID: 6696
	public List<string> favoriteShapes = new List<string>();

	// Token: 0x04001A29 RID: 6697
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i m_rentedVMPosition = Vector3i.zero;

	// Token: 0x04001A2A RID: 6698
	public ulong RentalEndTime;

	// Token: 0x04001A2B RID: 6699
	public int RentalEndDay;

	// Token: 0x04001A2C RID: 6700
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 averageVel;

	// Token: 0x04001A2D RID: 6701
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 averagVelLastPos;

	// Token: 0x04001A2E RID: 6702
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cBreadcrumbMask = 31;

	// Token: 0x04001A2F RID: 6703
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3[] breadcrumbs = new Vector3[32];

	// Token: 0x04001A30 RID: 6704
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 breadcrumbLastPos;

	// Token: 0x04001A31 RID: 6705
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int breadcrumbIndex;

	// Token: 0x04001A32 RID: 6706
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isAdmin;

	// Token: 0x04001A33 RID: 6707
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i lastChunkPos = new Vector3i(int.MinValue, int.MinValue, int.MinValue);

	// Token: 0x04001A34 RID: 6708
	public bool HasUpdated;

	// Token: 0x04001A35 RID: 6709
	public FastTags<TagGroup.Global> generalTags = FastTags<TagGroup.Global>.none;

	// Token: 0x04001A36 RID: 6710
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 attachedModelPos;

	// Token: 0x04001A37 RID: 6711
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int visiblityCheckTicks;

	// Token: 0x04001A38 RID: 6712
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lastVehiclePositionOnDismount = Vector3.zero;

	// Token: 0x04001A39 RID: 6713
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeOfVehicleDismount;

	// Token: 0x04001A3A RID: 6714
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float vehicleTeleportThresholdSeconds = 10f;

	// Token: 0x04001A3B RID: 6715
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool forcedDetach;

	// Token: 0x04001A3C RID: 6716
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Party party;

	// Token: 0x04001A3D RID: 6717
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CompanionGroup companions;

	// Token: 0x04001A42 RID: 6722
	public List<EntityPlayer> partyInvites = new List<EntityPlayer>();

	// Token: 0x04001A43 RID: 6723
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<int, PrefabInstance> prefabsAroundNear = new Dictionary<int, PrefabInstance>();

	// Token: 0x04001A44 RID: 6724
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool laserSightActive;

	// Token: 0x04001A45 RID: 6725
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 laserSightPosition;

	// Token: 0x02000456 RID: 1110
	public enum TwitchActionsStates
	{
		// Token: 0x04001A47 RID: 6727
		Disabled,
		// Token: 0x04001A48 RID: 6728
		Enabled,
		// Token: 0x04001A49 RID: 6729
		TempDisabled,
		// Token: 0x04001A4A RID: 6730
		TempDisabledEnding
	}
}
