using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016C7 RID: 5831
	[Preserve]
	public class ActionBlockDoorState : ActionBaseBlockAction
	{
		// Token: 0x0600B10F RID: 45327 RVA: 0x00451340 File Offset: 0x0044F540
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (!blockValue.isair)
			{
				BlockDoor blockDoor = blockValue.Block as BlockDoor;
				if (blockDoor != null)
				{
					blockValue.meta = (byte)((this.setOpen ? 1 : 0) | ((int)blockValue.meta & -2));
					if (this.handleLock)
					{
						TileEntitySecureDoor tileEntitySecureDoor = (TileEntitySecureDoor)world.GetTileEntity(0, currentPos);
						if (tileEntitySecureDoor != null)
						{
							tileEntitySecureDoor.SetLocked(this.setLocked);
						}
					}
					blockDoor.HandleOpenCloseSound(this.setOpen, currentPos);
					return new BlockChangeInfo(0, currentPos, blockValue);
				}
			}
			return null;
		}

		// Token: 0x0600B110 RID: 45328 RVA: 0x004513C1 File Offset: 0x0044F5C1
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseBool(ActionBlockDoorState.PropSetOpenState, ref this.setOpen);
			if (properties.Contains(ActionBlockDoorState.PropSetLockState))
			{
				this.handleLock = true;
				properties.ParseBool(ActionBlockDoorState.PropSetLockState, ref this.setLocked);
			}
		}

		// Token: 0x0600B111 RID: 45329 RVA: 0x00451400 File Offset: 0x0044F600
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockDoorState
			{
				setOpen = this.setOpen,
				setLocked = this.setLocked,
				handleLock = this.handleLock
			};
		}

		// Token: 0x04008A98 RID: 35480
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool setOpen = true;

		// Token: 0x04008A99 RID: 35481
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool setLocked;

		// Token: 0x04008A9A RID: 35482
		[PublicizedFrom(EAccessModifier.Private)]
		public bool handleLock;

		// Token: 0x04008A9B RID: 35483
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSetOpenState = "set_open";

		// Token: 0x04008A9C RID: 35484
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSetLockState = "set_lock";
	}
}
