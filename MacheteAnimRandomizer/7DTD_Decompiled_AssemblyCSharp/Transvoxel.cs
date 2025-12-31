using System;
using UnityEngine;

// Token: 0x020005AC RID: 1452
public class Transvoxel
{
	// Token: 0x06002EF4 RID: 12020 RVA: 0x0013C508 File Offset: 0x0013A708
	// Note: this type is marked as 'beforefieldinit'.
	[PublicizedFrom(EAccessModifier.Private)]
	static Transvoxel()
	{
		Transvoxel.RegularCellData[,] array = new Transvoxel.RegularCellData[2, 16];
		array[0, 0] = new Transvoxel.RegularCellData(0, new byte[0]);
		array[0, 1] = new Transvoxel.RegularCellData(49, new byte[]
		{
			0,
			1,
			2
		});
		array[0, 2] = new Transvoxel.RegularCellData(98, new byte[]
		{
			0,
			1,
			2,
			3,
			4,
			5
		});
		array[0, 3] = new Transvoxel.RegularCellData(66, new byte[]
		{
			0,
			1,
			2,
			0,
			2,
			3
		});
		array[0, 4] = new Transvoxel.RegularCellData(83, new byte[]
		{
			0,
			1,
			4,
			1,
			3,
			4,
			1,
			2,
			3
		});
		array[0, 5] = new Transvoxel.RegularCellData(115, new byte[]
		{
			0,
			1,
			2,
			0,
			2,
			3,
			4,
			5,
			6
		});
		array[0, 6] = new Transvoxel.RegularCellData(147, new byte[]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8
		});
		array[0, 7] = new Transvoxel.RegularCellData(132, new byte[]
		{
			0,
			1,
			4,
			1,
			3,
			4,
			1,
			2,
			3,
			5,
			6,
			7
		});
		array[0, 8] = new Transvoxel.RegularCellData(132, new byte[]
		{
			0,
			1,
			2,
			0,
			2,
			3,
			4,
			5,
			6,
			4,
			6,
			7
		});
		array[0, 9] = new Transvoxel.RegularCellData(196, new byte[]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11
		});
		array[0, 10] = new Transvoxel.RegularCellData(100, new byte[]
		{
			0,
			4,
			5,
			0,
			1,
			4,
			1,
			3,
			4,
			1,
			2,
			3
		});
		array[0, 11] = new Transvoxel.RegularCellData(100, new byte[]
		{
			0,
			5,
			4,
			0,
			4,
			1,
			1,
			4,
			3,
			1,
			3,
			2
		});
		array[0, 12] = new Transvoxel.RegularCellData(100, new byte[]
		{
			0,
			4,
			5,
			0,
			3,
			4,
			0,
			1,
			3,
			1,
			2,
			3
		});
		array[0, 13] = new Transvoxel.RegularCellData(100, new byte[]
		{
			0,
			1,
			2,
			0,
			2,
			3,
			0,
			3,
			4,
			0,
			4,
			5
		});
		array[0, 14] = new Transvoxel.RegularCellData(117, new byte[]
		{
			0,
			1,
			2,
			0,
			2,
			3,
			0,
			3,
			4,
			0,
			4,
			5,
			0,
			5,
			6
		});
		array[0, 15] = new Transvoxel.RegularCellData(149, new byte[]
		{
			0,
			4,
			5,
			0,
			3,
			4,
			0,
			1,
			3,
			1,
			2,
			3,
			6,
			7,
			8
		});
		array[1, 0] = new Transvoxel.RegularCellData(0, new byte[0]);
		array[1, 1] = new Transvoxel.RegularCellData(0, new byte[0]);
		array[1, 2] = new Transvoxel.RegularCellData(0, new byte[0]);
		array[1, 3] = new Transvoxel.RegularCellData(66, new byte[]
		{
			0,
			1,
			3,
			1,
			2,
			3
		});
		array[1, 4] = new Transvoxel.RegularCellData(83, new byte[]
		{
			0,
			1,
			2,
			0,
			2,
			4,
			2,
			3,
			4
		});
		array[1, 5] = new Transvoxel.RegularCellData(115, new byte[]
		{
			0,
			1,
			3,
			1,
			2,
			3,
			4,
			5,
			6
		});
		array[1, 6] = new Transvoxel.RegularCellData(0, new byte[0]);
		array[1, 7] = new Transvoxel.RegularCellData(132, new byte[]
		{
			0,
			1,
			2,
			0,
			2,
			4,
			2,
			3,
			4,
			5,
			6,
			7
		});
		array[1, 8] = new Transvoxel.RegularCellData(132, new byte[]
		{
			0,
			1,
			3,
			1,
			2,
			3,
			4,
			5,
			7,
			5,
			6,
			7
		});
		array[1, 9] = new Transvoxel.RegularCellData(0, new byte[0]);
		array[1, 10] = new Transvoxel.RegularCellData(100, new byte[]
		{
			0,
			1,
			2,
			0,
			2,
			5,
			2,
			3,
			5,
			3,
			4,
			5
		});
		array[1, 11] = new Transvoxel.RegularCellData(100, new byte[]
		{
			0,
			2,
			1,
			0,
			5,
			2,
			2,
			5,
			3,
			3,
			5,
			4
		});
		array[1, 12] = new Transvoxel.RegularCellData(100, new byte[]
		{
			0,
			1,
			5,
			1,
			2,
			5,
			2,
			3,
			4,
			2,
			4,
			5
		});
		array[1, 13] = new Transvoxel.RegularCellData(100, new byte[]
		{
			0,
			1,
			5,
			1,
			4,
			5,
			1,
			2,
			4,
			2,
			3,
			4
		});
		array[1, 14] = new Transvoxel.RegularCellData(117, new byte[]
		{
			0,
			1,
			6,
			1,
			5,
			6,
			1,
			2,
			5,
			2,
			4,
			5,
			2,
			3,
			4
		});
		array[1, 15] = new Transvoxel.RegularCellData(149, new byte[]
		{
			0,
			1,
			5,
			1,
			2,
			5,
			2,
			3,
			4,
			2,
			4,
			5,
			6,
			7,
			8
		});
		Transvoxel.regularCellData = array;
		Transvoxel.InternalEdgeData[,] array2 = new Transvoxel.InternalEdgeData[2, 16];
		array2[0, 0] = new Transvoxel.InternalEdgeData(0, new byte[1, 0]);
		array2[0, 1] = new Transvoxel.InternalEdgeData(0, new byte[1, 0]);
		array2[0, 2] = new Transvoxel.InternalEdgeData(0, new byte[1, 0]);
		array2[0, 3] = new Transvoxel.InternalEdgeData(1, new byte[,]
		{
			{
				0,
				2,
				1,
				3
			}
		});
		array2[0, 4] = new Transvoxel.InternalEdgeData(2, new byte[,]
		{
			{
				1,
				3,
				2,
				4
			},
			{
				1,
				4,
				3,
				0
			}
		});
		array2[0, 5] = new Transvoxel.InternalEdgeData(1, new byte[,]
		{
			{
				0,
				2,
				1,
				3
			}
		});
		array2[0, 6] = new Transvoxel.InternalEdgeData(0, new byte[1, 0]);
		array2[0, 7] = new Transvoxel.InternalEdgeData(2, new byte[,]
		{
			{
				1,
				3,
				2,
				4
			},
			{
				1,
				4,
				3,
				0
			}
		});
		array2[0, 8] = new Transvoxel.InternalEdgeData(2, new byte[,]
		{
			{
				0,
				2,
				1,
				3
			},
			{
				4,
				6,
				5,
				7
			}
		});
		array2[0, 9] = new Transvoxel.InternalEdgeData(0, new byte[1, 0]);
		array2[0, 10] = new Transvoxel.InternalEdgeData(3, new byte[,]
		{
			{
				0,
				4,
				1,
				5
			},
			{
				1,
				4,
				3,
				0
			},
			{
				1,
				3,
				2,
				4
			}
		});
		array2[0, 11] = new Transvoxel.InternalEdgeData(3, new byte[,]
		{
			{
				0,
				4,
				5,
				1
			},
			{
				1,
				4,
				0,
				3
			},
			{
				1,
				3,
				4,
				2
			}
		});
		array2[0, 12] = new Transvoxel.InternalEdgeData(3, new byte[,]
		{
			{
				1,
				3,
				2,
				0
			},
			{
				0,
				3,
				1,
				4
			},
			{
				0,
				4,
				3,
				5
			}
		});
		array2[0, 13] = new Transvoxel.InternalEdgeData(3, new byte[,]
		{
			{
				0,
				2,
				1,
				3
			},
			{
				0,
				3,
				2,
				4
			},
			{
				0,
				4,
				3,
				5
			}
		});
		array2[0, 14] = new Transvoxel.InternalEdgeData(4, new byte[,]
		{
			{
				0,
				2,
				1,
				3
			},
			{
				0,
				3,
				2,
				4
			},
			{
				0,
				4,
				3,
				5
			},
			{
				0,
				5,
				4,
				6
			}
		});
		array2[0, 15] = new Transvoxel.InternalEdgeData(3, new byte[,]
		{
			{
				1,
				3,
				2,
				0
			},
			{
				0,
				3,
				1,
				4
			},
			{
				0,
				4,
				3,
				5
			}
		});
		array2[1, 0] = new Transvoxel.InternalEdgeData(0, new byte[1, 0]);
		array2[1, 1] = new Transvoxel.InternalEdgeData(0, new byte[1, 0]);
		array2[1, 2] = new Transvoxel.InternalEdgeData(0, new byte[1, 0]);
		array2[1, 3] = new Transvoxel.InternalEdgeData(1, new byte[,]
		{
			{
				1,
				3,
				2,
				0
			}
		});
		array2[1, 4] = new Transvoxel.InternalEdgeData(2, new byte[,]
		{
			{
				0,
				2,
				1,
				4
			},
			{
				2,
				4,
				3,
				0
			}
		});
		array2[1, 5] = new Transvoxel.InternalEdgeData(1, new byte[,]
		{
			{
				1,
				3,
				2,
				0
			}
		});
		array2[1, 6] = new Transvoxel.InternalEdgeData(0, new byte[1, 0]);
		array2[1, 7] = new Transvoxel.InternalEdgeData(2, new byte[,]
		{
			{
				0,
				2,
				1,
				4
			},
			{
				2,
				4,
				3,
				0
			}
		});
		array2[1, 8] = new Transvoxel.InternalEdgeData(2, new byte[,]
		{
			{
				1,
				3,
				2,
				0
			},
			{
				5,
				7,
				6,
				4
			}
		});
		array2[1, 9] = new Transvoxel.InternalEdgeData(0, new byte[1, 0]);
		array2[1, 10] = new Transvoxel.InternalEdgeData(3, new byte[,]
		{
			{
				0,
				2,
				1,
				5
			},
			{
				2,
				5,
				3,
				0
			},
			{
				3,
				5,
				4,
				2
			}
		});
		array2[1, 11] = new Transvoxel.InternalEdgeData(3, new byte[,]
		{
			{
				0,
				2,
				5,
				1
			},
			{
				2,
				5,
				0,
				3
			},
			{
				3,
				5,
				2,
				4
			}
		});
		array2[1, 12] = new Transvoxel.InternalEdgeData(3, new byte[,]
		{
			{
				1,
				5,
				2,
				0
			},
			{
				2,
				5,
				4,
				1
			},
			{
				2,
				4,
				3,
				5
			}
		});
		array2[1, 13] = new Transvoxel.InternalEdgeData(3, new byte[,]
		{
			{
				1,
				5,
				4,
				0
			},
			{
				1,
				4,
				2,
				5
			},
			{
				2,
				4,
				3,
				1
			}
		});
		array2[1, 14] = new Transvoxel.InternalEdgeData(4, new byte[,]
		{
			{
				1,
				6,
				5,
				0
			},
			{
				1,
				5,
				2,
				6
			},
			{
				2,
				5,
				4,
				1
			},
			{
				2,
				4,
				3,
				5
			}
		});
		array2[1, 15] = new Transvoxel.InternalEdgeData(3, new byte[,]
		{
			{
				1,
				5,
				2,
				0
			},
			{
				2,
				5,
				4,
				1
			},
			{
				2,
				4,
				3,
				5
			}
		});
		Transvoxel.regularInternalEdgeData = array2;
		Transvoxel.regularVertexData = new Transvoxel.RegularVertexData();
		Transvoxel.transitionCellClass = new byte[]
		{
			0,
			1,
			2,
			132,
			1,
			5,
			4,
			4,
			2,
			135,
			9,
			140,
			132,
			11,
			5,
			5,
			1,
			8,
			7,
			141,
			5,
			15,
			139,
			11,
			4,
			13,
			12,
			28,
			4,
			139,
			133,
			133,
			2,
			7,
			9,
			140,
			135,
			16,
			12,
			12,
			9,
			18,
			21,
			154,
			140,
			25,
			144,
			16,
			132,
			141,
			140,
			156,
			11,
			157,
			15,
			15,
			5,
			27,
			16,
			172,
			5,
			15,
			139,
			11,
			1,
			5,
			135,
			11,
			8,
			15,
			13,
			139,
			7,
			16,
			18,
			25,
			141,
			157,
			27,
			15,
			5,
			15,
			16,
			157,
			15,
			30,
			29,
			161,
			139,
			29,
			153,
			50,
			11,
			161,
			143,
			148,
			4,
			139,
			12,
			15,
			13,
			29,
			28,
			143,
			12,
			153,
			26,
			49,
			28,
			50,
			44,
			167,
			4,
			11,
			12,
			15,
			139,
			161,
			143,
			150,
			133,
			143,
			144,
			39,
			133,
			148,
			139,
			138,
			2,
			4,
			9,
			5,
			7,
			139,
			12,
			133,
			9,
			12,
			21,
			144,
			140,
			15,
			16,
			139,
			135,
			13,
			18,
			27,
			16,
			29,
			153,
			143,
			12,
			28,
			26,
			44,
			12,
			143,
			144,
			139,
			9,
			12,
			21,
			16,
			18,
			153,
			26,
			144,
			21,
			26,
			35,
			48,
			154,
			49,
			48,
			25,
			140,
			28,
			154,
			172,
			25,
			50,
			49,
			39,
			144,
			44,
			48,
			41,
			16,
			167,
			25,
			36,
			132,
			4,
			140,
			5,
			141,
			11,
			28,
			133,
			140,
			12,
			154,
			16,
			156,
			15,
			172,
			11,
			11,
			139,
			25,
			15,
			157,
			161,
			50,
			148,
			15,
			143,
			49,
			167,
			15,
			150,
			39,
			138,
			5,
			133,
			144,
			139,
			27,
			143,
			44,
			139,
			16,
			144,
			48,
			25,
			172,
			39,
			41,
			36,
			5,
			133,
			16,
			11,
			15,
			148,
			167,
			138,
			139,
			139,
			25,
			36,
			11,
			138,
			36,
			131,
			3,
			6,
			10,
			139,
			6,
			14,
			11,
			11,
			10,
			145,
			20,
			143,
			139,
			23,
			5,
			133,
			6,
			19,
			17,
			152,
			14,
			31,
			151,
			43,
			11,
			24,
			15,
			54,
			11,
			171,
			5,
			133,
			10,
			17,
			22,
			143,
			145,
			32,
			15,
			143,
			20,
			34,
			33,
			29,
			143,
			45,
			11,
			139,
			139,
			152,
			143,
			183,
			23,
			174,
			140,
			12,
			5,
			47,
			139,
			181,
			133,
			166,
			132,
			4,
			6,
			14,
			145,
			23,
			19,
			31,
			24,
			171,
			17,
			32,
			34,
			45,
			152,
			174,
			47,
			166,
			14,
			31,
			32,
			174,
			31,
			51,
			46,
			42,
			151,
			46,
			173,
			40,
			43,
			42,
			38,
			37,
			11,
			151,
			15,
			140,
			24,
			46,
			55,
			140,
			15,
			173,
			157,
			144,
			54,
			40,
			53,
			7,
			11,
			43,
			143,
			12,
			171,
			42,
			140,
			137,
			5,
			38,
			11,
			135,
			133,
			37,
			132,
			130,
			10,
			11,
			20,
			5,
			17,
			151,
			15,
			5,
			22,
			15,
			33,
			11,
			143,
			140,
			139,
			132,
			145,
			24,
			34,
			47,
			32,
			46,
			173,
			38,
			15,
			55,
			157,
			53,
			143,
			140,
			11,
			132,
			20,
			15,
			33,
			139,
			34,
			173,
			157,
			11,
			33,
			157,
			158,
			143,
			29,
			144,
			143,
			133,
			143,
			54,
			29,
			181,
			45,
			40,
			144,
			135,
			11,
			53,
			143,
			52,
			139,
			7,
			133,
			129,
			139,
			11,
			143,
			133,
			152,
			43,
			54,
			133,
			143,
			143,
			29,
			139,
			183,
			12,
			181,
			4,
			23,
			171,
			45,
			166,
			174,
			42,
			40,
			37,
			140,
			140,
			144,
			7,
			12,
			137,
			135,
			130,
			5,
			5,
			11,
			132,
			47,
			38,
			53,
			132,
			139,
			11,
			143,
			133,
			181,
			135,
			52,
			129,
			133,
			133,
			139,
			4,
			166,
			37,
			7,
			130,
			132,
			132,
			133,
			129,
			4,
			130,
			129,
			128
		};
		Transvoxel.transitionCellData = new Transvoxel.TransitionCellData[]
		{
			new Transvoxel.TransitionCellData(0, new byte[0]),
			new Transvoxel.TransitionCellData(66, new byte[]
			{
				0,
				1,
				3,
				1,
				2,
				3
			}),
			new Transvoxel.TransitionCellData(49, new byte[]
			{
				0,
				1,
				2
			}),
			new Transvoxel.TransitionCellData(66, new byte[]
			{
				0,
				1,
				2,
				0,
				2,
				3
			}),
			new Transvoxel.TransitionCellData(83, new byte[]
			{
				0,
				1,
				4,
				1,
				3,
				4,
				1,
				2,
				3
			}),
			new Transvoxel.TransitionCellData(100, new byte[]
			{
				0,
				1,
				5,
				1,
				2,
				5,
				2,
				4,
				5,
				2,
				3,
				4
			}),
			new Transvoxel.TransitionCellData(132, new byte[]
			{
				0,
				1,
				3,
				1,
				2,
				3,
				4,
				5,
				6,
				4,
				6,
				7
			}),
			new Transvoxel.TransitionCellData(115, new byte[]
			{
				0,
				1,
				3,
				1,
				2,
				3,
				4,
				5,
				6
			}),
			new Transvoxel.TransitionCellData(132, new byte[]
			{
				0,
				1,
				3,
				1,
				2,
				3,
				4,
				5,
				7,
				5,
				6,
				7
			}),
			new Transvoxel.TransitionCellData(98, new byte[]
			{
				0,
				1,
				2,
				3,
				4,
				5
			}),
			new Transvoxel.TransitionCellData(83, new byte[]
			{
				0,
				1,
				3,
				0,
				3,
				4,
				1,
				2,
				3
			}),
			new Transvoxel.TransitionCellData(117, new byte[]
			{
				0,
				1,
				6,
				1,
				2,
				6,
				2,
				5,
				6,
				2,
				3,
				5,
				3,
				4,
				5
			}),
			new Transvoxel.TransitionCellData(132, new byte[]
			{
				0,
				1,
				4,
				1,
				3,
				4,
				1,
				2,
				3,
				5,
				6,
				7
			}),
			new Transvoxel.TransitionCellData(149, new byte[]
			{
				0,
				1,
				4,
				1,
				3,
				4,
				1,
				2,
				3,
				5,
				6,
				8,
				6,
				7,
				8
			}),
			new Transvoxel.TransitionCellData(166, new byte[]
			{
				0,
				1,
				5,
				1,
				2,
				5,
				2,
				4,
				5,
				2,
				3,
				4,
				6,
				7,
				8,
				6,
				8,
				9
			}),
			new Transvoxel.TransitionCellData(134, new byte[]
			{
				0,
				1,
				7,
				1,
				2,
				7,
				2,
				3,
				7,
				3,
				6,
				7,
				3,
				4,
				6,
				4,
				5,
				6
			}),
			new Transvoxel.TransitionCellData(149, new byte[]
			{
				0,
				1,
				5,
				1,
				2,
				5,
				2,
				4,
				5,
				2,
				3,
				4,
				6,
				7,
				8
			}),
			new Transvoxel.TransitionCellData(149, new byte[]
			{
				0,
				1,
				3,
				1,
				2,
				3,
				4,
				5,
				7,
				4,
				7,
				8,
				5,
				6,
				7
			}),
			new Transvoxel.TransitionCellData(164, new byte[]
			{
				0,
				1,
				3,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8,
				9
			}),
			new Transvoxel.TransitionCellData(198, new byte[]
			{
				0,
				1,
				3,
				1,
				2,
				3,
				4,
				5,
				7,
				5,
				6,
				7,
				8,
				9,
				10,
				8,
				10,
				11
			}),
			new Transvoxel.TransitionCellData(100, new byte[]
			{
				0,
				1,
				3,
				1,
				2,
				3,
				0,
				3,
				4,
				0,
				4,
				5
			}),
			new Transvoxel.TransitionCellData(147, new byte[]
			{
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8
			}),
			new Transvoxel.TransitionCellData(100, new byte[]
			{
				0,
				1,
				4,
				0,
				4,
				5,
				1,
				3,
				4,
				1,
				2,
				3
			}),
			new Transvoxel.TransitionCellData(151, new byte[]
			{
				0,
				1,
				8,
				1,
				7,
				8,
				1,
				2,
				7,
				2,
				3,
				7,
				3,
				4,
				7,
				4,
				5,
				7,
				5,
				6,
				7
			}),
			new Transvoxel.TransitionCellData(183, new byte[]
			{
				0,
				1,
				6,
				1,
				2,
				6,
				2,
				5,
				6,
				2,
				3,
				5,
				3,
				4,
				5,
				7,
				8,
				10,
				8,
				9,
				10
			}),
			new Transvoxel.TransitionCellData(166, new byte[]
			{
				0,
				1,
				6,
				1,
				2,
				6,
				2,
				5,
				6,
				2,
				3,
				5,
				3,
				4,
				5,
				7,
				8,
				9
			}),
			new Transvoxel.TransitionCellData(181, new byte[]
			{
				0,
				1,
				4,
				1,
				3,
				4,
				1,
				2,
				3,
				5,
				6,
				7,
				8,
				9,
				10
			}),
			new Transvoxel.TransitionCellData(166, new byte[]
			{
				0,
				1,
				5,
				1,
				2,
				5,
				2,
				4,
				5,
				2,
				3,
				4,
				6,
				7,
				9,
				7,
				8,
				9
			}),
			new Transvoxel.TransitionCellData(166, new byte[]
			{
				0,
				1,
				4,
				1,
				3,
				4,
				1,
				2,
				3,
				5,
				6,
				9,
				6,
				8,
				9,
				6,
				7,
				8
			}),
			new Transvoxel.TransitionCellData(151, new byte[]
			{
				0,
				1,
				8,
				1,
				2,
				8,
				2,
				3,
				8,
				3,
				7,
				8,
				3,
				4,
				7,
				4,
				5,
				7,
				5,
				6,
				7
			}),
			new Transvoxel.TransitionCellData(134, new byte[]
			{
				0,
				1,
				7,
				1,
				6,
				7,
				1,
				2,
				6,
				2,
				5,
				6,
				2,
				4,
				5,
				2,
				3,
				4
			}),
			new Transvoxel.TransitionCellData(200, new byte[]
			{
				0,
				1,
				7,
				1,
				2,
				7,
				2,
				3,
				7,
				3,
				6,
				7,
				3,
				4,
				6,
				4,
				5,
				6,
				8,
				9,
				10,
				8,
				10,
				11
			}),
			new Transvoxel.TransitionCellData(183, new byte[]
			{
				0,
				1,
				5,
				1,
				2,
				5,
				2,
				4,
				5,
				2,
				3,
				4,
				6,
				9,
				10,
				6,
				7,
				9,
				7,
				8,
				9
			}),
			new Transvoxel.TransitionCellData(117, new byte[]
			{
				0,
				1,
				6,
				1,
				3,
				6,
				1,
				2,
				3,
				3,
				4,
				6,
				4,
				5,
				6
			}),
			new Transvoxel.TransitionCellData(166, new byte[]
			{
				0,
				1,
				3,
				1,
				2,
				3,
				4,
				5,
				9,
				5,
				8,
				9,
				5,
				6,
				8,
				6,
				7,
				8
			}),
			new Transvoxel.TransitionCellData(196, new byte[]
			{
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11
			}),
			new Transvoxel.TransitionCellData(134, new byte[]
			{
				1,
				2,
				4,
				2,
				3,
				4,
				0,
				1,
				7,
				1,
				4,
				7,
				4,
				6,
				7,
				4,
				5,
				6
			}),
			new Transvoxel.TransitionCellData(100, new byte[]
			{
				0,
				4,
				5,
				0,
				1,
				4,
				1,
				3,
				4,
				1,
				2,
				3
			}),
			new Transvoxel.TransitionCellData(134, new byte[]
			{
				0,
				1,
				4,
				1,
				3,
				4,
				1,
				2,
				3,
				0,
				4,
				7,
				4,
				6,
				7,
				4,
				5,
				6
			}),
			new Transvoxel.TransitionCellData(151, new byte[]
			{
				1,
				2,
				3,
				1,
				3,
				4,
				1,
				4,
				5,
				0,
				1,
				8,
				1,
				5,
				8,
				5,
				7,
				8,
				5,
				6,
				7
			}),
			new Transvoxel.TransitionCellData(166, new byte[]
			{
				0,
				1,
				3,
				1,
				2,
				3,
				4,
				5,
				9,
				5,
				8,
				9,
				5,
				6,
				8,
				6,
				7,
				8
			}),
			new Transvoxel.TransitionCellData(200, new byte[]
			{
				0,
				1,
				5,
				1,
				2,
				5,
				2,
				4,
				5,
				2,
				3,
				4,
				6,
				7,
				11,
				7,
				10,
				11,
				7,
				8,
				10,
				8,
				9,
				10
			}),
			new Transvoxel.TransitionCellData(151, new byte[]
			{
				0,
				1,
				8,
				1,
				2,
				8,
				2,
				7,
				8,
				2,
				3,
				7,
				3,
				6,
				7,
				3,
				4,
				6,
				4,
				5,
				6
			}),
			new Transvoxel.TransitionCellData(151, new byte[]
			{
				0,
				1,
				4,
				1,
				3,
				4,
				1,
				2,
				3,
				0,
				4,
				8,
				4,
				7,
				8,
				4,
				5,
				7,
				5,
				6,
				7
			}),
			new Transvoxel.TransitionCellData(183, new byte[]
			{
				0,
				1,
				5,
				1,
				2,
				5,
				2,
				4,
				5,
				2,
				3,
				4,
				6,
				7,
				10,
				7,
				9,
				10,
				7,
				8,
				9
			}),
			new Transvoxel.TransitionCellData(168, new byte[]
			{
				0,
				1,
				9,
				1,
				2,
				9,
				2,
				8,
				9,
				2,
				3,
				8,
				3,
				7,
				8,
				3,
				4,
				7,
				4,
				6,
				7,
				4,
				5,
				6
			}),
			new Transvoxel.TransitionCellData(185, new byte[]
			{
				0,
				1,
				7,
				1,
				6,
				7,
				1,
				2,
				6,
				2,
				5,
				6,
				2,
				3,
				5,
				3,
				4,
				5,
				0,
				7,
				10,
				7,
				9,
				10,
				7,
				8,
				9
			}),
			new Transvoxel.TransitionCellData(166, new byte[]
			{
				0,
				1,
				5,
				1,
				4,
				5,
				1,
				2,
				4,
				2,
				3,
				4,
				6,
				7,
				9,
				7,
				8,
				9
			}),
			new Transvoxel.TransitionCellData(198, new byte[]
			{
				0,
				1,
				5,
				1,
				2,
				5,
				2,
				4,
				5,
				2,
				3,
				4,
				6,
				7,
				8,
				9,
				10,
				11
			}),
			new Transvoxel.TransitionCellData(183, new byte[]
			{
				0,
				1,
				7,
				1,
				2,
				7,
				2,
				3,
				7,
				3,
				6,
				7,
				3,
				4,
				6,
				4,
				5,
				6,
				8,
				9,
				10
			}),
			new Transvoxel.TransitionCellData(168, new byte[]
			{
				1,
				2,
				3,
				1,
				3,
				4,
				1,
				4,
				6,
				4,
				5,
				6,
				0,
				1,
				9,
				1,
				6,
				9,
				6,
				8,
				9,
				6,
				7,
				8
			}),
			new Transvoxel.TransitionCellData(204, new byte[]
			{
				0,
				1,
				9,
				1,
				8,
				9,
				1,
				2,
				8,
				2,
				11,
				8,
				2,
				3,
				11,
				3,
				4,
				11,
				4,
				5,
				11,
				5,
				10,
				11,
				5,
				6,
				10,
				6,
				9,
				10,
				6,
				7,
				9,
				7,
				0,
				9
			}),
			new Transvoxel.TransitionCellData(134, new byte[]
			{
				0,
				1,
				2,
				0,
				2,
				3,
				0,
				6,
				7,
				0,
				3,
				6,
				1,
				4,
				5,
				1,
				5,
				2
			}),
			new Transvoxel.TransitionCellData(151, new byte[]
			{
				0,
				1,
				4,
				1,
				3,
				4,
				1,
				2,
				3,
				2,
				5,
				6,
				2,
				6,
				3,
				0,
				7,
				8,
				0,
				4,
				7
			}),
			new Transvoxel.TransitionCellData(168, new byte[]
			{
				0,
				1,
				5,
				1,
				4,
				5,
				1,
				2,
				4,
				2,
				3,
				4,
				3,
				6,
				7,
				3,
				7,
				4,
				0,
				8,
				9,
				0,
				5,
				8
			}),
			new Transvoxel.TransitionCellData(168, new byte[]
			{
				0,
				1,
				5,
				1,
				4,
				5,
				1,
				2,
				4,
				2,
				3,
				4,
				2,
				6,
				3,
				3,
				6,
				7,
				0,
				8,
				9,
				0,
				5,
				8
			})
		};
		Transvoxel.transitionCornerData = new byte[]
		{
			48,
			33,
			32,
			18,
			64,
			130,
			16,
			129,
			128,
			55,
			39,
			23,
			135
		};
		Transvoxel.transitionVertexData = new Transvoxel.TransitionVertexData();
	}

	// Token: 0x040024EF RID: 9455
	public const int kTerrainLogDimension = 4;

	// Token: 0x040024F0 RID: 9456
	public const int kTerrainDimension = 16;

	// Token: 0x040024F1 RID: 9457
	public const int kMaxTerrainTriangleCount = 8192;

	// Token: 0x040024F2 RID: 9458
	public const int kMaxTerrainVertexCount = 8192;

	// Token: 0x040024F3 RID: 9459
	public const int kVoxelFractionSize = 8;

	// Token: 0x040024F4 RID: 9460
	public const int kVoxelFixedUnit = 256;

	// Token: 0x040024F5 RID: 9461
	public const int kMaxTerrainChannelCount = 2;

	// Token: 0x040024F6 RID: 9462
	public const int kMaxTerrainMaterialCount = 255;

	// Token: 0x040024F7 RID: 9463
	public const int kTerrainMaterialUsageTableSize = 256;

	// Token: 0x040024F8 RID: 9464
	public const int kDeadTerrainMaterialIndex = 255;

	// Token: 0x040024F9 RID: 9465
	public const float one_over_127 = 0.007874016f;

	// Token: 0x040024FA RID: 9466
	public const float one_over_256 = 0.00390625f;

	// Token: 0x040024FB RID: 9467
	public static readonly byte[] regularCellClass = new byte[]
	{
		0,
		1,
		1,
		3,
		1,
		3,
		2,
		4,
		1,
		2,
		3,
		4,
		3,
		4,
		4,
		3,
		1,
		3,
		2,
		4,
		2,
		4,
		6,
		12,
		2,
		5,
		5,
		11,
		5,
		10,
		7,
		4,
		1,
		2,
		3,
		4,
		2,
		5,
		5,
		10,
		2,
		6,
		4,
		12,
		5,
		7,
		11,
		4,
		3,
		4,
		4,
		3,
		5,
		11,
		7,
		4,
		5,
		7,
		10,
		4,
		8,
		14,
		14,
		3,
		1,
		2,
		2,
		5,
		3,
		4,
		5,
		11,
		2,
		6,
		5,
		7,
		4,
		12,
		10,
		4,
		3,
		4,
		5,
		10,
		4,
		3,
		7,
		4,
		5,
		7,
		8,
		14,
		11,
		4,
		14,
		3,
		2,
		6,
		5,
		7,
		5,
		7,
		8,
		14,
		6,
		9,
		7,
		15,
		7,
		15,
		14,
		13,
		4,
		12,
		11,
		4,
		10,
		4,
		14,
		3,
		7,
		15,
		14,
		13,
		14,
		13,
		2,
		1,
		1,
		2,
		2,
		5,
		2,
		5,
		6,
		7,
		3,
		5,
		4,
		10,
		4,
		11,
		12,
		4,
		2,
		5,
		6,
		7,
		6,
		7,
		9,
		15,
		5,
		8,
		7,
		14,
		7,
		14,
		15,
		13,
		3,
		5,
		4,
		11,
		5,
		8,
		7,
		14,
		4,
		7,
		3,
		4,
		10,
		14,
		4,
		3,
		4,
		10,
		12,
		4,
		7,
		14,
		15,
		13,
		11,
		14,
		4,
		3,
		14,
		2,
		13,
		1,
		3,
		5,
		5,
		8,
		4,
		10,
		7,
		14,
		4,
		7,
		11,
		14,
		3,
		4,
		4,
		3,
		4,
		11,
		7,
		14,
		12,
		4,
		15,
		13,
		10,
		14,
		14,
		2,
		4,
		3,
		13,
		1,
		4,
		7,
		10,
		14,
		11,
		14,
		14,
		2,
		12,
		15,
		4,
		13,
		4,
		13,
		3,
		1,
		3,
		4,
		4,
		3,
		4,
		3,
		13,
		1,
		4,
		13,
		3,
		1,
		3,
		1,
		1,
		0
	};

	// Token: 0x040024FC RID: 9468
	public static readonly Transvoxel.RegularCellData[,] regularCellData;

	// Token: 0x040024FD RID: 9469
	public static readonly Transvoxel.InternalEdgeData[,] regularInternalEdgeData;

	// Token: 0x040024FE RID: 9470
	public static readonly Transvoxel.RegularVertexData regularVertexData;

	// Token: 0x040024FF RID: 9471
	public static readonly byte[] transitionCellClass;

	// Token: 0x04002500 RID: 9472
	public static readonly Transvoxel.TransitionCellData[] transitionCellData;

	// Token: 0x04002501 RID: 9473
	public static readonly byte[] transitionCornerData;

	// Token: 0x04002502 RID: 9474
	public static Transvoxel.TransitionVertexData transitionVertexData;

	// Token: 0x020005AD RID: 1453
	public struct BuildVertex
	{
		// Token: 0x04002503 RID: 9475
		public Vector3i position0;

		// Token: 0x04002504 RID: 9476
		public Vector3 normal;

		// Token: 0x04002505 RID: 9477
		public Color border;

		// Token: 0x04002506 RID: 9478
		public Color color;

		// Token: 0x04002507 RID: 9479
		public Vector2 uv;

		// Token: 0x04002508 RID: 9480
		public Vector2 uv2;

		// Token: 0x04002509 RID: 9481
		public Vector2 uv3;

		// Token: 0x0400250A RID: 9482
		public Vector2 uv4;

		// Token: 0x0400250B RID: 9483
		public byte material;

		// Token: 0x0400250C RID: 9484
		public byte statusFlags;

		// Token: 0x0400250D RID: 9485
		public int remapIndex;

		// Token: 0x0400250E RID: 9486
		public int texture;

		// Token: 0x0400250F RID: 9487
		public bool bTopSoil;
	}

	// Token: 0x020005AE RID: 1454
	public struct BuildTriangle
	{
		// Token: 0x04002510 RID: 9488
		public int index0;

		// Token: 0x04002511 RID: 9489
		public int index1;

		// Token: 0x04002512 RID: 9490
		public int index2;

		// Token: 0x04002513 RID: 9491
		public bool inclusionFlag;

		// Token: 0x04002514 RID: 9492
		public int submeshIdx;
	}

	// Token: 0x020005AF RID: 1455
	public class RegularCellData
	{
		// Token: 0x06002EF5 RID: 12021 RVA: 0x0013D3E4 File Offset: 0x0013B5E4
		public RegularCellData(byte _geometryCounts, byte[] _vertexIndex)
		{
			this.geometryCounts = _geometryCounts;
			this.vertexIndex = _vertexIndex;
		}

		// Token: 0x06002EF6 RID: 12022 RVA: 0x0013D3FA File Offset: 0x0013B5FA
		public int GetVertexCount()
		{
			return this.geometryCounts >> 4;
		}

		// Token: 0x06002EF7 RID: 12023 RVA: 0x0013D404 File Offset: 0x0013B604
		public int GetTriangleCount()
		{
			return (int)(this.geometryCounts & 15);
		}

		// Token: 0x04002515 RID: 9493
		public byte geometryCounts;

		// Token: 0x04002516 RID: 9494
		public byte[] vertexIndex;
	}

	// Token: 0x020005B0 RID: 1456
	public class TransitionCellData
	{
		// Token: 0x06002EF8 RID: 12024 RVA: 0x0013D40F File Offset: 0x0013B60F
		public TransitionCellData(int _geometryCounts, byte[] _vertexIndex)
		{
			this.geometryCounts = _geometryCounts;
			this.vertexIndex = _vertexIndex;
		}

		// Token: 0x06002EF9 RID: 12025 RVA: 0x0013D425 File Offset: 0x0013B625
		public int GetVertexCount()
		{
			return this.geometryCounts >> 4;
		}

		// Token: 0x06002EFA RID: 12026 RVA: 0x0013D42F File Offset: 0x0013B62F
		public int GetTriangleCount()
		{
			return this.geometryCounts & 15;
		}

		// Token: 0x04002517 RID: 9495
		public int geometryCounts;

		// Token: 0x04002518 RID: 9496
		public byte[] vertexIndex;
	}

	// Token: 0x020005B1 RID: 1457
	public class InternalEdgeData
	{
		// Token: 0x06002EFB RID: 12027 RVA: 0x0013D43A File Offset: 0x0013B63A
		public InternalEdgeData(byte _edgeCount, byte[,] _vertexIndex)
		{
			this.edgeCount = _edgeCount;
			this.vertexIndex = _vertexIndex;
		}

		// Token: 0x04002519 RID: 9497
		public byte edgeCount;

		// Token: 0x0400251A RID: 9498
		public byte[,] vertexIndex;
	}

	// Token: 0x020005B2 RID: 1458
	public class RegularVertexData
	{
		// Token: 0x17000495 RID: 1173
		public Transvoxel.RegularVertexData.Row this[int _index]
		{
			get
			{
				return this.rows[_index];
			}
		}

		// Token: 0x06002EFD RID: 12029 RVA: 0x0013D45C File Offset: 0x0013B65C
		public RegularVertexData()
		{
			this.rows = new Transvoxel.RegularVertexData.Row[]
			{
				new Transvoxel.RegularVertexData.Row(new ushort[0]),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					13060,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					25089,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					16659,
					20738,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					4902,
					13060,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					33591,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					13060,
					16931,
					16659,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					33591,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					13060,
					8981,
					33591,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16659,
					33591,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					33591,
					4902,
					13060,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					33591,
					4902,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					8981,
					33591,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					16659,
					13060,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					16659,
					20738,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16931,
					4902,
					13060,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4422,
					8773,
					25089,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4422,
					8773,
					25089,
					8981,
					16659,
					20738,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					4902,
					4422,
					8773,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					16659,
					33591,
					13060,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					4422,
					8773,
					16931,
					16659,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					25089,
					8981,
					33591,
					13060,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					33591,
					8981,
					8773,
					4422,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16659,
					33591,
					4902,
					13060,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					33591,
					4902,
					4422,
					8773,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					33591,
					4902,
					20738,
					13060,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8773,
					8981,
					33591,
					4902,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					13060,
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					25089,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8773,
					33111,
					16659,
					20738,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16931,
					4902,
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16931,
					4902,
					13060,
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8773,
					33111,
					16659,
					20738,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					4902,
					13060,
					8773,
					33111,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					16659,
					33591,
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					13060,
					16931,
					16659,
					33591,
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					16931,
					25089,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					13060,
					8773,
					33111,
					33591,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16659,
					33591,
					4902,
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					33591,
					4902,
					13060,
					25089,
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					4902,
					33591,
					33111,
					8773,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33111,
					33591,
					4902,
					13060,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					13060,
					4422,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					4422,
					33111,
					8981
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4422,
					33111,
					16659,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					20738,
					4422,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					13060,
					4422,
					33111,
					20738,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					16931,
					25089,
					8981,
					33111,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4422,
					33111,
					16659,
					25089,
					20738,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					4422,
					33111,
					16659,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					13060,
					4422,
					33111,
					16931,
					16659,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					4422,
					33111,
					8981,
					16931,
					16659,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4422,
					33111,
					33591,
					16931,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					20738,
					4422,
					33111,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					13060,
					4422,
					33111,
					20738,
					16659,
					33591,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16659,
					33591,
					4902,
					4422,
					33111,
					8981
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					13060,
					4422,
					33111,
					33591,
					4902,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					4422,
					33111,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					13060,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					16659,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					13060,
					8981,
					16659,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16931,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					25089,
					16931,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16931,
					33383,
					4422,
					25089,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4422,
					33383,
					16931,
					16659,
					8981,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					33591,
					16931,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					13060,
					16931,
					16659,
					33591,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					33591,
					16931,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					13060,
					8981,
					33591,
					16931,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33383,
					4422,
					20738,
					16659,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16659,
					33591,
					33383,
					4422,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					33591,
					33383,
					4422,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4422,
					13060,
					8981,
					33591,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4902,
					33383,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					33383,
					8773,
					25089,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4902,
					33383,
					8773,
					25089,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					33383,
					8773,
					8981,
					16659,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16931,
					33383,
					8773,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16931,
					33383,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16931,
					33383,
					8773,
					13060,
					25089,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					16931,
					33383,
					8773,
					8981
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4902,
					33383,
					8773,
					16931,
					16659,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					33383,
					8773,
					25089,
					20738,
					16931,
					16659,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4902,
					33383,
					8773,
					16931,
					25089,
					8981,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					4902,
					33383,
					8773,
					8981,
					33591,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					8773,
					33383,
					33591,
					16659,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					33383,
					8773,
					25089,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					25089,
					8981,
					33591,
					33383,
					8773,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					33591,
					33383,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					8773,
					33111,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					13060,
					8981,
					8773,
					33111,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8773,
					33111,
					16659,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8773,
					33111,
					16659,
					20738,
					13060,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					33383,
					4422,
					20738,
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					25089,
					16931,
					33383,
					4422,
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					33383,
					4422,
					20738,
					25089,
					8773,
					33111,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					8773,
					33111,
					16659,
					16931,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					16659,
					33591,
					8981,
					8773,
					33111,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					13060,
					16931,
					16659,
					33591,
					8981,
					8773,
					33111,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					16931,
					25089,
					8773,
					33111,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					20738,
					13060,
					8773,
					33111,
					33591,
					4902,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33383,
					4422,
					20738,
					16659,
					33591,
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16659,
					33591,
					33383,
					4422,
					13060,
					8981,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					33383,
					4422,
					20738,
					25089,
					8773,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					8773,
					33111,
					33591,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33111,
					8981,
					13060,
					4902,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33383,
					33111,
					8981,
					25089,
					20738,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33383,
					4902,
					13060,
					25089,
					16659,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33383,
					33111,
					16659,
					20738,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16931,
					33383,
					33111,
					8981,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					25089,
					16931,
					33383,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					20738,
					16931,
					33383,
					33111,
					16659,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					16931,
					33383,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33111,
					8981,
					13060,
					4902,
					33383,
					16931,
					16659,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33111,
					8981,
					25089,
					20738,
					4902,
					33383,
					16931,
					16659,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33111,
					33591,
					16931,
					25089,
					13060,
					4902,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					4902,
					33383,
					33111,
					33591,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33383,
					33111,
					8981,
					13060,
					20738,
					16659,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16659,
					33591,
					33383,
					33111,
					8981
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					13060,
					20738,
					33591,
					33383,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					33383,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					13060,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					16659,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					13060,
					8981,
					16659,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16931,
					4902,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16931,
					4902,
					13060,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					16659,
					20738,
					16931,
					4902,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					4902,
					13060,
					8981,
					16659,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					33111,
					33383,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					16659,
					33111,
					33383,
					25089,
					20738,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33111,
					33383,
					16931,
					25089,
					8981
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					8981,
					33111,
					33383,
					16931,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					20738,
					16659,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33111,
					16659,
					25089,
					13060,
					4902,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					20738,
					25089,
					8981,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33383,
					4902,
					13060,
					8981,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4422,
					8773,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					4422,
					8773,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					16659,
					13060,
					4422,
					8773,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					16659,
					20738,
					4422,
					8773,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16931,
					4902,
					13060,
					4422,
					8773,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4422,
					8773,
					25089,
					16931,
					4902,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8981,
					16659,
					20738,
					16931,
					4902,
					13060,
					4422,
					8773,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					16931,
					4902,
					4422,
					8773,
					8981,
					33591,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					16659,
					33111,
					33383,
					13060,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					20738,
					4422,
					8773,
					16931,
					16659,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33111,
					33383,
					16931,
					25089,
					8981,
					13060,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					33111,
					33383,
					16931,
					20738,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					20738,
					16659,
					33111,
					33383,
					13060,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					4422,
					8773,
					25089,
					16659,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					25089,
					8981,
					33111,
					33383,
					4902,
					13060,
					4422,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					4422,
					8773,
					8981,
					33111,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					8773,
					33383,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					8773,
					33383,
					33591,
					25089,
					20738,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					25089,
					8773,
					33383,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16659,
					33591,
					33383,
					8773,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					8773,
					33383,
					33591,
					20738,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16931,
					4902,
					13060,
					33591,
					8981,
					8773,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					25089,
					8773,
					33383,
					33591,
					20738,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					16931,
					4902,
					13060,
					8773,
					33383,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					8773,
					33383,
					16931,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					8773,
					33383,
					16931,
					16659,
					25089,
					20738,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8773,
					33383,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					8773,
					33383,
					16931,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16659,
					8981,
					8773,
					33383,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					8981,
					8773,
					33383,
					4902,
					13060,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					25089,
					8773,
					33383,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					8773,
					33383,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33383,
					33591,
					8981,
					13060,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					4422,
					33383,
					33591,
					8981,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4422,
					33383,
					33591,
					16659,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					16659,
					20738,
					4422,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33383,
					33591,
					8981,
					13060,
					4422,
					20738,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4422,
					33383,
					33591,
					8981,
					25089,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33383,
					33591,
					16659,
					25089,
					13060,
					4422,
					20738,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					16931,
					4902,
					4422,
					33383,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					8981,
					16659,
					16931,
					33383,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					25089,
					20738,
					4422,
					33383,
					16931,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4422,
					33383,
					16931,
					25089,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					4422,
					33383,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33383,
					4902,
					20738,
					16659,
					8981,
					13060,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16659,
					8981,
					4902,
					4422,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					13060,
					4422,
					33383,
					4902,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					4422,
					33383
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					33591,
					33111,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					33111,
					4422,
					4902,
					25089,
					20738,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					33111,
					4422,
					4902,
					25089,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					20738,
					13060,
					8981,
					4902,
					33591,
					33111,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					33111,
					4422,
					20738,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16931,
					33591,
					33111,
					4422,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					33111,
					4422,
					20738,
					16931,
					25089,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					33591,
					33111,
					4422,
					13060,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					16659,
					33111,
					4422,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					16659,
					33111,
					4422,
					4902,
					25089,
					20738,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4422,
					33111,
					8981,
					25089,
					16931,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					20738,
					13060,
					8981,
					33111,
					4422,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					33111,
					4422,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16659,
					33111,
					4422,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					33111,
					4422,
					20738,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					33111,
					4422,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8773,
					13060,
					4902,
					33591,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8773,
					33111,
					33591,
					4902,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8773,
					13060,
					4902,
					33591,
					33111,
					25089,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8773,
					8981,
					16659,
					20738,
					4902,
					33591,
					33111
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					33591,
					33111,
					8773,
					13060,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33111,
					8773,
					25089,
					16931,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8773,
					13060,
					20738,
					16931,
					33591,
					33111,
					16659,
					25089,
					8981
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					33591,
					33111,
					8773,
					8981,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					33111,
					8773,
					13060,
					4902,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					16931,
					16659,
					33111,
					8773,
					25089,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33111,
					8773,
					13060,
					4902,
					16931,
					25089,
					8981
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					4902,
					16931,
					8981,
					33111,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					20738,
					16659,
					33111,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					33111,
					8773,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					25089,
					8981,
					33111,
					8773,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8981,
					33111,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4422,
					4902,
					33591,
					8981,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4422,
					4902,
					33591,
					8981,
					8773,
					25089,
					20738,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8773,
					4422,
					4902,
					33591,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8773,
					4422,
					4902,
					33591,
					16659,
					20738,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					4422,
					8773,
					8981,
					33591,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4422,
					13060,
					25089,
					16931,
					33591,
					8981,
					8773
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					33591,
					16659,
					25089,
					8773,
					4422,
					20738,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					33591,
					16659,
					13060,
					8773,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					8981,
					8773,
					4422,
					4902,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4422,
					4902,
					16931,
					16659,
					8981,
					8773,
					25089,
					20738,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					16931,
					25089,
					8773,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					20738,
					13060,
					8773,
					4422,
					4902
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					8773,
					4422,
					20738,
					16659,
					8981
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					8981,
					8773,
					4422,
					13060,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					8773,
					4422,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					8773,
					4422
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4902,
					33591,
					8981
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					4902,
					33591,
					8981,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					13060,
					4902,
					33591,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					4902,
					33591,
					16659
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16931,
					33591,
					8981,
					13060,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16931,
					33591,
					8981
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					20738,
					16931,
					33591,
					16659,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					16931,
					33591
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					16659,
					8981,
					13060,
					4902,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					4902,
					16931,
					16659,
					8981,
					25089,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					13060,
					4902,
					16931,
					25089
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					4902,
					16931
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					20738,
					16659,
					8981,
					13060
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					16659,
					8981
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[]
				{
					25089,
					13060,
					20738
				}),
				new Transvoxel.RegularVertexData.Row(new ushort[0])
			};
		}

		// Token: 0x0400251B RID: 9499
		[PublicizedFrom(EAccessModifier.Private)]
		public Transvoxel.RegularVertexData.Row[] rows;

		// Token: 0x020005B3 RID: 1459
		public class Row
		{
			// Token: 0x06002EFE RID: 12030 RVA: 0x0013EFF2 File Offset: 0x0013D1F2
			public Row(ushort[] _data)
			{
				this.data = _data;
			}

			// Token: 0x0400251C RID: 9500
			public ushort[] data;
		}
	}

	// Token: 0x020005B4 RID: 1460
	public class TransitionVertexData
	{
		// Token: 0x17000496 RID: 1174
		public Transvoxel.TransitionVertexData.Row this[int _index]
		{
			get
			{
				return this.rows[_index];
			}
		}

		// Token: 0x06002F00 RID: 12032 RVA: 0x0013F00C File Offset: 0x0013D20C
		public TransitionVertexData()
		{
			this.rows = new Transvoxel.TransitionVertexData.Row[]
			{
				new Transvoxel.TransitionVertexData.Row(new ushort[0]),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					10394,
					6555,
					34392,
					34085,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					34392,
					17477,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					9234,
					10394,
					6555,
					34392,
					34085,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					8961,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					6555,
					10394,
					33912,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					35244,
					35004,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					9234,
					10394,
					6555,
					34392,
					33912,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					8961,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					34085,
					34392,
					33912,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					17684,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					34085,
					35244,
					35004,
					8961,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					34085,
					35244,
					35004,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					34085,
					35244,
					35004,
					9234,
					17684,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					9234,
					17477,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					33912,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					17477,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					6555,
					10394,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					9234,
					17684,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					9234,
					10394,
					6555,
					33639,
					33912,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					35244,
					10394,
					33639,
					33912,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					6555,
					35244,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					8961,
					10394,
					35244,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					5379,
					6555,
					35244,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					33639,
					17991,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					6555,
					10394,
					33912,
					33639,
					17991,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					33639,
					17991,
					34085,
					34392,
					17477,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					9234,
					10394,
					6555,
					34392,
					34085,
					17477,
					33639,
					33912,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					34392,
					35244,
					10394,
					33639,
					33912,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					8961,
					5379,
					6555,
					35244,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					34392,
					35244,
					10394,
					33639,
					33912,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					5379,
					6555,
					35244,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					33639,
					35004,
					35244,
					5379,
					8961,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					33639,
					35004,
					35244,
					9234,
					8961,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					9234,
					10394,
					6555,
					34392,
					17991,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					34392,
					34085,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					9234,
					34085,
					34392,
					17991,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					34392,
					34085,
					17684,
					8961,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					34392,
					34085,
					17684,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					34085,
					35244,
					35004,
					8961,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					34085,
					35244,
					35004,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					33639,
					35004,
					35244,
					5379,
					17684,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					9234,
					8961,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					17991,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					17684,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					6555,
					35004,
					9234,
					8961,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					5686,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					10394,
					35244,
					5686,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					5686,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					8961,
					10394,
					35244,
					5686,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					5379,
					17684,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					35004,
					6555,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					35004,
					10394,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					35004,
					6555,
					34085,
					34392,
					17477,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					5686,
					33639,
					35004,
					10394,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					34392,
					35244,
					10394,
					33639,
					5686,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					5379,
					8961,
					9234,
					17477,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					8961,
					10394,
					35244,
					5686,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					5379,
					5686,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					33912,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					35244,
					6555,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					33639,
					5686,
					5379,
					17684,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					34085,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					34085,
					9234,
					8961,
					5379
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					34085,
					17684,
					8961,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					34085,
					34392,
					33912,
					33639,
					5686
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					33912,
					33639,
					5686,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					33912,
					17477,
					34085,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					33912,
					33639,
					5686,
					6555,
					35244,
					9234,
					8961,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					5686,
					33639,
					33912,
					17477,
					34085,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					17477,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					33912,
					33639,
					5686,
					5379,
					8961
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					33912,
					33639,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					5379,
					17684,
					17477,
					33912
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					5686,
					5379,
					8961,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					33912,
					35004,
					6555,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					5686,
					17991,
					33912,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					33912,
					35004,
					6555,
					34085,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					5686,
					17991,
					33912,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					8961,
					10394,
					35244,
					5686,
					17991,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					5686,
					5379,
					17684,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					33912,
					35004,
					6555,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					5686,
					5379,
					8961,
					10394,
					35004,
					34392,
					34085,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					33912,
					35004,
					6555,
					34085,
					34392,
					17477,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					5686,
					17991,
					33912,
					35004,
					10394,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					33912,
					35004,
					6555,
					34392,
					17477,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					8961,
					5379,
					5686,
					17991,
					33912,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					8961,
					10394,
					35244,
					5686,
					17991,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					5686,
					5379,
					17684,
					17477,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					17991,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					34392,
					35244,
					6555,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					5686,
					17991,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					34392,
					17991,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					5686,
					5379,
					8961,
					9234,
					34085
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					34085,
					34392,
					17991,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					34085,
					34392,
					17991,
					5686
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					5686,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					5686,
					5379,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					5686,
					6555,
					35244,
					9234,
					8961,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					5686,
					17991,
					17477,
					34085,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					17991,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					9234,
					17477,
					17991,
					5686
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					17991,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					17477,
					17991,
					5686
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					9234,
					17684,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					10394,
					35244,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					8961,
					9234,
					34085,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					8961,
					10394,
					35244,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					34085,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					5379,
					17204,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					5686,
					6555,
					10394,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					34392,
					17477,
					8961,
					9234,
					17684,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					9234,
					10394,
					6555,
					34392,
					34085,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					34392,
					35244,
					10394,
					5379,
					5686,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					8961,
					17204,
					5686,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					8961,
					10394,
					35244,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					17477,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					35004,
					35244,
					5379,
					5686,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					5686,
					6555,
					10394,
					33912,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					35244,
					35004,
					8961,
					9234,
					17684,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					5686,
					6555,
					10394,
					33912,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					10394,
					35004,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					8961,
					17204,
					5686,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					34085,
					34392,
					33912,
					35004,
					10394,
					5379,
					5686,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					34085,
					34392,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					34085,
					35244,
					35004,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					5686,
					6555,
					10394,
					33912,
					17477,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					34085,
					35244,
					35004,
					8961,
					9234,
					17684,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					5686,
					6555,
					10394,
					33912,
					17477,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					9234,
					10394,
					35004,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					8961,
					9234,
					17477,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					33912,
					35004,
					10394,
					5379,
					5686,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					17477,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					5379,
					17204,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					5686,
					6555,
					10394,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					9234,
					17684,
					5686,
					5379,
					17204,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					5686,
					6555,
					10394,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					10394,
					35244,
					5686,
					5379,
					17204,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					8961,
					9234,
					34085,
					35244,
					6555,
					33639,
					33912,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					8961,
					10394,
					35244,
					5686,
					5379,
					17204,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					34085,
					35244,
					6555,
					33639,
					33912,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					5379,
					17204,
					33912,
					33639,
					17991,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					5686,
					6555,
					10394,
					33912,
					33639,
					17991,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					9234,
					17684,
					5686,
					5379,
					17204,
					33912,
					33639,
					17991,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					5686,
					6555,
					10394,
					33912,
					33639,
					17991,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					34392,
					35244,
					10394,
					33639,
					33912,
					17991,
					5379,
					5686,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					8961,
					17204,
					5686,
					6555,
					35244,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					8961,
					10394,
					35244,
					5686,
					5379,
					17204,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					17204,
					5686,
					6555,
					35244,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					33639,
					35004,
					35244,
					5379,
					5686,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					5686,
					6555,
					10394,
					33639,
					17991,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					33639,
					35004,
					35244,
					5379,
					5686,
					17204,
					9234,
					8961,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					9234,
					10394,
					6555,
					34392,
					17991,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					34392,
					34085,
					9234,
					10394,
					35004,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					34392,
					34085,
					9234,
					8961,
					17204,
					5686,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					34392,
					34085,
					17684,
					8961,
					10394,
					35004,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					34392,
					34085,
					17684,
					17204,
					5686,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					33639,
					35004,
					35244,
					5379,
					5686,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					34085,
					35244,
					35004,
					8961,
					17204,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					34085,
					35244,
					35004,
					8961,
					9234,
					17684,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					34085,
					35244,
					35004,
					9234,
					17684,
					17204,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					9234,
					10394,
					35004,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					8961,
					9234,
					17477,
					17991,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					17684,
					8961,
					10394,
					35004,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					17684,
					17204,
					5686,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					5379,
					6555,
					35004,
					9234,
					8961,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					5379,
					6555,
					35004,
					9234,
					34085,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					17204,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					8961,
					10394,
					35244,
					5379,
					17204,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					17684,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					5379,
					6555,
					35004,
					34392,
					34085,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					33639,
					35004,
					10394,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					5379,
					6555,
					35004,
					9234,
					8961,
					17684,
					34392,
					34085,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					33639,
					35004,
					10394,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					34392,
					35244,
					10394,
					33639,
					17204,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					8961,
					17204,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					34392,
					35244,
					10394,
					33639,
					17204,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					17204,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					33639,
					33912,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					33639,
					17204,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					33639,
					33912,
					34392,
					35244,
					6555,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					33639,
					33912,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					34392,
					33912,
					33639,
					17204,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					8961,
					9234,
					34085,
					34392,
					33912
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					33639,
					33912,
					34392,
					34085,
					17684,
					8961,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					33639,
					17204,
					17684,
					34085
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					33639,
					33912,
					17477,
					34085,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					33912,
					33639,
					17204,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					33639,
					33912,
					17477,
					34085,
					35244,
					6555,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					33912,
					33639,
					17204,
					17684,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					33639,
					33912,
					17477,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					8961,
					9234,
					17477,
					33912
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					33639,
					33912,
					17477,
					17684,
					8961,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					17684,
					17477,
					33912
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					8961,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					5379,
					6555,
					35004,
					9234,
					8961,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					17684,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					33912,
					35004,
					6555,
					34085,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					8961,
					9234,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					33912,
					35004,
					6555,
					34085,
					17684,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					17684,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					33912,
					35004,
					6555,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					8961,
					10394,
					35004,
					34392,
					34085,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					33912,
					35004,
					6555,
					34085,
					34392,
					17477,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					17991,
					33912,
					35004,
					10394,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					5379,
					6555,
					35004,
					9234,
					17477,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					8961,
					17204,
					17991,
					33912,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					8961,
					10394,
					35244,
					5379,
					17204,
					17991,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					17204,
					17991,
					33912,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					17204,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					34392,
					35244,
					6555,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					17991,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					34392,
					34085,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					34392,
					17991,
					17204,
					8961
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					34085,
					34392,
					17991,
					17204,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					17204,
					17684,
					34085
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					17204,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					17204,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					17477,
					34085,
					35244,
					6555,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					17991,
					17477,
					34085,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					17477,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					17991,
					17204,
					8961
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					17477,
					17684,
					8961,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					17684,
					17477,
					17991,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					17684,
					17477,
					17991,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					6555,
					10394,
					17204,
					17684,
					17477,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					17991,
					17204,
					8961
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					17477,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					10394,
					35244,
					17684,
					17477,
					17991,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					6555,
					35244,
					17684,
					17477,
					17991,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					17204,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					17204,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					17204,
					17684,
					34085
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					10394,
					6555,
					34085,
					17684,
					17204,
					17991,
					34392
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					34392,
					17991,
					17204,
					8961
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					34392,
					34085,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					17991,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					17204,
					17684,
					9234,
					8961,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					17204,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					35244,
					35004,
					17477,
					17991,
					17204,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					6555,
					10394,
					33912,
					34392,
					35244,
					35004,
					17204,
					17684,
					17477,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					35244,
					35004,
					9234,
					17477,
					17991,
					17204,
					8961
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					17477,
					9234,
					10394,
					6555,
					34392,
					33912,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					10394,
					35004,
					17477,
					17991,
					17204,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					8961,
					5379,
					6555,
					35004,
					17684,
					17477,
					17991,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					17991,
					17477,
					34085,
					34392,
					33912,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					17477,
					17991,
					17204,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					17684,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					17684,
					34085,
					35244,
					35004,
					8961,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					8961,
					9234,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					5379,
					9234,
					34085,
					6555,
					10394,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					17684,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					9234,
					17684,
					17204,
					17991,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					17204,
					8961,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					17991,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					17684,
					17477,
					33912
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					6555,
					10394,
					33639,
					17204,
					17684,
					17477,
					33912
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					8961,
					9234,
					17477,
					33912
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					33639,
					33912,
					17477,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					35244,
					10394,
					33912,
					17477,
					17684,
					17204,
					33639
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					6555,
					35244,
					33639,
					17204,
					17684,
					17477,
					33912
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					33912,
					33639,
					17204,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					33639,
					33912,
					17477,
					34085,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					33639,
					17204,
					17684,
					34085
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					6555,
					10394,
					33639,
					17204,
					17684,
					34085,
					34392,
					33912
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					8961,
					9234,
					34085,
					34392,
					33912
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					34392,
					33912,
					33639,
					17204,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					33639,
					33912,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					33639,
					17204,
					17684,
					9234,
					8961,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					33639,
					17204,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					33639,
					33912,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					17204,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					17204,
					33639,
					35004,
					35244,
					5379,
					8961,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					8961,
					17204,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					5379,
					17204,
					33639,
					10394,
					6555,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					17684,
					17477,
					34392,
					34085,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					9234,
					34085,
					34392,
					17477,
					17684,
					17204,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					33639,
					35004,
					10394,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					5379,
					6555,
					35004,
					34392,
					34085,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					17684,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					17684,
					34085,
					35244,
					35004,
					8961,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					17204,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17204,
					33639,
					34085,
					9234,
					35004,
					35244,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					9234,
					17684,
					17204,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17204,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					35004,
					6555,
					17991,
					17204,
					17684,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					35004,
					10394,
					17204,
					17684,
					17477,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					6555,
					35004,
					8961,
					17204,
					17991,
					17477,
					9234
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					17991,
					17204,
					5379,
					5686,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					10394,
					35244,
					5686,
					33639,
					35004,
					6555,
					17684,
					17477,
					17991,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					5686,
					33639,
					35004,
					35244,
					17204,
					17684,
					17477,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					17204,
					8961,
					10394,
					35244,
					5686,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					5379,
					17204,
					17991,
					17477,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					35004,
					6555,
					34392,
					17991,
					17204,
					17684,
					34085
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					35004,
					10394,
					34392,
					17991,
					17204,
					17684,
					34085
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					35004,
					6555,
					34392,
					17991,
					17204,
					8961,
					9234,
					34085
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					34392,
					17991,
					17204,
					5379,
					5686,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					17991,
					34392,
					35244,
					10394,
					33639,
					5686,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					5379,
					8961,
					9234,
					17684,
					17204,
					17991,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					17204,
					8961,
					10394,
					35244,
					5686,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					5379,
					17204,
					17991,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					35244,
					6555,
					17991,
					17204,
					17684,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					33912,
					34392,
					35244,
					10394,
					17991,
					17204,
					17684,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					35244,
					6555,
					9234,
					17477,
					17991,
					17204,
					8961
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					33639,
					5686,
					5379,
					17204,
					17991,
					17477,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					34085,
					9234,
					10394,
					6555,
					17477,
					17991,
					17204,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					33912,
					34392,
					34085,
					9234,
					17204,
					17684,
					17477,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					34085,
					17477,
					17991,
					17204,
					8961,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					5686,
					33639,
					33912,
					34392,
					34085,
					17477,
					17991,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					17204,
					17991,
					33912,
					33639,
					5686,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					33912,
					17991,
					17204,
					17684,
					34085,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					17204,
					17991,
					33912,
					33639,
					5686,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					35244,
					10394,
					5379,
					5686,
					33639,
					33912,
					17991,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					17991,
					17204,
					17684,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					8961,
					5379,
					5686,
					33639,
					33912,
					17991,
					17204,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					17991,
					17204,
					8961,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					17991,
					17204,
					5379
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					17477,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					17684,
					17204,
					5686,
					5379,
					8961,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					8961,
					9234,
					17477,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					9234,
					10394,
					35004,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					17477,
					33912,
					35004,
					6555,
					34085,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					5686,
					17204,
					17684,
					17477,
					33912,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					8961,
					34085,
					17477,
					33912,
					10394,
					35244,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					34085,
					35244,
					35004,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					34085,
					34392,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					17684,
					17204,
					5686,
					5379,
					8961,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					8961,
					17204,
					5686,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					10394,
					35004,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					9234,
					34392,
					33912,
					10394,
					35244,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					35004,
					35244,
					9234,
					8961,
					5379,
					5686,
					17204,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					8961,
					34392,
					33912,
					10394,
					35244,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					35004,
					35244,
					5379,
					5686,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					17477,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					17204,
					17684,
					17477,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					8961,
					17204,
					5686,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					34392,
					35244,
					10394,
					5379,
					5686,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					34392,
					17477,
					17684,
					17204,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					34085,
					9234,
					8961,
					5379,
					5686,
					17204,
					17684,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					5686,
					6555,
					10394,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					5379,
					17204,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					17684,
					34085,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					17204,
					17684,
					34085,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17204,
					8961,
					9234,
					34085,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					10394,
					35244,
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					17204,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					17204,
					17684,
					9234
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17204,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					5379,
					17204
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					17477,
					17991,
					5686
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					17991,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					9234,
					17477,
					17991,
					5686
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					17991,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					10394,
					35244,
					5379,
					17684,
					17477,
					17991,
					5686
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					17477,
					17684,
					8961,
					9234,
					34085,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					5686,
					5379,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					5686,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					34085,
					34392,
					17991,
					5686
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					34085,
					34392,
					17991,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					5686,
					5379,
					8961,
					9234,
					34085
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					34392,
					17991,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					5686,
					17991,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					34392,
					35244,
					6555,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					17991,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					35004,
					35244,
					5686,
					17991,
					17477,
					17684,
					5379
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					17991,
					5686,
					6555,
					10394,
					33912,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					35244,
					35004,
					9234,
					17477,
					17991,
					5686,
					5379,
					8961
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					17991,
					5686,
					6555,
					10394,
					33912,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					10394,
					35004,
					5379,
					17684,
					17477,
					17991,
					5686
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					8961,
					17684,
					17477,
					17991,
					5686,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					17991,
					17477,
					34085,
					34392,
					33912,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					17477,
					17991,
					5686,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					5686,
					5379,
					17684,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					34085,
					33912,
					17991,
					5686,
					35244,
					35004,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					5686,
					17991,
					33912,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					5686,
					9234,
					34085,
					6555,
					10394,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					5686,
					17991,
					33912,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					33912,
					35004,
					6555,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17991,
					5686,
					5379,
					8961,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					17991,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					5379,
					17684,
					17477,
					33912
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					33912,
					33639,
					5686,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					33912,
					33639,
					5686,
					5379,
					8961
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					17477,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					10394,
					35244,
					5379,
					17684,
					17477,
					33912,
					33639,
					5686
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					17477,
					17684,
					8961,
					9234,
					34085,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					33912,
					17477,
					34085,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					33912,
					33639,
					5686,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					34085,
					34392,
					33912,
					33639,
					5686
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					34085,
					17684,
					8961,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					34085,
					9234,
					8961,
					5379
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					34085,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					33639,
					5686,
					5379,
					17684,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					35244,
					6555,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					33912,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					33912,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					5379,
					5686,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					34392,
					33639,
					5686,
					35244,
					35004,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					5379,
					8961,
					9234,
					17477,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					5686,
					33639,
					10394,
					6555,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					5379,
					17684,
					17477,
					34392,
					34085,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					6555,
					35004,
					34392,
					34085,
					9234,
					8961,
					17684,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					35004,
					10394,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					35004,
					6555,
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					5379,
					17684,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					34085,
					33639,
					5686,
					35244,
					35004,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					5686,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					33639,
					5686,
					35244,
					35004,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					5686,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					5686,
					6555,
					35004,
					9234,
					8961,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					5686,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5686,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					17684,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					17991,
					33639,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					9234,
					8961,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					17684,
					5379,
					6555,
					35004,
					9234,
					34085,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					17684,
					17477,
					17991,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					17991,
					33639,
					5379,
					8961,
					35004,
					6555,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					17477,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					34392,
					34085,
					17684,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					34392,
					34085,
					17684,
					8961,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					9234,
					34085,
					34392,
					17991,
					33639,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					34392,
					34085,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					33639,
					17991,
					34392,
					6555,
					35004,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					33639,
					35004,
					35244,
					9234,
					8961,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					17991,
					34392,
					8961,
					5379,
					35244,
					10394,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17991,
					33639,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					17477,
					17991,
					33639,
					33912,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					33639,
					17991,
					17477,
					17684,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					9234,
					17477,
					17991,
					33639,
					33912,
					34392,
					35244,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					33912,
					33639,
					17991,
					17477,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					34392,
					33912,
					33639,
					17991,
					17477,
					17684,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33639,
					33912,
					34392,
					34085,
					9234,
					8961,
					17684,
					17477,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					10394,
					6555,
					33639,
					33912,
					34392,
					34085,
					17477,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					17477,
					17991,
					33639
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					5379,
					6555,
					35244,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					8961,
					10394,
					35244,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					6555,
					35244,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					35244,
					10394,
					33639,
					33912,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					9234,
					10394,
					6555,
					33639,
					33912,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					9234,
					17684,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					6555,
					10394,
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					33639,
					17991
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					17477,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					17477,
					33912,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					9234,
					17477,
					33912,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					17477,
					33912,
					35004,
					6555,
					34085,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					17684,
					17477,
					33912,
					35004,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17477,
					33912,
					5379,
					8961,
					35004,
					6555,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					17477,
					34085,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					17684,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					17684,
					34085,
					34392,
					33912,
					35004,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					8961,
					5379,
					6555,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					34085,
					9234,
					10394,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17684,
					5379,
					33912,
					34392,
					6555,
					35004,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					35244,
					35004,
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					34392,
					33912,
					10394,
					35244,
					35004,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					33912,
					34392,
					35244,
					35004
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					17684,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34392,
					17477,
					9234,
					8961,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					17477,
					34392,
					35244,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					9234,
					34085,
					34392,
					17477,
					17684,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					17684,
					17477,
					34392
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					8961,
					10394,
					6555,
					34392,
					34085,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					34392,
					17477
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					17684,
					8961,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					8961,
					5379,
					6555,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					34085,
					9234,
					10394,
					35244
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					5379,
					17684,
					9234,
					10394,
					6555
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					9234,
					17684
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[]
				{
					8961,
					5379,
					6555,
					10394
				}),
				new Transvoxel.TransitionVertexData.Row(new ushort[0])
			};
		}

		// Token: 0x0400251D RID: 9501
		[PublicizedFrom(EAccessModifier.Private)]
		public Transvoxel.TransitionVertexData.Row[] rows;

		// Token: 0x020005B5 RID: 1461
		public class Row
		{
			// Token: 0x17000497 RID: 1175
			public ushort this[int _index]
			{
				get
				{
					return this.data[_index];
				}
			}

			// Token: 0x06002F02 RID: 12034 RVA: 0x00142970 File Offset: 0x00140B70
			public Row(ushort[] _data)
			{
				this.data = _data;
			}

			// Token: 0x0400251E RID: 9502
			[PublicizedFrom(EAccessModifier.Private)]
			public ushort[] data;
		}
	}
}
