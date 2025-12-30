using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02001128 RID: 4392
public class MemoryBackedArray<[IsUnmanaged] T> : IBackedArray<T>, IDisposable where T : struct, ValueType
{
	// Token: 0x060089FF RID: 35327 RVA: 0x0037D829 File Offset: 0x0037BA29
	public MemoryBackedArray(int length)
	{
		this.m_array = new T[length];
	}

	// Token: 0x06008A00 RID: 35328 RVA: 0x0037D83D File Offset: 0x0037BA3D
	[PublicizedFrom(EAccessModifier.Private)]
	public void Dispose(bool disposing)
	{
		if (!disposing)
		{
			Log.Error("MemoryBackedArray<T> is being finalized, it should be disposed properly.");
			return;
		}
		this.m_array = null;
	}

	// Token: 0x06008A01 RID: 35329 RVA: 0x0037D854 File Offset: 0x0037BA54
	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	// Token: 0x06008A02 RID: 35330 RVA: 0x0037D864 File Offset: 0x0037BA64
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~MemoryBackedArray()
	{
		this.Dispose(false);
	}

	// Token: 0x06008A03 RID: 35331 RVA: 0x0037D894 File Offset: 0x0037BA94
	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void ThrowIfDisposed()
	{
		if (this.m_array == null)
		{
			throw new ObjectDisposedException("MemoryBackedArray has already been disposed.");
		}
	}

	// Token: 0x17000E66 RID: 3686
	// (get) Token: 0x06008A04 RID: 35332 RVA: 0x0037D8A9 File Offset: 0x0037BAA9
	public int Length
	{
		get
		{
			return this.m_array.Length;
		}
	}

	// Token: 0x06008A05 RID: 35333 RVA: 0x0037D8B4 File Offset: 0x0037BAB4
	[PublicizedFrom(EAccessModifier.Private)]
	public static MemoryBackedArray<T>.MemoryBackedArrayHandle GetStaticHandle(BackedArrayHandleMode mode)
	{
		MemoryBackedArray<T>.MemoryBackedArrayHandle result;
		if (mode != BackedArrayHandleMode.ReadOnly)
		{
			if (mode != BackedArrayHandleMode.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("mode", mode, string.Format("Unknown mode: {0}", mode));
			}
			result = MemoryBackedArray<T>.s_handleReadWrite;
		}
		else
		{
			result = MemoryBackedArray<T>.s_handleReadOnly;
		}
		return result;
	}

	// Token: 0x06008A06 RID: 35334 RVA: 0x0037D8FC File Offset: 0x0037BAFC
	[PublicizedFrom(EAccessModifier.Private)]
	public IBackedArrayHandle GetMemoryInternal(int start, int length, out Memory<T> memory, BackedArrayHandleMode mode)
	{
		memory = this.m_array.AsMemory(start, length);
		return MemoryBackedArray<T>.GetStaticHandle(mode);
	}

	// Token: 0x06008A07 RID: 35335 RVA: 0x0037D918 File Offset: 0x0037BB18
	public IBackedArrayHandle GetMemory(int start, int length, out Memory<T> memory)
	{
		return this.GetMemoryInternal(start, length, out memory, BackedArrayHandleMode.ReadWrite);
	}

	// Token: 0x06008A08 RID: 35336 RVA: 0x0037D924 File Offset: 0x0037BB24
	public IBackedArrayHandle GetReadOnlyMemory(int start, int length, out ReadOnlyMemory<T> memory)
	{
		Memory<T> memory2;
		IBackedArrayHandle memoryInternal = this.GetMemoryInternal(start, length, out memory2, BackedArrayHandleMode.ReadOnly);
		memory = memory2;
		return memoryInternal;
	}

	// Token: 0x06008A09 RID: 35337 RVA: 0x0037D948 File Offset: 0x0037BB48
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe IBackedArrayHandle GetMemoryUnsafeInternal(int start, int length, out T* arrayPtr, BackedArrayHandleMode mode)
	{
		if (length < 0)
		{
			throw new ArgumentOutOfRangeException(string.Format("Expected length to be non-negative but was {0}.", length));
		}
		if (start < 0 || start + length > this.m_array.Length)
		{
			throw new ArgumentOutOfRangeException(string.Format("Expected requested memory range [{0}, {1}) to be a subset of [0, {2}).", start, start + length, this.m_array.Length));
		}
		GCHandle gcHandle = GCHandle.Alloc(this.m_array, GCHandleType.Pinned);
		arrayPtr = (void*)gcHandle.AddrOfPinnedObject();
		arrayPtr += (IntPtr)start * (IntPtr)sizeof(T);
		return new MemoryBackedArray<T>.MemoryBackedArrayUnsafeHandle(gcHandle, mode);
	}

	// Token: 0x06008A0A RID: 35338 RVA: 0x0037D9DC File Offset: 0x0037BBDC
	public unsafe IBackedArrayHandle GetMemoryUnsafe(int start, int length, out T* arrayPtr)
	{
		return this.GetMemoryUnsafeInternal(start, length, out arrayPtr, BackedArrayHandleMode.ReadWrite);
	}

