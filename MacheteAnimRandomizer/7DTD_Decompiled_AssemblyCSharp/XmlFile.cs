using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using UnityEngine;

// Token: 0x0200125C RID: 4700
public class XmlFile
{
	// Token: 0x17000F25 RID: 3877
	// (get) Token: 0x06009363 RID: 37731 RVA: 0x003AA19C File Offset: 0x003A839C
	// (set) Token: 0x06009364 RID: 37732 RVA: 0x003AA1A4 File Offset: 0x003A83A4
	public bool Loaded { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06009365 RID: 37733 RVA: 0x003AA1AD File Offset: 0x003A83AD
	public XmlFile(XmlFile _orig)
	{
		this.Directory = _orig.Directory;
		this.Filename = _orig.Filename;
		this.Loaded = _orig.Loaded;
		this.XmlDoc = new XDocument(_orig.XmlDoc);
	}

	// Token: 0x06009366 RID: 37734 RVA: 0x003AA1EC File Offset: 0x003A83EC
	public XmlFile(string _name)
	{
		this.Directory = GameIO.GetGameDir("Data/Config");
		this.Filename = ((!_name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) ? (_name + ".xml") : _name);
		this.load(this.Directory, this.Filename, false);
	}

	// Token: 0x06009367 RID: 37735 RVA: 0x003AA244 File Offset: 0x003A8444
	public XmlFile(string _text, string _directory, string _filename, bool _throwExc = false)
	{
		this.Directory = _directory;
		this.Filename = _filename;
		this.toXml(_text, _filename, _throwExc);
	}

	// Token: 0x06009368 RID: 37736 RVA: 0x003AA264 File Offset: 0x003A8464
	public XmlFile(TextAsset _ta)
	{
		using (MemoryStream memoryStream = new MemoryStream(_ta.bytes))
		{
			this.load(memoryStream, _ta.name, false);
		}
	}

	// Token: 0x06009369 RID: 37737 RVA: 0x003AA2B0 File Offset: 0x003A84B0
	public XmlFile(byte[] _data, bool _throwExc = false)
	{
		using (MemoryStream memoryStream = new MemoryStream(_data))
		{
			this.load(memoryStream, null, _throwExc);
		}
	}

	// Token: 0x0600936A RID: 37738 RVA: 0x003AA2F0 File Offset: 0x003A84F0
	public XmlFile(string _directory, string _file, bool _loadAsync = false, bool _throwExc = false)
	{
		XmlFile <>4__this = this;
		this.Directory = _directory;
		this.Filename = ((!_file.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) ? (_file + ".xml") : _file);
		if (!_loadAsync)
		{
			this.load(_directory, this.Filename, false);
			return;
		}
		ThreadManager.AddSingleTask(delegate(ThreadManager.TaskInfo _)
		{
			<>4__this.load(_directory, <>4__this.Filename, false);
		}, null, null, true);
	}

	// Token: 0x0600936B RID: 37739 RVA: 0x003AA374 File Offset: 0x003A8574
	public XmlFile(string _directory, string _file, Action<Exception> _doneCallback)
	{
		XmlFile <>4__this = this;
		this.Directory = _directory;
		this.Filename = ((!_file.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) ? (_file + ".xml") : _file);
		ThreadManager.AddSingleTask(delegate(ThreadManager.TaskInfo _)
		{
			try
			{
				<>4__this.load(_directory, <>4__this.Filename, false);
				_doneCallback(null);
			}
			catch (Exception obj)
			{
				_doneCallback(obj);
			}
		}, null, null, true);
	}

	// Token: 0x0600936C RID: 37740 RVA: 0x003AA3E5 File Offset: 0x003A85E5
	public XmlFile(Stream _stream)
	{
		this.load(_stream, null, false);
	}

	// Token: 0x0600936D RID: 37741 RVA: 0x003AA3F8 File Offset: 0x003A85F8
	public string SerializeToString(bool _minified = false)
	{
		string result;
		using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, XmlFile.GetWriterSettings(!_minified, Encoding.UTF8)))
			{
				this.XmlDoc.WriteTo(xmlWriter);
			}
			result = stringWriter.ToString();
		}
		return result;
	}

	// Token: 0x0600936E RID: 37742 RVA: 0x003AA470 File Offset: 0x003A8670
	public byte[] SerializeToBytes(bool _minified = false, Encoding _encoding = null)
	{
		PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
		this.SerializeToStream(pooledExpandableMemoryStream, _minified, _encoding);
		byte[] result = pooledExpandableMemoryStream.ToArray();
		MemoryPools.poolMemoryStream.FreeSync(pooledExpandableMemoryStream);
		return result;
	}

	// Token: 0x0600936F RID: 37743 RVA: 0x003AA4A4 File Offset: 0x003A86A4
	public void SerializeToFile(string _path, bool _minified = false, Encoding _encoding = null)
	{
		using (Stream stream = SdFile.Create(_path))
		{
			this.SerializeToStream(stream, _minified, _encoding);
		}
	}

	// Token: 0x06009370 RID: 37744 RVA: 0x003AA4E0 File Offset: 0x003A86E0
	public void SerializeToStream(Stream _stream, bool _minified = false, Encoding _encoding = null)
	{
		if (_encoding == null)
		{
			_encoding = Encoding.UTF8;
		}
		using (XmlWriter xmlWriter = XmlWriter.Create(_stream, XmlFile.GetWriterSettings(!_minified, _encoding)))
		{
			this.XmlDoc.WriteTo(xmlWriter);
		}
	}

	// Token: 0x06009371 RID: 37745 RVA: 0x003AA530 File Offset: 0x003A8730
	[PublicizedFrom(EAccessModifier.Private)]
	public static XmlWriterSettings GetWriterSettings(bool _indent, Encoding _encoding)
	{
		return new XmlWriterSettings
		{
			Encoding = _encoding,
			Indent = _indent,
			OmitXmlDeclaration = true
		};
	}

	// Token: 0x06009372 RID: 37746 RVA: 0x003AA54C File Offset: 0x003A874C
	[PublicizedFrom(EAccessModifier.Private)]
	public void toXml(string _data, string _filename = null, bool _throwExc = false)
	{
		try
		{
			this.XmlDoc = XDocument.Parse(_data, LoadOptions.SetLineInfo);
			this.Loaded = true;
		}
		catch (Exception e)
		{
			if (_throwExc)
			{
				throw;
			}
			Log.Error("Failed parsing XML" + ((!string.IsNullOrEmpty(_filename)) ? (" (" + _filename + ")") : "") + ":");
			Log.Exception(e);
		}
	}

	// Token: 0x06009373 RID: 37747 RVA: 0x003AA5C0 File Offset: 0x003A87C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void load(byte[] _bytes, bool _throwExc = false)
	{
		using (MemoryStream memoryStream = new MemoryStream(_bytes))
		{
			this.load(memoryStream, null, _throwExc);
		}
	}

	// Token: 0x06009374 RID: 37748 RVA: 0x003AA5FC File Offset: 0x003A87FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void load(string _directory, string _file, bool _throwExc = false)
	{
		if (_file == null)
		{
			SdFileInfo[] directory = GameIO.GetDirectory(_directory, "*.xml");
			if (directory.Length == 0)
			{
				return;
			}
			_file = directory[0].Name;
		}
		string text = _directory + "/" + _file;
		using (Stream stream = SdFile.OpenRead(text))
		{
			this.load(stream, text, _throwExc);
		}
	}

	// Token: 0x06009375 RID: 37749 RVA: 0x003AA660 File Offset: 0x003A8860
	[PublicizedFrom(EAccessModifier.Private)]
	public void load(Stream _stream, string _name = null, bool _throwExc = false)
	{
		try
		{
			using (StreamReader streamReader = new StreamReader(_stream, Encoding.UTF8))
			{
				this.XmlDoc = XDocument.Load(streamReader, LoadOptions.SetLineInfo);
				this.Loaded = true;
			}
		}
		catch (Exception e)
		{
			if (_throwExc)
			{
				throw;
			}
			Log.Error("Failed parsing XML" + ((!string.IsNullOrEmpty(_name)) ? (" (" + _name + ")") : "") + ":");
			Log.Exception(e);
		}
	}

	// Token: 0x06009376 RID: 37750 RVA: 0x003AA6F8 File Offset: 0x003A88F8
	public void RemoveComments()
	{
		this.XmlDoc.DescendantNodes().OfType<XComment>().Remove<XComment>();
	}

	// Token: 0x06009377 RID: 37751 RVA: 0x003AA70F File Offset: 0x003A890F
	public bool GetXpathResults(string _xpath, out List<XObject> _matchList)
	{
		if (this.tempXpathMatchList == null)
		{
			this.tempXpathMatchList = new List<XObject>();
		}
		if (this.GetXpathResultsInList(_xpath, this.tempXpathMatchList))
		{
			_matchList = this.tempXpathMatchList;
			return true;
		}
		_matchList = null;
		return false;
	}

	// Token: 0x06009378 RID: 37752 RVA: 0x003AA741 File Offset: 0x003A8941
	public int ClearXpathResults()
	{
		int count = this.tempXpathMatchList.Count;
		this.tempXpathMatchList.Clear();
		return count;
	}

	// Token: 0x06009379 RID: 37753 RVA: 0x003AA75C File Offset: 0x003A895C
	public bool GetXpathResultsInList(string _xpath, List<XObject> _matchList)
	{
		if (_matchList == null)
		{
			throw new ArgumentNullException("_matchList", "GetXpathResultsInList can not be called with a null _matchList argument");
		}
		_matchList.Clear();
		IEnumerable enumerable = this.XmlDoc.XPathEvaluate(_xpath) as IEnumerable;
		if (enumerable == null)
		{
			return false;
		}
		_matchList.AddRange(enumerable.Cast<XObject>());
		return _matchList.Count != 0;
	}

	// Token: 0x04007091 RID: 28817
	public readonly string Directory;

	// Token: 0x04007092 RID: 28818
	public readonly string Filename;

	// Token: 0x04007094 RID: 28820
	public XDocument XmlDoc;

	// Token: 0x04007095 RID: 28821
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XObject> tempXpathMatchList;
}
