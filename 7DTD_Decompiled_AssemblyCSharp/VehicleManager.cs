using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Platform;
using UnityEngine;

// Token: 0x02000B3F RID: 2879
public class VehicleManager
{
	// Token: 0x1700091B RID: 2331
	// (get) Token: 0x0600599D RID: 22941 RVA: 0x002411C5 File Offset: 0x0023F3C5
	public static VehicleManager Instance
	{
		get
		{
			return VehicleManager.instance;
		}
	}

	// Token: 0x0600599E RID: 22942 RVA: 0x002411CC File Offset: 0x0023F3CC
	public VehicleManager()
	{
		this.vehiclesList = new List<EntityCreationData>();
	}

	// Token: 0x0600599F RID: 22943 RVA: 0x00241200 File Offset: 0x0023F400
	public static void Init()
	{
		VehicleManager.instance = new VehicleManager();
		VehicleManager.instance.Load();
	}

	// Token: 0x060059A0 RID: 22944 RVA: 0x00241216 File Offset: 0x0023F416
	public void AddTrackedVehicle(EntityVehicle _vehicle)
	{
		if (!_vehicle)
		{
			Log.Error("VehicleManager AddTrackedVehicle null");
			return;
		}
		if (!this.vehiclesActive.Contains(_vehicle))
		{
			this.vehiclesActive.Add(_vehicle);
			this.TriggerSave();
		}
	}

