using System;

// Token: 0x0200117B RID: 4475
public abstract class DynamicClassFactory
{
	// Token: 0x06008BC8 RID: 35784
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract object[] getTable();

	// Token: 0x06008BC9 RID: 35785 RVA: 0x003854F8 File Offset: 0x003836F8
	public object Instantiate(string _className)
	{
		Type type = Type.GetType(_className);
		if (type != null)
		{
			return Activator.CreateInstance(type);
		}
		object[] table = this.getTable();
		for (int i = 0; i < table.Length / 2; i++)
		{
			if (_className.Equals(Utils.UnCryptFromBase64((char[])table[i * 2 + 1])))
			{
				try
				{
					return Activator.CreateInstance((Type)table[i * 2]);
				}
				catch (Exception ex)
				{
					throw new Exception("Class '" + Utils.UnCryptFromBase64((char[])table[i * 2 + 1]) + "' not found! Msg: " + ex.Message);
				}
			}
		}
		return null;
	}

	// Token: 0x06008BCA RID: 35786 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public DynamicClassFactory()
	{
	}
}
