using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000B5A RID: 2906
public struct UnsafeFixedBuffer<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
{
	// Token: 0x17000939 RID: 2361
	// (get) Token: 0x06005A6A RID: 23146 RVA: 0x00244730 File Offset: 0x00242930
	public unsafe int Count
	{
		get
		{
			return this.data->count;
		}
	}

	// Token: 0x1700093A RID: 2362
	// (get) Token: 0x06005A6B RID: 23147 RVA: 0x0024473D File Offset: 0x0024293D
	public bool IsCreated
	{
		get
		{
			return this.data != null;
		}
	}

	// Token: 0x06005A6C RID: 23148 RVA: 0x0024474C File Offset: 0x0024294C
	public unsafe UnsafeFixedBuffer(int _capacity, AllocatorManager.AllocatorHandle _allocator)
	{
		this.data = AllocatorManager.Allocate<UnsafeFixedBuffer<T>.Data>(_allocator, 1);
		this.data->buffer = AllocatorManager.Allocate<T>(_allocator, _capacity);
		this.data->count = 0;
		this.capacity = _capacity;
		this.allocator = _allocator;
	}

	// Token: 0x06005A6D RID: 23149 RVA: 0x00244788 File Offset: 0x00242988
	public unsafe void AddThreadSafe(T item)
	{
		int count;
		for (;;)
		{
			count = this.data->count;
			int num = this.data->count + 1;
			if (num > this.capacity)
			{
				break;
			}
			if (Interlocked.CompareExchange(ref this.data->count, num, count) == count)
			{
				goto Block_1;
			}
		}
		throw new IndexOutOfRangeException(string.Format("Index {0} is outside the UnsafeFixedBuffer capacity {1}", count, this.capacity));
		Block_1:
		UnsafeUtility.WriteArrayElement<T>((void*)this.data->buffer, count, item);
	}

	// Token: 0x06005A6E RID: 23150 RVA: 0x00244800 File Offset: 0x00242A00
	public unsafe void Clear()
	{
		this.data->count = 0;
	}

	// Token: 0x06005A6F RID: 23151 RVA: 0x0024480E File Offset: 0x00242A0E
	public unsafe NativeArray<T> AsNativeArray()
	{
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.data->buffer, this.data->count, Allocator.Invalid);
	}

	// Token: 0x06005A70 RID: 23152 RVA: 0x0024482C File Offset: 0x00242A2C
	public unsafe void Dispose()
	{
		if (this.data != null)
		{
			AllocatorManager.Free<T>(this.allocator, this.data->buffer, this.capacity);
			AllocatorManager.Free<UnsafeFixedBuffer<T>.Data>(this.allocator, this.data, 1);
			this.data = null;
		}
	}

	// Token: 0x06005A71 RID: 23153 RVA: 0x00244879 File Offset: 0x00242A79
	public int CalculateOwnedBytes()
	{
		return UnsafeUtility.SizeOf<UnsafeFixedBuffer<T>>() + CollectionHelper.Align(this.capacity * UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>());
	}

	// Token: 0x0400452A RID: 17706
	[NativeDisableUnsafePtrRestriction]
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe UnsafeFixedBuffer<T>.Data* data;

	// Token: 0x0400452B RID: 17707
	[PublicizedFrom(EAccessModifier.Private)]
	public int capacity;

	// Token: 0x0400452C RID: 17708
	[PublicizedFrom(EAccessModifier.Private)]
	public AllocatorManager.AllocatorHandle allocator;

	// Token: 0x02000B5B RID: 2907
	[PublicizedFrom(EAccessModifier.Private)]
	public struct Data
	{
		// Token: 0x0400452D RID: 17709
		public unsafe T* buffer;

		// Token: 0x0400452E RID: 17710
		public int count;
	}
}
