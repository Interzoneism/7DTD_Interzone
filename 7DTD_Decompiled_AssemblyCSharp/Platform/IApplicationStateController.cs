using System;

namespace Platform
{
	// Token: 0x020017DB RID: 6107
	public interface IApplicationStateController
	{
		// Token: 0x1400010B RID: 267
		// (add) Token: 0x0600B670 RID: 46704
		// (remove) Token: 0x0600B671 RID: 46705
		event IApplicationStateController.ApplicationStateChanged OnApplicationStateChanged;

		// Token: 0x1400010C RID: 268
		// (add) Token: 0x0600B672 RID: 46706
		// (remove) Token: 0x0600B673 RID: 46707
		event IApplicationStateController.NetworkStateChanged OnNetworkStateChanged;

		// Token: 0x17001490 RID: 5264
		// (get) Token: 0x0600B674 RID: 46708
		bool NetworkConnectionState { get; }

		// Token: 0x17001491 RID: 5265
		// (get) Token: 0x0600B675 RID: 46709
		ApplicationState CurrentApplicationState { get; }

		// Token: 0x0600B676 RID: 46710
		void Init(IPlatform owner);

		// Token: 0x0600B677 RID: 46711
		void Destroy();

		// Token: 0x0600B678 RID: 46712
		void Update();

		// Token: 0x020017DC RID: 6108
		// (Invoke) Token: 0x0600B67A RID: 46714
		public delegate void ApplicationStateChanged(ApplicationState newState);

		// Token: 0x020017DD RID: 6109
		// (Invoke) Token: 0x0600B67E RID: 46718
		public delegate void NetworkStateChanged(bool connectionState);
	}
}
