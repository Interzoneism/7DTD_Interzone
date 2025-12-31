using System;
using UnityEngine;

// Token: 0x02000FC8 RID: 4040
public abstract class GUIWindowUGUI : GUIWindow
{
	// Token: 0x17000D62 RID: 3426
	// (get) Token: 0x060080CB RID: 32971
	public abstract string UIPrefabPath { get; }

	// Token: 0x060080CC RID: 32972 RVA: 0x00344F8C File Offset: 0x0034318C
	public GUIWindowUGUI(string _id) : base(_id)
	{
		this.uiPrefab = DataLoader.LoadAsset<GameObject>(this.UIPrefabPath, false);
		this.canvas = UnityEngine.Object.Instantiate<GameObject>(this.uiPrefab).GetComponent<Canvas>();
		this.canvas.gameObject.SetActive(false);
	}

	// Token: 0x060080CD RID: 32973 RVA: 0x00344FD9 File Offset: 0x003431D9
	public override void OnOpen()
	{
		base.OnOpen();
		if (ThreadManager.IsMainThread())
		{
			this.canvas.gameObject.SetActive(true);
			return;
		}
		this.shouldOpen = true;
	}

	// Token: 0x060080CE RID: 32974 RVA: 0x00345001 File Offset: 0x00343201
	public override void Update()
	{
		if (this.shouldOpen && !this.canvas.gameObject.activeSelf)
		{
			this.canvas.gameObject.SetActive(true);
			this.shouldOpen = false;
		}
	}

	// Token: 0x060080CF RID: 32975 RVA: 0x00345035 File Offset: 0x00343235
	public override void OnClose()
	{
		base.OnClose();
		this.canvas.gameObject.SetActive(false);
	}

	// Token: 0x060080D0 RID: 32976 RVA: 0x0034504E File Offset: 0x0034324E
	public override void Cleanup()
	{
		UnityEngine.Object.Destroy(this.canvas.gameObject);
		this.uiPrefab = null;
	}

	// Token: 0x0400637E RID: 25470
	[PublicizedFrom(EAccessModifier.Protected)]
	public GameObject uiPrefab;

	// Token: 0x0400637F RID: 25471
	[PublicizedFrom(EAccessModifier.Protected)]
	public Canvas canvas;

	// Token: 0x04006380 RID: 25472
	[PublicizedFrom(EAccessModifier.Private)]
	public bool shouldOpen;
}
