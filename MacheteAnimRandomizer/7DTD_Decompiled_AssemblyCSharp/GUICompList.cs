using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000FB9 RID: 4025
public class GUICompList : GUIComp
{
	// Token: 0x06008027 RID: 32807 RVA: 0x003414F8 File Offset: 0x0033F6F8
	public GUICompList(Rect _rect)
	{
		this.rect = _rect;
		this.boxStyle = "box";
		this.listContent = new List<GUIContent>();
	}

	// Token: 0x06008028 RID: 32808 RVA: 0x0034152C File Offset: 0x0033F72C
	public GUICompList(Rect _rect, string[] _listContent) : this(_rect)
	{
		foreach (string text in _listContent)
		{
			this.listContent.Add(new GUIContent(text));
		}
	}

	// Token: 0x06008029 RID: 32809 RVA: 0x00341568 File Offset: 0x0033F768
	[PublicizedFrom(EAccessModifier.Private)]
	public void initStyle()
	{
		this.listStyle = new GUIStyle("button");
		this.listStyle.fontSize = 12;
		this.listStyle.normal.textColor = Color.white;
		this.listStyle.alignment = TextAnchor.MiddleLeft;
		this.listStyle.fixedHeight = (float)(this.listStyle.fontSize + 9);
		this.listStyle.fontStyle = FontStyle.Normal;
		this.listStyle.normal.background = null;
		this.listStyle.padding.left = 2;
		this.listStyle.padding.right = 2;
		this.listStyle.padding.top = 0;
		this.listStyle.padding.bottom = 0;
		this.listStyle.margin = new RectOffset(0, 0, 0, 0);
		this.listStyle.hover.textColor = Color.yellow;
		this.bInitStyleDone = true;
	}

	// Token: 0x0600802A RID: 32810 RVA: 0x00341663 File Offset: 0x0033F863
	public void AddLine(string _line)
	{
		this.listContent.Add(new GUIContent(_line));
	}

	// Token: 0x0600802B RID: 32811 RVA: 0x00341676 File Offset: 0x0033F876
	public void RemoveSelectedEntry()
	{
		if (this.listContent.Count > 0 && this.selectedItemIndex != -1)
		{
			this.listContent.RemoveAt(this.selectedItemIndex);
		}
	}

	// Token: 0x0600802C RID: 32812 RVA: 0x003416A0 File Offset: 0x0033F8A0
	public void MoveSelectedEntryUp()
	{
		if (this.listContent.Count > 0 && this.selectedItemIndex > 0 && this.selectedItemIndex < this.listContent.Count)
		{
			GUIContent item = this.listContent[this.selectedItemIndex];
			this.RemoveSelectedEntry();
			this.selectedItemIndex--;
			this.listContent.Insert(this.selectedItemIndex, item);
		}
	}

	// Token: 0x0600802D RID: 32813 RVA: 0x00341710 File Offset: 0x0033F910
	public void MoveSelectedEntryDown()
	{
		if (this.listContent.Count > 0 && this.selectedItemIndex < this.listContent.Count - 1)
		{
			GUIContent item = this.listContent[this.selectedItemIndex];
			this.RemoveSelectedEntry();
			this.selectedItemIndex++;
			this.listContent.Insert(this.selectedItemIndex, item);
		}
	}

	// Token: 0x0600802E RID: 32814 RVA: 0x00341778 File Offset: 0x0033F978
	public void Clear()
	{
		this.listContent.Clear();
	}

	// Token: 0x0600802F RID: 32815 RVA: 0x00341788 File Offset: 0x0033F988
	public override void OnGUI()
	{
		if (!this.bInitStyleDone)
		{
			this.initStyle();
		}
		if (this.bScrollToSelection)
		{
			this.scroll = new Vector2(0f, this.listStyle.fixedHeight * (float)this.selectedItemIndex);
			this.bScrollToSelection = false;
		}
		Rect rect = new Rect(this.rect.x, this.rect.y, this.rect.width - 18f, this.listStyle.fixedHeight * (float)this.listContent.Count);
		GUI.Box(this.rect, "", this.boxStyle);
		this.scroll = GUI.BeginScrollView(this.rect, this.scroll, rect);
		this.selectedItemIndex = GUI.SelectionGrid(rect, this.selectedItemIndex, this.listContent.ToArray(), 1, this.listStyle);
		GUI.EndScrollView();
	}

