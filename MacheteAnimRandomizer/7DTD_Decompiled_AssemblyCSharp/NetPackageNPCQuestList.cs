using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000762 RID: 1890
[Preserve]
public class NetPackageNPCQuestList : NetPackage
{
	// Token: 0x06003718 RID: 14104 RVA: 0x0016945D File Offset: 0x0016765D
	public NetPackageNPCQuestList Setup(int _npcEntityID, int _playerEntityID)
	{
		this.npcEntityID = _npcEntityID;
		this.playerEntityID = _playerEntityID;
		this.eventType = NetPackageNPCQuestList.NPCQuestEventTypes.ResetQuests;
		return this;
	}

	// Token: 0x06003719 RID: 14105 RVA: 0x00169475 File Offset: 0x00167675
	public NetPackageNPCQuestList Setup(int _playerEntityID, Vector2 _questGiverPos, int _tierLevel, Vector2 _prefabPos)
	{
		this.playerEntityID = _playerEntityID;
		this.tierLevel = _tierLevel;
		this.questGiverPos = _questGiverPos;
		this.prefabPos = _prefabPos;
		this.eventType = NetPackageNPCQuestList.NPCQuestEventTypes.AddUsedPOI;
		return this;
	}

	// Token: 0x0600371A RID: 14106 RVA: 0x0016949C File Offset: 0x0016769C
	public NetPackageNPCQuestList SetupClear(int _playerEntityID, Vector2 _questGiverPos, int _tierLevel)
	{
		this.playerEntityID = _playerEntityID;
		this.tierLevel = _tierLevel;
		this.questGiverPos = _questGiverPos;
		this.eventType = NetPackageNPCQuestList.NPCQuestEventTypes.ClearUsedPOI;
		return this;
	}

	// Token: 0x0600371B RID: 14107 RVA: 0x001694BB File Offset: 0x001676BB
	public NetPackageNPCQuestList Setup(int _npcEntityID, int _playerEntityID, int _tierLevel)
	{
		this.npcEntityID = _npcEntityID;
		this.playerEntityID = _playerEntityID;
		this.tierLevel = _tierLevel;
		this.eventType = NetPackageNPCQuestList.NPCQuestEventTypes.FetchList;
		return this;
	}

	// Token: 0x0600371C RID: 14108 RVA: 0x001694DA File Offset: 0x001676DA
	public NetPackageNPCQuestList Setup(int _npcEntityID, int _playerEntityID, int _tierLevel, byte _removeIndex)
	{
		this.npcEntityID = _npcEntityID;
		this.playerEntityID = _playerEntityID;
		this.tierLevel = _tierLevel;
		this.eventType = NetPackageNPCQuestList.NPCQuestEventTypes.RemoveQuest;
		this.removeIndex = _removeIndex;
		return this;
	}

	// Token: 0x0600371D RID: 14109 RVA: 0x00169501 File Offset: 0x00167701
	public NetPackageNPCQuestList Setup(int _npcEntityID, int _playerEntityID, NetPackageNPCQuestList.QuestPacketEntry[] _questPacketEntries)
	{
		this.npcEntityID = _npcEntityID;
		this.playerEntityID = _playerEntityID;
		this.questPacketEntries = _questPacketEntries;
		this.eventType = NetPackageNPCQuestList.NPCQuestEventTypes.FetchList;
		return this;
	}

