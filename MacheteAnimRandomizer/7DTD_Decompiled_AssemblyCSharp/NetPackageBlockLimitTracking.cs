using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000703 RID: 1795
[Preserve]
public class NetPackageBlockLimitTracking : NetPackage
{
	// Token: 0x060034E1 RID: 13537 RVA: 0x00161D60 File Offset: 0x0015FF60
	public NetPackageBlockLimitTracking()
	{
		this.amounts = new List<int>();
	}

	// Token: 0x060034E2 RID: 13538 RVA: 0x00161D73 File Offset: 0x0015FF73
	public NetPackageBlockLimitTracking Setup(List<int> _amounts)
	{
		this.amounts = _amounts;
		return this;
	}

	// Token: 0x060034E3 RID: 13539 RVA: 0x00161D80 File Offset: 0x0015FF80
	public override void read(PooledBinaryReader _br)
	{
		this.amounts.Clear();
		int num = _br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			this.amounts.Add(_br.ReadInt32());
		}
	}

	// Token: 0x060034E4 RID: 13540 RVA: 0x00161DBC File Offset: 0x0015FFBC
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.amounts.Count);
		for (int i = 0; i < this.amounts.Count; i++)
		{
			_bw.Write(this.amounts[i]);
		}
	}

	// Token: 0x060034E5 RID: 13541 RVA: 0x00161E09 File Offset: 0x00160009
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Log.Warning("Server should not receive a NetPackageBlockLimitTracking");
			return;
		}
		BlockLimitTracker.instance.UpdateClientAmounts(this.amounts);
	}

	// Token: 0x060034E6 RID: 13542 RVA: 0x00161E32 File Offset: 0x00160032
	public override int GetLength()
	{
		return 4 + this.amounts.Count * 4;
	}

	// Token: 0x04002B22 RID: 11042
	public List<int> amounts;
}
