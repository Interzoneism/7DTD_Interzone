using System;

// Token: 0x02001267 RID: 4711
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class XmlPatchMethodAttribute : Attribute
{
	// Token: 0x060093AC RID: 37804 RVA: 0x003ABB3A File Offset: 0x003A9D3A
	public XmlPatchMethodAttribute(string _patchName)
	{
		this.PatchName = _patchName;
	}

	// Token: 0x060093AD RID: 37805 RVA: 0x003ABB50 File Offset: 0x003A9D50
	public XmlPatchMethodAttribute(string _patchName, bool _requiresXpath)
	{
		this.PatchName = _patchName;
		this.RequiresXpath = _requiresXpath;
	}

	// Token: 0x040070B0 RID: 28848
	public readonly string PatchName;

	// Token: 0x040070B1 RID: 28849
	public readonly bool RequiresXpath = true;
}
