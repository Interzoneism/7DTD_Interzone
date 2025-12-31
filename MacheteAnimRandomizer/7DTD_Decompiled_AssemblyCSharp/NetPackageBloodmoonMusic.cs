using System;
using UnityEngine.Scripting;

// Token: 0x02000705 RID: 1797
[Preserve]
public class NetPackageBloodmoonMusic : NetPackage
{
	// Token: 0x17000558 RID: 1368
	// (get) Token: 0x060034EE RID: 13550 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x060034EF RID: 13551 RVA: 0x00161F11 File Offset: 0x00160111
	public NetPackageBloodmoonMusic Setup(bool _isBloodmoonMusicEligible)
	{
		this.IsBloodMoonMusicEligible = _isBloodmoonMusicEligible;
		return this;
	}

	// Token: 0x060034F0 RID: 13552 RVA: 0x00161F1B File Offset: 0x0016011B
	public override void read(PooledBinaryReader _reader)
	{
		this.IsBloodMoonMusicEligible = _reader.ReadBoolean();
	}

	// Token: 0x060034F1 RID: 13553 RVA: 0x00161F29 File Offset: 0x00160129
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.IsBloodMoonMusicEligible);
	}

	// Token: 0x060034F2 RID: 13554 RVA: 0x00161F3E File Offset: 0x0016013E
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (GameManager.Instance.World != null && GameManager.Instance.World.dmsConductor != null)
		{
			GameManager.Instance.World.dmsConductor.IsBloodmoonMusicEligible = this.IsBloodMoonMusicEligible;
		}
	}

	// Token: 0x060034F3 RID: 13555 RVA: 0x000197A5 File Offset: 0x000179A5
	public override int GetLength()
	{
		return 1;
	}

	// Token: 0x04002B26 RID: 11046
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool IsBloodMoonMusicEligible;
}
