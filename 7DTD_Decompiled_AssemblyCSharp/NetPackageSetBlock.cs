using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200078C RID: 1932
[Preserve]
public class NetPackageSetBlock : NetPackage
{
	// Token: 0x0600381B RID: 14363 RVA: 0x0016E5CE File Offset: 0x0016C7CE
	public NetPackageSetBlock Setup(PersistentPlayerData _persistentPlayerData, List<BlockChangeInfo> _blockChanges, int _localPlayerThatChanged)
	{
		this.persistentPlayerId = ((_persistentPlayerData != null) ? _persistentPlayerData.PrimaryId : null);
		this.blockChanges = _blockChanges;
		this.localPlayerThatChanged = _localPlayerThatChanged;
		return this;
	}

	// Token: 0x0600381C RID: 14364 RVA: 0x0016E5F4 File Offset: 0x0016C7F4
	public override void read(PooledBinaryReader _br)
	{
		this.persistentPlayerId = PlatformUserIdentifierAbs.FromStream(_br, false, false);
		int num = (int)_br.ReadInt16();
		this.blockChanges = new List<BlockChangeInfo>();
		for (int i = 0; i < num; i++)
		{
			BlockChangeInfo blockChangeInfo = new BlockChangeInfo();
			blockChangeInfo.Read(_br);
			this.blockChanges.Add(blockChangeInfo);
		}
		this.localPlayerThatChanged = _br.ReadInt32();
	}

	// Token: 0x0600381D RID: 14365 RVA: 0x0016E654 File Offset: 0x0016C854
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		this.persistentPlayerId.ToStream(_bw, false);
		int count = this.blockChanges.Count;
		_bw.Write((short)count);
		for (int i = 0; i < count; i++)
		{
			this.blockChanges[i].Write(_bw);
		}
		_bw.Write(this.localPlayerThatChanged);
	}

	// Token: 0x0600381E RID: 14366 RVA: 0x0016E6B4 File Offset: 0x0016C8B4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (!base.ValidUserIdForSender(this.persistentPlayerId))
		{
			return;
		}
		if (!base.ValidEntityIdForSender(this.localPlayerThatChanged, false))
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			_callbacks.SetBlocksOnClients(this.localPlayerThatChanged, this);
		}
		if (_world == null || _world.ChunkClusters[0] == null)
		{
			return;
		}
		if (DynamicMeshManager.CONTENT_ENABLED)
		{
			foreach (BlockChangeInfo blockChangeInfo in this.blockChanges)
			{
				DynamicMeshManager.ChunkChanged(blockChangeInfo.pos, -1, blockChangeInfo.blockValue.type);
			}
		}
		_callbacks.ChangeBlocks(this.persistentPlayerId, this.blockChanges);
	}

	// Token: 0x0600381F RID: 14367 RVA: 0x0016E77C File Offset: 0x0016C97C
	public override int GetLength()
	{
		return this.blockChanges.Count * 16;
	}

	// Token: 0x04002D83 RID: 11651
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BlockChangeInfo> blockChanges;

	// Token: 0x04002D84 RID: 11652
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs persistentPlayerId;

	// Token: 0x04002D85 RID: 11653
	[PublicizedFrom(EAccessModifier.Private)]
	public int localPlayerThatChanged;
}
