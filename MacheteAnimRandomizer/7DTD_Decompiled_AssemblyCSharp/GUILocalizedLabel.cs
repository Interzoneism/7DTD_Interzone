using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000FBA RID: 4026
public class GUILocalizedLabel : MonoBehaviour
{
	// Token: 0x06008036 RID: 32822 RVA: 0x00341A34 File Offset: 0x0033FC34
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		Text component = base.GetComponent<Text>();
		if (component)
		{
			component.text = Localization.Get(this.localizationKey, false);
		}
	}

	// Token: 0x0400631A RID: 25370
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizationKey;
}
