using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000B59 RID: 2905
public struct UnsafeChunkXZMap<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
{
	// Token: 0x17000938 RID: 2360
	// (get) Token: 0x06005A61 RID: 23137 RVA: 0x00244638 File Offset: 0x00242838
	public bool IsCreated
	{
		get
		{
			return this.map != null;
		}
	}

	// Token: 0x06005A62 RID: 23138 RVA: 0x00244647 File Offset: 0x00242847
	public UnsafeChunkXZMap(AllocatorManager.AllocatorHandle _allocator)
	{
		this.allocator = _allocator;
		this.map = AllocatorManager.Allocate<T>(_allocator, 256);
	}

	// Token: 0x06005A63 RID: 23139 RVA: 0x00244661 File Offset: 0x00242861
	public unsafe T Get(int _x, int _z)
	{
		return this.map[(IntPtr)UnsafeChunkXZMap<T>.GetMapIndex(_x, _z) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
	}

	// Token: 0x06005A64 RID: 23140 RVA: 0x0024467E File Offset: 0x0024287E
	public unsafe void Set(int _x, int _z, T value)
	{
		this.map[(IntPtr)UnsafeChunkXZMap<T>.GetMapIndex(_x, _z) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
	}

	// Token: 0x06005A65 RID: 23141 RVA: 0x0024469C File Offset: 0x0024289C
	public unsafe void Clear()
	{
		if (this.map != null)
		{
			UnsafeUtility.MemClear((void*)this.map, (long)(256 * UnsafeUtility.SizeOf<T>()));
		}
	}

	// Token: 0x06005A66 RID: 23142 RVA: 0x002446BF File Offset: 0x002428BF
	public void Dispose()
	{
		if (this.map != null)
		{
			AllocatorManager.Free<T>(this.allocator, this.map, 256);
		}
	}

	// Token: 0x06005A67 RID: 23143 RVA: 0x002446E4 File Offset: 0x002428E4
	public int CalculateOwnedBytes()
	{
		int num = UnsafeUtility.SizeOf<UnsafeChunkXZMap<T>>();
		if (this.map != null)
		{
			num += CollectionHelper.Align(256 * UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>());
		}
		return num;
	}

	// Token: 0x06005A68 RID: 23144 RVA: 0x0024471A File Offset: 0x0024291A
	public static int GetMapIndex(int _x, int _z)
	{
		return _x + 16 * _z;
	}

	// Token: 0x06005A69 RID: 23145 RVA: 0x00244722 File Offset: 0x00242922
	public static void GetMapCoords(int _index, out int _x, out int _z)
	{
		_z = _index / 16;
		_x = _index % 16;
	}

	// Token: 0x04004527 RID: 17703
	public const int MAP_SIZE = 256;

	// Token: 0x04004528 RID: 17704
	[NativeDisableUnsafePtrRestriction]
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe T* map;

	// Token: 0x04004529 RID: 17705
	[PublicizedFrom(EAccessModifier.Private)]
	public AllocatorManager.AllocatorHandle allocator;
}
