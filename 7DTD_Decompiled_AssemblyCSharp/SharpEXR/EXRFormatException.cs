using System;

namespace SharpEXR
{
	// Token: 0x020013FE RID: 5118
	public class EXRFormatException : Exception
	{
		// Token: 0x06009F49 RID: 40777 RVA: 0x0038441F File Offset: 0x0038261F
		public EXRFormatException()
		{
		}

		// Token: 0x06009F4A RID: 40778 RVA: 0x00384427 File Offset: 0x00382627
		public EXRFormatException(string message) : base(message)
		{
		}

		// Token: 0x06009F4B RID: 40779 RVA: 0x00384430 File Offset: 0x00382630
		public EXRFormatException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
