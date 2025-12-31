using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x0200125B RID: 4699
public static class XmlExtensions
{
	// Token: 0x0600934D RID: 37709 RVA: 0x003A9D8C File Offset: 0x003A7F8C
	public static string GetElementString(this XElement _elem)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('<');
		stringBuilder.Append(_elem.Name);
		foreach (XAttribute xattribute in _elem.Attributes())
		{
			stringBuilder.Append(' ');
			stringBuilder.Append(xattribute.Name);
			stringBuilder.Append("=\"");
			stringBuilder.Append(xattribute.Value);
			stringBuilder.Append('"');
		}
		stringBuilder.Append(' ');
		return stringBuilder.ToString();
	}

	// Token: 0x0600934E RID: 37710 RVA: 0x003A9E38 File Offset: 0x003A8038
	public static string GetXPath(this XElement _elem)
	{
		StringBuilder stringBuilder = new StringBuilder();
		XmlExtensions.getXPath(stringBuilder, _elem);
		return stringBuilder.ToString();
	}

	// Token: 0x0600934F RID: 37711 RVA: 0x003A9E4B File Offset: 0x003A804B
	public static string GetXPath(this XAttribute _attr)
	{
		StringBuilder stringBuilder = new StringBuilder();
		XmlExtensions.getXPath(stringBuilder, _attr.Parent);
		stringBuilder.Append("[@");
		stringBuilder.Append(_attr.Name);
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	// Token: 0x06009350 RID: 37712 RVA: 0x003A9E85 File Offset: 0x003A8085
	[PublicizedFrom(EAccessModifier.Private)]
	public static void getXPath(StringBuilder _sb, XElement _current)
	{
		if (_current.Parent != null)
		{
			XmlExtensions.getXPath(_sb, _current.Parent);
		}
		_sb.Append('/');
		_sb.Append(_current.Name);
	}

	// Token: 0x06009351 RID: 37713 RVA: 0x003A9EB1 File Offset: 0x003A80B1
	public static bool HasAttribute(this XElement _element, XName _name)
	{
		return _element.Attribute(_name) != null;
	}

	// Token: 0x06009352 RID: 37714 RVA: 0x003A9EC0 File Offset: 0x003A80C0
	public static string GetAttribute(this XElement _element, XName _name)
	{
		XAttribute xattribute = _element.Attribute(_name);
		if (xattribute == null)
		{
			return "";
		}
		return xattribute.Value;
	}

	// Token: 0x06009353 RID: 37715 RVA: 0x003A9EE4 File Offset: 0x003A80E4
	public static bool TryGetAttribute(this XElement _element, XName _name, out string _result)
	{
		XAttribute xattribute = _element.Attribute(_name);
		_result = ((xattribute != null) ? xattribute.Value : null);
		return xattribute != null;
	}

	// Token: 0x06009354 RID: 37716 RVA: 0x003A9F0C File Offset: 0x003A810C
	public static bool ParseAttribute(this XElement _element, XName _name, ref int _result)
	{
		string s;
		if (_element.TryGetAttribute(_name, out s))
		{
			_result = int.Parse(s);
			return true;
		}
		return false;
	}

	// Token: 0x06009355 RID: 37717 RVA: 0x003A9F30 File Offset: 0x003A8130
	public static bool ParseAttribute(this XElement _element, XName _name, ref string _result)
	{
		string text;
		if (_element.TryGetAttribute(_name, out text))
		{
			_result = text;
			return true;
		}
		return false;
	}

	// Token: 0x06009356 RID: 37718 RVA: 0x003A9F50 File Offset: 0x003A8150
	public static bool ParseAttribute(this XElement _element, XName _name, ref float _result)
	{
		string input;
		if (_element.TryGetAttribute(_name, out input))
		{
			_result = StringParsers.ParseFloat(input, 0, -1, NumberStyles.Any);
			return true;
		}
		return false;
	}

	// Token: 0x06009357 RID: 37719 RVA: 0x003A9F7C File Offset: 0x003A817C
	public static bool ParseAttribute(this XElement _element, XName _name, ref Vector2 _result)
	{
		string input;
		if (_element.TryGetAttribute(_name, out input))
		{
			_result = StringParsers.ParseVector2(input);
			return true;
		}
		return false;
	}

	// Token: 0x06009358 RID: 37720 RVA: 0x003A9FA4 File Offset: 0x003A81A4
	public static bool ParseAttribute(this XElement _element, XName _name, ref Vector3 _result)
	{
		string input;
		if (_element.TryGetAttribute(_name, out input))
		{
			_result = StringParsers.ParseVector3(input, 0, -1);
			return true;
		}
		return false;
	}

	// Token: 0x06009359 RID: 37721 RVA: 0x003A9FD0 File Offset: 0x003A81D0
	public static bool ParseAttribute(this XElement _element, XName _name, ref ulong _result)
	{
		string s;
		if (_element.TryGetAttribute(_name, out s))
		{
			_result = ulong.Parse(s);
			return true;
		}
		return false;
	}

	// Token: 0x0600935A RID: 37722 RVA: 0x003A9FF4 File Offset: 0x003A81F4
	public static bool ParseAttribute(this XElement _element, XName _name, ref bool _result)
	{
		string value;
		if (_element.TryGetAttribute(_name, out value))
		{
			_result = bool.Parse(value);
			return true;
		}
		return false;
	}

	// Token: 0x0600935B RID: 37723 RVA: 0x003AA018 File Offset: 0x003A8218
	public static List<XmlNode> ToList(this XmlNodeList _xmlNodeList)
	{
		List<XmlNode> list = new List<XmlNode>(_xmlNodeList.Count);
		foreach (object obj in _xmlNodeList)
		{
			XmlNode item = (XmlNode)obj;
			list.Add(item);
		}
		return list;
	}

	// Token: 0x0600935C RID: 37724 RVA: 0x003AA07C File Offset: 0x003A827C
	public static void CreateXmlDeclaration(this XmlDocument _doc)
	{
		XmlDeclaration newChild = _doc.CreateXmlDeclaration("1.0", "UTF-8", null);
		_doc.InsertBefore(newChild, _doc.DocumentElement);
	}

	// Token: 0x0600935D RID: 37725 RVA: 0x003AA0AC File Offset: 0x003A82AC
	public static XmlElement AddXmlElement(this XmlNode _node, string _name)
	{
		XmlDocument xmlDocument;
		if (_node.NodeType == XmlNodeType.Document)
		{
			xmlDocument = (XmlDocument)_node;
		}
		else
		{
			xmlDocument = _node.OwnerDocument;
		}
		XmlElement xmlElement = xmlDocument.CreateElement(_name);
		_node.AppendChild(xmlElement);
		return xmlElement;
	}

	// Token: 0x0600935E RID: 37726 RVA: 0x003AA0E4 File Offset: 0x003A82E4
	public static XmlComment AddXmlComment(this XmlNode _node, string _content)
	{
		XmlDocument xmlDocument;
		if (_node.NodeType == XmlNodeType.Document)
		{
			xmlDocument = (XmlDocument)_node;
		}
		else
		{
			xmlDocument = _node.OwnerDocument;
		}
		XmlComment xmlComment = xmlDocument.CreateComment(_content);
		_node.AppendChild(xmlComment);
		return xmlComment;
	}

	// Token: 0x0600935F RID: 37727 RVA: 0x003AA11C File Offset: 0x003A831C
	public static XmlText AddXmlText(this XmlNode _node, string _text)
	{
		XmlDocument xmlDocument;
		if (_node.NodeType == XmlNodeType.Document)
		{
			xmlDocument = (XmlDocument)_node;
		}
		else
		{
			xmlDocument = _node.OwnerDocument;
		}
		XmlText xmlText = xmlDocument.CreateTextNode(_text);
		_node.AppendChild(xmlText);
		return xmlText;
	}

	// Token: 0x06009360 RID: 37728 RVA: 0x003AA154 File Offset: 0x003A8354
	public static XmlElement SetAttrib(this XmlElement _element, string _name, string _value)
	{
		_element.SetAttribute(_name, _value);
		return _element;
	}

	// Token: 0x06009361 RID: 37729 RVA: 0x003AA15F File Offset: 0x003A835F
	public static XmlElement AddXmlKeyValueProperty(this XmlNode _node, string _name, string _value)
	{
		return _node.AddXmlElement("property").SetAttrib("name", _name).SetAttrib("value", _value);
	}

	// Token: 0x06009362 RID: 37730 RVA: 0x003AA182 File Offset: 0x003A8382
	public static bool TryGetAttribute(this XmlElement _element, string _name, out string _result)
	{
		if (!_element.HasAttribute(_name))
		{
			_result = null;
			return false;
		}
		_result = _element.GetAttribute(_name);
		return true;
	}
}
