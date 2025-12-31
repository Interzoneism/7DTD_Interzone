using System;

// Token: 0x02000AC4 RID: 2756
public class QTDataElement
{
	// Token: 0x060054DC RID: 21724 RVA: 0x0022AAD0 File Offset: 0x00228CD0
	public QTDataElement()
	{
		this.Data = null;
		this.Key = 0L;
		this.LLPosX = 0;
		this.LLPosY = 0;
	}

	// Token: 0x060054DD RID: 21725 RVA: 0x0022AAF5 File Offset: 0x00228CF5
	public QTDataElement(int _LLPosX, int _LLPosY, byte[] _Data)
	{
		this.LLPosX = _LLPosX;
		this.LLPosY = _LLPosY;
		this.Data = _Data;
	}

	// Token: 0x040041B5 RID: 16821
	public byte[] Data;

	// Token: 0x040041B6 RID: 16822
	public long Key;

	// Token: 0x040041B7 RID: 16823
	public int LLPosX;

	// Token: 0x040041B8 RID: 16824
	public int LLPosY;
}
