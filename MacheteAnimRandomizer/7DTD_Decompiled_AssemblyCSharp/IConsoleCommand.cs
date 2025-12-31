using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000280 RID: 640
[Preserve]
public interface IConsoleCommand
{
	// Token: 0x170001EA RID: 490
	// (get) Token: 0x0600122F RID: 4655
	bool IsExecuteOnClient { get; }

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x06001230 RID: 4656
	int DefaultPermissionLevel { get; }

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x06001231 RID: 4657
	bool AllowedInMainMenu { get; }

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x06001232 RID: 4658
	DeviceFlag AllowedDeviceTypes { get; }

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x06001233 RID: 4659
	DeviceFlag AllowedDeviceTypesClient { get; }

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x06001234 RID: 4660 RVA: 0x00071BF0 File Offset: 0x0006FDF0
	bool CanExecuteForDevice
	{
		get
		{
			if (!Submission.Enabled)
			{
				return true;
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				return this.AllowedDeviceTypesClient.IsCurrent();
			}
			return this.AllowedDeviceTypes.IsCurrent();
		}
	}

	// Token: 0x06001235 RID: 4661
	string[] GetCommands();

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x06001236 RID: 4662
	string PrimaryCommand { get; }

	// Token: 0x06001237 RID: 4663
	string GetDescription();

	// Token: 0x06001238 RID: 4664
	string GetHelp();

	// Token: 0x06001239 RID: 4665
	void Execute(List<string> _params, CommandSenderInfo _senderInfo);
}
