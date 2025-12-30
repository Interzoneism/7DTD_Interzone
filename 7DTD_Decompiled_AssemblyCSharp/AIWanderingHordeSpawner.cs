using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003D1 RID: 977
[Preserve]
public class AIWanderingHordeSpawner
{
	// Token: 0x06001DB4 RID: 7604 RVA: 0x000B8C88 File Offset: 0x000B6E88
	public AIWanderingHordeSpawner(AIDirector _director, AIWanderingHordeSpawner.SpawnType _spawnType, AIWanderingHordeSpawner.HordeArrivedDelegate _arrivedEvent, List<AIDirectorPlayerState> _targets, ulong _endTime, Vector3 _startPos, Vector3 _pitStopPos, Vector3 _endPos)
	{
		this.director = _director;
		this.startPos = _startPos;
		this.pitStopPos = _pitStopPos;
		this.endPos = _endPos;
		this.endTime = _endTime;
		this.arrivedCallback = _arrivedEvent;
		this.spawnType = _spawnType;
		AIWanderingHordeSpawner.SpawnType spawnType = this.spawnType;
		string gameStageName;
		int mod;
		if (spawnType != AIWanderingHordeSpawner.SpawnType.Bandits)
		{
			if (spawnType != AIWanderingHordeSpawner.SpawnType.Horde)
			{
			}
			gameStageName = "WanderingHorde";
			mod = 50;
		}
		else
		{
			gameStageName = "WanderingBandits";
			mod = 0;
		}
		this.spawner = new AIDirectorGameStagePartySpawner(_director.World, gameStageName);
		for (int i = 0; i < _targets.Count; i++)
		{
			this.spawner.AddMember(_targets[i].Player);
		}
		this.spawner.ResetPartyLevel(mod);
		this.spawner.ClearMembers();
	}

	// Token: 0x06001DB5 RID: 7605 RVA: 0x000B8D54 File Offset: 0x000B6F54
	public bool Update(World world, float _deltaTime)
	{
		if (world.GetPlayers().Count == 0)
		{
			return true;
		}
		if (world.worldTime >= this.endTime)
		{
			if (this.arrivedCallback != null)
			{
				this.arrivedCallback();
			}
			return true;
		}
		bool flag = this.UpdateSpawn(world, _deltaTime);
		if (flag && this.commandList.Count == 0)
		{
			if (this.arrivedCallback != null)
			{
				this.arrivedCallback();
			}
			return true;
		}
		if (!flag)
		{
			AstarManager.Instance.AddLocationLine(this.startPos, this.endPos, 64);
		}
		else
		{
			Vector3 vector = Vector3.zero;
			int num = 0;
			for (int i = 0; i < this.commandList.Count; i++)
			{
				Entity enemy = this.commandList[i].Enemy;
				if (!enemy.IsDead())
				{
					vector += enemy.position;
					num++;
				}
			}
			if (num > 0)
			{
				vector *= 1f / (float)num;
				AstarManager.Instance.AddLocation(vector, 64);
			}
		}
		this.UpdateHorde(_deltaTime);
		return false;
	}

