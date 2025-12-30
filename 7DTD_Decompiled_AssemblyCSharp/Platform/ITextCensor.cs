using System;

namespace Platform
{
	// Token: 0x02001824 RID: 6180
	public interface ITextCensor
	{
		// Token: 0x0600B7C6 RID: 47046
		void Init(IPlatform _owner);

		// Token: 0x0600B7C7 RID: 47047
		void Update();

		// Token: 0x0600B7C8 RID: 47048
		void CensorProfanity(string _input, Action<CensoredTextResult> _censoredCallback);
	}
}
