using System;

namespace Platform
{
	// Token: 0x020017F7 RID: 6135
	public interface IPlatformApi
	{
		// Token: 0x170014C4 RID: 5316
		// (get) Token: 0x0600B717 RID: 46871
		EApiStatus ClientApiStatus { get; }

		// Token: 0x14000111 RID: 273
		// (add) Token: 0x0600B718 RID: 46872
		// (remove) Token: 0x0600B719 RID: 46873
		event Action ClientApiInitialized;

		// Token: 0x0600B71A RID: 46874
		void Init(IPlatform _owner);

		// Token: 0x0600B71B RID: 46875
		bool InitClientApis();

		// Token: 0x0600B71C RID: 46876
		bool InitServerApis();

		// Token: 0x0600B71D RID: 46877
		void ServerApiLoaded();

		// Token: 0x0600B71E RID: 46878
		void Update();

		// Token: 0x0600B71F RID: 46879
		void Destroy();

		// Token: 0x0600B720 RID: 46880
		float GetScreenBoundsValueFromSystem();
	}
}
