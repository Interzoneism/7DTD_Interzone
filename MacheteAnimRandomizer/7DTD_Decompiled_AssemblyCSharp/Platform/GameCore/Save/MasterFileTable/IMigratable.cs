using System;

namespace Platform.GameCore.Save.MasterFileTable
{
	// Token: 0x020018A8 RID: 6312
	public interface IMigratable
	{
		// Token: 0x17001537 RID: 5431
		// (get) Token: 0x0600BA5C RID: 47708
		ushort Version { get; }

		// Token: 0x0600BA5D RID: 47709
		void Write(PooledBinaryWriter writer);

		// Token: 0x0600BA5E RID: 47710
		void Read(PooledBinaryReader reader);
	}
}
