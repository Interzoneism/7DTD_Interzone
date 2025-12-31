using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000823 RID: 2083
[Preserve]
public class NetPackagePersistentPlayerPositions : NetPackage
{
	// Token: 0x1700061A RID: 1562
	// (get) Token: 0x06003BE3 RID: 15331 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x1700061B RID: 1563
	// (get) Token: 0x06003BE4 RID: 15332 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool FlushQueue
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003BE5 RID: 15333 RVA: 0x00181338 File Offset: 0x0017F538
	public NetPackagePersistentPlayerPositions Setup(PersistentPlayerList _ppl)
	{
		foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> keyValuePair in _ppl.Players)
		{
			PlatformUserIdentifierAbs platformUserIdentifierAbs;
			PersistentPlayerData persistentPlayerData;
			keyValuePair.Deconstruct(out platformUserIdentifierAbs, out persistentPlayerData);
			PlatformUserIdentifierAbs key = platformUserIdentifierAbs;
			PersistentPlayerData persistentPlayerData2 = persistentPlayerData;
			if (persistentPlayerData2.EntityId != -1)
			{
				persistentPlayerData2.UpdatePositionFromEntity();
				this.positions[key] = persistentPlayerData2.Position;
			}
		}
		return this;
	}

	// Token: 0x06003BE6 RID: 15334 RVA: 0x001813B4 File Offset: 0x0017F5B4
	public override void read(PooledBinaryReader _reader)
	{
		int num = _reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			PlatformUserIdentifierAbs key = PlatformUserIdentifierAbs.FromStream(_reader, false, false);
			Vector3i value = StreamUtils.ReadVector3i(_reader);
			this.positions[key] = value;
		}
	}

	// Token: 0x06003BE7 RID: 15335 RVA: 0x001813F4 File Offset: 0x0017F5F4
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.positions.Count);
		foreach (KeyValuePair<PlatformUserIdentifierAbs, Vector3i> keyValuePair in this.positions)
		{
			PlatformUserIdentifierAbs platformUserIdentifierAbs;
			Vector3i vector3i;
			keyValuePair.Deconstruct(out platformUserIdentifierAbs, out vector3i);
			PlatformUserIdentifierAbs instance = platformUserIdentifierAbs;
			Vector3i v = vector3i;
			instance.ToStream(_writer, false);
			StreamUtils.Write(_writer, v);
		}
	}

	// Token: 0x06003BE8 RID: 15336 RVA: 0x00181478 File Offset: 0x0017F678
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		PersistentPlayerList persistentPlayers = _callbacks.persistentPlayers;
		if (persistentPlayers == null)
		{
			return;
		}
		foreach (KeyValuePair<PlatformUserIdentifierAbs, Vector3i> keyValuePair in this.positions)
		{
			PlatformUserIdentifierAbs platformUserIdentifierAbs;
			Vector3i vector3i;
			keyValuePair.Deconstruct(out platformUserIdentifierAbs, out vector3i);
			PlatformUserIdentifierAbs userIdentifier = platformUserIdentifierAbs;
			Vector3i position = vector3i;
			PersistentPlayerData playerData = persistentPlayers.GetPlayerData(userIdentifier);
			if (playerData != null)
			{
				playerData.Position = position;
			}
		}
		XUiC_SpawnNearFriendsList.Instance.UpdatePlayers();
	}

	// Token: 0x06003BE9 RID: 15337 RVA: 0x00181500 File Offset: 0x0017F700
	public override int GetLength()
	{
		return this.positions.Count * 44;
	}

	// Token: 0x0400308D RID: 12429
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<PlatformUserIdentifierAbs, Vector3i> positions = new Dictionary<PlatformUserIdentifierAbs, Vector3i>();
}
