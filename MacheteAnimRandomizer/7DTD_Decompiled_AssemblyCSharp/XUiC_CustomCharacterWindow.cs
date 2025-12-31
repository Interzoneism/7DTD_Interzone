using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C77 RID: 3191
[Preserve]
public class XUiC_CustomCharacterWindow : XUiController
{
	// Token: 0x06006254 RID: 25172 RVA: 0x0027E3AC File Offset: 0x0027C5AC
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (!(name == "locked_sprite"))
			{
				if (!(name == "unlocked_sprite"))
				{
					if (!(name == "locked_color"))
					{
						if (!(name == "unlocked_color"))
						{
							return false;
						}
						this.unlockedColor = StringParsers.ParseColor32(value);
					}
					else
					{
						this.lockedColor = StringParsers.ParseColor32(value);
					}
				}
				else
				{
					this.unlockedSprite = value;
				}
			}
			else
			{
				this.lockedSprite = value;
			}
			return true;
		}
		return flag;
	}

	// Token: 0x040049FC RID: 18940
	public string lockedSprite = "ui_game_symbol_lock";

	// Token: 0x040049FD RID: 18941
	public string unlockedSprite = "ui_game_symbol_lock";

	// Token: 0x040049FE RID: 18942
	public Color lockedColor;

	// Token: 0x040049FF RID: 18943
	public Color unlockedColor;
}
