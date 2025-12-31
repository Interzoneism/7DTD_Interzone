using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200043F RID: 1087
[Preserve]
public class EntityEnemyAnimal : EntityEnemy
{
	// Token: 0x0600220D RID: 8717 RVA: 0x000D6260 File Offset: 0x000D4460
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		if (this.ModelTransform)
		{
			this.animator = this.ModelTransform.GetComponentInChildren<Animator>();
		}
	}

	// Token: 0x0600220E RID: 8718 RVA: 0x000CBB40 File Offset: 0x000C9D40
	public override Color GetMapIconColor()
	{
		return new Color(1f, 0.8235294f, 0.34117648f);
	}

	// Token: 0x0600220F RID: 8719 RVA: 0x000CBB56 File Offset: 0x000C9D56
	[PublicizedFrom(EAccessModifier.Protected)]
	public override float getNextStepSoundDistance()
	{
		return 0.8f;
	}

	// Token: 0x06002210 RID: 8720 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isGameMessageOnDeath()
	{
		return false;
	}

	// Token: 0x06002211 RID: 8721 RVA: 0x000D6288 File Offset: 0x000D4488
	public override bool CanDamageEntity(int _sourceEntityId)
	{
		Entity entity = this.world.GetEntity(_sourceEntityId);
		return !entity || entity.entityClass != this.entityClass;
	}

	// Token: 0x06002212 RID: 8722 RVA: 0x000D62BC File Offset: 0x000D44BC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTasks()
	{
		if (this.Electrocuted)
		{
			base.SetMoveForward(0f);
			if (this.animator)
			{
				this.animator.enabled = false;
			}
			return;
		}
		if (this.animator)
		{
			this.animator.enabled = true;
		}
		base.updateTasks();
	}

	// Token: 0x04001972 RID: 6514
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator animator;
}
