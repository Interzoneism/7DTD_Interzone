using System;
using System.Runtime.CompilerServices;

// Token: 0x020009F8 RID: 2552
public abstract class PinnedBuffer : IDisposable
{
	// Token: 0x06004E29 RID: 20009 RVA: 0x001EDF70 File Offset: 0x001EC170
	public static PinnedBuffer<T> Create<[IsUnmanaged] T>(ArrayListMP<T> list, bool copy) where T : struct, ValueType
	{
		if (list == null || list.Count <= 0)
		{
			return new PinnedBuffer<T>();
		}
		if (!copy)
		{
			return new PinnedBuffer<T>(list.Items, list.Count);
		}
		PinnedBuffer<T> pinnedBuffer = new PinnedBuffer<T>(list.pool, list.Count);
		list.Items.AsSpan(0, list.Count).CopyTo(pinnedBuffer.AsSpan());
		return pinnedBuffer;
	}

	// Token: 0x06004E2A RID: 20010
	public abstract void Dispose();

	// Token: 0x06004E2B RID: 20011 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public PinnedBuffer()
	{
	}
}
