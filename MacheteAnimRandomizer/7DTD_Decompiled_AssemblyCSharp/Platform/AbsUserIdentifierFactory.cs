using System;

namespace Platform
{
	// Token: 0x020017D4 RID: 6100
	public abstract class AbsUserIdentifierFactory
	{
		// Token: 0x0600B651 RID: 46673
		public abstract PlatformUserIdentifierAbs FromId(string _userId);

		// Token: 0x0600B652 RID: 46674 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbsUserIdentifierFactory()
		{
		}
	}
}
