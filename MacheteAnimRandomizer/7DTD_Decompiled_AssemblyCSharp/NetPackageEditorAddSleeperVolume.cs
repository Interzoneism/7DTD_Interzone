using System;
using UnityEngine.Scripting;

// Token: 0x0200071E RID: 1822
[Preserve]
public class NetPackageEditorAddSleeperVolume : NetPackage
{
	// Token: 0x06003586 RID: 13702 RVA: 0x001640B3 File Offset: 0x001622B3
	public NetPackageEditorAddSleeperVolume Setup(Vector3i _startPos, Vector3i _size)
	{
		this.startPos = _startPos;
		this.size = _size;
		return this;
	}

	// Token: 0x1700056E RID: 1390
	// (get) Token: 0x06003587 RID: 13703 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x06003588 RID: 13704 RVA: 0x001640C4 File Offset: 0x001622C4
	public override void read(PooledBinaryReader _br)
	{
		this.startPos = StreamUtils.ReadVector3i(_br);
		this.size = StreamUtils.ReadVector3i(_br);
	}

	// Token: 0x06003589 RID: 13705 RVA: 0x001640DE File Offset: 0x001622DE
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		StreamUtils.Write(_bw, this.startPos);
		StreamUtils.Write(_bw, this.size);
	}

	// Token: 0x0600358A RID: 13706 RVA: 0x001640FF File Offset: 0x001622FF
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			PrefabSleeperVolumeManager.Instance.AddSleeperVolumeServer(this.startPos, this.size);
		}
	}

	// Token: 0x0600358B RID: 13707 RVA: 0x00164123 File Offset: 0x00162323
	public override int GetLength()
	{
		return 28;
	}

	// Token: 0x04002B91 RID: 11153
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3i startPos;

	// Token: 0x04002B92 RID: 11154
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3i size;
}
