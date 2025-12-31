using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003E6 RID: 998
[Preserve]
public class EAILook : EAIBase
{
	// Token: 0x06001E2F RID: 7727 RVA: 0x000BBBF9 File Offset: 0x000B9DF9
	public EAILook()
	{
		this.MutexBits = 1;
	}

	// Token: 0x06001E30 RID: 7728 RVA: 0x000BBC08 File Offset: 0x000B9E08
	public override bool CanExecute()
	{
		return this.manager.lookTime > 0f && !this.theEntity.Jumping;
	}

	// Token: 0x06001E31 RID: 7729 RVA: 0x000BBC2C File Offset: 0x000B9E2C
	public override void Start()
	{
		this.waitTicks = (int)(this.manager.lookTime * 20f);
		this.manager.lookTime = 0f;
		this.theEntity.GetEntitySenses().Clear();
		this.lookAtTicks = 0;
		this.turnTicks = 0;
		this.theEntity.moveHelper.Stop();
	}

	// Token: 0x06001E32 RID: 7730 RVA: 0x000BBC90 File Offset: 0x000B9E90
	public override bool Continue()
	{
		if (this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			return false;
		}
		int num;
		if (this.theEntity.IsAlert)
		{
			this.waitTicks--;
			this.lookAtTicks -= 2;
			num = this.turnTicks - 1;
			this.turnTicks = num;
			if (num <= 0)
			{
				this.turnTicks = 14;
				this.theEntity.SeekYaw(this.theEntity.rotation.y + (base.RandomFloat * 120f - 60f), 0f, 35f);
			}
		}
		num = this.waitTicks - 1;
		this.waitTicks = num;
		if (num <= 0)
		{
			return false;
		}
		num = this.lookAtTicks - 1;
		this.lookAtTicks = num;
		if (num <= 0)
		{
			this.lookAtTicks = 40;
			Vector3 headPosition = this.theEntity.getHeadPosition();
			Vector3 vector = this.theEntity.GetForwardVector() * 20f;
			vector = Quaternion.Euler(base.RandomFloat * 60f - 30f, base.RandomFloat * 120f - 60f, 0f) * vector;
			this.theEntity.SetLookPosition(headPosition + vector);
		}
		return true;
	}

	// Token: 0x06001E33 RID: 7731 RVA: 0x000BBDCB File Offset: 0x000B9FCB
	public override void Reset()
	{
		this.theEntity.SetLookPosition(Vector3.zero);
	}

	// Token: 0x06001E34 RID: 7732 RVA: 0x000BBDDD File Offset: 0x000B9FDD
	public override string ToString()
	{
		return string.Format("{0}, wait {1}", base.ToString(), ((float)this.waitTicks / 20f).ToCultureInvariantString());
	}

	// Token: 0x040014C8 RID: 5320
	[PublicizedFrom(EAccessModifier.Private)]
	public int waitTicks;

	// Token: 0x040014C9 RID: 5321
	[PublicizedFrom(EAccessModifier.Private)]
	public int lookAtTicks;

	// Token: 0x040014CA RID: 5322
	[PublicizedFrom(EAccessModifier.Private)]
	public int turnTicks;
}
