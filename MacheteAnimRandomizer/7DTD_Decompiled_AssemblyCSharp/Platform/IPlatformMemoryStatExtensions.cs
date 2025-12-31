using System;

namespace Platform
{
	// Token: 0x02001803 RID: 6147
	public static class IPlatformMemoryStatExtensions
	{
		// Token: 0x0600B743 RID: 46915 RVA: 0x00467C84 File Offset: 0x00465E84
		public static IPlatformMemoryStat<T> AddColumnSetHandler<T>(this IPlatformMemoryStat<T> stat, PlatformMemoryColumnChangedHandler<T> handler)
		{
			stat.ColumnSetAfter += handler;
			return stat;
		}

		// Token: 0x0600B744 RID: 46916 RVA: 0x00467C90 File Offset: 0x00465E90
		public static IPlatformMemoryStat<T> WithUpdatePeak<T>(this IPlatformMemoryStat<T> stat) where T : IComparable<T>
		{
			return stat.AddColumnSetHandler(delegate(MemoryStatColumn column, T value)
			{
				if (column != MemoryStatColumn.Current)
				{
					return;
				}
				T other;
				if (!stat.TryGet(MemoryStatColumn.Peak, out other) || value.CompareTo(other) > 0)
				{
					stat.Set(MemoryStatColumn.Peak, value);
				}
			});
		}

		// Token: 0x0600B745 RID: 46917 RVA: 0x00467CC4 File Offset: 0x00465EC4
		public static IPlatformMemoryStat<T> WithUpdateMin<T>(this IPlatformMemoryStat<T> stat) where T : IComparable<T>
		{
			return stat.AddColumnSetHandler(delegate(MemoryStatColumn column, T value)
			{
				if (column != MemoryStatColumn.Current)
				{
					return;
				}
				T other;
				if (!stat.TryGet(MemoryStatColumn.Min, out other) || value.CompareTo(other) < 0)
				{
					stat.Set(MemoryStatColumn.Min, value);
				}
			});
		}

		// Token: 0x0600B746 RID: 46918 RVA: 0x00467CF5 File Offset: 0x00465EF5
		public static bool TryGetCurrentAndLast<T>(this IPlatformMemoryStat<T> stat, MemoryStatColumn column, out T current, out T last)
		{
			if (!stat.TryGet(column, out current))
			{
				last = default(T);
				return false;
			}
			return stat.TryGetLast(column, out last);
		}

		// Token: 0x0600B747 RID: 46919 RVA: 0x00467D14 File Offset: 0x00465F14
		public static bool HasColumnChanged<T>(this IPlatformMemoryStat<T> stat, MemoryStatColumn column, PlatformMemoryStatHasChangedSignificantly<T> checkCurrentVsLast)
		{
			T current;
			T last;
			return stat.TryGetCurrentAndLast(column, out current, out last) && checkCurrentVsLast(current, last);
		}

		// Token: 0x0600B748 RID: 46920 RVA: 0x00467D38 File Offset: 0x00465F38
		public static bool HasColumnIncreased<T>(this IPlatformMemoryStat<T> stat, MemoryStatColumn column) where T : IComparable<T>
		{
			return stat.HasColumnChanged(column, (T current, T last) => current.CompareTo(last) > 0);
		}

		// Token: 0x0600B749 RID: 46921 RVA: 0x00467D60 File Offset: 0x00465F60
		public static bool HasColumnDecreased<T>(this IPlatformMemoryStat<T> stat, MemoryStatColumn column) where T : IComparable<T>
		{
			return stat.HasColumnChanged(column, (T current, T last) => current.CompareTo(last) < 0);
		}

		// Token: 0x0600B74A RID: 46922 RVA: 0x00467D88 File Offset: 0x00465F88
		public static bool HasBytesChangedSignificantly(this IPlatformMemoryStat<long> stat, MemoryStatColumn column)
		{
			return stat.HasColumnChanged(column, delegate(long current, long last)
			{
				long num = Math.Abs(current - last);
				long num2;
				long num3;
				if (stat.TryGet(MemoryStatColumn.Limit, out num2) && last > num2 / 2L)
				{
					num3 = Math.Abs(num2 - last);
				}
				else
				{
					num3 = Math.Abs(last);
				}
				return num >= num3 / 128L;
			});
		}
	}
}
