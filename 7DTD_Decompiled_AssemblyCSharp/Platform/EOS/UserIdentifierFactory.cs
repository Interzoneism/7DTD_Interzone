using System;
using UnityEngine.Scripting;

namespace Platform.EOS
{
	// Token: 0x0200194D RID: 6477
	[Preserve]
	[UserIdentifierFactory(EPlatformIdentifier.EOS)]
	public class UserIdentifierFactory : AbsUserIdentifierFactory
	{
		// Token: 0x0600BEF7 RID: 48887 RVA: 0x004862CC File Offset: 0x004844CC
		public override PlatformUserIdentifierAbs FromId(string _userId)
		{
			return new UserIdentifierEos(_userId);
		}
	}
}
