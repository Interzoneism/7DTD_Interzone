using System;

// Token: 0x02000ED4 RID: 3796
public class BindingItemCvar : BindingItem
{
	// Token: 0x060077E2 RID: 30690 RVA: 0x0030CB54 File Offset: 0x0030AD54
	public BindingItemCvar(BindingInfo _parent, XUiView _view, string _sourceText) : base(_sourceText)
	{
		this.FieldName = this.FieldName.Replace("cvar(", "").Replace(")", "");
		if (this.FieldName.IndexOf(BindingItem.cvarFormatSplitChar) >= 0)
		{
			string[] array = this.FieldName.Split(BindingItem.cvarFormatSplitCharArray);
			this.FieldName = array[0];
			this.format = array[1];
		}
		for (XUiController xuiController = _view.Controller; xuiController != null; xuiController = xuiController.Parent)
		{
			if (xuiController.GetType() != typeof(XUiController))
			{
				this.DataContext = xuiController;
				this.DataContext.AddBinding(_parent);
				return;
			}
		}
	}

	// Token: 0x060077E3 RID: 30691 RVA: 0x0030CC08 File Offset: 0x0030AE08
	public override string GetValue(bool _forceAll = false)
	{
		if (this.BindingType == BindingItem.BindingTypes.Complete && !_forceAll)
		{
			return this.CurrentValue;
		}
		this.CurrentValue = XUiM_Player.GetPlayer().GetCVar(this.FieldName).ToString(this.format);
		if (this.BindingType == BindingItem.BindingTypes.Once)
		{
			this.BindingType = BindingItem.BindingTypes.Complete;
		}
		return this.CurrentValue;
	}

	// Token: 0x04005B72 RID: 23410
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string format;
}
