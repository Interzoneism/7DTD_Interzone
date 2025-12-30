using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000B57 RID: 2903
public struct UnsafeChunkData<[IsUnmanaged] T> : IDisposable where T : struct, ValueType, IEquatable<T>
{
	// Token: 0x17000937 RID: 2359
	// (get) Token: 0x06005A57 RID: 23127 RVA: 0x002441F9 File Offset: 0x002423F9
	public bool IsCreated
	{
		get
		{
			return this.layers != null;
		}
	}

	// Token: 0x06005A58 RID: 23128 RVA: 0x00244208 File Offset: 0x00242408
	public unsafe UnsafeChunkData(AllocatorManager.AllocatorHandle _allocator)
	{
		this.allocator = _allocator;
		this.layers = AllocatorManager.Allocate<UnsafeChunkData<T>.LayerData>(this.allocator, 64);
		UnsafeUtility.MemClear((void*)this.layers, (long)(UnsafeUtility.SizeOf<UnsafeChunkData<T>.LayerData>() * 64));
		this.sameValues = AllocatorManager.Allocate<T>(this.allocator, 64);
		UnsafeUtility.MemClear((void*)this.sameValues, (long)(UnsafeUtility.SizeOf<T>() * 64));
	}

	// Token: 0x06005A59 RID: 23129 RVA: 0x0024426C File Offset: 0x0024246C
	public unsafe T Get(int _x, int _y, int _z)
	{
		int num = _y / 4;
		UnsafeChunkData<T>.LayerData* ptr = this.layers + (IntPtr)num * (IntPtr)sizeof(UnsafeChunkData<T>.LayerData) / (IntPtr)sizeof(UnsafeChunkData<T>.LayerData);
		if (ptr->items == null)
		{
			return this.sameValues[(IntPtr)num * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
		}
		int num2 = _x + 16 * _z;
		int num3 = _y % 4;
		int num4 = num2 + num3 * 256;
		return ptr->items[(IntPtr)num4 * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
	}

	// Token: 0x06005A5A RID: 23130 RVA: 0x002442D8 File Offset: 0x002424D8
	public unsafe T Get(int _chunkIndex)
	{
		int num = _chunkIndex / 256;
		int num2 = num / 4;
		UnsafeChunkData<T>.LayerData* ptr = this.layers + (IntPtr)num2 * (IntPtr)sizeof(UnsafeChunkData<T>.LayerData) / (IntPtr)sizeof(UnsafeChunkData<T>.LayerData);
		if (ptr->items == null)
		{
			return this.sameValues[(IntPtr)num2 * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
		}
		int num3 = _chunkIndex % 256;
		int num4 = num % 4;
		int num5 = num3 + num4 * 256;
		return ptr->items[(IntPtr)num5 * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
	}

	// Token: 0x06005A5B RID: 23131 RVA: 0x00244350 File Offset: 0x00242550
	public void CheckSameValues()
	{
		for (int i = 0; i < 64; i++)
		{
			this.CheckSameValue(i);
		}
	}

	// Token: 0x06005A5C RID: 23132 RVA: 0x00244374 File Offset: 0x00242574
	public unsafe void CheckSameValue(int _layerIndex)
	{
		UnsafeChunkData<T>.LayerData* ptr = this.layers + (IntPtr)_layerIndex * (IntPtr)sizeof(UnsafeChunkData<T>.LayerData) / (IntPtr)sizeof(UnsafeChunkData<T>.LayerData);
		T* items = ptr->items;
		if (items == null)
		{
			return;
		}
		if (items != null)
		{
			T t = *items;
			for (int i = 1; i < 1024; i++)
			{
				if (!t.Equals(items[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)]))
				{
					return;
				}
			}
			this.sameValues[(IntPtr)_layerIndex * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = t;
			AllocatorManager.Free<T>(this.allocator, ptr->items, 1024);
			ptr->items = null;
		}
	}

	// Token: 0x06005A5D RID: 23133 RVA: 0x00244410 File Offset: 0x00242610
	public unsafe void Set(int _chunkIndex, T _value)
	{
		int num = _chunkIndex / 256;
		int num2 = num / 4;
		UnsafeChunkData<T>.LayerData* ptr = this.layers + (IntPtr)num2 * (IntPtr)sizeof(UnsafeChunkData<T>.LayerData) / (IntPtr)sizeof(UnsafeChunkData<T>.LayerData);
		if (ptr->items == null)
		{
			T* ptr2 = AllocatorManager.Allocate<T>(this.allocator, 1024);
			T t = this.sameValues[(IntPtr)num2 * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			for (int i = 0; i < 1024; i++)
			{
				ptr2[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = t;
			}
			ptr->items = ptr2;
		}
		int num3 = _chunkIndex % 256;
		int num4 = num % 4;
		int num5 = num3 + num4 * 256;
		ptr->items[(IntPtr)num5 * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = _value;
	}

	// Token: 0x06005A5E RID: 23134 RVA: 0x002444CC File Offset: 0x002426CC
	public unsafe void Clear()
	{
		if (this.layers != null)
		{
			for (int i = 0; i < 64; i++)
			{
				UnsafeChunkData<T>.LayerData* ptr = this.layers + (IntPtr)i * (IntPtr)sizeof(UnsafeChunkData<T>.LayerData) / (IntPtr)sizeof(UnsafeChunkData<T>.LayerData);
				if (ptr->items != null)
				{
					AllocatorManager.Free<T>(this.allocator, ptr->items, 1024);
					ptr->items = null;
				}
			}
		}
		if (this.sameValues != null)
		{
			UnsafeUtility.MemClear((void*)this.sameValues, (long)(UnsafeUtility.SizeOf<T>() * 64));
		}
	}

	// Token: 0x06005A5F RID: 23135 RVA: 0x00244548 File Offset: 0x00242748
	public void Dispose()
	{
		this.Clear();
		if (this.layers != null)
		{
			AllocatorManager.Free<UnsafeChunkData<T>.LayerData>(this.allocator, this.layers, 64);
			this.layers = null;
		}
		if (this.sameValues != null)
		{
			AllocatorManager.Free<T>(this.allocator, this.sameValues, 64);
			this.sameValues = null;
		}
	}

	// Token: 0x06005A60 RID: 23136 RVA: 0x002445A8 File Offset: 0x002427A8
	public unsafe int CalculateOwnedBytes()
	{
		int num = UnsafeUtility.SizeOf<UnsafeChunkData<T>>();
		if (this.layers != null)
		{
			num += CollectionHelper.Align(64 * UnsafeUtility.SizeOf<UnsafeChunkData<T>.LayerData>(), UnsafeUtility.AlignOf<UnsafeChunkData<T>.LayerData>());
			for (int i = 0; i < 64; i++)
			{
				if (this.layers[(IntPtr)i * (IntPtr)sizeof(UnsafeChunkData<T>.LayerData) / (IntPtr)sizeof(UnsafeChunkData<T>.LayerData)].items != null)
				{
					num += CollectionHelper.Align(1024 * UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>());
				}
			}
		}
		if (this.sameValues != null)
		{
			num += CollectionHelper.Align(64 * UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>());
		}
		return num;
	}

	// Token: 0x04004521 RID: 17697
	public const int LAYER_SIZE = 1024;

	// Token: 0x04004522 RID: 17698
	public const int NUM_LAYERS = 64;

	// Token: 0x04004523 RID: 17699
	[NativeDisableUnsafePtrRestriction]
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe UnsafeChunkData<T>.LayerData* layers;

	// Token: 0x04004524 RID: 17700
	[NativeDisableUnsafePtrRestriction]
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe T* sameValues;

	// Token: 0x04004525 RID: 17701
	[PublicizedFrom(EAccessModifier.Private)]
	public AllocatorManager.AllocatorHandle allocator;

	// Token: 0x02000B58 RID: 2904
	[PublicizedFrom(EAccessModifier.Private)]
	public struct LayerData
	{
		// Token: 0x04004526 RID: 17702
		public unsafe T* items;
	}
}