	// Token: 0x06001DB6 RID: 7606 RVA: 0x000B8E54 File Offset: 0x000B7054
	[PublicizedFrom(EAccessModifier.Private)]
	public bool UpdateSpawn(World _world, float _deltaTime)
	{
		if (!AIDirector.CanSpawn(1f))
		{
			return true;
		}
		if (!this.spawner.Tick((double)_deltaTime))
		{
			return true;
		}
		this.spawnDelay -= _deltaTime;
		if (this.spawnDelay >= 0f)
		{
			return false;
		}
		this.spawnDelay = 1f;
		if (!this.spawner.canSpawn)
		{
			return false;
		}
		Vector3 transformPos;
		if (!_world.GetMobRandomSpawnPosWithWater(this.startPos, 1, 6, 15, true, out transformPos))
		{
			return false;
		}
		EntityEnemy entityEnemy = (EntityEnemy)EntityFactory.CreateEntity(EntityGroups.GetRandomFromGroup(this.spawner.spawnGroupName, ref this.lastClassId, null), transformPos);
		_world.SpawnEntityInWorld(entityEnemy);
		entityEnemy.SetSpawnerSource(EnumSpawnerSource.Dynamic);
		entityEnemy.IsHordeZombie = true;
		entityEnemy.bIsChunkObserver = true;
		entityEnemy.IsHordeZombie = true;
		entityEnemy.bIsChunkObserver = true;
		int num = this.bonusLootSpawnCount + 1;
		this.bonusLootSpawnCount = num;
		if (num >= GameStageDefinition.LootWanderingBonusEvery)
		{
			this.bonusLootSpawnCount = 0;
			entityEnemy.lootDropProb *= GameStageDefinition.LootWanderingBonusScale;
		}
		AIWanderingHordeSpawner.ZombieCommand zombieCommand = new AIWanderingHordeSpawner.ZombieCommand();
		zombieCommand.Enemy = entityEnemy;
		zombieCommand.TargetPos = AIWanderingHordeSpawner.RandomPos(this.director, this.endPos, 6f);
		zombieCommand.Command = AIWanderingHordeSpawner.ECommand.EndPos;
		this.commandList.Add(zombieCommand);
		entityEnemy.SetInvestigatePosition(zombieCommand.TargetPos, 6000, false);
		AIDirector.LogAI("Spawned wandering horde (group {0}, zombie {1})", new object[]
		{
			this.spawner.spawnGroupName,
			entityEnemy
		});
		this.spawner.IncSpawnCount();
		return false;
	}

