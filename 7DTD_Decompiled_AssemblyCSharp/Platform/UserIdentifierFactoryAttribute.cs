using System;

namespace Platform
{
	// Token: 0x0200186B RID: 6251
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class UserIdentifierFactoryAttribute : Attribute
	{
		// Token: 0x0600B928 RID: 47400 RVA: 0x0046C804 File Offset: 0x0046AA04
		public UserIdentifierFactoryAttribute(EPlatformIdentifier _targetPlatform)
		{
			this.TargetPlatform = _targetPlatform;
		}

		// Token: 0x040090D0 RID: 37072
		public readonly EPlatformIdentifier TargetPlatform;
	}
}
