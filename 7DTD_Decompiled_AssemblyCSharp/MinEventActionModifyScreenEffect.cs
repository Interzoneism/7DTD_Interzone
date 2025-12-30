using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000666 RID: 1638
[Preserve]
public class MinEventActionModifyScreenEffect : MinEventActionBase
{
	// Token: 0x06003171 RID: 12657 RVA: 0x001513A8 File Offset: 0x0014F5A8
	public override void Execute(MinEventParams _params)
	{
		EntityPlayerLocal entityPlayerLocal = _params.Self as EntityPlayerLocal;
		if (entityPlayerLocal != null)
		{
			entityPlayerLocal.ScreenEffectManager.SetScreenEffect(this.effect_name, this.intensity, this.fade);
		}
	}

	// Token: 0x06003172 RID: 12658 RVA: 0x001513E7 File Offset: 0x0014F5E7
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && _params.Self is EntityPlayerLocal && this.effect_name != "";
	}

	// Token: 0x06003173 RID: 12659 RVA: 0x00151414 File Offset: 0x0014F614
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "effect_name")
			{
				this.effect_name = _attribute.Value;
				return true;
			}
			if (localName == "intensity")
			{
				this.intensity = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				return true;
			}
			if (localName == "fade")
			{
				this.fade = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x040027D9 RID: 10201
	[PublicizedFrom(EAccessModifier.Private)]
	public string effect_name = "";

	// Token: 0x040027DA RID: 10202
	[PublicizedFrom(EAccessModifier.Private)]
	public float intensity = 1f;

	// Token: 0x040027DB RID: 10203
	[PublicizedFrom(EAccessModifier.Private)]
	public float fade = 4f;
}
