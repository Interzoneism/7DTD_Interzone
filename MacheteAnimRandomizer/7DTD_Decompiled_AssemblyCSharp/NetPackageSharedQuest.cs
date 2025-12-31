using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000791 RID: 1937
[Preserve]
public class NetPackageSharedQuest : NetPackage
{
	// Token: 0x06003837 RID: 14391 RVA: 0x0016EA5A File Offset: 0x0016CC5A
	public NetPackageSharedQuest Setup(int _questCode, int _sharedByEntityID)
	{
		this.questCode = _questCode;
		this.sharedByEntityID = _sharedByEntityID;
		this.questEvent = NetPackageSharedQuest.SharedQuestEvents.RemoveQuest;
		return this;
	}

	// Token: 0x06003838 RID: 14392 RVA: 0x0016EA72 File Offset: 0x0016CC72
	public NetPackageSharedQuest Setup(int _questCode, int _sharedByEntityID, int _sharedWithEntityID, bool adding)
	{
		this.questCode = _questCode;
		this.sharedByEntityID = _sharedByEntityID;
		this.sharedWithEntityID = _sharedWithEntityID;
		this.questEvent = (adding ? NetPackageSharedQuest.SharedQuestEvents.AddSharedMember : NetPackageSharedQuest.SharedQuestEvents.RemoveSharedMember);
		return this;
	}

	// Token: 0x06003839 RID: 14393 RVA: 0x0016EA98 File Offset: 0x0016CC98
	public NetPackageSharedQuest Setup(int _questCode, string _questID, string _poiName, Vector3 _position, Vector3 _size, Vector3 _returnPos, int _sharedByEntityID, int _sharedWithEntityID, int _questGiverID)
	{
		this.questCode = _questCode;
		this.questID = _questID;
		this.poiName = _poiName;
		this.position = _position;
		this.size = _size;
		this.returnPos = _returnPos;
		this.sharedByEntityID = _sharedByEntityID;
		this.sharedWithEntityID = _sharedWithEntityID;
		this.questGiverID = _questGiverID;
		this.questEvent = NetPackageSharedQuest.SharedQuestEvents.ShareQuest;
		return this;
	}

	// Token: 0x0600383A RID: 14394 RVA: 0x0016EAF4 File Offset: 0x0016CCF4
	public override void read(PooledBinaryReader _br)
	{
		this.sharedByEntityID = _br.ReadInt32();
		this.questEvent = (NetPackageSharedQuest.SharedQuestEvents)_br.ReadByte();
		if (this.questEvent == NetPackageSharedQuest.SharedQuestEvents.ShareQuest)
		{
			this.questCode = _br.ReadInt32();
			this.questID = _br.ReadString();
			this.poiName = _br.ReadString();
			this.position = StreamUtils.ReadVector3(_br);
			this.size = StreamUtils.ReadVector3(_br);
			this.returnPos = StreamUtils.ReadVector3(_br);
			this.questGiverID = _br.ReadInt32();
			this.sharedWithEntityID = _br.ReadInt32();
			return;
		}
		if (this.questEvent == NetPackageSharedQuest.SharedQuestEvents.RemoveQuest)
		{
			this.questCode = _br.ReadInt32();
			return;
		}
		this.questCode = _br.ReadInt32();
		this.sharedWithEntityID = _br.ReadInt32();
	}

