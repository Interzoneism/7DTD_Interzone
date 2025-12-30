using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x0200030D RID: 781
[Preserve]
public class AdminSpeedConsoleCmd : ConsoleCmdAbstract
{
	// Token: 0x17000273 RID: 627
	// (get) Token: 0x0600161C RID: 5660 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000274 RID: 628
	// (get) Token: 0x0600161D RID: 5661 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x0600161E RID: 5662 RVA: 0x000812BC File Offset: 0x0007F4BC
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		this.parameters = _params;
		string s = (_params.Count == 0) ? "nope" : _params[0].ToLower();
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		float godModeSpeedModifier = 0f;
		if (!float.TryParse(s, out godModeSpeedModifier))
		{
			godModeSpeedModifier = (float)((primaryPlayer.GodModeSpeedModifier == 15f) ? 5 : 15);
		}
		primaryPlayer.GodModeSpeedModifier = godModeSpeedModifier;
		Log.Out("Admin speed: " + godModeSpeedModifier.ToString());
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x0008133B File Offset: 0x0007F53B
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetParam(List<string> _params, int index)
	{
		if (_params == null)
		{
			return null;
		}
		if (index >= _params.Count)
		{
			return null;
		}
		return _params[index];
	}

	// Token: 0x06001620 RID: 5664 RVA: 0x00081354 File Offset: 0x0007F554
	[PublicizedFrom(EAccessModifier.Private)]
	public int GetParamAsInt(int index)
	{
		if (this.parameters == null)
		{
			return -1;
		}
		if (index >= this.parameters.Count)
		{
			return -1;
		}
		int result = -1;
		int.TryParse(this.parameters[index], out result);
		return result;
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x00081392 File Offset: 0x0007F592
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			AdminSpeedConsoleCmd.info,
			"as"
		};
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x000813AA File Offset: 0x0007F5AA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return AdminSpeedConsoleCmd.info;
	}

	// Token: 0x04000DF8 RID: 3576
	[PublicizedFrom(EAccessModifier.Private)]
	public static string info = "AdminSpeed";

	// Token: 0x04000DF9 RID: 3577
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> parameters;
}
