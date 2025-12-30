using System;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001447 RID: 5191
	public class PathTile
	{
		// Token: 0x04007C02 RID: 31746
		public PathTile.PathTileStates TileState;

		// Token: 0x04007C03 RID: 31747
		public byte PathRadius;

		// Token: 0x04007C04 RID: 31748
		public Path Path;

		// Token: 0x02001448 RID: 5192
		public enum PathTileStates : byte
		{
			// Token: 0x04007C06 RID: 31750
			Free,
			// Token: 0x04007C07 RID: 31751
			Blocked,
			// Token: 0x04007C08 RID: 31752
			Highway,
			// Token: 0x04007C09 RID: 31753
			Country
		}
	}
}
