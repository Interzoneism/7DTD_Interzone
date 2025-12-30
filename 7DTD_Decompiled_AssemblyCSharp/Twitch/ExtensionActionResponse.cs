using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x020014F6 RID: 5366
	[Serializable]
	public class ExtensionActionResponse
	{
		// Token: 0x04008043 RID: 32835
		public List<ExtensionAction> standardActions;

		// Token: 0x04008044 RID: 32836
		public List<ExtensionBitAction> bitActions;
	}
}
