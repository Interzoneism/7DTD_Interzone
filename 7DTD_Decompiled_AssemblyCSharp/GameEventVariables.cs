using System;
using System.Collections.Generic;

// Token: 0x020004BB RID: 1211
public class GameEventVariables
{
	// Token: 0x060027B9 RID: 10169 RVA: 0x001016C8 File Offset: 0x000FF8C8
	public void ModifyEventVariable(string name, GameEventVariables.OperationTypes operation, int value, int minValue = -2147483648, int maxValue = 2147483647)
	{
		if (this.EventVariables == null)
		{
			this.EventVariables = new Dictionary<string, object>();
		}
		if (this.operationType == GameEventVariables.OperationTypes.Set)
		{
			this.EventVariables[name] = Utils.FastClamp(value, minValue, maxValue);
			return;
		}
		int num = 0;
		this.ParseVarInt(name, ref num);
		switch (this.operationType)
		{
		case GameEventVariables.OperationTypes.Add:
			this.EventVariables[name] = Utils.FastClamp(num + value, minValue, maxValue);
			return;
		case GameEventVariables.OperationTypes.Subtract:
			this.EventVariables[name] = Utils.FastClamp(num - value, minValue, maxValue);
			return;
		case GameEventVariables.OperationTypes.Multiply:
			this.EventVariables[name] = Utils.FastClamp(num * value, minValue, maxValue);
			return;
		default:
			return;
		}
	}

	// Token: 0x060027BA RID: 10170 RVA: 0x0010178C File Offset: 0x000FF98C
	public void ModifyEventVariable(string name, GameEventVariables.OperationTypes operation, float value, float minValue = -3.4028235E+38f, float maxValue = 3.4028235E+38f)
	{
		if (this.EventVariables == null)
		{
			this.EventVariables = new Dictionary<string, object>();
		}
		if (this.operationType == GameEventVariables.OperationTypes.Set)
		{
			this.EventVariables[name] = Utils.FastClamp(value, minValue, maxValue);
			return;
		}
		float num = 0f;
		this.ParseVarFloat(name, ref num);
		switch (this.operationType)
		{
		case GameEventVariables.OperationTypes.Add:
			this.EventVariables[name] = Utils.FastClamp(num + value, minValue, maxValue);
			return;
		case GameEventVariables.OperationTypes.Subtract:
			this.EventVariables[name] = Utils.FastClamp(num - value, minValue, maxValue);
			return;
		case GameEventVariables.OperationTypes.Multiply:
			this.EventVariables[name] = Utils.FastClamp(num * value, minValue, maxValue);
			return;
		default:
			return;
		}
	}

	// Token: 0x060027BB RID: 10171 RVA: 0x00101854 File Offset: 0x000FFA54
	public void SetEventVariable(string name, bool value)
	{
		if (this.EventVariables == null)
		{
			this.EventVariables = new Dictionary<string, object>();
		}
		this.EventVariables[name] = value;
	}

	// Token: 0x060027BC RID: 10172 RVA: 0x0010187B File Offset: 0x000FFA7B
	public void SetEventVariable(string name, string value)
	{
		if (this.EventVariables == null)
		{
			this.EventVariables = new Dictionary<string, object>();
		}
		this.EventVariables[name] = value;
	}

	// Token: 0x060027BD RID: 10173 RVA: 0x0010189D File Offset: 0x000FFA9D
	public void ParseVarInt(string varName, ref int optionalValue)
	{
		if (this.EventVariables == null || !this.EventVariables.ContainsKey(varName))
		{
			return;
		}
		optionalValue = (int)this.EventVariables[varName];
	}

	// Token: 0x060027BE RID: 10174 RVA: 0x001018C9 File Offset: 0x000FFAC9
	public void ParseVarFloat(string varName, ref float optionalValue)
	{
		if (this.EventVariables == null || !this.EventVariables.ContainsKey(varName))
		{
			return;
		}
		optionalValue = (float)this.EventVariables[varName];
	}

	// Token: 0x060027BF RID: 10175 RVA: 0x001018F5 File Offset: 0x000FFAF5
	public void ParseString(string varName, ref string optionalValue)
	{
		if (this.EventVariables == null || !this.EventVariables.ContainsKey(varName))
		{
			return;
		}
		optionalValue = (string)this.EventVariables[varName];
	}

	// Token: 0x060027C0 RID: 10176 RVA: 0x00101921 File Offset: 0x000FFB21
	public void ParseBool(string varName, ref bool optionalValue)
	{
		if (this.EventVariables == null || !this.EventVariables.ContainsKey(varName))
		{
			return;
		}
		optionalValue = (bool)this.EventVariables[varName];
	}

	// Token: 0x04001E67 RID: 7783
	public Dictionary<string, object> EventVariables = new Dictionary<string, object>();

	// Token: 0x04001E68 RID: 7784
	public GameEventVariables.OperationTypes operationType;

	// Token: 0x020004BC RID: 1212
	public enum OperationTypes
	{
		// Token: 0x04001E6A RID: 7786
		Set,
		// Token: 0x04001E6B RID: 7787
		Add,
		// Token: 0x04001E6C RID: 7788
		Subtract,
		// Token: 0x04001E6D RID: 7789
		Multiply
	}
}
