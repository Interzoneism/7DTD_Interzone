using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

// Token: 0x02001260 RID: 4704
public class XmlPatcher
{
	// Token: 0x0600938C RID: 37772 RVA: 0x003AB1BC File Offset: 0x003A93BC
	public static IEnumerator LoadAndPatchConfig(string _configName, Action<XmlFile> _callback)
	{
		if (!_configName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
		{
			_configName += ".xml";
		}
		Exception xmlLoadException = null;
		XmlFile xmlFile = new XmlFile(GameIO.GetGameDir("Data/Config"), _configName, delegate(Exception _exception)
		{
			if (_exception != null)
			{
				xmlLoadException = _exception;
			}
		});
		while (!xmlFile.Loaded && xmlLoadException == null)
		{
			yield return null;
		}
		if (xmlLoadException != null)
		{
			Log.Error("XML loader: Loading base XML '" + xmlFile.Filename + "' failed:");
			Log.Exception(xmlLoadException);
			yield break;
		}
		MicroStopwatch msw = new MicroStopwatch(true);
		foreach (Mod mod in ModManager.GetLoadedMods())
		{
			string text = mod.Path + "/Config/" + _configName;
			if (SdFile.Exists(text))
			{
				try
				{
					XmlFile xmlFile2 = XmlPatcher.ReadPatchXmlWithFixedModFolders(mod, text);
					if (xmlFile2 == null)
					{
						continue;
					}
					XElement root = xmlFile2.XmlDoc.Root;
					XmlPatcher.PatchXml(xmlFile, root, xmlFile2, mod);
				}
				catch (Exception e)
				{
					Log.Error(string.Concat(new string[]
					{
						"XML loader: Patching '",
						xmlFile.Filename,
						"' from mod '",
						mod.Name,
						"' failed:"
					}));
					Log.Exception(e);
				}
				if (msw.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
				{
					yield return null;
					msw.ResetAndRestart();
				}
			}
		}
		List<Mod>.Enumerator enumerator = default(List<Mod>.Enumerator);
		_callback(xmlFile);
		yield break;
		yield break;
	}

	// Token: 0x0600938D RID: 37773 RVA: 0x003AB1D4 File Offset: 0x003A93D4
	public static XmlFile ReadPatchXmlWithFixedModFolders(Mod _parentMod, string _file)
	{
		string text = SdFile.ReadAllText(_file, Encoding.UTF8);
		text = text.Replace("@modfolder:", "@modfolder(" + _parentMod.Name + "):");
		string fileName = Path.GetFileName(_file);
		try
		{
			return new XmlFile(text, Path.GetDirectoryName(_file), fileName, true);
		}
		catch (Exception e)
		{
			Log.Error(string.Concat(new string[]
			{
				"XML loader: Loading XML patch file '",
				fileName,
				"' from mod '",
				_parentMod.Name,
				"' failed:"
			}));
			Log.Exception(e);
		}
		return null;
	}

	// Token: 0x0600938E RID: 37774 RVA: 0x003AB278 File Offset: 0x003A9478
	public static bool PatchXml(XmlFile _xmlFile, XElement _containerElement, XmlFile _patchFile, Mod _patchingMod)
	{
		if (_containerElement == null)
		{
			return false;
		}
		bool flag = true;
		foreach (XElement xelement in _containerElement.Elements())
		{
			bool flag2 = XmlPatcher.singlePatch(_xmlFile, xelement, _patchFile, _patchingMod);
			if (!flag2)
			{
				IXmlLineInfo xmlLineInfo = xelement;
				Log.Warning(string.Format("XML patch for \"{0}\" from mod \"{1}\" did not apply: {2} (line {3} at pos {4})", new object[]
				{
					_xmlFile.Filename,
					_patchingMod.Name,
					xelement.GetElementString(),
					xmlLineInfo.LineNumber,
					xmlLineInfo.LinePosition
				}));
			}
			flag = (flag && flag2);
		}
		return flag;
	}

	// Token: 0x0600938F RID: 37775 RVA: 0x003AB328 File Offset: 0x003A9528
	public static IEnumerator ApplyConditionalXmlBlocks(string _xmlName, XmlFile _xmlFile, MicroStopwatch _timer, XmlPatcher.EEvaluator _evaluator, Action _errorCallback)
	{
		if (_timer != null)
		{
			_timer.ResetAndRestart();
		}
		string xpath = (_evaluator == XmlPatcher.EEvaluator.Host) ? "//conditional[(@evaluator='host' or not(@evaluator)) and not(ancestor::conditional[@evaluator='host' or not(@evaluator)])]" : "//conditional[@evaluator='client' and not(ancestor::conditional)]";
		List<XObject> list;
		while (_xmlFile.GetXpathResults(xpath, out list))
		{
			using (List<XObject>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XObject xobject = enumerator.Current;
					XElement xelement = xobject as XElement;
					if (xelement != null)
					{
						XElement xelement2 = XmlPatchConditionEvaluator.FindActiveConditionalBranchElement(_xmlFile, xelement);
						if (xelement2 != null)
						{
							xelement.AddAfterSelf(xelement2.Nodes());
						}
						xelement.Remove();
					}
				}
				continue;
			}
			break;
		}
		_xmlFile.ClearXpathResults();
		if (_timer != null)
		{
			_timer.Stop();
		}
		if (((_timer != null) ? _timer.ElapsedMilliseconds : 0L) > 50L)
		{
			Log.Out(string.Format("Conditionals handling in {0} for {1} took {2} ms", _xmlName, _evaluator, _timer.ElapsedMilliseconds));
		}
		if (_timer != null)
		{
			_timer.Start();
		}
		yield break;
	}

	// Token: 0x06009390 RID: 37776 RVA: 0x003AB34C File Offset: 0x003A954C
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool singlePatch(XmlFile _targetFile, XElement _patchElement, XmlFile _patchFile, Mod _patchingMod)
	{
		string localName = _patchElement.Name.LocalName;
		XmlPatcher.PatchMethodDefinition patchMethodDefinition;
		if (!XmlPatcher.XpathPatchMethods.TryGetValue(localName, out patchMethodDefinition))
		{
			Log.Warning(string.Format("XML.Patch ({0}, line {1} at pos {2}): Patch type ({3}) unknown", new object[]
			{
				_patchElement.GetXPath(),
				((IXmlLineInfo)_patchElement).LineNumber,
				((IXmlLineInfo)_patchElement).LinePosition,
				localName
			}));
			return false;
		}
		if (patchMethodDefinition.RequiresXpath && !_patchElement.HasAttribute("xpath"))
		{
			throw new Exception(string.Format("XML.Patch ({0}, line {1} at pos {2}): Patch element does not have an 'xpath' attribute", _patchElement.GetXPath(), ((IXmlLineInfo)_patchElement).LineNumber, ((IXmlLineInfo)_patchElement).LinePosition));
		}
		string attribute = _patchElement.GetAttribute("xpath");
		bool result;
		try
		{
			result = (patchMethodDefinition.Delegate(_targetFile, attribute, _patchElement, _patchFile, _patchingMod) > 0);
		}
		catch (XPathException ex)
		{
			throw new XPathException(string.Format("XML.Patch ({0}, line {1} at pos {2}): XPath evaluation failed: {3}", new object[]
			{
				_patchElement.GetXPath(),
				((IXmlLineInfo)_patchElement).LineNumber,
				((IXmlLineInfo)_patchElement).LinePosition,
				ex.Message
			}));
		}
		catch (XmlException)
		{
			Log.Error(string.Format("XML.Patch ({0}, line {1} at pos {2}): Unknown XML exception while applying patch:", _patchElement.GetXPath(), ((IXmlLineInfo)_patchElement).LineNumber, ((IXmlLineInfo)_patchElement).LinePosition));
			throw;
		}
		return result;
	}

	// Token: 0x06009391 RID: 37777 RVA: 0x003AB4B8 File Offset: 0x003A96B8
	[PublicizedFrom(EAccessModifier.Private)]
	static XmlPatcher()
	{
		ReflectionHelpers.FindTypesWithAttribute<XmlPatchMethodsClassAttribute>(new Action<Type>(XmlPatcher.<.cctor>g__TypeFoundCallback|9_0), true);
	}

	// Token: 0x06009392 RID: 37778 RVA: 0x003AB4D8 File Offset: 0x003A96D8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void addXmlFilePatchMethod(string _patchName, string _methodName, bool _requiresXpath = true)
	{
		MethodInfo method = typeof(XmlFile).GetMethod(_methodName);
		XmlPatcher.XpathDelegate @delegate = (XmlPatcher.XpathDelegate)Delegate.CreateDelegate(typeof(XmlPatcher.XpathDelegate), method);
		if (!XmlPatcher.XpathPatchMethods.TryAdd(_patchName, new XmlPatcher.PatchMethodDefinition(@delegate, _requiresXpath)))
		{
			XmlPatcher.redeclarationLog(_patchName, method);
		}
	}

	// Token: 0x06009393 RID: 37779 RVA: 0x003AB528 File Offset: 0x003A9728
	[PublicizedFrom(EAccessModifier.Private)]
	public static void addXmlFilePatchMethod(string _patchName, MethodInfo _method, bool _requiresXpath = true)
	{
		XmlPatcher.XpathDelegate @delegate = (XmlPatcher.XpathDelegate)Delegate.CreateDelegate(typeof(XmlPatcher.XpathDelegate), _method);
		if (!XmlPatcher.XpathPatchMethods.TryAdd(_patchName, new XmlPatcher.PatchMethodDefinition(@delegate, _requiresXpath)))
		{
			XmlPatcher.redeclarationLog(_patchName, _method);
		}
	}

	// Token: 0x06009394 RID: 37780 RVA: 0x003AB568 File Offset: 0x003A9768
	[PublicizedFrom(EAccessModifier.Private)]
	public static void redeclarationLog(string _patchName, MethodInfo _newMethod)
	{
		MethodInfo method = XmlPatcher.XpathPatchMethods[_patchName].Delegate.Method;
		Log.Warning(string.Concat(new string[]
		{
			"XML patch method '",
			_patchName,
			"' already defined in ",
			method.DeclaringType.FullName,
			".",
			method.Name,
			". Redeclaration in ",
			_newMethod.DeclaringType.FullName,
			".",
			_newMethod.Name
		}));
	}

	// Token: 0x06009396 RID: 37782 RVA: 0x003AB5F7 File Offset: 0x003A97F7
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <.cctor>g__TypeFoundCallback|9_0(Type _type)
	{
		ReflectionHelpers.GetMethodsWithAttribute<XmlPatchMethodAttribute>(_type, new Action<MethodInfo>(XmlPatcher.<.cctor>g__MethodFoundCallback|9_1));
	}

	// Token: 0x06009397 RID: 37783 RVA: 0x003AB60C File Offset: 0x003A980C
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <.cctor>g__MethodFoundCallback|9_1(MethodInfo _method)
	{
		if (!ReflectionHelpers.MethodCompatibleWithDelegate<XmlPatcher.XpathDelegate>(_method))
		{
			Log.Error(string.Concat(new string[]
			{
				"XML patch method ",
				_method.DeclaringType.FullName,
				".",
				_method.Name,
				" does not have the expected signature"
			}));
			return;
		}
		foreach (Attribute attribute in _method.GetCustomAttributes(typeof(XmlPatchMethodAttribute)))
		{
			XmlPatchMethodAttribute xmlPatchMethodAttribute = attribute as XmlPatchMethodAttribute;
			if (xmlPatchMethodAttribute != null)
			{
				string patchName = xmlPatchMethodAttribute.PatchName;
				bool requiresXpath = xmlPatchMethodAttribute.RequiresXpath;
				XmlPatcher.addXmlFilePatchMethod(patchName, _method, requiresXpath);
				break;
			}
		}
	}

	// Token: 0x0400709B RID: 28827
	public static readonly Dictionary<string, XmlPatcher.PatchMethodDefinition> XpathPatchMethods = new CaseInsensitiveStringDictionary<XmlPatcher.PatchMethodDefinition>();

	// Token: 0x02001261 RID: 4705
	public enum EEvaluator
	{
		// Token: 0x0400709D RID: 28829
		Host,
		// Token: 0x0400709E RID: 28830
		Client
	}

	// Token: 0x02001262 RID: 4706
	public struct PatchMethodDefinition
	{
		// Token: 0x06009398 RID: 37784 RVA: 0x003AB6C4 File Offset: 0x003A98C4
		public PatchMethodDefinition(XmlPatcher.XpathDelegate _delegate, bool _requiresXpath)
		{
			this.Delegate = _delegate;
			this.RequiresXpath = _requiresXpath;
		}

		// Token: 0x0400709F RID: 28831
		public readonly XmlPatcher.XpathDelegate Delegate;

		// Token: 0x040070A0 RID: 28832
		public readonly bool RequiresXpath;
	}

	// Token: 0x02001263 RID: 4707
	// (Invoke) Token: 0x0600939A RID: 37786
	public delegate int XpathDelegate(XmlFile _targetFile, string _xpath, XElement _patchSourceElement, XmlFile _patchFile, Mod _patchingMod);
}
