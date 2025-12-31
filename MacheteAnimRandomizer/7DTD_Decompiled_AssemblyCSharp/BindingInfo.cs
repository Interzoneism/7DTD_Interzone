using System;
using System.Collections.Generic;

// Token: 0x02000ED0 RID: 3792
public class BindingInfo
{
	// Token: 0x060077D9 RID: 30681 RVA: 0x0030C610 File Offset: 0x0030A810
	public BindingInfo(XUiView _view, string _property, string _sourceText)
	{
		this.view = _view;
		this.propertyName = _property;
		this.SourceText = _sourceText;
		int num2;
		for (int num = this.SourceText.IndexOf("{", StringComparison.Ordinal); num != -1; num = this.SourceText.IndexOf("{", num2, StringComparison.Ordinal))
		{
			num2 = this.SourceText.IndexOf("}", num, StringComparison.Ordinal);
			if (num2 == -1)
			{
				return;
			}
			string text = this.SourceText.Substring(num, num2 - num + 1);
			bool flag = false;
			for (int i = 0; i < this.bindingList.Count; i++)
			{
				if (this.bindingList[i].SourceText == text)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				BindingItem item;
				if (text.StartsWith("{cvar("))
				{
					item = new BindingItemCvar(this, this.view, text);
				}
				else if (text.StartsWith("{#"))
				{
					item = new BindingItemNcalc(this, this.view, text);
				}
				else
				{
					item = new BindingItemStandard(this, this.view, text);
				}
				this.bindingList.Add(item);
			}
		}
	}

	// Token: 0x060077DA RID: 30682 RVA: 0x0030C744 File Offset: 0x0030A944
	public void RefreshValue(bool _forceAll = false)
	{
		bool flag = this.cachedResultValue == null;
		for (int i = 0; i < this.bindingList.Count; i++)
		{
			string value = this.bindingList[i].GetValue(_forceAll);
			if (i < this.cachedBindingValues.Count)
			{
				flag |= !string.Equals(this.cachedBindingValues[i], value, StringComparison.Ordinal);
				this.cachedBindingValues[i] = value;
			}
			else
			{
				flag = true;
				this.cachedBindingValues.Add(value);
			}
		}
		if (flag)
		{
			string text = this.SourceText;
			if (this.bindingList.Count == 1 && text.Equals(this.bindingList[0].SourceText, StringComparison.Ordinal))
			{
				text = (this.cachedBindingValues[0] ?? "");
			}
			else
			{
				for (int j = 0; j < this.bindingList.Count; j++)
				{
					BindingItem bindingItem = this.bindingList[j];
					text = text.Replace(bindingItem.SourceText, this.cachedBindingValues[j]);
				}
			}
			this.cachedResultValue = text;
		}
		try
		{
			this.view.ParseAttributeViewAndController(this.propertyName, this.cachedResultValue, this.view.Controller.Parent, false);
		}
		catch (Exception e)
		{
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Exception parsing result of binding. Attribute: '",
				this.propertyName,
				"', binding string: '",
				this.SourceText,
				"', binding result: '",
				this.cachedResultValue,
				"', view hierarchy: ",
				this.view.Controller.GetXuiHierarchy(),
				":"
			}));
			Log.Exception(e);
		}
	}

	// Token: 0x04005B60 RID: 23392
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly XUiView view;

	// Token: 0x04005B61 RID: 23393
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string propertyName;

	// Token: 0x04005B62 RID: 23394
	public readonly string SourceText;

	// Token: 0x04005B63 RID: 23395
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<BindingItem> bindingList = new List<BindingItem>();

	// Token: 0x04005B64 RID: 23396
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<string> cachedBindingValues = new List<string>();

	// Token: 0x04005B65 RID: 23397
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedResultValue;
}
