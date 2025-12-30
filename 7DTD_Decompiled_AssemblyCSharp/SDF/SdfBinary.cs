using System;
using System.IO;

namespace SDF
{
	// Token: 0x020013D0 RID: 5072
	public class SdfBinary : SdfTag
	{
		// Token: 0x06009E86 RID: 40582 RVA: 0x003F0549 File Offset: 0x003EE749
		public SdfBinary(string _name, string _value)
		{
			base.TagType = SdfTagType.Binary;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x06009E87 RID: 40583 RVA: 0x003F0566 File Offset: 0x003EE766
		public override void WritePayload(BinaryWriter bw)
		{
			bw.Write((short)Utils.ToBase64(base.Value.ToString()).Length);
			bw.Write(Utils.ToBase64((string)base.Value));
		}
	}
}
