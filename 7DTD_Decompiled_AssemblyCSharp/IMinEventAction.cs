using System;
using System.Xml.Linq;

// Token: 0x02000660 RID: 1632
public interface IMinEventAction
{
	// Token: 0x0600315C RID: 12636
	bool CanExecute(MinEventTypes _eventType, MinEventParams _params);

	// Token: 0x0600315D RID: 12637
	void Execute(MinEventParams _params);

	// Token: 0x0600315E RID: 12638
	bool ParseXmlAttribute(XAttribute _attribute);

	// Token: 0x0600315F RID: 12639
	void ParseXMLPostProcess();
}
