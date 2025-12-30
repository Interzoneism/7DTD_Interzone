using System;

namespace Platform
{
	// Token: 0x02001847 RID: 6215
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class PlatformFactoryAttribute : Attribute
	{
		// Token: 0x0600B85B RID: 47195 RVA: 0x00469714 File Offset: 0x00467914
		public PlatformFactoryAttribute(EPlatformIdentifier _targetPlatform)
		{
			this.TargetPlatform = _targetPlatform;
		}

		// Token: 0x0400904E RID: 36942
		public readonly EPlatformIdentifier TargetPlatform;
	}
}
