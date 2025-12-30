using System;
using System.Runtime.CompilerServices;

// Token: 0x02001125 RID: 4389
public interface IBackedArray<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
{
	// Token: 0x17000E61 RID: 3681
	// (get) Token: 0x060089F1 RID: 35313
	int Length { get; }

	// Token: 0x060089F2 RID: 35314
	IBackedArrayHandle GetMemory(int start, int length, out Memory<T> memory);

	// Token: 0x060089F3 RID: 35315
	IBackedArrayHandle GetReadOnlyMemory(int start, int length, out ReadOnlyMemory<T> memory);

	// Token: 0x060089F4 RID: 35316
	unsafe IBackedArrayHandle GetMemoryUnsafe(int start, int length, out T* arrayPtr);

	// Token: 0x060089F5 RID: 35317
	unsafe IBackedArrayHandle GetReadOnlyMemoryUnsafe(int start, int length, out T* arrayPtr);

	// Token: 0x060089F6 RID: 35318
	IBackedArrayHandle GetSpan(int start, int length, out Span<T> span);

	// Token: 0x060089F7 RID: 35319
	IBackedArrayHandle GetReadOnlySpan(int start, int length, out ReadOnlySpan<T> span);
}
