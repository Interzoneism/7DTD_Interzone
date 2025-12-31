using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000C1C RID: 3100
[Preserve]
public class XUiC_CamPositionsList : XUiC_List<XUiC_CamPositionsList.CamPerspectiveEntry>
{
	// Token: 0x170009D1 RID: 2513
	// (get) Token: 0x06005F32 RID: 24370 RVA: 0x0026A169 File Offset: 0x00268369
	// (set) Token: 0x06005F33 RID: 24371 RVA: 0x0026A171 File Offset: 0x00268371
	public bool ShowAddCamPositionWindow
	{
		get
		{
			return this.showAddCamPositionWindow;
		}
		set
		{
			if (value == this.showAddCamPositionWindow)
			{
				return;
			}
			this.showAddCamPositionWindow = value;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06005F34 RID: 24372 RVA: 0x0026A18C File Offset: 0x0026838C
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("btnAddCamPosition");
		XUiV_Button xuiV_Button = ((childById != null) ? childById.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button != null)
		{
			xuiV_Button.Controller.OnPress += delegate(XUiController _, int _)
			{
				this.ShowAddCamPositionWindow = !this.ShowAddCamPositionWindow;
			};
		}
	}

	// Token: 0x06005F35 RID: 24373 RVA: 0x0026A1D8 File Offset: 0x002683D8
	public void UpdateList()
	{
		int page = base.Page;
		this.RebuildList(false);
		base.Page = page;
	}

	// Token: 0x06005F36 RID: 24374 RVA: 0x0026A1FC File Offset: 0x002683FC
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		this.perspectives.Load();
		foreach (KeyValuePair<string, CameraPerspectives.Perspective> keyValuePair in this.perspectives.Perspectives)
		{
			string text;
			CameraPerspectives.Perspective perspective;
			keyValuePair.Deconstruct(out text, out perspective);
			CameraPerspectives.Perspective perspective2 = perspective;
			this.allEntries.Add(new XUiC_CamPositionsList.CamPerspectiveEntry(perspective2));
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x06005F37 RID: 24375 RVA: 0x0026A298 File Offset: 0x00268498
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "cam_position_add_open")
		{
			_value = this.ShowAddCamPositionWindow.ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x06005F38 RID: 24376 RVA: 0x0026A2CC File Offset: 0x002684CC
	public override void OnVisibilityChanged(bool _isVisible)
	{
		base.OnVisibilityChanged(_isVisible);
		if (_isVisible)
		{
			this.RebuildList(false);
		}
	}

	// Token: 0x06005F39 RID: 24377 RVA: 0x0026A2E0 File Offset: 0x002684E0
	public void Add(string _name, string _comment)
	{
		CameraPerspectives.Perspective perspective = new CameraPerspectives.Perspective(_name, base.xui.playerUI.entityPlayer, _comment);
		this.perspectives.Perspectives.Add(perspective.Name, perspective);
		this.perspectives.Save();
		this.UpdateList();
	}

	// Token: 0x040047D8 RID: 18392
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CameraPerspectives perspectives = new CameraPerspectives(false);

	// Token: 0x040047D9 RID: 18393
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showAddCamPositionWindow;

	// Token: 0x02000C1D RID: 3101
	[Preserve]
	public class CamPerspectiveEntry : XUiListEntry<XUiC_CamPositionsList.CamPerspectiveEntry>
	{
		// Token: 0x06005F3C RID: 24380 RVA: 0x0026A352 File Offset: 0x00268552
		public CamPerspectiveEntry(CameraPerspectives.Perspective _perspective)
		{
			this.Perspective = _perspective;
		}

		// Token: 0x06005F3D RID: 24381 RVA: 0x0026A361 File Offset: 0x00268561
		public override int CompareTo(XUiC_CamPositionsList.CamPerspectiveEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return -1;
			}
			return string.Compare(this.Perspective.Name, _otherEntry.Perspective.Name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06005F3E RID: 24382 RVA: 0x0026A384 File Offset: 0x00268584
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = this.Perspective.Name;
				return true;
			}
			if (_bindingName == "position")
			{
				_value = ValueDisplayFormatters.WorldPos(this.Perspective.Position, " ", false);
				return true;
			}
			if (_bindingName == "direction")
			{
				_value = this.Perspective.Direction.ToCultureInvariantString();
				return true;
			}
			if (!(_bindingName == "comment"))
			{
				return false;
			}
			_value = (this.Perspective.Comment ?? "");
			return true;
		}

		// Token: 0x06005F3F RID: 24383 RVA: 0x0026A41D File Offset: 0x0026861D
		public override bool MatchesSearch(string _searchString)
		{
			if (string.IsNullOrEmpty(_searchString))
			{
				return true;
			}
			if (!this.Perspective.Name.ContainsCaseInsensitive(_searchString))
			{
				string comment = this.Perspective.Comment;
				return comment != null && comment.ContainsCaseInsensitive(_searchString);
			}
			return true;
		}

		// Token: 0x06005F40 RID: 24384 RVA: 0x0026A458 File Offset: 0x00268658
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = string.Empty;
				return true;
			}
			if (_bindingName == "position")
			{
				_value = string.Empty;
				return true;
			}
			if (_bindingName == "direction")
			{
				_value = string.Empty;
				return true;
			}
			if (!(_bindingName == "comment"))
			{
				return false;
			}
			_value = string.Empty;
			return true;
		}

		// Token: 0x040047DA RID: 18394
		public readonly CameraPerspectives.Perspective Perspective;
	}

	// Token: 0x02000C1E RID: 3102
	[Preserve]
	public class CamPositionsListEntryController : XUiC_ListEntry<XUiC_CamPositionsList.CamPerspectiveEntry>
	{
		// Token: 0x06005F41 RID: 24385 RVA: 0x0026A4C0 File Offset: 0x002686C0
		public override void Init()
		{
			base.Init();
			this.parentList = base.GetParentByType<XUiC_CamPositionsList>();
			XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("camButton") as XUiC_SimpleButton;
			if (xuiC_SimpleButton != null)
			{
				xuiC_SimpleButton.OnPressed += delegate(XUiController _, int _)
				{
					base.GetEntry().Perspective.ToPlayer(base.xui.playerUI.entityPlayer);
				};
			}
			XUiController childById = base.GetChildById("btnDelete");
			XUiV_Button xuiV_Button = ((childById != null) ? childById.ViewComponent : null) as XUiV_Button;
			if (xuiV_Button != null)
			{
				xuiV_Button.Controller.OnPress += delegate(XUiController _, int _)
				{
					XUiC_CamPositionsList.CamPerspectiveEntry entry = base.GetEntry();
					this.parentList.perspectives.Perspectives.Remove(entry.Perspective.Name);
					this.parentList.perspectives.Save();
					this.parentList.UpdateList();
				};
			}
		}

		// Token: 0x040047DB RID: 18395
		[PublicizedFrom(EAccessModifier.Private)]
		public XUiC_CamPositionsList parentList;
	}
}
