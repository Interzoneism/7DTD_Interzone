using System;
using System.Threading;

// Token: 0x0200115E RID: 4446
public sealed class AtomicCounter
{
	// Token: 0x17000E7D RID: 3709
	// (get) Token: 0x06008B2F RID: 35631 RVA: 0x00383DF8 File Offset: 0x00381FF8
	public int Value
	{
		get
		{
			return this.m_counter;
		}
	}

	// Token: 0x06008B30 RID: 35632 RVA: 0x00383E00 File Offset: 0x00382000
	public int Increment()
	{
		return Interlocked.Increment(ref this.m_counter);
	}

	// Token: 0x06008B31 RID: 35633 RVA: 0x00383E0D File Offset: 0x0038200D
	public int Decrement()
	{
		return Interlocked.Decrement(ref this.m_counter);
	}

	// Token: 0x04006CE5 RID: 27877
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_counter;
}
