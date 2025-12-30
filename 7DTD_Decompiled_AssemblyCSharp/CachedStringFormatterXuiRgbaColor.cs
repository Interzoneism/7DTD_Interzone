using System;
using UnityEngine;

// Token: 0x0200114C RID: 4428
public class CachedStringFormatterXuiRgbaColor : CachedStringFormatter<Color32>
{
	// Token: 0x06008AD2 RID: 35538 RVA: 0x00381E28 File Offset: 0x00380028
	public CachedStringFormatterXuiRgbaColor() : base(new Func<Color32, string>(CachedStringFormatterXuiRgbaColor.formatterFunc))
	{
		this.comparer1 = Color32EqualityComparer.Instance;
	}

	// Token: 0x06008AD3 RID: 35539 RVA: 0x00381E47 File Offset: 0x00380047
	[PublicizedFrom(EAccessModifier.Private)]
	public static string formatterFunc(Color32 _color)
	{
		return _color.ToXuiColorString();
	}
}
