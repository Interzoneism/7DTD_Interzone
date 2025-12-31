using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000844 RID: 2116
public class PowerManager
{
	// Token: 0x17000644 RID: 1604
	// (get) Token: 0x06003CC5 RID: 15557 RVA: 0x001866D2 File Offset: 0x001848D2
	// (set) Token: 0x06003CC6 RID: 15558 RVA: 0x001866DA File Offset: 0x001848DA
	public byte CurrentFileVersion { get; set; }

	// Token: 0x17000645 RID: 1605
	// (get) Token: 0x06003CC7 RID: 15559 RVA: 0x001866E3 File Offset: 0x001848E3
	public static PowerManager Instance
	{
		get
		{
			if (PowerManager.instance == null)
			{
				PowerManager.instance = new PowerManager();
			}
			return PowerManager.instance;
		}
	}

	// Token: 0x17000646 RID: 1606
	// (get) Token: 0x06003CC8 RID: 15560 RVA: 0x001866FB File Offset: 0x001848FB
	public static bool HasInstance
	{
		get
		{
			return PowerManager.instance != null;
		}
	}

	// Token: 0x06003CC9 RID: 15561 RVA: 0x00186708 File Offset: 0x00184908
	[PublicizedFrom(EAccessModifier.Private)]
	public PowerManager()
	{
		PowerManager.instance = this;
		this.Circuits = new List<PowerItem>();
		this.PowerSources = new List<PowerSource>();
		this.PowerTriggers = new List<PowerTrigger>();
	}

