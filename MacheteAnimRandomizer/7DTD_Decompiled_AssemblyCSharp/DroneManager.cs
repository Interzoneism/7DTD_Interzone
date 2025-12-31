using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000395 RID: 917
[Preserve]
public class DroneManager
{
	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06001B53 RID: 6995 RVA: 0x000AB344 File Offset: 0x000A9544
	public static DroneManager Instance
	{
		get
		{
			return DroneManager.instance;
		}
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x000AB38A File Offset: 0x000A958A
	public static void Init()
	{
		DroneManager.instance = new DroneManager();
		DroneManager.instance.Load();
	}

	// Token: 0x06001B56 RID: 6998 RVA: 0x000AB3A0 File Offset: 0x000A95A0
	public void AddTrackedDrone(EntityDrone _drone)
	{
		if (!_drone)
		{
			Log.Error("{0} AddTrackedDrone null", new object[]
			{
				base.GetType()
			});
			return;
		}
		_drone.OnWakeUp();
		if (!this.dronesActive.Contains(_drone))
		{
			this.dronesActive.Add(_drone);
			this.TriggerSave();
		}
	}

	// Token: 0x06001B57 RID: 6999 RVA: 0x000AB3F8 File Offset: 0x000A95F8
	public void RemoveTrackedDrone(EntityDrone _drone, EnumRemoveEntityReason _reason)
	{
		this.dronesActive.Remove(_drone);
		if (_reason == EnumRemoveEntityReason.Unloaded)
		{
			EntityAlive owner = _drone.Owner;
			if (owner)
			{
				OwnedEntityData ownedEntity = owner.GetOwnedEntity(_drone.entityId);
				if (ownedEntity != null)
				{
					ownedEntity.SetLastKnownPosition(_drone.position);
					EntityClass entityClass = EntityClass.list[ownedEntity.ClassId];
				}
			}
			this.dronesUnloaded.Add(new EntityCreationData(_drone, true));
		}
		this.TriggerSave();
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageVehicleCount>().Setup(), false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x06001B58 RID: 7000 RVA: 0x000AB491 File Offset: 0x000A9691
	public void TriggerSave()
	{
		this.saveTime = Mathf.Min(this.saveTime, 10f);
	}

	// Token: 0x06001B59 RID: 7001 RVA: 0x000AB4AC File Offset: 0x000A96AC
	public void Update()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		World world = GameManager.Instance.World;
		if (world == null || world.Players == null || world.Players.Count == 0)
		{
			return;
		}
		if (!GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		int num = 0;
		for (int i = this.dronesUnloaded.Count - 1; i >= 0; i--)
		{
			EntityCreationData entityCreationData = this.dronesUnloaded[i];
			EntityDrone entityDrone = world.GetEntity(entityCreationData.id) as EntityDrone;
			if (entityDrone)
			{
				Log.Warning("{0} already loaded #{1}, id {2}, {3}, {4}", new object[]
				{
					base.GetType(),
					i,
					entityCreationData.id,
					entityDrone,
					entityDrone.position.ToCultureInvariantString()
				});
				this.dronesUnloaded.RemoveAt(i);
			}
			else if (world.IsChunkAreaCollidersLoaded(entityCreationData.pos))
			{
				if (!this.isValidDronePos(entityCreationData.pos))
				{
					bool flag = false;
					IDictionary<PlatformUserIdentifierAbs, PersistentPlayerData> players = GameManager.Instance.GetPersistentPlayerList().Players;
					if (players != null)
					{
						foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in players)
						{
							EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(keyValuePair.Value.EntityId) as EntityPlayer;
							if (entityPlayer)
							{
								OwnedEntityData[] ownedEntities = entityPlayer.GetOwnedEntities();
								for (int j = 0; j < ownedEntities.Length; j++)
								{
									if (entityCreationData.id == ownedEntities[j].Id)
									{
										Log.Warning("recovering {0} owned entity for {1}", new object[]
										{
											entityCreationData.id,
											entityPlayer.entityId
										});
										entityCreationData.pos = entityPlayer.getHeadPosition();
										entityCreationData.belongsPlayerId = entityPlayer.entityId;
										flag = true;
										break;
									}
								}
							}
						}
					}
					if (!flag)
					{
						entityCreationData.pos = Vector3.zero;
						if (entityCreationData.belongsPlayerId == -1)
						{
							this.dronesWithoutOwner.Add(entityCreationData);
							this.dronesUnloaded.RemoveAt(i);
							goto IL_290;
						}
						goto IL_290;
					}
				}
				entityDrone = (EntityFactory.CreateEntity(entityCreationData) as EntityDrone);
				if (entityDrone)
				{
					this.dronesActive.Add(entityDrone);
					world.SpawnEntityInWorld(entityDrone);
					num++;
				}
				else
				{
					Log.Error("DroneManager load failed #{0}, id {1}, {2}", new object[]
					{
						i,
						entityCreationData.id,
						EntityClass.GetEntityClassName(entityCreationData.entityClass)
					});
				}
				this.dronesUnloaded.RemoveAt(i);
			}
			IL_290:;
		}
		this.saveTime -= Time.deltaTime;
		if (this.saveTime <= 0f && (this.saveThread == null || this.saveThread.HasTerminated()))
		{
			this.saveTime = 120f;
			this.Save();
		}
	}

	// Token: 0x06001B5A RID: 7002 RVA: 0x000AB7B0 File Offset: 0x000A99B0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isValidDronePos(Vector3 pos)
	{
		return !float.IsNaN(pos.x) && !float.IsNaN(pos.y) && !float.IsNaN(pos.z);
	}

	// Token: 0x06001B5B RID: 7003 RVA: 0x000AB7DC File Offset: 0x000A99DC
	public void RemoveAllDronesFromMap()
	{
		GameManager gameManager = GameManager.Instance;
		PersistentPlayerList persistentPlayerList = gameManager.GetPersistentPlayerList();
		World world = gameManager.World;
		foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in persistentPlayerList.Players)
		{
			EntityPlayer entityPlayer = world.GetEntity(keyValuePair.Value.EntityId) as EntityPlayer;
			if (entityPlayer)
			{
				OwnedEntityData[] ownedEntities = entityPlayer.GetOwnedEntities();
				int j;
				int i;
				Predicate<EntityDrone> <>9__0;
				for (i = ownedEntities.Length - 1; i >= 0; i = j)
				{
					List<EntityDrone> list = this.dronesActive;
					Predicate<EntityDrone> match;
					if ((match = <>9__0) == null)
					{
						match = (<>9__0 = ((EntityDrone v) => v.entityId == ownedEntities[i].Id));
					}
					EntityDrone entityDrone = list.Find(match);
					if (entityDrone && entityPlayer.HasOwnedEntity(entityDrone.entityId))
					{
						GameManager.Instance.World.RemoveEntityFromMap(entityDrone, EnumRemoveEntityReason.Unloaded);
					}
					j = i - 1;
				}
			}
		}
		this.UpdateWaypointsForAllPlayers();
	}

