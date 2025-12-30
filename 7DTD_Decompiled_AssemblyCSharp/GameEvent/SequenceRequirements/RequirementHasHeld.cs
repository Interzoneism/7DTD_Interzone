using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200161C RID: 5660
	[Preserve]
	public class RequirementHasHeld : BaseRequirement
	{
		// Token: 0x0600ADFC RID: 44540 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600ADFD RID: 44541 RVA: 0x00440550 File Offset: 0x0043E750
		public override bool CanPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			return entityPlayer != null && !entityPlayer.inventory.holdingItemStack.IsEmpty();
		}

		// Token: 0x0600ADFE RID: 44542 RVA: 0x0044057C File Offset: 0x0043E77C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
		}

		// Token: 0x0600ADFF RID: 44543 RVA: 0x00440585 File Offset: 0x0043E785
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementHasHeld();
		}
	}
}