	// Token: 0x06003CCA RID: 15562 RVA: 0x00186764 File Offset: 0x00184964
	public void Update()
	{
		if (GameManager.Instance.World == null || GameManager.Instance.World.Players == null || GameManager.Instance.World.Players.Count == 0)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && GameManager.Instance.gameStateManager.IsGameStarted())
		{
			this.updateTime -= Time.deltaTime;
			if (this.updateTime <= 0f)
			{
				for (int i = 0; i < this.PowerSources.Count; i++)
				{
					this.PowerSources[i].Update();
				}
				for (int j = 0; j < this.PowerTriggers.Count; j++)
				{
					this.PowerTriggers[j].CachedUpdateCall();
				}
				this.updateTime = 0.16f;
			}
			this.saveTime -= Time.deltaTime;
			if (this.saveTime <= 0f && (this.dataSaveThreadInfo == null || this.dataSaveThreadInfo.HasTerminated()))
			{
				this.saveTime = 120f;
				this.SavePowerManager();
			}
		}
		for (int k = 0; k < this.ClientUpdateList.Count; k++)
		{
			this.ClientUpdateList[k].ClientUpdate();
		}
	}

	// Token: 0x06003CCB RID: 15563 RVA: 0x001868AC File Offset: 0x00184AAC
	[PublicizedFrom(EAccessModifier.Private)]
	public int savePowerDataThreaded(ThreadManager.ThreadInfo _threadInfo)
	{
		PooledExpandableMemoryStream pooledExpandableMemoryStream = (PooledExpandableMemoryStream)_threadInfo.parameter;
		string text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "power.dat");
		if (SdFile.Exists(text))
		{
			SdFile.Copy(text, string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "power.dat.bak"), true);
		}
		pooledExpandableMemoryStream.Position = 0L;
		StreamUtils.WriteStreamToFile(pooledExpandableMemoryStream, text);
		MemoryPools.poolMemoryStream.FreeSync(pooledExpandableMemoryStream);
		return -1;
	}

	// Token: 0x06003CCC RID: 15564 RVA: 0x00186918 File Offset: 0x00184B18
	public void LoadPowerManager()
	{
		string path = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "power.dat");
		if (SdFile.Exists(path))
		{
			try
			{
				using (Stream stream = SdFile.OpenRead(path))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(stream);
						this.Read(pooledBinaryReader);
					}
				}
			}
			catch (Exception)
			{
				path = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "power.dat.bak");
				if (SdFile.Exists(path))
				{
					using (Stream stream2 = SdFile.OpenRead(path))
					{
						using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader2.SetBaseStream(stream2);
							this.Read(pooledBinaryReader2);
						}
					}
				}
			}
		}
	}

	// Token: 0x06003CCD RID: 15565 RVA: 0x00186A20 File Offset: 0x00184C20
	public void SavePowerManager()
	{
		if (this.dataSaveThreadInfo == null || !ThreadManager.ActiveThreads.ContainsKey("powerDataSave"))
		{
			PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				this.Write(pooledBinaryWriter);
			}
			this.dataSaveThreadInfo = ThreadManager.StartThread("powerDataSave", null, new ThreadManager.ThreadFunctionLoopDelegate(this.savePowerDataThreaded), null, pooledExpandableMemoryStream, null, false, true);
		}
	}

	// Token: 0x06003CCE RID: 15566 RVA: 0x00186AAC File Offset: 0x00184CAC
	public void Write(BinaryWriter bw)
	{
		bw.Write(PowerManager.FileVersion);
		bw.Write(this.Circuits.Count);
		for (int i = 0; i < this.Circuits.Count; i++)
		{
			bw.Write((byte)this.Circuits[i].PowerItemType);
			this.Circuits[i].write(bw);
		}
	}

	// Token: 0x06003CCF RID: 15567 RVA: 0x00186B18 File Offset: 0x00184D18
	public void Read(BinaryReader br)
	{
		this.CurrentFileVersion = br.ReadByte();
		this.Circuits.Clear();
		int num = br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			PowerItem powerItem = PowerItem.CreateItem((PowerItem.PowerItemTypes)br.ReadByte());
			powerItem.read(br, this.CurrentFileVersion);
			this.AddPowerNode(powerItem, null);
		}
	}

	// Token: 0x06003CD0 RID: 15568 RVA: 0x00186B70 File Offset: 0x00184D70
	public void Cleanup()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.SavePowerManager();
		}
		PowerManager.instance = null;
		this.Circuits.Clear();
		if (this.dataSaveThreadInfo != null)
		{
			this.dataSaveThreadInfo.WaitForEnd(30);
			this.dataSaveThreadInfo = null;
		}
	}

	// Token: 0x06003CD1 RID: 15569 RVA: 0x00186BBC File Offset: 0x00184DBC
	public void AddPowerNode(PowerItem node, PowerItem parent = null)
	{
		this.Circuits.Add(node);
		this.SetParent(node, parent);
		if (node is PowerSource)
		{
			this.PowerSources.Add((PowerSource)node);
		}
		if (node is PowerTrigger)
		{
			this.PowerTriggers.Add((PowerTrigger)node);
		}
		this.PowerItemDictionary.Add(node.Position, node);
	}

	// Token: 0x06003CD2 RID: 15570 RVA: 0x00186C24 File Offset: 0x00184E24
	public void RemovePowerNode(PowerItem node)
	{
		foreach (PowerItem child in new List<PowerItem>(node.Children))
		{
			this.SetParent(child, null);
		}
		this.SetParent(node, null);
		this.Circuits.Remove(node);
		if (node is PowerSource)
		{
			this.PowerSources.Remove((PowerSource)node);
		}
		if (node is PowerTrigger)
		{
			this.PowerTriggers.Remove((PowerTrigger)node);
		}
		if (this.PowerItemDictionary.ContainsKey(node.Position))
		{
			this.PowerItemDictionary.Remove(node.Position);
		}
	}

	// Token: 0x06003CD3 RID: 15571 RVA: 0x00186CEC File Offset: 0x00184EEC
	public unsafe void RemoveUnloadedPowerNodes(ICollection<long> _chunks)
	{
		int num = 0;
		int count = this.PowerItemDictionary.Count;
		Span<Vector3i> span = new Span<Vector3i>(stackalloc byte[checked(unchecked((UIntPtr)count) * (UIntPtr)sizeof(Vector3i))], count);
		foreach (KeyValuePair<Vector3i, PowerItem> keyValuePair in this.PowerItemDictionary)
		{
			long num2 = WorldChunkCache.MakeChunkKey(World.toChunkXZ(keyValuePair.Key.x), World.toChunkXZ(keyValuePair.Key.z));
			using (IEnumerator<long> enumerator2 = _chunks.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current == num2)
					{
						*span[num++] = keyValuePair.Key;
					}
				}
			}
		}
		for (int i = 0; i < num; i++)
		{
			PowerItem node;
			if (this.PowerItemDictionary.TryGetValue(*span[i], out node))
			{
				this.RemovePowerNode(node);
			}
		}
	}

	// Token: 0x06003CD4 RID: 15572 RVA: 0x00186E08 File Offset: 0x00185008
	public void SetParent(PowerItem child, PowerItem parent)
	{
		if (child == null)
		{
			return;
		}
		if (child.Parent == parent)
		{
			return;
		}
		if (this.CircularParentCheck(parent, child))
		{
			return;
		}
		if (child.Parent != null)
		{
			this.RemoveParent(child);
		}
		if (parent == null)
		{
			return;
		}
		if (child != null && this.Circuits.Contains(child))
		{
			this.Circuits.Remove(child);
		}
		parent.Children.Add(child);
		child.Parent = parent;
		child.SendHasLocalChangesToRoot();
	}

	// Token: 0x06003CD5 RID: 15573 RVA: 0x00186E78 File Offset: 0x00185078
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CircularParentCheck(PowerItem Parent, PowerItem Child)
	{
		return Parent == Child || (Parent != null && Parent.Parent != null && this.CircularParentCheck(Parent.Parent, Child));
	}

	// Token: 0x06003CD6 RID: 15574 RVA: 0x00186E9C File Offset: 0x0018509C
	public void RemoveParent(PowerItem node)
	{
		if (node.Parent != null)
		{
			PowerItem parent = node.Parent;
			node.Parent.Children.Remove(node);
			if (node.Parent.TileEntity != null)
			{
				node.Parent.TileEntity.CreateWireDataFromPowerItem();
				node.Parent.TileEntity.DrawWires();
			}
			node.Parent = null;
			this.Circuits.Add(node);
			parent.SendHasLocalChangesToRoot();
			node.HandleDisconnect();
		}
	}

	// Token: 0x06003CD7 RID: 15575 RVA: 0x00186F14 File Offset: 0x00185114
	public void RemoveChild(PowerItem child)
	{
		child.Parent.Children.Remove(child);
		child.Parent = null;
		this.Circuits.Add(child);
	}

	// Token: 0x06003CD8 RID: 15576 RVA: 0x00186F3C File Offset: 0x0018513C
	public void SetParent(Vector3i childPos, Vector3i parentPos)
	{
		PowerItem powerItemByWorldPos = this.GetPowerItemByWorldPos(parentPos);
		PowerItem powerItemByWorldPos2 = this.GetPowerItemByWorldPos(childPos);
		this.SetParent(powerItemByWorldPos2, powerItemByWorldPos);
	}

	// Token: 0x06003CD9 RID: 15577 RVA: 0x00186F61 File Offset: 0x00185161
	public PowerItem GetPowerItemByWorldPos(Vector3i position)
	{
		if (this.PowerItemDictionary.ContainsKey(position))
		{
			return this.PowerItemDictionary[position];
		}
		return null;
	}

	// Token: 0x06003CDA RID: 15578 RVA: 0x00186F80 File Offset: 0x00185180
	public void LogPowerManager()
	{
		for (int i = 0; i < this.PowerSources.Count; i++)
		{
			this.LogChildren(this.PowerSources[i]);
		}
	}

	// Token: 0x06003CDB RID: 15579 RVA: 0x00186FB8 File Offset: 0x001851B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogChildren(PowerItem item)
	{
		try
		{
			Log.Out(string.Format("{0}{1}({2}) - Pos:{3} | Powered:{4}", new object[]
			{
				new string('\t', (int)((item.Depth > 100) ? 0 : (item.Depth + 1))),
				item.ToString(),
				item.Depth,
				item.Position,
				item.IsPowered
			}));
			for (int i = 0; i < item.Children.Count; i++)
			{
				this.LogChildren(item.Children[i]);
			}
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
	}

	// Token: 0x04003121 RID: 12577
	[PublicizedFrom(EAccessModifier.Private)]
	public const float UPDATE_TIME_SEC = 0.16f;

	// Token: 0x04003122 RID: 12578
	[PublicizedFrom(EAccessModifier.Private)]
	public const float SAVE_TIME_SEC = 120f;

	// Token: 0x04003123 RID: 12579
	public static byte FileVersion = 2;

	// Token: 0x04003125 RID: 12581
	[PublicizedFrom(EAccessModifier.Private)]
	public static PowerManager instance;

	// Token: 0x04003126 RID: 12582
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PowerItem> Circuits;

	// Token: 0x04003127 RID: 12583
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PowerSource> PowerSources;

	// Token: 0x04003128 RID: 12584
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PowerTrigger> PowerTriggers;

	// Token: 0x04003129 RID: 12585
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<Vector3i, PowerItem> PowerItemDictionary = new Dictionary<Vector3i, PowerItem>();

	// Token: 0x0400312A RID: 12586
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateTime;

	// Token: 0x0400312B RID: 12587
	[PublicizedFrom(EAccessModifier.Private)]
	public float saveTime = 120f;

	// Token: 0x0400312C RID: 12588
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.ThreadInfo dataSaveThreadInfo;

	// Token: 0x0400312D RID: 12589
	public List<TileEntityPoweredBlock> ClientUpdateList = new List<TileEntityPoweredBlock>();
}