	// Token: 0x0600371E RID: 14110 RVA: 0x00169520 File Offset: 0x00167720
	public override void read(PooledBinaryReader _reader)
	{
		this.npcEntityID = _reader.ReadInt32();
		this.playerEntityID = _reader.ReadInt32();
		this.eventType = (NetPackageNPCQuestList.NPCQuestEventTypes)_reader.ReadByte();
		if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.FetchList)
		{
			this.tierLevel = _reader.ReadInt32();
			int num = _reader.ReadInt32();
			if (num > 0)
			{
				this.questPacketEntries = new NetPackageNPCQuestList.QuestPacketEntry[num];
				for (int i = 0; i < num; i++)
				{
					this.questPacketEntries[i].read(_reader);
				}
				return;
			}
			this.questPacketEntries = null;
			return;
		}
		else
		{
			if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.RemoveQuest)
			{
				this.tierLevel = _reader.ReadInt32();
				this.removeIndex = _reader.ReadByte();
				return;
			}
			if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.AddUsedPOI)
			{
				this.tierLevel = _reader.ReadInt32();
				this.questGiverPos = StreamUtils.ReadVector2(_reader);
				this.prefabPos = StreamUtils.ReadVector2(_reader);
				return;
			}
			if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.ClearUsedPOI)
			{
				this.tierLevel = _reader.ReadInt32();
				this.questGiverPos = StreamUtils.ReadVector2(_reader);
			}
			return;
		}
	}

	// Token: 0x0600371F RID: 14111 RVA: 0x00169614 File Offset: 0x00167814
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.npcEntityID);
		_writer.Write(this.playerEntityID);
		_writer.Write((byte)this.eventType);
		if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.FetchList)
		{
			_writer.Write(this.tierLevel);
			if (this.questPacketEntries != null)
			{
				_writer.Write(this.questPacketEntries.Length);
				for (int i = 0; i < this.questPacketEntries.Length; i++)
				{
					this.questPacketEntries[i].write(_writer);
				}
				return;
			}
			_writer.Write(0);
			return;
		}
		else
		{
			if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.RemoveQuest)
			{
				_writer.Write(this.tierLevel);
				_writer.Write(this.removeIndex);
				return;
			}
			if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.AddUsedPOI)
			{
				_writer.Write(this.tierLevel);
				StreamUtils.Write(_writer, this.questGiverPos);
				StreamUtils.Write(_writer, this.prefabPos);
				return;
			}
			if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.ClearUsedPOI)
			{
				_writer.Write(this.tierLevel);
				StreamUtils.Write(_writer, this.questGiverPos);
			}
			return;
		}
	}

	// Token: 0x06003720 RID: 14112 RVA: 0x00169718 File Offset: 0x00167918
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			EntityPlayer entityPlayer = _world.GetEntity(this.playerEntityID) as EntityPlayer;
			if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.AddUsedPOI)
			{
				entityPlayer.QuestJournal.AddPOIToTraderData(this.tierLevel, this.questGiverPos, this.prefabPos);
				return;
			}
			EntityTrader entityTrader = _world.GetEntity(this.npcEntityID) as EntityTrader;
			entityTrader.activeQuests = QuestEventManager.Current.GetQuestList(_world, this.npcEntityID, this.playerEntityID);
			if (entityTrader.activeQuests == null)
			{
				entityTrader.activeQuests = entityTrader.PopulateActiveQuests(entityPlayer, this.tierLevel, -1);
			}
			QuestEventManager.Current.SetupQuestList(entityTrader, this.playerEntityID, entityTrader.activeQuests);
			if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.FetchList)
			{
				NetPackageNPCQuestList.SendQuestPacketsToPlayer(entityTrader, this.playerEntityID);
				return;
			}
			if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.RemoveQuest)
			{
				List<Quest> questList = QuestEventManager.Current.GetQuestList(_world, this.npcEntityID, this.playerEntityID);
				int num = 0;
				for (int i = 0; i < questList.Count; i++)
				{
					if ((int)questList[i].QuestClass.DifficultyTier == this.tierLevel)
					{
						if (num == (int)this.removeIndex)
						{
							questList.RemoveAt(i);
							break;
						}
						num++;
					}
				}
				QuestEventManager.Current.SetupQuestList(entityTrader, this.playerEntityID, questList);
				return;
			}
			QuestEventManager.Current.ClearQuestList(this.npcEntityID);
			Log.Out(string.Concat(new string[]
			{
				"Quests Reset for NPC: ",
				this.npcEntityID.ToString(),
				" by Player: ",
				this.playerEntityID.ToString(),
				"."
			}));
			return;
		}
		else
		{
			EntityPlayer entityPlayer2 = _world.GetEntity(this.playerEntityID) as EntityPlayer;
			if (this.eventType == NetPackageNPCQuestList.NPCQuestEventTypes.ClearUsedPOI)
			{
				entityPlayer2.QuestJournal.ClearTraderDataTier(this.tierLevel, this.questGiverPos);
				return;
			}
			(_world.GetEntity(this.npcEntityID) as EntityTrader).SetActiveQuests(entityPlayer2, this.questPacketEntries);
			return;
		}
	}

	// Token: 0x06003721 RID: 14113 RVA: 0x0016990C File Offset: 0x00167B0C
	public static void SendQuestPacketsToPlayer(EntityTrader npc, int playerEntityID)
	{
		if (npc.activeQuests != null)
		{
			int count = npc.activeQuests.Count;
			NetPackageNPCQuestList.QuestPacketEntry[] array = new NetPackageNPCQuestList.QuestPacketEntry[count];
			for (int i = 0; i < count; i++)
			{
				Quest quest = npc.activeQuests[i];
				Vector3 traderPos = (npc.traderArea != null) ? npc.traderArea.Position : npc.position;
				array[i].QuestID = quest.ID;
				array[i].QuestLocation = quest.GetLocation();
				array[i].QuestSize = quest.GetLocationSize();
				array[i].POIName = ((quest.QuestPrefab != null) ? quest.QuestPrefab.location.Name : "UNNAMED");
				array[i].TraderPos = traderPos;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageNPCQuestList>().Setup(npc.entityId, playerEntityID, array), false, playerEntityID, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06003722 RID: 14114 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002CA1 RID: 11425
	public int npcEntityID;

	// Token: 0x04002CA2 RID: 11426
	public int playerEntityID;

	// Token: 0x04002CA3 RID: 11427
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageNPCQuestList.NPCQuestEventTypes eventType;

	// Token: 0x04002CA4 RID: 11428
	public NetPackageNPCQuestList.QuestPacketEntry[] questPacketEntries;

	// Token: 0x04002CA5 RID: 11429
	public int tierLevel = -1;

	// Token: 0x04002CA6 RID: 11430
	public byte removeIndex;

	// Token: 0x04002CA7 RID: 11431
	public Vector2 questGiverPos;

	// Token: 0x04002CA8 RID: 11432
	public Vector2 prefabPos;

	// Token: 0x02000763 RID: 1891
	[PublicizedFrom(EAccessModifier.Private)]
	public enum NPCQuestEventTypes
	{
		// Token: 0x04002CAA RID: 11434
		FetchList,
		// Token: 0x04002CAB RID: 11435
		RemoveQuest,
		// Token: 0x04002CAC RID: 11436
		ResetQuests,
		// Token: 0x04002CAD RID: 11437
		AddUsedPOI,
		// Token: 0x04002CAE RID: 11438
		ClearUsedPOI
	}

	// Token: 0x02000764 RID: 1892
	public struct QuestPacketEntry
	{
		// Token: 0x06003724 RID: 14116 RVA: 0x00169A26 File Offset: 0x00167C26
		public void read(BinaryReader _reader)
		{
			this.QuestID = _reader.ReadString();
			this.QuestLocation = StreamUtils.ReadVector3(_reader);
			this.QuestSize = StreamUtils.ReadVector3(_reader);
			this.POIName = _reader.ReadString();
			this.TraderPos = StreamUtils.ReadVector3(_reader);
		}

		// Token: 0x06003725 RID: 14117 RVA: 0x00169A64 File Offset: 0x00167C64
		public void write(BinaryWriter _writer)
		{
			_writer.Write(this.QuestID);
			StreamUtils.Write(_writer, this.QuestLocation);
			StreamUtils.Write(_writer, this.QuestSize);
			_writer.Write(this.POIName);
			StreamUtils.Write(_writer, this.TraderPos);
		}

		// Token: 0x04002CAF RID: 11439
		public string QuestID;

		// Token: 0x04002CB0 RID: 11440
		public Vector3 QuestLocation;

		// Token: 0x04002CB1 RID: 11441
		public Vector3 QuestSize;

		// Token: 0x04002CB2 RID: 11442
		public Vector3 TraderPos;

		// Token: 0x04002CB3 RID: 11443
		public string POIName;
	}
}