	// Token: 0x06008030 RID: 32816 RVA: 0x00341878 File Offset: 0x0033FA78
	public override void OnGUILayout()
	{
		if (!this.bInitStyleDone)
		{
			this.initStyle();
		}
		if (this.bScrollToSelection)
		{
			this.scroll = new Vector2(0f, this.listStyle.fixedHeight * (float)this.selectedItemIndex);
			this.bScrollToSelection = false;
		}
		GUILayout.BeginVertical("box", new GUILayoutOption[]
		{
			GUILayout.Width(this.rect.width)
		});
		this.scroll = GUILayout.BeginScrollView(this.scroll, false, true, new GUILayoutOption[]
		{
			GUILayout.Width(this.rect.width),
			GUILayout.Height(this.rect.height)
		});
		this.lastSelectedItemIndex = this.selectedItemIndex;
		this.selectedItemIndex = GUILayout.SelectionGrid(this.lastSelectedItemIndex, this.listContent.ToArray(), 1, this.listStyle, new GUILayoutOption[]
		{
			GUILayout.Width(this.rect.width - 18f)
		});
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
	}

	// Token: 0x06008031 RID: 32817 RVA: 0x00341984 File Offset: 0x0033FB84
	public bool OnListClicked()
	{
		return this.lastSelectedItemIndex != this.selectedItemIndex;
	}

	// Token: 0x17000D59 RID: 3417
	// (get) Token: 0x06008032 RID: 32818 RVA: 0x00341997 File Offset: 0x0033FB97
	// (set) Token: 0x06008033 RID: 32819 RVA: 0x0034199F File Offset: 0x0033FB9F
	public int SelectedItemIndex
	{
		get
		{
			return this.selectedItemIndex;
		}
		set
		{
			this.selectedItemIndex = value;
			this.lastSelectedItemIndex = value;
			this.bScrollToSelection = true;
		}
	}

	// Token: 0x17000D5A RID: 3418
	// (get) Token: 0x06008034 RID: 32820 RVA: 0x003419B6 File Offset: 0x0033FBB6
	public string SelectedEntry
	{
		get
		{
			if (this.selectedItemIndex < 0 || this.selectedItemIndex >= this.listContent.Count)
			{
				return null;
			}
			return this.listContent[this.selectedItemIndex].text;
		}
	}

	// Token: 0x06008035 RID: 32821 RVA: 0x003419EC File Offset: 0x0033FBEC
	public bool SelectEntry(string _entry)
	{
		for (int i = 0; i < this.listContent.Count; i++)
		{
			if (this.listContent[i].text.Equals(_entry))
			{
				this.SelectedItemIndex = i;
				return true;
			}
		}
		return false;
	}

	// Token: 0x04006312 RID: 25362
	[PublicizedFrom(EAccessModifier.Private)]
	public int selectedItemIndex = -1;

	// Token: 0x04006313 RID: 25363
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastSelectedItemIndex = -1;

	// Token: 0x04006314 RID: 25364
	[PublicizedFrom(EAccessModifier.Private)]
	public List<GUIContent> listContent;

	// Token: 0x04006315 RID: 25365
	[PublicizedFrom(EAccessModifier.Private)]
	public string boxStyle;

	// Token: 0x04006316 RID: 25366
	[PublicizedFrom(EAccessModifier.Private)]
	public GUIStyle listStyle;

	// Token: 0x04006317 RID: 25367
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2 scroll;

	// Token: 0x04006318 RID: 25368
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bScrollToSelection;

	// Token: 0x04006319 RID: 25369
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bInitStyleDone;
}
