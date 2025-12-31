using System;
using System.Xml;
using System.Xml.Linq;

// Token: 0x02001259 RID: 4697
public class XmlPatchException : Exception
{
	// Token: 0x06009343 RID: 37699 RVA: 0x003A9CB4 File Offset: 0x003A7EB4
	public XmlPatchException(XElement _patchElement, string _patchMethodName, string _message) : this(XmlPatchException.buildMessage(_patchElement, _patchMethodName, _message))
	{
	}

	// Token: 0x06009344 RID: 37700 RVA: 0x003A9CC4 File Offset: 0x003A7EC4
	public XmlPatchException(XElement _patchElement, string _patchMethodName, string _message, Exception _innerException) : this(XmlPatchException.buildMessage(_patchElement, _patchMethodName, _message), _innerException)
	{
	}

	// Token: 0x06009345 RID: 37701 RVA: 0x003A9CD8 File Offset: 0x003A7ED8
	[PublicizedFrom(EAccessModifier.Private)]
	public static string buildMessage(XElement _patchElement, string _patchMethodName, string _message)
	{
		return string.Format("XML.{0} ({1}, line {2} at pos {3}): {4}", new object[]
		{
			_patchMethodName,
			_patchElement.GetXPath(),
			((IXmlLineInfo)_patchElement).LineNumber,
			((IXmlLineInfo)_patchElement).LinePosition,
			_message
		});
	}

	// Token: 0x06009346 RID: 37702 RVA: 0x00384427 File Offset: 0x00382627
	[PublicizedFrom(EAccessModifier.Private)]
	public XmlPatchException(string _message) : base(_message)
	{
	}

	// Token: 0x06009347 RID: 37703 RVA: 0x00384430 File Offset: 0x00382630
	public XmlPatchException(string _message, Exception _innerException) : base(_message, _innerException)
	{
	}
}
