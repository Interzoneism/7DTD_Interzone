using System;
using Audio;
using UnityEngine;

// Token: 0x02000FBB RID: 4027
public class GUIWindow
{
	// Token: 0x17000D5B RID: 3419
	// (get) Token: 0x06008038 RID: 32824 RVA: 0x00341A62 File Offset: 0x0033FC62
	// (set) Token: 0x06008039 RID: 32825 RVA: 0x00341A6A File Offset: 0x0033FC6A
	public Rect windowRect
	{
		get
		{
			return this.internalWindowRect;
		}
		set
		{
			this.internalWindowRect = value;
			this.matrix.SetTRS(new Vector3(this.internalWindowRect.x, this.internalWindowRect.y, 0f), Quaternion.identity, Vector3.one);
		}
	}

	// Token: 0x0600803A RID: 32826 RVA: 0x00341AA8 File Offset: 0x0033FCA8
	public GUIWindow(string _id, int _w, int _h, bool _bDrawBackground) : this(_id, _w, _h, _bDrawBackground, true)
	{
	}

	// Token: 0x0600803B RID: 32827 RVA: 0x00341AB6 File Offset: 0x0033FCB6
	public GUIWindow(string _id, int _w, int _h, bool _bDrawBackground, bool _isDimBackground) : this(_id, new Rect((float)(Screen.width - _w) / 2f, (float)(Screen.height - _h) / 2f, (float)_w, (float)_h), _bDrawBackground, _isDimBackground)
	{
		this.bCenterWindow = true;
	}

	// Token: 0x0600803C RID: 32828 RVA: 0x00341AEF File Offset: 0x0033FCEF
	public GUIWindow(string _id, Rect _rect) : this(_id, _rect, false)
	{
	}

	// Token: 0x0600803D RID: 32829 RVA: 0x00341AFC File Offset: 0x0033FCFC
	public GUIWindow(string _id) : this(_id, default(Rect))
	{
	}

	// Token: 0x0600803E RID: 32830 RVA: 0x00341B19 File Offset: 0x0033FD19
	public GUIWindow(string _id, Rect _rect, bool _bDrawBackground) : this(_id, _rect, _bDrawBackground, true)
	{
	}

	// Token: 0x0600803F RID: 32831 RVA: 0x00341B28 File Offset: 0x0033FD28
	public GUIWindow(string _id, Rect _rect, bool _bDrawBackground, bool _isDimBackground)
	{
		this.windowRect = _rect;
		this.bDrawBackground = _bDrawBackground;
		this.isDimBackground = _isDimBackground;
		this.id = _id;
		this.bActionSetEnabled = false;
	}

	// Token: 0x17000D5C RID: 3420
	// (get) Token: 0x06008040 RID: 32832 RVA: 0x0032277C File Offset: 0x0032097C
	public string Id
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x06008041 RID: 32833 RVA: 0x00341B75 File Offset: 0x0033FD75
	public bool GUIButton(Rect _rect, string _text)
	{
		if (GUI.Button(_rect, _text))
		{
			Manager.PlayButtonClick();
			return true;
		}
		return false;
	}

	// Token: 0x06008042 RID: 32834 RVA: 0x00341B88 File Offset: 0x0033FD88
	public bool GUIButton(Rect _rect, GUIContent _guiContent)
	{
		if (GUI.Button(_rect, _guiContent))
		{
			Manager.PlayButtonClick();
			return true;
		}
		return false;
	}

	// Token: 0x06008043 RID: 32835 RVA: 0x00341B9B File Offset: 0x0033FD9B
	public bool GUIButton(Rect _rect, GUIContent _guiContent, GUIStyle _guiStyle)
	{
		if (GUI.Button(_rect, _guiContent, _guiStyle))
		{
			Manager.PlayButtonClick();
			return true;
		}
		return false;
	}

	// Token: 0x06008044 RID: 32836 RVA: 0x00341BAF File Offset: 0x0033FDAF
	public bool GUILayoutButton(string _text)
	{
		return this.GUILayoutButton(_text, GUILayout.ExpandWidth(false));
	}

	// Token: 0x06008045 RID: 32837 RVA: 0x00341BBE File Offset: 0x0033FDBE
	public bool GUILayoutButton(string _text, GUILayoutOption options)
	{
		if (GUILayout.Button(_text, new GUILayoutOption[]
		{
			options
		}))
		{
			Manager.PlayButtonClick();
			return true;
		}
		return false;
	}

	// Token: 0x06008046 RID: 32838 RVA: 0x00341BDA File Offset: 0x0033FDDA
	public bool GUIToggle(Rect _rect, bool _v, string _s)
	{
		bool flag = GUI.Toggle(_rect, _v, _s);
		if (flag != _v)
		{
			Manager.PlayButtonClick();
		}
		return flag;
	}

