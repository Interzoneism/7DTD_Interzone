using System;
using System.Collections;

// Token: 0x02001169 RID: 4457
public class EnumerableDebugWrapper : DebugWrapper, IEnumerable
{
	// Token: 0x06008B6D RID: 35693 RVA: 0x0038464E File Offset: 0x0038284E
	public EnumerableDebugWrapper(IEnumerable enumerable) : this(null, enumerable)
	{
	}

	// Token: 0x06008B6E RID: 35694 RVA: 0x00384658 File Offset: 0x00382858
	public EnumerableDebugWrapper(DebugWrapper parent, IEnumerable enumerable) : base(parent)
	{
		this.m_enumerable = enumerable;
	}

	// Token: 0x06008B6F RID: 35695 RVA: 0x00384668 File Offset: 0x00382868
	public IEnumerator GetEnumerator()
	{
		return base.DebugEnumerator(this.m_enumerable.GetEnumerator());
	}

	// Token: 0x04006CFA RID: 27898
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IEnumerable m_enumerable;
}
