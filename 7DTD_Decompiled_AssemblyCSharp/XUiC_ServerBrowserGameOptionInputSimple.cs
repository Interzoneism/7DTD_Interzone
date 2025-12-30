using System;
using System.Globalization;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000E02 RID: 3586
[Preserve]
public class XUiC_ServerBrowserGameOptionInputSimple : XUiController
{
	// Token: 0x0600705D RID: 28765 RVA: 0x002DD72C File Offset: 0x002DB92C
	public override void Init()
	{
		base.Init();
		this.gameInfoInt = EnumUtils.Parse<GameInfoInt>(this.viewComponent.ID, false);
		this.value = base.GetChildById("value").GetChildByType<XUiC_TextInput>();
		this.value.OnChangeHandler += this.ControlText_OnChangeHandler;
		XUiController childById = base.GetChildById("comparison");
		childById.OnPress += this.ComparisonLabel_OnPress;
		this.comparisonLabel = (XUiV_Label)childById.ViewComponent;
	}

	// Token: 0x0600705E RID: 28766 RVA: 0x002DD7B4 File Offset: 0x002DB9B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComparisonLabel_OnPress(XUiController _sender, int _mouseButton)
	{
		this.currentComparison = this.currentComparison.CycleEnum(XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.Smaller, XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.Larger, false, true);
		switch (this.currentComparison)
		{
		case XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.Smaller:
			this.comparisonLabel.Text = "<";
			break;
		case XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.SmallerEquals:
			this.comparisonLabel.Text = "<=";
			break;
		case XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.Equals:
			this.comparisonLabel.Text = "=";
			break;
		case XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.LargerEquals:
			this.comparisonLabel.Text = ">=";
			break;
		case XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.Larger:
			this.comparisonLabel.Text = ">";
			break;
		}
		if (this.OnValueChanged != null)
		{
			this.OnValueChanged(this);
		}
	}

	// Token: 0x0600705F RID: 28767 RVA: 0x002DD865 File Offset: 0x002DBA65
	[PublicizedFrom(EAccessModifier.Private)]
	public void ControlText_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (this.OnValueChanged != null)
		{
			this.OnValueChanged(this);
		}
	}

	// Token: 0x06007060 RID: 28768 RVA: 0x002DD87C File Offset: 0x002DBA7C
	public XUiC_ServersList.UiServerFilter GetFilter()
	{
		string name = this.gameInfoInt.ToStringCached<GameInfoInt>();
		if (this.value.Text.Length == 0 || this.value.Text == "-")
		{
			return new XUiC_ServersList.UiServerFilter(name, XUiC_ServersList.EnumServerLists.Regular, null, IServerListInterface.ServerFilter.EServerFilterType.Any, 0, 0, false, null);
		}
		int filterVal = StringParsers.ParseSInt32(this.value.Text, 0, -1, NumberStyles.Integer);
		int intMinValue = 0;
		int intMaxValue = 0;
		Func<XUiC_ServersList.ListEntry, bool> func;
		IServerListInterface.ServerFilter.EServerFilterType type;
		switch (this.currentComparison)
		{
		case XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.Smaller:
			func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) < filterVal);
			type = IServerListInterface.ServerFilter.EServerFilterType.IntMax;
			intMaxValue = filterVal - 1;
			break;
		case XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.SmallerEquals:
			func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) <= filterVal);
			type = IServerListInterface.ServerFilter.EServerFilterType.IntMax;
			intMaxValue = filterVal;
			break;
		case XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.Equals:
			func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) == filterVal);
			type = IServerListInterface.ServerFilter.EServerFilterType.IntValue;
			intMinValue = filterVal;
			break;
		case XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.LargerEquals:
			func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) >= filterVal);
			type = IServerListInterface.ServerFilter.EServerFilterType.IntMin;
			intMinValue = filterVal;
			break;
		case XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.Larger:
			func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) > filterVal);
			type = IServerListInterface.ServerFilter.EServerFilterType.IntMin;
			intMinValue = filterVal + 1;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return new XUiC_ServersList.UiServerFilter(name, XUiC_ServersList.EnumServerLists.Regular, func, type, intMinValue, intMaxValue, false, null);
	}

	// Token: 0x0400555A RID: 21850
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label comparisonLabel;

	// Token: 0x0400555B RID: 21851
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput value;

	// Token: 0x0400555C RID: 21852
	[PublicizedFrom(EAccessModifier.Private)]
	public GameInfoInt gameInfoInt;

	// Token: 0x0400555D RID: 21853
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServerBrowserGameOptionInputSimple.EComparisonType currentComparison = XUiC_ServerBrowserGameOptionInputSimple.EComparisonType.Equals;

	// Token: 0x0400555E RID: 21854
	public Action<XUiC_ServerBrowserGameOptionInputSimple> OnValueChanged;

	// Token: 0x02000E03 RID: 3587
	[PublicizedFrom(EAccessModifier.Private)]
	public enum EComparisonType
	{
		// Token: 0x04005560 RID: 21856
		Smaller,
		// Token: 0x04005561 RID: 21857
		SmallerEquals,
		// Token: 0x04005562 RID: 21858
		Equals,
		// Token: 0x04005563 RID: 21859
		LargerEquals,
		// Token: 0x04005564 RID: 21860
		Larger
	}
}