	// Token: 0x06001B5C RID: 7004 RVA: 0x000AB908 File Offset: 0x000A9B08
	public void ClearAllDronesForPlayer(EntityPlayer player)
	{
		this.ClearAllDronesForPlayer(player.entityId);
	}

	// Token: 0x06001B5D RID: 7005 RVA: 0x000AB916 File Offset: 0x000A9B16
	public void ClearAllDronesForPlayer(int entityId)
	{
		this.ClearUnloadedDrones(entityId);
		this.ClearActiveDrones(entityId);
		this.TriggerSave();
		this.UpdateWaypointsForPlayer(entityId);
	}

	// Token: 0x06001B5E RID: 7006 RVA: 0x000AB933 File Offset: 0x000A9B33
	public void ClearUnloadedDrones(EntityPlayer player)
	{
		this.ClearUnloadedDrones(player.entityId);
	}

	// Token: 0x06001B5F RID: 7007 RVA: 0x000AB944 File Offset: 0x000A9B44
	public void ClearUnloadedDrones(int entityId)
	{
		for (int i = this.dronesUnloaded.Count - 1; i >= 0; i--)
		{
			if (this.dronesUnloaded[i].belongsPlayerId == entityId)
			{
				this.dronesUnloaded.RemoveAt(i);
			}
		}
	}

	// Token: 0x06001B60 RID: 7008 RVA: 0x000AB98C File Offset: 0x000A9B8C
	public void ClearActiveDrones(int entityId)
	{
		for (int i = this.dronesActive.Count - 1; i >= 0; i--)
		{
			EntityDrone entityDrone = this.dronesActive[i];
			if (entityDrone.belongsPlayerId == entityId)
			{
				this.dronesActive.RemoveAt(i);
				GameManager.Instance.World.RemoveEntity(entityDrone.entityId, EnumRemoveEntityReason.Killed);
			}
		}
	}

