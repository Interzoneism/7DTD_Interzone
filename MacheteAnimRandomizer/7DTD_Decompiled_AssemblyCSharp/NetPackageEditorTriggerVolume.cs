using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000728 RID: 1832
[Preserve]
public class NetPackageEditorTriggerVolume : NetPackage
{
	// Token: 0x060035AC RID: 13740 RVA: 0x00164A7D File Offset: 0x00162C7D
	public NetPackageEditorTriggerVolume Setup(NetPackageEditorSleeperVolume.EChangeType _changeType, int _prefabInstanceId, int _volumeId, Prefab.PrefabTriggerVolume _volume)
	{
		this.changeType = _changeType;
		this.prefabInstanceId = _prefabInstanceId;
		this.volumeId = _volumeId;
		this.startPos = _volume.startPos;
		this.size = _volume.size;
		this.triggersIndices = _volume.TriggersIndices;
		return this;
	}

	// Token: 0x060035AD RID: 13741 RVA: 0x00164ABC File Offset: 0x00162CBC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		Prefab.PrefabTriggerVolume volumeSettings = new Prefab.PrefabTriggerVolume
		{
			startPos = this.startPos,
			size = this.size
		};
		if (!_world.IsRemote())
		{
			switch (this.changeType)
			{
			case NetPackageEditorSleeperVolume.EChangeType.Changed:
				PrefabTriggerVolumeManager.Instance.UpdateTriggerPropertiesServer(this.prefabInstanceId, this.volumeId, volumeSettings, false);
				return;
			case NetPackageEditorSleeperVolume.EChangeType.Removed:
				PrefabTriggerVolumeManager.Instance.UpdateTriggerPropertiesServer(this.prefabInstanceId, this.volumeId, volumeSettings, true);
				return;
			}
			throw new ArgumentOutOfRangeException();
		}
		NetPackageEditorSleeperVolume.EChangeType echangeType = this.changeType;
		if (echangeType <= NetPackageEditorSleeperVolume.EChangeType.Changed)
		{
			PrefabTriggerVolumeManager.Instance.AddUpdateTriggerPropertiesClient(this.prefabInstanceId, this.volumeId, volumeSettings, false);
			return;
		}
		if (echangeType != NetPackageEditorSleeperVolume.EChangeType.Removed)
		{
			throw new ArgumentOutOfRangeException();
		}
		PrefabTriggerVolumeManager.Instance.AddUpdateTriggerPropertiesClient(this.prefabInstanceId, this.volumeId, volumeSettings, true);
	}

	// Token: 0x060035AE RID: 13742 RVA: 0x00164B90 File Offset: 0x00162D90
	public override void read(PooledBinaryReader _br)
	{
		this.changeType = (NetPackageEditorSleeperVolume.EChangeType)_br.ReadByte();
		this.prefabInstanceId = _br.ReadInt32();
		this.volumeId = _br.ReadInt32();
		this.startPos = StreamUtils.ReadVector3i(_br);
		this.size = StreamUtils.ReadVector3i(_br);
		int num = (int)_br.ReadByte();
		this.triggersIndices.Clear();
		for (int i = 0; i < num; i++)
		{
			this.triggersIndices.Add(_br.ReadByte());
		}
	}

	// Token: 0x060035AF RID: 13743 RVA: 0x00164C08 File Offset: 0x00162E08
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.changeType);
		_bw.Write(this.prefabInstanceId);
		_bw.Write(this.volumeId);
		StreamUtils.Write(_bw, this.startPos);
		StreamUtils.Write(_bw, this.size);
		_bw.Write((byte)this.triggersIndices.Count);
		for (int i = 0; i < this.triggersIndices.Count; i++)
		{
			_bw.Write(this.triggersIndices[i]);
		}
	}

	// Token: 0x060035B0 RID: 13744 RVA: 0x00164329 File Offset: 0x00162529
	public override int GetLength()
	{
		return 37;
	}

	// Token: 0x04002BBC RID: 11196
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageEditorSleeperVolume.EChangeType changeType;

	// Token: 0x04002BBD RID: 11197
	[PublicizedFrom(EAccessModifier.Private)]
	public int prefabInstanceId;

	// Token: 0x04002BBE RID: 11198
	[PublicizedFrom(EAccessModifier.Private)]
	public int volumeId;

	// Token: 0x04002BBF RID: 11199
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i startPos;

	// Token: 0x04002BC0 RID: 11200
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i size;

	// Token: 0x04002BC1 RID: 11201
	[PublicizedFrom(EAccessModifier.Private)]
	public List<byte> triggersIndices = new List<byte>();
}
