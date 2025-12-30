using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001610 RID: 5648
	[Preserve]
	public class BaseRequirement
	{
		// Token: 0x0600ADB1 RID: 44465 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnInit()
		{
		}

		// Token: 0x0600ADB2 RID: 44466 RVA: 0x0043FD66 File Offset: 0x0043DF66
		public void Init()
		{
			this.OnInit();
		}

		// Token: 0x0600ADB3 RID: 44467 RVA: 0x000197A5 File Offset: 0x000179A5
		public virtual bool CanPerform(Entity target)
		{
			return true;
		}

		// Token: 0x0600ADB4 RID: 44468 RVA: 0x0043FD70 File Offset: 0x0043DF70
		public virtual void ParseProperties(DynamicProperties properties)
		{
			this.Properties = properties;
			this.Owner.HandleVariablesForProperties(properties);
			if (properties.Values.ContainsKey(BaseRequirement.PropInvert))
			{
				this.Invert = StringParsers.ParseBool(properties.Values[BaseRequirement.PropInvert], 0, -1, true);
			}
		}

		// Token: 0x0600ADB5 RID: 44469 RVA: 0x0043FDC0 File Offset: 0x0043DFC0
		public virtual BaseRequirement Clone()
		{
			BaseRequirement baseRequirement = this.CloneChildSettings();
			if (this.Properties != null)
			{
				baseRequirement.Properties = new DynamicProperties();
				baseRequirement.Properties.CopyFrom(this.Properties, null);
			}
			return baseRequirement;
		}

		// Token: 0x0600ADB6 RID: 44470 RVA: 0x00019766 File Offset: 0x00017966
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual BaseRequirement CloneChildSettings()
		{
			return null;
		}

		// Token: 0x040086F0 RID: 34544
		public DynamicProperties Properties;

		// Token: 0x040086F1 RID: 34545
		public GameEventActionSequence Owner;

		// Token: 0x040086F2 RID: 34546
		public bool Invert;

		// Token: 0x040086F3 RID: 34547
		public static string PropInvert = "invert";
	}
}
