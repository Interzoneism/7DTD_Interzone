using System;
using System.IO;
using System.Xml.Linq;

// Token: 0x02000935 RID: 2357
public static class SdXDocumentExtensions
{
	// Token: 0x060046E9 RID: 18153 RVA: 0x001BFBF8 File Offset: 0x001BDDF8
	public static void SdSave(this XDocument xmlDoc, string filename)
	{
		using (Stream stream = SdFile.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
		{
			xmlDoc.Save(stream);
		}
	}
}
