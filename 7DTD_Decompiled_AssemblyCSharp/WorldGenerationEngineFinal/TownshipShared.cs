using System;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001469 RID: 5225
	public class TownshipShared
	{
		// Token: 0x0600A1EA RID: 41450 RVA: 0x004051CC File Offset: 0x004033CC
		public TownshipShared(WorldBuilder _worldBuilder)
		{
			this.worldBuilder = _worldBuilder;
		}

		// Token: 0x04007CDB RID: 31963
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04007CDC RID: 31964
		public int NextId;

		// Token: 0x04007CDD RID: 31965
		public readonly Vector2i[] dir4way = new Vector2i[]
		{
			new Vector2i(0, 1),
			new Vector2i(1, 0),
			new Vector2i(0, -1),
			new Vector2i(-1, 0)
		};
	}
}
