using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200097B RID: 2427
public class EntitySpawner
{
	// Token: 0x170007BA RID: 1978
	// (get) Token: 0x06004946 RID: 18758 RVA: 0x001CF7F3 File Offset: 0x001CD9F3
	public int CurrentWave
	{
		get
		{
			return this.currentWave;
		}
	}

	// Token: 0x06004947 RID: 18759 RVA: 0x001CF7FB File Offset: 0x001CD9FB
	public static int ModifySpawnCountByGameDifficulty(int count)
	{
		if (count < 1)
		{
			return count;
		}
		return (int)Mathf.Max(Mathf.Floor((GameStats.GetBool(EnumGameStats.EnemySpawnMode) ? 1f : 0f) * (float)count + 0.5f), 1f);
	}

	// Token: 0x06004948 RID: 18760 RVA: 0x001CF834 File Offset: 0x001CDA34
	public EntitySpawner()
	{
		this.position = Vector3i.zero;
		this.size = Vector3i.one;
		this.totalSpawnedThisWave = 0;
		this.timeDelayToNextWave = 0f;
		this.timeDelayBetweenSpawns = 0f;
		this.currentWave = 0;
		this.numberToSpawnThisWave = 0;
		this.lastDaySpawnCalled = -1;
		this.bCaveSpawn = false;
		PList<int> plist = new PList<int>();
		plist.writeElement = delegate(BinaryWriter _bw, int _el)
		{
			_bw.Write(_el);
		};
		plist.readElement = ((BinaryReader _br, uint _version) => _br.ReadInt32());
		this.entityIdSpawned = plist;
	}

	// Token: 0x06004949 RID: 18761 RVA: 0x001CF8EC File Offset: 0x001CDAEC
	public EntitySpawner(string _esClassname, Vector3i _position, Vector3i _size, int _triggerDiameter, ICollection<int> _entityIdsAlreadySpawned = null) : this()
	{
		this.position = _position;
		this.size = _size;
		this.triggerDiameter = _triggerDiameter;
		this.entitySpawnerClassName = _esClassname;
		if (_entityIdsAlreadySpawned != null)
		{
			this.entityIdSpawned.AddRange(_entityIdsAlreadySpawned);
		}
		this.timeDelayBetweenSpawns = (EntitySpawnerClass.list.ContainsKey(this.entitySpawnerClassName) ? EntitySpawnerClass.list[this.entitySpawnerClassName].Day(0).delayBetweenSpawns : 0f);
	}

	// Token: 0x0600494A RID: 18762 RVA: 0x001CF967 File Offset: 0x001CDB67
	public ICollection<int> GetEntityIdsSpaned()
	{
		return this.entityIdSpawned;
	}

