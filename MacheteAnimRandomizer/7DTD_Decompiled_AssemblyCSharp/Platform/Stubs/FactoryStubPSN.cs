using System;
using UnityEngine.Scripting;

namespace Platform.Stubs
{
	// Token: 0x020018A9 RID: 6313
	[Preserve]
	[PlatformFactory(EPlatformIdentifier.PSN)]
	public class FactoryStubPSN : AbsPlatform
	{
		// Token: 0x0600BA5F RID: 47711 RVA: 0x0047104C File Offset: 0x0046F24C
		public override void CreateInstances()
		{
			if (!base.AsServerOnly)
			{
				throw new NotSupportedException("This platform can only be used as a server platform.");
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform == null || crossplatformPlatform.PlatformIdentifier != EPlatformIdentifier.EOS)
			{
				throw new NotSupportedException("This server platform requires EOS as the cross-platform.");
			}
		}
	}
}
