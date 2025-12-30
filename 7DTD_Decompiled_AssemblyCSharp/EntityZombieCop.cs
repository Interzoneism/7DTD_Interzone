using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200047D RID: 1149
[Preserve]
public class EntityZombieCop : EntityZombie
{
	// Token: 0x0600256C RID: 9580 RVA: 0x000F2409 File Offset: 0x000F0609
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
	}

	// Token: 0x0600256D RID: 9581 RVA: 0x000F2412 File Offset: 0x000F0612
	public override void InitFromPrefab(int _entityClass)
	{
		base.InitFromPrefab(_entityClass);
	}

	// Token: 0x0600256E RID: 9582 RVA: 0x000F241B File Offset: 0x000F061B
	public override void PostInit()
	{
		this.inventory.SetItem(0, this.inventory.GetBareHandItemValue(), 1, true);
		this.HandleNavObject();
	}

	// Token: 0x0600256F RID: 9583 RVA: 0x000F243C File Offset: 0x000F063C
	public override void CopyPropertiesFromEntityClass()
	{
		DynamicProperties properties = EntityClass.list[this.entityClass].Properties;
		properties.ParseFloat(EntityClass.PropExplodeDelay, ref this.explodeDelay);
		properties.ParseFloat(EntityClass.PropExplodeHealthThreshold, ref this.explodeHealthThreshold);
		properties.ParseString(EntityClass.PropSoundExplodeWarn, ref this.warnSoundName);
		properties.ParseString(EntityClass.PropSoundTick, ref this.tickSoundName);
		if (this.tickSoundName != null)
		{
			string[] array = this.tickSoundName.Split(',', StringSplitOptions.None);
			this.tickSoundName = array[0];
			if (array.Length >= 2)
			{
				this.tickSoundDelayStart = StringParsers.ParseFloat(array[1], 0, -1, NumberStyles.Any);
				if (array.Length >= 3)
				{
					this.tickSoundDelayScale = StringParsers.ParseFloat(array[2], 0, -1, NumberStyles.Any);
				}
			}
		}
		base.CopyPropertiesFromEntityClass();
	}

	// Token: 0x06002570 RID: 9584 RVA: 0x000F2500 File Offset: 0x000F0700
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		if (this.isEntityRemote)
		{
			return;
		}
		if (!this.isPrimed && !this.IsSleeping && !this.Buffs.HasBuff("buffShocked"))
		{
			float num = (float)this.Health;
			if (num > 0f && num < (float)this.GetMaxHealth() * this.explodeHealthThreshold)
			{
				this.isPrimed = true;
				this.ticksToStartToExplode = (int)(this.explodeDelay * 20f);
				this.PlayOneShot(this.warnSoundName, false, false, false, null);
			}
		}
		if (this.isPrimed && !this.IsDead())
		{
			if (this.ticksToStartToExplode > 0)
			{
				this.ticksToStartToExplode--;
				if (this.ticksToStartToExplode == 0)
				{
					this.SpecialAttack2 = true;
					this.ticksToExplode = (int)(this.explodeDelay / 5f * 1.5f * 20f);
				}
			}
			if (this.ticksToExplode > 0)
			{
				this.ticksToExplode--;
				if (this.ticksToExplode == 0)
				{
					base.NotifySleeperDeath();
					this.SetModelLayer(2, false, null);
					this.ticksToExplode = -1;
					GameManager.Instance.ExplosionServer(0, base.GetPosition(), World.worldToBlockPos(base.GetPosition()), base.transform.rotation, EntityClass.list[this.entityClass].explosionData, this.entityId, 0f, false, null);
					this.timeStayAfterDeath = 0;
					this.SetDead();
				}
			}
			this.tickSoundDelay -= 0.05f;
			if (this.tickSoundDelay <= 0f)
			{
				this.tickSoundDelayStart *= this.tickSoundDelayScale;
				this.tickSoundDelay = this.tickSoundDelayStart;
				if (this.tickSoundDelay < 0.2f)
				{
					this.tickSoundDelay = 0.2f;
				}
				this.PlayOneShot(this.tickSoundName, false, false, false, null);
			}
		}
		if (this.ticksToExplode < 0)
		{
			this.motion.x = this.motion.x * 0.7f;
			this.motion.z = this.motion.z * 0.7f;
		}
	}

	// Token: 0x06002571 RID: 9585 RVA: 0x000F2709 File Offset: 0x000F0909
	public override float GetMoveSpeed()
	{
		if (this.ticksToExplode != 0)
		{
			return 0f;
		}
		return base.GetMoveSpeed();
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x000F271F File Offset: 0x000F091F
	public override float GetMoveSpeedAggro()
	{
		if (this.ticksToExplode != 0)
		{
			return 0f;
		}
		if (this.isPrimed)
		{
			return this.moveSpeedAggroMax;
		}
		return base.GetMoveSpeedAggro();
	}

	// Token: 0x06002573 RID: 9587 RVA: 0x000F2744 File Offset: 0x000F0944
	public override bool IsAttackValid()
	{
		return !this.isPrimed && base.IsAttackValid();
	}

	// Token: 0x06002574 RID: 9588 RVA: 0x000F2758 File Offset: 0x000F0958
	public override void ProcessDamageResponseLocal(DamageResponse _dmResponse)
	{
		if (!this.isEntityRemote && (_dmResponse.HitBodyPart & EnumBodyPartHit.Special) > EnumBodyPartHit.None)
		{
			bool flag = !this.isPrimed;
			ItemClass itemClass = _dmResponse.Source.ItemClass;
			if (itemClass != null && itemClass is ItemClassBlock && _dmResponse.Source.CreatorEntityId == -2)
			{
				flag = false;
			}
			if (flag)
			{
				this.HandlePrimingDetonator(-1f);
			}
		}
		base.ProcessDamageResponseLocal(_dmResponse);
	}

	// Token: 0x06002575 RID: 9589 RVA: 0x000F27C4 File Offset: 0x000F09C4
	public void PrimeDetonator()
	{
		Detonator componentInChildren = base.gameObject.GetComponentInChildren<Detonator>(true);
		if (componentInChildren != null)
		{
			componentInChildren.PulseRateScale = 1f;
			componentInChildren.gameObject.GetComponent<Light>().color = Color.red;
			componentInChildren.StartCountdown();
			return;
		}
		Log.Out("PrimeDetonator found no Detonator component");
	}

	// Token: 0x06002576 RID: 9590 RVA: 0x000F2818 File Offset: 0x000F0A18
	public void HandlePrimingDetonator(float overrideDelay = -1f)
	{
		this.PlayOneShot(this.warnSoundName, false, false, false, null);
		this.isPrimed = true;
		this.ticksToStartToExplode = (int)(((overrideDelay > 0f) ? overrideDelay : this.explodeDelay) * 20f);
		this.PrimeDetonator();
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityPrimeDetonator>().Setup(this), false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x04001C76 RID: 7286
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int ticksToStartToExplode;

	// Token: 0x04001C77 RID: 7287
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int ticksToExplode;

	// Token: 0x04001C78 RID: 7288
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float explodeDelay = 5f;

	// Token: 0x04001C79 RID: 7289
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float explodeHealthThreshold = 0.4f;

	// Token: 0x04001C7A RID: 7290
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isPrimed;

	// Token: 0x04001C7B RID: 7291
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string warnSoundName;

	// Token: 0x04001C7C RID: 7292
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string tickSoundName;

	// Token: 0x04001C7D RID: 7293
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float tickSoundDelayStart = 1f;

	// Token: 0x04001C7E RID: 7294
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float tickSoundDelayScale = 1f;

	// Token: 0x04001C7F RID: 7295
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float tickSoundDelay;
}
