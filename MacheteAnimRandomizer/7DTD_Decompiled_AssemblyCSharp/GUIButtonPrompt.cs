using System;
using Platform;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000FB7 RID: 4023
public class GUIButtonPrompt : MonoBehaviour
{
	// Token: 0x0600801F RID: 32799 RVA: 0x00341493 File Offset: 0x0033F693
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.image = base.GetComponent<Image>();
	}

	// Token: 0x06008020 RID: 32800 RVA: 0x003414A1 File Offset: 0x0033F6A1
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.RefreshIcon();
	}

	// Token: 0x06008021 RID: 32801 RVA: 0x003414AC File Offset: 0x0033F6AC
	public void RefreshIcon()
	{
		PlayerInputManager.InputStyle inputStyle = PlayerInputManager.InputStyleFromSelectedIconStyle();
		this.image.sprite = ((inputStyle == PlayerInputManager.InputStyle.PS4) ? this.PSSprite : this.XBSprite);
	}

	// Token: 0x0400630E RID: 25358
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public Sprite XBSprite;

	// Token: 0x0400630F RID: 25359
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public Sprite PSSprite;

	// Token: 0x04006310 RID: 25360
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Image image;
}
