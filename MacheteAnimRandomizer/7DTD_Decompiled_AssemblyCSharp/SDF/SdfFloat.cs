using System;
using System.IO;

namespace SDF
{
	// Token: 0x020013CC RID: 5068
	public class SdfFloat : SdfTag
	{
		// Token: 0x06009E7E RID: 40574 RVA: 0x003F0430 File Offset: 0x003EE630
		public SdfFloat(string _name, float _value)
		{
			base.TagType = SdfTagType.Float;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x06009E7F RID: 40575 RVA: 0x003F0452 File Offset: 0x003EE652
		public override void WritePayload(BinaryWriter bw)
		{
			bw.Write((float)base.Value);
		}
	}
}
