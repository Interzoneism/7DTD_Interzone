using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200161B RID: 5659
	[Preserve]
	public class RequirementHasEntityTag : BaseRequirement
	{
		// Token: 0x0600ADF7 RID: 44535 RVA: 0x004404A0 File Offset: 0x0043E6A0
		public override bool CanPerform(Entity target)
		{
			FastTags<TagGroup.Global> tags = (this.tag == "") ? FastTags<TagGroup.Global>.none : FastTags<TagGroup.Global>.Parse(this.tag);
			if (!(target is EntityAlive))
			{
				return false;
			}
			if (target.HasAnyTags(tags))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600ADF8 RID: 44536 RVA: 0x004404F5 File Offset: 0x0043E6F5
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementHasEntityTag.PropTag, ref this.tag);
		}

		// Token: 0x0600ADF9 RID: 44537 RVA: 0x0044050F File Offset: 0x0043E70F
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementHasEntityTag
			{
				tag = this.tag,
				Invert = this.Invert
			};
		}

		// Token: 0x0400870F RID: 34575
		[PublicizedFrom(EAccessModifier.Protected)]
		public string tag = "";

		// Token: 0x04008710 RID: 34576
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTag = "entity_tags";
	}
}
