using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02001168 RID: 4456
public class EnumerableDebugWrapper<T> : EnumerableDebugWrapper, IEnumerable<T>, IEnumerable
{
	// Token: 0x06008B6A RID: 35690 RVA: 0x00384620 File Offset: 0x00382820
	public EnumerableDebugWrapper(IEnumerable<T> enumerable) : this(null, enumerable)
	{
	}

	// Token: 0x06008B6B RID: 35691 RVA: 0x0038462A File Offset: 0x0038282A
	public EnumerableDebugWrapper(DebugWrapper parent, IEnumerable<T> enumerable) : base(parent, enumerable)
	{
		this.m_enumerable = enumerable;
	}

	// Token: 0x06008B6C RID: 35692 RVA: 0x0038463B File Offset: 0x0038283B
	public new IEnumerator<T> GetEnumerator()
	{
		return base.DebugEnumerator<T>(this.m_enumerable.GetEnumerator());
	}

	// Token: 0x04006CF9 RID: 27897
	[PublicizedFrom(EAccessModifier.Private)]
	public new readonly IEnumerable<T> m_enumerable;
}
