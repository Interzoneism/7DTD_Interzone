using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F06 RID: 3846
public class XUiV_Grid : XUiView
{
	// Token: 0x17000C72 RID: 3186
	// (get) Token: 0x060079CF RID: 31183 RVA: 0x00317F26 File Offset: 0x00316126
	// (set) Token: 0x060079D0 RID: 31184 RVA: 0x00317F2E File Offset: 0x0031612E
	public UIGrid Grid { get; set; }

	// Token: 0x17000C73 RID: 3187
	// (get) Token: 0x060079D1 RID: 31185 RVA: 0x00317F37 File Offset: 0x00316137
	// (set) Token: 0x060079D2 RID: 31186 RVA: 0x00317F3F File Offset: 0x0031613F
	public UIGrid.Arrangement Arrangement { get; set; }

	// Token: 0x140000DF RID: 223
	// (add) Token: 0x060079D3 RID: 31187 RVA: 0x00317F48 File Offset: 0x00316148
	// (remove) Token: 0x060079D4 RID: 31188 RVA: 0x00317F80 File Offset: 0x00316180
	public event UIGrid.OnSizeChanged OnSizeChanged;

	// Token: 0x140000E0 RID: 224
	// (add) Token: 0x060079D5 RID: 31189 RVA: 0x00317FB8 File Offset: 0x003161B8
	// (remove) Token: 0x060079D6 RID: 31190 RVA: 0x00317FF0 File Offset: 0x003161F0
	public event Action OnSizeChangedSimple;