	// Token: 0x06001DB7 RID: 7607 RVA: 0x000B8FC8 File Offset: 0x000B71C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateHorde(float dt)
	{
		int i = 0;
		while (i < this.commandList.Count)
		{
			AIWanderingHordeSpawner.ZombieCommand zombieCommand = this.commandList[i];
			bool flag = zombieCommand.Enemy.IsDead() || zombieCommand.Enemy.GetAttackTarget() != null;
			if (!flag)
			{
				if (zombieCommand.Command == AIWanderingHordeSpawner.ECommand.PitStop || zombieCommand.Command == AIWanderingHordeSpawner.ECommand.EndPos)
				{
					if (zombieCommand.Enemy.HasInvestigatePosition)
					{
						if (zombieCommand.Enemy.InvestigatePosition != zombieCommand.TargetPos)
						{
							flag = true;
							string str = "Wandering horde zombie '";
							EntityEnemy enemy = zombieCommand.Enemy;
							AIDirector.LogAIExtra(str + ((enemy != null) ? enemy.ToString() : null) + "' removed from horde control. Was killed or investigating", Array.Empty<object>());
						}
						else
						{
							zombieCommand.Enemy.SetInvestigatePosition(zombieCommand.TargetPos, 6000, false);
						}
					}
					else if (zombieCommand.Command == AIWanderingHordeSpawner.ECommand.PitStop)
					{
						string str2 = "Wandering horde zombie '";
						EntityEnemy enemy2 = zombieCommand.Enemy;
						AIDirector.LogAIExtra(str2 + ((enemy2 != null) ? enemy2.ToString() : null) + "' reached pitstop. Wander around for awhile", Array.Empty<object>());
						zombieCommand.WanderTime = 90f + this.director.random.RandomFloat * 4f;
						zombieCommand.Command = AIWanderingHordeSpawner.ECommand.Wander;
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					zombieCommand.WanderTime -= dt;
					zombieCommand.Enemy.ResetDespawnTime();
					if (zombieCommand.WanderTime <= 0f && zombieCommand.Enemy.GetAttackTarget() == null)
					{
						string str3 = "Wandering horde zombie '";
						EntityEnemy enemy3 = zombieCommand.Enemy;
						AIDirector.LogAIExtra(str3 + ((enemy3 != null) ? enemy3.ToString() : null) + "' wandered long enough. Going to endstop", Array.Empty<object>());
						zombieCommand.Command = AIWanderingHordeSpawner.ECommand.EndPos;
						zombieCommand.TargetPos = AIWanderingHordeSpawner.RandomPos(this.director, this.endPos, 6f);
						zombieCommand.Enemy.SetInvestigatePosition(zombieCommand.TargetPos, 6000, false);
						zombieCommand.Enemy.IsHordeZombie = false;
					}
				}
			}
			if (flag)
			{
				string str4 = "Wandering horde zombie '";
				EntityEnemy enemy4 = zombieCommand.Enemy;
				AIDirector.LogAIExtra(str4 + ((enemy4 != null) ? enemy4.ToString() : null) + "' removed from control", Array.Empty<object>());
				zombieCommand.Enemy.IsHordeZombie = false;
				zombieCommand.Enemy.bIsChunkObserver = false;
				this.commandList.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x06001DB8 RID: 7608 RVA: 0x000B9218 File Offset: 0x000B7418
	public void Cleanup()
	{
		for (int i = 0; i < this.commandList.Count; i++)
		{
			AIWanderingHordeSpawner.ZombieCommand zombieCommand = this.commandList[i];
			zombieCommand.Enemy.IsHordeZombie = false;
			zombieCommand.Enemy.bIsChunkObserver = false;
		}
	}

	// Token: 0x06001DB9 RID: 7609 RVA: 0x000B9260 File Offset: 0x000B7460
	public static Vector3 RandomPos(AIDirector director, Vector3 target, float radius)
	{
		Vector2 vector = director.random.RandomOnUnitCircle * radius;
		return target + new Vector3(vector.x, 0f, vector.y);
	}

	// Token: 0x04001458 RID: 5208
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cInvestigateTime = 6000;

	// Token: 0x04001459 RID: 5209
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirector director;

	// Token: 0x0400145A RID: 5210
	[PublicizedFrom(EAccessModifier.Private)]
	public AIWanderingHordeSpawner.HordeArrivedDelegate arrivedCallback;

	// Token: 0x0400145B RID: 5211
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 startPos;

	// Token: 0x0400145C RID: 5212
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 pitStopPos;

	// Token: 0x0400145D RID: 5213
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 endPos;

	// Token: 0x0400145E RID: 5214
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong endTime;

	// Token: 0x0400145F RID: 5215
	public AIWanderingHordeSpawner.SpawnType spawnType;

	// Token: 0x04001460 RID: 5216
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorGameStagePartySpawner spawner;

	// Token: 0x04001461 RID: 5217
	[PublicizedFrom(EAccessModifier.Private)]
	public float spawnDelay;

	// Token: 0x04001462 RID: 5218
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastClassId;

	// Token: 0x04001463 RID: 5219
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIWanderingHordeSpawner.ZombieCommand> commandList = new List<AIWanderingHordeSpawner.ZombieCommand>();

	// Token: 0x04001464 RID: 5220
	[PublicizedFrom(EAccessModifier.Private)]
	public int bonusLootSpawnCount;

	// Token: 0x020003D2 RID: 978
	// (Invoke) Token: 0x06001DBB RID: 7611
	public delegate void HordeArrivedDelegate();

	// Token: 0x020003D3 RID: 979
	public enum SpawnType
	{
		// Token: 0x04001466 RID: 5222
		Bandits,
		// Token: 0x04001467 RID: 5223
		Horde
	}

	// Token: 0x020003D4 RID: 980
	[PublicizedFrom(EAccessModifier.Private)]
	public enum ECommand
	{
		// Token: 0x04001469 RID: 5225
		PitStop,
		// Token: 0x0400146A RID: 5226
		Wander,
		// Token: 0x0400146B RID: 5227
		EndPos
	}

	// Token: 0x020003D5 RID: 981
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public class ZombieCommand
	{
		// Token: 0x0400146C RID: 5228
		public AIWanderingHordeSpawner.ECommand Command;

		// Token: 0x0400146D RID: 5229
		public EntityEnemy Enemy;

		// Token: 0x0400146E RID: 5230
		public float WanderTime;

		// Token: 0x0400146F RID: 5231
		public Vector3 TargetPos;
	}
}
