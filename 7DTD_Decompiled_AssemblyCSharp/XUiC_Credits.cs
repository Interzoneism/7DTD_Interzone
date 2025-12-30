using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C76 RID: 3190
[Preserve]
public class XUiC_Credits : XUiController
{
	// Token: 0x06006248 RID: 25160 RVA: 0x0027DBBC File Offset: 0x0027BDBC
	public override void Init()
	{
		base.Init();
		XUiC_Credits.ID = base.WindowGroup.ID;
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnBack") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += this.BtnBack_OnPressed;
		}
		this.credigtsGridXui = (base.GetChildById("creditsGrid").ViewComponent as XUiV_Grid);
		this.creditsGrid = this.credigtsGridXui.UiTransform;
		this.startPos = -this.credigtsGridXui.Size.y;
		this.getTemplates("categoryTemplates", this.categoryTemplates, ref this.defaultCategoryTemplate);
		this.getTemplates("creditTemplates", this.creditTemplates, ref this.defaultCreditTemplate);
	}

	// Token: 0x06006249 RID: 25161 RVA: 0x0027DC76 File Offset: 0x0027BE76
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "file")
		{
			this.creditsFile = _value;
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x0600624A RID: 25162 RVA: 0x0027DC98 File Offset: 0x0027BE98
	[PublicizedFrom(EAccessModifier.Private)]
	public void getTemplates(string _parentRectName, Dictionary<string, GameObject> _templatesDict, ref string _defaultTemplateField)
	{
		XUiController childById = base.GetChildById(_parentRectName);
		if (childById == null)
		{
			return;
		}
		for (int i = 0; i < childById.Children.Count; i++)
		{
			XUiView viewComponent = childById.Children[i].ViewComponent;
			string id = viewComponent.ID;
			if (_defaultTemplateField == null)
			{
				_defaultTemplateField = id;
			}
			_templatesDict[id] = viewComponent.UiTransform.gameObject;
		}
	}

	// Token: 0x0600624B RID: 25163 RVA: 0x0027DCF9 File Offset: 0x0027BEF9
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_MainMenu.ID, true, false, true);
	}

	// Token: 0x0600624C RID: 25164 RVA: 0x0027DD38 File Offset: 0x0027BF38
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.windowGroup.isModal)
		{
			this.windowGroup.openWindowOnEsc = XUiC_MainMenu.ID;
		}
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.categoryTemplates)
		{
			string text;
			GameObject gameObject;
			keyValuePair.Deconstruct(out text, out gameObject);
			gameObject.SetActive(false);
		}
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.creditTemplates)
		{
			string text;
			GameObject gameObject;
			keyValuePair.Deconstruct(out text, out gameObject);
			gameObject.SetActive(false);
		}
		this.LoadCredits();
		this.firstUpdate = true;
	}

	// Token: 0x0600624D RID: 25165 RVA: 0x0027DE14 File Offset: 0x0027C014
	public override void OnClose()
	{
		base.OnClose();
		this.creditsGrid.localPosition = new Vector3(0f, (float)this.startPos, 0f);
	}

	// Token: 0x0600624E RID: 25166 RVA: 0x0027DE40 File Offset: 0x0027C040
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.firstUpdate)
		{
			this.creditsGrid.localPosition = new Vector3(0f, (float)this.startPos, 0f);
			this.firstUpdate = false;
			return;
		}
		this.creditsGrid.localPosition += new Vector3(0f, 40f * _dt, 0f);
	}

	// Token: 0x0600624F RID: 25167 RVA: 0x0027DEB4 File Offset: 0x0027C0B4
	[PublicizedFrom(EAccessModifier.Private)]
	public XmlFile loadXml()
	{
		if (this.creditsFile == null)
		{
			return new XmlFile(Resources.Load("Data/Credits/Credits") as TextAsset);
		}
		string text = ModManager.PatchModPathString(this.creditsFile);
		if (text != null)
		{
			return new XmlFile(Path.GetDirectoryName(text), Path.GetFileName(text), false, false);
		}
		return null;
	}

	// Token: 0x06006250 RID: 25168 RVA: 0x0027DF02 File Offset: 0x0027C102
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject getTemplate(string _name, string _defaultTemplateName, Dictionary<string, GameObject> _templatesDict)
	{
		if (string.IsNullOrEmpty(_name))
		{
			_name = _defaultTemplateName;
		}
		return _templatesDict[_name];
	}

	// Token: 0x06006251 RID: 25169 RVA: 0x0027DF18 File Offset: 0x0027C118
	[PublicizedFrom(EAccessModifier.Protected)]
	public void LoadCredits()
	{
		if (this.loaded || this.defaultCategoryTemplate == null || this.defaultCreditTemplate == null)
		{
			return;
		}
		this.loaded = true;
		XmlFile xmlFile = this.loadXml();
		XElement xelement = (xmlFile != null) ? xmlFile.XmlDoc.Root : null;
		if (xelement == null)
		{
			Log.Error("Credits.xml not found or no XML root");
			return;
		}
		int num = 10000;
		foreach (XElement xelement2 in xelement.Elements("category"))
		{
			XAttribute xattribute = xelement2.Attribute("name");
			string text = ((xattribute != null) ? xattribute.Value : null) ?? "";
			XAttribute xattribute2 = xelement2.Attribute("template");
			string name = ((xattribute2 != null) ? xattribute2.Value : null) ?? this.defaultCategoryTemplate;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.getTemplate(name, this.defaultCategoryTemplate, this.categoryTemplates), this.creditsGrid.transform);
			gameObject.name = num++.ToString() + gameObject.name;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.Find("caption").GetComponent<UILabel>().text = text;
			gameObject.SetActive(true);
			foreach (XElement xelement3 in xelement2.Elements())
			{
				XAttribute xattribute3 = xelement3.Attribute("name");
				string text2 = ((xattribute3 != null) ? xattribute3.Value : null) ?? "";
				XAttribute xattribute4 = xelement3.Attribute("center_text");
				string text3 = ((xattribute4 != null) ? xattribute4.Value : null) ?? "";
				XAttribute xattribute5 = xelement3.Attribute("contribution");
				string text4 = ((xattribute5 != null) ? xattribute5.Value : null) ?? "";
				XAttribute xattribute6 = xelement3.Attribute("template");
				name = (((xattribute6 != null) ? xattribute6.Value : null) ?? this.defaultCreditTemplate);
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.getTemplate(name, this.defaultCreditTemplate, this.creditTemplates), this.creditsGrid.transform);
				gameObject2.name = num++.ToString() + gameObject2.name;
				gameObject2.transform.localScale = Vector3.one;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.Find("name").GetComponent<UILabel>().text = text2;
				gameObject2.transform.Find("contribution").GetComponent<UILabel>().text = text4;
				gameObject2.transform.Find("centertext").GetComponent<UILabel>().text = text3;
				gameObject2.transform.Find("line").gameObject.SetActive(text2.Length > 0 && text4.Length > 0);
				gameObject2.SetActive(true);
			}
			GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.getTemplate("", this.defaultCreditTemplate, this.creditTemplates), this.creditsGrid.transform);
			gameObject3.name = num++.ToString() + gameObject3.name;
			gameObject3.transform.localScale = Vector3.one;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.Find("name").GetComponent<UILabel>().text = "";
			gameObject3.transform.Find("contribution").GetComponent<UILabel>().text = "";
			gameObject3.transform.Find("centertext").GetComponent<UILabel>().text = "";
			gameObject3.transform.Find("line").gameObject.SetActive(false);
			gameObject3.SetActive(true);
		}
		this.creditsGrid.GetComponent<UIGrid>().Reposition();
	}

	// Token: 0x040049F1 RID: 18929
	public static string ID = "";

	// Token: 0x040049F2 RID: 18930
	[PublicizedFrom(EAccessModifier.Private)]
	public string creditsFile;

	// Token: 0x040049F3 RID: 18931
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform creditsGrid;

	// Token: 0x040049F4 RID: 18932
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Grid credigtsGridXui;

	// Token: 0x040049F5 RID: 18933
	[PublicizedFrom(EAccessModifier.Private)]
	public string defaultCategoryTemplate;

	// Token: 0x040049F6 RID: 18934
	[PublicizedFrom(EAccessModifier.Private)]
	public string defaultCreditTemplate;

	// Token: 0x040049F7 RID: 18935
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, GameObject> categoryTemplates = new CaseInsensitiveStringDictionary<GameObject>();

	// Token: 0x040049F8 RID: 18936
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, GameObject> creditTemplates = new CaseInsensitiveStringDictionary<GameObject>();

	// Token: 0x040049F9 RID: 18937
	[PublicizedFrom(EAccessModifier.Private)]
	public int startPos;

	// Token: 0x040049FA RID: 18938
	[PublicizedFrom(EAccessModifier.Private)]
	public bool firstUpdate;

	// Token: 0x040049FB RID: 18939
	[PublicizedFrom(EAccessModifier.Private)]
	public bool loaded;
}