	// Token: 0x06008047 RID: 32839 RVA: 0x00341BED File Offset: 0x0033FDED
	public bool GUILayoutToggle(bool _v, string _s)
	{
		return this.GUILayoutToggle(_v, _s, null);
	}

	// Token: 0x06008048 RID: 32840 RVA: 0x00341BF8 File Offset: 0x0033FDF8
	public bool GUILayoutToggle(bool _v, string _s, GUILayoutOption options)
	{
		bool flag = (options != null) ? GUILayout.Toggle(_v, _s, new GUILayoutOption[]
		{
			options
		}) : GUILayout.Toggle(_v, _s, Array.Empty<GUILayoutOption>());
		if (flag != _v)
		{
			Manager.PlayButtonClick();
		}
		return flag;
	}

	// Token: 0x06008049 RID: 32841 RVA: 0x00341C28 File Offset: 0x0033FE28
	public virtual void OnGUI(bool _inputActive)
	{
		if (this.bDrawBackground)
		{
			GUI.Box(new Rect(0f, 0f, this.windowRect.width, this.windowRect.height), "");
		}
		if (this.bCenterWindow)
		{
			this.SetPosition(((float)Screen.width - this.windowRect.width) / 2f, ((float)Screen.height - this.windowRect.height) / 2f);
		}
	}

	// Token: 0x0600804A RID: 32842 RVA: 0x00341CB8 File Offset: 0x0033FEB8
	public void SetPosition(float _x, float _y)
	{
		this.windowRect = new Rect(_x, _y, this.windowRect.width, this.windowRect.height);
	}

	// Token: 0x0600804B RID: 32843 RVA: 0x00341CEE File Offset: 0x0033FEEE
	public void SetSize(float _w, float _h)
	{
		this.windowRect = new Rect(((float)Screen.width - _w) / 2f, ((float)Screen.height - _h) / 2f, _w, _h);
	}

	// Token: 0x0600804C RID: 32844 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Update()
	{
	}

	// Token: 0x0600804D RID: 32845 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnOpen()
	{
	}

	// Token: 0x0600804E RID: 32846 RVA: 0x00341D19 File Offset: 0x0033FF19
	public virtual void OnClose()
	{
		Action onWindowClose = this.OnWindowClose;
		if (onWindowClose != null)
		{
			onWindowClose();
		}
		this.OnWindowClose = null;
	}

	// Token: 0x0600804F RID: 32847 RVA: 0x00341D33 File Offset: 0x0033FF33
	public virtual void OnXPressed()
	{
		this.windowManager.Close(this, false);
	}

	// Token: 0x06008050 RID: 32848 RVA: 0x00341D42 File Offset: 0x0033FF42
	public virtual PlayerActionsBase GetActionSet()
	{
		return this.playerUI.playerInput.GUIActions;
	}

	// Token: 0x06008051 RID: 32849 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool HasActionSet()
	{
		return true;
	}

	// Token: 0x06008052 RID: 32850 RVA: 0x00341D54 File Offset: 0x0033FF54
	public override bool Equals(object obj)
	{
		return obj is GUIWindow && ((GUIWindow)obj).id.Equals(this.id);
	}

	// Token: 0x06008053 RID: 32851 RVA: 0x00341D76 File Offset: 0x0033FF76
	public override int GetHashCode()
	{
		return this.id.GetHashCode();
	}

	// Token: 0x06008054 RID: 32852 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Cleanup()
	{
	}

	// Token: 0x0400631B RID: 25371
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly string id;

	// Token: 0x0400631C RID: 25372
	public bool bActionSetEnabled;

	// Token: 0x0400631D RID: 25373
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool bDrawBackground;

	// Token: 0x0400631E RID: 25374
	public Rect internalWindowRect;

	// Token: 0x0400631F RID: 25375
	public bool isShowing;

	// Token: 0x04006320 RID: 25376
	public bool isModal;

	// Token: 0x04006321 RID: 25377
	public bool alwaysUsesMouseCursor;

	// Token: 0x04006322 RID: 25378
	public bool isEscClosable;

	// Token: 0x04006323 RID: 25379
	public bool isInputActive;

	// Token: 0x04006324 RID: 25380
	public bool isDimBackground;

	// Token: 0x04006325 RID: 25381
	public GUIWindowManager windowManager;

	// Token: 0x04006326 RID: 25382
	public NGUIWindowManager nguiWindowManager;

	// Token: 0x04006327 RID: 25383
	public LocalPlayerUI playerUI;

	// Token: 0x04006328 RID: 25384
	public Matrix4x4 matrix = Matrix4x4.identity;

	// Token: 0x04006329 RID: 25385
	public bool bCenterWindow;

	// Token: 0x0400632A RID: 25386
	public string openWindowOnEsc = string.Empty;

	// Token: 0x0400632B RID: 25387
	public Action OnWindowClose;
}
