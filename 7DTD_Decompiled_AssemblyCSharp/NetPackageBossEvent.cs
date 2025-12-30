using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000706 RID: 1798
[Preserve]
public class NetPackageBossEvent : NetPackage
{
	// Token: 0x060034F5 RID: 13557 RVA: 0x00161F77 File Offset: 0x00160177
	public NetPackageBossEvent Setup(NetPackageBossEvent.BossEventTypes _eventType, int _bossGroupID)
	{
		this.bossGroupID = _bossGroupID;
		this.entityID = -1;
		this.minionIDs = null;
		this.eventType = _eventType;
		this.bossIcon1 = "";
		return this;
	}

	// Token: 0x060034F6 RID: 13558 RVA: 0x00161FA1 File Offset: 0x001601A1
	public NetPackageBossEvent Setup(NetPackageBossEvent.BossEventTypes _eventType, int _bossGroupID, BossGroup.BossGroupTypes _bossGroupType)
	{
		this.bossGroupID = _bossGroupID;
		this.bossGroupType = _bossGroupType;
		this.entityID = -1;
		this.minionIDs = null;
		this.eventType = _eventType;
		this.bossIcon1 = "";
		return this;
	}

	// Token: 0x060034F7 RID: 13559 RVA: 0x00161FD2 File Offset: 0x001601D2
	public NetPackageBossEvent Setup(NetPackageBossEvent.BossEventTypes _eventType, int _bossGroupID, int _entityID)
	{
		this.bossGroupID = _bossGroupID;
		this.entityID = _entityID;
		this.minionIDs = null;
		this.eventType = _eventType;
		this.bossIcon1 = "";
		return this;
	}

	// Token: 0x060034F8 RID: 13560 RVA: 0x00161FFC File Offset: 0x001601FC
	public NetPackageBossEvent Setup(NetPackageBossEvent.BossEventTypes _eventType, int _bossGroupID, BossGroup.BossGroupTypes _bossGroupType, int _bossID, List<int> _minionIDs, string _bossIcon1)
	{
		this.bossGroupID = _bossGroupID;
		this.bossGroupType = _bossGroupType;
		this.entityID = _bossID;
		this.minionIDs = _minionIDs;
		this.eventType = _eventType;
		this.bossIcon1 = _bossIcon1;
		return this;
	}

	// Token: 0x060034F9 RID: 13561 RVA: 0x0016202C File Offset: 0x0016022C
	public override void read(PooledBinaryReader _reader)
	{
		this.bossGroupID = _reader.ReadInt32();
		this.eventType = (NetPackageBossEvent.BossEventTypes)_reader.ReadByte();
		this.bossGroupType = (BossGroup.BossGroupTypes)_reader.ReadByte();
		this.entityID = _reader.ReadInt32();
		this.bossIcon1 = _reader.ReadString();
		if (this.eventType == NetPackageBossEvent.BossEventTypes.AddGroup)
		{
			int num = _reader.ReadInt32();
			this.minionIDs = new List<int>();
			for (int i = 0; i < num; i++)
			{
				this.minionIDs.Add(_reader.ReadInt32());
			}
		}
	}

	// Token: 0x060034FA RID: 13562 RVA: 0x001620B0 File Offset: 0x001602B0
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.bossGroupID);
		_writer.Write((byte)this.eventType);
		_writer.Write((byte)this.bossGroupType);
		_writer.Write(this.entityID);
		_writer.Write(this.bossIcon1);
		if (this.eventType == NetPackageBossEvent.BossEventTypes.AddGroup)
		{
			_writer.Write(this.minionIDs.Count);
			for (int i = 0; i < this.minionIDs.Count; i++)
			{
				_writer.Write(this.minionIDs[i]);
			}
		}
	}

	// Token: 0x060034FB RID: 13563 RVA: 0x00162144 File Offset: 0x00160344
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		switch (this.eventType)
		{
		case NetPackageBossEvent.BossEventTypes.RequestGroups:
			GameEventManager.Current.SendBossGroups(base.Sender.entityId);
			return;
		case NetPackageBossEvent.BossEventTypes.AddGroup:
			GameEventManager.Current.SetupClientBossGroup(this.bossGroupID, this.bossGroupType, this.entityID, this.minionIDs, this.bossIcon1);
			return;
		case NetPackageBossEvent.BossEventTypes.UpdateGroupType:
			GameEventManager.Current.UpdateBossGroupType(this.bossGroupID, this.bossGroupType);
			return;
		case NetPackageBossEvent.BossEventTypes.RemoveGroup:
			GameEventManager.Current.RemoveClientBossGroup(this.bossGroupID);
			return;
		case NetPackageBossEvent.BossEventTypes.RemoveMinion:
			GameEventManager.Current.RemoveEntityFromBossGroup(this.bossGroupID, this.entityID);
			return;
		case NetPackageBossEvent.BossEventTypes.RequestStats:
			GameEventManager.Current.RequestBossGroupStatRefresh(this.bossGroupID, base.Sender.entityId);
			return;
		default:
			return;
		}
	}

	// Token: 0x060034FC RID: 13564 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002B27 RID: 11047
	public int bossGroupID;

	// Token: 0x04002B28 RID: 11048
	public int entityID;

	// Token: 0x04002B29 RID: 11049
	public List<int> minionIDs;

	// Token: 0x04002B2A RID: 11050
	public string bossIcon1;

	// Token: 0x04002B2B RID: 11051
	public BossGroup.BossGroupTypes bossGroupType;

	// Token: 0x04002B2C RID: 11052
	public NetPackageBossEvent.BossEventTypes eventType;

	// Token: 0x02000707 RID: 1799
	public enum BossEventTypes
	{
		// Token: 0x04002B2E RID: 11054
		RequestGroups,
		// Token: 0x04002B2F RID: 11055
		AddGroup,
		// Token: 0x04002B30 RID: 11056
		UpdateGroupType,
		// Token: 0x04002B31 RID: 11057
		RemoveGroup,
		// Token: 0x04002B32 RID: 11058
		RemoveMinion,
		// Token: 0x04002B33 RID: 11059
		RequestStats
	}
}
