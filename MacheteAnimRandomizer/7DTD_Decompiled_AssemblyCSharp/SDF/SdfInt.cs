using System;
using System.IO;

namespace SDF
{
	// Token: 0x020013CD RID: 5069
	public class SdfInt : SdfTag
	{
		// Token: 0x06009E80 RID: 40576 RVA: 0x003F0465 File Offset: 0x003EE665
		public SdfInt(string _name, int _value)
		{
			base.TagType = SdfTagType.Int;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x06009E81 RID: 40577 RVA: 0x003F0487 File Offset: 0x003EE687
		public override void WritePayload(BinaryWriter bw)
		{
			bw.Write((int)base.Value);
		}
	}
}