	// Token: 0x17000C74 RID: 3188
	// (get) Token: 0x060079D7 RID: 31191 RVA: 0x00318025 File Offset: 0x00316225
	// (set) Token: 0x060079D8 RID: 31192 RVA: 0x00318030 File Offset: 0x00316230
	public int Columns
	{
		get
		{
			return this.columns;
		}
		set
		{
			if (this.initialized && this.Arrangement == UIGrid.Arrangement.Horizontal && this.Grid.maxPerLine != value)
			{
				this.Grid.maxPerLine = value;
				this.Grid.Reposition();
			}
			this.columns = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C75 RID: 3189
	// (get) Token: 0x060079D9 RID: 31193 RVA: 0x00318080 File Offset: 0x00316280
	// (set) Token: 0x060079DA RID: 31194 RVA: 0x00318088 File Offset: 0x00316288
	public int Rows
	{
		get
		{
			return this.rows;
		}
		set
		{
			if (this.initialized && this.Arrangement != UIGrid.Arrangement.Horizontal && this.Grid.maxPerLine != value)
			{
				this.Grid.maxPerLine = value;
				this.Grid.Reposition();
			}
			this.rows = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000C76 RID: 3190
	// (get) Token: 0x060079DB RID: 31195 RVA: 0x003180D8 File Offset: 0x003162D8
	// (set) Token: 0x060079DC RID: 31196 RVA: 0x00002914 File Offset: 0x00000B14
	public override int RepeatCount
	{
		get
		{
			return this.Columns * this.Rows;
		}
		set
		{
		}
	}

	// Token: 0x17000C77 RID: 3191
	// (get) Token: 0x060079DD RID: 31197 RVA: 0x003180E7 File Offset: 0x003162E7
	// (set) Token: 0x060079DE RID: 31198 RVA: 0x003180EF File Offset: 0x003162EF
	public int CellWidth { get; set; }

	// Token: 0x17000C78 RID: 3192
	// (get) Token: 0x060079DF RID: 31199 RVA: 0x003180F8 File Offset: 0x003162F8
	// (set) Token: 0x060079E0 RID: 31200 RVA: 0x00318100 File Offset: 0x00316300
	public int CellHeight { get; set; }

	// Token: 0x17000C79 RID: 3193
	// (get) Token: 0x060079E1 RID: 31201 RVA: 0x00318109 File Offset: 0x00316309
	// (set) Token: 0x060079E2 RID: 31202 RVA: 0x00318111 File Offset: 0x00316311
	public bool HideInactive { get; set; }

	// Token: 0x060079E3 RID: 31203 RVA: 0x0031811A File Offset: 0x0031631A
	public XUiV_Grid(string _id) : base(_id)
	{
	}

	// Token: 0x060079E4 RID: 31204 RVA: 0x00318123 File Offset: 0x00316323
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateComponents(GameObject _go)
	{
		_go.AddComponent<UIWidget>();
		_go.AddComponent<UIGrid>();
	}

	// Token: 0x060079E5 RID: 31205 RVA: 0x00318134 File Offset: 0x00316334
	public override void InitView()
	{
		base.InitView();
		this.widget = this.uiTransform.gameObject.GetComponent<UIWidget>();
		this.widget.pivot = this.pivot;
		this.widget.depth = base.Depth + 2;
		this.widget.autoResizeBoxCollider = true;
		this.Grid = this.uiTransform.gameObject.GetComponent<UIGrid>();
		this.Grid.hideInactive = this.HideInactive;
		this.Grid.arrangement = this.Arrangement;
		this.Grid.pivot = this.pivot;
		this.Grid.onSizeChanged = new UIGrid.OnSizeChanged(this.OnGridSizeChanged);
		if (this.Arrangement == UIGrid.Arrangement.Horizontal)
		{
			this.Grid.maxPerLine = this.Columns;
		}
		else
		{
			this.Grid.maxPerLine = this.Rows;
		}
		this.Grid.cellWidth = (float)this.CellWidth;
		this.Grid.cellHeight = (float)this.CellHeight;
		this.uiTransform.localScale = Vector3.one;
		this.uiTransform.localPosition = new Vector3((float)base.PaddedPosition.x, (float)base.PaddedPosition.y, 0f);
		this.initialized = true;
	}

	// Token: 0x060079E6 RID: 31206 RVA: 0x00318284 File Offset: 0x00316484
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGridSizeChanged(Vector2Int _cells, Vector2 _size)
	{
		this.widget.width = Mathf.RoundToInt(_size.x);
		this.widget.height = Mathf.RoundToInt(_size.y);
		UIGrid.OnSizeChanged onSizeChanged = this.OnSizeChanged;
		if (onSizeChanged != null)
		{
			onSizeChanged(_cells, _size);
		}
		Action onSizeChangedSimple = this.OnSizeChangedSimple;
		if (onSizeChangedSimple == null)
		{
			return;
		}
		onSizeChangedSimple();
	}

	// Token: 0x060079E7 RID: 31207 RVA: 0x003182E0 File Offset: 0x003164E0
	public override void Update(float _dt)
	{
		if (this.isDirty)
		{
			if (this.Arrangement == UIGrid.Arrangement.Horizontal)
			{
				this.Grid.maxPerLine = this.Columns;
			}
			else
			{
				this.Grid.maxPerLine = this.Rows;
			}
		}
		this.Grid.cellWidth = (float)this.CellWidth;
		this.Grid.cellHeight = (float)this.CellHeight;
		this.Grid.repositionNow = true;
		base.Update(_dt);
	}

	// Token: 0x060079E8 RID: 31208 RVA: 0x00318358 File Offset: 0x00316558
	public override void SetDefaults(XUiController _parent)
	{
		base.SetDefaults(_parent);
		this.Columns = 0;
		this.Rows = 0;
		this.CellWidth = 0;
		this.CellHeight = 0;
		this.Arrangement = UIGrid.Arrangement.Horizontal;
		this.HideInactive = true;
	}

	// Token: 0x060079E9 RID: 31209 RVA: 0x0031838C File Offset: 0x0031658C
	public override bool ParseAttribute(string attribute, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(attribute, value, _parent);
		if (!flag)
		{
			if (!(attribute == "cols"))
			{
				if (!(attribute == "rows"))
				{
					if (!(attribute == "cell_width"))
					{
						if (!(attribute == "cell_height"))
						{
							if (!(attribute == "arrangement"))
							{
								if (!(attribute == "hide_inactive"))
								{
									return false;
								}
								this.HideInactive = StringParsers.ParseBool(value, 0, -1, true);
							}
							else
							{
								this.Arrangement = EnumUtils.Parse<UIGrid.Arrangement>(value, true);
							}
						}
						else
						{
							this.CellHeight = int.Parse(value);
						}
					}
					else
					{
						this.CellWidth = int.Parse(value);
					}
				}
				else
				{
					this.Rows = int.Parse(value);
				}
			}
			else
			{
				this.Columns = int.Parse(value);
			}
			return true;
		}
		return flag;
	}

	// Token: 0x060079EA RID: 31210 RVA: 0x00318458 File Offset: 0x00316658
	public override void setRepeatContentTemplateParams(Dictionary<string, object> _templateParams, int _curRepeatNum)
	{
		base.setRepeatContentTemplateParams(_templateParams, _curRepeatNum);
		int num;
		int num2;
		if (this.Arrangement == UIGrid.Arrangement.Horizontal)
		{
			num = _curRepeatNum % this.Columns;
			num2 = _curRepeatNum / this.Columns;
		}
		else
		{
			num = _curRepeatNum / this.Rows;
			num2 = _curRepeatNum % this.Rows;
		}
		_templateParams["repeat_col"] = num;
		_templateParams["repeat_row"] = num2;
	}

	// Token: 0x04005C66 RID: 23654
	[PublicizedFrom(EAccessModifier.Private)]
	public int columns;

	// Token: 0x04005C67 RID: 23655
	[PublicizedFrom(EAccessModifier.Private)]
	public int rows;

	// Token: 0x04005C6A RID: 23658
	[PublicizedFrom(EAccessModifier.Private)]
	public UIWidget widget;
}
