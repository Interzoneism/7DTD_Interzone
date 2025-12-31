using System;
using System.IO;
using System.Xml;

// Token: 0x02000936 RID: 2358
public static class SdXmlDocumentExtensions
{
	// Token: 0x060046EA RID: 18154 RVA: 0x001BFC34 File Offset: 0x001BDE34
	public static void SdLoad(this XmlDocument xmlDoc, string filename)
	{
		using (Stream stream = SdFile.OpenRead(filename))
		{
			xmlDoc.Load(stream);
		}
	}

	// Token: 0x060046EB RID: 18155 RVA: 0x001BFC6C File Offset: 0x001BDE6C
	public static void SdSave(this XmlDocument xmlDoc, string filename)
	{
		using (Stream stream = SdFile.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
		{
			xmlDoc.Save(stream);
		}
	}
}
