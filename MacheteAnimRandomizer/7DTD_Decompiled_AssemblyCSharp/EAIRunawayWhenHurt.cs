using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003EC RID: 1004
[Preserve]
public class EAIRunawayWhenHurt : EAIRunAway
{
	// Token: 0x06001E68 RID: 7784 RVA: 0x000BDB67 File Offset: 0x000BBD67
	public EAIRunawayWhenHurt()
	{
		this.MutexBits = 1;
	}

	// Token: 0x06001E69 RID: 7785 RVA: 0x000BDB78 File Offset: 0x000BBD78
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
		string input;
		if (data.TryGetValue("runChance", out input))
		{
			this.lowHealthPercent = 0f;
			if (StringParsers.ParseFloat(input, 0, -1, NumberStyles.Any) >= base.RandomFloat)
			{
				base.GetData(data, "healthPer", ref this.lowHealthPercent);
				if (data.TryGetValue("healthPerMax", out input))
				{
					float num = StringParsers.ParseFloat(input, 0, -1, NumberStyles.Any);
					this.lowHealthPercent += base.RandomFloat * (num - this.lowHealthPercent);
				}
			}
		}
	}

	// Token: 0x06001E6A RID: 7786 RVA: 0x000BDC08 File Offset: 0x000BBE08
	public override bool CanExecute()
	{
		this.enemy = this.theEntity.GetRevengeTarget();
		return this.enemy && (this.lowHealthPercent >= 1f || (float)this.theEntity.Health / (float)this.theEntity.GetMaxHealth() < this.lowHealthPercent) && base.CanExecute();
	}

	// Token: 0x06001E6B RID: 7787 RVA: 0x000BDC6B File Offset: 0x000BBE6B
	public override bool Continue()
	{
		return base.Continue();
	}

	// Token: 0x06001E6C RID: 7788 RVA: 0x000BDC73 File Offset: 0x000BBE73
	public override void Update()
	{
		base.Update();
		this.theEntity.navigator.setMoveSpeed(this.theEntity.IsInWater() ? this.theEntity.GetMoveSpeed() : this.theEntity.GetMoveSpeedPanic());
	}

	// Token: 0x06001E6D RID: 7789 RVA: 0x000BDCB0 File Offset: 0x000BBEB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override Vector3 GetFleeFromPos()
	{
		if (this.enemy)
		{
			return this.enemy.position;
		}
		return this.theEntity.position;
	}

	// Token: 0x06001E6E RID: 7790 RVA: 0x000BDCD6 File Offset: 0x000BBED6
	public override string ToString()
	{
		return string.Format("{0}, per {1}", base.ToString(), this.lowHealthPercent);
	}

	// Token: 0x040014FC RID: 5372
	[PublicizedFrom(EAccessModifier.Private)]
	public float lowHealthPercent;

	// Token: 0x040014FD RID: 5373
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive enemy;
}
