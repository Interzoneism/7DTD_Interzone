using System;

namespace Platform
{
	// Token: 0x02001801 RID: 6145
	public interface IPlatformMemoryStat<T> : IPlatformMemoryStat
	{
		// Token: 0x14000112 RID: 274
		// (add) Token: 0x0600B736 RID: 46902
		// (remove) Token: 0x0600B737 RID: 46903
		event PlatformMemoryColumnChangedHandler<T> ColumnSetAfter;

		// Token: 0x170014C7 RID: 5319
		// (get) Token: 0x0600B738 RID: 46904
		// (set) Token: 0x0600B739 RID: 46905
		PlatformMemoryRenderValue<T> RenderValue { get; set; }

		// Token: 0x170014C8 RID: 5320
		// (get) Token: 0x0600B73A RID: 46906
		// (set) Token: 0x0600B73B RID: 46907
		PlatformMemoryRenderDelta<T> RenderDelta { get; set; }

		// Token: 0x0600B73C RID: 46908
		void Set(MemoryStatColumn column, T value);

		// Token: 0x0600B73D RID: 46909
		bool TryGet(MemoryStatColumn column, out T value);

		// Token: 0x0600B73E RID: 46910
		bool TryGetLast(MemoryStatColumn column, out T value);
	}
}
