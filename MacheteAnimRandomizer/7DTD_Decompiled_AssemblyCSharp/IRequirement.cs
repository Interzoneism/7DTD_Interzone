using System;
using System.Collections.Generic;
using System.Xml.Linq;

// Token: 0x020005C5 RID: 1477
public interface IRequirement
{
	// Token: 0x06002F54 RID: 12116
	bool IsValid(MinEventParams _params);

	// Token: 0x06002F55 RID: 12117
	bool ParseXAttribute(XAttribute _attribute);

	// Token: 0x06002F56 RID: 12118
	void GetInfoStrings(ref List<string> list);

	// Token: 0x06002F57 RID: 12119
	string GetInfoString();

	// Token: 0x06002F58 RID: 12120
	void SetDescription(string desc);
}