	// Token: 0x0600383B RID: 14395 RVA: 0x0016EBB0 File Offset: 0x0016CDB0
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.sharedByEntityID);
		_bw.Write((byte)this.questEvent);
		if (this.questEvent == NetPackageSharedQuest.SharedQuestEvents.ShareQuest)
		{
			_bw.Write(this.questCode);
			_bw.Write(this.questID);
			_bw.Write(this.poiName);
			StreamUtils.Write(_bw, this.position);
			StreamUtils.Write(_bw, this.size);
			StreamUtils.Write(_bw, this.returnPos);
			_bw.Write(this.questGiverID);
			_bw.Write(this.sharedWithEntityID);
			return;
		}
		if (this.questEvent == NetPackageSharedQuest.SharedQuestEvents.RemoveQuest)
		{
			_bw.Write(this.questCode);
			return;
		}
		_bw.Write(this.questCode);
		_bw.Write(this.sharedWithEntityID);
	}

	// Token: 0x0600383C RID: 14396 RVA: 0x0016EC74 File Offset: 0x0016CE74
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		switch (this.questEvent)
		{
		case NetPackageSharedQuest.SharedQuestEvents.ShareQuest:
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				GameManager.Instance.QuestShareServer(this.questCode, this.questID, this.poiName, this.position, this.size, this.returnPos, this.sharedByEntityID, this.sharedWithEntityID, this.questGiverID);
				return;
			}
			GameManager.Instance.QuestShareClient(this.questCode, this.questID, this.poiName, this.position, this.size, this.returnPos, this.sharedByEntityID, this.questGiverID, null);
			return;
		case NetPackageSharedQuest.SharedQuestEvents.RemoveQuest:
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(this.sharedByEntityID) as EntityPlayer;
				if (entityPlayer != null && entityPlayer.Party != null)
				{
					for (int i = 0; i < entityPlayer.Party.MemberList.Count; i++)
					{
						EntityPlayer entityPlayer2 = entityPlayer.Party.MemberList[i];
						if (entityPlayer2 != entityPlayer)
						{
							if (entityPlayer2 is EntityPlayerLocal)
							{
								entityPlayer2.QuestJournal.RemoveSharedQuestByOwner(this.questCode);
								entityPlayer2.QuestJournal.RemoveSharedQuestEntry(this.questCode);
							}
							else
							{
								SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(this.questCode, this.sharedByEntityID), false, entityPlayer2.entityId, -1, -1, null, 192, false);
							}
						}
					}
					return;
				}
			}
			else
			{
				List<EntityPlayerLocal> localPlayers = GameManager.Instance.World.GetLocalPlayers();
				if (localPlayers != null && localPlayers.Count > 0)
				{
					EntityPlayerLocal entityPlayerLocal = localPlayers[0];
					entityPlayerLocal.QuestJournal.RemoveSharedQuestByOwner(this.questCode);
					entityPlayerLocal.QuestJournal.RemoveSharedQuestEntry(this.questCode);
					return;
				}
			}
			break;
		case NetPackageSharedQuest.SharedQuestEvents.AddSharedMember:
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				EntityPlayer entityPlayer3 = GameManager.Instance.World.GetEntity(this.sharedByEntityID) as EntityPlayer;
				if (entityPlayer3 != null && entityPlayer3.Party != null)
				{
					EntityPlayerLocal entityPlayerLocal2 = entityPlayer3 as EntityPlayerLocal;
					if (entityPlayerLocal2 == null)
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(this.questCode, this.sharedByEntityID, this.sharedWithEntityID, true), false, this.sharedByEntityID, -1, -1, null, 192, false);
						return;
					}
					EntityPlayer entityPlayer4 = GameManager.Instance.World.GetEntity(this.sharedWithEntityID) as EntityPlayer;
					if (entityPlayer4 != null)
					{
						Quest sharedQuest = entityPlayerLocal2.QuestJournal.GetSharedQuest(this.questCode);
						if (sharedQuest != null)
						{
							sharedQuest.AddSharedWith(entityPlayer4);
							GameManager.ShowTooltip(entityPlayerLocal2, string.Format(Localization.Get("ttQuestSharedAccepted", false), sharedQuest.QuestClass.Name, entityPlayer4.PlayerDisplayName), false, false, 0f);
							return;
						}
					}
				}
			}
			else
			{
				EntityPlayer entityPlayer5 = GameManager.Instance.World.GetEntity(this.sharedByEntityID) as EntityPlayer;
				EntityPlayerLocal entityPlayerLocal3 = entityPlayer5 as EntityPlayerLocal;
				if (entityPlayerLocal3 != null)
				{
					EntityPlayer entityPlayer6 = GameManager.Instance.World.GetEntity(this.sharedWithEntityID) as EntityPlayer;
					if (entityPlayer6 != null)
					{
						Quest sharedQuest2 = entityPlayer5.QuestJournal.GetSharedQuest(this.questCode);
						if (sharedQuest2 != null)
						{
							sharedQuest2.AddSharedWith(entityPlayer6);
							GameManager.ShowTooltip(entityPlayerLocal3, string.Format(Localization.Get("ttQuestSharedAccepted", false), sharedQuest2.QuestClass.Name, entityPlayer6.PlayerDisplayName), false, false, 0f);
							return;
						}
					}
				}
			}
			break;
		case NetPackageSharedQuest.SharedQuestEvents.RemoveSharedMember:
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				EntityPlayer entityPlayer7 = GameManager.Instance.World.GetEntity(this.sharedByEntityID) as EntityPlayer;
				if (entityPlayer7 != null)
				{
					EntityPlayerLocal entityPlayerLocal4 = entityPlayer7 as EntityPlayerLocal;
					if (entityPlayerLocal4 == null)
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(this.questCode, this.sharedByEntityID, this.sharedWithEntityID, false), false, this.sharedByEntityID, -1, -1, null, 192, false);
						return;
					}
					EntityPlayer entityPlayer8 = GameManager.Instance.World.GetEntity(this.sharedWithEntityID) as EntityPlayer;
					if (entityPlayer8 != null)
					{
						Quest sharedQuest3 = entityPlayerLocal4.QuestJournal.GetSharedQuest(this.questCode);
						if (sharedQuest3 != null && sharedQuest3.RemoveSharedWith(entityPlayer8))
						{
							GameManager.ShowTooltip(entityPlayerLocal4, string.Format(Localization.Get("ttQuestSharedRemoved", false), sharedQuest3.QuestClass.Name, entityPlayer8.PlayerDisplayName), false, false, 0f);
							return;
						}
					}
				}
			}
			else
			{
				EntityPlayer entityPlayer9 = GameManager.Instance.World.GetEntity(this.sharedByEntityID) as EntityPlayer;
				EntityPlayerLocal entityPlayerLocal5 = entityPlayer9 as EntityPlayerLocal;
				if (entityPlayerLocal5 != null)
				{
					EntityPlayer entityPlayer10 = GameManager.Instance.World.GetEntity(this.sharedWithEntityID) as EntityPlayer;
					if (entityPlayer10 != null)
					{
						Quest sharedQuest4 = entityPlayer9.QuestJournal.GetSharedQuest(this.questCode);
						if (sharedQuest4 != null && sharedQuest4.RemoveSharedWith(entityPlayer10))
						{
							GameManager.ShowTooltip(entityPlayerLocal5, string.Format(Localization.Get("ttQuestSharedRemoved", false), sharedQuest4.QuestClass.Name, entityPlayer10.PlayerDisplayName), false, false, 0f);
						}
					}
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600383D RID: 14397 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override int GetLength()
	{
		return 4;
	}

	// Token: 0x04002D94 RID: 11668
	[PublicizedFrom(EAccessModifier.Private)]
	public string questID;

	// Token: 0x04002D95 RID: 11669
	[PublicizedFrom(EAccessModifier.Private)]
	public string poiName;

	// Token: 0x04002D96 RID: 11670
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 position;

	// Token: 0x04002D97 RID: 11671
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 size;

	// Token: 0x04002D98 RID: 11672
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 returnPos;

	// Token: 0x04002D99 RID: 11673
	[PublicizedFrom(EAccessModifier.Private)]
	public int sharedByEntityID;

	// Token: 0x04002D9A RID: 11674
	[PublicizedFrom(EAccessModifier.Private)]
	public int sharedWithEntityID;

	// Token: 0x04002D9B RID: 11675
	[PublicizedFrom(EAccessModifier.Private)]
	public int questGiverID;

	// Token: 0x04002D9C RID: 11676
	[PublicizedFrom(EAccessModifier.Private)]
	public int questCode = -1;

	// Token: 0x04002D9D RID: 11677
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageSharedQuest.SharedQuestEvents questEvent;

	// Token: 0x02000792 RID: 1938
	public enum SharedQuestEvents
	{
		// Token: 0x04002D9F RID: 11679
		ShareQuest,
		// Token: 0x04002DA0 RID: 11680
		RemoveQuest,
		// Token: 0x04002DA1 RID: 11681
		AddSharedMember,
		// Token: 0x04002DA2 RID: 11682
		RemoveSharedMember
	}
}
