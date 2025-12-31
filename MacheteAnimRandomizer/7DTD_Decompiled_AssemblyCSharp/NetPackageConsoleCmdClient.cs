using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000712 RID: 1810
[Preserve]
public class NetPackageConsoleCmdClient : NetPackage
{
	// Token: 0x0600353B RID: 13627 RVA: 0x00162DC9 File Offset: 0x00160FC9
	public NetPackageConsoleCmdClient Setup(List<string> _lines, bool _bExecute)
	{
		this.lines = _lines;
		this.bExecute = _bExecute;
		return this;
	}

	// Token: 0x0600353C RID: 13628 RVA: 0x00162DDA File Offset: 0x00160FDA
	public NetPackageConsoleCmdClient Setup(string _line, bool _bExecute)
	{
		this.lines = new List<string>
		{
			_line
		};
		this.bExecute = _bExecute;
		return this;
	}

	// Token: 0x0600353D RID: 13629 RVA: 0x00162DF8 File Offset: 0x00160FF8
	public override void read(PooledBinaryReader _br)
	{
		int num = _br.ReadInt32();
		this.lines = new List<string>(num);
		for (int i = 0; i < num; i++)
		{
			this.lines.Add(_br.ReadString());
		}
		this.bExecute = _br.ReadBoolean();
	}

	// Token: 0x0600353E RID: 13630 RVA: 0x00162E44 File Offset: 0x00161044
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.lines.Count);
		for (int i = 0; i < this.lines.Count; i++)
		{
			_bw.Write(this.lines[i]);
		}
		_bw.Write(this.bExecute);
	}

	// Token: 0x0600353F RID: 13631 RVA: 0x00162EA0 File Offset: 0x001610A0
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (this.bExecute)
		{
			GameManager.Instance.m_GUIConsole.AddLines(SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync(this.lines[0], null));
			return;
		}
		GameManager.Instance.m_GUIConsole.AddLines(this.lines);
	}

	// Token: 0x17000565 RID: 1381
	// (get) Token: 0x06003540 RID: 13632 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003541 RID: 13633 RVA: 0x00162999 File Offset: 0x00160B99
	public override int GetLength()
	{
		return 40;
	}

	// Token: 0x04002B59 RID: 11097
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> lines;

	// Token: 0x04002B5A RID: 11098
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bExecute;
}
