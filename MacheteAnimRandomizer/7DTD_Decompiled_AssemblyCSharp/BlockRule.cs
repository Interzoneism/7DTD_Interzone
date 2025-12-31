using System;

// Token: 0x02000194 RID: 404
public class BlockRule<O, M>
{
	// Token: 0x06000C46 RID: 3142 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public BlockRule()
	{
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x00053E87 File Offset: 0x00052087
	public BlockRule(O _output, M[] _mask)
	{
		this.Output = _output;
		this.Mask = _mask;
	}

	// Token: 0x04000A4D RID: 2637
	public O Output;

	// Token: 0x04000A4E RID: 2638
	public M[] Mask;
}
