using System;
using UnityEngine;

// Token: 0x02000056 RID: 86
[AddComponentMenu("NGUI/Examples/Slider Colors")]
public class UISliderColors : MonoBehaviour
{
	// Token: 0x0600019C RID: 412 RVA: 0x0000F2D7 File Offset: 0x0000D4D7
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.mBar = base.GetComponent<UIProgressBar>();
		this.mSprite = base.GetComponent<UIBasicSprite>();
		this.Update();
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000F2F8 File Offset: 0x0000D4F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.sprite == null || this.colors.Length == 0)
		{
			return;
		}
		float num = (this.mBar != null) ? this.mBar.value : this.mSprite.fillAmount;
		num *= (float)(this.colors.Length - 1);
		int num2 = Mathf.FloorToInt(num);
		Color color = this.colors[0];
		if (num2 >= 0)
		{
			if (num2 + 1 < this.colors.Length)
			{
				float t = num - (float)num2;
				color = Color.Lerp(this.colors[num2], this.colors[num2 + 1], t);
			}
			else if (num2 < this.colors.Length)
			{
				color = this.colors[num2];
			}
			else
			{
				color = this.colors[this.colors.Length - 1];
			}
		}
		color.a = this.sprite.color.a;
		this.sprite.color = color;
	}

	// Token: 0x04000247 RID: 583
	public UISprite sprite;

	// Token: 0x04000248 RID: 584
	public Color[] colors = new Color[]
	{
		Color.red,
		Color.yellow,
		Color.green
	};

	// Token: 0x04000249 RID: 585
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UIProgressBar mBar;

	// Token: 0x0400024A RID: 586
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UIBasicSprite mSprite;
}
