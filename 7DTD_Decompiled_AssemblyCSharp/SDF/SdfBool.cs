using System;
using System.IO;

namespace SDF
{
	// Token: 0x020013CF RID: 5071
	public class SdfBool : SdfTag
	{
		// Token: 0x06009E84 RID: 40580 RVA: 0x003F0514 File Offset: 0x003EE714
		public SdfBool(string _name, bool _value)
		{
			base.TagType = SdfTagType.Bool;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x06009E85 RID: 40581 RVA: 0x003F0536 File Offset: 0x003EE736
		public override void WritePayload(BinaryWriter bw)
		{
			bw.Write((bool)base.Value);
		}
	}
}
