using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000632 RID: 1586
[Preserve]
public class MinEventActionLogMessage : MinEventActionBase
{
	// Token: 0x060030B3 RID: 12467 RVA: 0x0014C7D6 File Offset: 0x0014A9D6
	public override void Execute(MinEventParams _params)
	{
		Log.Out("MinEventLogMessage: {0}", new object[]
		{
			this.message
		});
	}

	// Token: 0x060030B4 RID: 12468 RVA: 0x0014C7F4 File Offset: 0x0014A9F4
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "message")
		{
			this.message = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x0400272D RID: 10029
	[PublicizedFrom(EAccessModifier.Private)]
	public string message;
}
