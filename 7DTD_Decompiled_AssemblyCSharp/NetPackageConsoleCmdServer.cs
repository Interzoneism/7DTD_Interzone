using System;
using UnityEngine.Scripting;

// Token: 0x02000713 RID: 1811
[Preserve]
public class NetPackageConsoleCmdServer : NetPackage
{
	// Token: 0x06003543 RID: 13635 RVA: 0x00162EF5 File Offset: 0x001610F5
	public NetPackageConsoleCmdServer Setup(string _cmd)
	{
		this.cmd = _cmd;
		return this;
	}

	// Token: 0x06003544 RID: 13636 RVA: 0x00162EFF File Offset: 0x001610FF
	public override void read(PooledBinaryReader _br)
	{
		this.cmd = _br.ReadString();
	}

	// Token: 0x06003545 RID: 13637 RVA: 0x00162F0D File Offset: 0x0016110D
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.cmd);
	}

	// Token: 0x06003546 RID: 13638 RVA: 0x00162F22 File Offset: 0x00161122
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.ServerConsoleCommand(base.Sender, this.cmd);
	}

	// Token: 0x17000566 RID: 1382
	// (get) Token: 0x06003547 RID: 13639 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x06003548 RID: 13640 RVA: 0x000F298B File Offset: 0x000F0B8B
	public override int GetLength()
	{
		return 30;
	}

	// Token: 0x04002B5B RID: 11099
	[PublicizedFrom(EAccessModifier.Private)]
	public string cmd;
}
