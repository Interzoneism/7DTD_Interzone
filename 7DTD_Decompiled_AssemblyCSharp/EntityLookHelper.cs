using System;

// Token: 0x0200048D RID: 1165
public class EntityLookHelper
{
	// Token: 0x060025F1 RID: 9713 RVA: 0x000F3FE7 File Offset: 0x000F21E7
	public EntityLookHelper(EntityAlive _e)
	{
		this.entity = _e;
	}

	// Token: 0x060025F2 RID: 9714 RVA: 0x000F3FF8 File Offset: 0x000F21F8
	public void onUpdateLook()
	{
		if (this.entity.rotation.x > 1f)
		{
			EntityAlive entityAlive = this.entity;
			entityAlive.rotation.x = entityAlive.rotation.x - 1f;
			return;
		}
		if (this.entity.rotation.x < -1f)
		{
			EntityAlive entityAlive2 = this.entity;
			entityAlive2.rotation.x = entityAlive2.rotation.x + 1f;
		}
	}

	// Token: 0x04001CBB RID: 7355
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entity;
}
