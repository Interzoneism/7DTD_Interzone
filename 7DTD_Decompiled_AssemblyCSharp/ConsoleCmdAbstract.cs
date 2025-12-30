using System;
using System.Collections.Generic;
using Platform;

// Token: 0x020001AC RID: 428
public abstract class ConsoleCmdAbstract : IConsoleCommand
{
	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000D16 RID: 3350 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsExecuteOnClient
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000D17 RID: 3351 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual int DefaultPermissionLevel
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000D18 RID: 3352 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool AllowedInMainMenu
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000D19 RID: 3353 RVA: 0x000583BD File Offset: 0x000565BD
	public virtual DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX;
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000D1A RID: 3354 RVA: 0x000583BD File Offset: 0x000565BD
	public virtual DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX;
		}
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public ConsoleCmdAbstract()
	{
	}

	// Token: 0x06000D1C RID: 3356
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract string[] getCommands();

	// Token: 0x06000D1D RID: 3357 RVA: 0x000583C0 File Offset: 0x000565C0
	public virtual string[] GetCommands()
	{
		string[] result;
		if ((result = this.commandNamesCache) == null)
		{
			result = (this.commandNamesCache = this.getCommands());
		}
		return result;
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000D1E RID: 3358 RVA: 0x000583E8 File Offset: 0x000565E8
	public virtual string PrimaryCommand
	{
		get
		{
			string result;
			if ((result = this.primaryCommand) == null)
			{
				result = (this.primaryCommand = this.GetCommands()[0]);
			}
			return result;
		}
	}

	// Token: 0x06000D1F RID: 3359
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract string getDescription();

	// Token: 0x06000D20 RID: 3360 RVA: 0x00058410 File Offset: 0x00056610
	public virtual string GetDescription()
	{
		string result;
		if ((result = this.commandDescriptionCache) == null)
		{
			result = (this.commandDescriptionCache = this.getDescription());
		}
		return result;
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x00019766 File Offset: 0x00017966
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string getHelp()
	{
		return null;
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x00058438 File Offset: 0x00056638
	public virtual string GetHelp()
	{
		string result;
		if ((result = this.commandHelpCache) == null)
		{
			result = (this.commandHelpCache = this.getHelp());
		}
		return result;
	}

	// Token: 0x06000D23 RID: 3363
	public abstract void Execute(List<string> _params, CommandSenderInfo _senderInfo);

	// Token: 0x04000ACE RID: 2766
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] commandNamesCache;

	// Token: 0x04000ACF RID: 2767
	[PublicizedFrom(EAccessModifier.Private)]
	public string commandDescriptionCache;

	// Token: 0x04000AD0 RID: 2768
	[PublicizedFrom(EAccessModifier.Private)]
	public string commandHelpCache;

	// Token: 0x04000AD1 RID: 2769
	[PublicizedFrom(EAccessModifier.Private)]
	public string primaryCommand;
}
