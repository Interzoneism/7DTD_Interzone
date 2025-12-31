using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003B2 RID: 946
[Preserve]
public class AIDirectorBloodMoonParty
{
	// Token: 0x06001CEC RID: 7404 RVA: 0x000B509C File Offset: 0x000B329C
	public AIDirectorBloodMoonParty(EntityPlayer _initialPlayer, AIDirectorBloodMoonComponent _controller, int _bloodMoonCountUNUSED)
	{
		this.spawnWorld = _initialPlayer.world;
		this.spawnBasePos = _initialPlayer.position;
		this.controller = _controller;
		this.partySpawner = new AIDirectorGameStagePartySpawner(_controller.Director.World, "BloodMoonHorde");
		this.partySpawner.AddMember(_initialPlayer);
		_initialPlayer.bloodMoonParty = this;
		this.spawnBaseDir = _controller.Random.RandomRange(0, 360);
		this.groupIndex = -1;
	}

	// Token: 0x06001CED RID: 7405 RVA: 0x000B5128 File Offset: 0x000B3328
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcBestDir(Vector3 basePos)
	{
		int[] array = new int[16];
		int num = 0;
		for (int i = 0; i < 16; i++)
		{
			float num2 = (float)i * 22.5f;
			this.spawnDirectionV = Quaternion.AngleAxis(num2, Vector3.up) * Vector3.forward * 40f;
			int num3 = 0;
			for (int j = 0; j < 9; j++)
			{
				Vector3 vector;
				if (this.spawnWorld.GetRandomSpawnPositionMinMaxToPosition(basePos + this.spawnDirectionV, 0, 10, 30, false, out vector, -1, true, 30, false, EnumLandClaimOwner.None, false))
				{
					num3++;
				}
			}
			if (num3 > 0)
			{
				num3 = (num3 + 2) / 3;
				if (Utils.FastAbs(Mathf.DeltaAngle(num2, (float)this.spawnBaseDir)) <= 60f)
				{
					num3 *= 3;
				}
			}
			array[i] = num3;
			num = Utils.FastMax(num, num3);
		}
		int num4 = 0;
		for (int k = 0; k < 16; k++)
		{
			if (array[k] == num)
			{
				num4++;
			}
		}
		int num5 = 0;
		int num6 = this.controller.Random.RandomRange(0, num4);
		for (int l = 0; l < 16; l++)
		{
			if (array[l] >= num && --num6 < 0)
			{
				num5 = l;
				break;
			}
		}
		this.spawnDirectionV = Quaternion.AngleAxis((float)num5 * 22.5f, Vector3.up) * Vector3.forward * 40f;
	}

