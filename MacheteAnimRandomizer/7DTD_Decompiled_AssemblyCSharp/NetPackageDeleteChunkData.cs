using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200071A RID: 1818
[Preserve]
public class NetPackageDeleteChunkData : NetPackage
{
	// Token: 0x0600356F RID: 13679 RVA: 0x00163D6E File Offset: 0x00161F6E
	public NetPackageDeleteChunkData Setup(ICollection<long> _chunkKeys)
	{
		this.chunkKeys.Clear();
		this.chunkKeys.AddRange(_chunkKeys);
		this.length = 4 + 8 * this.chunkKeys.Count;
		return this;
	}

	// Token: 0x06003570 RID: 13680 RVA: 0x00163DA0 File Offset: 0x00161FA0
	public override void read(PooledBinaryReader _br)
	{
		int num = _br.ReadInt32();
		this.chunkKeys = new List<long>();
		for (int i = 0; i < num; i++)
		{
			this.chunkKeys.Add(_br.ReadInt64());
		}
	}

	// Token: 0x06003571 RID: 13681 RVA: 0x00163DDC File Offset: 0x00161FDC
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.chunkKeys.Count);
		for (int i = 0; i < this.chunkKeys.Count; i++)
		{
			_bw.Write(this.chunkKeys[i]);
		}
	}

	// Token: 0x06003572 RID: 13682 RVA: 0x00163E29 File Offset: 0x00162029
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		DynamicMeshUnity.DeleteDynamicMeshData(this.chunkKeys);
		WaterSimulationNative.Instance.changeApplier.DiscardChangesForChunks(this.chunkKeys);
		MultiBlockManager.Instance.CullChunklessDataOnClient(this.chunkKeys);
	}

	// Token: 0x1700056B RID: 1387
	// (get) Token: 0x06003573 RID: 13683 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003574 RID: 13684 RVA: 0x00163E5F File Offset: 0x0016205F
	public override int GetLength()
	{
		return this.length;
	}

	// Token: 0x04002B8A RID: 11146
	public List<long> chunkKeys = new List<long>();

	// Token: 0x04002B8B RID: 11147
	[PublicizedFrom(EAccessModifier.Private)]
	public int length;
}
