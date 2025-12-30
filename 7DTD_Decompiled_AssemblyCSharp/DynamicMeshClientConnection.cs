using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000356 RID: 854
public class DynamicMeshClientConnection
{
	// Token: 0x170002CC RID: 716
	// (get) Token: 0x0600190C RID: 6412 RVA: 0x00099A7C File Offset: 0x00097C7C
	public bool TriggerSend
	{
		get
		{
			return (DateTime.Now - this.LastSend).TotalSeconds > 1.0;
		}
	}

	// Token: 0x170002CD RID: 717
	// (get) Token: 0x0600190D RID: 6413 RVA: 0x00099AAC File Offset: 0x00097CAC
	public bool HasMessage
	{
		get
		{
			return this.ItemsToSend.Count > 0;
		}
	}

	// Token: 0x0600190E RID: 6414 RVA: 0x00099ABC File Offset: 0x00097CBC
	public DynamicMeshClientConnection(int entityId)
	{
		this.EntityId = entityId;
	}

	// Token: 0x0600190F RID: 6415 RVA: 0x00099B4C File Offset: 0x00097D4C
	public void AddToQueue(DynamicMeshSyncRequest package)
	{
		ValueTuple<int, int> key = new ValueTuple<int, int>(DynamicMeshUnity.RoundRegion(package.Item.WorldPosition.x), DynamicMeshUnity.RoundRegion(package.Item.WorldPosition.z));
		ConcurrentQueue<DynamicMeshSyncRequest> orAdd = this.ItemsToSend.GetOrAdd(key, new ConcurrentQueue<DynamicMeshSyncRequest>());
		orAdd.Enqueue(package);
		this.SendMessage = (DynamicMeshServer.AutoSend || this.SendMessage || orAdd.Count == 1);
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x00099BC4 File Offset: 0x00097DC4
	public bool RequestChunk()
	{
		if (this.CurrentRequestedChunk != 9223372036854775807L)
		{
			return false;
		}
		if (!this.RequestedChunks.TryDequeue(out this.CurrentRequestedChunk))
		{
			return false;
		}
		this.RequestTime = DateTime.Now;
		DynamicMeshThread.RequestChunk(this.CurrentRequestedChunk);
		return true;
	}

	// Token: 0x06001911 RID: 6417 RVA: 0x00099C10 File Offset: 0x00097E10
	public void UpdateItemsToSend(NetPackageDynamicClientArrive package)
	{
		DynamicMeshClientConnection.UpdateItemsToSend(this, package);
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x00099C1C File Offset: 0x00097E1C
	public static void UpdateItemsToSend(DynamicMeshClientConnection data, NetPackageDynamicClientArrive package)
	{
		if (DynamicMeshManager.Instance == null || DynamicMeshManager.Instance.ItemsDictionary == null)
		{
			DynamicMeshManager.LogMsg(package.Sender.playerName + " connected before the world was ready. Can not sync dymesh data. They must reconnect to start the sync");
			data.SendMessage = true;
			return;
		}
		data.SendMessage = false;
		data.ItemsToSend.Clear();
		DynamicMeshManager.LogMsg(string.Concat(new string[]
		{
			"Update items to send for ",
			package.Sender.playerName,
			" id: ",
			package.Sender.entityId.ToString(),
			"  recieved: ",
			package.Items.Count.ToString()
		}));
		EntityPlayer entityPlayer = GameManager.Instance.World.GetPlayers().FirstOrDefault((EntityPlayer d) => d.entityId == package.Sender.entityId);
		Vector3 playerPos = (entityPlayer == null) ? Vector3.zero : entityPlayer.GetPosition();
		playerPos.y = 0f;
		List<DynamicMeshRegion> list = (from d in DynamicMeshRegion.Regions.Values
		orderby Math.Abs(Vector3.Distance(playerPos, d.WorldPosition.ToVector3()))
		select d).ToList<DynamicMeshRegion>();
		int num = 0;
		List<DynamicMeshItem> list2 = new List<DynamicMeshItem>(DynamicMeshManager.Instance.ItemsDictionary.Count);
		NetPackageRegionMetaData package2 = NetPackageManager.GetPackage<NetPackageRegionMetaData>();
		foreach (DynamicMeshRegion dynamicMeshRegion in list)
		{
			num += DynamicMeshClientConnection.ProcessItem(data, dynamicMeshRegion, dynamicMeshRegion.LoadedItems, package, package2);
			num += DynamicMeshClientConnection.ProcessItem(data, dynamicMeshRegion, dynamicMeshRegion.UnloadedItems, package, package2);
		}
		package2.ChunksWithData.AddRange(from d in DynamicMeshThread.PrimaryQueue.Values
		select new Vector2i(d.WorldPosition.x, d.WorldPosition.z));
		package2.ChunksWithData.AddRange(from d in DynamicMeshThread.SecondaryQueue.Values
		select new Vector2i(d.WorldPosition.x, d.WorldPosition.z));
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package2, false, data.EntityId, -1, -1, null, 192, false);
		DynamicMeshManager.LogMsg(string.Concat(new string[]
		{
			"Items to send: ",
			list2.Count.ToString(),
			"   Added: ",
			num.ToString(),
			"   chunks: ",
			package2.ChunksWithData.Count.ToString()
		}));
		data.SendMessage = (data.ItemsToSend.Count > 0);
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x00099EFC File Offset: 0x000980FC
	[PublicizedFrom(EAccessModifier.Private)]
	public static int ProcessItem(DynamicMeshClientConnection conn, DynamicMeshRegion r, List<DynamicMeshItem> items, NetPackageDynamicClientArrive package, NetPackageRegionMetaData allChunkData)
	{
		int num = 0;
		using (List<DynamicMeshItem>.Enumerator enumerator = items.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				DynamicMeshItem i = enumerator.Current;
				if (i != null)
				{
					if (i.FileExists())
					{
						allChunkData.ChunksWithData.Add(new Vector2i(i.WorldPosition.x, i.WorldPosition.z));
					}
					if (!package.Items.Any((RegionItemData d) => d.X == i.WorldPosition.x && d.Z == i.WorldPosition.z && d.UpdateTime == i.UpdateTime))
					{
						DynamicMeshSyncRequest package2 = DynamicMeshSyncRequest.Create(i, false, conn.EntityId);
						conn.AddToQueue(package2);
						num++;
					}
				}
			}
		}
		return num;
	}

	// Token: 0x04001005 RID: 4101
	public int EntityId;

	// Token: 0x04001006 RID: 4102
	public ConcurrentDictionary<ValueTuple<int, int>, ConcurrentQueue<DynamicMeshSyncRequest>> ItemsToSend = new ConcurrentDictionary<ValueTuple<int, int>, ConcurrentQueue<DynamicMeshSyncRequest>>();

	// Token: 0x04001007 RID: 4103
	public ConcurrentQueue<long> RequestedChunks = new ConcurrentQueue<long>();

	// Token: 0x04001008 RID: 4104
	public List<long> FinalChunks = new List<long>();

	// Token: 0x04001009 RID: 4105
	public long CurrentRequestedChunk = long.MaxValue;

	// Token: 0x0400100A RID: 4106
	public DateTime RequestTime = DateTime.Now.AddDays(-1.0);

	// Token: 0x0400100B RID: 4107
	public DateTime LastSend = DateTime.Now.AddDays(-1.0);

	// Token: 0x0400100C RID: 4108
	public bool SendMessage;

	// Token: 0x0400100D RID: 4109
	public ValueTuple<int, int> LastKey = ValueTuple.Create<int, int>(0, 0);
}
