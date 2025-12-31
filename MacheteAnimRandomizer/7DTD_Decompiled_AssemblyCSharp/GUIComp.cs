using System;
using UnityEngine;

// Token: 0x02000FB8 RID: 4024
public abstract class GUIComp
{
	// Token: 0x06008023 RID: 32803
	public abstract void OnGUI();

	// Token: 0x06008024 RID: 32804 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnGUILayout()
	{
	}

	// Token: 0x06008025 RID: 32805 RVA: 0x003414DC File Offset: 0x0033F6DC
	public void SetPosition(int _x, int _y)
	{
		this.rect.x = (float)_x;
		this.rect.y = (float)_y;
	}

	// Token: 0x06008026 RID: 32806 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public GUIComp()
	{
	}

	// Token: 0x04006311 RID: 25361
	public Rect rect;
}
