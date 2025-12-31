using System;
using System.IO;

namespace SDF
{
	// Token: 0x020013D2 RID: 5074
	public abstract class SdfTag
	{
		// Token: 0x17001110 RID: 4368
		// (get) Token: 0x06009E8A RID: 40586 RVA: 0x003F05DD File Offset: 0x003EE7DD
		// (set) Token: 0x06009E8B RID: 40587 RVA: 0x003F05E5 File Offset: 0x003EE7E5
		public SdfTagType TagType { get; set; }

		// Token: 0x17001111 RID: 4369
		// (get) Token: 0x06009E8C RID: 40588 RVA: 0x003F05EE File Offset: 0x003EE7EE
		// (set) Token: 0x06009E8D RID: 40589 RVA: 0x003F05F6 File Offset: 0x003EE7F6
		public string Name { get; set; }

		// Token: 0x17001112 RID: 4370
		// (get) Token: 0x06009E8E RID: 40590 RVA: 0x003F05FF File Offset: 0x003EE7FF
		// (set) Token: 0x06009E8F RID: 40591 RVA: 0x003F0607 File Offset: 0x003EE807
		public object Value { get; set; }

		// Token: 0x06009E90 RID: 40592
		public abstract void WritePayload(BinaryWriter bw);

		// Token: 0x06009E91 RID: 40593 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Protected)]
		public SdfTag()
		{
		}
	}
}
