using System;
using UnityEngine.Scripting;

// Token: 0x020000F4 RID: 244
[Preserve]
public class BlockCampfire : BlockWorkstation
{
	// Token: 0x0600063E RID: 1598 RVA: 0x0002CEB4 File Offset: 0x0002B0B4
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return Localization.Get("useCampfire", false);
	}
}
