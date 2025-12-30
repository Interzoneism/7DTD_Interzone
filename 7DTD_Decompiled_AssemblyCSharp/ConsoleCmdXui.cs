using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200027D RID: 637
[Preserve]
public class ConsoleCmdXui : ConsoleCmdAbstract
{
	// Token: 0x06001215 RID: 4629 RVA: 0x000713CA File Offset: 0x0006F5CA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"xui"
		};
	}

	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x06001216 RID: 4630 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x06001217 RID: 4631 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x000713DA File Offset: 0x0006F5DA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Execute XUi operations";
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x000713E1 File Offset: 0x0006F5E1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\n   xui open <window group name> [instance] [closeOthers]\n   xui close <window group name> [instance]\n   xui reload [window group name] [instance]\n   xui list <\"instances\" / \"windows\"> [instance]\n";
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x000713E8 File Offset: 0x0006F5E8
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No sub command given.");
			return;
		}
		if (_params[0].EqualsCaseInsensitive("open"))
		{
			this.ExecuteOpen(_params);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("close"))
		{
			this.ExecuteClose(_params);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("reload"))
		{
			this.ExecuteReload(_params);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("list"))
		{
			this.ExecuteList(_params);
			return;
		}
		if (_params[0].EqualsCaseInsensitive("limits"))
		{
			this.ExecuteLimits();
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Invalid sub command \"" + _params[0] + "\".");
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x000714B8 File Offset: 0x0006F6B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteOpen(List<string> _params)
	{
		if (_params.Count < 2 || _params.Count > 4)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 2 to 4, found " + _params.Count.ToString() + ".");
			return;
		}
		string text = _params[1];
		bool bModal = true;
		int num;
		XUi xuiInstance = this.getXuiInstance(_params, 2, out num);
		if (xuiInstance == null)
		{
			return;
		}
		if (_params.Count > 3)
		{
			bModal = ConsoleHelper.ParseParamBool(_params[3], false);
		}
		text = this.getXuiWindow(xuiInstance, text);
		if (text != null)
		{
			xuiInstance.playerUI.windowManager.Open(text, bModal, false, true);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("XUi window group \"" + text + "\" opened.");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("XUi window group \"" + _params[1] + "\" does not exist.");
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x00071594 File Offset: 0x0006F794
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteClose(List<string> _params)
	{
		if (_params.Count < 2 || _params.Count > 3)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 2 to 3, found " + _params.Count.ToString() + ".");
			return;
		}
		string text = _params[1];
		int num;
		XUi xuiInstance = this.getXuiInstance(_params, 2, out num);
		if (xuiInstance == null)
		{
			return;
		}
		text = this.getXuiWindow(xuiInstance, text);
		if (text != null)
		{
			xuiInstance.playerUI.windowManager.Close(text);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("XUi window group \"" + text + "\" closed.");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("XUi window group \"" + _params[1] + "\" does not exist.");
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x00071650 File Offset: 0x0006F850
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteReload(List<string> _params)
	{
		if (_params.Count > 3)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Wrong number of arguments, expected 1 to 3, found " + _params.Count.ToString() + ".");
			return;
		}
		string text = "*";
		if (_params.Count > 1)
		{
			text = _params[1];
		}
		int num;
		XUi xuiInstance = this.getXuiInstance(_params, 2, out num);
		if (xuiInstance == null)
		{
			return;
		}
		if (text == "*")
		{
			XUi.Reload(xuiInstance.playerUI);
			xuiInstance.SetDataConnections();
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("XUi windows reloaded.");
			return;
		}
		text = this.getXuiWindow(xuiInstance, text);
		if (text != null)
		{
			XUi.ReloadWindow(xuiInstance.playerUI, text);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("XUi window group \"" + text + "\" reloaded.");
			return;
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("XUi window group \"" + _params[1] + "\" does not exist.");
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x0007173C File Offset: 0x0006F93C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteList(List<string> _params)
	{
		if (_params.Count < 2)
		{
			_params.Add("windows");
		}
		if (_params[1].EqualsCaseInsensitive("instances"))
		{
			XUi[] array = this.xuiInstances();
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Loaded XUi instances:");
			for (int i = 0; i < array.Length; i++)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("  " + i.ToString() + ". " + array[i].playerUI.windowManager.gameObject.name);
			}
			return;
		}
		if (!_params[1].EqualsCaseInsensitive("windows"))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Argument 2 has to be either \"instances\" or \"windows\".");
			return;
		}
		int num;
		XUi xuiInstance = this.getXuiInstance(_params, 2, out num);
		if (xuiInstance == null)
		{
			return;
		}
		List<string> list = new List<string>();
		for (int j = 0; j < xuiInstance.WindowGroups.Count; j++)
		{
			list.Add(xuiInstance.WindowGroups[j].ID);
		}
		list.Sort();
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Concat(new string[]
		{
			"Loaded XUi window groups in instance ",
			num.ToString(),
			" (\"",
			xuiInstance.playerUI.gameObject.name,
			"\"):"
		}));
		for (int k = 0; k < list.Count; k++)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("  " + list[k]);
		}
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x000718C8 File Offset: 0x0006FAC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void ExecuteLimits()
	{
		XUi xui = this.xuiInstances()[0];
		string xuiWindow = this.getXuiWindow(xui, "uiLimitsTest");
		if (xuiWindow == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("XUi window group \"uiLimitsTest\" does not exist.");
			return;
		}
		if (xui.playerUI.windowManager.IsWindowOpen(xuiWindow))
		{
			xui.playerUI.windowManager.Close(xuiWindow);
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("XUi window group \"" + xuiWindow + "\" closed.");
			return;
		}
		xui.playerUI.windowManager.Open(xuiWindow, false, false, true);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("XUi window group \"" + xuiWindow + "\" opened.");
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x0007196C File Offset: 0x0006FB6C
	[PublicizedFrom(EAccessModifier.Private)]
	public XUi getXuiInstance(List<string> _params, int _index, out int _xuiInstanceId)
	{
		XUi[] array = this.xuiInstances();
		_xuiInstanceId = array.Length - 1;
		if (_params.Count > _index)
		{
			if (!StringParsers.TryParseSInt32(_params[_index], out _xuiInstanceId, 0, -1, NumberStyles.Integer))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[_index] + "\" is not a valid integer.");
				return null;
			}
			if (_xuiInstanceId >= array.Length)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("XUi instance " + _xuiInstanceId.ToString() + " does not exist.");
				return null;
			}
		}
		return array[_xuiInstanceId];
	}

	// Token: 0x06001221 RID: 4641 RVA: 0x000719F0 File Offset: 0x0006FBF0
	[PublicizedFrom(EAccessModifier.Private)]
	public string getXuiWindow(XUi _xuiInstance, string _name)
	{
		for (int i = 0; i < _xuiInstance.WindowGroups.Count; i++)
		{
			if (_xuiInstance.WindowGroups[i].ID.EqualsCaseInsensitive(_name))
			{
				return _xuiInstance.WindowGroups[i].ID;
			}
		}
		return null;
	}

	// Token: 0x06001222 RID: 4642 RVA: 0x00071A3F File Offset: 0x0006FC3F
	[PublicizedFrom(EAccessModifier.Private)]
	public XUi[] xuiInstances()
	{
		XUi[] array = UnityEngine.Object.FindObjectsOfType<XUi>();
		Array.Sort<XUi>(array, (XUi _x, XUi _y) => string.Compare(_x.playerUI.windowManager.gameObject.name, _y.playerUI.windowManager.gameObject.name, StringComparison.Ordinal));
		return array;
	}
}
