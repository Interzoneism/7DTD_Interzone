using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003ED RID: 1005
[Preserve]
public class EAISetAsTargetIfHurt : EAITarget
{
	// Token: 0x06001E6F RID: 7791 RVA: 0x000BDCF3 File Offset: 0x000BBEF3
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity, 0f, false);
		this.MutexBits = 1;
	}

	// Token: 0x06001E70 RID: 7792 RVA: 0x000BDD0C File Offset: 0x000BBF0C
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
		this.targetClasses = new List<EAISetAsTargetIfHurt.TargetClass>();
		string text;
		if (data.TryGetValue("class", out text))
		{
			string[] array = text.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				EAISetAsTargetIfHurt.TargetClass item = default(EAISetAsTargetIfHurt.TargetClass);
				item.type = EntityFactory.GetEntityType(array[i]);
				this.targetClasses.Add(item);
			}
		}
	}

	// Token: 0x06001E71 RID: 7793 RVA: 0x000BDD78 File Offset: 0x000BBF78
	public override bool CanExecute()
	{
		EntityAlive revengeTarget = this.theEntity.GetRevengeTarget();
		EntityAlive attackTarget = this.theEntity.GetAttackTarget();
		if (revengeTarget && revengeTarget != attackTarget && revengeTarget.entityType != this.theEntity.entityType)
		{
			if (this.targetClasses != null)
			{
				bool flag = false;
				Type type = revengeTarget.GetType();
				for (int i = 0; i < this.targetClasses.Count; i++)
				{
					EAISetAsTargetIfHurt.TargetClass targetClass = this.targetClasses[i];
					if (targetClass.type != null && targetClass.type.IsAssignableFrom(type))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			if (attackTarget != null && attackTarget.IsAlive() && base.RandomFloat < 0.66f)
			{
				this.theEntity.SetRevengeTarget(null);
				return false;
			}
			if (base.check(revengeTarget))
			{
				return true;
			}
			Vector3 vector = this.theEntity.position - revengeTarget.position;
			float searchRadius = EntityClass.list[this.theEntity.entityClass].SearchRadius;
			vector = revengeTarget.position + vector.normalized * (searchRadius * 0.35f);
			Vector2 vector2 = this.manager.random.RandomInsideUnitCircle * searchRadius;
			vector.x += vector2.x;
			vector.z += vector2.y;
			Vector3i vector3i = World.worldToBlockPos(vector);
			int height = (int)this.theEntity.world.GetHeight(vector3i.x, vector3i.z);
			if (height > 0)
			{
				vector.y = (float)height;
			}
			int ticks = this.theEntity.CalcInvestigateTicks(1200, revengeTarget);
			this.theEntity.SetInvestigatePosition(vector, ticks, true);
			this.theEntity.SetRevengeTarget(null);
		}
		return false;
	}

	// Token: 0x06001E72 RID: 7794 RVA: 0x000BDF5C File Offset: 0x000BC15C
	public override void Start()
	{
		this.theEntity.SetAttackTarget(this.theEntity.GetRevengeTarget(), 400);
		this.viewAngleSave = this.theEntity.GetMaxViewAngle();
		this.theEntity.SetMaxViewAngle(270f);
		this.viewAngleRestoreCounter = 100;
		base.Start();
	}

	// Token: 0x06001E73 RID: 7795 RVA: 0x000BDFB3 File Offset: 0x000BC1B3
	public override void Update()
	{
		if (this.viewAngleRestoreCounter > 0)
		{
			this.viewAngleRestoreCounter--;
			if (this.viewAngleRestoreCounter == 0)
			{
				this.restoreViewAngle();
			}
		}
	}

	// Token: 0x06001E74 RID: 7796 RVA: 0x000BDFDA File Offset: 0x000BC1DA
	public override bool Continue()
	{
		return (!(this.theEntity.GetRevengeTarget() != null) || !(this.theEntity.GetAttackTarget() != this.theEntity.GetRevengeTarget())) && base.Continue();
	}

	// Token: 0x06001E75 RID: 7797 RVA: 0x000BE014 File Offset: 0x000BC214
	public override void Reset()
	{
		base.Reset();
		this.restoreViewAngle();
	}

	// Token: 0x06001E76 RID: 7798 RVA: 0x000BE022 File Offset: 0x000BC222
	[PublicizedFrom(EAccessModifier.Private)]
	public void restoreViewAngle()
	{
		if (this.viewAngleSave > 0f)
		{
			this.theEntity.SetMaxViewAngle(this.viewAngleSave);
			this.viewAngleSave = 0f;
			this.viewAngleRestoreCounter = 0;
		}
	}

	// Token: 0x040014FE RID: 5374
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAISetAsTargetIfHurt.TargetClass> targetClasses;

	// Token: 0x040014FF RID: 5375
	[PublicizedFrom(EAccessModifier.Private)]
	public float viewAngleSave;

	// Token: 0x04001500 RID: 5376
	[PublicizedFrom(EAccessModifier.Private)]
	public int viewAngleRestoreCounter;

	// Token: 0x020003EE RID: 1006
	[PublicizedFrom(EAccessModifier.Private)]
	public struct TargetClass
	{
		// Token: 0x04001501 RID: 5377
		public Type type;
	}
}
