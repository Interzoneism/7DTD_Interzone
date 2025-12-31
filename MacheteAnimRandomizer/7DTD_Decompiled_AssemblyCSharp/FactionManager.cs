using System;
using System.IO;
using UnityEngine;

// Token: 0x020004A4 RID: 1188
public class FactionManager
{
	// Token: 0x060026F3 RID: 9971 RVA: 0x000FCF8C File Offset: 0x000FB18C
	public void PrintData()
	{
		for (int i = 0; i < this.Factions.Length; i++)
		{
			if (this.Factions[i] != null)
			{
				Log.Out(this.Factions[i].ToString());
			}
		}
	}

	// Token: 0x060026F4 RID: 9972 RVA: 0x000FCFC8 File Offset: 0x000FB1C8
	public static void Init()
	{
		FactionManager.Instance = new FactionManager();
	}

	// Token: 0x060026F5 RID: 9973 RVA: 0x000FCFD4 File Offset: 0x000FB1D4
	public void Update()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || GameManager.Instance.World == null || GameManager.Instance.World.Players == null || GameManager.Instance.World.Players.Count == 0 || GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		this.saveTime -= Time.deltaTime;
		if (this.saveTime <= 0f && (this.dataSaveThreadInfo == null || this.dataSaveThreadInfo.HasTerminated()))
		{
			this.saveTime = 60f;
			this.Save();
		}
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x000FD078 File Offset: 0x000FB278
	public FactionManager.Relationship GetRelationshipTier(EntityAlive checkingEntity, EntityAlive targetEntity)
	{
		if (checkingEntity == null || targetEntity == null)
		{
			return FactionManager.Relationship.Neutral;
		}
		this.rel = this.GetRelationshipValue(checkingEntity, targetEntity);
		if (this.rel < 200f)
		{
			return FactionManager.Relationship.Hate;
		}
		if (this.rel < 400f)
		{
			return FactionManager.Relationship.Dislike;
		}
		if (this.rel < 600f)
		{
			return FactionManager.Relationship.Neutral;
		}
		if (this.rel < 800f)
		{
			return FactionManager.Relationship.Like;
		}
		if (this.rel < 1001f)
		{
			return FactionManager.Relationship.Love;
		}
		return FactionManager.Relationship.Leader;
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x000FD10C File Offset: 0x000FB30C
	public Faction CreateFaction(string _name = "", bool _playerFaction = true, string _icon = "")
	{
		Faction faction = new Faction(_name, _playerFaction, _icon);
		this.AddFaction(faction);
		return faction;
	}

	// Token: 0x060026F8 RID: 9976 RVA: 0x000FD12C File Offset: 0x000FB32C
	public void AddFaction(Faction _faction)
	{
		for (int i = _faction.IsPlayerFaction ? 8 : 0; i < this.Factions.Length; i++)
		{
			if (this.Factions[i] == null)
			{
				this.Factions[i] = _faction;
				_faction.ID = (byte)i;
				return;
			}
		}
	}

	// Token: 0x060026F9 RID: 9977 RVA: 0x000FD173 File Offset: 0x000FB373
	public void RemoveFaction(byte _id)
	{
		this.Factions[(int)_id] = null;
	}

	// Token: 0x060026FA RID: 9978 RVA: 0x000FD17E File Offset: 0x000FB37E
	public Faction GetFaction(byte _id)
	{
		return this.Factions[(int)_id];
	}

	// Token: 0x060026FB RID: 9979 RVA: 0x000FD188 File Offset: 0x000FB388
	public Faction GetFactionByName(string _name)
	{
		for (int i = 0; i < this.Factions.Length; i++)
		{
			if (this.Factions[i].Name == _name)
			{
				return this.Factions[i];
			}
		}
		return null;
	}

	// Token: 0x060026FC RID: 9980 RVA: 0x000FD1C8 File Offset: 0x000FB3C8
	public float GetRelationshipValue(EntityAlive checkingEntity, EntityAlive targetEntity)
	{
		if (checkingEntity == null || targetEntity == null)
		{
			return 400f;
		}
		if (checkingEntity.factionId == targetEntity.factionId)
		{
			return 800f;
		}
		if (this.Factions[(int)checkingEntity.factionId] != null && this.Factions[(int)targetEntity.factionId] != null)
		{
			return this.Factions[(int)checkingEntity.factionId].GetRelationship(targetEntity.factionId);
		}
		return 400f;
	}

	// Token: 0x060026FD RID: 9981 RVA: 0x000FD23D File Offset: 0x000FB43D
	public void SetRelationship(byte _myFaction, byte _targetFaction, sbyte _modification)
	{
		if (this.Factions[(int)_myFaction] != null)
		{
			this.Factions[(int)_myFaction].ModifyRelationship(_targetFaction, (float)_modification);
		}
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x000FD23D File Offset: 0x000FB43D
	public void ModifyRelationship(byte _myFaction, byte _targetFaction, sbyte _modification)
	{
		if (this.Factions[(int)_myFaction] != null)
		{
			this.Factions[(int)_myFaction].ModifyRelationship(_targetFaction, (float)_modification);
		}
	}

	// Token: 0x060026FF RID: 9983 RVA: 0x000FD25C File Offset: 0x000FB45C
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(FactionManager.Version);
		for (int i = 0; i < this.Factions.Length; i++)
		{
			_bw.Write(this.Factions[i] != null);
			if (this.Factions[i] != null)
			{
				this.Factions[i].Write(_bw);
			}
		}
	}

	// Token: 0x06002700 RID: 9984 RVA: 0x000FD2B0 File Offset: 0x000FB4B0
	public void Read(BinaryReader _br)
	{
		_br.ReadByte();
		for (int i = 0; i < 255; i++)
		{
			if (_br.ReadBoolean())
			{
				this.Factions[i] = new Faction();
				this.Factions[i].Read(_br);
			}
		}
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x000FD2F8 File Offset: 0x000FB4F8
	[PublicizedFrom(EAccessModifier.Private)]
	public int saveFactionDataThreaded(ThreadManager.ThreadInfo _threadInfo)
	{
		PooledExpandableMemoryStream pooledExpandableMemoryStream = (PooledExpandableMemoryStream)_threadInfo.parameter;
		string text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "factions.dat");
		if (!SdDirectory.Exists(GameIO.GetSaveGameDir()))
		{
			return -1;
		}
		if (SdFile.Exists(text))
		{
			SdFile.Copy(text, string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "factions.dat.bak"), true);
		}
		pooledExpandableMemoryStream.Position = 0L;
		StreamUtils.WriteStreamToFile(pooledExpandableMemoryStream, text);
		MemoryPools.poolMemoryStream.FreeSync(pooledExpandableMemoryStream);
		return -1;
	}

