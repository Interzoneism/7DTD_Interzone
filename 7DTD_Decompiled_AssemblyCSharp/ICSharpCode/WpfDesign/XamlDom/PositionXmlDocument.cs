using System;
using System.Reflection;
using System.Xml;

namespace ICSharpCode.WpfDesign.XamlDom
{
	// Token: 0x0200137A RID: 4986
	public class PositionXmlDocument : XmlDocument
	{
		// Token: 0x06009C2C RID: 39980 RVA: 0x003E1488 File Offset: 0x003DF688
		public PositionXmlDocument()
		{
			this.setImplementation();
		}

		// Token: 0x06009C2D RID: 39981 RVA: 0x003E1496 File Offset: 0x003DF696
		[PublicizedFrom(EAccessModifier.ProtectedInternal)]
		public PositionXmlDocument(XmlImplementation _imp) : base(_imp)
		{
			this.setImplementation();
		}

		// Token: 0x06009C2E RID: 39982 RVA: 0x003E14A5 File Offset: 0x003DF6A5
		public PositionXmlDocument(XmlNameTable _nt) : base(_nt)
		{
			this.setImplementation();
		}

		// Token: 0x06009C2F RID: 39983 RVA: 0x003E14B4 File Offset: 0x003DF6B4
		[PublicizedFrom(EAccessModifier.Private)]
		public void setImplementation()
		{
			if (this.implementationFieldInfo == null)
			{
				this.implementationFieldInfo = typeof(XmlDocument).GetField("implementation", BindingFlags.Instance | BindingFlags.NonPublic);
			}
			this.implementationFieldInfo.SetValue(this, new PositionXmlImplementation());
		}

		// Token: 0x06009C30 RID: 39984 RVA: 0x003E14F1 File Offset: 0x003DF6F1
		public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			return new PositionXmlElement(prefix, localName, namespaceURI, this, this.lineInfo);
		}

		// Token: 0x06009C31 RID: 39985 RVA: 0x003E1502 File Offset: 0x003DF702
		public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
		{
			return new PositionXmlAttribute(prefix, localName, namespaceURI, this, this.lineInfo);
		}

		// Token: 0x06009C32 RID: 39986 RVA: 0x003E1513 File Offset: 0x003DF713
		public override XmlCDataSection CreateCDataSection(string data)
		{
			return new PositionXmlCDataSection(data, this, this.lineInfo);
		}

		// Token: 0x06009C33 RID: 39987 RVA: 0x003E1522 File Offset: 0x003DF722
		public override XmlComment CreateComment(string data)
		{
			return new PositionXmlComment(data, this, this.lineInfo);
		}

		// Token: 0x06009C34 RID: 39988 RVA: 0x003E1502 File Offset: 0x003DF702
		[PublicizedFrom(EAccessModifier.Protected)]
		public override XmlAttribute CreateDefaultAttribute(string prefix, string localName, string namespaceURI)
		{
			return new PositionXmlAttribute(prefix, localName, namespaceURI, this, this.lineInfo);
		}

		// Token: 0x06009C35 RID: 39989 RVA: 0x003E1531 File Offset: 0x003DF731
		public override XmlDocumentFragment CreateDocumentFragment()
		{
			return new PositionXmlDocumentFragment(this, this.lineInfo);
		}

		// Token: 0x06009C36 RID: 39990 RVA: 0x003E153F File Offset: 0x003DF73F
		public override XmlDocumentType CreateDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			return new PositionXmlDocumentType(name, publicId, systemId, internalSubset, this, this.lineInfo);
		}

		// Token: 0x06009C37 RID: 39991 RVA: 0x003E1552 File Offset: 0x003DF752
		public override XmlEntityReference CreateEntityReference(string name)
		{
			return new PositionXmlEntityReference(name, this, this.lineInfo);
		}

		// Token: 0x06009C38 RID: 39992 RVA: 0x003E1561 File Offset: 0x003DF761
		public override XmlNode CreateNode(string nodeTypeString, string name, string namespaceURI)
		{
			Console.WriteLine("CREATING NODE1: " + name);
			return base.CreateNode(nodeTypeString, name, namespaceURI);
		}

		// Token: 0x06009C39 RID: 39993 RVA: 0x003E157C File Offset: 0x003DF77C
		public override XmlNode CreateNode(XmlNodeType type, string name, string namespaceURI)
		{
			Console.WriteLine("CREATING NODE2: " + name);
			return base.CreateNode(type, name, namespaceURI);
		}

		// Token: 0x06009C3A RID: 39994 RVA: 0x003E1597 File Offset: 0x003DF797
		public override XmlNode CreateNode(XmlNodeType type, string prefix, string name, string namespaceURI)
		{
			Console.WriteLine("CREATING NODE3: " + name);
			return base.CreateNode(type, prefix, name, namespaceURI);
		}

		// Token: 0x06009C3B RID: 39995 RVA: 0x003E15B4 File Offset: 0x003DF7B4
		public override XmlProcessingInstruction CreateProcessingInstruction(string target, string data)
		{
			return new PositionXmlProcessingInstruction(target, data, this, this.lineInfo);
		}

		// Token: 0x06009C3C RID: 39996 RVA: 0x003E15C4 File Offset: 0x003DF7C4
		public override XmlSignificantWhitespace CreateSignificantWhitespace(string text)
		{
			return new PositionXmlSignificantWhitespace(text, this, this.lineInfo);
		}

		// Token: 0x06009C3D RID: 39997 RVA: 0x003E15D3 File Offset: 0x003DF7D3
		public override XmlText CreateTextNode(string text)
		{
			return new PositionXmlText(text, this, this.lineInfo);
		}

		// Token: 0x06009C3E RID: 39998 RVA: 0x003E15E2 File Offset: 0x003DF7E2
		public override XmlWhitespace CreateWhitespace(string text)
		{
			return new PositionXmlWhitespace(text, this, this.lineInfo);
		}

		// Token: 0x06009C3F RID: 39999 RVA: 0x003E15F1 File Offset: 0x003DF7F1
		public override XmlDeclaration CreateXmlDeclaration(string version, string encoding, string standalone)
		{
			return new PositionXmlDeclaration(version, encoding, standalone, this, this.lineInfo);
		}

		// Token: 0x06009C40 RID: 40000 RVA: 0x003E1604 File Offset: 0x003DF804
		public override void Load(XmlReader reader)
		{
			this.lineInfo = (reader as IXmlLineInfo);
			try
			{
				base.Load(reader);
			}
			finally
			{
				this.lineInfo = null;
			}
		}

		// Token: 0x040078D0 RID: 30928
		[PublicizedFrom(EAccessModifier.Private)]
		public IXmlLineInfo lineInfo;

		// Token: 0x040078D1 RID: 30929
		[PublicizedFrom(EAccessModifier.Private)]
		public FieldInfo implementationFieldInfo;
	}
}
