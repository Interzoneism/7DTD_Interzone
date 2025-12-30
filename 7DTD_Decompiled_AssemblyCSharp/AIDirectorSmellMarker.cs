using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003C8 RID: 968
[Preserve]
public sealed class AIDirectorSmellMarker : IAIDirectorMarker, IMemoryPoolableObject
{
	// Token: 0x06001D75 RID: 7541 RVA: 0x000B7BDF File Offset: 0x000B5DDF
	public void Reference()
	{
		this.m_refCount++;
	}

	// Token: 0x06001D76 RID: 7542 RVA: 0x000B7BF0 File Offset: 0x000B5DF0
	public bool Release()
	{
		int num = this.m_refCount - 1;
		this.m_refCount = num;
		if (num == 0)
		{
			this.Reset();
			AIDirectorSmellMarker.s_pool.Free(this);
			return true;
		}
		return false;
	}

	// Token: 0x06001D77 RID: 7543 RVA: 0x000B7C24 File Offset: 0x000B5E24
	public void Reset()
	{
		this.m_playerState = null;
	}

	// Token: 0x06001D78 RID: 7544 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x000B7C30 File Offset: 0x000B5E30
	public void Tick(double dt)
	{
		this.m_ttl -= dt;
		if (this.m_ttl < 0.0)
		{
			this.m_ttl = 0.0;
		}
		this.m_validTime -= dt;
		if (this.m_validTime < 0.0)
		{
			this.m_validTime = 0.0;
		}
		this.m_time += dt;
		if (this.m_time > this.m_lifetime)
		{
			this.m_time = this.m_lifetime;
		}
		this.m_effectiveRadius = ((this.m_speed > 0.0) ? Math.Min(this.m_radius, this.m_speed * this.m_time) : this.m_radius);
		this.m_effectiveStrength = this.m_strength * (1.0 - this.m_time / this.m_lifetime);
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06001D7A RID: 7546 RVA: 0x000B7D1C File Offset: 0x000B5F1C
	public EntityPlayer Player
	{
		get
		{
			if (this.m_playerState != null)
			{
				return this.m_playerState.Player;
			}
			return null;
		}
	}

	// Token: 0x06001D7B RID: 7547 RVA: 0x000B7D34 File Offset: 0x000B5F34
	public double IntensityForPosition(Vector3 position)
	{
		double num = (double)(this.m_pos - position).magnitude;
		if (num > this.m_effectiveRadius)
		{
			return 0.0;
		}
		double num2 = 1.0;
		if (num > 0.0)
		{
			num2 /= num * num;
		}
		return this.m_effectiveStrength * num2;
	}

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x06001D7C RID: 7548 RVA: 0x000B7D8E File Offset: 0x000B5F8E
	public Vector3 Position
	{
		get
		{
			return this.m_pos;
		}
	}

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x06001D7D RID: 7549 RVA: 0x000B7D96 File Offset: 0x000B5F96
	public Vector3 TargetPosition
	{
		get
		{
			return this.m_targetPos;
		}
	}

	// Token: 0x17000345 RID: 837
	// (get) Token: 0x06001D7E RID: 7550 RVA: 0x000B7D9E File Offset: 0x000B5F9E
	public bool Valid
	{
		get
		{
			return this.m_validTime > 0.0 && (this.Player == null || !this.Player.IsDead());
		}
	}

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x06001D7F RID: 7551 RVA: 0x000B7DD1 File Offset: 0x000B5FD1
	public float MaxRadius
	{
		get
		{
			return (float)this.m_radius;
		}
	}

	// Token: 0x17000347 RID: 839
	// (get) Token: 0x06001D80 RID: 7552 RVA: 0x000B7DDA File Offset: 0x000B5FDA
	public float Radius
	{
		get
		{
			return (float)this.m_effectiveRadius;
		}
	}

	// Token: 0x17000348 RID: 840
	// (get) Token: 0x06001D81 RID: 7553 RVA: 0x000B7DE3 File Offset: 0x000B5FE3
	public float TimeToLive
	{
		get
		{
			return (float)this.m_ttl;
		}
	}

	// Token: 0x17000349 RID: 841
	// (get) Token: 0x06001D82 RID: 7554 RVA: 0x000B7DEC File Offset: 0x000B5FEC
	public float ValidTime
	{
		get
		{
			return (float)this.m_validTime;
		}
	}

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x06001D83 RID: 7555 RVA: 0x000B7DF5 File Offset: 0x000B5FF5
	public float Speed
	{
		get
		{
			return (float)this.m_speed;
		}
	}

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x06001D84 RID: 7556 RVA: 0x000B7DFE File Offset: 0x000B5FFE
	public int Priority
	{
		get
		{
			return this.m_priority;
		}
	}

	// Token: 0x1700034C RID: 844
	// (get) Token: 0x06001D85 RID: 7557 RVA: 0x000B7E06 File Offset: 0x000B6006
	public bool InterruptsNonPlayerAttack
	{
		get
		{
			return this.m_interruptsNonPlayerAttack;
		}
	}

	// Token: 0x1700034D RID: 845
	// (get) Token: 0x06001D86 RID: 7558 RVA: 0x000B7E0E File Offset: 0x000B600E
	public bool IsDistraction
	{
		get
		{
			return this.m_isDistraction;
		}
	}

	// Token: 0x06001D87 RID: 7559 RVA: 0x00019766 File Offset: 0x00017966
	public static AIDirectorSmellMarker Allocate(AIDirectorPlayerState ps, Vector3 position, Vector3 targetPosition, double radius, double strength, double speed, int priority, double ttl, bool interruptsNonPlayerAttack, bool isDistraction)
	{
		return null;
	}

	// Token: 0x06001D88 RID: 7560 RVA: 0x000B7E18 File Offset: 0x000B6018
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorSmellMarker Construct(AIDirectorPlayerState ps, Vector3 position, Vector3 targetPosition, double radius, double strength, double speed, int priority, double ttl, bool interruptsNonPlayerAttack, bool isDistraction)
	{
		this.m_refCount = 1;
		this.m_playerState = ps;
		this.m_pos = position;
		this.m_targetPos = targetPosition;
		this.m_radius = radius;
		this.m_strength = strength;
		this.m_speed = speed;
		this.m_priority = priority;
		this.m_validTime = ttl;
		this.m_lifetime = ttl;
		this.m_time = 0.0;
		this.m_effectiveRadius = 0.0;
		this.m_effectiveStrength = strength;
		this.m_interruptsNonPlayerAttack = interruptsNonPlayerAttack;
		this.m_isDistraction = isDistraction;
		if (isDistraction)
		{
			this.m_ttl = (double)Mathf.Max((float)ttl, 20f);
		}
		else
		{
			this.m_ttl = (double)Constants.cEnemySenseMemory;
		}
		return this;
	}

	// Token: 0x04001426 RID: 5158
	public const int kMax = 256;

	// Token: 0x04001427 RID: 5159
	[PublicizedFrom(EAccessModifier.Private)]
	public static MemoryPooledObject<AIDirectorSmellMarker> s_pool = new MemoryPooledObject<AIDirectorSmellMarker>(256);

	// Token: 0x04001428 RID: 5160
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_radius;

	// Token: 0x04001429 RID: 5161
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_strength;

	// Token: 0x0400142A RID: 5162
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_speed;

	// Token: 0x0400142B RID: 5163
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_ttl;

	// Token: 0x0400142C RID: 5164
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_validTime;

	// Token: 0x0400142D RID: 5165
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_time;

	// Token: 0x0400142E RID: 5166
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_lifetime;

	// Token: 0x0400142F RID: 5167
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_effectiveRadius;

	// Token: 0x04001430 RID: 5168
	[PublicizedFrom(EAccessModifier.Private)]
	public double m_effectiveStrength;

	// Token: 0x04001431 RID: 5169
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_priority;

	// Token: 0x04001432 RID: 5170
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_refCount;

	// Token: 0x04001433 RID: 5171
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 m_pos;

	// Token: 0x04001434 RID: 5172
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 m_targetPos;

	// Token: 0x04001435 RID: 5173
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorPlayerState m_playerState;

	// Token: 0x04001436 RID: 5174
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_interruptsNonPlayerAttack;

	// Token: 0x04001437 RID: 5175
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_isDistraction;
}