	// Token: 0x06002702 RID: 9986 RVA: 0x000FD374 File Offset: 0x000FB574
	public void Save()
	{
		if (this.dataSaveThreadInfo == null || !ThreadManager.ActiveThreads.ContainsKey("factionDataSave"))
		{
			PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				this.Write(pooledBinaryWriter);
			}
			this.dataSaveThreadInfo = ThreadManager.StartThread("factionDataSave", null, new ThreadManager.ThreadFunctionLoopDelegate(this.saveFactionDataThreaded), null, pooledExpandableMemoryStream, null, false, true);
		}
	}

	// Token: 0x06002703 RID: 9987 RVA: 0x000FD400 File Offset: 0x000FB600
	public void Load()
	{
		string path = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "factions.dat");
		if (SdDirectory.Exists(GameIO.GetSaveGameDir()) && SdFile.Exists(path))
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
				path = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "factions.dat.bak");
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

	// Token: 0x04001DC7 RID: 7623
	[PublicizedFrom(EAccessModifier.Private)]
	public const float SAVE_TIME_SEC = 60f;

	// Token: 0x04001DC8 RID: 7624
	public static FactionManager Instance;

	// Token: 0x04001DC9 RID: 7625
	[PublicizedFrom(EAccessModifier.Private)]
	public static byte Version = 1;

	// Token: 0x04001DCA RID: 7626
	[PublicizedFrom(EAccessModifier.Private)]
	public Faction[] Factions = new Faction[255];

	// Token: 0x04001DCB RID: 7627
	[PublicizedFrom(EAccessModifier.Private)]
	public float saveTime = 60f;

	// Token: 0x04001DCC RID: 7628
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.ThreadInfo dataSaveThreadInfo;

	// Token: 0x04001DCD RID: 7629
	[PublicizedFrom(EAccessModifier.Private)]
	public float rel;

	// Token: 0x020004A5 RID: 1189
	public enum Relationship
	{
		// Token: 0x04001DCF RID: 7631
		Hate,
		// Token: 0x04001DD0 RID: 7632
		Dislike = 200,
		// Token: 0x04001DD1 RID: 7633
		Neutral = 400,
		// Token: 0x04001DD2 RID: 7634
		Like = 600,
		// Token: 0x04001DD3 RID: 7635
		Love = 800,
		// Token: 0x04001DD4 RID: 7636
		Leader = 1001
	}
}
