using System;
using UnityEngine;

// Token: 0x02000F0B RID: 3851
public class XUiV_Table : XUiView
{
	// Token: 0x17000CA5 RID: 3237
	// (get) Token: 0x06007A68 RID: 31336 RVA: 0x0031AD0F File Offset: 0x00318F0F
	// (set) Token: 0x06007A69 RID: 31337 RVA: 0x0031AD17 File Offset: 0x00318F17
	public UITable Table { get; set; }

	// Token: 0x17000CA6 RID: 3238
	// (get) Token: 0x06007A6A RID: 31338 RVA: 0x0031AD20 File Offset: 0x00318F20
	// (set) Token: 0x06007A6B RID: 31339 RVA: 0x0031AD28 File Offset: 0x00318F28
	public UITable.Sorting Sorting { get; set; }

	// Token: 0x17000CA7 RID: 3239
	// (get) Token: 0x06007A6C RID: 31340 RVA: 0x0031AD31 File Offset: 0x00318F31
	// (set) Token: 0x06007A6D RID: 31341 RVA: 0x0031AD39 File Offset: 0x00318F39
	public int Columns { get; set; } = 1;

	// Token: 0x17000CA8 RID: 3240
	// (get) Token: 0x06007A6E RID: 31342 RVA: 0x0031AD42 File Offset: 0x00318F42
	// (set) Token: 0x06007A6F RID: 31343 RVA: 0x0031AD4A File Offset: 0x00318F4A
	public Vector2 Padding { get; set; }

	// Token: 0x17000CA9 RID: 3241
	// (get) Token: 0x06007A70 RID: 31344 RVA: 0x0031AD53 File Offset: 0x00318F53
	// (set) Token: 0x06007A71 RID: 31345 RVA: 0x0031AD5B File Offset: 0x00318F5B
	public bool HideInactive { get; set; } = true;

	// Token: 0x17000CAA RID: 3242
	// (get) Token: 0x06007A72 RID: 31346 RVA: 0x0031AD64 File Offset: 0x00318F64
	// (set) Token: 0x06007A73 RID: 31347 RVA: 0x0031AD6C File Offset: 0x00318F6C
	public bool AlwaysReposition { get; set; }

	// Token: 0x17000CAB RID: 3243
	// (get) Token: 0x06007A74 RID: 31348 RVA: 0x0031AD75 File Offset: 0x00318F75
	// (set) Token: 0x06007A75 RID: 31349 RVA: 0x0031AD7D File Offset: 0x00318F7D
	public bool RepositionTwice { get; set; }

	// Token: 0x06007A76 RID: 31350 RVA: 0x0031AD86 File Offset: 0x00318F86
	public XUiV_Table(string _id) : base(_id)
	{
	}

	// Token: 0x06007A77 RID: 31351 RVA: 0x0031ADA4 File Offset: 0x00318FA4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateComponents(GameObject _go)
	{
		_go.AddComponent<UITable>();
	}

	// Token: 0x06007A78 RID: 31352 RVA: 0x0031ADB0 File Offset: 0x00318FB0
	public override void InitView()
	{
		base.InitView();
		this.Table = this.uiTransform.gameObject.GetComponent<UITable>();
		this.Table.hideInactive = this.HideInactive;
		this.Table.sorting = this.Sorting;
		this.Table.direction = UITable.Direction.Down;
		this.Table.columns = this.Columns;
		this.Table.padding = this.Padding;
		this.uiTransform.localScale = Vector3.one;
		this.uiTransform.localPosition = new Vector3((float)base.PaddedPosition.x, (float)base.PaddedPosition.y, 0f);
		this.Table.Reposition();
	}

	// Token: 0x06007A79 RID: 31353 RVA: 0x0031AE71 File Offset: 0x00319071
	public override void SetDefaults(XUiController _parent)
	{
		base.SetDefaults(_parent);
		this.IsVisible = false;
	}

	// Token: 0x06007A7A RID: 31354 RVA: 0x0031AE81 File Offset: 0x00319081
	public override void OnOpen()
	{
		base.OnOpen();
		this.Table.repositionNow = true;
		if (this.RepositionTwice)
		{
			this.repositionNextFrame = true;
		}
	}

	// Token: 0x06007A7B RID: 31355 RVA: 0x0031AEA4 File Offset: 0x003190A4
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.firstUpdate || this.AlwaysReposition)
		{
			this.firstUpdate = false;
			this.Table.Reposition();
		}
		if (this.repositionNextFrame && !this.Table.enabled)
		{
			this.Table.repositionNow = true;
			this.repositionNextFrame = false;
		}
	}

	// Token: 0x06007A7C RID: 31356 RVA: 0x0031AF04 File Offset: 0x00319104
	public override bool ParseAttribute(string attribute, string value, XUiController _parent)
	{
		if (!(attribute == "columns"))
		{
			if (!(attribute == "padding"))
			{
				if (!(attribute == "sorting"))
				{
					if (!(attribute == "hide_inactive"))
					{
						if (!(attribute == "always_reposition"))
						{
							if (!(attribute == "reposition_twice"))
							{
								return base.ParseAttribute(attribute, value, _parent);
							}
							this.RepositionTwice = StringParsers.ParseBool(value, 0, -1, true);
						}
						else
						{
							this.AlwaysReposition = StringParsers.ParseBool(value, 0, -1, true);
						}
					}
					else
					{
						this.HideInactive = StringParsers.ParseBool(value, 0, -1, true);
					}
				}
				else
				{
					this.Sorting = EnumUtils.Parse<UITable.Sorting>(value, true);
				}
			}
			else
			{
				this.Padding = StringParsers.ParseVector2(value);
			}
		}
		else
		{
			this.Columns = int.Parse(value);
		}
		return true;
	}

	// Token: 0x04005CBA RID: 23738
	[PublicizedFrom(EAccessModifier.Private)]
	public bool firstUpdate = true;

	// Token: 0x04005CBB RID: 23739
	[PublicizedFrom(EAccessModifier.Private)]
	public bool repositionNextFrame;
}
