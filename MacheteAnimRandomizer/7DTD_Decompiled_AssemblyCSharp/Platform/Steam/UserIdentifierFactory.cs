using System;
using UnityEngine.Scripting;

namespace Platform.Steam
{
	// Token: 0x020018D1 RID: 6353
	[Preserve]
	[UserIdentifierFactory(EPlatformIdentifier.Steam)]
	public class UserIdentifierFactory : AbsUserIdentifierFactory
	{
		// Token: 0x0600BBA1 RID: 48033 RVA: 0x00476360 File Offset: 0x00474560
		public override PlatformUserIdentifierAbs FromId(string _userId)
		{
			return new UserIdentifierSteam(_userId);
		}
	}
}
