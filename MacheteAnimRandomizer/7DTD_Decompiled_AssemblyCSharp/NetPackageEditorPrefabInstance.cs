using System;
using UnityEngine.Scripting;

// Token: 0x02000723 RID: 1827
[Preserve]
public class NetPackageEditorPrefabInstance : NetPackage
{
	// Token: 0x06003599 RID: 13721 RVA: 0x00164330 File Offset: 0x00162530
	public NetPackageEditorPrefabInstance Setup(NetPackageEditorPrefabInstance.EChangeType _changeType, PrefabInstance _prefabInstance)
	{
		this.changeType = _changeType;
		this.id = _prefabInstance.id;
		this.boundingBoxPosition = _prefabInstance.boundingBoxPosition;
		this.boundingBoxSize = _prefabInstance.boundingBoxSize;
		this.name = _prefabInstance.name;
		this.size = _prefabInstance.prefab.size;
		this.filename = _prefabInstance.prefab.PrefabName;
		this.localRotation = _prefabInstance.prefab.GetLocalRotation();
		this.yOffset = _prefabInstance.prefab.yOffset;
		return this;
	}

	// Token: 0x1700056F RID: 1391
	// (get) Token: 0x0600359A RID: 13722 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x0600359B RID: 13723 RVA: 0x001643BC File Offset: 0x001625BC
	public override void read(PooledBinaryReader _br)
	{
		this.changeType = (NetPackageEditorPrefabInstance.EChangeType)_br.ReadByte();
		this.id = _br.ReadInt32();
		this.boundingBoxPosition = StreamUtils.ReadVector3i(_br);
		this.boundingBoxSize = StreamUtils.ReadVector3i(_br);
		this.name = _br.ReadString();
		this.size = StreamUtils.ReadVector3i(_br);
		this.filename = _br.ReadString();
		this.localRotation = _br.ReadInt32();
		this.yOffset = _br.ReadInt32();
	}

	// Token: 0x0600359C RID: 13724 RVA: 0x00164438 File Offset: 0x00162638
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.changeType);
		_bw.Write(this.id);
		StreamUtils.Write(_bw, this.boundingBoxPosition);
		StreamUtils.Write(_bw, this.boundingBoxSize);
		_bw.Write(this.name);
		StreamUtils.Write(_bw, this.size);
		_bw.Write(this.filename);
		_bw.Write(this.localRotation);
		_bw.Write(this.yOffset);
	}

	// Token: 0x0600359D RID: 13725 RVA: 0x001644BC File Offset: 0x001626BC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			return;
		}
		switch (this.changeType)
		{
		case NetPackageEditorPrefabInstance.EChangeType.Added:
			PrefabSleeperVolumeManager.Instance.PrefabLoadedClient(this.id, this.boundingBoxPosition, this.boundingBoxSize, this.name, this.size, this.filename, this.localRotation, this.yOffset);
			return;
		case NetPackageEditorPrefabInstance.EChangeType.Changed:
			PrefabSleeperVolumeManager.Instance.PrefabChangedClient(this.id, this.boundingBoxPosition, this.boundingBoxSize, this.name, this.size, this.filename, this.localRotation, this.yOffset);
			return;
		case NetPackageEditorPrefabInstance.EChangeType.Removed:
			PrefabSleeperVolumeManager.Instance.PrefabRemovedClient(this.id);
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600359E RID: 13726 RVA: 0x00164583 File Offset: 0x00162783
	public override int GetLength()
	{
		return 33 + this.name.Length + 12 + this.filename.Length + 4;
	}

	// Token: 0x04002B98 RID: 11160
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageEditorPrefabInstance.EChangeType changeType;

	// Token: 0x04002B99 RID: 11161
	[PublicizedFrom(EAccessModifier.Private)]
	public int id;

	// Token: 0x04002B9A RID: 11162
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i boundingBoxPosition;

	// Token: 0x04002B9B RID: 11163
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i boundingBoxSize;

	// Token: 0x04002B9C RID: 11164
	[PublicizedFrom(EAccessModifier.Private)]
	public string name;

	// Token: 0x04002B9D RID: 11165
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i size;

	// Token: 0x04002B9E RID: 11166
	[PublicizedFrom(EAccessModifier.Private)]
	public string filename;

	// Token: 0x04002B9F RID: 11167
	[PublicizedFrom(EAccessModifier.Private)]
	public int localRotation;

	// Token: 0x04002BA0 RID: 11168
	[PublicizedFrom(EAccessModifier.Private)]
	public int yOffset;

	// Token: 0x02000724 RID: 1828
	public enum EChangeType
	{
		// Token: 0x04002BA2 RID: 11170
		Added,
		// Token: 0x04002BA3 RID: 11171
		Changed,
		// Token: 0x04002BA4 RID: 11172
		Removed
	}
}
