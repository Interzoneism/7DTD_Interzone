using System;
using InControl;

namespace Platform
{
	// Token: 0x0200183D RID: 6205
	public interface IUtils
	{
		// Token: 0x0600B81C RID: 47132
		void Init(IPlatform _owner);

		// Token: 0x0600B81D RID: 47133
		bool OpenBrowser(string _url);

		// Token: 0x0600B81E RID: 47134
		void ControllerDisconnected(InputDevice inputDevice);

		// Token: 0x0600B81F RID: 47135
		string GetPlatformLanguage();

		// Token: 0x0600B820 RID: 47136
		string GetAppLanguage();

		// Token: 0x0600B821 RID: 47137
		string GetCountry();

		// Token: 0x0600B822 RID: 47138
		void ClearTempFiles();

		// Token: 0x0600B823 RID: 47139
		string GetTempFileName(string prefix = "", string suffix = "");

		// Token: 0x0600B824 RID: 47140
		string GetCrossplayPlayerIcon(EPlayGroup _playGroup, bool _fetchGenericIcons, EPlatformIdentifier _nativePlatform = EPlatformIdentifier.None);
	}
}
