using System;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class BuffEntityUINotification : EntityUINotification
{
	// Token: 0x06000CE5 RID: 3301 RVA: 0x00057BB3 File Offset: 0x00055DB3
	public BuffEntityUINotification(EntityAlive _owner, BuffValue _buff)
	{
		this.owner = _owner;
		this.buff = _buff;
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x06000CE6 RID: 3302 RVA: 0x00057BC9 File Offset: 0x00055DC9
	public BuffValue Buff
	{
		get
		{
			return this.buff;
		}
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06000CE7 RID: 3303 RVA: 0x00057BD1 File Offset: 0x00055DD1
	public string Icon
	{
		get
		{
			return this.buff.BuffClass.Icon;
		}
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x00057BE3 File Offset: 0x00055DE3
	public Color GetColor()
	{
		return this.Buff.BuffClass.IconColor;
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000CE9 RID: 3305 RVA: 0x00057BF8 File Offset: 0x00055DF8
	public float CurrentValue
	{
		get
		{
			if (this.buff != null && this.buff.BuffClass != null && this.buff.BuffClass.DisplayValueCVar != null)
			{
				return this.owner.Buffs.GetCustomVar(this.buff.BuffClass.DisplayValueCVar);
			}
			return 0f;
		}
	}

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x06000CEA RID: 3306 RVA: 0x00057C54 File Offset: 0x00055E54
	public string Units
	{
		get
		{
			if (this.buff == null || this.buff.BuffClass == null || this.buff.BuffClass.DisplayValueCVar == null)
			{
				return "";
			}
			if (this.buff.BuffClass.DisplayValueCVar.StartsWith("$") || this.buff.BuffClass.DisplayValueCVar.StartsWith(".") || this.buff.BuffClass.DisplayValueCVar.StartsWith("_"))
			{
				return "cvar";
			}
			return this.buff.BuffClass.DisplayValueCVar;
		}
	}

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x06000CEB RID: 3307 RVA: 0x00057CFC File Offset: 0x00055EFC
	public EnumEntityUINotificationDisplayMode DisplayMode
	{
		get
		{
			EnumEntityUINotificationDisplayMode result = (this.buff != null && this.buff.BuffClass != null) ? this.buff.BuffClass.DisplayType : EnumEntityUINotificationDisplayMode.IconOnly;
			if (this.buff.BuffClass.DisplayValueCVar != null && this.buff.BuffClass.DisplayValueCVar != "")
			{
				result = EnumEntityUINotificationDisplayMode.IconPlusCurrentValue;
			}
			return result;
		}
	}

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x06000CEC RID: 3308 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public EnumEntityUINotificationSubject Subject
	{
		get
		{
			return EnumEntityUINotificationSubject.Buff;
		}
	}

	// Token: 0x04000AAD RID: 2733
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive owner;

	// Token: 0x04000AAE RID: 2734
	[PublicizedFrom(EAccessModifier.Private)]
	public BuffValue buff;
}
