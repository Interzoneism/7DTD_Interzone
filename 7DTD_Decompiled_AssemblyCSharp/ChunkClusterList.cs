using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// Token: 0x020009B3 RID: 2483
public class ChunkClusterList
{
	// Token: 0x06004BEF RID: 19439 RVA: 0x001E0268 File Offset: 0x001DE468
	public ChunkClusterList()
	{
		this.AddLayerMappingTable(0, new Dictionary<string, int>
		{
			{
				"terraincollision",
				16
			},
			{
				"nocollision",
				14
			},
			{
				"grass",
				18
			},
			{
				"Glass",
				30
			},
			{
				"water",
				4
			},
			{
				"terrain",
				28
			}
		});
	}

	// Token: 0x06004BF0 RID: 19440 RVA: 0x001E02DF File Offset: 0x001DE4DF
	public void AddFixed(ChunkCluster _cc, int _index)
	{
		this.Cluster0 = _cc;
	}

	// Token: 0x06004BF1 RID: 19441 RVA: 0x001E02E8 File Offset: 0x001DE4E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddLayerMappingTable(int _id, Dictionary<string, int> _table)
	{
		for (int i = this.LayerMappingTable.Count - 1; i < _id; i++)
		{
			this.LayerMappingTable.Add(null);
		}
		this.LayerMappingTable[_id] = _table;
	}

	// Token: 0x170007D9 RID: 2009
	public ChunkCluster this[int _idx]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.Cluster0;
		}
	}

	// Token: 0x170007DA RID: 2010
	// (get) Token: 0x06004BF3 RID: 19443 RVA: 0x001E032E File Offset: 0x001DE52E
	public int Count
	{
		get
		{
			if (this.Cluster0 == null)
			{
				return 0;
			}
			return 1;
		}
	}

	// Token: 0x06004BF4 RID: 19444 RVA: 0x001E033B File Offset: 0x001DE53B
	public void Cleanup()
	{
		if (this.Cluster0 != null)
		{
			this.Cluster0.Cleanup();
			this.Cluster0 = null;
		}
	}

	// Token: 0x040039EF RID: 14831
	public ChunkCluster Cluster0;

	// Token: 0x040039F0 RID: 14832
	public List<Dictionary<string, int>> LayerMappingTable = new List<Dictionary<string, int>>();
}
