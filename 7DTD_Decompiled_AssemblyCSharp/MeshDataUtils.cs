using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020009F7 RID: 2551
public static class MeshDataUtils
{
	// Token: 0x06004E21 RID: 20001 RVA: 0x001EDA64 File Offset: 0x001EBC64
	public static void CopyInterleaved(int elementSize, int offset, int stride, ReadOnlySpan<byte> src, Span<byte> dest)
	{
		int num = 0;
		int num2 = offset;
		while (num + elementSize <= src.Length && num2 + stride - offset <= dest.Length)
		{
			src.Slice(num, elementSize).CopyTo(dest.Slice(num2, elementSize));
			num += elementSize;
			num2 += stride;
		}
	}

	// Token: 0x06004E22 RID: 20002 RVA: 0x001EDAB4 File Offset: 0x001EBCB4
	public static MeshDataLayout SetAttributes(Mesh.MeshData meshData, ReadOnlySpan<Vector3> vertices, ReadOnlySpan<int> indices, ReadOnlySpan<Vector3> normals, ReadOnlySpan<Vector4> tangents, ReadOnlySpan<Color> colors, ReadOnlySpan<Vector2> texCoord0s, ReadOnlySpan<Vector2> texCoord1s, NativeList<VertexAttributeDescriptor> vertexAttributesOut)
	{
		int num = 0;
		MeshDataLayout result;
		result.PositionSize = 12;
		result.PositionOffset = num;
		num += 12;
		VertexAttributeDescriptor vertexAttributeDescriptor = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0);
		vertexAttributesOut.Add(vertexAttributeDescriptor);
		if (normals.Length > 0)
		{
			result.NormalSize = 12;
			result.NormalOffset = num;
			num += 12;
			vertexAttributeDescriptor = new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 0);
			vertexAttributesOut.Add(vertexAttributeDescriptor);
		}
		else
		{
			result.NormalSize = -1;
			result.NormalOffset = -1;
		}
		if (tangents.Length > 0)
		{
			result.TangentSize = 16;
			result.TangentOffset = num;
			num += 16;
			vertexAttributeDescriptor = new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4, 0);
			vertexAttributesOut.Add(vertexAttributeDescriptor);
		}
		else
		{
			result.TangentSize = -1;
			result.TangentOffset = -1;
		}
		if (colors.Length > 0)
		{
			result.ColorSize = 16;
			result.ColorOffset = num;
			num += 16;
			vertexAttributeDescriptor = new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4, 0);
			vertexAttributesOut.Add(vertexAttributeDescriptor);
		}
		else
		{
			result.ColorSize = -1;
			result.ColorOffset = -1;
		}
		if (texCoord0s.Length > 0)
		{
			result.TexCoord0Size = 8;
			result.TexCoord0Offset = num;
			num += 8;
			vertexAttributeDescriptor = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 0);
			vertexAttributesOut.Add(vertexAttributeDescriptor);
		}
		else
		{
			result.TexCoord0Size = -1;
			result.TexCoord0Offset = -1;
		}
		if (texCoord1s.Length > 0)
		{
			result.TexCoord1Size = 8;
			result.TexCoord1Offset = num;
			num += 8;
			vertexAttributeDescriptor = new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float32, 2, 0);
			vertexAttributesOut.Add(vertexAttributeDescriptor);
		}
		else
		{
			result.TexCoord1Size = -1;
			result.TexCoord1Offset = -1;
		}
		meshData.SetVertexBufferParams(vertices.Length, vertexAttributesOut.AsArray());
		result.Stride = num;
		meshData.SetIndexBufferParams(indices.Length, IndexFormat.UInt32);
		return result;
	}

	// Token: 0x06004E23 RID: 20003 RVA: 0x001EDC70 File Offset: 0x001EBE70
	public static void SetSubmesh(Mesh.MeshData meshData, int vertexCount, int indexCount, Bounds bounds)
	{
		meshData.subMeshCount = 1;
		meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount, MeshTopology.Triangles)
		{
			bounds = bounds,
			vertexCount = vertexCount
		}, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds);
	}

	// Token: 0x06004E24 RID: 20004 RVA: 0x001EDCAC File Offset: 0x001EBEAC
	public unsafe static Bounds RecalculateBounds(ReadOnlySpan<Vector3> vertices)
	{
		Bounds result = new Bounds(*vertices[0], Vector3.zero);
		for (int i = 1; i < vertices.Length; i++)
		{
			result.Encapsulate(*vertices[i]);
		}
		return result;
	}

	// Token: 0x06004E25 RID: 20005 RVA: 0x001EDCF9 File Offset: 0x001EBEF9
	public static void ApplyMeshDataCompression(Mesh.MeshData meshData, NativeArray<VertexAttributeDescriptor> vertexAttributes)
	{
		GameUtils.ApplyCompressedVertexAttributes(vertexAttributes, false);
		meshData.SetVertexBufferParams(meshData.vertexCount, vertexAttributes);
	}

	// Token: 0x06004E26 RID: 20006 RVA: 0x001EDD18 File Offset: 0x001EBF18
	public unsafe static void CalculateNormals(ReadOnlySpan<Vector3> vertices, ReadOnlySpan<int> indices, Span<Vector3> normals)
	{
		int num = Math.Min(vertices.Length, normals.Length);
		for (int i = 0; i < normals.Length; i++)
		{
			*normals[i] = Vector3.zero;
		}
		using (NativeParallelMultiHashMap<Vector3, int> vertexToIndices = new NativeParallelMultiHashMap<Vector3, int>(num, Allocator.Temp))
		{
			for (int j = 0; j < num; j++)
			{
				vertexToIndices.Add(*vertices[j], j);
			}
			using (NativeArray<int> normalsAdded = new NativeArray<int>(num, Allocator.Temp, NativeArrayOptions.ClearMemory))
			{
				int num2 = 0;
				while (num2 + 3 <= indices.Length)
				{
					Vector3 vector = *vertices[num2];
					Vector3 vector2 = *vertices[num2 + 1];
					Vector3 vector3 = *vertices[num2 + 2];
					Vector3 normal = Vector3.Cross(vector2 - vector, vector3 - vector);
					normal.Normalize();
					MeshDataUtils.AddTriangleNormal(ref normals, vertexToIndices, normalsAdded, vector, normal);
					MeshDataUtils.AddTriangleNormal(ref normals, vertexToIndices, normalsAdded, vector2, normal);
					MeshDataUtils.AddTriangleNormal(ref normals, vertexToIndices, normalsAdded, vector3, normal);
					num2 += 3;
				}
				for (int k = 0; k < normals.Length; k++)
				{
					int num3 = normalsAdded[k];
					if (num3 > 0)
					{
						*normals[k] /= (float)num3;
					}
				}
			}
		}
	}

	// Token: 0x06004E27 RID: 20007 RVA: 0x001EDEC8 File Offset: 0x001EC0C8
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void AddTriangleNormal(ref Span<Vector3> normals, NativeParallelMultiHashMap<Vector3, int> vertexToIndices, NativeArray<int> normalsAdded, Vector3 vertex, Vector3 normal)
	{
		foreach (int num in vertexToIndices.GetValuesForKey(vertex))
		{
			*normals[num] += normal;
			int index = num;
			int num2 = normalsAdded[index];
			normalsAdded[index] = num2 + 1;
		}
	}

	// Token: 0x06004E28 RID: 20008 RVA: 0x001EDF4C File Offset: 0x001EC14C
	public unsafe static AtomicSafeHandleScope CreateNativeArray<[IsUnmanaged] T>(T* ptr, int length, out NativeArray<T> array) where T : struct, ValueType
	{
		array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)ptr, length, Allocator.None);
		return default(AtomicSafeHandleScope);
	}

	// Token: 0x04003BB9 RID: 15289
	public const MeshUpdateFlags DoNothing = MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds | MeshUpdateFlags.DontNotifyMeshUsers | MeshUpdateFlags.DontRecalculateBounds;
}
