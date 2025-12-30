using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001457 RID: 5207
	public class StampGroup
	{
		// Token: 0x0600A15B RID: 41307 RVA: 0x003FED1B File Offset: 0x003FCF1B
		public StampGroup(string _name)
		{
			this.Name = _name;
			this.Stamps = new List<Stamp>();
		}

		// Token: 0x04007C57 RID: 31831
		public string Name;

		// Token: 0x04007C58 RID: 31832
		public List<Stamp> Stamps;
	}
}
