using System;

// Token: 0x020005B8 RID: 1464
public class BuffManager
{
	// Token: 0x06002F0C RID: 12044 RVA: 0x00142C60 File Offset: 0x00140E60
	public static void UpdateBuffTimers(BuffValue _ev, float _deltaTime)
	{
		BuffClass buffClass = _ev.BuffClass;
		if (buffClass != null)
		{
			buffClass.UpdateTimer(_ev, _deltaTime);
			return;
		}
		_ev.Remove = true;
	}

	// Token: 0x06002F0D RID: 12045 RVA: 0x00142C87 File Offset: 0x00140E87
	public static void Cleanup()
	{
		if (BuffManager.Buffs != null)
		{
			BuffManager.Buffs.Clear();
			BuffManager.Buffs = null;
		}
	}

	// Token: 0x06002F0E RID: 12046 RVA: 0x00142CA0 File Offset: 0x00140EA0
	public static void AddBuff(BuffClass _buffClass)
	{
		BuffManager.Buffs[_buffClass.Name] = _buffClass;
	}

	// Token: 0x06002F0F RID: 12047 RVA: 0x00142CB4 File Offset: 0x00140EB4
	public static BuffClass GetBuff(string _name)
	{
		BuffClass result;
		BuffManager.Buffs.TryGetValue(_name, out result);
		return result;
	}

	// Token: 0x04002545 RID: 9541
	public static CaseInsensitiveStringDictionary<BuffClass> Buffs;
}
