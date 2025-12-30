using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpEXR
{
	// Token: 0x0200140E RID: 5134
	public class OffsetTable : IEnumerable<uint>, IEnumerable
	{
		// Token: 0x1700115E RID: 4446
		// (get) Token: 0x0600A011 RID: 40977 RVA: 0x003F5D56 File Offset: 0x003F3F56
		// (set) Token: 0x0600A012 RID: 40978 RVA: 0x003F5D5E File Offset: 0x003F3F5E
		public List<uint> Offsets { get; set; }

		// Token: 0x0600A013 RID: 40979 RVA: 0x003F5D67 File Offset: 0x003F3F67
		public OffsetTable()
		{
			this.Offsets = new List<uint>();
		}

		// Token: 0x0600A014 RID: 40980 RVA: 0x003F5D7A File Offset: 0x003F3F7A
		public OffsetTable(int capacity)
		{
			this.Offsets = new List<uint>(capacity);
		}

		// Token: 0x0600A015 RID: 40981 RVA: 0x003F5D90 File Offset: 0x003F3F90
		public void Read(IEXRReader reader, int count)
		{
			for (int i = 0; i < count; i++)
			{
				this.Offsets.Add(reader.ReadUInt32());
				reader.ReadUInt32();
			}
		}

		// Token: 0x0600A016 RID: 40982 RVA: 0x003F5DC1 File Offset: 0x003F3FC1
		public IEnumerator<uint> GetEnumerator()
		{
			return this.Offsets.GetEnumerator();
		}

		// Token: 0x0600A017 RID: 40983 RVA: 0x003F5DD3 File Offset: 0x003F3FD3
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
