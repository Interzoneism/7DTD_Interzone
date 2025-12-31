using System;
using System.Threading;
using UnityEngine;

// Token: 0x02000A6F RID: 2671
public class VoxelMeshLayer : IMemoryPoolableObject
{
	// Token: 0x0600516D RID: 20845 RVA: 0x0020B26C File Offset: 0x0020946C
	public VoxelMeshLayer()
	{
		this.idx = 0;
		this.meshes = new VoxelMesh[MeshDescription.meshes.Length];
		for (int i = 0; i < MeshDescription.meshes.Length; i++)
		{
			this.meshes[i] = VoxelMesh.Create(i, MeshDescription.meshes[i].meshType, 0);
		}
		Interlocked.Increment(ref VoxelMeshLayer.InstanceCount);
	}

	// Token: 0x0600516E RID: 20846 RVA: 0x0020B2D0 File Offset: 0x002094D0
	public void Reset()
	{
		for (int i = 0; i < this.meshes.Length; i++)
		{
			this.meshes[i].ClearMesh();
		}
		if (this.pendingCopyOperations != 0)
		{
			Log.Error("Resetting VML with pendingCopyOperations");
		}
		this.pendingCopyOperations = 0;
	}

	// Token: 0x0600516F RID: 20847 RVA: 0x0020B316 File Offset: 0x00209516
	public void Cleanup()
	{
		Interlocked.Decrement(ref VoxelMeshLayer.InstanceCount);
	}

	// Token: 0x06005170 RID: 20848 RVA: 0x0020B323 File Offset: 0x00209523
	public static void StaticCleanup()
	{
		VoxelMeshLayer.InstanceCount = 0;
	}

	// Token: 0x06005171 RID: 20849 RVA: 0x0020B32C File Offset: 0x0020952C
	public void SizeToChunkDefaults()
	{
		for (int i = 0; i < this.meshes.Length; i++)
		{
			this.meshes[i].SizeToChunkDefaults(i);
		}
	}

	// Token: 0x06005172 RID: 20850 RVA: 0x0020B35C File Offset: 0x0020955C
	public bool HasContent()
	{
		for (int i = 0; i < this.meshes.Length; i++)
		{
			if (this.meshes[i].Vertices.Count > 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005173 RID: 20851 RVA: 0x0020B394 File Offset: 0x00209594
	public int GetTris()
	{
		int num = 0;
		for (int i = 0; i < this.meshes.Length; i++)
		{
			num += this.meshes[i].Triangles;
		}
		return num;
	}

	// Token: 0x06005174 RID: 20852 RVA: 0x0020B3C7 File Offset: 0x002095C7
	public int GetTrisInMesh(int _idx)
	{
		return this.meshes[_idx].Triangles;
	}

	// Token: 0x06005175 RID: 20853 RVA: 0x0020B3D6 File Offset: 0x002095D6
	public int GetSizeOfMesh(int _idx)
	{
		return this.meshes[_idx].Size;
	}

	// Token: 0x06005176 RID: 20854 RVA: 0x0020B3E5 File Offset: 0x002095E5
	[PublicizedFrom(EAccessModifier.Internal)]
	public int CopyToMesh(int _meshIdx, MeshFilter[] _mf, MeshRenderer[] _mr, int _lodLevel)
	{
		this.pendingCopyOperations++;
		return this.meshes[_meshIdx].CopyToMesh(_mf, _mr, _lodLevel, delegate
		{
			this.TryFree();
		});
	}

	// Token: 0x06005177 RID: 20855 RVA: 0x0020B412 File Offset: 0x00209612
	[PublicizedFrom(EAccessModifier.Internal)]
	public void StartCopyMeshes()
	{
		if (this.pendingCopyOperations != 0)
		{
			Log.Error("StartCopyMeshes called with pendingCopyOperations != 0");
		}
		this.pendingCopyOperations++;
	}

	// Token: 0x06005178 RID: 20856 RVA: 0x0020B434 File Offset: 0x00209634
	[PublicizedFrom(EAccessModifier.Internal)]
	public void EndCopyMeshes()
	{
		this.TryFree();
	}

	// Token: 0x06005179 RID: 20857 RVA: 0x0020B43D File Offset: 0x0020963D
	public bool TryFree()
	{
		this.pendingCopyOperations--;
		if (this.pendingCopyOperations > 0)
		{
			return false;
		}
		MemoryPools.poolVML.FreeSync(this);
		return true;
	}

	// Token: 0x04003E7D RID: 15997
	public int idx;

	// Token: 0x04003E7E RID: 15998
	public VoxelMesh[] meshes;

	// Token: 0x04003E7F RID: 15999
	public static int InstanceCount;

	// Token: 0x04003E80 RID: 16000
	public int pendingCopyOperations;
}
