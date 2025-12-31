using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016D6 RID: 5846
	[Preserve]
	public class ActionRenameSigns : ActionBaseContainersAction
	{
		// Token: 0x0600B14E RID: 45390 RVA: 0x004526A0 File Offset: 0x004508A0
		public override bool CheckValidTileEntity(TileEntity te, out bool isEmpty)
		{
			isEmpty = true;
			ITileEntitySignable tileEntitySignable;
			if (te.TryGetSelfOrFeature(out tileEntitySignable) && tileEntitySignable.EntityId == -1)
			{
				isEmpty = (tileEntitySignable.GetAuthoredText().Text == base.ModifiedName);
				return true;
			}
			return false;
		}

		// Token: 0x0600B14F RID: 45391 RVA: 0x004526E0 File Offset: 0x004508E0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool HandleContainerAction(List<TileEntity> tileEntityList)
		{
			bool result = false;
			for (int i = 0; i < tileEntityList.Count; i++)
			{
				ITileEntitySignable tileEntitySignable;
				if (tileEntityList[i].TryGetSelfOrFeature(out tileEntitySignable) && tileEntitySignable.EntityId == -1)
				{
					tileEntitySignable.SetText(base.ModifiedName, true, PlatformManager.MultiPlatform.User.PlatformUserId);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600B150 RID: 45392 RVA: 0x00452738 File Offset: 0x00450938
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRenameSigns
			{
				TargetingType = this.TargetingType,
				maxDistance = this.maxDistance,
				newName = this.newName,
				changeName = this.changeName,
				tileEntityList = this.tileEntityList
			};
		}
	}
}
