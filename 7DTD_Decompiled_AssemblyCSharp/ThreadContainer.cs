using System;

// Token: 0x02000AC2 RID: 2754
public class ThreadContainer
{
	// Token: 0x060054D2 RID: 21714 RVA: 0x0022A878 File Offset: 0x00228A78
	public ThreadContainer(DistantTerrain _TerExt, DistantChunk _DChunk, DistantChunkBasicMesh _BMesh, bool _WasReset)
	{
		this.Init(_TerExt, _DChunk, _BMesh, _WasReset);
	}

	// Token: 0x060054D3 RID: 21715 RVA: 0x0022A892 File Offset: 0x00228A92
	public ThreadContainer()
	{
		this.TerExt = null;
		this.DChunk = null;
		this.BMesh = null;
		this.WasReset = false;
	}

	// Token: 0x060054D4 RID: 21716 RVA: 0x0022A8BD File Offset: 0x00228ABD
	public void Init(DistantTerrain _TerExt, DistantChunk _DChunk, DistantChunkBasicMesh _BMesh, bool _WasReset)
	{
		this.TerExt = _TerExt;
		this.DChunk = _DChunk;
		this.BMesh = _BMesh;
		this.WasReset = _WasReset;
	}

	// Token: 0x060054D5 RID: 21717 RVA: 0x0022A8DC File Offset: 0x00228ADC
	public void ThreadExtraWork()
	{
		this.TerExt.ThreadExtraWork(this.DChunk, this.BMesh, this.WasReset);
	}

	// Token: 0x060054D6 RID: 21718 RVA: 0x0022A8FB File Offset: 0x00228AFB
	public void MainExtraWork()
	{
		this.TerExt.MainExtraWork(this.DChunk, this.BMesh);
	}

	// Token: 0x060054D7 RID: 21719 RVA: 0x0022A914 File Offset: 0x00228B14
	public void Clear(bool IsClearItem)
	{
		if (IsClearItem)
		{
			this.TerExt = null;
			this.DChunk = null;
			this.BMesh = null;
			this.WasReset = false;
		}
	}

	// Token: 0x040041AE RID: 16814
	public int DEBUG_TCId = -1;

	// Token: 0x040041AF RID: 16815
	[PublicizedFrom(EAccessModifier.Private)]
	public DistantTerrain TerExt;

	// Token: 0x040041B0 RID: 16816
	public DistantChunk DChunk;

	// Token: 0x040041B1 RID: 16817
	public DistantChunkBasicMesh BMesh;

	// Token: 0x040041B2 RID: 16818
	public bool WasReset;
}
