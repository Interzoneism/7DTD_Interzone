using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200074C RID: 1868
[Preserve]
public class NetPackageGameEventResponse : NetPackage
{
	// Token: 0x0600368A RID: 13962 RVA: 0x001678E7 File Offset: 0x00165AE7
	public NetPackageGameEventResponse Setup(string _event, int _targetEntityID, string _extraData, string _tag, NetPackageGameEventResponse.ResponseTypes _responseType, int _entitySpawnedID = -1, int _index = -1, bool _isDespawn = false)
	{
		this.eventName = _event;
		this.targetEntityID = _targetEntityID;
		this.extraData = _extraData;
		this.tag = _tag;
		this.responseType = _responseType;
		this.entitySpawnedID = _entitySpawnedID;
		this.index = _index;
		this.isDespawn = _isDespawn;
		return this;
	}

	// Token: 0x0600368B RID: 13963 RVA: 0x00167928 File Offset: 0x00165B28
	public NetPackageGameEventResponse Setup(NetPackageGameEventResponse.ResponseTypes _responseType, int _entitySpawnedID = -1, int _index = -1, string _tag = "", bool _isDespawn = false)
	{
		this.eventName = "";
		this.targetEntityID = -1;
		this.extraData = "";
		this.tag = _tag;
		this.responseType = _responseType;
		this.entitySpawnedID = _entitySpawnedID;
		this.index = _index;
		this.isDespawn = _isDespawn;
		return this;
	}

	// Token: 0x0600368C RID: 13964 RVA: 0x00167978 File Offset: 0x00165B78
	public NetPackageGameEventResponse Setup(NetPackageGameEventResponse.ResponseTypes _responseType, string _event, int _blockGroupID, List<Vector3i> _blockList, string _tag = "", bool _isDespawn = false)
	{
		this.eventName = _event;
		this.targetEntityID = -1;
		this.extraData = "";
		this.tag = _tag;
		this.index = _blockGroupID;
		this.responseType = _responseType;
		this.blockList = _blockList;
		this.isDespawn = _isDespawn;
		return this;
	}

	// Token: 0x0600368D RID: 13965 RVA: 0x001679C5 File Offset: 0x00165BC5
	public NetPackageGameEventResponse Setup(NetPackageGameEventResponse.ResponseTypes _responseType, Vector3i _blockPos)
	{
		this.eventName = "";
		this.targetEntityID = -1;
		this.extraData = "";
		this.tag = "";
		this.responseType = _responseType;
		this.blockPos = _blockPos;
		return this;
	}

	// Token: 0x0600368E RID: 13966 RVA: 0x00167A00 File Offset: 0x00165C00
	public override void read(PooledBinaryReader _br)
	{
		this.eventName = _br.ReadString();
		this.targetEntityID = _br.ReadInt32();
		this.extraData = _br.ReadString();
		this.tag = _br.ReadString();
		this.responseType = (NetPackageGameEventResponse.ResponseTypes)_br.ReadByte();
		this.entitySpawnedID = _br.ReadInt32();
		if (this.responseType == NetPackageGameEventResponse.ResponseTypes.ClientSequenceAction)
		{
			this.index = (int)_br.ReadByte();
			return;
		}
		if (this.responseType == NetPackageGameEventResponse.ResponseTypes.BlocksAdded)
		{
			this.index = _br.ReadInt32();
			int num = _br.ReadInt32();
			this.blockList = new List<Vector3i>();
			for (int i = 0; i < num; i++)
			{
				this.blockList.Add(StreamUtils.ReadVector3i(_br));
			}
			return;
		}
		if (this.responseType == NetPackageGameEventResponse.ResponseTypes.BlocksRemoved)
		{
			this.index = _br.ReadInt32();
			this.isDespawn = _br.ReadBoolean();
			return;
		}
		if (this.responseType == NetPackageGameEventResponse.ResponseTypes.BlocksRemoved || this.responseType == NetPackageGameEventResponse.ResponseTypes.BlockDamaged)
		{
			this.blockPos = StreamUtils.ReadVector3i(_br);
		}
	}