	// Token: 0x06008A0B RID: 35339 RVA: 0x0037D9E8 File Offset: 0x0037BBE8
	public unsafe IBackedArrayHandle GetReadOnlyMemoryUnsafe(int start, int length, out T* arrayPtr)
	{
		return this.GetMemoryUnsafeInternal(start, length, out arrayPtr, BackedArrayHandleMode.ReadOnly);
	}

	// Token: 0x06008A0C RID: 35340 RVA: 0x0037D9F4 File Offset: 0x0037BBF4
	[PublicizedFrom(EAccessModifier.Private)]
	public IBackedArrayHandle GetSpanInternal(int start, int length, out Span<T> span, BackedArrayHandleMode mode)
	{
		span = this.m_array.AsSpan(start, length);
		return MemoryBackedArray<T>.GetStaticHandle(mode);
	}

	// Token: 0x06008A0D RID: 35341 RVA: 0x0037DA10 File Offset: 0x0037BC10
	public IBackedArrayHandle GetSpan(int start, int length, out Span<T> span)
	{
		return this.GetSpanInternal(start, length, out span, BackedArrayHandleMode.ReadWrite);
	}

	// Token: 0x06008A0E RID: 35342 RVA: 0x0037DA1C File Offset: 0x0037BC1C
	public IBackedArrayHandle GetReadOnlySpan(int start, int length, out ReadOnlySpan<T> span)
	{
		Span<T> span2;
		IBackedArrayHandle spanInternal = this.GetSpanInternal(start, length, out span2, BackedArrayHandleMode.ReadOnly);
		span = span2;
		return spanInternal;
	}

	// Token: 0x04006C14 RID: 27668
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly MemoryBackedArray<T>.MemoryBackedArrayHandle s_handleReadWrite = new MemoryBackedArray<T>.MemoryBackedArrayHandle(BackedArrayHandleMode.ReadWrite);

	// Token: 0x04006C15 RID: 27669
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly MemoryBackedArray<T>.MemoryBackedArrayHandle s_handleReadOnly = new MemoryBackedArray<T>.MemoryBackedArrayHandle(BackedArrayHandleMode.ReadOnly);

	// Token: 0x04006C16 RID: 27670
	[PublicizedFrom(EAccessModifier.Private)]
	public T[] m_array;

	// Token: 0x02001129 RID: 4393
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class MemoryBackedArrayHandle : IBackedArrayHandle, IDisposable
	{
		// Token: 0x06008A10 RID: 35344 RVA: 0x0037DA58 File Offset: 0x0037BC58
		public MemoryBackedArrayHandle(BackedArrayHandleMode mode)
		{
			this.m_mode = mode;
		}

		// Token: 0x06008A11 RID: 35345 RVA: 0x00002914 File Offset: 0x00000B14
		public void Dispose()
		{
		}

		// Token: 0x17000E67 RID: 3687
		// (get) Token: 0x06008A12 RID: 35346 RVA: 0x0037DA67 File Offset: 0x0037BC67
		public BackedArrayHandleMode Mode
		{
			get
			{
				return this.m_mode;
			}
		}

		// Token: 0x06008A13 RID: 35347 RVA: 0x00002914 File Offset: 0x00000B14
		public void Flush()
		{
		}

		// Token: 0x04006C17 RID: 27671
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly BackedArrayHandleMode m_mode;
	}

