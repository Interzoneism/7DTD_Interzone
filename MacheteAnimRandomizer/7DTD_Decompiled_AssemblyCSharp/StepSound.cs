using System;

// Token: 0x02000168 RID: 360
public class StepSound
{
	// Token: 0x06000AC0 RID: 2752 RVA: 0x00045764 File Offset: 0x00043964
	public StepSound(string _name)
	{
		this.name = _name;
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x00045773 File Offset: 0x00043973
	public static StepSound FromString(string _name)
	{
		return new StepSound(_name);
	}

	// Token: 0x0400097E RID: 2430
	public static StepSound stone = new StepSound("stone");

	// Token: 0x0400097F RID: 2431
	public string name;
}
