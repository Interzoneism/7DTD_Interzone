using System;

// Token: 0x02000ABE RID: 2750
public class ThreadInfoParam
{
	// Token: 0x060054BA RID: 21690 RVA: 0x00229D50 File Offset: 0x00227F50
	public ThreadInfoParam()
	{
		this.IsBigCapacity = false;
		this.ThreadContListA = new ThreadContainer[this.TmpArraySizeFristDim * 20];
		this.ForwardChunkToDeleteIdA = new DistantChunk[this.TmpArraySizeFristDim];
		this.BackwardChunkToDeleteIdA = new DistantChunk[this.TmpArraySizeFristDim * 2];
		this.ForwardChunkSeamToAdjust = new DistantChunk[this.TmpArraySizeFristDim][];
		this.BackwardChunkSeamToAdjust = new DistantChunk[this.TmpArraySizeFristDim][];
		this.ForwardEdgeId = new int[this.TmpArraySizeFristDim][];
		this.BackwardEdgeId = new int[this.TmpArraySizeFristDim][];
		this.SDLengthForwardChunkSeamToAdjust = new int[this.TmpArraySizeFristDim];
		this.SDLengthBackwardChunkSeamToAdjust = new int[this.TmpArraySizeFristDim];
		for (int i = 0; i < this.TmpArraySizeFristDim; i++)
		{
			this.ForwardChunkSeamToAdjust[i] = new DistantChunk[this.TmpArraySizeSecondDim];
			this.BackwardChunkSeamToAdjust[i] = new DistantChunk[this.TmpArraySizeSecondDim];
			this.ForwardEdgeId[i] = new int[this.TmpArraySizeSecondDim];
			this.BackwardEdgeId[i] = new int[this.TmpArraySizeSecondDim];
		}
		this.FDLengthForwardChunkSeamToAdjust = 0;
		this.FDLengthBackwardChunkSeamToAdjust = 0;
		this.CntForwardChunkSeamToAdjust = 0;
		this.CntBackwardChunkSeamToAdjust = 0;
		this.ResLevel = 0;
		this.OutId = 0;
		this.IsThreadDone = false;
		this.IsCoroutineDone = false;
	}

	// Token: 0x060054BB RID: 21691 RVA: 0x00229EB0 File Offset: 0x002280B0
	public ThreadInfoParam(DistantChunkMap _CMap, int _ResLevel, int _OutId, bool _IsBigCapacity)
	{
		this.ThreadContListA = new ThreadContainer[this.TmpArraySizeFristDim * 20];
		this.ForwardChunkToDeleteIdA = new DistantChunk[this.TmpArraySizeFristDim];
		this.BackwardChunkToDeleteIdA = new DistantChunk[this.TmpArraySizeFristDim * 2];
		this.ForwardChunkSeamToAdjust = new DistantChunk[this.TmpArraySizeFristDim][];
		this.BackwardChunkSeamToAdjust = new DistantChunk[this.TmpArraySizeFristDim][];
		this.ForwardEdgeId = new int[this.TmpArraySizeFristDim][];
		this.BackwardEdgeId = new int[this.TmpArraySizeFristDim][];
		this.SDLengthForwardChunkSeamToAdjust = new int[this.TmpArraySizeFristDim];
		this.SDLengthBackwardChunkSeamToAdjust = new int[this.TmpArraySizeFristDim];
		for (int i = 0; i < this.TmpArraySizeFristDim; i++)
		{
			this.ForwardChunkSeamToAdjust[i] = new DistantChunk[this.TmpArraySizeSecondDim];
			this.BackwardChunkSeamToAdjust[i] = new DistantChunk[this.TmpArraySizeSecondDim];
			this.ForwardEdgeId[i] = new int[this.TmpArraySizeSecondDim];
			this.BackwardEdgeId[i] = new int[this.TmpArraySizeSecondDim];
			this.SDLengthForwardChunkSeamToAdjust[i] = 0;
			this.SDLengthBackwardChunkSeamToAdjust[i] = 0;
		}
		this.Init(_CMap, _ResLevel, _OutId, _IsBigCapacity);
	}

