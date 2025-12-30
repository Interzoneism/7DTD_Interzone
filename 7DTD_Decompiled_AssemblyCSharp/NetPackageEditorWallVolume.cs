using System;
using UnityEngine.Scripting;

// Token: 0x02000729 RID: 1833
[Preserve]
public class NetPackageEditorWallVolume : NetPackage
{
	// Token: 0x060035B2 RID: 13746 RVA: 0x00164CA6 File Offset: 0x00162EA6
	public NetPackageEditorWallVolume Setup(NetPackageEditorSleeperVolume.EChangeType _changeType, int _prefabInstanceId, int _volumeId, Prefab.PrefabWallVolume _volume)
	{
		this.changeType = _changeType;
		this.prefabInstanceId = _prefabInstanceId;
		this.volumeId = _volumeId;
		this.startPos = _volume.startPos;
		this.size = _volume.size;
		return this;
	}

	// Token: 0x060035B3 RID: 13747 RVA: 0x00164CD8 File Offset: 0x00162ED8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		Prefab.PrefabWallVolume volumeSettings = new Prefab.PrefabWallVolume
		{
			startPos = this.startPos,
			size = this.size
		};
		if (!_world.IsRemote())
		{
			switch (this.changeType)
			{
			case NetPackageEditorSleeperVolume.EChangeType.Changed:
				PrefabVolumeManager.Instance.UpdateWallPropertiesServer(this.prefabInstanceId, this.volumeId, volumeSettings, false);
				return;
			case NetPackageEditorSleeperVolume.EChangeType.Removed:
				PrefabVolumeManager.Instance.UpdateWallPropertiesServer(this.prefabInstanceId, this.volumeId, volumeSettings, true);
				return;
			}
			throw new ArgumentOutOfRangeException();
		}
		NetPackageEditorSleeperVolume.EChangeType echangeType = this.changeType;
		if (echangeType <= NetPackageEditorSleeperVolume.EChangeType.Changed)
		{
			PrefabVolumeManager.Instance.AddUpdateWallPropertiesClient(this.prefabInstanceId, this.volumeId, volumeSettings, false);
			return;
		}
		if (echangeType != NetPackageEditorSleeperVolume.EChangeType.Removed)
		{
			throw new ArgumentOutOfRangeException();
		}
		PrefabVolumeManager.Instance.AddUpdateWallPropertiesClient(this.prefabInstanceId, this.volumeId, volumeSettings, true);
	}

	// Token: 0x060035B4 RID: 13748 RVA: 0x00164DAA File Offset: 0x00162FAA
	public override void read(PooledBinaryReader _br)
	{
		this.changeType = (NetPackageEditorSleeperVolume.EChangeType)_br.ReadByte();
		this.prefabInstanceId = _br.ReadInt32();
		this.volumeId = _br.ReadInt32();
		this.startPos = StreamUtils.ReadVector3i(_br);
		this.size = StreamUtils.ReadVector3i(_br);
	}

	// Token: 0x060035B5 RID: 13749 RVA: 0x00164DE8 File Offset: 0x00162FE8
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.changeType);
		_bw.Write(this.prefabInstanceId);
		_bw.Write(this.volumeId);
		StreamUtils.Write(_bw, this.startPos);
		StreamUtils.Write(_bw, this.size);
	}

	// Token: 0x060035B6 RID: 13750 RVA: 0x00164329 File Offset: 0x00162529
	public override int GetLength()
	{
		return 37;
	}

	// Token: 0x04002BC2 RID: 11202
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageEditorSleeperVolume.EChangeType changeType;

	// Token: 0x04002BC3 RID: 11203
	[PublicizedFrom(EAccessModifier.Private)]
	public int prefabInstanceId;

	// Token: 0x04002BC4 RID: 11204
	[PublicizedFrom(EAccessModifier.Private)]
	public int volumeId;

	// Token: 0x04002BC5 RID: 11205
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i startPos;

	// Token: 0x04002BC6 RID: 11206
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i size;
}
