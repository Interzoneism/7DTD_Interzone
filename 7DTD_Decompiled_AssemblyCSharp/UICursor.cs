using System;
using UnityEngine;

// Token: 0x02000036 RID: 54
[RequireComponent(typeof(UISprite))]
[AddComponentMenu("NGUI/Examples/UI Cursor")]
public class UICursor : MonoBehaviour
{
	// Token: 0x06000127 RID: 295 RVA: 0x0000D3C3 File Offset: 0x0000B5C3
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		UICursor.instance = this;
	}

	// Token: 0x06000128 RID: 296 RVA: 0x0000D3CB File Offset: 0x0000B5CB
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		UICursor.instance = null;
	}

	// Token: 0x06000129 RID: 297 RVA: 0x0000D3D4 File Offset: 0x0000B5D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.mTrans = base.transform;
		this.mSprite = base.GetComponentInChildren<UISprite>();
		if (this.uiCamera == null)
		{
			this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		if (this.mSprite != null)
		{
			this.mAtlas = this.mSprite.atlas;
			this.mSpriteName = this.mSprite.spriteName;
			if (this.mSprite.depth < 100)
			{
				this.mSprite.depth = 100;
			}
		}
	}

	// Token: 0x0600012A RID: 298 RVA: 0x0000D46C File Offset: 0x0000B66C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		Vector3 mousePosition = Input.mousePosition;
		if (this.uiCamera != null)
		{
			mousePosition.x = Mathf.Clamp01(mousePosition.x / (float)Screen.width);
			mousePosition.y = Mathf.Clamp01(mousePosition.y / (float)Screen.height);
			this.mTrans.position = this.uiCamera.ViewportToWorldPoint(mousePosition);
			if (this.uiCamera.orthographic)
			{
				Vector3 localPosition = this.mTrans.localPosition;
				localPosition.x = Mathf.Round(localPosition.x);
				localPosition.y = Mathf.Round(localPosition.y);
				this.mTrans.localPosition = localPosition;
				return;
			}
		}
		else
		{
			mousePosition.x -= (float)Screen.width * 0.5f;
			mousePosition.y -= (float)Screen.height * 0.5f;
			mousePosition.x = Mathf.Round(mousePosition.x);
			mousePosition.y = Mathf.Round(mousePosition.y);
			this.mTrans.localPosition = mousePosition;
		}
	}

	// Token: 0x0600012B RID: 299 RVA: 0x0000D584 File Offset: 0x0000B784
	public static void Clear()
	{
		if (UICursor.instance != null && UICursor.instance.mSprite != null)
		{
			UICursor.Set(UICursor.instance.mAtlas, UICursor.instance.mSpriteName);
		}
	}

	// Token: 0x0600012C RID: 300 RVA: 0x0000D5C0 File Offset: 0x0000B7C0
	public static void Set(INGUIAtlas atlas, string sprite)
	{
		if (UICursor.instance != null && UICursor.instance.mSprite)
		{
			UICursor.instance.mSprite.atlas = atlas;
			UICursor.instance.mSprite.spriteName = sprite;
			UICursor.instance.mSprite.MakePixelPerfect();
			UICursor.instance.Update();
		}
	}

	// Token: 0x040001BB RID: 443
	public static UICursor instance;

	// Token: 0x040001BC RID: 444
	public Camera uiCamera;

	// Token: 0x040001BD RID: 445
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform mTrans;

	// Token: 0x040001BE RID: 446
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UISprite mSprite;

	// Token: 0x040001BF RID: 447
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public INGUIAtlas mAtlas;

	// Token: 0x040001C0 RID: 448
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string mSpriteName;
}
