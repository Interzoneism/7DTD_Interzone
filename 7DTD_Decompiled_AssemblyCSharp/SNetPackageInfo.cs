using System;

// Token: 0x020011CE RID: 4558
public struct SNetPackageInfo
{
	// Token: 0x06008E74 RID: 36468 RVA: 0x00390E19 File Offset: 0x0038F019
	public SNetPackageInfo(int _id, int _size)
	{
		this.Tick = GameTimer.Instance.ticks;
		this.Id = _id;
		this.Size = _size;
	}

	// Token: 0x04006E38 RID: 28216
	public readonly ulong Tick;

	// Token: 0x04006E39 RID: 28217
	public readonly int Id;

	// Token: 0x04006E3A RID: 28218
	public readonly int Size;
}
