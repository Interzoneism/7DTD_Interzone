using System;
using UnityEngine.Scripting;

// Token: 0x02000725 RID: 1829
[Preserve]
public class NetPackageEditorSleeperVolume : NetPackage
{
	// Token: 0x060035A0 RID: 13728 RVA: 0x001645A4 File Offset: 0x001627A4
	public NetPackageEditorSleeperVolume Setup(NetPackageEditorSleeperVolume.EChangeType _changeType, int _prefabInstanceId, int _volumeId, Prefab.PrefabSleeperVolume _volume)
	{
		this.changeType = _changeType;
		this.prefabInstanceId = _prefabInstanceId;
		this.volumeId = _volumeId;
		this.used = _volume.used;
		this.startPos = _volume.startPos;
		this.size = _volume.size;
		this.groupName = _volume.groupName;
		this.isPriority = _volume.isPriority;
		this.isQuestExclude = _volume.isQuestExclude;
		this.spawnCountMin = _volume.spawnCountMin;
		this.spawnCountMax = _volume.spawnCountMax;
		this.groupId = _volume.groupId;
		this.flags = _volume.flags;
		this.minScript = _volume.minScript;
		return this;
	}

	// Token: 0x060035A1 RID: 13729 RVA: 0x00164658 File Offset: 0x00162858
	public override void read(PooledBinaryReader _br)
	{
		this.changeType = (NetPackageEditorSleeperVolume.EChangeType)_br.ReadByte();
		this.prefabInstanceId = _br.ReadInt32();
		this.volumeId = _br.ReadInt32();
		this.used = _br.ReadBoolean();
		this.startPos = StreamUtils.ReadVector3i(_br);
		this.size = StreamUtils.ReadVector3i(_br);
		this.groupName = _br.ReadString();
		this.isPriority = _br.ReadBoolean();
		this.isQuestExclude = _br.ReadBoolean();
		this.spawnCountMin = _br.ReadInt16();
		this.spawnCountMax = _br.ReadInt16();
		this.groupId = _br.ReadInt16();
		this.flags = _br.ReadInt32();
		this.minScript = _br.ReadString();
	}

	// Token: 0x060035A2 RID: 13730 RVA: 0x00164710 File Offset: 0x00162910
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.changeType);
		_bw.Write(this.prefabInstanceId);
		_bw.Write(this.volumeId);
		_bw.Write(this.used);
		StreamUtils.Write(_bw, this.startPos);
		StreamUtils.Write(_bw, this.size);
		_bw.Write(this.groupName);
		_bw.Write(this.isPriority);
		_bw.Write(this.isQuestExclude);
		_bw.Write(this.spawnCountMin);
		_bw.Write(this.spawnCountMax);
		_bw.Write(this.groupId);
		_bw.Write(this.flags);
		_bw.Write(this.minScript ?? "");
	}

	// Token: 0x060035A3 RID: 13731 RVA: 0x001647D8 File Offset: 0x001629D8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		Prefab.PrefabSleeperVolume volumeSettings = new Prefab.PrefabSleeperVolume
		{
			used = this.used,
			startPos = this.startPos,
			size = this.size,
			groupName = this.groupName,
			isPriority = this.isPriority,
			isQuestExclude = this.isQuestExclude,
			spawnCountMin = this.spawnCountMin,
			spawnCountMax = this.spawnCountMax,
			groupId = this.groupId,
			flags = this.flags,
			minScript = this.minScript
		};
		if (!_world.IsRemote())
		{
			NetPackageEditorSleeperVolume.EChangeType echangeType = this.changeType;
			if (echangeType != NetPackageEditorSleeperVolume.EChangeType.Added && echangeType - NetPackageEditorSleeperVolume.EChangeType.Changed <= 1)
			{
				PrefabSleeperVolumeManager.Instance.UpdateSleeperPropertiesServer(this.prefabInstanceId, this.volumeId, volumeSettings);
				return;
			}
			throw new ArgumentOutOfRangeException();
		}
		else
		{
			NetPackageEditorSleeperVolume.EChangeType echangeType = this.changeType;
			if (echangeType <= NetPackageEditorSleeperVolume.EChangeType.Removed)
			{
				PrefabSleeperVolumeManager.Instance.AddUpdateSleeperPropertiesClient(this.prefabInstanceId, this.volumeId, volumeSettings);
				return;
			}
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060035A4 RID: 13732 RVA: 0x001648D1 File Offset: 0x00162AD1
	public override int GetLength()
	{
		return 38 + this.groupName.Length + 1 + 1 + 2 + 2;
	}

	// Token: 0x04002BA5 RID: 11173
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageEditorSleeperVolume.EChangeType changeType;

	// Token: 0x04002BA6 RID: 11174
	[PublicizedFrom(EAccessModifier.Private)]
	public int prefabInstanceId;

	// Token: 0x04002BA7 RID: 11175
	[PublicizedFrom(EAccessModifier.Private)]
	public int volumeId;

	// Token: 0x04002BA8 RID: 11176
	[PublicizedFrom(EAccessModifier.Private)]
	public bool used;

	// Token: 0x04002BA9 RID: 11177
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i startPos;

	// Token: 0x04002BAA RID: 11178
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i size;

	// Token: 0x04002BAB RID: 11179
	[PublicizedFrom(EAccessModifier.Private)]
	public string groupName;

	// Token: 0x04002BAC RID: 11180
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPriority;

	// Token: 0x04002BAD RID: 11181
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isQuestExclude;

	// Token: 0x04002BAE RID: 11182
	[PublicizedFrom(EAccessModifier.Private)]
	public short spawnCountMin;

	// Token: 0x04002BAF RID: 11183
	[PublicizedFrom(EAccessModifier.Private)]
	public short spawnCountMax;

	// Token: 0x04002BB0 RID: 11184
	[PublicizedFrom(EAccessModifier.Private)]
	public short groupId;

	// Token: 0x04002BB1 RID: 11185
	[PublicizedFrom(EAccessModifier.Private)]
	public int flags;

	// Token: 0x04002BB2 RID: 11186
	[PublicizedFrom(EAccessModifier.Private)]
	public string minScript;

	// Token: 0x02000726 RID: 1830
	public enum EChangeType
	{
		// Token: 0x04002BB4 RID: 11188
		Added,
		// Token: 0x04002BB5 RID: 11189
		Changed,
		// Token: 0x04002BB6 RID: 11190
		Removed
	}
}
