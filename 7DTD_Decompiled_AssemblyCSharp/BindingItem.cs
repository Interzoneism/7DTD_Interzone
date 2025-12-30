using System;

// Token: 0x02000ED1 RID: 3793
public abstract class BindingItem
{
	// Token: 0x060077DB RID: 30683 RVA: 0x0030C910 File Offset: 0x0030AB10
	[PublicizedFrom(EAccessModifier.Protected)]
	public BindingItem(string _sourceText)
	{
		this.SourceText = _sourceText;
		this.FieldName = _sourceText.Substring(1, _sourceText.Length - 2);
	}

	// Token: 0x060077DC RID: 30684
	public abstract string GetValue(bool _forceAll = false);

	// Token: 0x060077DD RID: 30685 RVA: 0x0030C940 File Offset: 0x0030AB40
	[PublicizedFrom(EAccessModifier.Protected)]
	public string ParseCVars(string _fullText)
	{
		for (int num = _fullText.IndexOf("{cvar(", StringComparison.Ordinal); num != -1; num = _fullText.IndexOf("{cvar(", num, StringComparison.Ordinal))
		{
			string text = _fullText.Substring(num, _fullText.IndexOf('}', num) + 1 - num);
			string format = "";
			int num2 = text.IndexOf('(') + 1;
			string text2 = text.Substring(num2, text.IndexOf(')') - num2);
			if (text2.IndexOf(BindingItem.cvarFormatSplitChar) >= 0)
			{
				string[] array = text2.Split(BindingItem.cvarFormatSplitCharArray);
				text2 = array[0];
				format = array[1];
			}
			_fullText = _fullText.Replace(text, XUiM_Player.GetPlayer().GetCVar(text2).ToString(format));
		}
		return _fullText;
	}

	// Token: 0x04005B66 RID: 23398
	[PublicizedFrom(EAccessModifier.Protected)]
	public static readonly char cvarFormatSplitChar = ':';

	// Token: 0x04005B67 RID: 23399
	[PublicizedFrom(EAccessModifier.Protected)]
	public static readonly char[] cvarFormatSplitCharArray = new char[]
	{
		BindingItem.cvarFormatSplitChar
	};

	// Token: 0x04005B68 RID: 23400
	[PublicizedFrom(EAccessModifier.Protected)]
	public string FieldName;

	// Token: 0x04005B69 RID: 23401
	public readonly string SourceText;

	// Token: 0x04005B6A RID: 23402
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController DataContext;

	// Token: 0x04005B6B RID: 23403
	[PublicizedFrom(EAccessModifier.Protected)]
	public BindingItem.BindingTypes BindingType;

	// Token: 0x04005B6C RID: 23404
	[PublicizedFrom(EAccessModifier.Protected)]
	public string CurrentValue = "";

	// Token: 0x02000ED2 RID: 3794
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum BindingTypes
	{
		// Token: 0x04005B6E RID: 23406
		Always,
		// Token: 0x04005B6F RID: 23407
		Once,
		// Token: 0x04005B70 RID: 23408
		Complete
	}
}
