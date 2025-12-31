using System;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class CharacterClickHandler : MonoBehaviour
{
	// Token: 0x0600004F RID: 79 RVA: 0x00006EC5 File Offset: 0x000050C5
	public void HandleClick()
	{
		this.parentScript.OnCharacterClicked(base.gameObject);
	}

	// Token: 0x0400009A RID: 154
	public CharacterConstruct parentScript;
}
