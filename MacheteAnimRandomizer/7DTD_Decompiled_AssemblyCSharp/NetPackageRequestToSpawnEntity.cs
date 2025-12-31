using System;
using UnityEngine.Scripting;

// Token: 0x02000789 RID: 1929
[Preserve]
public class NetPackageRequestToSpawnEntity : NetPackage
{
	// Token: 0x170005B0 RID: 1456
	// (get) Token: 0x06003807 RID: 14343 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x06003808 RID: 14344 RVA: 0x0016E45F File Offset: 0x0016C65F
	public NetPackageRequestToSpawnEntity Setup(EntityCreationData _es)
	{
		this.ecd = _es;
		return this;
	}

	// Token: 0x06003809 RID: 14345 RVA: 0x0016E469 File Offset: 0x0016C669
	public override void read(PooledBinaryReader _reader)
	{
		this.ecd = new EntityCreationData();
		this.ecd.read(_reader, true);
	}

	// Token: 0x0600380A RID: 14346 RVA: 0x0016E483 File Offset: 0x0016C683
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		this.ecd.write(_writer, true);
	}

	// Token: 0x0600380B RID: 14347 RVA: 0x0016E499 File Offset: 0x0016C699
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_world.GetGameManager().RequestToSpawnEntityServer(this.ecd);
	}

	// Token: 0x0600380C RID: 14348 RVA: 0x00169085 File Offset: 0x00167285
	public override int GetLength()
	{
		return 32;
	}

	// Token: 0x04002D7D RID: 11645
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityCreationData ecd;
}
