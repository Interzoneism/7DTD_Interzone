using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200165C RID: 5724
	[Preserve]
	public class ActionClearGroup : BaseAction
	{
		// Token: 0x0600AF23 RID: 44835 RVA: 0x004464D1 File Offset: 0x004446D1
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			base.Owner.ClearEntityGroup(this.groupName);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AF24 RID: 44836 RVA: 0x004464E5 File Offset: 0x004446E5
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionClearGroup.PropGroupName, ref this.groupName);
		}

		// Token: 0x0600AF25 RID: 44837 RVA: 0x004464FF File Offset: 0x004446FF
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionClearGroup
			{
				groupName = this.groupName
			};
		}

		// Token: 0x04008844 RID: 34884
		[PublicizedFrom(EAccessModifier.Protected)]
		public string groupName = "";

		// Token: 0x04008845 RID: 34885
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGroupName = "group_name";
	}
}
