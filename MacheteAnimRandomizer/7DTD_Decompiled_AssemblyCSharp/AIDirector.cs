using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Twitch;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003AD RID: 941
[Preserve]
public class AIDirector
{
	// Token: 0x06001CA2 RID: 7330 RVA: 0x000B328D File Offset: 0x000B148D
	public AIDirector(World _world)
	{
		this.World = _world;
		this.CreateComponents();
		this.Init();
	}

	// Token: 0x06001CA3 RID: 7331 RVA: 0x000B32BE File Offset: 0x000B14BE
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init()
	{
		this.random = GameRandomManager.Instance.CreateGameRandom();
		this.ComponentsInitNewGame();
	}

	// Token: 0x06001CA4 RID: 7332 RVA: 0x000B32D8 File Offset: 0x000B14D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateComponents()
	{
		this.CreateComponent<AIDirectorMarkerManagementComponent>();
		this.CreateComponent<AIDirectorPlayerManagementComponent>();
		this.CreateComponent<AIDirectorWanderingHordeComponent>();
		this.CreateComponent<AIDirectorAirDropComponent>();
		this.CreateComponent<AIDirectorChunkEventComponent>();
		this.CreateComponent<AIDirectorBloodMoonComponent>();
		this.playerManagementComponent = this.GetComponent<AIDirectorPlayerManagementComponent>();
		this.chunkEventComponent = this.GetComponent<AIDirectorChunkEventComponent>();
		this.bloodMoonComponent = this.GetComponent<AIDirectorBloodMoonComponent>();
	}

	// Token: 0x06001CA5 RID: 7333 RVA: 0x000B3334 File Offset: 0x000B1534
	[PublicizedFrom(EAccessModifier.Private)]
	public T CreateComponent<T>() where T : AIDirectorComponent, new()
	{
		string fullName = typeof(T).FullName;
		if (this.components.dict.ContainsKey(fullName))
		{
			throw new Exception("Multiple instances of the same component type are not allowed!");
		}
		T t = Activator.CreateInstance<T>();
		t.Director = this;
		this.components.Add(fullName, t);
		return t;
	}

