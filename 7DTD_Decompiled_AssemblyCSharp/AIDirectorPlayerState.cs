using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003C5 RID: 965
[Preserve]
public class AIDirectorPlayerState : IMemoryPoolableObject
{
	// Token: 0x06001D67 RID: 7527 RVA: 0x000B7A4C File Offset: 0x000B5C4C
	public AIDirectorPlayerState Construct(EntityPlayer _player)
	{
		this.Player = _player;
		this.m_smellEmitTime = 1.0;
		this.m_dead = false;
		return this;
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x000B7A6C File Offset: 0x000B5C6C
	public void Reset()
	{
		this.Player = null;
	}

	// Token: 0x06001D69 RID: 7529 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x06001D6A RID: 7530 RVA: 0x000B7A75 File Offset: 0x000B5C75
	public void EmitSmell(double dt)
	{
		this.m_smellEmitTime -= dt;
		if (this.m_smellEmitTime <= 0.0)
		{
			this.UpdateSmell();
			this.m_smellEmitTime += 1.0;
		}
	}

	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06001D6B RID: 7531 RVA: 0x000B7AB2 File Offset: 0x000B5CB2
	// (set) Token: 0x06001D6C RID: 7532 RVA: 0x000B7ABA File Offset: 0x000B5CBA
	public AIDirectorPlayerInventory Inventory
	{
		get
		{
			return this.m_inventory;
		}
		set
		{
			this.m_inventory = value;
		}
	}

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06001D6D RID: 7533 RVA: 0x000B7AC3 File Offset: 0x000B5CC3
	// (set) Token: 0x06001D6E RID: 7534 RVA: 0x000B7ACB File Offset: 0x000B5CCB
	public bool Dead
	{
		get
		{
			return this.m_dead;
		}
		set
		{
			if (this.m_dead && !value)
			{
				this.m_smellEmitTime = 2.0;
			}
			this.m_dead = value;
		}
	}

	// Token: 0x06001D6F RID: 7535 RVA: 0x000B7AF0 File Offset: 0x000B5CF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateSmell()
	{
		float num = 0f;
		if (this.m_inventory.bag != null)
		{
			for (int i = 0; i < this.m_inventory.bag.Count; i++)
			{
				ItemClass forId = ItemClass.GetForId(this.m_inventory.bag[i].id);
				if (forId != null && forId.Smell != null)
				{
					num = Math.Max(forId.Smell.range, num);
				}
			}
		}
		if (this.m_inventory.belt != null)
		{
			for (int j = 0; j < this.m_inventory.belt.Count; j++)
			{
				ItemClass forId2 = ItemClass.GetForId(this.m_inventory.belt[j].id);
				if (forId2 != null && forId2.Smell != null)
				{
					num = Math.Max(forId2.Smell.beltRange, num);
				}
			}
		}
		this.Player.Stealth.smell = Mathf.FloorToInt(num);
	}

	// Token: 0x0400141F RID: 5151
	public const double kSmellEmitTime = 1.0;

	// Token: 0x04001420 RID: 5152
	public const float kCheckUndergroundTime = 5f;

	// Token: 0x04001421 RID: 5153
	public const int kNumBlocksUnderground = 10;

	// Token: 0x04001422 RID: 5154
	public EntityPlayer Player;

	// Token: 0x04001423 RID: 5155
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorPlayerInventory m_inventory;

	// Token: 0x04001424 RID: 5156
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_smellEmitTime;

	// Token: 0x04001425 RID: 5157
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_dead;
}
