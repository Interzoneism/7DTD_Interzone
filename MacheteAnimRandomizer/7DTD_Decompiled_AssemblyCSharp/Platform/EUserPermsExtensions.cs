using System;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x0200182D RID: 6189
	public static class EUserPermsExtensions
	{
		// Token: 0x0600B7F0 RID: 47088 RVA: 0x00468915 File Offset: 0x00466B15
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasMultiplayer(this EUserPerms _perms)
		{
			return _perms.HasFlag(EUserPerms.Multiplayer);
		}

		// Token: 0x0600B7F1 RID: 47089 RVA: 0x00468928 File Offset: 0x00466B28
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasCommunication(this EUserPerms _perms)
		{
			return _perms.HasFlag(EUserPerms.Communication);
		}

		// Token: 0x0600B7F2 RID: 47090 RVA: 0x0046893B File Offset: 0x00466B3B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasCrossplay(this EUserPerms _perms)
		{
			return _perms.HasFlag(EUserPerms.Crossplay);
		}

		// Token: 0x0600B7F3 RID: 47091 RVA: 0x0046894E File Offset: 0x00466B4E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasHostMultiplayer(this EUserPerms _perms)
		{
			return _perms.HasFlag(EUserPerms.HostMultiplayer);
		}
	}
}
