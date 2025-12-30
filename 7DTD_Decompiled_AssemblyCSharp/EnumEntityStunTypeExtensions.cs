using System;
using UnityEngine.Scripting;

// Token: 0x02000406 RID: 1030
[Preserve]
public static class EnumEntityStunTypeExtensions
{
	// Token: 0x06001EEC RID: 7916 RVA: 0x000C07F3 File Offset: 0x000BE9F3
	public static bool CanMove(this EnumEntityStunType type)
	{
		return type == EnumEntityStunType.None;
	}
}
