using System;

namespace Platform
{
	// Token: 0x0200180E RID: 6158
	public struct PlayerInteraction
	{
		// Token: 0x0600B77D RID: 46973 RVA: 0x00468442 File Offset: 0x00466642
		public PlayerInteraction(PlayerData _playerData, PlayerInteractionType _type)
		{
			this.PlayerData = _playerData;
			this.Type = _type;
		}

		// Token: 0x04008FE3 RID: 36835
		public PlayerData PlayerData;

		// Token: 0x04008FE4 RID: 36836
		public PlayerInteractionType Type;
	}
}
