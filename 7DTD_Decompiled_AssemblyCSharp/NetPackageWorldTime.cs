using System;
using UnityEngine.Scripting;

// Token: 0x020007BF RID: 1983
[Preserve]
public class NetPackageWorldTime : NetPackage
{
	// Token: 0x0600394A RID: 14666 RVA: 0x00173440 File Offset: 0x00171640
	public NetPackageWorldTime Setup(ulong _worldTime)
	{
		this.worldTime = _worldTime;
		return this;
	}

	// Token: 0x0600394B RID: 14667 RVA: 0x0017344A File Offset: 0x0017164A
	public override void read(PooledBinaryReader _br)
	{
		this.worldTime = _br.ReadUInt64();
	}

	// Token: 0x0600394C RID: 14668 RVA: 0x00173458 File Offset: 0x00171658
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.worldTime);
	}

	// Token: 0x0600394D RID: 14669 RVA: 0x0017346D File Offset: 0x0017166D
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_world.gameManager.SetWorldTime(this.worldTime);
	}

	// Token: 0x0600394E RID: 14670 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x170005CE RID: 1486
	// (get) Token: 0x0600394F RID: 14671 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x04002E6A RID: 11882
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong worldTime;
}