	// Token: 0x0200112A RID: 4394
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class MemoryBackedArrayUnsafeHandle : IBackedArrayHandle, IDisposable
	{
		// Token: 0x06008A14 RID: 35348 RVA: 0x0037DA6F File Offset: 0x0037BC6F
		public MemoryBackedArrayUnsafeHandle(GCHandle gcHandle, BackedArrayHandleMode mode)
		{
			this.m_gcHandle = gcHandle;
			this.m_mode = mode;
		}

		// Token: 0x06008A15 RID: 35349 RVA: 0x0037DA88 File Offset: 0x0037BC88
		[PublicizedFrom(EAccessModifier.Private)]
		public void Dispose(bool disposing)
		{
			if (!disposing)
			{
				Log.Error("MemoryBackedArrayHandle is being finalized, it should be disposed properly.");
				return;
			}
			if (this.m_gcHandle != default(GCHandle))
			{
				this.m_gcHandle.Free();
				this.m_gcHandle = default(GCHandle);
			}
		}

		// Token: 0x06008A16 RID: 35350 RVA: 0x0037DAD0 File Offset: 0x0037BCD0
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06008A17 RID: 35351 RVA: 0x0037DAE0 File Offset: 0x0037BCE0
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~MemoryBackedArrayUnsafeHandle()
		{
			this.Dispose(false);
		}

		// Token: 0x06008A18 RID: 35352 RVA: 0x0037DB10 File Offset: 0x0037BD10
		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		[PublicizedFrom(EAccessModifier.Private)]
		public void ThrowIfDisposed()
		{
			if (this.IsDisposed())
			{
				throw new ObjectDisposedException("MemoryBackedArrayUnsafeHandle has already been disposed.");
			}
		}

		// Token: 0x06008A19 RID: 35353 RVA: 0x0037DB28 File Offset: 0x0037BD28
		[PublicizedFrom(EAccessModifier.Private)]
		public bool IsDisposed()
		{
			return this.m_gcHandle == default(GCHandle);
		}

		// Token: 0x06008A1A RID: 35354 RVA: 0x0037DB49 File Offset: 0x0037BD49
		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		[PublicizedFrom(EAccessModifier.Private)]
		public void ThrowIfCannotWrite()
		{
			if (!this.m_mode.CanWrite())
			{
				throw new NotSupportedException("This MemoryBackedArrayUnsafeHandle is not writable.");
			}
		}

		// Token: 0x17000E68 RID: 3688
		// (get) Token: 0x06008A1B RID: 35355 RVA: 0x0037DB63 File Offset: 0x0037BD63
		public BackedArrayHandleMode Mode
		{
			get
			{
				return this.m_mode;
			}
		}

		// Token: 0x06008A1C RID: 35356 RVA: 0x00002914 File Offset: 0x00000B14
		public void Flush()
		{
		}

		// Token: 0x04006C18 RID: 27672
		[PublicizedFrom(EAccessModifier.Private)]
		public GCHandle m_gcHandle;

		// Token: 0x04006C19 RID: 27673
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly BackedArrayHandleMode m_mode;
	}

	// Token: 0x0200112B RID: 4395
	public sealed class MemoryBackedArrayView : IBackedArrayView<T>, IDisposable
	{
		// Token: 0x06008A1D RID: 35357 RVA: 0x0037DB6B File Offset: 0x0037BD6B
		public MemoryBackedArrayView(MemoryBackedArray<T> array, BackedArrayHandleMode mode)
		{
			this.m_array = array.m_array;
			this.m_mode = mode;
		}

		// Token: 0x06008A1E RID: 35358 RVA: 0x0037DB86 File Offset: 0x0037BD86
		[PublicizedFrom(EAccessModifier.Private)]
		public void Dispose(bool disposing)
		{
			if (!disposing)
			{
				Log.Error("MemoryBackedArrayView is being finalized, it should be disposed properly.");
				return;
			}
			this.m_array = null;
		}

		// Token: 0x06008A1F RID: 35359 RVA: 0x0037DB9D File Offset: 0x0037BD9D
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06008A20 RID: 35360 RVA: 0x0037DBAC File Offset: 0x0037BDAC
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~MemoryBackedArrayView()
		{
			this.Dispose(false);
		}

		// Token: 0x06008A21 RID: 35361 RVA: 0x0037DBDC File Offset: 0x0037BDDC
		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		[PublicizedFrom(EAccessModifier.Private)]
		public void ThrowIfDisposed()
		{
			if (this.IsDisposed())
			{
				throw new ObjectDisposedException("MemoryBackedArrayView has already been disposed.");
			}
		}

		// Token: 0x06008A22 RID: 35362 RVA: 0x0037DBF1 File Offset: 0x0037BDF1
		[PublicizedFrom(EAccessModifier.Private)]
		public bool IsDisposed()
		{
			return this.m_array == null;
		}

		// Token: 0x06008A23 RID: 35363 RVA: 0x0037DBFC File Offset: 0x0037BDFC
		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		[PublicizedFrom(EAccessModifier.Private)]
		public void ThrowIfCannotWrite()
		{
			if (!this.m_mode.CanWrite())
			{
				throw new NotSupportedException("This MemoryBackedArrayView is not writable.");
			}
		}

		// Token: 0x17000E69 RID: 3689
		// (get) Token: 0x06008A24 RID: 35364 RVA: 0x0037DC16 File Offset: 0x0037BE16
		public int Length
		{
			get
			{
				return this.m_array.Length;
			}
		}

		// Token: 0x17000E6A RID: 3690
		// (get) Token: 0x06008A25 RID: 35365 RVA: 0x0037DC20 File Offset: 0x0037BE20
		public BackedArrayHandleMode Mode
		{
			get
			{
				return this.m_mode;
			}
		}

		// Token: 0x17000E6B RID: 3691
		public T this[int i]
		{
			get
			{
				return this.m_array[i];
			}
			set
			{
				this.m_array[i] = value;
			}
		}

		// Token: 0x06008A28 RID: 35368 RVA: 0x00002914 File Offset: 0x00000B14
		public void Flush()
		{
		}

		// Token: 0x04006C1A RID: 27674
		[PublicizedFrom(EAccessModifier.Private)]
		public T[] m_array;

		// Token: 0x04006C1B RID: 27675
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly BackedArrayHandleMode m_mode;
	}
}
