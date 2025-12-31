using System;
using System.Text;

namespace Platform
{
	// Token: 0x020017FD RID: 6141
	public interface IPlatformMemoryStat
	{
		// Token: 0x170014C6 RID: 5318
		// (get) Token: 0x0600B727 RID: 46887
		string Name { get; }

		// Token: 0x0600B728 RID: 46888
		void RenderColumn(StringBuilder builder, MemoryStatColumn column, bool delta);

		// Token: 0x0600B729 RID: 46889
		void UpdateLast();
	}
}
