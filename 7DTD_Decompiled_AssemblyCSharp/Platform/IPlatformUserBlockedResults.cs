using System;

namespace Platform
{
	// Token: 0x0200182F RID: 6191
	public interface IPlatformUserBlockedResults
	{
		// Token: 0x170014D9 RID: 5337
		// (get) Token: 0x0600B7F4 RID: 47092
		IPlatformUser User { get; }

		// Token: 0x0600B7F5 RID: 47093
		void Block(EBlockType blockType);

		// Token: 0x0600B7F6 RID: 47094 RVA: 0x00468964 File Offset: 0x00466B64
		void BlockAll()
		{
			foreach (EBlockType blockType in EnumUtils.Values<EBlockType>())
			{
				this.Block(blockType);
			}
		}

		// Token: 0x0600B7F7 RID: 47095
		void Error();
	}
}
