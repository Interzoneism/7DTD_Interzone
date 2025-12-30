using System;
using UnityEngine.Scripting;

namespace Platform.Local
{
	// Token: 0x020018F0 RID: 6384
	[Preserve]
	[UserIdentifierFactory(EPlatformIdentifier.Local)]
	public class UserIdentifierFactory : AbsUserIdentifierFactory
	{
		// Token: 0x0600BCB4 RID: 48308 RVA: 0x004786F6 File Offset: 0x004768F6
		public override PlatformUserIdentifierAbs FromId(string _userId)
		{
			return new UserIdentifierLocal(_userId);
		}
	}
}
