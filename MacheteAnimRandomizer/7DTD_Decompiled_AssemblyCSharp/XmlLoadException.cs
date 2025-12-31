using System;
using System.Xml;
using System.Xml.Linq;

// Token: 0x0200125A RID: 4698
public class XmlLoadException : Exception
{
	// Token: 0x06009348 RID: 37704 RVA: 0x003A9D24 File Offset: 0x003A7F24
	public XmlLoadException(string _xmlName, XElement _element, string _message) : this(XmlLoadException.buildMessage(_element, _xmlName, _message))
	{
	}

	// Token: 0x06009349 RID: 37705 RVA: 0x003A9D34 File Offset: 0x003A7F34
	public XmlLoadException(string _xmlName, XElement _element, string _message, Exception _innerException) : this(XmlLoadException.buildMessage(_element, _xmlName, _message), _innerException)
	{
	}

	// Token: 0x0600934A RID: 37706 RVA: 0x003A9D48 File Offset: 0x003A7F48
	[PublicizedFrom(EAccessModifier.Private)]
	public static string buildMessage(XElement _element, string _xmlName, string _message)
	{
		return string.Format("Error loading {0}: {1} (line {2} at pos {3})", new object[]
		{
			_xmlName,
			_message,
			((IXmlLineInfo)_element).LineNumber,
			((IXmlLineInfo)_element).LinePosition
		});
	}

	// Token: 0x0600934B RID: 37707 RVA: 0x00384427 File Offset: 0x00382627
	[PublicizedFrom(EAccessModifier.Private)]
	public XmlLoadException(string _message) : base(_message)
	{
	}

	// Token: 0x0600934C RID: 37708 RVA: 0x00384430 File Offset: 0x00382630
	public XmlLoadException(string _message, Exception _innerException) : base(_message, _innerException)
	{
	}
}
