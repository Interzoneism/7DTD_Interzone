using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003C9 RID: 969
[Preserve]
public class AIDirectorWanderingHordeComponent : AIDirectorHordeComponent
{
	// Token: 0x06001D8B RID: 7563 RVA: 0x000B7EDF File Offset: 0x000B60DF
	public override void InitNewGame()
	{
		this.isPlaytest = GameUtils.IsPlaytesting();
		this.BanditNextTime = 0UL;
		this.HordeNextTime = 0UL;
	}

	// Token: 0x06001D8C RID: 7564 RVA: 0x000B7EFC File Offset: 0x000B60FC
	public override void Tick(double _dt)
	{
		if (this.isPlaytest)
		{
			return;
		}
		base.Tick(_dt);
		this.TickActiveSpawns((float)_dt);
		this.TickNextTime(ref this.HordeNextTime, AIWanderingHordeSpawner.SpawnType.Horde);
	}

	// Token: 0x06001D8D RID: 7565 RVA: 0x000B7F24 File Offset: 0x000B6124
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickActiveSpawns(float dt)
	{
		for (int i = this.spawners.Count - 1; i >= 0; i--)
		{
			AIWanderingHordeSpawner aiwanderingHordeSpawner = this.spawners[i];
			if (aiwanderingHordeSpawner.Update(this.Director.World, dt))
			{
				AIDirector.LogAIExtra("Wandering spawner finished {0}", new object[]
				{
					aiwanderingHordeSpawner.spawnType
				});
				aiwanderingHordeSpawner.Cleanup();
				this.spawners.RemoveAt(i);
			}
		}
	}

	// Token: 0x06001D8E RID: 7566 RVA: 0x000B7F9C File Offset: 0x000B619C
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickNextTime(ref ulong _nextTime, AIWanderingHordeSpawner.SpawnType _spawnType)
	{
		if (!GameStats.GetBool(EnumGameStats.ZombieHordeMeter) || !GameStats.GetBool(EnumGameStats.IsSpawnEnemies))
		{
			_nextTime = 0UL;
			return;
		}
		if (_nextTime == 0UL)
		{
			if (this.Director.World.worldTime > 28000UL)
			{
				this.ChooseNextTime(_spawnType);
				return;
			}
		}
		else
		{
			int num = (int)(_nextTime - this.Director.World.worldTime);
			int num2 = num / 1000;
			if (num2 < 7)
			{
				if (this.OtherHordesAreActive)
				{
					_nextTime += (ulong)((7 - num2) * 1000);
					return;
				}
				if (num <= 0)
				{
					if (this.Director.World.Players.Count > 0)
					{
						this.StartSpawning(_spawnType);
						return;
					}
					this.ChooseNextTime(_spawnType);
				}
			}
		}
	}

	// Token: 0x06001D8F RID: 7567 RVA: 0x000B8048 File Offset: 0x000B6248
	public override void Read(BinaryReader _stream, int _version)
	{
		base.Read(_stream, _version);
		this.HordeNextTime = _stream.ReadUInt64();
		if (_version > 3)
		{
			this.BanditNextTime = _stream.ReadUInt64();
		}
	}

	// Token: 0x06001D90 RID: 7568 RVA: 0x000B806E File Offset: 0x000B626E
	public override void Write(BinaryWriter _stream)
	{
		base.Write(_stream);
		_stream.Write(this.HordeNextTime);
		_stream.Write(this.BanditNextTime);
	}

