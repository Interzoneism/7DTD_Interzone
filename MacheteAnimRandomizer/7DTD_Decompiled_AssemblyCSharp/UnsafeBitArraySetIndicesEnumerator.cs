using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000B56 RID: 2902
public struct UnsafeBitArraySetIndicesEnumerator : IEnumerator<int>, IEnumerator, IDisposable
{
	// Token: 0x06005A51 RID: 23121 RVA: 0x00243F8C File Offset: 0x0024218C
	public unsafe UnsafeBitArraySetIndicesEnumerator(UnsafeBitArray bitArray)
	{
		this.bitArray = bitArray;
		this.currentSlice = *bitArray.Ptr;
		this.sliceIndex = 0;
		this.numSlices = bitArray.Length / 64;
		this.currentIndex = -1;
		this.leadingZeroCount = 0;
		this.numSetBits = bitArray.CountBits(0, bitArray.Length);
		this.foundBits = 0;
	}

	// Token: 0x17000935 RID: 2357
	// (get) Token: 0x06005A52 RID: 23122 RVA: 0x00243FEC File Offset: 0x002421EC
	public int Current
	{
		get
		{
			return this.currentIndex;
		}
	}

	// Token: 0x17000936 RID: 2358
	// (get) Token: 0x06005A53 RID: 23123 RVA: 0x00243FF4 File Offset: 0x002421F4
	public object Current
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.Current;
		}
	}

	// Token: 0x06005A54 RID: 23124 RVA: 0x00244004 File Offset: 0x00242204
	public unsafe bool MoveNext()
	{
		while (this.foundBits < this.numSetBits && this.sliceIndex < this.numSlices)
		{
			if (this.currentSlice == 0UL)
			{
				this.sliceIndex++;
				if (this.sliceIndex < this.numSlices)
				{
					this.currentSlice = this.bitArray.Ptr[this.sliceIndex];
					this.leadingZeroCount = 0;
				}
			}
			else
			{
				if ((this.currentSlice & 1UL) != 0UL)
				{
					this.currentIndex = this.leadingZeroCount + this.sliceIndex * 64;
					this.leadingZeroCount++;
					this.currentSlice >>= 1;
					this.foundBits++;
					return true;
				}
				if ((this.currentSlice & (ulong)-1) == 0UL)
				{
					this.leadingZeroCount += 32;
					this.currentSlice >>= 32;
				}
				if ((this.currentSlice & 65535UL) == 0UL)
				{
					this.leadingZeroCount += 16;
					this.currentSlice >>= 16;
				}
				if ((this.currentSlice & 255UL) == 0UL)
				{
					this.leadingZeroCount += 8;
					this.currentSlice >>= 8;
				}
				if ((this.currentSlice & 15UL) == 0UL)
				{
					this.leadingZeroCount += 4;
					this.currentSlice >>= 4;
				}
				if ((this.currentSlice & 3UL) == 0UL)
				{
					this.leadingZeroCount += 2;
					this.currentSlice >>= 2;
				}
				if ((this.currentSlice & 1UL) == 0UL)
				{
					this.leadingZeroCount++;
					this.currentSlice >>= 1;
				}
			}
		}
		return false;
	}

	// Token: 0x06005A55 RID: 23125 RVA: 0x002441C9 File Offset: 0x002423C9
	public unsafe void Reset()
	{
		this.sliceIndex = 0;
		this.currentSlice = *this.bitArray.Ptr;
		this.currentIndex = -1;
		this.leadingZeroCount = 0;
		this.foundBits = 0;
	}

	// Token: 0x06005A56 RID: 23126 RVA: 0x00002914 File Offset: 0x00000B14
	public void Dispose()
	{
	}

	// Token: 0x04004519 RID: 17689
	[PublicizedFrom(EAccessModifier.Private)]
	public UnsafeBitArray bitArray;

	// Token: 0x0400451A RID: 17690
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong currentSlice;

	// Token: 0x0400451B RID: 17691
	[PublicizedFrom(EAccessModifier.Private)]
	public int sliceIndex;

	// Token: 0x0400451C RID: 17692
	[PublicizedFrom(EAccessModifier.Private)]
	public int numSlices;

	// Token: 0x0400451D RID: 17693
	[PublicizedFrom(EAccessModifier.Private)]
	public int leadingZeroCount;

	// Token: 0x0400451E RID: 17694
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentIndex;

	// Token: 0x0400451F RID: 17695
	[PublicizedFrom(EAccessModifier.Private)]
	public int numSetBits;

	// Token: 0x04004520 RID: 17696
	[PublicizedFrom(EAccessModifier.Private)]
	public int foundBits;
}
