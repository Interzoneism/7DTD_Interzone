using System;

namespace Platform
{
	// Token: 0x0200183E RID: 6206
	public interface IVirtualKeyboard
	{
		// Token: 0x0600B825 RID: 47141
		void Init(IPlatform _owner);

		// Token: 0x0600B826 RID: 47142
		string Open(string _title, string _defaultText, Action<bool, string> _onTextReceived, UIInput.InputType _mode = UIInput.InputType.Standard, bool _multiLine = false, uint singleLineLength = 200U);

		// Token: 0x0600B827 RID: 47143
		void Destroy();
	}
}
