using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x020009F9 RID: 2553
public sealed class PinnedBuffer<[IsUnmanaged] T> : PinnedBuffer where T : struct, ValueType
{
	// Token: 0x06004E2C RID: 20012 RVA: 0x001EDFD7 File Offset: 0x001EC1D7
	public PinnedBuffer(MemoryPooledArray<T> pool, int length)
	{
		if (length <= 0)
		{
			this.InitBuffer(Array.Empty<T>(), 0);
			return;
		}
		this.m_pool = pool;
		this.m_poolArray = this.m_pool.Alloc(length);
		this.InitBuffer(this.m_poolArray, length);
	}

	// Token: 0x06004E2D RID: 20013 RVA: 0x001EE016 File Offset: 0x001EC216
	public PinnedBuffer()
	{
		this.InitBuffer(Array.Empty<T>(), 0);
	}

	// Token: 0x06004E2E RID: 20014 RVA: 0x001EE02A File Offset: 0x001EC22A
	public PinnedBuffer(int length)
	{
		if (length <= 0)
		{
			this.InitBuffer(Array.Empty<T>(), 0);
			return;
		}
		this.InitBuffer(new T[length], length);
	}

	// Token: 0x06004E2F RID: 20015 RVA: 0x001EE050 File Offset: 0x001EC250
	public PinnedBuffer(T[] buffer, int length)
	{
		if (length <= 0)
		{
			this.InitBuffer(Array.Empty<T>(), 0);
			return;
		}
		this.InitBuffer(buffer, length);
	}

	// Token: 0x06004E30 RID: 20016 RVA: 0x001EE071 File Offset: 0x001EC271
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe void InitBuffer(T[] buffer, int length)
	{
		this.m_ptr = (T*)UnsafeUtility.PinGCArrayAndGetDataAddress(buffer, out this.m_gcHandle);
		this.m_length = length;
	}

	// Token: 0x06004E31 RID: 20017 RVA: 0x001EE08C File Offset: 0x001EC28C
	public override void Dispose()
	{
		this.m_length = 0;
		this.m_ptr = null;
		if (this.m_gcHandle > 0UL)
		{
			UnsafeUtility.ReleaseGCObject(this.m_gcHandle);
			this.m_gcHandle = 0UL;
		}
		if (this.m_pool != null)
		{
			this.m_pool.Free(this.m_poolArray);
			this.m_pool = null;
		}
		this.m_poolArray = null;
	}

	// Token: 0x170007FE RID: 2046
	// (get) Token: 0x06004E32 RID: 20018 RVA: 0x001EE0EC File Offset: 0x001EC2EC
	public unsafe T* Ptr
	{
		get
		{
			return this.m_ptr;
		}
	}

	// Token: 0x170007FF RID: 2047
	// (get) Token: 0x06004E33 RID: 20019 RVA: 0x001EE0F4 File Offset: 0x001EC2F4
	public int Length
	{
		get
		{
			return this.m_length;
		}
	}

	// Token: 0x06004E34 RID: 20020 RVA: 0x001EE0FC File Offset: 0x001EC2FC
	public PinnedBufferRef<T> AsRef()
	{
		return new PinnedBufferRef<T>(this.m_ptr, this.m_length);
	}

	// Token: 0x06004E35 RID: 20021 RVA: 0x001EE10F File Offset: 0x001EC30F
	public static implicit operator PinnedBufferRef<T>(PinnedBuffer<T> buffer)
	{
		return buffer.AsRef();
	}

	// Token: 0x06004E36 RID: 20022 RVA: 0x001EE118 File Offset: 0x001EC318
	public PinnedBufferRef<byte> AsBytes()
	{
		return this.AsRef().AsBytes();
	}

	// Token: 0x06004E37 RID: 20023 RVA: 0x001EE133 File Offset: 0x001EC333
	public static implicit operator PinnedBufferRef<byte>(PinnedBuffer<T> buffer)
	{
		return buffer.AsBytes();
	}

	// Token: 0x06004E38 RID: 20024 RVA: 0x001EE13B File Offset: 0x001EC33B
	public unsafe Span<T> AsSpan()
	{
		return new Span<T>((void*)this.m_ptr, this.m_length);
	}

	// Token: 0x06004E39 RID: 20025 RVA: 0x001EE14E File Offset: 0x001EC34E
	public static implicit operator Span<T>(PinnedBuffer<T> buffer)
	{
		return buffer.AsSpan();
	}

	// Token: 0x06004E3A RID: 20026 RVA: 0x001EE156 File Offset: 0x001EC356
	public unsafe ReadOnlySpan<T> AsReadOnlySpan()
	{
		return new ReadOnlySpan<T>((void*)this.m_ptr, this.m_length);
	}

	// Token: 0x06004E3B RID: 20027 RVA: 0x001EE169 File Offset: 0x001EC369
	public static implicit operator ReadOnlySpan<T>(PinnedBuffer<T> buffer)
	{
		return buffer.AsReadOnlySpan();
	}

	// Token: 0x06004E3C RID: 20028 RVA: 0x001EE171 File Offset: 0x001EC371
	public AtomicSafeHandleScope CreateNativeArray(out NativeArray<T> array)
	{
		return MeshDataUtils.CreateNativeArray<T>(this.m_ptr, this.m_length, out array);
	}

	// Token: 0x04003BBA RID: 15290
	[PublicizedFrom(EAccessModifier.Private)]
	public MemoryPooledArray<T> m_pool;

	// Token: 0x04003BBB RID: 15291
	[PublicizedFrom(EAccessModifier.Private)]
	public T[] m_poolArray;

	// Token: 0x04003BBC RID: 15292
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe T* m_ptr;

	// Token: 0x04003BBD RID: 15293
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_length;

	// Token: 0x04003BBE RID: 15294
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong m_gcHandle;
}
