using System;
using UnityEngine.Scripting;

// Token: 0x02000727 RID: 1831
[Preserve]
public class NetPackageEditorTeleportVolume : NetPackage
{
	// Token: 0x060035A6 RID: 13734 RVA: 0x001648E9 File Offset: 0x00162AE9
	public NetPackageEditorTeleportVolume Setup(NetPackageEditorSleeperVolume.EChangeType _changeType, int _prefabInstanceId, int _volumeId, Prefab.PrefabTeleportVolume _volume)
	{
		this.changeType = _changeType;
		this.prefabInstanceId = _prefabInstanceId;
		this.volumeId = _volumeId;
		this.startPos = _volume.startPos;
		this.size = _volume.size;
		return this;
	}

	// Token: 0x060035A7 RID: 13735 RVA: 0x0016491C File Offset: 0x00162B1C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		Prefab.PrefabTeleportVolume volumeSettings = new Prefab.PrefabTeleportVolume
		{
			startPos = this.startPos,
			size = this.size
		};
		if (!_world.IsRemote())
		{
			switch (this.changeType)
			{
			case NetPackageEditorSleeperVolume.EChangeType.Changed:
				PrefabVolumeManager.Instance.UpdateTeleportPropertiesServer(this.prefabInstanceId, this.volumeId, volumeSettings, false);
				return;
			case NetPackageEditorSleeperVolume.EChangeType.Removed:
				PrefabVolumeManager.Instance.UpdateTeleportPropertiesServer(this.prefabInstanceId, this.volumeId, volumeSettings, true);
				return;
			}
			throw new ArgumentOutOfRangeException();
		}
		NetPackageEditorSleeperVolume.EChangeType echangeType = this.changeType;
		if (echangeType <= NetPackageEditorSleeperVolume.EChangeType.Changed)
		{
			PrefabVolumeManager.Instance.AddUpdateTeleportPropertiesClient(this.prefabInstanceId, this.volumeId, volumeSettings, false);
			return;
		}
		if (echangeType != NetPackageEditorSleeperVolume.EChangeType.Removed)
		{
			throw new ArgumentOutOfRangeException();
		}
		PrefabVolumeManager.Instance.AddUpdateTeleportPropertiesClient(this.prefabInstanceId, this.volumeId, volumeSettings, true);
	}

	// Token: 0x060035A8 RID: 13736 RVA: 0x001649EE File Offset: 0x00162BEE
	public override void read(PooledBinaryReader _br)
	{
		this.changeType = (NetPackageEditorSleeperVolume.EChangeType)_br.ReadByte();
		this.prefabInstanceId = _br.ReadInt32();
		this.volumeId = _br.ReadInt32();
		this.startPos = StreamUtils.ReadVector3i(_br);
		this.size = StreamUtils.ReadVector3i(_br);
	}

	// Token: 0x060035A9 RID: 13737 RVA: 0x00164A2C File Offset: 0x00162C2C
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.changeType);
		_bw.Write(this.prefabInstanceId);
		_bw.Write(this.volumeId);
		StreamUtils.Write(_bw, this.startPos);
		StreamUtils.Write(_bw, this.size);
	}

	// Token: 0x060035AA RID: 13738 RVA: 0x00164329 File Offset: 0x00162529
	public override int GetLength()
	{
		return 37;
	}

	// Token: 0x04002BB7 RID: 11191
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageEditorSleeperVolume.EChangeType changeType;

	// Token: 0x04002BB8 RID: 11192
	[PublicizedFrom(EAccessModifier.Private)]
	public int prefabInstanceId;

	// Token: 0x04002BB9 RID: 11193
	[PublicizedFrom(EAccessModifier.Private)]
	public int volumeId;

	// Token: 0x04002BBA RID: 11194
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i startPos;

	// Token: 0x04002BBB RID: 11195
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i size;
}
