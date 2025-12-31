using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000965 RID: 2405
public class SelectionCategory
{
	// Token: 0x170007A6 RID: 1958
	// (get) Token: 0x060048B3 RID: 18611 RVA: 0x001CC49D File Offset: 0x001CA69D
	// (set) Token: 0x060048B4 RID: 18612 RVA: 0x001CC4A5 File Offset: 0x001CA6A5
	public ISelectionBoxCallback callback { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060048B5 RID: 18613 RVA: 0x001CC4B0 File Offset: 0x001CA6B0
	public SelectionCategory(string _name, Transform _transform, Color _colActive, Color _colInactive, Color _colFaceSelected, bool _bCollider, string _tag, ISelectionBoxCallback _callback, int _layer = 0)
	{
		this.name = _name;
		this.transform = _transform;
		this.colActive = _colActive;
		this.colInactive = _colInactive;
		this.colFaceSelected = _colFaceSelected;
		this.bCollider = _bCollider;
		this.tag = _tag;
		this.callback = _callback;
		this.layer = _layer;
	}

	// Token: 0x060048B6 RID: 18614 RVA: 0x001CC513 File Offset: 0x001CA713
	public void SetCallback(ISelectionBoxCallback _callback)
	{
		this.callback = _callback;
	}

	// Token: 0x060048B7 RID: 18615 RVA: 0x001CC51C File Offset: 0x001CA71C
	public bool IsVisible()
	{
		return this.transform.gameObject.activeSelf;
	}

	// Token: 0x060048B8 RID: 18616 RVA: 0x001CC530 File Offset: 0x001CA730
	public void SetVisible(bool _bVisible)
	{
		this.transform.gameObject.SetActive(_bVisible);
		string a = this.name;
		if (!(a == "SleeperVolume"))
		{
			if (a == "POIMarker")
			{
				POIMarkerToolManager.UpdateAllColors();
				bool bShow;
				if (_bVisible)
				{
					ValueTuple<SelectionCategory, SelectionBox>? selection = SelectionBoxManager.Instance.Selection;
					bShow = (((selection != null) ? selection.GetValueOrDefault().Item1 : null) != null);
				}
				else
				{
					bShow = false;
				}
				POIMarkerToolManager.ShowPOIMarkers(bShow);
			}
		}
		else
		{
			SleeperVolumeToolManager.SetVisible(_bVisible);
		}
		if (!_bVisible)
		{
			ValueTuple<SelectionCategory, SelectionBox>? selection = SelectionBoxManager.Instance.Selection;
			if (((selection != null) ? selection.GetValueOrDefault().Item1 : null) == this)
			{
				SelectionBoxManager.Instance.Deactivate();
			}
		}
	}

	// Token: 0x060048B9 RID: 18617 RVA: 0x001CC5E4 File Offset: 0x001CA7E4
	public void SetCaptionVisibility(bool _visible)
	{
		foreach (KeyValuePair<string, SelectionBox> keyValuePair in this.boxes)
		{
			keyValuePair.Value.SetCaptionVisibility(_visible);
		}
	}

	// Token: 0x060048BA RID: 18618 RVA: 0x001CC640 File Offset: 0x001CA840
	public void Clear()
	{
		foreach (KeyValuePair<string, SelectionBox> keyValuePair in this.boxes)
		{
			UnityEngine.Object.Destroy(keyValuePair.Value.gameObject);
		}
		this.boxes.Clear();
		if (this.name == "SleeperVolume")
		{
			SleeperVolumeToolManager.ClearSleeperVolumes();
		}
	}

	// Token: 0x060048BB RID: 18619 RVA: 0x001CC6C0 File Offset: 0x001CA8C0
	public SelectionBox AddBox(string _name, Vector3 _pos, Vector3i _size, bool _bDrawDirection = false, bool _bAlwaysDrawDirection = false)
	{
		SelectionBox selectionBox;
		if (this.boxes.TryGetValue(_name, out selectionBox))
		{
			this.RemoveBox(_name);
		}
		Transform transform = new GameObject(_name).transform;
		transform.parent = this.transform;
		SelectionBox selectionBox2 = transform.gameObject.AddComponent<SelectionBox>();
		selectionBox2.SetOwner(this);
		selectionBox2.SetAllFacesColor(this.colInactive, true);
		selectionBox2.bDrawDirection = _bDrawDirection;
		selectionBox2.bAlwaysDrawDirection = _bAlwaysDrawDirection;
		selectionBox2.SetPositionAndSize(_pos, _size);
		if (this.bCollider)
		{
			selectionBox2.EnableCollider(this.tag, this.layer);
		}
		this.boxes[_name] = selectionBox2;
		if (this.name == "SleeperVolume")
		{
			SleeperVolumeToolManager.RegisterSleeperVolume(selectionBox2);
		}
		return selectionBox2;
	}

	// Token: 0x060048BC RID: 18620 RVA: 0x001CC774 File Offset: 0x001CA974
	public SelectionBox GetBox(string _name)
	{
		SelectionBox result;
		this.boxes.TryGetValue(_name, out result);
		return result;
	}

	// Token: 0x060048BD RID: 18621 RVA: 0x001CC794 File Offset: 0x001CA994
	public void RenameBox(string _name, string _newName)
	{
		if (_name.Equals(_newName))
		{
			return;
		}
		SelectionBox selectionBox;
		if (!this.boxes.TryGetValue(_name, out selectionBox))
		{
			return;
		}
		selectionBox.name = _newName;
		this.boxes[_newName] = selectionBox;
		this.boxes.Remove(_name);
	}

	// Token: 0x060048BE RID: 18622 RVA: 0x001CC7E0 File Offset: 0x001CA9E0
	public void RemoveBox(string _name)
	{
		SelectionBox selectionBox;
		if (!this.boxes.TryGetValue(_name, out selectionBox))
		{
			return;
		}
		ValueTuple<SelectionCategory, SelectionBox>? valueTuple;
		if (((SelectionBoxManager.Instance.Selection != null) ? valueTuple.GetValueOrDefault().Item2 : null) == selectionBox)
		{
			SelectionBoxManager.Instance.Deactivate();
		}
		if (this.name == "SleeperVolume")
		{
			SleeperVolumeToolManager.UnRegisterSleeperVolume(selectionBox);
		}
		UnityEngine.Object.Destroy(selectionBox.gameObject);
		this.boxes.Remove(_name);
	}

	// Token: 0x040037EA RID: 14314
	public readonly string name;

	// Token: 0x040037EB RID: 14315
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Transform transform;

	// Token: 0x040037EC RID: 14316
	public readonly Color colActive;

	// Token: 0x040037ED RID: 14317
	public readonly Color colInactive;

	// Token: 0x040037EE RID: 14318
	public readonly Color colFaceSelected;

	// Token: 0x040037EF RID: 14319
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly bool bCollider;

	// Token: 0x040037F0 RID: 14320
	public readonly string tag;

	// Token: 0x040037F2 RID: 14322
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int layer;

	// Token: 0x040037F3 RID: 14323
	public readonly Dictionary<string, SelectionBox> boxes = new Dictionary<string, SelectionBox>();
}
