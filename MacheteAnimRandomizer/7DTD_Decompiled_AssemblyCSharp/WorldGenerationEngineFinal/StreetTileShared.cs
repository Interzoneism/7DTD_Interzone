using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200145B RID: 5211
	public class StreetTileShared
	{
		// Token: 0x0600A178 RID: 41336 RVA: 0x00400094 File Offset: 0x003FE294
		public StreetTileShared(WorldBuilder _worldBuilder)
		{
			bool[][] array = new bool[5][];
			int num = 0;
			bool[] array2 = new bool[4];
			array2[0] = true;
			array2[2] = true;
			array[num] = array2;
			array[1] = new bool[]
			{
				true,
				true,
				true,
				false
			};
			array[2] = new bool[]
			{
				true,
				true,
				true,
				true
			};
			int num2 = 3;
			bool[] array3 = new bool[4];
			array3[2] = true;
			array[num2] = array3;
			int num3 = 4;
			bool[] array4 = new bool[4];
			array4[1] = true;
			array4[2] = true;
			array[num3] = array4;
			this.RoadShapeExits = array;
			this.RoadShapeExitCounts = new List<int>
			{
				2,
				3,
				4,
				1,
				2
			};
			this.RoadShapeExitsPerRotation = new List<bool[][]>();
			this.dir4way = new Vector2i[]
			{
				new Vector2i(0, 1),
				new Vector2i(1, 0),
				new Vector2i(0, -1),
				new Vector2i(-1, 0)
			};
			this.dir8way = new Vector2i[]
			{
				new Vector2i(0, 1),
				new Vector2i(1, 1),
				new Vector2i(1, 0),
				new Vector2i(1, -1),
				new Vector2i(0, -1),
				new Vector2i(-1, -1),
				new Vector2i(-1, 0),
				new Vector2i(-1, 1)
			};
			this.dir9way = new Vector2i[]
			{
				new Vector2i(0, 1),
				new Vector2i(1, 1),
				new Vector2i(1, 0),
				new Vector2i(1, -1),
				new Vector2i(0, 0),
				new Vector2i(0, -1),
				new Vector2i(-1, -1),
				new Vector2i(-1, 0),
				new Vector2i(-1, 1)
			};
			base..ctor();
			this.worldBuilder = _worldBuilder;
			for (int i = 0; i < this.RoadShapeExits.Length; i++)
			{
				bool[][] array5 = new bool[4][];
				for (int j = 0; j < 4; j++)
				{
					bool[][] array6 = array5;
					int num4 = j;
					bool[] array7;
					switch (j)
					{
					case 1:
						array7 = new bool[]
						{
							this.RoadShapeExits[i][3],
							this.RoadShapeExits[i][0],
							this.RoadShapeExits[i][1],
							this.RoadShapeExits[i][2]
						};
						break;
					case 2:
						array7 = new bool[]
						{
							this.RoadShapeExits[i][2],
							this.RoadShapeExits[i][3],
							this.RoadShapeExits[i][0],
							this.RoadShapeExits[i][1]
						};
						break;
					case 3:
						array7 = new bool[]
						{
							this.RoadShapeExits[i][1],
							this.RoadShapeExits[i][2],
							this.RoadShapeExits[i][3],
							this.RoadShapeExits[i][0]
						};
						break;
					default:
						array7 = new bool[]
						{
							this.RoadShapeExits[i][0],
							this.RoadShapeExits[i][1],
							this.RoadShapeExits[i][2],
							this.RoadShapeExits[i][3]
						};
						break;
					}
					array6[num4] = array7;
				}
				this.RoadShapeExitsPerRotation.Add(array5);
			}
		}

		// Token: 0x04007C6B RID: 31851
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04007C6C RID: 31852
		public readonly FastTags<TagGroup.Poi> traderTag = FastTags<TagGroup.Poi>.Parse("trader");

		// Token: 0x04007C6D RID: 31853
		public readonly FastTags<TagGroup.Poi> wildernessTag = FastTags<TagGroup.Poi>.Parse("wilderness");

		// Token: 0x04007C6E RID: 31854
		public readonly string[] RoadShapes = new string[]
		{
			"rwg_tile_straight",
			"rwg_tile_t",
			"rwg_tile_intersection",
			"rwg_tile_cap",
			"rwg_tile_corner"
		};

		// Token: 0x04007C6F RID: 31855
		public readonly string[] RoadShapesDistrict = new string[]
		{
			"rwg_tile_{0}straight",
			"rwg_tile_{0}t",
			"rwg_tile_{0}intersection",
			"rwg_tile_{0}cap",
			"rwg_tile_{0}corner"
		};

		// Token: 0x04007C70 RID: 31856
		[PublicizedFrom(EAccessModifier.Private)]
		public bool[][] RoadShapeExits;

		// Token: 0x04007C71 RID: 31857
		public readonly List<int> RoadShapeExitCounts;

		// Token: 0x04007C72 RID: 31858
		public readonly List<bool[][]> RoadShapeExitsPerRotation;

		// Token: 0x04007C73 RID: 31859
		public readonly Vector2i[] dir4way;

		// Token: 0x04007C74 RID: 31860
		public readonly Vector2i[] dir8way;

		// Token: 0x04007C75 RID: 31861
		public readonly Vector2i[] dir9way;
	}
}
