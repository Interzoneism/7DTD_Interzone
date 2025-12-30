using System;

// Token: 0x02001296 RID: 4758
public class vp_Activity<V> : vp_Activity
{
	// Token: 0x060094F7 RID: 38135 RVA: 0x003B6809 File Offset: 0x003B4A09
	public vp_Activity(string name) : base(name)
	{
	}

	// Token: 0x060094F8 RID: 38136 RVA: 0x003B6812 File Offset: 0x003B4A12
	public bool TryStart<T>(T argument)
	{
		if (this.m_Active)
		{
			return false;
		}
		this.m_Argument = argument;
		return base.TryStart(true);
	}
}
