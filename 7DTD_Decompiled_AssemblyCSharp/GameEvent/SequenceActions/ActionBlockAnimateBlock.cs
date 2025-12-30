using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016C5 RID: 5829
	[Preserve]
	public class ActionBlockAnimateBlock : ActionBaseBlockAction
	{
		// Token: 0x0600B102 RID: 45314 RVA: 0x00450FE0 File Offset: 0x0044F1E0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (!blockValue.isair)
			{
				Chunk chunk = (Chunk)world.GetChunkFromWorldPos(currentPos);
				if (chunk != null)
				{
					BlockEntityData blockEntity = world.ChunkClusters[chunk.ClrIdx].GetBlockEntity(currentPos);
					if (blockEntity != null)
					{
						if (blockEntity.transform == null)
						{
							GameManager.Instance.StartCoroutine(this.WaitForBEDTransform(blockEntity));
						}
						else
						{
							this.AnimateBlock(blockEntity);
						}
					}
				}
			}
			return null;
		}

		// Token: 0x0600B103 RID: 45315 RVA: 0x0045104B File Offset: 0x0044F24B
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator WaitForBEDTransform(BlockEntityData bed)
		{
			int num;
			for (int frames = 0; frames < 10; frames = num + 1)
			{
				yield return 0;
				if (bed == null)
				{
					yield break;
				}
				if (bed.transform != null)
				{
					this.AnimateBlock(bed);
					yield break;
				}
				num = frames;
			}
			yield break;
		}

		// Token: 0x0600B104 RID: 45316 RVA: 0x00451064 File Offset: 0x0044F264
		[PublicizedFrom(EAccessModifier.Private)]
		public void AnimateBlock(BlockEntityData bed)
		{
			Animator[] componentsInChildren = bed.transform.GetComponentsInChildren<Animator>();
			if (componentsInChildren != null)
			{
				for (int i = componentsInChildren.Length - 1; i >= 0; i--)
				{
					Animator animator = componentsInChildren[i];
					animator.enabled = true;
					if (this.animationBool != null)
					{
						animator.SetBool(this.animationBool, this.animationBoolValue);
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageAnimateBlock>().Setup(bed.pos, this.animationBool, this.animationBoolValue), false, -1, -1, -1, null, 192, false);
					}
					if (this.animationInteger != null)
					{
						animator.SetInteger(this.animationInteger, this.animationIntegerValue);
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageAnimateBlock>().Setup(bed.pos, this.animationInteger, this.animationIntegerValue), false, -1, -1, -1, null, 192, false);
					}
					if (this.animationTrigger != null)
					{
						animator.SetTrigger(this.animationTrigger);
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageAnimateBlock>().Setup(bed.pos, this.animationTrigger), false, -1, -1, -1, null, 192, false);
					}
				}
			}
		}

		// Token: 0x0600B105 RID: 45317 RVA: 0x00451194 File Offset: 0x0044F394
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionBlockAnimateBlock.PropAnimationBool, ref this.animationBool);
			properties.ParseBool(ActionBlockAnimateBlock.PropAnimationBoolValue, ref this.animationBoolValue);
			properties.ParseString(ActionBlockAnimateBlock.PropAnimationInteger, ref this.animationInteger);
			properties.ParseInt(ActionBlockAnimateBlock.PropAnimationIntegerValue, ref this.animationIntegerValue);
			properties.ParseString(ActionBlockAnimateBlock.PropAnimationTrigger, ref this.animationTrigger);
		}

		// Token: 0x0600B106 RID: 45318 RVA: 0x00451200 File Offset: 0x0044F400
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockAnimateBlock
			{
				animationBool = this.animationBool,
				animationBoolValue = this.animationBoolValue,
				animationInteger = this.animationInteger,
				animationIntegerValue = this.animationIntegerValue,
				animationTrigger = this.animationTrigger
			};
		}

		// Token: 0x04008A89 RID: 35465
		[PublicizedFrom(EAccessModifier.Protected)]
		public string animationBool;

		// Token: 0x04008A8A RID: 35466
		[PublicizedFrom(EAccessModifier.Protected)]
		public string animationInteger;

		// Token: 0x04008A8B RID: 35467
		[PublicizedFrom(EAccessModifier.Protected)]
		public string animationTrigger;

		// Token: 0x04008A8C RID: 35468
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool animationBoolValue = true;

		// Token: 0x04008A8D RID: 35469
		[PublicizedFrom(EAccessModifier.Protected)]
		public int animationIntegerValue;

		// Token: 0x04008A8E RID: 35470
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAnimationBool = "animation_bool";

		// Token: 0x04008A8F RID: 35471
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAnimationBoolValue = "animation_bool_value";

		// Token: 0x04008A90 RID: 35472
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAnimationInteger = "animation_integer";

		// Token: 0x04008A91 RID: 35473
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAnimationIntegerValue = "animation_integer_value";

		// Token: 0x04008A92 RID: 35474
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAnimationTrigger = "animation_trigger";
	}
}
