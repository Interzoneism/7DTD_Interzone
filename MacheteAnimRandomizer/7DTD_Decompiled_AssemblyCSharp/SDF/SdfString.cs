using System;
using System.IO;

namespace SDF
{
	// Token: 0x020013CE RID: 5070
	public class SdfString : SdfTag
	{
		// Token: 0x06009E82 RID: 40578 RVA: 0x003F049A File Offset: 0x003EE69A
		public SdfString(string _name, string _value)
		{
			base.TagType = SdfTagType.String;
			base.Name = _name;
			base.Value = _value;
		}

		// Token: 0x06009E83 RID: 40579 RVA: 0x003F04B8 File Offset: 0x003EE6B8
		public override void WritePayload(BinaryWriter bw)
		{
			if (base.Value == null)
			{
				Log.Error("Null value: " + base.Name);
			}
			bw.Write((short)Utils.ToBase64(base.Value.ToString()).Length);
			bw.Write(Utils.ToBase64(base.Value.ToString()));
		}
	}
}
