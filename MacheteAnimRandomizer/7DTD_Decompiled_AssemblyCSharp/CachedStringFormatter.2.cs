using System;
using System.Collections.Generic;

// Token: 0x02001143 RID: 4419
public class CachedStringFormatter<T1, T2>
{
	// Token: 0x06008AB6 RID: 35510 RVA: 0x003819F2 File Offset: 0x0037FBF2
	public CachedStringFormatter(Func<T1, T2, string> _formatterFunc)
	{
		this.formatter = _formatterFunc;
	}

	// Token: 0x06008AB7 RID: 35511 RVA: 0x00381A18 File Offset: 0x0037FC18
	public string Format(T1 _v1, T2 _v2)
	{
		bool flag;
		return this.Format(_v1, _v2, out flag);
	}

	// Token: 0x06008AB8 RID: 35512 RVA: 0x00381A30 File Offset: 0x0037FC30
	public string Format(T1 _v1, T2 _v2, out bool _valueChanged)
	{
		_valueChanged = (this.cachedResult == null);
		if (!this.comparer1.Equals(this.oldValue1, _v1))
		{
			this.oldValue1 = _v1;
			_valueChanged = true;
		}
		if (!this.comparer2.Equals(this.oldValue2, _v2))
		{
			this.oldValue2 = _v2;
			_valueChanged = true;
		}
		if (_valueChanged)
		{
			this.cachedResult = this.formatter(_v1, _v2);
		}
		return this.cachedResult;
	}

	// Token: 0x04006C75 RID: 27765
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Func<T1, T2, string> formatter;

	// Token: 0x04006C76 RID: 27766
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedResult;

	// Token: 0x04006C77 RID: 27767
	[PublicizedFrom(EAccessModifier.Private)]
	public T1 oldValue1;

	// Token: 0x04006C78 RID: 27768
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEqualityComparer<T1> comparer1 = EqualityComparer<T1>.Default;

	// Token: 0x04006C79 RID: 27769
	[PublicizedFrom(EAccessModifier.Private)]
	public T2 oldValue2;

	// Token: 0x04006C7A RID: 27770
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEqualityComparer<T2> comparer2 = EqualityComparer<T2>.Default;
}
