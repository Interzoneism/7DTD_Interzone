using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000B55 RID: 2901
[NativeContainer]
public struct NativeSafeHandle<T> : IDisposable where T : struct, IDisposable
{
	// Token: 0x17000934 RID: 2356
	// (get) Token: 0x06005A4E RID: 23118 RVA: 0x00243F62 File Offset: 0x00242162
	public T Target
	{
		get
		{
			return this.target;
		}
	}

	// Token: 0x06005A4F RID: 23119 RVA: 0x00243F6A File Offset: 0x0024216A
	public NativeSafeHandle(ref T _target, Allocator allocator)
	{
		this.target = _target;
	}

	// Token: 0x06005A50 RID: 23120 RVA: 0x00243F78 File Offset: 0x00242178
	public void Dispose()
	{
		this.target.Dispose();
	}

	// Token: 0x04004518 RID: 17688
	[PublicizedFrom(EAccessModifier.Private)]
	public T target;
}
