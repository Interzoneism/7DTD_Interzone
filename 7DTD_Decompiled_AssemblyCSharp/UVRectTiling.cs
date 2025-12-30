using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02001232 RID: 4658
public struct UVRectTiling
{
	// Token: 0x06009177 RID: 37239 RVA: 0x0039FCC0 File Offset: 0x0039DEC0
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"x=\"",
			this.uv.x.ToCultureInvariantString(),
			"\" y=\"",
			this.uv.y.ToCultureInvariantString(),
			"\" w=\"",
			this.uv.width.ToCultureInvariantString(),
			"\" h=\"",
			this.uv.height.ToCultureInvariantString(),
			"\" blockw=\"",
			this.blockW.ToString(),
			"\" blockh=\"",
			this.blockH.ToString(),
			"\" color=\"",
			this.color.r.ToCultureInvariantString(),
			",",
			this.color.g.ToCultureInvariantString(),
			",",
			this.color.b.ToCultureInvariantString(),
			"\" globaluv=\"",
			this.bGlobalUV.ToString(),
			"\" index=\"",
			this.index.ToString(),
			"\""
		});
	}

	// Token: 0x06009178 RID: 37240 RVA: 0x0039FE04 File Offset: 0x0039E004
	public void ToXML(XmlElement _elem)
	{
		_elem.SetAttrib("x", this.uv.x.ToCultureInvariantString());
		_elem.SetAttrib("y", this.uv.y.ToCultureInvariantString());
		_elem.SetAttrib("w", this.uv.width.ToCultureInvariantString());
		_elem.SetAttrib("h", this.uv.height.ToCultureInvariantString());
		_elem.SetAttrib("blockw", this.blockW.ToString());
		_elem.SetAttrib("blockh", this.blockH.ToString());
		_elem.SetAttrib("color", string.Concat(new string[]
		{
			this.color.r.ToCultureInvariantString(),
			",",
			this.color.g.ToCultureInvariantString(),
			",",
			this.color.b.ToCultureInvariantString()
		}));
		_elem.SetAttrib("globaluv", this.bGlobalUV.ToString());
		_elem.SetAttrib("index", this.index.ToString());
	}

	// Token: 0x06009179 RID: 37241 RVA: 0x0039FF40 File Offset: 0x0039E140
	public void FromXML(XElement _element)
	{
		this.uv.x = StringParsers.ParseFloat(_element.GetAttribute("x"), 0, -1, NumberStyles.Any);
		this.uv.y = StringParsers.ParseFloat(_element.GetAttribute("y"), 0, -1, NumberStyles.Any);
		this.uv.width = StringParsers.ParseFloat(_element.GetAttribute("w"), 0, -1, NumberStyles.Any);
		this.uv.height = StringParsers.ParseFloat(_element.GetAttribute("h"), 0, -1, NumberStyles.Any);
		this.blockW = int.Parse(_element.GetAttribute("blockw"));
		this.blockH = int.Parse(_element.GetAttribute("blockh"));
		this.bSwitchUV = (_element.HasAttribute("switchuv") && StringParsers.ParseBool(_element.GetAttribute("switchuv"), 0, -1, true));
		this.bGlobalUV = (_element.HasAttribute("globaluv") && StringParsers.ParseBool(_element.GetAttribute("globaluv"), 0, -1, true));
		this.material = MaterialBlock.fromString(_element.GetAttribute("material"));
		this.textureName = _element.GetAttribute("texture");
		string[] array = _element.GetAttribute("color").Split(',', StringSplitOptions.None);
		this.color = new Color(StringParsers.ParseFloat(array[0], 0, -1, NumberStyles.Any), StringParsers.ParseFloat(array[1], 0, -1, NumberStyles.Any), StringParsers.ParseFloat(array[2], 0, -1, NumberStyles.Any));
		this.index = (_element.HasAttribute("index") ? int.Parse(_element.GetAttribute("index")) : 0);
	}

	// Token: 0x04006FA8 RID: 28584
	public static UVRectTiling Empty;

	// Token: 0x04006FA9 RID: 28585
	public Rect uv;

	// Token: 0x04006FAA RID: 28586
	public int blockW;

	// Token: 0x04006FAB RID: 28587
	public int blockH;

	// Token: 0x04006FAC RID: 28588
	public bool bSwitchUV;

	// Token: 0x04006FAD RID: 28589
	public bool bGlobalUV;

	// Token: 0x04006FAE RID: 28590
	public Color color;

	// Token: 0x04006FAF RID: 28591
	public int index;

	// Token: 0x04006FB0 RID: 28592
	public MaterialBlock material;

	// Token: 0x04006FB1 RID: 28593
	public string textureName;
}
