using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200076C RID: 1900
[Preserve]
public class NetPackagePartyQuestChange : NetPackage
{
	// Token: 0x06003746 RID: 14150 RVA: 0x0016A676 File Offset: 0x00168876
	public NetPackagePartyQuestChange Setup(int _senderEntityID, byte _objectiveIndex, bool _isComplete, int _questCode)
	{
		this.senderEntityID = _senderEntityID;
		this.objectiveIndex = _objectiveIndex;
		this.isComplete = _isComplete;
		this.questCode = _questCode;
		return this;
	}

	// Token: 0x06003747 RID: 14151 RVA: 0x0016A696 File Offset: 0x00168896
	public override void read(PooledBinaryReader _br)
	{
		this.senderEntityID = _br.ReadInt32();
		this.objectiveIndex = _br.ReadByte();
		this.isComplete = _br.ReadBoolean();
		this.questCode = _br.ReadInt32();
	}

	// Token: 0x06003748 RID: 14152 RVA: 0x0016A6C8 File Offset: 0x001688C8
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.senderEntityID);
		_bw.Write(this.objectiveIndex);
		_bw.Write(this.isComplete);
		_bw.Write(this.questCode);
	}

	// Token: 0x06003749 RID: 14153 RVA: 0x0016A704 File Offset: 0x00168904
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
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
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyQuestChange>().Setup(this.senderEntityID, this.objectiveIndex, this.isComplete, this.questCode), false, entityPlayer2.entityId, -1, -1, null, 192, false);
				}
			}
		}
	}

	// Token: 0x0600374A RID: 14154 RVA: 0x0016A7F0 File Offset: 0x001689F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandlePlayer(World _world, EntityPlayerLocal localPlayer)
	{
		if (localPlayer == null)
		{
			Log.Warning("HandlePlayer NetPackagePartyQuestChange with no active local player");
			return;
		}
		EntityPlayer entityPlayer = _world.GetEntity(this.senderEntityID) as EntityPlayer;
		Quest sharedQuest = localPlayer.QuestJournal.GetSharedQuest(this.questCode);
		if (sharedQuest != null)
		{
			Rect locationRect = sharedQuest.GetLocationRect();
			bool flag;
			if (locationRect != Rect.zero)
			{
				Vector3 position = localPlayer.position;
				position.y = position.z;
				flag = locationRect.Contains(position);
			}
			else
			{
				flag = (entityPlayer.GetDistance(localPlayer) < 15f);
			}
			if (flag)
			{
				sharedQuest.Objectives[(int)this.objectiveIndex].ChangeStatus(this.isComplete);
			}
			else
			{
				localPlayer.QuestJournal.RemoveSharedQuestByOwner(this.questCode);
			}
		}
		localPlayer.QuestJournal.RemoveSharedQuestEntry(this.questCode);
	}

	// Token: 0x0600374B RID: 14155 RVA: 0x0011934C File Offset: 0x0011754C
	public override int GetLength()
	{
		return 9;
	}

	// Token: 0x04002CDF RID: 11487
	[PublicizedFrom(EAccessModifier.Private)]
	public int senderEntityID;

	// Token: 0x04002CE0 RID: 11488
	[PublicizedFrom(EAccessModifier.Private)]
	public byte objectiveIndex;

	// Token: 0x04002CE1 RID: 11489
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isComplete;

	// Token: 0x04002CE2 RID: 11490
	[PublicizedFrom(EAccessModifier.Private)]
	public int questCode;
}