	// Token: 0x06001CA6 RID: 7334 RVA: 0x000B3394 File Offset: 0x000B1594
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComponentsInitNewGame()
	{
		for (int i = 0; i < this.components.list.Count; i++)
		{
			this.components.list[i].InitNewGame();
		}
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x000B33D4 File Offset: 0x000B15D4
	public T GetComponent<T>() where T : AIDirectorComponent
	{
		string fullName = typeof(T).FullName;
		AIDirectorComponent aidirectorComponent;
		if (this.components.dict.TryGetValue(fullName, out aidirectorComponent))
		{
			return aidirectorComponent as T;
		}
		return default(T);
	}

	// Token: 0x17000331 RID: 817
	// (get) Token: 0x06001CA8 RID: 7336 RVA: 0x000B341B File Offset: 0x000B161B
	public AIDirectorBloodMoonComponent BloodMoonComponent
	{
		get
		{
			return this.bloodMoonComponent;
		}
	}

	// Token: 0x06001CA9 RID: 7337 RVA: 0x000B3424 File Offset: 0x000B1624
	public void Load(BinaryReader stream)
	{
		int version = stream.ReadInt32();
		this.ComponentsLoad(stream, version);
		if (this.World.worldTime == 0UL)
		{
			this.Init();
		}
	}

	// Token: 0x06001CAA RID: 7338 RVA: 0x000B3453 File Offset: 0x000B1653
	public void Save(BinaryWriter stream)
	{
		stream.Write(10);
		this.ComponentsSave(stream);
	}

	// Token: 0x06001CAB RID: 7339 RVA: 0x000B3464 File Offset: 0x000B1664
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComponentsLoad(BinaryReader reader, int version)
	{
		for (int i = 0; i < this.components.list.Count; i++)
		{
			this.components.list[i].Read(reader, version);
		}
	}

	// Token: 0x06001CAC RID: 7340 RVA: 0x000B34A4 File Offset: 0x000B16A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComponentsSave(BinaryWriter writer)
	{
		for (int i = 0; i < this.components.list.Count; i++)
		{
			this.components.list[i].Write(writer);
		}
	}

	// Token: 0x06001CAD RID: 7341 RVA: 0x000B34E3 File Offset: 0x000B16E3
	public void Tick(double dt)
	{
		this.ComponentsTick(dt);
		this.DebugTick();
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x000B34F4 File Offset: 0x000B16F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComponentsTick(double _dt)
	{
		for (int i = 0; i < this.components.list.Count; i++)
		{
			this.components.list[i].Tick(_dt);
		}
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x000B3533 File Offset: 0x000B1733
	public static bool CanSpawn(float _priority = 1f)
	{
		return (float)GameStats.GetInt(EnumGameStats.EnemyCount) < (float)GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies) * _priority;
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x000B354C File Offset: 0x000B174C
	public static ulong GetActivityWorldTimeDelay()
	{
		float num = (float)GameStats.GetInt(EnumGameStats.TimeOfDayIncPerSec) / 6f;
		num = Utils.FastClamp(num, 0.2f, 5f);
		return (ulong)(1000f * num);
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x000B3584 File Offset: 0x000B1784
	public void NotifyActivity(EnumAIDirectorChunkEvent type, Vector3i position, float value, float _duration = 720f)
	{
		if (value > 0f && GameStats.GetBool(EnumGameStats.ZombieHordeMeter) && GameStats.GetBool(EnumGameStats.IsSpawnEnemies) && !this.BloodMoonComponent.BloodMoonActive && !TwitchManager.BossHordeActive)
		{
			AIDirectorChunkEvent chunkEvent = new AIDirectorChunkEvent(type, position, value, _duration);
			this.chunkEventComponent.NotifyEvent(chunkEvent);
		}
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x000B35D8 File Offset: 0x000B17D8
	public void NotifyNoise(Entity instigator, Vector3 position, string clipName, float volumeScale)
	{
		AIDirectorData.Noise noise;
		if (!AIDirectorData.FindNoise(clipName, out noise))
		{
			return;
		}
		if (instigator is EntityEnemy)
		{
			return;
		}
		AIDirectorPlayerState aidirectorPlayerState = null;
		if (instigator)
		{
			if (instigator.IsIgnoredByAI())
			{
				return;
			}
			this.playerManagementComponent.trackedPlayers.dict.TryGetValue(instigator.entityId, out aidirectorPlayerState);
		}
		EntityItem entityItem = instigator as EntityItem;
		if (entityItem != null && ItemClass.GetForId(entityItem.itemStack.itemValue.type).ThrowableDecoy.Value)
		{
			return;
		}
		if (aidirectorPlayerState != null)
		{
			if (aidirectorPlayerState.Player.IsCrouching)
			{
				volumeScale *= noise.muffledWhenCrouched;
			}
			float volume = noise.volume * volumeScale;
			if (aidirectorPlayerState.Player.Stealth.NotifyNoise(volume, noise.duration))
			{
				instigator.world.CheckSleeperVolumeNoise(position);
			}
		}
		if (noise.heatMapStrength > 0f)
		{
			this.NotifyActivity(EnumAIDirectorChunkEvent.Sound, World.worldToBlockPos(position), noise.heatMapStrength * volumeScale, 240f);
		}
	}

	// Token: 0x06001CB3 RID: 7347 RVA: 0x00002914 File Offset: 0x00000B14
	public void NotifyIntentToAttack(EntityAlive zombie, EntityAlive player)
	{
	}

	// Token: 0x06001CB4 RID: 7348 RVA: 0x000B36C8 File Offset: 0x000B18C8
	public void UpdatePlayerInventory(EntityPlayerLocal player)
	{
		this.playerManagementComponent.UpdatePlayerInventory(player);
	}

	// Token: 0x06001CB5 RID: 7349 RVA: 0x000B36D6 File Offset: 0x000B18D6
	public void UpdatePlayerInventory(int entityId, AIDirectorPlayerInventory inventory)
	{
		this.playerManagementComponent.UpdatePlayerInventory(entityId, inventory);
	}

	// Token: 0x06001CB6 RID: 7350 RVA: 0x000B36E8 File Offset: 0x000B18E8
	public void OnSoundPlayedAtPosition(int _entityThatCausedSound, Vector3 _position, string clipName, float volumeScale)
	{
		Entity instigator = null;
		if (_entityThatCausedSound != -1)
		{
			instigator = this.World.GetEntity(_entityThatCausedSound);
		}
		this.NotifyNoise(instigator, _position, clipName, volumeScale);
	}

	// Token: 0x06001CB7 RID: 7351 RVA: 0x000B3714 File Offset: 0x000B1914
	public void AddEntity(Entity entity)
	{
		EntityPlayer player;
		if (player = (entity as EntityPlayer))
		{
			this.AddPlayer(player);
		}
	}

	// Token: 0x06001CB8 RID: 7352 RVA: 0x000B3738 File Offset: 0x000B1938
	public void RemoveEntity(Entity entity)
	{
		EntityPlayer player;
		if (player = (entity as EntityPlayer))
		{
			this.RemovePlayer(player);
		}
	}

	// Token: 0x06001CB9 RID: 7353 RVA: 0x000B375B File Offset: 0x000B195B
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddPlayer(EntityPlayer player)
	{
		this.playerManagementComponent.AddPlayer(player);
		this.BloodMoonComponent.AddPlayer(player);
	}

	// Token: 0x06001CBA RID: 7354 RVA: 0x000B3775 File Offset: 0x000B1975
	[PublicizedFrom(EAccessModifier.Private)]
	public void RemovePlayer(EntityPlayer player)
	{
		this.playerManagementComponent.RemovePlayer(player);
		this.BloodMoonComponent.RemovePlayer(player);
	}

	// Token: 0x06001CBB RID: 7355 RVA: 0x000B378F File Offset: 0x000B198F
	public static void LogAI(string _format, params object[] _args)
	{
		_format = string.Format("AIDirector: {0}", _format);
		Log.Out(_format, _args);
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x000B37A5 File Offset: 0x000B19A5
	public static void LogAIExtra(string _format, params object[] _args)
	{
		if (AIDirectorConstants.DebugOutput)
		{
			AIDirector.LogAI(_format, _args);
		}
	}

	// Token: 0x06001CBD RID: 7357 RVA: 0x000B37B5 File Offset: 0x000B19B5
	public void DebugFrameLateUpdate()
	{
		if (AIDirector.debugSendLatencyToPlayerIds.Count > 0)
		{
			this.DebugSendLatency();
		}
	}

	// Token: 0x06001CBE RID: 7358 RVA: 0x000B37CA File Offset: 0x000B19CA
	[PublicizedFrom(EAccessModifier.Private)]
	public void DebugTick()
	{
		if (AIDirector.debugSendNameInfoToPlayerIds.Count > 0)
		{
			this.DebugSendNameInfo();
		}
	}

	// Token: 0x06001CBF RID: 7359 RVA: 0x000B37E0 File Offset: 0x000B19E0
	public static void DebugToggleSendNameInfo(int playerId)
	{
		if (AIDirector.debugSendNameInfoToPlayerIds.Remove(playerId))
		{
			Log.Out("DebugToggleSendNames {0} off", new object[]
			{
				playerId
			});
			NetPackageDebug package = NetPackageManager.GetPackage<NetPackageDebug>().Setup(NetPackageDebug.Type.AINameInfoClientOff, -1, null);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, playerId, -1, -1, null, 192, false);
			return;
		}
		Log.Out("DebugToggleSendNames {0} on", new object[]
		{
			playerId
		});
		AIDirector.debugSendNameInfoToPlayerIds.Add(playerId);
	}

	// Token: 0x06001CC0 RID: 7360 RVA: 0x000B3864 File Offset: 0x000B1A64
	[PublicizedFrom(EAccessModifier.Private)]
	public void DebugSendNameInfo()
	{
		int num = this.debugNameInfoTicks - 1;
		this.debugNameInfoTicks = num;
		if (num > 0)
		{
			return;
		}
		this.debugNameInfoTicks = 5;
		World world = GameManager.Instance.World;
		for (int i = 0; i < AIDirector.debugSendNameInfoToPlayerIds.Count; i++)
		{
			int num2 = AIDirector.debugSendNameInfoToPlayerIds[i];
			EntityPlayer entityPlayer;
			world.Players.dict.TryGetValue(num2, out entityPlayer);
			if (entityPlayer)
			{
				ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(num2);
				if (clientInfo != null)
				{
					Bounds bb = new Bounds(entityPlayer.position, new Vector3(50f, 50f, 50f));
					world.GetEntitiesInBounds(typeof(EntityAlive), bb, this.debugEntities);
					for (int j = this.debugEntities.Count - 1; j >= 0; j--)
					{
						EntityAlive entityAlive = (EntityAlive)this.debugEntities[j];
						if (entityAlive.aiManager != null)
						{
							string s = entityAlive.aiManager.MakeDebugName(entityPlayer);
							NetPackageDebug package = NetPackageManager.GetPackage<NetPackageDebug>().Setup(NetPackageDebug.Type.AINameInfo, entityAlive.entityId, Encoding.UTF8.GetBytes(s));
							clientInfo.SendPackage(package);
						}
					}
					this.debugEntities.Clear();
				}
			}
		}
	}

	// Token: 0x06001CC1 RID: 7361 RVA: 0x000B39B0 File Offset: 0x000B1BB0
	public static void DebugReceiveNameInfo(int entityId, byte[] _data)
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		EntityAlive entityAlive = world.GetEntity(entityId) as EntityAlive;
		if (entityAlive)
		{
			entityAlive.SetupDebugNameHUD(true);
			string @string = Encoding.UTF8.GetString(_data);
			entityAlive.DebugNameInfo = @string;
		}
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x000B39FB File Offset: 0x000B1BFB
	public static void DebugToggleFreezePos()
	{
		AIDirector.debugFreezePos = !AIDirector.debugFreezePos;
		Log.Out("DebugToggleFreezePos {0}", new object[]
		{
			AIDirector.debugFreezePos
		});
	}

	// Token: 0x06001CC3 RID: 7363 RVA: 0x000B3A28 File Offset: 0x000B1C28
	public static void DebugToggleSendLatency(int playerId)
	{
		if (!AIDirector.debugSendLatencyToPlayerIds.Remove(playerId))
		{
			Log.Out("DebugToggleSendLatency {0} on", new object[]
			{
				playerId
			});
			AIDirector.debugSendLatencyToPlayerIds.Add(playerId);
			return;
		}
		Log.Out("DebugToggleSendLatency {0} off", new object[]
		{
			playerId
		});
		if (GameManager.Instance.World.GetPrimaryPlayerId() != playerId)
		{
			NetPackageDebug package = NetPackageManager.GetPackage<NetPackageDebug>().Setup(NetPackageDebug.Type.AILatencyClientOff, -1, null);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, playerId, -1, -1, null, 192, false);
			return;
		}
		AIDirector.DebugLatencyOff();
	}

	// Token: 0x06001CC4 RID: 7364 RVA: 0x000B3AC4 File Offset: 0x000B1CC4
	[PublicizedFrom(EAccessModifier.Private)]
	public void DebugSendLatency()
	{
		World world = GameManager.Instance.World;
		for (int i = 0; i < AIDirector.debugSendLatencyToPlayerIds.Count; i++)
		{
			int num = AIDirector.debugSendLatencyToPlayerIds[i];
			EntityPlayer entityPlayer;
			world.Players.dict.TryGetValue(num, out entityPlayer);
			if (entityPlayer)
			{
				ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(num);
				Bounds bb = new Bounds(entityPlayer.position, new Vector3(50f, 50f, 50f));
				world.GetEntitiesInBounds(typeof(EntityAlive), bb, this.debugEntities);
				for (int j = this.debugEntities.Count - 1; j >= 0; j--)
				{
					EntityAlive entityAlive = (EntityAlive)this.debugEntities[j];
					if (entityAlive.aiManager != null)
					{
						using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
						{
							pooledBinaryWriter.SetBaseStream(AIDirector.latencyStream);
							AIDirector.latencyStream.Position = 0L;
							pooledBinaryWriter.Write(entityAlive.position.x);
							pooledBinaryWriter.Write(entityAlive.position.y);
							pooledBinaryWriter.Write(entityAlive.position.z);
							Vector3 vector = entityAlive.GetVelocityPerSecond();
							Vector3 vector2 = entityAlive.motion * 20f;
							if (vector.sqrMagnitude < vector2.sqrMagnitude)
							{
								vector = vector2;
							}
							pooledBinaryWriter.Write(vector.x);
							pooledBinaryWriter.Write(vector.y);
							pooledBinaryWriter.Write(vector.z);
							Quaternion rotation = entityAlive.transform.rotation;
							pooledBinaryWriter.Write(rotation.x);
							pooledBinaryWriter.Write(rotation.y);
							pooledBinaryWriter.Write(rotation.z);
							pooledBinaryWriter.Write(rotation.w);
							byte[] data = AIDirector.latencyStream.ToArray();
							if (clientInfo != null)
							{
								NetPackageDebug package = NetPackageManager.GetPackage<NetPackageDebug>().Setup(NetPackageDebug.Type.AILatency, entityAlive.entityId, data);
								clientInfo.SendPackage(package);
							}
							else
							{
								AIDirector.DebugReceiveLatency(entityAlive.entityId, data);
							}
						}
					}
				}
				this.debugEntities.Clear();
			}
		}
	}

	// Token: 0x06001CC5 RID: 7365 RVA: 0x000B3D24 File Offset: 0x000B1F24
	public static void DebugReceiveLatency(int entityId, byte[] _data)
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		EntityAlive entityAlive = world.GetEntity(entityId) as EntityAlive;
		if (entityAlive)
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				MemoryStream baseStream = new MemoryStream(_data);
				pooledBinaryReader.SetBaseStream(baseStream);
				Vector3 a;
				a.x = pooledBinaryReader.ReadSingle();
				a.y = pooledBinaryReader.ReadSingle();
				a.z = pooledBinaryReader.ReadSingle();
				Vector3 vector;
				vector.x = pooledBinaryReader.ReadSingle();
				vector.y = pooledBinaryReader.ReadSingle();
				vector.z = pooledBinaryReader.ReadSingle();
				Quaternion rotation;
				rotation.x = pooledBinaryReader.ReadSingle();
				rotation.y = pooledBinaryReader.ReadSingle();
				rotation.z = pooledBinaryReader.ReadSingle();
				rotation.w = pooledBinaryReader.ReadSingle();
				Transform transform = entityAlive.transform;
				Transform parent = transform.parent;
				Transform transform2 = parent.Find("DebugLatency");
				if (!transform2)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Debug/DebugLatency"), parent);
					gameObject.name = "DebugLatency";
					transform2 = gameObject.transform;
				}
				Vector3 vector2 = a - Origin.position;
				transform2.position = vector2;
				transform2.rotation = rotation;
				LineRenderer component = transform2.GetComponent<LineRenderer>();
				component.SetPosition(0, Quaternion.Inverse(rotation) * (transform.position - vector2));
				float num = (float)world.GetPrimaryPlayer().pingToServer * 0.001f;
				if (num < 0f)
				{
					num = 0f;
				}
				num *= 2f;
				if (vector.y < 0f)
				{
					vector.y = 0f;
				}
				component.SetPosition(2, Quaternion.Inverse(rotation) * (vector * num));
			}
		}
	}

