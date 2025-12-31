using System;
using UnityEngine.Scripting;

namespace Platform.LAN
{
	// Token: 0x020018F2 RID: 6386
	[Preserve]
	[PlatformFactory(EPlatformIdentifier.LAN)]
	public class Factory : AbsPlatform
	{
		// Token: 0x0600BCBE RID: 48318 RVA: 0x004787CD File Offset: 0x004769CD
		public override void CreateInstances()
		{
			if (!base.AsServerOnly)
			{
				throw new NotSupportedException("This platform can only be used as a server platform.");
			}
			base.ServerListAnnouncer = new LANMasterServerAnnouncer();
		}
	}
}
