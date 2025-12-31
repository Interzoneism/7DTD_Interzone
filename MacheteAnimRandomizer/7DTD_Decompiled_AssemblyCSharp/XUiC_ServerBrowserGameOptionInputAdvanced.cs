using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DFD RID: 3581
[Preserve]
public class XUiC_ServerBrowserGameOptionInputAdvanced : XUiController
{
	// Token: 0x06007046 RID: 28742 RVA: 0x002DCF40 File Offset: 0x002DB140
	public override void Init()
	{
		base.Init();
		this.gameInfoInt = EnumUtils.Parse<GameInfoInt>(this.viewComponent.ID, false);
		this.valueField = base.GetChildById("value").GetChildByType<XUiC_TextInput>();
		this.valueField.OnChangeHandler += this.ControlText_OnChangeHandler;
	}

	// Token: 0x06007047 RID: 28743 RVA: 0x002DCF97 File Offset: 0x002DB197
	[PublicizedFrom(EAccessModifier.Private)]
	public void ControlText_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		if (!this.Parse())
		{
			this.valueField.ActiveTextColor = Color.red;
			return;
		}
		this.valueField.ActiveTextColor = Color.white;
		if (this.OnValueChanged != null)
		{
			this.OnValueChanged(this);
		}
	}

	// Token: 0x06007048 RID: 28744 RVA: 0x002DCFD8 File Offset: 0x002DB1D8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool Parse()
	{
		string name = this.gameInfoInt.ToStringCached<GameInfoInt>();
		string text = this.valueField.Text;
		if (text.Length == 0)
		{
			this.filter = new XUiC_ServersList.UiServerFilter(name, XUiC_ServersList.EnumServerLists.Regular, null, IServerListInterface.ServerFilter.EServerFilterType.Any, 0, 0, false, null);
			return true;
		}
		Match match = XUiC_ServerBrowserGameOptionInputAdvanced.rangeMatcher.Match(text);
		if (match.Success)
		{
			int minVal;
			if (!StringParsers.TryParseSInt32(match.Groups[1].Value, out minVal, 0, -1, NumberStyles.Integer))
			{
				return false;
			}
			int maxVal;
			if (!StringParsers.TryParseSInt32(match.Groups[2].Value, out maxVal, 0, -1, NumberStyles.Integer))
			{
				return false;
			}
			string value2;
			this.filter = new XUiC_ServersList.UiServerFilter(name, XUiC_ServersList.EnumServerLists.Regular, delegate(XUiC_ServersList.ListEntry _entry)
			{
				int value2 = _entry.gameServerInfo.GetValue(this.gameInfoInt);
				return value2 >= minVal && value2 <= maxVal;
			}, IServerListInterface.ServerFilter.EServerFilterType.IntRange, minVal, maxVal, false, null);
			return true;
		}
		else
		{
			match = XUiC_ServerBrowserGameOptionInputAdvanced.comparisonMatcher.Match(text);
			if (!match.Success)
			{
				return false;
			}
			string value2 = match.Groups[1].Value;
			int value;
			if (!StringParsers.TryParseSInt32(match.Groups[2].Value, out value, 0, -1, NumberStyles.Integer))
			{
				return false;
			}
			int intMinValue = 0;
			int intMaxValue = 0;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(value2);
			Func<XUiC_ServersList.ListEntry, bool> func;
			IServerListInterface.ServerFilter.EServerFilterType type;
			if (num > 990687777U)
			{
				if (num <= 2428715011U)
				{
					if (num != 2166136261U)
					{
						if (num != 2415188796U)
						{
							if (num != 2428715011U)
							{
								goto IL_385;
							}
							if (!(value2 == "!="))
							{
								goto IL_385;
							}
							goto IL_368;
						}
						else if (!(value2 == "=<"))
						{
							goto IL_385;
						}
					}
					else
					{
						if (value2 == null)
						{
							goto IL_385;
						}
						if (value2.Length != 0)
						{
							goto IL_385;
						}
						goto IL_30F;
					}
				}
				else if (num != 2431966415U)
				{
					if (num != 2448744034U)
					{
						if (num != 2499223986U)
						{
							goto IL_385;
						}
						if (!(value2 == "<="))
						{
							goto IL_385;
						}
					}
					else
					{
						if (!(value2 == "=>"))
						{
							goto IL_385;
						}
						goto IL_32C;
					}
				}
				else
				{
					if (!(value2 == "=="))
					{
						goto IL_385;
					}
					goto IL_30F;
				}
				func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) <= value);
				type = IServerListInterface.ServerFilter.EServerFilterType.IntMax;
				intMaxValue = value;
				goto IL_38B;
			}
			if (num <= 604802540U)
			{
				if (num != 284975636U)
				{
					if (num != 604802540U)
					{
						goto IL_385;
					}
					if (!(value2 == "!"))
					{
						goto IL_385;
					}
					goto IL_368;
				}
				else
				{
					if (!(value2 == ">="))
					{
						goto IL_385;
					}
					goto IL_32C;
				}
			}
			else if (num != 940354920U)
			{
				if (num != 957132539U)
				{
					if (num != 990687777U)
					{
						goto IL_385;
					}
					if (!(value2 == ">"))
					{
						goto IL_385;
					}
					func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) > value);
					type = IServerListInterface.ServerFilter.EServerFilterType.IntMin;
					intMinValue = value + 1;
					goto IL_38B;
				}
				else
				{
					if (!(value2 == "<"))
					{
						goto IL_385;
					}
					func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) < value);
					type = IServerListInterface.ServerFilter.EServerFilterType.IntMax;
					intMaxValue = value - 1;
					goto IL_38B;
				}
			}
			else if (!(value2 == "="))
			{
				goto IL_385;
			}
			IL_30F:
			func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) == value);
			type = IServerListInterface.ServerFilter.EServerFilterType.IntValue;
			intMinValue = value;
			goto IL_38B;
			IL_32C:
			func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) >= value);
			type = IServerListInterface.ServerFilter.EServerFilterType.IntMin;
			intMinValue = value;
			goto IL_38B;
			IL_368:
			func = ((XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoInt) != value);
			type = IServerListInterface.ServerFilter.EServerFilterType.IntNotValue;
			intMinValue = value;
			goto IL_38B;
			IL_385:
			throw new ArgumentOutOfRangeException();
			IL_38B:
			this.filter = new XUiC_ServersList.UiServerFilter(name, XUiC_ServersList.EnumServerLists.Regular, func, type, intMinValue, intMaxValue, false, null);
			return true;
		}
	}

	// Token: 0x06007049 RID: 28745 RVA: 0x002DD38A File Offset: 0x002DB58A
	public XUiC_ServersList.UiServerFilter GetFilter()
	{
		return this.filter;
	}

	// Token: 0x04005548 RID: 21832
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput valueField;

	// Token: 0x04005549 RID: 21833
	[PublicizedFrom(EAccessModifier.Private)]
	public GameInfoInt gameInfoInt;

	// Token: 0x0400554A RID: 21834
	public Action<XUiC_ServerBrowserGameOptionInputAdvanced> OnValueChanged;

	// Token: 0x0400554B RID: 21835
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServersList.UiServerFilter filter;

	// Token: 0x0400554C RID: 21836
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex rangeMatcher = new Regex("^\\s*(\\d+)\\s*-\\s*(\\d+)\\s*$");

	// Token: 0x0400554D RID: 21837
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex comparisonMatcher = new Regex("^\\s*(|<|<=|=|==|>=|>|!|!=)\\s*(\\d+)\\s*$");
}
