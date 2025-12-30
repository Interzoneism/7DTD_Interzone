using System;
using UnityEngine.Scripting;

// Token: 0x02000722 RID: 1826
[Preserve]
public class NetPackageEditorInfoVolume : NetPackage
{
	// Token: 0x06003593 RID: 13715 RVA: 0x00164193 File Offset: 0x00162393
	public NetPackageEditorInfoVolume Setup(NetPackageEditorSleeperVolume.EChangeType _changeType, int _prefabInstanceId, int _volumeId, Prefab.PrefabInfoVolume _volume)
	{
		this.changeType = _changeType;
		this.prefabInstanceId = _prefabInstanceId;
		this.volumeId = _volumeId;
		this.startPos = _volume.startPos;
		this.size = _volume.size;
		return this;
	}

	// Token: 0x06003594 RID: 13716 RVA: 0x001641C8 File Offset: 0x001623C8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		Prefab.PrefabInfoVolume volumeSettings = new Prefab.PrefabInfoVolume
		{
			startPos = this.startPos,
			size = this.size
		};
		if (!_world.IsRemote())
		{
			switch (this.changeType)
			{
			case NetPackageEditorSleeperVolume.EChangeType.Changed:
				PrefabVolumeManager.Instance.UpdateInfoPropertiesServer(this.prefabInstanceId, this.volumeId, volumeSettings, false);
				return;
			case NetPackageEditorSleeperVolume.EChangeType.Removed:
				PrefabVolumeManager.Instance.UpdateInfoPropertiesServer(this.prefabInstanceId, this.volumeId, volumeSettings, true);
				return;
			}
			throw new ArgumentOutOfRangeException();
		}
		NetPackageEditorSleeperVolume.EChangeType echangeType = this.changeType;
		if (echangeType <= NetPackageEditorSleeperVolume.EChangeType.Changed)
		{
			PrefabVolumeManager.Instance.AddUpdateInfoPropertiesClient(this.prefabInstanceId, this.volumeId, volumeSettings, false);
			return;
		}
		if (echangeType != NetPackageEditorSleeperVolume.EChangeType.Removed)
		{
			throw new ArgumentOutOfRangeException();
		}
		PrefabVolumeManager.Instance.AddUpdateInfoPropertiesClient(this.prefabInstanceId, this.volumeId, volumeSettings, true);
	}

	// Token: 0x06003595 RID: 13717 RVA: 0x0016429A File Offset: 0x0016249A
	public override void read(PooledBinaryReader _br)
	{
		this.changeType = (NetPackageEditorSleeperVolume.EChangeType)_br.ReadByte();
		this.prefabInstanceId = _br.ReadInt32();
		this.volumeId = _br.ReadInt32();
		this.startPos = StreamUtils.ReadVector3i(_br);
		this.size = StreamUtils.ReadVector3i(_br);
	}

	// Token: 0x06003596 RID: 13718 RVA: 0x001642D8 File Offset: 0x001624D8
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.changeType);
		_bw.Write(this.prefabInstanceId);
		_bw.Write(this.volumeId);
		StreamUtils.Write(_bw, this.startPos);
		StreamUtils.Write(_bw, this.size);
	}

	// Token: 0x06003597 RID: 13719 RVA: 0x00164329 File Offset: 0x00162529
	public override int GetLength()
	{
		return 37;
	}

	// Token: 0x04002B93 RID: 11155
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageEditorSleeperVolume.EChangeType changeType;

	// Token: 0x04002B94 RID: 11156
	[PublicizedFrom(EAccessModifier.Private)]
	public int prefabInstanceId;

	// Token: 0x04002B95 RID: 11157
	[PublicizedFrom(EAccessModifier.Private)]
	public int volumeId;

	// Token: 0x04002B96 RID: 11158
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i startPos;

	// Token: 0x04002B97 RID: 11159
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i size;
}
