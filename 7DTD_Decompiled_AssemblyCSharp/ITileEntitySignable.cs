using System;

// Token: 0x02000AF0 RID: 2800
public interface ITileEntitySignable : ITileEntity
{
	// Token: 0x0600561B RID: 22043
	void SetText(AuthoredText _authoredText, bool _syncData = true);

	// Token: 0x0600561C RID: 22044
	void SetText(string _text, bool _syncData = true, PlatformUserIdentifierAbs _signingPlayer = null);

	// Token: 0x0600561D RID: 22045
	AuthoredText GetAuthoredText();

	// Token: 0x0600561E RID: 22046
	bool CanRenderString(string _text);
}
