using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001686 RID: 5766
	[Preserve]
	public class ActionRandomizeRotation : ActionBaseClientAction
	{
		// Token: 0x0600AFC8 RID: 45000 RVA: 0x0044B50C File Offset: 0x0044970C
		public override void OnClientPerform(Entity target)
		{
			if (target != null)
			{
				Entity attachedToEntity = target.AttachedToEntity;
				float num = (float)GameEventManager.Current.Random.RandomRange(45, 315);
				if (attachedToEntity)
				{
					Transform physicsTransform = attachedToEntity.PhysicsTransform;
					Quaternion quaternion = physicsTransform.rotation;
					quaternion = Quaternion.AngleAxis(num, physicsTransform.up) * quaternion;
					attachedToEntity.SetRotation(quaternion.eulerAngles);
					physicsTransform.rotation = quaternion;
					EntityVehicle entityVehicle = attachedToEntity as EntityVehicle;
					if (entityVehicle)
					{
						entityVehicle.CameraChangeRotation(num);
						return;
					}
				}
				else
				{
					Vector3 rotation = target.rotation;
					rotation.y += num;
					target.SetRotation(rotation);
				}
			}
		}

		// Token: 0x0600AFC9 RID: 45001 RVA: 0x0044B5B7 File Offset: 0x004497B7
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRandomizeRotation
			{
				targetGroup = this.targetGroup
			};
		}
	}
}
