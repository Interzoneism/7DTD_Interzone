using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x020009FA RID: 2554
public readonly struct PinnedBufferRef<[IsUnmanaged] T> where T : struct, ValueType
{
	// Token: 0x06004E3D RID: 20029 RVA: 0x001EE185 File Offset: 0x001EC385
	public unsafe PinnedBufferRef(T* ptr, int length)
	{
		this.Ptr = ptr;
		this.Length = length;
	}

	// Token: 0x06004E3E RID: 20030 RVA: 0x001EE195 File Offset: 0x001EC395
	public unsafe Span<T> AsSpan()
	{
		return new Span<T>((void*)this.Ptr, this.Length);
	}

	// Token: 0x06004E3F RID: 20031 RVA: 0x001EE1A8 File Offset: 0x001EC3A8
	public static implicit operator Span<T>(PinnedBufferRef<T> buffer)
	{
		return buffer.AsSpan();
	}

	// Token: 0x06004E40 RID: 20032 RVA: 0x001EE1B1 File Offset: 0x001EC3B1
	public unsafe ReadOnlySpan<T> AsReadOnlySpan()
	{
		return new ReadOnlySpan<T>((void*)this.Ptr, this.Length);
	}

	// Token: 0x06004E41 RID: 20033 RVA: 0x001EE1C4 File Offset: 0x001EC3C4
	public static implicit operator ReadOnlySpan<T>(PinnedBufferRef<T> buffer)
	{
		return buffer.AsReadOnlySpan();
	}

	// Token: 0x06004E42 RID: 20034 RVA: 0x001EE1CD File Offset: 0x001EC3CD
	public unsafe PinnedBufferRef<byte> AsBytes()
	{
		return new PinnedBufferRef<byte>((byte*)this.Ptr, this.Length * sizeof(T));
	}

	// Token: 0x06004E43 RID: 20035 RVA: 0x001EE1E7 File Offset: 0x001EC3E7
	public static implicit operator PinnedBufferRef<byte>(PinnedBufferRef<T> buffer)
	{
		return buffer.AsBytes();
	}

	// Token: 0x06004E44 RID: 20036 RVA: 0x001EE1F0 File Offset: 0x001EC3F0
	public AtomicSafeHandleScope CreateNativeArray(out NativeArray<T> array)
	{
		return MeshDataUtils.CreateNativeArray<T>(this.Ptr, this.Length, out array);
	}

	// Token: 0x04003BBF RID: 15295
	[NativeDisableUnsafePtrRestriction]
	public unsafe readonly T* Ptr;

	// Token: 0x04003BC0 RID: 15296
	public readonly int Length;
}