	// Token: 0x060054BC RID: 21692 RVA: 0x00229FF0 File Offset: 0x002281F0
	public void Init(DistantChunkMap _CMap, int _ResLevel, int _OutId, bool _IsBigCapacity)
	{
		this.IsBigCapacity = _IsBigCapacity;
		this.CntThreadContList = 0;
		this.LengthThreadContList = 0;
		this.CntForwardChunkToDeleteId = 0;
		this.LengthForwardChunkToDeleteId = 0;
		this.CntBackwardChunkToDeleteId = 0;
		this.LengthBackwardChunkToDeleteId = 0;
		for (int i = 0; i < this.TmpArraySizeFristDim; i++)
		{
			this.SDLengthForwardChunkSeamToAdjust[i] = 0;
			this.SDLengthBackwardChunkSeamToAdjust[i] = 0;
		}
		this.CntForwardChunkSeamToAdjust = 0;
		this.CntBackwardChunkSeamToAdjust = 0;
		this.FDLengthForwardChunkSeamToAdjust = 0;
		this.FDLengthBackwardChunkSeamToAdjust = 0;
		this.ResLevel = _ResLevel;
		this.OutId = _OutId;
		this.IsThreadDone = false;
		this.IsCoroutineDone = false;
	}

	// Token: 0x060054BD RID: 21693 RVA: 0x0022A08C File Offset: 0x0022828C
	public void ClearAll(ThreadContainerPool TmpThContPool = null)
	{
		if (TmpThContPool != null)
		{
			while (this.CntThreadContList < this.LengthThreadContList)
			{
				TmpThContPool.ReturnObject(this.ThreadContListA[this.CntThreadContList], true);
				this.ThreadContListA[this.CntThreadContList] = null;
				this.CntThreadContList++;
			}
		}
		this.IsThreadDone = true;
		this.IsCoroutineDone = true;
	}

	// Token: 0x04004189 RID: 16777
	public ThreadContainer[] ThreadContListA;

	// Token: 0x0400418A RID: 16778
	public int CntThreadContList;

	// Token: 0x0400418B RID: 16779
	public int LengthThreadContList;

	// Token: 0x0400418C RID: 16780
	public int ResLevel;

	// Token: 0x0400418D RID: 16781
	public int OutId;

	// Token: 0x0400418E RID: 16782
	public DistantChunk[] ForwardChunkToDeleteIdA;

	// Token: 0x0400418F RID: 16783
	public DistantChunk[] BackwardChunkToDeleteIdA;

	// Token: 0x04004190 RID: 16784
	public int CntForwardChunkToDeleteId;

	// Token: 0x04004191 RID: 16785
	public int LengthForwardChunkToDeleteId;

	// Token: 0x04004192 RID: 16786
	public int CntBackwardChunkToDeleteId;

	// Token: 0x04004193 RID: 16787
	public int LengthBackwardChunkToDeleteId;

	// Token: 0x04004194 RID: 16788
	public DistantChunk[][] ForwardChunkSeamToAdjust;

	// Token: 0x04004195 RID: 16789
	public DistantChunk[][] BackwardChunkSeamToAdjust;

	// Token: 0x04004196 RID: 16790
	public int[][] ForwardEdgeId;

	// Token: 0x04004197 RID: 16791
	public int[][] BackwardEdgeId;

	// Token: 0x04004198 RID: 16792
	public int CntForwardChunkSeamToAdjust;

	// Token: 0x04004199 RID: 16793
	public int CntBackwardChunkSeamToAdjust;

	// Token: 0x0400419A RID: 16794
	public int FDLengthForwardChunkSeamToAdjust;

	// Token: 0x0400419B RID: 16795
	public int FDLengthBackwardChunkSeamToAdjust;

	// Token: 0x0400419C RID: 16796
	public int[] SDLengthForwardChunkSeamToAdjust;

	// Token: 0x0400419D RID: 16797
	public int[] SDLengthBackwardChunkSeamToAdjust;

	// Token: 0x0400419E RID: 16798
	[PublicizedFrom(EAccessModifier.Private)]
	public int TmpArraySizeFristDim = 150;

	// Token: 0x0400419F RID: 16799
	[PublicizedFrom(EAccessModifier.Private)]
	public int TmpArraySizeSecondDim = 64;

	// Token: 0x040041A0 RID: 16800
	public bool IsThreadDone;

	// Token: 0x040041A1 RID: 16801
	public bool IsCoroutineDone;

	// Token: 0x040041A2 RID: 16802
	public bool IsBigCapacity;

	// Token: 0x040041A3 RID: 16803
	public bool IsAsynchronous;
}
