using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A48 RID: 2632
public class SpawnPointList : List<SpawnPoint>
{
	// Token: 0x06005074 RID: 20596 RVA: 0x001FF5A0 File Offset: 0x001FD7A0
	public SpawnPoint Find(Vector3i _blockPos)
	{
		for (int i = 0; i < base.Count; i++)
		{
			SpawnPoint spawnPoint = base[i];
			if (spawnPoint.spawnPosition.ToBlockPos().Equals(_blockPos))
			{
				return spawnPoint;
			}
		}
		return null;
	}

	// Token: 0x06005075 RID: 20597 RVA: 0x001FF5E0 File Offset: 0x001FD7E0
	public virtual SpawnPosition GetRandomSpawnPosition(World _world, Vector3? _refPosition = null, int _minDistance = 0, int _maxDistance = 0)
	{
		if (base.Count > 0)
		{
			GameRandom gameRandom = _world.GetGameRandom();
			if (_refPosition == null)
			{
				return base[gameRandom.RandomRange(base.Count)].spawnPosition;
			}
			Vector3 value = _refPosition.Value;
			for (int i = 0; i < 100; i++)
			{
				int index = gameRandom.RandomRange(base.Count);
				SpawnPosition spawnPosition = base[index].spawnPosition;
				float magnitude = (spawnPosition.position - value).magnitude;
				if (magnitude >= (float)_minDistance && magnitude <= (float)_maxDistance)
				{
					return spawnPosition;
				}
			}
			float num = float.MaxValue;
			int num2 = -1;
			for (int j = 0; j < base.Count; j++)
			{
				float sqrMagnitude = (base[j].spawnPosition.position - value).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					num2 = j;
				}
			}
			if (num2 != -1)
			{
				return base[num2].spawnPosition;
			}
		}
		return SpawnPosition.Undef;
	}

	// Token: 0x06005076 RID: 20598 RVA: 0x001FF6E0 File Offset: 0x001FD8E0
	public void Read(IBinaryReaderOrWriter _readerOrWriter)
	{
		if (_readerOrWriter == null)
		{
			return;
		}
		base.Clear();
		uint version = (uint)_readerOrWriter.ReadWrite(0);
		int num = _readerOrWriter.ReadWrite(0);
		for (int i = 0; i < num; i++)
		{
			SpawnPoint spawnPoint = new SpawnPoint();
			spawnPoint.Read(_readerOrWriter, version);
			base.Add(spawnPoint);
		}
	}

	// Token: 0x06005077 RID: 20599 RVA: 0x001FF728 File Offset: 0x001FD928
	public void Read(PooledBinaryReader _br)
	{
		if (_br == null)
		{
			return;
		}
		base.Clear();
		uint version = (uint)_br.ReadByte();
		int num = _br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			SpawnPoint spawnPoint = new SpawnPoint();
			spawnPoint.Read(_br, version);
			base.Add(spawnPoint);
		}
	}

	// Token: 0x06005078 RID: 20600 RVA: 0x001FF770 File Offset: 0x001FD970
	public void Write(PooledBinaryWriter _bw)
	{
		_bw.Write((byte)SpawnPointList.CurrentSaveVersion);
		_bw.Write(base.Count);
		for (int i = 0; i < base.Count; i++)
		{
			base[i].Write(_bw);
		}
	}

	// Token: 0x04003D9D RID: 15773
	[PublicizedFrom(EAccessModifier.Private)]
	public static uint CurrentSaveVersion = 2U;
}
