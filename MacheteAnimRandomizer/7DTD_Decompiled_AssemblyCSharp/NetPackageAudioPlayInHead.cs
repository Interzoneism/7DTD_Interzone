using System;
using Audio;
using UnityEngine.Scripting;

// Token: 0x020006FF RID: 1791
[Preserve]
public class NetPackageAudioPlayInHead : NetPackage
{
	// Token: 0x17000551 RID: 1361
	// (get) Token: 0x060034C3 RID: 13507 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x060034C4 RID: 13508 RVA: 0x00161BB5 File Offset: 0x0015FDB5
	public NetPackageAudioPlayInHead Setup(string _soundName, bool _isUnique)
	{
		this.soundName = _soundName;
		this.isUnique = _isUnique;
		return this;
	}

	// Token: 0x060034C5 RID: 13509 RVA: 0x00161BC6 File Offset: 0x0015FDC6
	public override void read(PooledBinaryReader _br)
	{
		this.soundName = _br.ReadString();
		this.isUnique = _br.ReadBoolean();
	}

	// Token: 0x060034C6 RID: 13510 RVA: 0x00161BE0 File Offset: 0x0015FDE0
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.soundName);
		_bw.Write(this.isUnique);
	}

	// Token: 0x060034C7 RID: 13511 RVA: 0x00161C01 File Offset: 0x0015FE01
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		Manager.PlayInsidePlayerHead(this.soundName, -1, 0f, false, this.isUnique);
	}

	// Token: 0x060034C8 RID: 13512 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04002B1E RID: 11038
	[PublicizedFrom(EAccessModifier.Private)]
	public string soundName;

	// Token: 0x04002B1F RID: 11039
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isUnique;
}
