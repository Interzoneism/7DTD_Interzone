using System;
using System.Collections.Generic;

// Token: 0x02000982 RID: 2434
public class EntitySpawnerClassForDay
{
	// Token: 0x06004964 RID: 18788 RVA: 0x001D04C0 File Offset: 0x001CE6C0
	public void AddForDay(int _day, EntitySpawnerClass _class)
	{
		if (_day != 0 && this.days.Count == 0)
		{
			this.days.Add(_class);
		}
		while (this.days.Count <= _day)
		{
			this.days.Add(null);
		}
		this.days[_day] = _class;
	}

	// Token: 0x06004965 RID: 18789 RVA: 0x001D0514 File Offset: 0x001CE714
	public EntitySpawnerClass Day(int _day)
	{
		if (this.days.Count == 0)
		{
			return null;
		}
		if (this.bWrapDays && _day > 0 && _day >= this.days.Count)
		{
			if (this.days.Count > 1)
			{
				_day %= this.days.Count - 1;
				if (_day == 0)
				{
					_day = this.days.Count - 1;
				}
			}
			else
			{
				_day = 1;
			}
			if (_day == 0)
			{
				_day++;
			}
		}
		else if (this.bClampDays && _day >= this.days.Count && this.days.Count > 0)
		{
			_day = this.days.Count - 1;
		}
		if (_day >= this.days.Count || this.days[_day] == null)
		{
			return this.days[0];
		}
		return this.days[_day];
	}

	// Token: 0x06004966 RID: 18790 RVA: 0x001D05F2 File Offset: 0x001CE7F2
	public int Count()
	{
		return this.days.Count;
	}

	// Token: 0x04003888 RID: 14472
	public bool bDynamicSpawner;

	// Token: 0x04003889 RID: 14473
	public bool bWrapDays;

	// Token: 0x0400388A RID: 14474
	public bool bClampDays;

	// Token: 0x0400388B RID: 14475
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntitySpawnerClass> days = new List<EntitySpawnerClass>();
}
