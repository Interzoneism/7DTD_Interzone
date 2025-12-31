using System;
using System.Collections.Generic;
using System.Text;

namespace Platform
{
	// Token: 0x02001809 RID: 6153
	public sealed class PlatformMemoryStat<T> : IPlatformMemoryStat<T>, IPlatformMemoryStat
	{
		// Token: 0x0600B757 RID: 46935 RVA: 0x00467EC3 File Offset: 0x004660C3
		[PublicizedFrom(EAccessModifier.Private)]
		public PlatformMemoryStat(string name)
		{
			this.m_columnValues = new EnumDictionary<MemoryStatColumn, T>();
			this.m_columnLastValues = new EnumDictionary<MemoryStatColumn, T>();
			this.Name = name;
		}

		// Token: 0x170014C9 RID: 5321
		// (get) Token: 0x0600B758 RID: 46936 RVA: 0x00467EE8 File Offset: 0x004660E8
		public string Name { get; }

		// Token: 0x0600B759 RID: 46937 RVA: 0x00467EF0 File Offset: 0x004660F0
		public void UpdateLast()
		{
			foreach (KeyValuePair<MemoryStatColumn, T> keyValuePair in this.m_columnValues)
			{
				MemoryStatColumn memoryStatColumn;
				T t;
				keyValuePair.Deconstruct(out memoryStatColumn, out t);
				MemoryStatColumn key = memoryStatColumn;
				T value = t;
				this.m_columnLastValues[key] = value;
			}
		}

		// Token: 0x0600B75A RID: 46938 RVA: 0x00467F5C File Offset: 0x0046615C
		public void RenderColumn(StringBuilder builder, MemoryStatColumn column, bool delta)
		{
			T t;
			if (!this.m_columnValues.TryGetValue(column, out t))
			{
				return;
			}
			if (!delta)
			{
				PlatformMemoryRenderValue<T> renderValue = this.RenderValue;
				if (renderValue == null)
				{
					return;
				}
				renderValue(builder, t);
				return;
			}
			else
			{
				T last = this.m_columnLastValues[column];
				PlatformMemoryRenderDelta<T> renderDelta = this.RenderDelta;
				if (renderDelta == null)
				{
					return;
				}
				renderDelta(builder, t, last);
				return;
			}
		}

		// Token: 0x14000113 RID: 275
		// (add) Token: 0x0600B75B RID: 46939 RVA: 0x00467FB0 File Offset: 0x004661B0
		// (remove) Token: 0x0600B75C RID: 46940 RVA: 0x00467FE8 File Offset: 0x004661E8
		public event PlatformMemoryColumnChangedHandler<T> ColumnSetAfter;

		// Token: 0x170014CA RID: 5322
		// (get) Token: 0x0600B75D RID: 46941 RVA: 0x0046801D File Offset: 0x0046621D
		// (set) Token: 0x0600B75E RID: 46942 RVA: 0x00468025 File Offset: 0x00466225
		public PlatformMemoryRenderValue<T> RenderValue { get; set; }

		// Token: 0x170014CB RID: 5323
		// (get) Token: 0x0600B75F RID: 46943 RVA: 0x0046802E File Offset: 0x0046622E
		// (set) Token: 0x0600B760 RID: 46944 RVA: 0x00468036 File Offset: 0x00466236
		public PlatformMemoryRenderDelta<T> RenderDelta { get; set; }

		// Token: 0x0600B761 RID: 46945 RVA: 0x0046803F File Offset: 0x0046623F
		public void Set(MemoryStatColumn column, T value)
		{
			this.m_columnValues[column] = value;
			if (!this.m_columnLastValues.ContainsKey(column))
			{
				this.m_columnLastValues[column] = value;
			}
			PlatformMemoryColumnChangedHandler<T> columnSetAfter = this.ColumnSetAfter;
			if (columnSetAfter == null)
			{
				return;
			}
			columnSetAfter(column, value);
		}

		// Token: 0x0600B762 RID: 46946 RVA: 0x0046807B File Offset: 0x0046627B
		public bool TryGet(MemoryStatColumn column, out T value)
		{
			return this.m_columnValues.TryGetValue(column, out value);
		}

		// Token: 0x0600B763 RID: 46947 RVA: 0x0046808A File Offset: 0x0046628A
		public bool TryGetLast(MemoryStatColumn column, out T value)
		{
			return this.m_columnLastValues.TryGetValue(column, out value);
		}

		// Token: 0x0600B764 RID: 46948 RVA: 0x00468099 File Offset: 0x00466299
		public static IPlatformMemoryStat<T> Create(string name)
		{
			return new PlatformMemoryStat<T>(name);
		}

		// Token: 0x04008FD9 RID: 36825
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly EnumDictionary<MemoryStatColumn, T> m_columnValues;

		// Token: 0x04008FDA RID: 36826
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly EnumDictionary<MemoryStatColumn, T> m_columnLastValues;
	}
}
