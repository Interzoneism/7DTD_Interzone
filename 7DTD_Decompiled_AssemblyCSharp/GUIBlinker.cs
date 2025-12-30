using System;

// Token: 0x02000FCB RID: 4043
public class GUIBlinker
{
	// Token: 0x060080D9 RID: 32985 RVA: 0x003452F6 File Offset: 0x003434F6
	public GUIBlinker(float _ms)
	{
		this.ms = _ms;
	}

	// Token: 0x060080DA RID: 32986 RVA: 0x0034530C File Offset: 0x0034350C
	public bool Draw(float _curTime)
	{
		if (_curTime - this.lastBlinkTime > this.ms)
		{
			this.lastBlinkTime = _curTime;
			this.bResult = !this.bResult;
		}
		return this.bResult;
	}

	// Token: 0x04006383 RID: 25475
	[PublicizedFrom(EAccessModifier.Private)]
	public float ms;

	// Token: 0x04006384 RID: 25476
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastBlinkTime;

	// Token: 0x04006385 RID: 25477
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bResult = true;
}