	// Token: 0x06001CC6 RID: 7366 RVA: 0x000B3F18 File Offset: 0x000B2118
	public static void DebugLatencyOff()
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		for (int i = 0; i < world.Entities.list.Count; i++)
		{
			EntityAlive entityAlive = world.Entities.list[i] as EntityAlive;
			if (entityAlive)
			{
				Transform transform = entityAlive.transform.parent.Find("DebugLatency");
				if (transform)
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
		}
	}

	// Token: 0x04001369 RID: 4969
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cActivityDuration = 720f;

	// Token: 0x0400136A RID: 4970
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cActivityNoiseDuration = 240f;

	// Token: 0x0400136B RID: 4971
	public readonly World World;

	// Token: 0x0400136C RID: 4972
	public GameRandom random;

	// Token: 0x0400136D RID: 4973
	[PublicizedFrom(EAccessModifier.Private)]
	public DictionaryList<string, AIDirectorComponent> components = new DictionaryList<string, AIDirectorComponent>();

	// Token: 0x0400136E RID: 4974
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorPlayerManagementComponent playerManagementComponent;

	// Token: 0x0400136F RID: 4975
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorChunkEventComponent chunkEventComponent;

	// Token: 0x04001370 RID: 4976
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorBloodMoonComponent bloodMoonComponent;

	// Token: 0x04001371 RID: 4977
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Entity> debugEntities = new List<Entity>();

	// Token: 0x04001372 RID: 4978
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cDebugSendNameInfoTickRate = 5;

	// Token: 0x04001373 RID: 4979
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<int> debugSendNameInfoToPlayerIds = new List<int>();

	// Token: 0x04001374 RID: 4980
	[PublicizedFrom(EAccessModifier.Private)]
	public int debugNameInfoTicks;

	// Token: 0x04001375 RID: 4981
	public static bool debugFreezePos;

	// Token: 0x04001376 RID: 4982
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cLatencyName = "DebugLatency";

	// Token: 0x04001377 RID: 4983
	[PublicizedFrom(EAccessModifier.Private)]
	public static MemoryStream latencyStream = new MemoryStream();

	// Token: 0x04001378 RID: 4984
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<int> debugSendLatencyToPlayerIds = new List<int>();

	// Token: 0x020003AE RID: 942
	public enum HordeEvent
	{
		// Token: 0x0400137A RID: 4986
		None,
		// Token: 0x0400137B RID: 4987
		Warn1,
		// Token: 0x0400137C RID: 4988
		Warn2,
		// Token: 0x0400137D RID: 4989
		Spawn
	}
}
