using System;

// Token: 0x02000042 RID: 66
[Serializable]
public class InvStat
{
	// Token: 0x06000160 RID: 352 RVA: 0x0000E4EB File Offset: 0x0000C6EB
	public static string GetName(InvStat.Identifier i)
	{
		return i.ToString();
	}

	// Token: 0x06000161 RID: 353 RVA: 0x0000E4FC File Offset: 0x0000C6FC
	public static string GetDescription(InvStat.Identifier i)
	{
		switch (i)
		{
		case InvStat.Identifier.Strength:
			return "Strength increases melee damage";
		case InvStat.Identifier.Constitution:
			return "Constitution increases health";
		case InvStat.Identifier.Agility:
			return "Agility increases armor";
		case InvStat.Identifier.Intelligence:
			return "Intelligence increases mana";
		case InvStat.Identifier.Damage:
			return "Damage adds to the amount of damage done in combat";
		case InvStat.Identifier.Crit:
			return "Crit increases the chance of landing a critical strike";
		case InvStat.Identifier.Armor:
			return "Armor protects from damage";
		case InvStat.Identifier.Health:
			return "Health prolongs life";
		case InvStat.Identifier.Mana:
			return "Mana increases the number of spells that can be cast";
		default:
			return null;
		}
	}

	// Token: 0x06000162 RID: 354 RVA: 0x0000E56C File Offset: 0x0000C76C
	public static int CompareArmor(InvStat a, InvStat b)
	{
		int num = (int)a.id;
		int num2 = (int)b.id;
		if (a.id == InvStat.Identifier.Armor)
		{
			num -= 10000;
		}
		else if (a.id == InvStat.Identifier.Damage)
		{
			num -= 5000;
		}
		if (b.id == InvStat.Identifier.Armor)
		{
			num2 -= 10000;
		}
		else if (b.id == InvStat.Identifier.Damage)
		{
			num2 -= 5000;
		}
		if (a.amount < 0)
		{
			num += 1000;
		}
		if (b.amount < 0)
		{
			num2 += 1000;
		}
		if (a.modifier == InvStat.Modifier.Percent)
		{
			num += 100;
		}
		if (b.modifier == InvStat.Modifier.Percent)
		{
			num2 += 100;
		}
		if (num < num2)
		{
			return -1;
		}
		if (num > num2)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06000163 RID: 355 RVA: 0x0000E61C File Offset: 0x0000C81C
	public static int CompareWeapon(InvStat a, InvStat b)
	{
		int num = (int)a.id;
		int num2 = (int)b.id;
		if (a.id == InvStat.Identifier.Damage)
		{
			num -= 10000;
		}
		else if (a.id == InvStat.Identifier.Armor)
		{
			num -= 5000;
		}
		if (b.id == InvStat.Identifier.Damage)
		{
			num2 -= 10000;
		}
		else if (b.id == InvStat.Identifier.Armor)
		{
			num2 -= 5000;
		}
		if (a.amount < 0)
		{
			num += 1000;
		}
		if (b.amount < 0)
		{
			num2 += 1000;
		}
		if (a.modifier == InvStat.Modifier.Percent)
		{
			num += 100;
		}
		if (b.modifier == InvStat.Modifier.Percent)
		{
			num2 += 100;
		}
		if (num < num2)
		{
			return -1;
		}
		if (num > num2)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x04000207 RID: 519
	public InvStat.Identifier id;

	// Token: 0x04000208 RID: 520
	public InvStat.Modifier modifier;

	// Token: 0x04000209 RID: 521
	public int amount;

	// Token: 0x02000043 RID: 67
	public enum Identifier
	{
		// Token: 0x0400020B RID: 523
		Strength,
		// Token: 0x0400020C RID: 524
		Constitution,
		// Token: 0x0400020D RID: 525
		Agility,
		// Token: 0x0400020E RID: 526
		Intelligence,
		// Token: 0x0400020F RID: 527
		Damage,
		// Token: 0x04000210 RID: 528
		Crit,
		// Token: 0x04000211 RID: 529
		Armor,
		// Token: 0x04000212 RID: 530
		Health,
		// Token: 0x04000213 RID: 531
		Mana,
		// Token: 0x04000214 RID: 532
		Other
	}

	// Token: 0x02000044 RID: 68
	public enum Modifier
	{
		// Token: 0x04000216 RID: 534
		Added,
		// Token: 0x04000217 RID: 535
		Percent
	}
}
