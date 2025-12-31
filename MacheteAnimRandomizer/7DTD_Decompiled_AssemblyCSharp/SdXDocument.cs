using System;
using System.IO;
using System.Xml.Linq;

// Token: 0x02000934 RID: 2356
public static class SdXDocument
{
	// Token: 0x060046E8 RID: 18152 RVA: 0x001BFBC0 File Offset: 0x001BDDC0
	public static XDocument Load(string filename)
	{
		XDocument result;
		using (Stream stream = SdFile.OpenRead(filename))
		{
			result = XDocument.Load(stream);
		}
		return result;
	}
}