	// Token: 0x06001B61 RID: 7009 RVA: 0x000AB9EC File Offset: 0x000A9BEC
	public string LogUnloadedDronesForPlayer(EntityPlayer player)
	{
		string text = string.Empty;
		for (int i = 0; i < this.dronesUnloaded.Count; i++)
		{
			if (this.dronesUnloaded[i].belongsPlayerId == player.entityId)
			{
				text = text + this.dronesUnloaded[i].clientEntityId.ToString() + Environment.NewLine;
			}
		}
		return text;
	}

	// Token: 0x06001B62 RID: 7010 RVA: 0x000ABA54 File Offset: 0x000A9C54
	public bool AssignUnloadedDrone(EntityPlayer player, int entityId)
	{
		for (int i = 0; i < this.dronesUnloaded.Count; i++)
		{
			EntityCreationData entityCreationData = this.dronesUnloaded[i];
			if (entityCreationData.id == entityId)
			{
				entityCreationData.pos = player.getHeadPosition();
				entityCreationData.belongsPlayerId = player.entityId;
				Log.Warning(entityCreationData.belongsPlayerId.ToString());
				this.debugDronePlayerAssignment.Add(entityCreationData.belongsPlayerId);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B63 RID: 7011 RVA: 0x000ABACC File Offset: 0x000A9CCC
	public List<EntityCreationData> GetAllDronesECD()
	{
		List<EntityCreationData> list = new List<EntityCreationData>();
		for (int i = 0; i < this.dronesActive.Count; i++)
		{
			list.Add(new EntityCreationData(this.dronesActive[i], false));
		}
		for (int j = 0; j < this.dronesUnloaded.Count; j++)
		{
			list.Add(this.dronesUnloaded[j]);
		}
		return list;
	}

	// Token: 0x06001B64 RID: 7012 RVA: 0x000ABB38 File Offset: 0x000A9D38
	public void UpdateWaypointsForAllPlayers()
	{
		foreach (EntityPlayer entityPlayer in GameManager.Instance.World.GetPlayers())
		{
			this.UpdateWaypointsForPlayer(entityPlayer.entityId);
		}
	}

	// Token: 0x06001B65 RID: 7013 RVA: 0x000ABB9C File Offset: 0x000A9D9C
	public void UpdateWaypointsForPlayer(int entityId)
	{
		EntityPlayer entityPlayer = (EntityPlayer)GameManager.Instance.World.GetEntity(entityId);
		if (entityPlayer == null)
		{
			return;
		}
		NavObjectManager.Instance.UnRegisterNavObjectByClass("entityJunkDrone");
		List<EntityCreationData> allDronesECD = this.GetAllDronesECD();
		OwnedEntityData[] ownedEntityClass = entityPlayer.GetOwnedEntityClass("entityJunkDrone");
		List<ValueTuple<int, Vector3>> list = new List<ValueTuple<int, Vector3>>();
		foreach (OwnedEntityData ownedEntityData in ownedEntityClass)
		{
			foreach (EntityCreationData entityCreationData in allDronesECD)
			{
				if (entityCreationData.id == ownedEntityData.Id)
				{
					list.Add(new ValueTuple<int, Vector3>(ownedEntityData.Id, entityCreationData.pos));
				}
			}
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer != null && entityId == primaryPlayer.entityId)
		{
			primaryPlayer.Waypoints.SetDroneWaypointsFromDroneManager(list);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityWaypointList>().Setup(eWayPointListType.Drone, list), false, entityId, -1, -1, null, 192, false);
	}

	// Token: 0x06001B66 RID: 7014 RVA: 0x000ABCCC File Offset: 0x000A9ECC
	public static void Cleanup()
	{
		if (DroneManager.instance != null)
		{
			DroneManager.instance.SaveAndClear();
		}
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x000ABCDF File Offset: 0x000A9EDF
	[PublicizedFrom(EAccessModifier.Private)]
	public void SaveAndClear()
	{
		this.WaitOnSave();
		this.Save();
		this.WaitOnSave();
		this.dronesActive.Clear();
		this.dronesUnloaded.Clear();
		DroneManager.instance = null;
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x000ABD0F File Offset: 0x000A9F0F
	[PublicizedFrom(EAccessModifier.Private)]
	public void WaitOnSave()
	{
		if (this.saveThread != null)
		{
			this.saveThread.WaitForEnd(30);
			this.saveThread = null;
		}
	}

	// Token: 0x06001B69 RID: 7017 RVA: 0x000ABD30 File Offset: 0x000A9F30
	public void Load()
	{
		string text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "drones.dat");
		if (SdFile.Exists(text))
		{
			try
			{
				using (Stream stream = SdFile.OpenRead(text))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(stream);
						this.read(pooledBinaryReader);
					}
				}
			}
			catch (Exception)
			{
				text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "drones.dat.bak");
				if (SdFile.Exists(text))
				{
					using (Stream stream2 = SdFile.OpenRead(text))
					{
						using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader2.SetBaseStream(stream2);
							this.read(pooledBinaryReader2);
						}
					}
				}
			}
			Log.Out("{0} {1}, loaded {2}", new object[]
			{
				base.GetType(),
				text,
				this.dronesUnloaded.Count
			});
		}
	}