	// Token: 0x0600494B RID: 18763 RVA: 0x001CF970 File Offset: 0x001CDB70
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(3);
		_bw.Write(this.position.x);
		_bw.Write(this.position.y);
		_bw.Write(this.position.z);
		_bw.Write((short)this.size.x);
		_bw.Write((short)this.size.y);
		_bw.Write((short)this.size.z);
		_bw.Write((ushort)this.triggerDiameter);
		_bw.Write(this.entitySpawnerClassName);
		_bw.Write((short)this.totalSpawnedThisWave);
		_bw.Write(this.timeDelayToNextWave);
		_bw.Write(this.timeDelayBetweenSpawns);
		this.entityIdSpawned.Write(_bw);
		_bw.Write((short)this.currentWave);
		_bw.Write(this.lastDaySpawnCalled);
		_bw.Write(this.numberToSpawnThisWave);
		_bw.Write(this.worldTimeNextWave);
		_bw.Write(this.bCaveSpawn);
	}

	// Token: 0x0600494C RID: 18764 RVA: 0x001CFA78 File Offset: 0x001CDC78
	public void Read(BinaryReader _br)
	{
		byte b = _br.ReadByte();
		this.position = new Vector3i(_br.ReadInt32(), _br.ReadInt32(), _br.ReadInt32());
		this.size = new Vector3i((int)_br.ReadInt16(), (int)_br.ReadInt16(), (int)_br.ReadInt16());
		this.triggerDiameter = (int)_br.ReadUInt16();
		this.entitySpawnerClassName = _br.ReadString();
		if (!EntitySpawnerClass.list.ContainsKey(this.entitySpawnerClassName))
		{
			string[] array = new string[5];
			array[0] = "Entity spawner at pos ";
			int num = 1;
			Vector3i vector3i = this.position;
			array[num] = vector3i.ToString();
			array[2] = " contains invalid spawner class reference '";
			array[3] = this.entitySpawnerClassName;
			array[4] = "'";
			Log.Warning(string.Concat(array));
			this.entitySpawnerClassName = EntitySpawnerClass.DefaultClassName.name;
		}
		this.totalSpawnedThisWave = (int)_br.ReadInt16();
		this.timeDelayToNextWave = _br.ReadSingle();
		this.timeDelayBetweenSpawns = _br.ReadSingle();
		this.entityIdSpawned.Read(_br);
		this.currentWave = (int)_br.ReadInt16();
		this.lastDaySpawnCalled = _br.ReadInt32();
		this.numberToSpawnThisWave = _br.ReadInt32();
		if (b > 1)
		{
			this.worldTimeNextWave = _br.ReadUInt64();
		}
		if (b > 2)
		{
			this.bCaveSpawn = _br.ReadBoolean();
		}
	}

	// Token: 0x0600494D RID: 18765 RVA: 0x001CFBBC File Offset: 0x001CDDBC
	public void Spawn(World _world, int _day, bool _bSpawnEnemies)
	{
		if (!AIDirector.CanSpawn(1f))
		{
			return;
		}
		this.SpawnManually(_world, _day, _bSpawnEnemies, delegate(EntitySpawner _es, out EntityPlayer _outPlayerToAttack)
		{
			_outPlayerToAttack = null;
			EntitySpawnerClass entitySpawnerClass = EntitySpawnerClass.list[_es.entitySpawnerClassName].Day(_day);
			if (entitySpawnerClass.bIgnoreTrigger)
			{
				if (entitySpawnerClass.bAttackPlayerImmediately)
				{
					_outPlayerToAttack = _world.GetClosestPlayer((float)this.position.x, (float)this.position.y, (float)this.position.z, 0, 160.0);
				}
				return true;
			}
			for (int i = 0; i < _world.Players.list.Count; i++)
			{
				if (_outPlayerToAttack == null)
				{
					Vector3 vector = _world.Players.list[i].GetPosition();
					if (Mathf.Abs(vector.x - (float)this.position.x) <= (float)(_es.triggerDiameter / 2) && Mathf.Abs(vector.y - (float)this.position.y) <= (float)(_es.triggerDiameter / 2) && Mathf.Abs(vector.z - (float)this.position.z) <= (float)(_es.triggerDiameter / 2))
					{
						_outPlayerToAttack = _world.Players.list[i];
					}
				}
				for (int j = 0; j < _world.Players.list[i].SpawnPoints.Count; j++)
				{
					Vector3 vector2 = _world.Players.list[i].SpawnPoints[j].ToVector3();
					if (Mathf.Abs(vector2.x - (float)this.position.x) <= (float)(_es.triggerDiameter / 2) && Mathf.Abs(vector2.y - (float)this.position.y) <= (float)(_es.triggerDiameter / 2) && Mathf.Abs(vector2.z - (float)this.position.z) <= (float)(_es.triggerDiameter / 2))
					{
						_outPlayerToAttack = null;
						return false;
					}
				}
			}
			return _outPlayerToAttack != null;
		}, delegate(EntitySpawner _es, EntityPlayer _inPlayerToAttack, out EntityPlayer _outPlayerToAttack, out Vector3 _pos)
		{
			_outPlayerToAttack = _inPlayerToAttack;
			int num;
			int num2;
			int num3;
			if (this.bCaveSpawn)
			{
				if (!_world.FindRandomSpawnPointNearPositionUnderground(_es.position.ToVector3(), 16, out num, out num2, out num3, _es.size.ToVector3()))
				{
					_pos = Vector3.zero;
					return false;
				}
			}
			else
			{
				bool bSpawnOnGround = EntitySpawnerClass.list[_es.entitySpawnerClassName].Day(_day).bSpawnOnGround;
				if (!_world.FindRandomSpawnPointNearPosition(_es.position.ToVector3(), 16, out num, out num2, out num3, _es.size.ToVector3(), bSpawnOnGround, true))
				{
					_pos = Vector3.zero;
					return false;
				}
			}
			_pos = new Vector3((float)num, (float)num2, (float)num3);
			return true;
		}, null, null);
	}

	// Token: 0x0600494E RID: 18766 RVA: 0x001CFC1E File Offset: 0x001CDE1E
	public void ResetSpawner()
	{
		this.resetRuntimeVariables();
	}

	// Token: 0x0600494F RID: 18767 RVA: 0x001CFC26 File Offset: 0x001CDE26
	[PublicizedFrom(EAccessModifier.Private)]
	public void resetRuntimeVariables()
	{
		this.totalSpawnedThisWave = 0;
		this.timeDelayToNextWave = 0f;
		this.timeDelayBetweenSpawns = 0f;
		this.entityIdSpawned.Clear();
		this.currentWave = 0;
		this.numberToSpawnThisWave = 0;
	}

	// Token: 0x06004950 RID: 18768 RVA: 0x001CFC60 File Offset: 0x001CDE60
	public void SpawnManually(World _world, int _day, bool _bSpawnEnemyEntities, EntitySpawner.ES_CheckSpawnPrecondition _checkSpawnPrecondition, EntitySpawner.ES_GetSpawnPosition _getSpawnPosition, EntitySpawner.ES_ModifySpawnCount _checkSpawnCount, List<Entity> _spawned)
	{
		if (this.entitySpawnerClassName == null)
		{
			return;
		}
		EntitySpawnerClassForDay entitySpawnerClassForDay = EntitySpawnerClass.list[this.entitySpawnerClassName];
		if (entitySpawnerClassForDay == null)
		{
			return;
		}
		EntitySpawnerClass entitySpawnerClass = entitySpawnerClassForDay.Day(_day);
		if (this.lastDaySpawnCalled != -1 && this.lastDaySpawnCalled != _day && entitySpawnerClass.bPropResetToday)
		{
			this.resetRuntimeVariables();
		}
		this.lastDaySpawnCalled = _day;
		if (entitySpawnerClass.numberOfWaves > 0 && this.currentWave >= entitySpawnerClass.numberOfWaves)
		{
			return;
		}
		if (entitySpawnerClass.spawnAtTimeOfDay != EDaytime.Any && !(_world.IsDaytime() ? (entitySpawnerClass.spawnAtTimeOfDay == EDaytime.Day) : (entitySpawnerClass.spawnAtTimeOfDay == EDaytime.Night)))
		{
			return;
		}
		float num = Time.time - this.lastTimeSpawnCalled;
		this.lastTimeSpawnCalled = Time.time;
		if (this.timeDelayToNextWave > 0f)
		{
			this.timeDelayToNextWave -= num;
			return;
		}
		if (this.timeDelayBetweenSpawns > 0f)
		{
			this.timeDelayBetweenSpawns -= num;
			if (this.timeDelayBetweenSpawns > 0f)
			{
				return;
			}
		}
		EntityPlayer entityPlayer;
		bool flag = _checkSpawnPrecondition(this, out entityPlayer);
		if (entityPlayer != null && entitySpawnerClass.daysToRespawnIfPlayerLeft != 0 && this.worldTimeNextWave != 0UL && this.worldTimeNextWave < _world.worldTime)
		{
			this.worldTimeNextWave = _world.worldTime + (ulong)((long)entitySpawnerClass.daysToRespawnIfPlayerLeft * 24000L);
		}
		if (this.worldTimeNextWave != 0UL && this.worldTimeNextWave >= _world.worldTime)
		{
			return;
		}
		this.worldTimeNextWave = 0UL;
		if (!flag)
		{
			return;
		}
		for (int i = 0; i < this.entityIdSpawned.Count; i++)
		{
			Entity entity;
			if ((entity = _world.GetEntity(this.entityIdSpawned[i])) == null || entity.IsDead())
			{
				this.entityIdSpawned.MarkToRemove(this.entityIdSpawned[i]);
			}
		}
		this.entityIdSpawned.RemoveAllMarked();
		if (this.numberToSpawnThisWave == 0)
		{
			this.numberToSpawnThisWave = _world.GetGameRandom().RandomRange(EntitySpawner.ModifySpawnCountByGameDifficulty(entitySpawnerClass.totalPerWaveMin), EntitySpawner.ModifySpawnCountByGameDifficulty(entitySpawnerClass.totalPerWaveMax + 1));
			if (_checkSpawnCount != null)
			{
				this.numberToSpawnThisWave = _checkSpawnCount(this, this.numberToSpawnThisWave);
			}
			Log.Out("Spawning this wave: " + this.numberToSpawnThisWave.ToString());
		}
		if (this.totalSpawnedThisWave >= this.numberToSpawnThisWave)
		{
			if ((float)this.entityIdSpawned.Count <= Utils.FastMax((float)this.numberToSpawnThisWave * 0.2f, 1f))
			{
				this.timeDelayToNextWave = entitySpawnerClass.delayToNextWave;
				if (entitySpawnerClass.daysToRespawnIfPlayerLeft > 0)
				{
					this.worldTimeNextWave = _world.worldTime + (ulong)((long)entitySpawnerClass.daysToRespawnIfPlayerLeft * 24000L);
				}
				Log.Out(string.Concat(new string[]
				{
					"Start a new wave '",
					this.entitySpawnerClassName,
					"'. timeout=",
					this.timeDelayToNextWave.ToCultureInvariantString(),
					"s. worldtime=",
					this.worldTimeNextWave.ToString()
				}));
				this.totalSpawnedThisWave = 0;
				this.numberToSpawnThisWave = 0;
				this.currentWave++;
			}
			return;
		}
		if (this.entityIdSpawned.Count >= entitySpawnerClass.totalAlive)
		{
			return;
		}
		int num2 = 1;
		if (entitySpawnerClass.delayBetweenSpawns == 0f)
		{
			num2 = Utils.FastMin(this.numberToSpawnThisWave, entitySpawnerClass.totalAlive - this.entityIdSpawned.Count);
		}
		for (int j = 0; j < num2; j++)
		{
			Vector3 vector;
			if (_getSpawnPosition(this, entityPlayer, out entityPlayer, out vector))
			{
				int randomFromGroup = EntityGroups.GetRandomFromGroup(entitySpawnerClass.entityGroupName, ref this.lastClassId, null);
				if (_bSpawnEnemyEntities || !EntityClass.list[randomFromGroup].bIsEnemyEntity)
				{
					Entity entity2 = EntityFactory.CreateEntity(randomFromGroup, vector, new Vector3(0f, _world.GetGameRandom().RandomFloat * 360f, 0f));
					_world.SpawnEntityInWorld(entity2);
					this.entityIdSpawned.Add(entity2.entityId);
					if (_spawned != null)
					{
						_spawned.Add(entity2);
					}
					if (this.totalSpawnedThisWave == 0 && entitySpawnerClass.startSound != null && entitySpawnerClass.startSound.Length > 0)
					{
						_world.GetGameManager().PlaySoundAtPositionServer(this.position.ToVector3(), entitySpawnerClass.startSound, AudioRolloffMode.Custom, 300);
					}
					this.totalSpawnedThisWave++;
					EntityAlive entityAlive = entity2 as EntityAlive;
					if (entityAlive)
					{
						if (entitySpawnerClass.bAttackPlayerImmediately && entityPlayer != null)
						{
							entityAlive.SetRevengeTarget(entityPlayer);
						}
						if (entitySpawnerClass.bTerritorial)
						{
							entityAlive.setHomeArea(new Vector3i(vector), entitySpawnerClass.territorialRange);
						}
					}
					entity2.SetSpawnerSource(entitySpawnerClassForDay.bDynamicSpawner ? EnumSpawnerSource.Dynamic : EnumSpawnerSource.StaticSpawner);
					string[] array = new string[10];
					array[0] = "Spawned ";
					int num3 = 1;
					Entity entity3 = entity2;
					array[num3] = ((entity3 != null) ? entity3.ToString() : null);
					array[2] = " at ";
					array[3] = vector.ToCultureInvariantString();
					array[4] = " Day=";
					array[5] = _day.ToString();
					array[6] = " TotalInWave=";
					array[7] = this.totalSpawnedThisWave.ToString();
					array[8] = " CurrentWave=";
					array[9] = (this.currentWave + 1).ToString();
					Log.Out(string.Concat(array));
					_world.DebugAddSpawnedEntity(entity2);
					this.timeDelayBetweenSpawns = entitySpawnerClass.delayBetweenSpawns;
				}
			}
		}
	}

	// Token: 0x0400386C RID: 14444
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cFileVersion = 3;

	// Token: 0x0400386D RID: 14445
	public Vector3i position;

	// Token: 0x0400386E RID: 14446
	public Vector3i size;

	// Token: 0x0400386F RID: 14447
	public int triggerDiameter;

	// Token: 0x04003870 RID: 14448
	public string entitySpawnerClassName;

	// Token: 0x04003871 RID: 14449
	public bool bCaveSpawn;

	// Token: 0x04003872 RID: 14450
	[PublicizedFrom(EAccessModifier.Protected)]
	public int totalSpawnedThisWave;

	// Token: 0x04003873 RID: 14451
	[PublicizedFrom(EAccessModifier.Protected)]
	public float timeDelayToNextWave;

	// Token: 0x04003874 RID: 14452
	[PublicizedFrom(EAccessModifier.Protected)]
	public float timeDelayBetweenSpawns;

	// Token: 0x04003875 RID: 14453
	[PublicizedFrom(EAccessModifier.Protected)]
	public ulong worldTimeNextWave;

	// Token: 0x04003876 RID: 14454
	[PublicizedFrom(EAccessModifier.Protected)]
	public PList<int> entityIdSpawned;

	// Token: 0x04003877 RID: 14455
	[PublicizedFrom(EAccessModifier.Protected)]
	public int currentWave;

	// Token: 0x04003878 RID: 14456
	[PublicizedFrom(EAccessModifier.Protected)]
	public int lastDaySpawnCalled;

	// Token: 0x04003879 RID: 14457
	public int numberToSpawnThisWave;

	// Token: 0x0400387A RID: 14458
	[PublicizedFrom(EAccessModifier.Protected)]
	public float lastTimeSpawnCalled;

	// Token: 0x0400387B RID: 14459
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastClassId;

	// Token: 0x0200097C RID: 2428
	// (Invoke) Token: 0x06004952 RID: 18770
	public delegate bool ES_CheckSpawnPrecondition(EntitySpawner _es, out EntityPlayer _outPlayerToAttack);

	// Token: 0x0200097D RID: 2429
	// (Invoke) Token: 0x06004956 RID: 18774
	public delegate bool ES_GetSpawnPosition(EntitySpawner _es, EntityPlayer _inPlayerToAttack, out EntityPlayer _outPlayerToAttack, out Vector3 _pos);

	// Token: 0x0200097E RID: 2430
	// (Invoke) Token: 0x0600495A RID: 18778
	public delegate int ES_ModifySpawnCount(EntitySpawner _es, int spawnCount);
}
