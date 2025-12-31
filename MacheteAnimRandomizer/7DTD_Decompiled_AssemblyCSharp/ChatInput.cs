using System;
using UnityEngine;

// Token: 0x02000045 RID: 69
[RequireComponent(typeof(UIInput))]
[AddComponentMenu("NGUI/Examples/Chat Input")]
public class ChatInput : MonoBehaviour
{
	// Token: 0x06000165 RID: 357 RVA: 0x0000E6CC File Offset: 0x0000C8CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.mInput = base.GetComponent<UIInput>();
		this.mInput.label.maxLineCount = 1;
		if (this.fillWithDummyData && this.textList != null)
		{
			for (int i = 0; i < 30; i++)
			{
				this.textList.Add(((i % 2 == 0) ? "[FFFFFF]" : "[AAAAAA]") + "This is an example paragraph for the text list, testing line " + i.ToString() + "[-]");
			}
		}
	}

	// Token: 0x06000166 RID: 358 RVA: 0x0000E74C File Offset: 0x0000C94C
	public void OnSubmit()
	{
		if (this.textList != null)
		{
			string text = NGUIText.StripSymbols(this.mInput.value);
			if (!string.IsNullOrEmpty(text))
			{
				this.textList.Add(text);
				this.mInput.value = "";
				this.mInput.isSelected = false;
			}
		}
	}

	// Token: 0x04000218 RID: 536
	public UITextList textList;

	// Token: 0x04000219 RID: 537
	public bool fillWithDummyData;

	// Token: 0x0400021A RID: 538
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UIInput mInput;
}