	// Token: 0x06001B6A RID: 7018 RVA: 0x000ABE68 File Offset: 0x000AA068
	[PublicizedFrom(EAccessModifier.Private)]
	public void Save()
	{
		if (this.saveThread == null || !ThreadManager.ActiveThreads.ContainsKey("droneDataSave"))
		{
			Log.Out("{0} saving {1} ({2} + {3})", new object[]
			{
				base.GetType(),
				this.dronesActive.Count + this.dronesUnloaded.Count,
				this.dronesActive.Count,
				this.dronesUnloaded.Count
			});
			PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				this.write(pooledBinaryWriter);
			}
			this.saveThread = ThreadManager.StartThread("droneDataSave", null, new ThreadManager.ThreadFunctionLoopDelegate(this.SaveThread), null, pooledExpandableMemoryStream, null, false, true);
		}
	}

	// Token: 0x06001B6B RID: 7019 RVA: 0x000ABF54 File Offset: 0x000AA154
	[PublicizedFrom(EAccessModifier.Private)]
	public int SaveThread(ThreadManager.ThreadInfo _threadInfo)
	{
		PooledExpandableMemoryStream pooledExpandableMemoryStream = (PooledExpandableMemoryStream)_threadInfo.parameter;
		string text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "drones.dat");
		if (SdFile.Exists(text))
		{
			SdFile.Copy(text, string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "drones.dat.bak"), true);
		}
		pooledExpandableMemoryStream.Position = 0L;
		StreamUtils.WriteStreamToFile(pooledExpandableMemoryStream, text);
		Log.Out("{0} saved {1} bytes", new object[]
		{
			base.GetType(),
			pooledExpandableMemoryStream.Length
		});
		MemoryPools.poolMemoryStream.FreeSync(pooledExpandableMemoryStream);
		return -1;
	}

	// Token: 0x06001B6C RID: 7020 RVA: 0x000ABFE8 File Offset: 0x000AA1E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void read(PooledBinaryReader _br)
	{
		if (_br.ReadChar() != 'v' || _br.ReadChar() != 'd' || _br.ReadChar() != 'a' || _br.ReadChar() != '\0')
		{
			Log.Error("{0} file bad signature", new object[]
			{
				base.GetType()
			});
			return;
		}
		if (_br.ReadByte() != 1)
		{
			Log.Error("{0} file bad version", new object[]
			{
				base.GetType()
			});
			return;
		}
		this.dronesUnloaded.Clear();
		int num = _br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			EntityCreationData entityCreationData = new EntityCreationData();
			entityCreationData.read(_br, false);
			this.dronesUnloaded.Add(entityCreationData);
		}
	}

	// Token: 0x06001B6D RID: 7021 RVA: 0x000AC090 File Offset: 0x000AA290
	[PublicizedFrom(EAccessModifier.Private)]
	public void write(PooledBinaryWriter _bw)
	{
		_bw.Write('v');
		_bw.Write('d');
		_bw.Write('a');
		_bw.Write(0);
		_bw.Write(1);
		List<EntityCreationData> list = new List<EntityCreationData>();
		this.GetDrones(list);
		_bw.Write(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			EntityCreationData entityCreationData = list[i];
			if (!this.isValidDronePos(entityCreationData.pos))
			{
				EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(entityCreationData.belongsPlayerId) as EntityPlayer;
				if (entityPlayer)
				{
					Log.Warning("corrupted data using the player position");
					entityCreationData.pos = entityPlayer.getHeadPosition();
				}
				else
				{
					Log.Warning("corrupted data clearing the drone position");
					entityCreationData.pos = Vector3.zero;
				}
			}
			entityCreationData.write(_bw, false);
		}
	}

	// Token: 0x06001B6E RID: 7022 RVA: 0x000AC15C File Offset: 0x000AA35C
	public List<EntityCreationData> GetDronesList()
	{
		List<EntityCreationData> list = new List<EntityCreationData>();
		this.GetDrones(list);
		return list;
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x000AC178 File Offset: 0x000AA378
	[PublicizedFrom(EAccessModifier.Private)]
	public void GetDrones(List<EntityCreationData> _list)
	{
		for (int i = 0; i < this.dronesActive.Count; i++)
		{
			_list.Add(new EntityCreationData(this.dronesActive[i], true));
		}
		for (int j = 0; j < this.dronesUnloaded.Count; j++)
		{
			_list.Add(this.dronesUnloaded[j]);
		}
		for (int k = 0; k < this.dronesWithoutOwner.Count; k++)
		{
			_list.Add(this.dronesWithoutOwner[k]);
		}
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x000AC204 File Offset: 0x000AA404
	[return: TupleElementNames(new string[]
	{
		"entityId",
		"position"
	})]
	public List<ValueTuple<int, Vector3>> GetDronePositionsList()
	{
		List<ValueTuple<int, Vector3>> list = new List<ValueTuple<int, Vector3>>();
		for (int i = 0; i < this.dronesActive.Count; i++)
		{
			list.Add(new ValueTuple<int, Vector3>(this.dronesActive[i].entityId, this.dronesActive[i].position));
		}
		for (int j = 0; j < this.dronesUnloaded.Count; j++)
		{
			list.Add(new ValueTuple<int, Vector3>(this.dronesUnloaded[j].id, this.dronesUnloaded[j].pos));
		}
		for (int k = 0; k < this.dronesWithoutOwner.Count; k++)
		{
			list.Add(new ValueTuple<int, Vector3>(this.dronesWithoutOwner[k].id, this.dronesWithoutOwner[k].pos));
		}
		return list;
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x000AC2E1 File Offset: 0x000AA4E1
	public static int GetServerDroneCount()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return DroneManager.Instance.dronesActive.Count + DroneManager.Instance.dronesUnloaded.Count;
		}
		return DroneManager.serverDroneCount;
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x000AC314 File Offset: 0x000AA514
	public static void SetServerDroneCount(int count)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		DroneManager.serverDroneCount = count;
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x000AC329 File Offset: 0x000AA529
	public static bool CanAddMoreDrones()
	{
		return !(DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() || DroneManager.GetServerDroneCount() < 500;
	}

	// Token: 0x06001B74 RID: 7028 RVA: 0x000AC344 File Offset: 0x000AA544
	[Conditional("DEBUG_DRONEMAN")]
	public static void VMLog(string _format = "", params object[] _args)
	{
		int frameCount = GameManager.frameCount;
		_format = string.Format("{0} {1} {2}", frameCount, "DroneManager", _format);
		Log.Out(_format, _args);
	}

	// Token: 0x04001235 RID: 4661
	public static bool Debug_LocalControl;

	// Token: 0x04001236 RID: 4662
	public static bool DebugLogEnabled;

	// Token: 0x04001237 RID: 4663
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cVersion = 1;

	// Token: 0x04001238 RID: 4664
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSaveTime = 120f;

	// Token: 0x04001239 RID: 4665
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cChangeSaveDelay = 10f;

	// Token: 0x0400123A RID: 4666
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cMaxDrones = 500;

	// Token: 0x0400123B RID: 4667
	[PublicizedFrom(EAccessModifier.Private)]
	public static int serverDroneCount;

	// Token: 0x0400123C RID: 4668
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<EntityDrone> dronesActive = new List<EntityDrone>();

	// Token: 0x0400123D RID: 4669
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<EntityCreationData> dronesUnloaded = new List<EntityCreationData>();

	// Token: 0x0400123E RID: 4670
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<EntityCreationData> dronesWithoutOwner = new List<EntityCreationData>();

	// Token: 0x0400123F RID: 4671
	[PublicizedFrom(EAccessModifier.Private)]
	public float saveTime = 120f;

	// Token: 0x04001240 RID: 4672
	[PublicizedFrom(EAccessModifier.Private)]
	public static DroneManager instance;

	// Token: 0x04001241 RID: 4673
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.ThreadInfo saveThread;

	// Token: 0x04001242 RID: 4674
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> debugDronePlayerAssignment = new List<int>();

	// Token: 0x04001243 RID: 4675
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cNameKey = "drones";

	// Token: 0x04001244 RID: 4676
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cThreadKey = "droneDataSave";

	// Token: 0x04001245 RID: 4677
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cStringName = "DroneManager";
}