	// Token: 0x06001D91 RID: 7569 RVA: 0x000B8090 File Offset: 0x000B6290
	public void StartSpawning(AIWanderingHordeSpawner.SpawnType _spawnType)
	{
		AIDirector.LogAI("Wandering StartSpawning {0}", new object[]
		{
			_spawnType
		});
		this.CleanupType(_spawnType);
		bool flag = false;
		DictionaryList<int, AIDirectorPlayerState> trackedPlayers = this.Director.GetComponent<AIDirectorPlayerManagementComponent>().trackedPlayers;
		for (int i = 0; i < trackedPlayers.list.Count; i++)
		{
			if (!trackedPlayers.list[i].Dead)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			AIDirector.LogAI("Spawn {0}, no living players, wait 4 hours", new object[]
			{
				_spawnType
			});
			this.SetNextTime(_spawnType, this.Director.World.worldTime + 4000UL);
			return;
		}
		List<AIDirectorPlayerState> list = new List<AIDirectorPlayerState>();
		Vector3 startPos;
		Vector3 pitStopPos;
		Vector3 endPos;
		uint num = base.FindTargets(out startPos, out pitStopPos, out endPos, list);
		if (num > 0U)
		{
			AIDirector.LogAI("Spawn {0}, find targets, wait {1} hours", new object[]
			{
				_spawnType,
				num
			});
			this.SetNextTime(_spawnType, this.Director.World.worldTime + (ulong)(1000U * num));
			return;
		}
		this.ChooseNextTime(_spawnType);
		this.spawners.Add(new AIWanderingHordeSpawner(this.Director, _spawnType, null, list, this.Director.World.worldTime + 12000UL, startPos, pitStopPos, endPos));
	}

	// Token: 0x06001D92 RID: 7570 RVA: 0x000B81DC File Offset: 0x000B63DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void CleanupType(AIWanderingHordeSpawner.SpawnType _spawnType)
	{
		for (int i = this.spawners.Count - 1; i >= 0; i--)
		{
			AIWanderingHordeSpawner aiwanderingHordeSpawner = this.spawners[i];
			if (aiwanderingHordeSpawner.spawnType == _spawnType)
			{
				aiwanderingHordeSpawner.Cleanup();
				this.spawners.RemoveAt(i);
			}
		}
	}

	// Token: 0x1700034E RID: 846
	// (get) Token: 0x06001D93 RID: 7571 RVA: 0x000B8229 File Offset: 0x000B6429
	public bool HasAnySpawns
	{
		get
		{
			return this.spawners.Count != 0;
		}
	}

	// Token: 0x1700034F RID: 847
	// (get) Token: 0x06001D94 RID: 7572 RVA: 0x000B8239 File Offset: 0x000B6439
	public bool OtherHordesAreActive
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return SkyManager.IsBloodMoonVisible() || this.Director.GetComponent<AIDirectorChunkEventComponent>().HasAnySpawns;
		}
	}

	// Token: 0x06001D95 RID: 7573 RVA: 0x000B8254 File Offset: 0x000B6454
	[PublicizedFrom(EAccessModifier.Private)]
	public void ChooseNextTime(AIWanderingHordeSpawner.SpawnType _spawnType)
	{
		if (_spawnType == AIWanderingHordeSpawner.SpawnType.Bandits)
		{
			this.BanditNextTime = this.Director.World.worldTime + (ulong)((long)base.Random.RandomRange(12000, 24000));
			this.BanditNextTime += 2000UL;
			return;
		}
		if (_spawnType == AIWanderingHordeSpawner.SpawnType.Horde)
		{
			this.HordeNextTime = this.Director.World.worldTime + (ulong)((long)base.Random.RandomRange(12000, 24000));
		}
	}

	// Token: 0x06001D96 RID: 7574 RVA: 0x000B82D6 File Offset: 0x000B64D6
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetNextTime(AIWanderingHordeSpawner.SpawnType _spawnType, ulong _time)
	{
		if (_spawnType == AIWanderingHordeSpawner.SpawnType.Bandits)
		{
			this.BanditNextTime = _time;
			return;
		}
		if (_spawnType == AIWanderingHordeSpawner.SpawnType.Horde)
		{
			this.HordeNextTime = _time;
		}
	}

	// Token: 0x06001D97 RID: 7575 RVA: 0x000B82EE File Offset: 0x000B64EE
	public void LogTimes()
	{
		AIDirector.LogAI("Next wandering - bandit {0}, horde {1}", new object[]
		{
			GameUtils.WorldTimeToString(this.BanditNextTime),
			GameUtils.WorldTimeToString(this.HordeNextTime)
		});
	}

	// Token: 0x04001438 RID: 5176
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cNextHourMin = 7;

	// Token: 0x04001439 RID: 5177
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPlaytest;

	// Token: 0x0400143A RID: 5178
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIWanderingHordeSpawner> spawners = new List<AIWanderingHordeSpawner>();

	// Token: 0x0400143B RID: 5179
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong BanditNextTime;

	// Token: 0x0400143C RID: 5180
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong HordeNextTime;
}
