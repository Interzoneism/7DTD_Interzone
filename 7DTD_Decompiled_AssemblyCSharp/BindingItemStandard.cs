using System;

// Token: 0x02000ED3 RID: 3795
public class BindingItemStandard : BindingItem
{
	// Token: 0x060077DF RID: 30687 RVA: 0x0030CA0C File Offset: 0x0030AC0C
	public BindingItemStandard(BindingInfo _parent, XUiView _view, string _sourceText) : base(_sourceText)
	{
		string[] array = this.FieldName.Split(BindingItemStandard.bindingTypeSplitChar);
		if (array.Length > 1)
		{
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i].EqualsCaseInsensitive("once"))
				{
					this.BindingType = BindingItem.BindingTypes.Once;
				}
			}
			this.FieldName = array[0];
		}
		for (XUiController xuiController = _view.Controller; xuiController != null; xuiController = xuiController.Parent)
		{
			if (xuiController.GetType() != typeof(XUiController))
			{
				this.DataContext = xuiController;
				string text = "";
				if (this.DataContext.GetBindingValue(ref text, this.FieldName))
				{
					this.DataContext.AddBinding(_parent);
					return;
				}
			}
		}
	}

	// Token: 0x060077E0 RID: 30688 RVA: 0x0030CAC0 File Offset: 0x0030ACC0
	public override string GetValue(bool _forceAll = false)
	{
		if (this.BindingType == BindingItem.BindingTypes.Complete && !_forceAll)
		{
			return this.CurrentValue;
		}
		if (!this.DataContext.GetBindingValue(ref this.CurrentValue, this.FieldName))
		{
			return this.CurrentValue;
		}
		if (this.CurrentValue != null && this.CurrentValue.Contains("{cvar("))
		{
			this.CurrentValue = base.ParseCVars(this.CurrentValue);
		}
		if (this.BindingType == BindingItem.BindingTypes.Once)
		{
			this.BindingType = BindingItem.BindingTypes.Complete;
		}
		return this.CurrentValue;
	}

	// Token: 0x04005B71 RID: 23409
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] bindingTypeSplitChar = new char[]
	{
		'|'
	};
}
