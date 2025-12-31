using System;
using UnityEngine.Scripting;

namespace Platform.XBL
{
	// Token: 0x0200187B RID: 6267
	[Preserve]
	[UserIdentifierFactory(EPlatformIdentifier.XBL)]
	public class UserIdentifierFactory : AbsUserIdentifierFactory
	{
		// Token: 0x0600B967 RID: 47463 RVA: 0x0046D6D2 File Offset: 0x0046B8D2
		public override PlatformUserIdentifierAbs FromId(string _userId)
		{
			return new UserIdentifierXbl(_userId);
		}
	}
}
