using System;
using UnityEngine.Scripting;

// Token: 0x02000702 RID: 1794
[Preserve]
public class NetPackageBiomeIntensity : NetPackage
{
	// Token: 0x060034DB RID: 13531 RVA: 0x00161D16 File Offset: 0x0015FF16
	public NetPackageBiomeIntensity Setup(BiomeIntensity _bi)
	{
		this.bi = _bi;
		return this;
	}

	// Token: 0x060034DC RID: 13532 RVA: 0x00161D20 File Offset: 0x0015FF20
	public override void read(PooledBinaryReader _reader)
	{
		this.bi = BiomeIntensity.Default;
		this.bi.Read(_reader);
	}

	// Token: 0x060034DD RID: 13533 RVA: 0x00161D39 File Offset: 0x0015FF39
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		this.bi.Write(_writer);
	}

	// Token: 0x060034DE RID: 13534 RVA: 0x00161D4E File Offset: 0x0015FF4E
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_world.LocalPlayerBiomeIntensityStandingOn = this.bi;
	}

	// Token: 0x060034DF RID: 13535 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002B21 RID: 11041
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeIntensity bi;
}
