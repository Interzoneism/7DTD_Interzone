using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000352 RID: 850
public static class DynamicMeshServer
{
	// Token: 0x060018F9 RID: 6393 RVA: 0x0009911C File Offset: 0x0009731C
	public static void OnClientConnect(ClientInfo info)
	{
		DynamicMeshServer.GetData(info.entityId);
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("User " + info.entityId.ToString() + " joined dynamic meshes: " + DynamicMeshServer.ClientData.Count.ToString());
		}
	}

	// Token: 0x060018FA RID: 6394 RVA: 0x00099170 File Offset: 0x00097370
	public static void OnClientDisconnect(ClientInfo info)
	{
		int entityId = info.entityId;
		Log.Out("Client disconnected from dy mesh: Id: " + entityId.ToString() + " Total: " + DynamicMeshServer.ClientData.Count.ToString());
		try
		{
			DynamicMeshServer.ClientData.Remove(entityId);
		}
		catch (Exception ex)
		{
			Log.Error("Client removal error: " + ex.Message);
		}
	}

	// Token: 0x060018FB RID: 6395 RVA: 0x000991E8 File Offset: 0x000973E8
	public static void CleanUp()
	{
		DynamicMeshServer.ClientData.Clear();
		DynamicMeshServer.DelayedClientChecks.Clear();
		DynamicMeshItem dynamicMeshItem;
		while (DynamicMeshServer.SyncReleaseQueue.TryDequeue(out dynamicMeshItem))
		{
		}
		DynamicMeshSyncRequest dynamicMeshSyncRequest;
		while (DynamicMeshServer.SyncRequests.TryDequeue(out dynamicMeshSyncRequest))
		{
		}
		DynamicMeshServer.ActiveSyncs.Clear();
	}

	// Token: 0x060018FC RID: 6396 RVA: 0x00099230 File Offset: 0x00097430
	public static void SendToAllClients(DynamicMeshItem item, bool isDelete)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount() == 0)
		{
			return;
		}
		DynamicMeshSyncRequest item2 = DynamicMeshSyncRequest.Create(item, isDelete);
		DynamicMeshServer.SyncRequests.Enqueue(item2);
	}

	// Token: 0x060018FD RID: 6397 RVA: 0x0009925D File Offset: 0x0009745D
	public static void SyncRelease(DynamicMeshItem item)
	{
		if (item == null)
		{
			return;
		}
		DynamicMeshServer.SyncReleaseQueue.Enqueue(item);
	}

	// Token: 0x060018FE RID: 6398 RVA: 0x00099270 File Offset: 0x00097470
	public static void Update()
	{
		for (;;)
		{
			DynamicMeshItem item;
			if (!DynamicMeshServer.SyncReleaseQueue.TryDequeue(out item))
			{
				break;
			}
			DynamicMeshSyncRequest dynamicMeshSyncRequest = DynamicMeshServer.ActiveSyncs.FirstOrDefault((DynamicMeshSyncRequest d) => d.Item.WorldPosition.x == item.WorldPosition.x && d.Item.WorldPosition.z == item.WorldPosition.z);
			if (dynamicMeshSyncRequest == null)
			{
				Log.Warning("Active sync could not be found to be cleared: " + item.ToDebugLocation());
			}
			else
			{
				dynamicMeshSyncRequest.SyncComplete = true;
			}
		}
		DynamicMeshSyncRequest dynamicMeshSyncRequest2;
		if (DynamicMeshServer.ActiveSyncs.Count < DynamicMeshServer.MaxActiveSyncs && DynamicMeshServer.SyncRequests.TryDequeue(out dynamicMeshSyncRequest2))
		{
			dynamicMeshSyncRequest2.Initiated = new DateTime?(DateTime.Now);
			DynamicMeshServer.ActiveSyncs.Add(dynamicMeshSyncRequest2);
		}
		for (int i = DynamicMeshServer.ActiveSyncs.Count - 1; i >= 0; i--)
		{
			DynamicMeshSyncRequest dynamicMeshSyncRequest3 = DynamicMeshServer.ActiveSyncs[i];
			if (dynamicMeshSyncRequest3.SyncComplete || SingletonMonoBehaviour<ConnectionManager>.Instance.ClientCount() == 0)
			{
				DynamicMeshThread.ChunkDataQueue.ManuallyReleaseBytes(dynamicMeshSyncRequest3.Data);
				if (DynamicMeshManager.DoLogNet)
				{
					int num = (int)(DateTime.Now - dynamicMeshSyncRequest3.Initiated.Value).TotalMilliseconds;
					Log.Out(string.Concat(new string[]
					{
						"Package for ",
						(dynamicMeshSyncRequest3.ClientId == -1) ? "all" : dynamicMeshSyncRequest3.ClientId.ToString(),
						" took ",
						num.ToString(),
						"ms for ",
						dynamicMeshSyncRequest3.Length.ToString(),
						" bytes"
					}));
				}
				DynamicMeshServer.ActiveSyncs.RemoveAt(i);
			}
			else if (!dynamicMeshSyncRequest3.HasData)
			{
				if (dynamicMeshSyncRequest3.IsDelete || dynamicMeshSyncRequest3.TryGetData())
				{
					NetPackageDynamicMesh package = NetPackageManager.GetPackage<NetPackageDynamicMesh>();
					package.Setup(dynamicMeshSyncRequest3.Item, dynamicMeshSyncRequest3.Data);
					package.PresumedLength = dynamicMeshSyncRequest3.Length;
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, dynamicMeshSyncRequest3.ClientId, -1, -1, null, 192, false);
				}
			}
			else if (dynamicMeshSyncRequest3.SecondsAttempted > 20)
			{
				Log.Warning("Sync waited more than 20 seconds. Removing...");
				DynamicMeshServer.ActiveSyncs.Remove(dynamicMeshSyncRequest3);
				break;
			}
		}
		using (Dictionary<int, DynamicMeshClientConnection>.ValueCollection.Enumerator enumerator = DynamicMeshServer.ClientData.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				DynamicMeshClientConnection con = enumerator.Current;
				if ((con.SendMessage || con.TriggerSend) && con.ItemsToSend.Count > 0)
				{
					con.SendMessage = false;
					ConcurrentQueue<DynamicMeshSyncRequest> concurrentQueue = null;
					if (!con.ItemsToSend.ContainsKey(con.LastKey))
					{
						EntityPlayer entityPlayer = GameManager.Instance.World.GetPlayers().FirstOrDefault((EntityPlayer d) => d.entityId == con.EntityId);
						Vector3 playerPos = (entityPlayer == null) ? Vector3.zero : entityPlayer.GetPosition();
						ValueTuple<int, int> valueTuple = (from d in con.ItemsToSend.Keys.ToList<ValueTuple<int, int>>()
						orderby Math.Abs(Mathf.Sqrt(Mathf.Pow(playerPos.x - (float)d.Item1, 2f) + Mathf.Pow(playerPos.z - (float)d.Item2, 2f)))
						select d).FirstOrDefault<ValueTuple<int, int>>();
						con.LastKey = valueTuple;
						if (DynamicMeshManager.DoLog || DynamicMeshManager.DoLogNet)
						{
							float num2 = Math.Abs(Mathf.Sqrt(Mathf.Pow(playerPos.x - (float)valueTuple.Item1, 2f) + Mathf.Pow(playerPos.z - (float)valueTuple.Item2, 2f)));
							int count = con.ItemsToSend[valueTuple].Count;
							string[] array = new string[6];
							array[0] = "Switching key to ";
							int num3 = 1;
							ValueTuple<int, int> valueTuple2 = valueTuple;
							array[num3] = valueTuple2.ToString();
							array[2] = " Dist: ";
							array[3] = num2.ToString();
							array[4] = "   Items: ";
							array[5] = count.ToString();
							Log.Out(string.Concat(array));
						}
					}
					DynamicMeshSyncRequest item2;
					if (!con.ItemsToSend.TryGetValue(con.LastKey, out concurrentQueue))
					{
						if (!DynamicMeshManager.DoLog || DynamicMeshManager.DoLogNet)
						{
							Log.Out("Could not find last key to skipping");
						}
					}
					else if (concurrentQueue.Count == 0)
					{
						con.ItemsToSend.TryRemove(con.LastKey, out concurrentQueue);
					}
					else if (concurrentQueue.TryDequeue(out item2))
					{
						DynamicMeshServer.SyncRequests.Enqueue(item2);
						con.LastSend = DateTime.Now;
					}
				}
			}
		}
		if (DynamicMeshServer.debugTime < Time.time)
		{
			if (DynamicMeshManager.DoLog || DynamicMeshManager.DoLogNet)
			{
				DynamicMeshManager.LogMsg(string.Concat(new string[]
				{
					"Dyn Mesh Server update. ",
					DynamicMeshServer.ClientData.Count.ToString(),
					"   Prefabs: ",
					DynamicMeshManager.Instance.PrefabCheck.ToString(),
					" buff: ",
					DynamicMeshManager.Instance.BufferRegionLoadRequests.Count.ToString(),
					"  chunkData: ",
					DynamicMeshManager.Instance.ChunkMeshData.Count.ToString(),
					"   Primary: ",
					DynamicMeshThread.PrimaryQueue.Count.ToString(),
					"   Secondary: ",
					DynamicMeshThread.SecondaryQueue.Count.ToString()
				}));
			}
			DynamicMeshServer.debugTime = Time.time + 10f;
		}
	}

	// Token: 0x060018FF RID: 6399 RVA: 0x00099844 File Offset: 0x00097A44
	public static void ChunkRequested(long key, bool isFinal, ClientInfo client)
	{
		DynamicMeshClientConnection data = DynamicMeshServer.GetData(client.entityId);
		data.RequestedChunks.Enqueue(key);
		if (isFinal)
		{
			data.FinalChunks.Add(key);
		}
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x00099878 File Offset: 0x00097A78
	public static DynamicMeshClientConnection GetData(int entityId)
	{
		DynamicMeshClientConnection dynamicMeshClientConnection;
		if (!DynamicMeshServer.ClientData.TryGetValue(entityId, out dynamicMeshClientConnection))
		{
			dynamicMeshClientConnection = new DynamicMeshClientConnection(entityId);
			DynamicMeshServer.ClientData.Add(entityId, dynamicMeshClientConnection);
		}
		return dynamicMeshClientConnection;
	}

	// Token: 0x06001901 RID: 6401 RVA: 0x000998A8 File Offset: 0x00097AA8
	public static DynamicMeshClientConnection GetData(NetPackage package)
	{
		return DynamicMeshServer.GetData(package.Sender.entityId);
	}

	// Token: 0x06001902 RID: 6402 RVA: 0x000998BA File Offset: 0x00097ABA
	public static void ClientMessageRecieved(NetPackageDynamicClientArrive package)
	{
		DynamicMeshServer.GetData(package).UpdateItemsToSend(package);
	}

	// Token: 0x06001903 RID: 6403 RVA: 0x000998C8 File Offset: 0x00097AC8
	public static void ProcessDelayedPackages()
	{
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg("==Processing delayed packages: " + DynamicMeshServer.DelayedClientChecks.Count.ToString());
		}
		foreach (NetPackageDynamicClientArrive package in DynamicMeshServer.DelayedClientChecks)
		{
			DynamicMeshServer.GetData(package).UpdateItemsToSend(package);
		}
	}

	// Token: 0x06001904 RID: 6404 RVA: 0x00099948 File Offset: 0x00097B48
	public static void ClientReadyForNextMesh(NetPackageDynamicMesh package)
	{
		DynamicMeshServer.GetData(package).SendMessage = true;
	}

	// Token: 0x04000FF7 RID: 4087
	public static Dictionary<int, DynamicMeshClientConnection> ClientData = new Dictionary<int, DynamicMeshClientConnection>();

	// Token: 0x04000FF8 RID: 4088
	public static List<NetPackageDynamicClientArrive> DelayedClientChecks = new List<NetPackageDynamicClientArrive>();

	// Token: 0x04000FF9 RID: 4089
	public static bool AutoSend = false;

	// Token: 0x04000FFA RID: 4090
	public static DynamicMeshServerType ConnectionType = DynamicMeshServerType.Mesh;

	// Token: 0x04000FFB RID: 4091
	[PublicizedFrom(EAccessModifier.Private)]
	public static float debugTime = 0f;

	// Token: 0x04000FFC RID: 4092
	public static bool ResendPackages = true;

	// Token: 0x04000FFD RID: 4093
	public static bool ShowSender = true;

	// Token: 0x04000FFE RID: 4094
	public static ConcurrentQueue<DynamicMeshSyncRequest> SyncRequests = new ConcurrentQueue<DynamicMeshSyncRequest>();

	// Token: 0x04000FFF RID: 4095
	public static List<DynamicMeshSyncRequest> ActiveSyncs = new List<DynamicMeshSyncRequest>();

	// Token: 0x04001000 RID: 4096
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConcurrentQueue<DynamicMeshItem> SyncReleaseQueue = new ConcurrentQueue<DynamicMeshItem>();

	// Token: 0x04001001 RID: 4097
	public static int MaxActiveSyncs = 10;
}
