using System;
using UnityEngine.Scripting;

// Token: 0x020007AA RID: 1962
[Preserve]
public class NetPackageWallVolume : NetPackage
{
	// Token: 0x060038C0 RID: 14528 RVA: 0x00170DBC File Offset: 0x0016EFBC
	public NetPackageWallVolume Setup(WallVolume _wallVolume)
	{
		this.wallVolume = _wallVolume;
		return this;
	}

	// Token: 0x170005B6 RID: 1462
	// (get) Token: 0x060038C1 RID: 14529 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x060038C2 RID: 14530 RVA: 0x00170DC6 File Offset: 0x0016EFC6
	public override void read(PooledBinaryReader _reader)
	{
		this.wallVolume = WallVolume.Read(_reader);
	}

	// Token: 0x060038C3 RID: 14531 RVA: 0x00170DD4 File Offset: 0x0016EFD4
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		this.wallVolume.Write(_writer);
	}

	// Token: 0x060038C4 RID: 14532 RVA: 0x00170DE9 File Offset: 0x0016EFE9
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			_world.AddWallVolume(this.wallVolume);
		}
	}

	// Token: 0x060038C5 RID: 14533 RVA: 0x00170E08 File Offset: 0x0016F008
	public override int GetLength()
	{
		return 29;
	}

	// Token: 0x04002DEE RID: 11758
	[PublicizedFrom(EAccessModifier.Private)]
	public WallVolume wallVolume;
}
