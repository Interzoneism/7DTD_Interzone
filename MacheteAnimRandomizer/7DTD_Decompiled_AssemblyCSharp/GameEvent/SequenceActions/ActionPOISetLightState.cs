using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016DD RID: 5853
	[Preserve]
	public class ActionPOISetLightState : BaseAction
	{
		// Token: 0x0600B170 RID: 45424 RVA: 0x00453254 File Offset: 0x00451454
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			Vector3i poiposition = base.Owner.POIPosition;
			World world = GameManager.Instance.World;
			PrefabInstance poiinstance = base.Owner.POIInstance;
			if (poiinstance == null)
			{
				return BaseAction.ActionCompleteStates.InCompleteRefund;
			}
			Vector3i size = poiinstance.prefab.size;
			int num = World.toChunkXZ(poiposition.x - 1);
			int num2 = World.toChunkXZ(poiposition.x + size.x + 1);
			int num3 = World.toChunkXZ(poiposition.z - 1);
			int num4 = World.toChunkXZ(poiposition.z + size.z + 1);
			Rect rect = new Rect((float)poiposition.x, (float)poiposition.z, (float)size.x, (float)size.z);
			List<BlockChangeInfo> list = new List<BlockChangeInfo>();
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					Chunk chunk = world.GetChunkSync(i, j) as Chunk;
					if (chunk != null)
					{
						for (int k = 0; k < this.indexBlockNames.Length; k++)
						{
							List<Vector3i> list2 = chunk.IndexedBlocks[this.indexBlockNames[k]];
							if (list2 != null)
							{
								for (int l = 0; l < list2.Count; l++)
								{
									BlockValue blockValue = chunk.GetBlock(list2[l]);
									if (!blockValue.ischild)
									{
										Vector3i vector3i = chunk.ToWorldPos(list2[l]);
										if (rect.Contains(new Vector2((float)vector3i.x, (float)vector3i.z)))
										{
											BlockLight blockLight = blockValue.Block as BlockLight;
											if (blockLight != null && blockLight.OriginalLightState(blockValue))
											{
												blockValue = blockLight.SetLightState(world, chunk.ClrIdx, vector3i, blockValue, this.enableLights);
												list.Add(new BlockChangeInfo(chunk.ClrIdx, vector3i, blockValue));
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (list.Count > 0)
			{
				GameManager.Instance.StartCoroutine(this.UpdateBlocks(list));
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B171 RID: 45425 RVA: 0x0045345C File Offset: 0x0045165C
		[PublicizedFrom(EAccessModifier.Protected)]
		public IEnumerator UpdateBlocks(List<BlockChangeInfo> blockChanges)
		{
			yield return new WaitForSeconds(1f);
			GameManager.Instance.World.SetBlocksRPC(blockChanges);
			yield break;
		}

		// Token: 0x0600B172 RID: 45426 RVA: 0x0045346C File Offset: 0x0045166C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseBool(ActionPOISetLightState.PropEnabled, ref this.enableLights);
			string text = "";
			properties.ParseString(ActionPOISetLightState.PropIndexBlockName, ref text);
			this.indexBlockNames = text.Split(',', StringSplitOptions.None);
		}

		// Token: 0x0600B173 RID: 45427 RVA: 0x004534B3 File Offset: 0x004516B3
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionPOISetLightState
			{
				enableLights = this.enableLights,
				indexBlockNames = this.indexBlockNames
			};
		}

		// Token: 0x04008AE3 RID: 35555
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool enableLights;

		// Token: 0x04008AE4 RID: 35556
		[PublicizedFrom(EAccessModifier.Protected)]
		public string[] indexBlockNames;

		// Token: 0x04008AE5 RID: 35557
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropEnabled = "enable_lights";

		// Token: 0x04008AE6 RID: 35558
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIndexBlockName = "index_block_name";
	}
}
