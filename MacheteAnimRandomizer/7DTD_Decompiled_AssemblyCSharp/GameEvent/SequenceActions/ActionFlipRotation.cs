using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001670 RID: 5744
	[Preserve]
	public class ActionFlipRotation : ActionBaseClientAction
	{
		// Token: 0x0600AF74 RID: 44916 RVA: 0x00449BCC File Offset: 0x00447DCC
		public override void OnClientPerform(Entity target)
		{
			if (target != null)
			{
				Entity attachedToEntity = target.AttachedToEntity;
				if (attachedToEntity)
				{
					Transform physicsTransform = attachedToEntity.PhysicsTransform;
					Quaternion quaternion = physicsTransform.rotation;
					quaternion = Quaternion.AngleAxis(180f, physicsTransform.up) * quaternion;
					attachedToEntity.SetRotation(quaternion.eulerAngles);
					physicsTransform.rotation = quaternion;
					EntityVehicle entityVehicle = attachedToEntity as EntityVehicle;
					if (entityVehicle)
					{
						entityVehicle.CameraChangeRotation(180f);
						entityVehicle.VelocityFlip();
						return;
					}
				}
				else
				{
					Vector3 rotation = target.rotation;
					rotation.y += 180f;
					target.SetRotation(rotation);
				}
			}
		}

		// Token: 0x0600AF75 RID: 44917 RVA: 0x00449C6E File Offset: 0x00447E6E
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionFlipRotation
			{
				targetGroup = this.targetGroup
			};
		}
	}
}