	// Token: 0x06001CEE RID: 7406 RVA: 0x000B528C File Offset: 0x000B348C
	public bool Tick(World _world, double _dt, bool _canSpawn)
	{
		if (this.partySpawner.partyLevel < 0)
		{
			this.InitParty();
		}
		for (int i = this.zombies.Count - 1; i >= 0; i--)
		{
			AIDirectorBloodMoonParty.ManagedZombie managedZombie = this.zombies[i];
			managedZombie.updateDelay -= (float)_dt;
			if (managedZombie.updateDelay <= 0f)
			{
				managedZombie.updateDelay = 1.8f;
				if (!this.SeekTarget(managedZombie))
				{
					this.zombies.RemoveAt(i);
				}
			}
		}
		this.partySpawner.Tick(_dt);
		bool result = false;
		if (_canSpawn)
		{
			if (!this.partySpawner.canSpawn || this.partySpawner.partyMembers.Count == 0)
			{
				return true;
			}
			if (AIDirector.CanSpawn(1.9f))
			{
				int num = this.partySpawner.groupIndex;
				if (num != this.groupIndex)
				{
					this.groupIndex = num;
					this.spawnBaseDir += 120;
					this.CalcBestDir(this.spawnBasePos);
				}
				result = true;
				int count = this.partySpawner.partyMembers.Count;
				int num2 = Utils.FastMin(this.partySpawner.maxAlive, this.enemyActiveMax);
				if (this.zombies.Count < num2)
				{
					for (int j = Utils.FastMin(count, 3); j > 0; j--)
					{
						if (this.nextPlayer >= count)
						{
							this.nextPlayer = 0;
						}
						EntityPlayer entityPlayer = this.partySpawner.partyMembers[this.nextPlayer];
						bool flag = false;
						if (this.IsPlayerATarget(entityPlayer))
						{
							flag = this.SpawnZombie(_world, entityPlayer, entityPlayer.position, this.spawnDirectionV);
						}
						this.nextPlayer++;
						if (flag)
						{
							break;
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001CEF RID: 7407 RVA: 0x000B543D File Offset: 0x000B363D
	public void PlayerLoggedOut(EntityPlayer _player)
	{
		this.partySpawner.RemoveMember(_player, false);
		if (this.nextPlayer >= this.partySpawner.partyMembers.Count)
		{
			this.nextPlayer = 0;
		}
	}

	// Token: 0x06001CF0 RID: 7408 RVA: 0x000B546C File Offset: 0x000B366C
	public void KillPartyZombies()
	{
		int count = this.zombies.Count;
		if (count > 0)
		{
			this.partySpawner.DecSpawnCount(count);
			for (int i = 0; i < count; i++)
			{
				EntityEnemy zombie = this.zombies[i].zombie;
				if (zombie && !zombie.IsDead() && !zombie.IsDespawned && zombie.gameObject)
				{
					zombie.Kill(DamageResponse.New(true));
				}
			}
			this.zombies.Clear();
		}
	}

	// Token: 0x17000333 RID: 819
	// (get) Token: 0x06001CF1 RID: 7409 RVA: 0x000B54EF File Offset: 0x000B36EF
	public bool IsEmpty
	{
		get
		{
			return this.partySpawner.partyMembers.Count <= 0;
		}
	}

	// Token: 0x06001CF2 RID: 7410 RVA: 0x000B5507 File Offset: 0x000B3707
	public bool IsMemberOfParty(int _entityID)
	{
		return this.partySpawner.IsMemberOfParty(_entityID);
	}

	// Token: 0x06001CF3 RID: 7411 RVA: 0x000B5518 File Offset: 0x000B3718
	public bool TryAddPlayer(EntityPlayer _player)
	{
		for (int i = 0; i < this.partySpawner.partyMembers.Count; i++)
		{
			if ((this.partySpawner.partyMembers[i].GetPosition() - _player.GetPosition()).sqrMagnitude <= 6400f)
			{
				this.AddPlayer(_player);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001CF4 RID: 7412 RVA: 0x000B557A File Offset: 0x000B377A
	public void AddPlayer(EntityPlayer _player)
	{
		this.partySpawner.AddMember(_player);
		_player.bloodMoonParty = this;
	}

	// Token: 0x06001CF5 RID: 7413 RVA: 0x000B5590 File Offset: 0x000B3790
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitParty()
	{
		int num = this.partySpawner.CalcPartyLevel();
		int num2 = GameStats.GetInt(EnumGameStats.BloodMoonEnemyCount) * this.partySpawner.partyMembers.Count;
		this.enemyActiveMax = Utils.FastMin(30, num2);
		float num3 = Utils.FastMax(1f, (float)num2 / (float)this.enemyActiveMax);
		num3 = Utils.FastLerp(1f, num3, (float)num / 60f);
		this.partySpawner.SetScaling(num3);
		this.partySpawner.SetPartyLevel(num);
		this.bonusLootSpawnCount = this.partySpawner.bonusLootEvery / 2;
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x000B5624 File Offset: 0x000B3824
	[PublicizedFrom(EAccessModifier.Private)]
	public bool SpawnZombie(World _world, EntityPlayer _target, Vector3 _focusPos, Vector3 _radiusV)
	{
		Vector3 vector;
		if (!this.CalcSpawnPos(_world, _focusPos, _radiusV, out vector))
		{
			return false;
		}
		bool flag = true;
		int et = EntityGroups.GetRandomFromGroup(this.partySpawner.spawnGroupName, ref this.lastClassId, null);
		if (_target.AttachedToEntity && this.controller.Random.RandomFloat < 0.5f)
		{
			flag = false;
			et = EntityClass.FromString("animalZombieVultureRadiated");
		}
		EntityEnemy entityEnemy = (EntityEnemy)EntityFactory.CreateEntity(et, vector);
		_world.SpawnEntityInWorld(entityEnemy);
		entityEnemy.SetSpawnerSource(EnumSpawnerSource.Dynamic);
		entityEnemy.IsHordeZombie = true;
		entityEnemy.IsBloodMoon = true;
		entityEnemy.bIsChunkObserver = true;
		entityEnemy.timeStayAfterDeath /= 3;
		if (flag)
		{
			int num = this.bonusLootSpawnCount + 1;
			this.bonusLootSpawnCount = num;
			if (num >= this.partySpawner.bonusLootEvery)
			{
				this.bonusLootSpawnCount = 0;
				entityEnemy.lootDropProb *= GameStageDefinition.LootBonusScale;
			}
		}
		AIDirectorBloodMoonParty.ManagedZombie managedZombie = new AIDirectorBloodMoonParty.ManagedZombie(entityEnemy, _target);
		this.zombies.Add(managedZombie);
		this.SeekTarget(managedZombie);
		this.partySpawner.IncSpawnCount();
		AstarManager.Instance.AddLocation(vector, 40);
		ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(_world.worldTime);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		Log.Out("BloodMoonParty: SpawnZombie grp {0}, cnt {1}, {2}, loot {3}, at player {4}, day/time {5} {6:D2}:{7:D2}", new object[]
		{
			this.partySpawner.ToString(),
			this.zombies.Count,
			entityEnemy.EntityName,
			entityEnemy.lootDropProb,
			_target.entityId,
			item,
			item2,
			item3
		});
		return true;
	}

	// Token: 0x06001CF7 RID: 7415 RVA: 0x000B57D8 File Offset: 0x000B39D8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CalcSpawnPos(World _world, Vector3 _focusPos, Vector3 _radiusV, out Vector3 spawnPos)
	{
		_radiusV = Quaternion.AngleAxis((this.controller.Random.RandomFloat - 0.5f) * 90f, Vector3.up) * _radiusV;
		return _world.GetMobRandomSpawnPosWithWater(_focusPos + _radiusV, 0, 10, 30, false, out spawnPos);
	}

	// Token: 0x06001CF8 RID: 7416 RVA: 0x000B5830 File Offset: 0x000B3A30
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer FindPartyTarget(Vector3 fromPos)
	{
		float num = float.MaxValue;
		EntityPlayer result = null;
		for (int i = this.partySpawner.partyMembers.Count - 1; i >= 0; i--)
		{
			EntityPlayer entityPlayer = this.partySpawner.partyMembers[i];
			if (this.IsPlayerATarget(entityPlayer))
			{
				float sqrMagnitude = (fromPos - entityPlayer.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = entityPlayer;
				}
			}
		}
		return result;
	}

	// Token: 0x06001CF9 RID: 7417 RVA: 0x000B58A0 File Offset: 0x000B3AA0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool SeekTarget(AIDirectorBloodMoonParty.ManagedZombie mz)
	{
		EntityAlive zombie = mz.zombie;
		if (!zombie || zombie.IsDead() || zombie.IsDespawned || !zombie.gameObject)
		{
			return false;
		}
		EntityPlayer entityPlayer = zombie.GetAttackTarget() as EntityPlayer;
		if (entityPlayer)
		{
			mz.player = entityPlayer;
		}
		if (!mz.player || !this.IsPlayerATarget(mz.player))
		{
			mz.player = this.FindPartyTarget(zombie.position);
		}
		if (mz.player)
		{
			Vector3 vector = zombie.position - mz.player.position;
			float sqrMagnitude = vector.sqrMagnitude;
			vector.y = 0f;
			Vector3 vector2;
			if (vector.sqrMagnitude >= 22500f && this.CalcSpawnPos(zombie.world, mz.player.position, this.spawnDirectionV, out vector2) && !zombie.world.IsPlayerAliveAndNear(zombie.position, 70f))
			{
				if (this.controller.Random.RandomFloat < 0.5f)
				{
					this.partySpawner.DecSpawnCount(1);
					zombie.lootDropProb = 0f;
					zombie.Kill(DamageResponse.New(true));
					return false;
				}
				zombie.SetPosition(vector2, true);
				zombie.moveHelper.Stop();
				Log.Warning("SeekTarget {0}, far, move {1}", new object[]
				{
					zombie.GetDebugName(),
					vector2
				});
			}
			if (sqrMagnitude <= 10000f || entityPlayer != mz.player)
			{
				zombie.SetAttackTarget(mz.player, 1200);
			}
			else
			{
				if (entityPlayer)
				{
					zombie.SetAttackTarget(null, 0);
				}
				zombie.SetInvestigatePosition(mz.player.position, 1200, true);
			}
			return true;
		}
		if (!zombie.world.IsPlayerAliveAndNear(zombie.position, 60f))
		{
			zombie.Kill(DamageResponse.New(true));
			return false;
		}
		return true;
	}

	// Token: 0x06001CFA RID: 7418 RVA: 0x000B5A94 File Offset: 0x000B3C94
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsPlayerATarget(EntityPlayer player)
	{
		return !player.IsDead() && player.IsSpawned() && player.entityId != -1 && !player.IsIgnoredByAI() && player.Progression.Level > 1 && !player.IsBloodMoonDead;
	}

	// Token: 0x17000334 RID: 820
	// (get) Token: 0x06001CFB RID: 7419 RVA: 0x000B5AD4 File Offset: 0x000B3CD4
	public bool BloodmoonZombiesRemain
	{
		get
		{
			return this.zombies.Count > 0;
		}
	}

	// Token: 0x04001397 RID: 5015
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cPartyJoinDistance = 80f;

	// Token: 0x04001398 RID: 5016
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cPartyJoinDistanceSq = 6400f;

	// Token: 0x04001399 RID: 5017
	public const float cSightDist = 100f;

	// Token: 0x0400139A RID: 5018
	public const float cSightDistSq = 10000f;

	// Token: 0x0400139B RID: 5019
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cTeleportDist = 150f;

	// Token: 0x0400139C RID: 5020
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cTeleportDistSq = 22500f;

	// Token: 0x0400139D RID: 5021
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSpawnPreferredArc = 120;

	// Token: 0x0400139E RID: 5022
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSpawnAngle = 90f;

	// Token: 0x0400139F RID: 5023
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSpawnDistance = 40f;

	// Token: 0x040013A0 RID: 5024
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSpawnMinRandDistance = 0;

	// Token: 0x040013A1 RID: 5025
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSpawnMaxRandDistance = 10;

	// Token: 0x040013A2 RID: 5026
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSpawnMinPlayerDistance = 30;

	// Token: 0x040013A3 RID: 5027
	public AIDirectorGameStagePartySpawner partySpawner;

	// Token: 0x040013A4 RID: 5028
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastClassId;

	// Token: 0x040013A5 RID: 5029
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIDirectorBloodMoonParty.ManagedZombie> zombies = new List<AIDirectorBloodMoonParty.ManagedZombie>();

	// Token: 0x040013A6 RID: 5030
	[PublicizedFrom(EAccessModifier.Private)]
	public World spawnWorld;

	// Token: 0x040013A7 RID: 5031
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 spawnBasePos;

	// Token: 0x040013A8 RID: 5032
	[PublicizedFrom(EAccessModifier.Private)]
	public int spawnBaseDir;

	// Token: 0x040013A9 RID: 5033
	[PublicizedFrom(EAccessModifier.Private)]
	public int enemyActiveMax;

	// Token: 0x040013AA RID: 5034
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorBloodMoonComponent controller;

	// Token: 0x040013AB RID: 5035
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 spawnDirectionV;

	// Token: 0x040013AC RID: 5036
	[PublicizedFrom(EAccessModifier.Private)]
	public int nextPlayer;

	// Token: 0x040013AD RID: 5037
	[PublicizedFrom(EAccessModifier.Private)]
	public int groupIndex;

	// Token: 0x040013AE RID: 5038
	[PublicizedFrom(EAccessModifier.Private)]
	public int bonusLootSpawnCount;

	// Token: 0x020003B3 RID: 947
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public class ManagedZombie
	{
		// Token: 0x06001CFC RID: 7420 RVA: 0x000B5AE4 File Offset: 0x000B3CE4
		public ManagedZombie(EntityEnemy _zombie, EntityPlayer _player)
		{
			this.zombie = _zombie;
			this.player = _player;
		}

		// Token: 0x040013AF RID: 5039
		public EntityPlayer player;

		// Token: 0x040013B0 RID: 5040
		public EntityEnemy zombie;

		// Token: 0x040013B1 RID: 5041
		public float updateDelay;
	}
}
