using System;
using UnityEngine.Scripting;

// Token: 0x02000783 RID: 1923
[Preserve]
public class NetPackageQuestObjectiveUpdate : NetPackage
{
	// Token: 0x060037E8 RID: 14312 RVA: 0x0016D403 File Offset: 0x0016B603
	public NetPackageQuestObjectiveUpdate Setup(NetPackageQuestObjectiveUpdate.QuestObjectiveEventTypes _eventType, int _entityID, int _questCode)
	{
		this.senderEntityID = _entityID;
		this.questCode = _questCode;
		this.eventType = _eventType;
		this.blockPos = Vector3i.zero;
		return this;
	}

	// Token: 0x060037E9 RID: 14313 RVA: 0x0016D426 File Offset: 0x0016B626
	public NetPackageQuestObjectiveUpdate Setup(NetPackageQuestObjectiveUpdate.QuestObjectiveEventTypes _eventType, int _entityID, int _questCode, Vector3i _blockPos)
	{
		this.senderEntityID = _entityID;
		this.questCode = _questCode;
		this.eventType = _eventType;
		this.blockPos = _blockPos;
		return this;
	}

	// Token: 0x060037EA RID: 14314 RVA: 0x0016D446 File Offset: 0x0016B646
	public override void read(PooledBinaryReader _reader)
	{
		this.senderEntityID = _reader.ReadInt32();
		this.questCode = _reader.ReadInt32();
		this.eventType = (NetPackageQuestObjectiveUpdate.QuestObjectiveEventTypes)_reader.ReadByte();
		this.blockPos = StreamUtils.ReadVector3i(_reader);
	}

	// Token: 0x060037EB RID: 14315 RVA: 0x0016D478 File Offset: 0x0016B678
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.senderEntityID);
		_writer.Write(this.questCode);
		_writer.Write((byte)this.eventType);
		StreamUtils.Write(_writer, this.blockPos);
	}

	// Token: 0x060037EC RID: 14316 RVA: 0x0016D4B4 File Offset: 0x0016B6B4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		switch (this.eventType)
		{
		case NetPackageQuestObjectiveUpdate.QuestObjectiveEventTypes.TreasureRadiusBreak:
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				EntityPlayerLocal primaryPlayer = _world.GetPrimaryPlayer();
				this.HandlePlayer(_world, primaryPlayer);
				return;
			}
			EntityPlayer entityPlayer = _world.GetEntity(this.senderEntityID) as EntityPlayer;
			if (entityPlayer == null || entityPlayer.Party == null)
			{
				return;
			}
			for (int i = 0; i < entityPlayer.Party.MemberList.Count; i++)
			{
				EntityPlayer entityPlayer2 = entityPlayer.Party.MemberList[i];
				if (entityPlayer2 != entityPlayer)
				{
					if (entityPlayer2 is EntityPlayerLocal)
					{
						this.HandlePlayer(_world, entityPlayer2 as EntityPlayerLocal);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageQuestObjectiveUpdate>().Setup(this.eventType, this.senderEntityID, this.questCode), false, entityPlayer2.entityId, -1, -1, null, 192, false);
					}
				}
			}
			return;
		}
		case NetPackageQuestObjectiveUpdate.QuestObjectiveEventTypes.TreasureComplete:
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				QuestEventManager.Current.FinishTreasureQuest(this.questCode, _world.GetEntity(this.senderEntityID) as EntityPlayer);
				return;
			}
			break;
		case NetPackageQuestObjectiveUpdate.QuestObjectiveEventTypes.BlockActivated:
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				EntityPlayer entityPlayer3 = _world.GetEntity(this.senderEntityID) as EntityPlayer;
				if (entityPlayer3 == null || entityPlayer3.Party == null)
				{
					return;
				}
				for (int j = 0; j < entityPlayer3.Party.MemberList.Count; j++)
				{
					EntityPlayer entityPlayer4 = entityPlayer3.Party.MemberList[j];
					if (entityPlayer4 != entityPlayer3)
					{
						if (entityPlayer4 is EntityPlayerLocal)
						{
							this.HandlePlayer(_world, entityPlayer4 as EntityPlayerLocal);
						}
						else
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageQuestObjectiveUpdate>().Setup(this.eventType, this.senderEntityID, this.questCode, this.blockPos), false, entityPlayer4.entityId, -1, -1, null, 192, false);
						}
					}
				}
				return;
			}
			else
			{
				EntityPlayerLocal primaryPlayer2 = _world.GetPrimaryPlayer();
				this.HandlePlayer(_world, primaryPlayer2);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060037ED RID: 14317 RVA: 0x0016D6D4 File Offset: 0x0016B8D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandlePlayer(World _world, EntityPlayerLocal localPlayer)
	{
		if (localPlayer == null)
		{
			Log.Warning(string.Format("HandlePlayer {0} {1} with no active local player", "NetPackageQuestObjectiveUpdate", this.eventType));
			return;
		}
		EntityPlayer entityPlayer = _world.GetEntity(this.senderEntityID) as EntityPlayer;
		Quest quest = localPlayer.QuestJournal.FindActiveQuest(this.questCode);
		if (quest != null && entityPlayer.GetDistance(localPlayer) < 15f)
		{
			NetPackageQuestObjectiveUpdate.QuestObjectiveEventTypes questObjectiveEventTypes = this.eventType;
			if (questObjectiveEventTypes == NetPackageQuestObjectiveUpdate.QuestObjectiveEventTypes.TreasureRadiusBreak)
			{
				for (int i = 0; i < quest.Objectives.Count; i++)
				{
					if (!quest.Objectives[i].Complete && quest.Objectives[i] is ObjectiveTreasureChest)
					{
						(quest.Objectives[i] as ObjectiveTreasureChest).AddToDestroyCount();
						return;
					}
				}
				return;
			}
			if (questObjectiveEventTypes != NetPackageQuestObjectiveUpdate.QuestObjectiveEventTypes.BlockActivated)
			{
				return;
			}
			for (int j = 0; j < quest.Objectives.Count; j++)
			{
				if (!quest.Objectives[j].Complete && quest.Objectives[j] is ObjectivePOIBlockActivate)
				{
					(quest.Objectives[j] as ObjectivePOIBlockActivate).AddActivatedBlock(this.blockPos);
					return;
				}
			}
		}
	}

	// Token: 0x060037EE RID: 14318 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002D4B RID: 11595
	[PublicizedFrom(EAccessModifier.Private)]
	public int senderEntityID;

	// Token: 0x04002D4C RID: 11596
	[PublicizedFrom(EAccessModifier.Private)]
	public int questCode;

	// Token: 0x04002D4D RID: 11597
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockPos;

	// Token: 0x04002D4E RID: 11598
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageQuestObjectiveUpdate.QuestObjectiveEventTypes eventType;

	// Token: 0x02000784 RID: 1924
	public enum QuestObjectiveEventTypes
	{
		// Token: 0x04002D50 RID: 11600
		TreasureRadiusBreak,
		// Token: 0x04002D51 RID: 11601
		TreasureComplete,
		// Token: 0x04002D52 RID: 11602
		BlockActivated
	}
}
