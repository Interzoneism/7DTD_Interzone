using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200142E RID: 5166
	public class GenerationLayer
	{
		// Token: 0x0600A06B RID: 41067 RVA: 0x003F7D8F File Offset: 0x003F5F8F
		public GenerationLayer(int _x, int _y, int _range)
		{
			this.x = _x;
			this.y = _y;
			this.Range = _range;
			this.children = new List<TranslationData>();
		}

		// Token: 0x04007B51 RID: 31569
		public int x;

		// Token: 0x04007B52 RID: 31570
		public int y;

		// Token: 0x04007B53 RID: 31571
		public int Range;

		// Token: 0x04007B54 RID: 31572
		public List<TranslationData> children;
	}
}
