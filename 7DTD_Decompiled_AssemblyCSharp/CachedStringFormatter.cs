using System;
using System.Collections.Generic;

// Token: 0x02001142 RID: 4418
public class CachedStringFormatter<T1>
{
	// Token: 0x06008AB3 RID: 35507 RVA: 0x0038196D File Offset: 0x0037FB6D
	public CachedStringFormatter(Func<T1, string> _formatterFunc)
	{
		this.formatter = _formatterFunc;
	}

	// Token: 0x06008AB4 RID: 35508 RVA: 0x00381988 File Offset: 0x0037FB88
	public string Format(T1 _v1)
	{
		bool flag;
		return this.Format(_v1, out flag);
	}

	// Token: 0x06008AB5 RID: 35509 RVA: 0x003819A0 File Offset: 0x0037FBA0
	public string Format(T1 _v1, out bool _valueChanged)
	{
		_valueChanged = (this.cachedResult == null);
		if (!this.comparer1.Equals(this.oldValue1, _v1))
		{
			this.oldValue1 = _v1;
			_valueChanged = true;
		}
		if (_valueChanged)
		{
			this.cachedResult = this.formatter(_v1);
		}
		return this.cachedResult;
	}

	// Token: 0x04006C71 RID: 27761
	[PublicizedFrom(EAccessModifier.Protected)]
	public Func<T1, string> formatter;

	// Token: 0x04006C72 RID: 27762
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedResult;

	// Token: 0x04006C73 RID: 27763
	[PublicizedFrom(EAccessModifier.Private)]
	public T1 oldValue1;

	// Token: 0x04006C74 RID: 27764
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEqualityComparer<T1> comparer1 = EqualityComparer<T1>.Default;
}
