using System;
using UnityEngine.Scripting;

// Token: 0x020007BE RID: 1982
[Preserve]
public class NetPackageWorldSpawnPoints : NetPackage
{
	// Token: 0x06003943 RID: 14659 RVA: 0x001733E0 File Offset: 0x001715E0
	public NetPackageWorldSpawnPoints Setup(SpawnPointList _spawnPoints)
	{
		this.spawnPoints = _spawnPoints;
		return this;
	}

	// Token: 0x06003944 RID: 14660 RVA: 0x001733EA File Offset: 0x001715EA
	public override void read(PooledBinaryReader _reader)
	{
		this.spawnPoints = new SpawnPointList();
		this.spawnPoints.Read(_reader);
	}

	// Token: 0x06003945 RID: 14661 RVA: 0x00173403 File Offset: 0x00171603
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		this.spawnPoints.Write(_writer);
	}

	// Token: 0x06003946 RID: 14662 RVA: 0x00173418 File Offset: 0x00171618
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		_callbacks.SetSpawnPointList(this.spawnPoints);
	}

	// Token: 0x06003947 RID: 14663 RVA: 0x00173426 File Offset: 0x00171626
	public override int GetLength()
	{
		if (this.spawnPoints == null)
		{
			return 0;
		}
		return this.spawnPoints.Count * 20;
	}

	// Token: 0x170005CD RID: 1485
	// (get) Token: 0x06003948 RID: 14664 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x04002E69 RID: 11881
	[PublicizedFrom(EAccessModifier.Protected)]
	public SpawnPointList spawnPoints;
}