	// Token: 0x060059A1 RID: 22945 RVA: 0x0024124C File Offset: 0x0023F44C
	public void RemoveTrackedVehicle(EntityVehicle _vehicle, EnumRemoveEntityReason _reason)
	{
		VehicleManager.VMLog("RemoveTrackedVehicle {0}, {1}", new object[]
		{
			_vehicle,
			_reason
		});
		this.vehiclesActive.Remove(_vehicle);
		if (_reason == EnumRemoveEntityReason.Unloaded)
		{
			this.vehiclesUnloaded.Add(new EntityCreationData(_vehicle, false));
		}
		this.TriggerSave();
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageVehicleCount>().Setup(), false, -1, -1, -1, null, 192, false);
		this.UpdateVehicleWaypoints();
	}

	// Token: 0x060059A2 RID: 22946 RVA: 0x002412CC File Offset: 0x0023F4CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void TriggerSave()
	{
		this.saveTime = Mathf.Min(this.saveTime, 10f);
	}

	// Token: 0x060059A3 RID: 22947 RVA: 0x002412E4 File Offset: 0x0023F4E4
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
		int i = this.vehiclesUnloaded.Count - 1;
		while (i >= 0)
		{
			EntityCreationData entityCreationData = this.vehiclesUnloaded[i];
			Entity entity = world.GetEntity(entityCreationData.id);
			if (!entity)
			{
				goto IL_135;
			}
			EntityVehicle entityVehicle = entity as EntityVehicle;
			if (entityVehicle == null)
			{
				Log.Warning("VehicleManager id used #{0}, id {1}, {2}, {3}", new object[]
				{
					i,
					entityCreationData.id,
					entity,
					entity.position.ToCultureInvariantString()
				});
				entityCreationData.id = -1;
				goto IL_135;
			}
			string format = "VehicleManager already loaded #{0}, id {1}, {2}, {3}, owner {4}";
			object[] array = new object[5];
			array[0] = i;
			array[1] = entityCreationData.id;
			array[2] = entityVehicle;
			array[3] = entityVehicle.position.ToCultureInvariantString();
			int num2 = 4;
			PlatformUserIdentifierAbs ownerId = entityVehicle.vehicle.OwnerId;
			array[num2] = ((ownerId != null) ? ownerId.CombinedString : null);
			Log.Warning(format, array);
			this.vehiclesUnloaded.RemoveAt(i);
			IL_2A5:
			i--;
			continue;
			IL_135:
			if (world.IsChunkAreaCollidersLoaded(entityCreationData.pos))
			{
				EntityCreationData entityCreationData2 = entityCreationData;
				entityCreationData2.pos.y = entityCreationData2.pos.y + 0.002f;
				EntityVehicle entityVehicle2 = EntityFactory.CreateEntity(entityCreationData) as EntityVehicle;
				if (entityVehicle2)
				{
					this.vehiclesActive.Add(entityVehicle2);
					world.SpawnEntityInWorld(entityVehicle2);
					int belongsPlayerId = -1;
					if (entityVehicle2.vehicle.OwnerId != null)
					{
						PersistentPlayerData playerData = GameManager.Instance.persistentPlayers.GetPlayerData(entityVehicle2.vehicle.OwnerId);
						if (playerData != null)
						{
							belongsPlayerId = playerData.EntityId;
						}
					}
					entityVehicle2.belongsPlayerId = belongsPlayerId;
					string format2 = "loaded #{0}, id {1}, {2}, {3}, chunk {4} ({5}, {6}), owner {7}";
					object[] array2 = new object[8];
					array2[0] = i;
					array2[1] = entityCreationData.id;
					array2[2] = entityVehicle2;
					array2[3] = entityVehicle2.position.ToCultureInvariantString();
					array2[4] = World.toChunkXZ(entityVehicle2.position);
					array2[5] = entityVehicle2.chunkPosAddedEntityTo.x;
					array2[6] = entityVehicle2.chunkPosAddedEntityTo.z;
					int num3 = 7;
					PlatformUserIdentifierAbs ownerId2 = entityVehicle2.vehicle.OwnerId;
					array2[num3] = ((ownerId2 != null) ? ownerId2.CombinedString : null);
					VehicleManager.VMLog(format2, array2);
					num++;
				}
				else
				{
					Log.Error("VehicleManager load failed #{0}, id {1}, {2}", new object[]
					{
						i,
						entityCreationData.id,
						EntityClass.GetEntityClassName(entityCreationData.entityClass)
					});
				}
				this.vehiclesUnloaded.RemoveAt(i);
				goto IL_2A5;
			}
			goto IL_2A5;
		}
		if (num > 0)
		{
			VehicleManager.VMLog("Update loaded {0}", new object[]
			{
				num
			});
		}
		this.saveTime -= Time.deltaTime;
		if (this.saveTime <= 0f && (this.saveThread == null || this.saveThread.HasTerminated()))
		{
			this.saveTime = 120f;
			this.Save();
		}
	}

	// Token: 0x060059A4 RID: 22948 RVA: 0x00241604 File Offset: 0x0023F804
	public void PhysicsWakeNear(Vector3 pos)
	{
		for (int i = 0; i < this.vehiclesActive.Count; i++)
		{
			EntityVehicle entityVehicle = this.vehiclesActive[i];
			if (entityVehicle && (entityVehicle.position - pos).sqrMagnitude <= 400f)
			{
				entityVehicle.AddForce(Vector3.zero, ForceMode.VelocityChange);
			}
		}
	}

	// Token: 0x060059A5 RID: 22949 RVA: 0x00241664 File Offset: 0x0023F864
	public void RemoveAllVehiclesFromMap()
	{
		for (int i = 0; i < this.vehiclesActive.Count; i++)
		{
			GameManager.Instance.World.RemoveEntityFromMap(this.vehiclesActive[i], EnumRemoveEntityReason.Unloaded);
		}
	}

	// Token: 0x060059A6 RID: 22950 RVA: 0x002416A4 File Offset: 0x0023F8A4
	public void RemoveUnloadedVehicle(int id)
	{
		EntityCreationData entityCreationData = null;
		foreach (EntityCreationData entityCreationData2 in this.vehiclesUnloaded)
		{
			if (entityCreationData2.id == id)
			{
				entityCreationData = entityCreationData2;
				break;
			}
		}
		if (entityCreationData != null)
		{
			this.vehiclesUnloaded.Remove(entityCreationData);
			this.TriggerSave();
			this.UpdateVehicleWaypoints();
		}
	}

	// Token: 0x060059A7 RID: 22951 RVA: 0x0024171C File Offset: 0x0023F91C
	public void UpdateVehicleWaypoints()
	{
		foreach (EntityPlayer entityPlayer in GameManager.Instance.World.GetPlayers())
		{
			this.UpdateVehicleWaypointsForPlayer(entityPlayer.entityId);
		}
	}

	// Token: 0x060059A8 RID: 22952 RVA: 0x00241780 File Offset: 0x0023F980
	public void UpdateVehicleWaypointsForPlayer(int entityId)
	{
		this.GetVehiclesECDList();
		if (this.vehiclesList.Count == 0)
		{
			return;
		}
		List<ValueTuple<int, Vector3>> list = new List<ValueTuple<int, Vector3>>();
		foreach (EntityCreationData entityCreationData in this.vehiclesList)
		{
			if (entityCreationData.belongsPlayerId == entityId || entityCreationData.belongsPlayerId == -1)
			{
				list.Add(new ValueTuple<int, Vector3>(entityCreationData.id, entityCreationData.pos));
			}
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer != null && entityId == primaryPlayer.entityId)
		{
			primaryPlayer.Waypoints.SetEntityVehicleWaypointFromVehicleManager(list);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityWaypointList>().Setup(eWayPointListType.Vehicle, list), false, entityId, -1, -1, null, 192, false);
	}

	// Token: 0x060059A9 RID: 22953 RVA: 0x0024186C File Offset: 0x0023FA6C
	public static void Cleanup()
	{
		if (VehicleManager.instance != null)
		{
			VehicleManager.instance.SaveAndClear();
		}
	}

	// Token: 0x060059AA RID: 22954 RVA: 0x0024187F File Offset: 0x0023FA7F
	[PublicizedFrom(EAccessModifier.Private)]
	public void SaveAndClear()
	{
		this.WaitOnSave();
		this.Save();
		this.WaitOnSave();
		this.vehiclesActive.Clear();
		this.vehiclesUnloaded.Clear();
		VehicleManager.instance = null;
	}

	// Token: 0x060059AB RID: 22955 RVA: 0x002418AF File Offset: 0x0023FAAF
	[PublicizedFrom(EAccessModifier.Private)]
	public void WaitOnSave()
	{
		if (this.saveThread != null)
		{
			this.saveThread.WaitForEnd(30);
			this.saveThread = null;
		}
	}

	// Token: 0x060059AC RID: 22956 RVA: 0x002418D0 File Offset: 0x0023FAD0
	public void Load()
	{
		string text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "vehicles.dat");
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
				text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "vehicles.dat.bak");
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
			Log.Out("VehicleManager {0}, loaded {1}", new object[]
			{
				text,
				this.vehiclesUnloaded.Count
			});
		}
	}

	// Token: 0x060059AD RID: 22957 RVA: 0x00241A00 File Offset: 0x0023FC00
	[PublicizedFrom(EAccessModifier.Private)]
	public void Save()
	{
		if (this.saveThread == null || !ThreadManager.ActiveThreads.ContainsKey("vehicleDataSave"))
		{
			Log.Out("VehicleManager saving {0} ({1} + {2})", new object[]
			{
				this.vehiclesActive.Count + this.vehiclesUnloaded.Count,
				this.vehiclesActive.Count,
				this.vehiclesUnloaded.Count
			});
			PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				this.write(pooledBinaryWriter);
			}
			this.saveThread = ThreadManager.StartThread("vehicleDataSave", null, new ThreadManager.ThreadFunctionLoopDelegate(this.SaveThread), null, pooledExpandableMemoryStream, null, false, true);
		}
	}

	// Token: 0x060059AE RID: 22958 RVA: 0x00241AE4 File Offset: 0x0023FCE4
	[PublicizedFrom(EAccessModifier.Private)]
	public int SaveThread(ThreadManager.ThreadInfo _threadInfo)
	{
		PooledExpandableMemoryStream pooledExpandableMemoryStream = (PooledExpandableMemoryStream)_threadInfo.parameter;
		string text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "vehicles.dat");
		if (SdFile.Exists(text))
		{
			SdFile.Copy(text, string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "vehicles.dat.bak"), true);
		}
		pooledExpandableMemoryStream.Position = 0L;
		StreamUtils.WriteStreamToFile(pooledExpandableMemoryStream, text);
		Log.Out("VehicleManager saved {0} bytes", new object[]
		{
			pooledExpandableMemoryStream.Length
		});
		MemoryPools.poolMemoryStream.FreeSync(pooledExpandableMemoryStream);
		return -1;
	}

	// Token: 0x060059AF RID: 22959 RVA: 0x00241B70 File Offset: 0x0023FD70
	[PublicizedFrom(EAccessModifier.Private)]
	public void read(PooledBinaryReader _br)
	{
		if (_br.ReadChar() != 'v' || _br.ReadChar() != 'd' || _br.ReadChar() != 'a' || _br.ReadChar() != '\0')
		{
			Log.Error("Vehicle file bad signature");
			return;
		}
		if (_br.ReadByte() != 1)
		{
			Log.Error("Vehicle file bad version");
			return;
		}
		this.vehiclesUnloaded.Clear();
		int num = _br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			EntityCreationData entityCreationData = new EntityCreationData();
			entityCreationData.read(_br, false);
			this.vehiclesUnloaded.Add(entityCreationData);
			VehicleManager.VMLog("read #{0}, id {1}, {2}, {3}, chunk {4}", new object[]
			{
				i,
				entityCreationData.id,
				EntityClass.GetEntityClassName(entityCreationData.entityClass),
				entityCreationData.pos.ToCultureInvariantString(),
				World.toChunkXZ(entityCreationData.pos)
			});
		}
	}

	// Token: 0x060059B0 RID: 22960 RVA: 0x00241C50 File Offset: 0x0023FE50
	[PublicizedFrom(EAccessModifier.Private)]
	public void write(PooledBinaryWriter _bw)
	{
		_bw.Write('v');
		_bw.Write('d');
		_bw.Write('a');
		_bw.Write(0);
		_bw.Write(1);
		List<EntityCreationData> vehiclesECDList = this.GetVehiclesECDList();
		_bw.Write(vehiclesECDList.Count);
		for (int i = 0; i < vehiclesECDList.Count; i++)
		{
			EntityCreationData entityCreationData = vehiclesECDList[i];
			entityCreationData.write(_bw, false);
			VehicleManager.VMLog("write #{0}, id {1}, {2}, {3}, chunk {4}", new object[]
			{
				i,
				entityCreationData.id,
				EntityClass.GetEntityClassName(entityCreationData.entityClass),
				entityCreationData.pos.ToCultureInvariantString(),
				World.toChunkXZ(entityCreationData.pos)
			});
		}
	}

	// Token: 0x060059B1 RID: 22961 RVA: 0x00241D10 File Offset: 0x0023FF10
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityCreationData> GetVehiclesECDList()
	{
		this.vehiclesList.Clear();
		for (int i = 0; i < this.vehiclesActive.Count; i++)
		{
			this.vehiclesList.Add(new EntityCreationData(this.vehiclesActive[i], false));
		}
		for (int j = 0; j < this.vehiclesUnloaded.Count; j++)
		{
			this.vehiclesList.Add(this.vehiclesUnloaded[j]);
		}
		return this.vehiclesList;
	}

	// Token: 0x060059B2 RID: 22962 RVA: 0x00241D90 File Offset: 0x0023FF90
	[return: TupleElementNames(new string[]
	{
		"entityId",
		"position"
	})]
	public List<ValueTuple<int, Vector3>> GetVehiclePositionsList()
	{
		List<ValueTuple<int, Vector3>> list = new List<ValueTuple<int, Vector3>>();
		for (int i = 0; i < this.vehiclesActive.Count; i++)
		{
			list.Add(new ValueTuple<int, Vector3>(this.vehiclesActive[i].entityId, this.vehiclesActive[i].position));
		}
		for (int j = 0; j < this.vehiclesUnloaded.Count; j++)
		{
			list.Add(new ValueTuple<int, Vector3>(this.vehiclesUnloaded[j].id, this.vehiclesUnloaded[j].pos));
		}
		return list;
	}

	// Token: 0x060059B3 RID: 22963 RVA: 0x00241E2A File Offset: 0x0024002A
	public static int GetServerVehicleCount()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return VehicleManager.Instance.vehiclesActive.Count + VehicleManager.Instance.vehiclesUnloaded.Count;
		}
		return VehicleManager.serverVehicleCount;
	}

	// Token: 0x060059B4 RID: 22964 RVA: 0x00241E5D File Offset: 0x0024005D
	public static void SetServerVehicleCount(int count)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		VehicleManager.serverVehicleCount = count;
	}

	// Token: 0x060059B5 RID: 22965 RVA: 0x00241E72 File Offset: 0x00240072
	public static bool CanAddMoreVehicles()
	{
		return !(DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() || VehicleManager.GetServerVehicleCount() < 500;
	}

	// Token: 0x060059B6 RID: 22966 RVA: 0x00241E8C File Offset: 0x0024008C
	[Conditional("DEBUG_VEHICLEMAN")]
	public static void VMLog(string _format = "", params object[] _args)
	{
		int frameCount = GameManager.frameCount;
		_format = string.Format("{0} VehicleManager {1}", frameCount, _format);
		Log.Out(_format, _args);
	}

	// Token: 0x04004490 RID: 17552
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cVersion = 1;

	// Token: 0x04004491 RID: 17553
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSaveTime = 120f;

	// Token: 0x04004492 RID: 17554
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cChangeSaveDelay = 10f;

	// Token: 0x04004493 RID: 17555
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cMaxVehicles = 500;

	// Token: 0x04004494 RID: 17556
	[PublicizedFrom(EAccessModifier.Private)]
	public static int serverVehicleCount;

	// Token: 0x04004495 RID: 17557
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<EntityVehicle> vehiclesActive = new List<EntityVehicle>();

	// Token: 0x04004496 RID: 17558
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<EntityCreationData> vehiclesUnloaded = new List<EntityCreationData>();

	// Token: 0x04004497 RID: 17559
	[PublicizedFrom(EAccessModifier.Private)]
	public float saveTime = 120f;

	// Token: 0x04004498 RID: 17560
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.ThreadInfo saveThread;

	// Token: 0x04004499 RID: 17561
	[PublicizedFrom(EAccessModifier.Private)]
	public static VehicleManager instance;

	// Token: 0x0400449A RID: 17562
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityCreationData> vehiclesList;
}
