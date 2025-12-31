using System;

// Token: 0x0200111D RID: 4381
public static class BackedArrayHandleModeExtensions
{
	// Token: 0x060089AC RID: 35244 RVA: 0x0037C9A4 File Offset: 0x0037ABA4
	public static bool CanRead(this BackedArrayHandleMode mode)
	{
		bool result;
		if (mode != BackedArrayHandleMode.ReadOnly)
		{
			if (mode != BackedArrayHandleMode.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("mode", mode, string.Format("Unknown mode: {0}", mode));
			}
			result = true;
		}
		else
		{
			result = true;
		}
		return result;
	}

	// Token: 0x060089AD RID: 35245 RVA: 0x0037C9E4 File Offset: 0x0037ABE4
	public static bool CanWrite(this BackedArrayHandleMode mode)
	{
		bool result;
		if (mode != BackedArrayHandleMode.ReadOnly)
		{
			if (mode != BackedArrayHandleMode.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("mode", mode, string.Format("Unknown mode: {0}", mode));
			}
			result = true;
		}
		else
		{
			result = false;
		}
		return result;
	}
}
