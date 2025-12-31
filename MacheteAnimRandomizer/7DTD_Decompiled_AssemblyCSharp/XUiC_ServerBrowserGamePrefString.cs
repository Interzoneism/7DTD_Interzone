using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000E10 RID: 3600
[Preserve]
public class XUiC_ServerBrowserGamePrefString : XUiController, IServerBrowserFilterControl
{
	// Token: 0x17000B5E RID: 2910
	// (get) Token: 0x060070B8 RID: 28856 RVA: 0x002DFBDC File Offset: 0x002DDDDC
	public GameInfoString GameInfoString
	{
		get
		{
			return this.gameInfoString;
		}
	}

	// Token: 0x060070B9 RID: 28857 RVA: 0x002DFBE4 File Offset: 0x002DDDE4
	public override void Init()
	{
		base.Init();
		this.gameInfoString = EnumUtils.Parse<GameInfoString>(this.viewComponent.ID, false);
		this.value = base.GetChildById("value").GetChildByType<XUiC_TextInput>();
		this.value.OnChangeHandler += this.ControlText_OnChangeHandler;
	}

	// Token: 0x060070BA RID: 28858 RVA: 0x002DFC3B File Offset: 0x002DDE3B
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetGameInfoName()
	{
		return this.gameInfoString.ToStringCached<GameInfoString>();
	}

	// Token: 0x060070BB RID: 28859 RVA: 0x002DFC48 File Offset: 0x002DDE48
	[PublicizedFrom(EAccessModifier.Private)]
	public void ControlText_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		Action<IServerBrowserFilterControl> onValueChanged = this.OnValueChanged;
		if (onValueChanged == null)
		{
			return;
		}
		onValueChanged(this);
	}

	// Token: 0x060070BC RID: 28860 RVA: 0x002DFC5B File Offset: 0x002DDE5B
	public void Reset()
	{
		this.value.Text = "";
		Action<IServerBrowserFilterControl> onValueChanged = this.OnValueChanged;
		if (onValueChanged == null)
		{
			return;
		}
		onValueChanged(this);
	}

	// Token: 0x060070BD RID: 28861 RVA: 0x002DFC7E File Offset: 0x002DDE7E
	public void SetValue(string _value)
	{
		this.value.Text = _value;
		Action<IServerBrowserFilterControl> onValueChanged = this.OnValueChanged;
		if (onValueChanged == null)
		{
			return;
		}
		onValueChanged(this);
	}

	// Token: 0x060070BE RID: 28862 RVA: 0x002DFC9D File Offset: 0x002DDE9D
	public string GetValue()
	{
		return this.value.Text;
	}

	// Token: 0x060070BF RID: 28863 RVA: 0x002DFCAC File Offset: 0x002DDEAC
	public XUiC_ServersList.UiServerFilter GetFilter()
	{
		string name = this.gameInfoString.ToStringCached<GameInfoString>();
		string input = this.value.Text.Trim();
		if (input.Length == 0)
		{
			return new XUiC_ServersList.UiServerFilter(name, XUiC_ServersList.EnumServerLists.Regular, null, IServerListInterface.ServerFilter.EServerFilterType.Any, 0, 0, false, null);
		}
		Func<XUiC_ServersList.ListEntry, bool> func = (XUiC_ServersList.ListEntry _entry) => _entry.gameServerInfo.GetValue(this.gameInfoString).ContainsCaseInsensitive(input);
		return new XUiC_ServersList.UiServerFilter(name, XUiC_ServersList.EnumServerLists.Regular, func, IServerListInterface.ServerFilter.EServerFilterType.StringContains, 0, 0, false, input);
	}

	// Token: 0x040055AF RID: 21935
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput value;

	// Token: 0x040055B0 RID: 21936
	[PublicizedFrom(EAccessModifier.Private)]
	public GameInfoString gameInfoString;

	// Token: 0x040055B1 RID: 21937
	public Action<IServerBrowserFilterControl> OnValueChanged;
}
