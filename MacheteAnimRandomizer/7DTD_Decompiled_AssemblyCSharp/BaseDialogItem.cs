using System;

// Token: 0x020002A4 RID: 676
public class BaseDialogItem
{
	// Token: 0x17000209 RID: 521
	// (get) Token: 0x06001320 RID: 4896 RVA: 0x000761BD File Offset: 0x000743BD
	// (set) Token: 0x06001321 RID: 4897 RVA: 0x000761C5 File Offset: 0x000743C5
	public virtual string HeaderName { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x1700020A RID: 522
	// (get) Token: 0x06001322 RID: 4898 RVA: 0x000761CE File Offset: 0x000743CE
	// (set) Token: 0x06001323 RID: 4899 RVA: 0x000761D6 File Offset: 0x000743D6
	public Dialog OwnerDialog { get; set; }

	// Token: 0x04000CA0 RID: 3232
	public string ID;
}
