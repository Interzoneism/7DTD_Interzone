using System;
using System.IO;

namespace SDF
{
	// Token: 0x020013D1 RID: 5073
	public class SdfByteArray : SdfTag
	{
		// Token: 0x06009E88 RID: 40584 RVA: 0x003F059A File Offset: 0x003EE79A
		public SdfByteArray(string _name, byte[] _value)
		{
			base.TagType = SdfTagType.ByteArray;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x06009E89 RID: 40585 RVA: 0x003F05B7 File Offset: 0x003EE7B7
		public override void WritePayload(BinaryWriter bw)
		{
			bw.Write(((byte[])base.Value).Length);
			bw.Write((byte[])base.Value);
		}
	}
}
