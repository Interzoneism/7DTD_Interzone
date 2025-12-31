using System;

// Token: 0x020008A7 RID: 2215
public abstract class BaseObjectiveModifier
{
	// Token: 0x170006AC RID: 1708
	// (get) Token: 0x0600408C RID: 16524 RVA: 0x001A4DDB File Offset: 0x001A2FDB
	// (set) Token: 0x0600408D RID: 16525 RVA: 0x001A4DE3 File Offset: 0x001A2FE3
	public BaseObjective OwnerObjective { get; set; }

	// Token: 0x0600408E RID: 16526 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public BaseObjectiveModifier()
	{
	}

	// Token: 0x0600408F RID: 16527 RVA: 0x001A4DEC File Offset: 0x001A2FEC
	public void HandleAddHooks()
	{
		this.AddHooks();
	}

	// Token: 0x06004090 RID: 16528 RVA: 0x001A4DF4 File Offset: 0x001A2FF4
	public void HandleRemoveHooks()
	{
		this.RemoveHooks();
	}

	// Token: 0x06004091 RID: 16529 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void AddHooks()
	{
	}

	// Token: 0x06004092 RID: 16530 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void RemoveHooks()
	{
	}

	// Token: 0x06004093 RID: 16531 RVA: 0x00019766 File Offset: 0x00017966
	public virtual BaseObjectiveModifier Clone()
	{
		return null;
	}

	// Token: 0x06004094 RID: 16532 RVA: 0x001A4DFC File Offset: 0x001A2FFC
	public virtual void ParseProperties(DynamicProperties properties)
	{
		this.Properties = properties;
		this.OwnerObjective.OwnerQuestClass.HandleVariablesForProperties(properties);
	}

	// Token: 0x040033AF RID: 13231
	public DynamicProperties Properties;
}
