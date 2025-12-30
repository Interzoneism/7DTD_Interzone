using System;
using System.Text;

namespace Platform
{
	// Token: 0x02001800 RID: 6144
	// (Invoke) Token: 0x0600B733 RID: 46899
	public delegate void PlatformMemoryRenderDelta<in T>(StringBuilder builder, T current, T last);
}
