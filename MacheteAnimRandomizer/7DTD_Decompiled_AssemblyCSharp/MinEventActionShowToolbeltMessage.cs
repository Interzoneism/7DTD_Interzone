using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000671 RID: 1649
[Preserve]
public class MinEventActionShowToolbeltMessage : MinEventActionTargetedBase
{
	// Token: 0x06003190 RID: 12688 RVA: 0x00152158 File Offset: 0x00150358
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i] as EntityPlayerLocal != null)
			{
				if (this.sound != null)
				{
					GameManager.ShowTooltip(this.targets[i] as EntityPlayerLocal, this.message, string.Empty, this.sound, null, false, false, 0f);
				}
				else
				{
					GameManager.ShowTooltip(this.targets[i] as EntityPlayerLocal, this.message, false, false, 0f);
				}
			}
		}
	}

	// Token: 0x06003191 RID: 12689 RVA: 0x001521F3 File Offset: 0x001503F3
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && this.message != null;
	}

	// Token: 0x06003192 RID: 12690 RVA: 0x0015220C File Offset: 0x0015040C
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "message")
			{
				if (this.message == null || this.message == "")
				{
					this.message = _attribute.Value;
				}
				return true;
			}
			if (localName == "message_key")
			{
				if (Localization.Exists(_attribute.Value, false))
				{
					this.message = Localization.Get(_attribute.Value, false);
				}
				return true;
			}
			if (localName == "sound")
			{
				if (_attribute.Value != "")
				{
					this.sound = _attribute.Value;
				}
				return true;
			}
		}
		return flag;
	}

	// Token: 0x04002806 RID: 10246
	[PublicizedFrom(EAccessModifier.Private)]
	public string message;

	// Token: 0x04002807 RID: 10247
	[PublicizedFrom(EAccessModifier.Private)]
	public string sound;
}