	// Token: 0x0600368F RID: 13967 RVA: 0x00167AF4 File Offset: 0x00165CF4
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.eventName);
		_bw.Write(this.targetEntityID);
		_bw.Write(this.extraData);
		_bw.Write(this.tag);
		_bw.Write((byte)this.responseType);
		_bw.Write(this.entitySpawnedID);
		if (this.responseType == NetPackageGameEventResponse.ResponseTypes.ClientSequenceAction)
		{
			_bw.Write((byte)this.index);
			return;
		}
		if (this.responseType == NetPackageGameEventResponse.ResponseTypes.BlocksAdded)
		{
			_bw.Write(this.index);
			if (this.blockList == null)
			{
				_bw.Write(0);
				return;
			}
			_bw.Write(this.blockList.Count);
			for (int i = 0; i < this.blockList.Count; i++)
			{
				StreamUtils.Write(_bw, this.blockList[i]);
			}
			return;
		}
		else
		{
			if (this.responseType == NetPackageGameEventResponse.ResponseTypes.BlocksRemoved)
			{
				_bw.Write(this.index);
				_bw.Write(this.isDespawn);
				return;
			}
			if (this.responseType == NetPackageGameEventResponse.ResponseTypes.BlocksRemoved || this.responseType == NetPackageGameEventResponse.ResponseTypes.BlockDamaged)
			{
				StreamUtils.Write(_bw, this.blockPos);
			}
			return;
		}
	}

	// Token: 0x06003690 RID: 13968 RVA: 0x00167C0C File Offset: 0x00165E0C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		switch (this.responseType)
		{
		case NetPackageGameEventResponse.ResponseTypes.Denied:
			GameEventManager.Current.HandleGameEventDenied(this.eventName, this.targetEntityID, this.extraData, this.tag);
			return;
		case NetPackageGameEventResponse.ResponseTypes.Approved:
			GameEventManager.Current.HandleGameEventApproved(this.eventName, this.targetEntityID, this.extraData, this.tag);
			return;
		case NetPackageGameEventResponse.ResponseTypes.TwitchPartyActionApproved:
			GameEventManager.Current.HandleTwitchPartyGameEventApproved(this.eventName, this.targetEntityID, this.extraData, this.tag);
			return;
		case NetPackageGameEventResponse.ResponseTypes.TwitchRefundNeeded:
			GameEventManager.Current.HandleTwitchRefundNeeded(this.eventName, this.targetEntityID, this.extraData, this.tag);
			return;
		case NetPackageGameEventResponse.ResponseTypes.TwitchSetOwner:
			GameEventManager.Current.HandleGameEntitySpawned(this.eventName, this.entitySpawnedID, this.tag);
			GameEventManager.Current.HandleTwitchSetOwner(this.targetEntityID, this.entitySpawnedID, this.extraData);
			return;
		case NetPackageGameEventResponse.ResponseTypes.EntitySpawned:
			GameEventManager.Current.HandleGameEntitySpawned(this.eventName, this.entitySpawnedID, this.tag);
			return;
		case NetPackageGameEventResponse.ResponseTypes.EntityDespawned:
			GameEventManager.Current.HandleGameEntityDespawned(this.entitySpawnedID);
			return;
		case NetPackageGameEventResponse.ResponseTypes.EntityKilled:
			GameEventManager.Current.HandleGameEntityKilled(this.entitySpawnedID);
			return;
		case NetPackageGameEventResponse.ResponseTypes.BlocksAdded:
			GameEventManager.Current.HandleGameBlocksAdded(this.eventName, this.index, this.blockList, this.tag);
			return;
		case NetPackageGameEventResponse.ResponseTypes.BlocksRemoved:
			GameEventManager.Current.HandleGameBlocksRemoved(this.index, this.isDespawn);
			return;
		case NetPackageGameEventResponse.ResponseTypes.BlockRemoved:
			GameEventManager.Current.HandleGameBlockRemoved(this.blockPos);
			return;
		case NetPackageGameEventResponse.ResponseTypes.BlockDamaged:
			GameEventManager.Current.SendBlockDamageUpdate(this.blockPos);
			return;
		case NetPackageGameEventResponse.ResponseTypes.ClientSequenceAction:
			GameEventManager.Current.HandleGameEventSequenceItemForClient(this.eventName, this.index);
			return;
		case NetPackageGameEventResponse.ResponseTypes.Completed:
			GameEventManager.Current.HandleGameEventCompleted(this.eventName, this.targetEntityID, this.extraData, this.tag);
			return;
		default:
			return;
		}
	}

	// Token: 0x17000586 RID: 1414
	// (get) Token: 0x06003691 RID: 13969 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.Both;
		}
	}

	// Token: 0x06003692 RID: 13970 RVA: 0x000F298B File Offset: 0x000F0B8B
	public override int GetLength()
	{
		return 30;
	}

	// Token: 0x04002C44 RID: 11332
	[PublicizedFrom(EAccessModifier.Private)]
	public string eventName;

	// Token: 0x04002C45 RID: 11333
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageGameEventResponse.ResponseTypes responseType;

	// Token: 0x04002C46 RID: 11334
	[PublicizedFrom(EAccessModifier.Private)]
	public int entitySpawnedID = -1;

	// Token: 0x04002C47 RID: 11335
	[PublicizedFrom(EAccessModifier.Private)]
	public int targetEntityID = -1;

	// Token: 0x04002C48 RID: 11336
	[PublicizedFrom(EAccessModifier.Private)]
	public string extraData;

	// Token: 0x04002C49 RID: 11337
	[PublicizedFrom(EAccessModifier.Private)]
	public string tag = "";

	// Token: 0x04002C4A RID: 11338
	[PublicizedFrom(EAccessModifier.Private)]
	public int index = -1;

	// Token: 0x04002C4B RID: 11339
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDespawn;

	// Token: 0x04002C4C RID: 11340
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3i> blockList;

	// Token: 0x04002C4D RID: 11341
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockPos;

	// Token: 0x0200074D RID: 1869
	public enum ResponseTypes
	{
		// Token: 0x04002C4F RID: 11343
		Denied,
		// Token: 0x04002C50 RID: 11344
		Approved,
		// Token: 0x04002C51 RID: 11345
		TwitchPartyActionApproved,
		// Token: 0x04002C52 RID: 11346
		TwitchRefundNeeded,
		// Token: 0x04002C53 RID: 11347
		TwitchSetOwner,
		// Token: 0x04002C54 RID: 11348
		EntitySpawned,
		// Token: 0x04002C55 RID: 11349
		EntityDespawned,
		// Token: 0x04002C56 RID: 11350
		EntityKilled,
		// Token: 0x04002C57 RID: 11351
		BlocksAdded,
		// Token: 0x04002C58 RID: 11352
		BlocksRemoved,
		// Token: 0x04002C59 RID: 11353
		BlockRemoved,
		// Token: 0x04002C5A RID: 11354
		BlockDamaged,
		// Token: 0x04002C5B RID: 11355
		ClientSequenceAction,
		// Token: 0x04002C5C RID: 11356
		Completed
	}
}
