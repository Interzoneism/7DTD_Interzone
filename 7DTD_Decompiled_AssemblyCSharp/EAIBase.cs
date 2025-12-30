using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003DB RID: 987
[Preserve]
public abstract class EAIBase
{
	// Token: 0x06001DEB RID: 7659 RVA: 0x000BA9EB File Offset: 0x000B8BEB
	public virtual void Init(EntityAlive _theEntity)
	{
		this.executeDelay = 0.5f;
		this.manager = _theEntity.aiManager;
		this.theEntity = _theEntity;
	}

	// Token: 0x06001DEC RID: 7660 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetData(DictionarySave<string, string> data)
	{
	}

	// Token: 0x06001DED RID: 7661 RVA: 0x000BAA0C File Offset: 0x000B8C0C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void GetData(DictionarySave<string, string> data, string name, ref float value)
	{
		string input;
		float num;
		if (data.TryGetValue(name, out input) && StringParsers.TryParseFloat(input, out num, 0, -1, NumberStyles.Any))
		{
			value = num;
		}
	}

	// Token: 0x06001DEE RID: 7662 RVA: 0x000BAA38 File Offset: 0x000B8C38
	[PublicizedFrom(EAccessModifier.Protected)]
	public void GetData(DictionarySave<string, string> data, string name, ref int value)
	{
		string input;
		int num;
		if (data.TryGetValue(name, out input) && StringParsers.TryParseSInt32(input, out num, 0, -1, NumberStyles.Integer))
		{
			value = num;
		}
	}

	// Token: 0x06001DEF RID: 7663
	public abstract bool CanExecute();

	// Token: 0x06001DF0 RID: 7664 RVA: 0x000BAA60 File Offset: 0x000B8C60
	public virtual bool Continue()
	{
		return this.CanExecute();
	}

	// Token: 0x06001DF1 RID: 7665 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsContinuous()
	{
		return true;
	}

	// Token: 0x06001DF2 RID: 7666 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Start()
	{
	}

	// Token: 0x06001DF3 RID: 7667 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Reset()
	{
	}

	// Token: 0x06001DF4 RID: 7668 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Update()
	{
	}

	// Token: 0x06001DF5 RID: 7669 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsPathUsageBlocked(PathEntity _path)
	{
		return false;
	}

	// Token: 0x06001DF6 RID: 7670 RVA: 0x000BAA68 File Offset: 0x000B8C68
	[PublicizedFrom(EAccessModifier.Protected)]
	public static Vector3 GetTargetPos(EntityAlive theEntity)
	{
		if (theEntity.GetAttackTarget() != null)
		{
			return theEntity.GetAttackTarget().position;
		}
		return theEntity.InvestigatePosition;
	}

	// Token: 0x06001DF7 RID: 7671 RVA: 0x000BAA8A File Offset: 0x000B8C8A
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool EntityHasTarget(EntityAlive theEntity)
	{
		return theEntity.GetAttackTarget() != null || theEntity.HasInvestigatePosition;
	}

	// Token: 0x17000360 RID: 864
	// (get) Token: 0x06001DF8 RID: 7672 RVA: 0x000BAAA2 File Offset: 0x000B8CA2
	public GameRandom Random
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.manager.random;
		}
	}

	// Token: 0x17000361 RID: 865
	// (get) Token: 0x06001DF9 RID: 7673 RVA: 0x000BAAAF File Offset: 0x000B8CAF
	public float RandomFloat
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.manager.random.RandomFloat;
		}
	}

	// Token: 0x06001DFA RID: 7674 RVA: 0x000BAAC1 File Offset: 0x000B8CC1
	[PublicizedFrom(EAccessModifier.Protected)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetRandom(int maxExclusive)
	{
		return this.manager.random.RandomRange(maxExclusive);
	}

	// Token: 0x06001DFB RID: 7675 RVA: 0x000BAAD4 File Offset: 0x000B8CD4
	public override string ToString()
	{
		if (this.shortedTypeName == null)
		{
			this.shortedTypeName = this.GetTypeName().Substring(3);
		}
		return this.shortedTypeName;
	}

	// Token: 0x06001DFC RID: 7676 RVA: 0x000BAAF6 File Offset: 0x000B8CF6
	public string GetTypeName()
	{
		if (this.cachedTypeName == null)
		{
			this.cachedTypeName = base.GetType().Name;
		}
		return this.cachedTypeName;
	}

	// Token: 0x06001DFD RID: 7677 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public EAIBase()
	{
	}

	// Token: 0x04001490 RID: 5264
	public EAIManager manager;

	// Token: 0x04001491 RID: 5265
	public EntityAlive theEntity;

	// Token: 0x04001492 RID: 5266
	public float executeWaitTime;

	// Token: 0x04001493 RID: 5267
	public float executeDelay;

	// Token: 0x04001494 RID: 5268
	public int MutexBits;

	// Token: 0x04001495 RID: 5269
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedTypeName;

	// Token: 0x04001496 RID: 5270
	[PublicizedFrom(EAccessModifier.Private)]
	public string shortedTypeName;
}
