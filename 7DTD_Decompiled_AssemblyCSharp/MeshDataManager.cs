using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020009F0 RID: 2544
public class MeshDataManager
{
	// Token: 0x06004DF0 RID: 19952 RVA: 0x001EC6FC File Offset: 0x001EA8FC
	[PublicizedFrom(EAccessModifier.Private)]
	public MeshDataManager()
	{
	}

	// Token: 0x06004DF1 RID: 19953 RVA: 0x001EC730 File Offset: 0x001EA930
	public static void Init()
	{
		MeshDataManager.Enabled = true;
	}

	// Token: 0x06004DF2 RID: 19954 RVA: 0x001EC738 File Offset: 0x001EA938
	public void LateUpdate()
	{
		this.CheckUnstarted();
		this.CompleteBatches();
	}

	// Token: 0x06004DF3 RID: 19955 RVA: 0x001EC746 File Offset: 0x001EA946
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckUnstarted()
	{
		if (this.m_toStartForEndOfFrame.Count <= 0 && this.m_toStartForEndOfNextFrame.Count <= 0)
		{
			return;
		}
		Log.Error("[MeshDataManager] There were unstarted batches at the end of the frame.");
		this.StartBatches();
	}

	// Token: 0x06004DF4 RID: 19956 RVA: 0x001EC778 File Offset: 0x001EA978
	[PublicizedFrom(EAccessModifier.Private)]
	public void CompleteBatches()
	{
		if (this.m_completeAtEndOfFrame.Count <= 0 && this.m_completeAtEndOfNextFrame.Count <= 0)
		{
			return;
		}
		using (MeshDataManager.s_markerCompleteBatches.Auto())
		{
			foreach (MeshDataManager.JobBatch jobBatch in this.m_completeAtEndOfFrame)
			{
				try
				{
					jobBatch.ApplyAndDispose();
				}
				catch (Exception arg)
				{
					Log.Error(string.Format("[MeshDataManager] Failed to apply and dispose job batch: {0}", arg));
				}
				finally
				{
					try
					{
						jobBatch.Dispose();
					}
					catch (Exception arg2)
					{
						Log.Error(string.Format("[MeshDataManager] Failed to dispose job batch: {0}", arg2));
					}
				}
			}
			this.m_completeAtEndOfFrame.Clear();
			List<MeshDataManager.JobBatch> completeAtEndOfNextFrame = this.m_completeAtEndOfNextFrame;
			List<MeshDataManager.JobBatch> completeAtEndOfFrame = this.m_completeAtEndOfFrame;
			this.m_completeAtEndOfFrame = completeAtEndOfNextFrame;
			this.m_completeAtEndOfNextFrame = completeAtEndOfFrame;
		}
	}

	// Token: 0x06004DF5 RID: 19957 RVA: 0x001EC898 File Offset: 0x001EAA98
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe static bool PreValidateJobData(Mesh mesh, ReadOnlySpan<Vector3> vertices, ReadOnlySpan<int> indices, ReadOnlySpan<Vector3> normals, ReadOnlySpan<Vector4> tangents, ReadOnlySpan<Color> colors, ReadOnlySpan<Vector2> texCoord0s, ReadOnlySpan<Vector2> texCoord1s)
	{
		if (!mesh)
		{
			Log.Error("Mesh is not valid.");
			return false;
		}
		int length = vertices.Length;
		if (length <= 0)
		{
			Log.Error(string.Format("Vertices.Length ({0}) <= 0", length));
			return false;
		}
		int length2 = indices.Length;
		if (length2 <= 0)
		{
			Log.Error(string.Format("Indices.Length ({0}) <= 0", length2));
			return false;
		}
		if (normals != null && normals.Length > 0 && normals.Length != length)
		{
			Log.Error(string.Format("Normals.Count ({0}) != Vertices.Length ({1})", normals.Length, length));
			return false;
		}
		if (tangents != null && tangents.Length > 0 && tangents.Length != length)
		{
			Log.Error(string.Format("Tangents.Length ({0}) != Vertices.Length ({1})", tangents.Length, length));
			return false;
		}
		if (colors.Length != length)
		{
			Log.Error(string.Format("Colors.Length ({0}) != Vertices.Length ({1})", colors.Length, length));
			return false;
		}
		if (texCoord0s != null && texCoord0s.Length > 0 && texCoord0s.Length != length)
		{
			Log.Error(string.Format("TexCoord0s.Length ({0}) != Vertices.Length ({1})", texCoord0s.Length, length));
			return false;
		}
		if (texCoord1s != null && texCoord1s.Length > 0 && texCoord1s.Length != length)
		{
			Log.Error(string.Format("TexCoord1s.Length ({0}) != Vertices.Length ({1})", texCoord1s.Length, length));
			return false;
		}
		for (int i = 0; i < length2; i++)
		{
			if (*indices[i] < 0)
			{
				Log.Error(string.Format("Indices[{0}] ({1}) < 0", i, *indices[i]));
				return false;
			}
			if (*indices[i] > length)
			{
				Log.Error(string.Format("Indices[{0}] ({1}) > Vertices.Length ({2})", i, *indices[i], length));
				return false;
			}
		}
		return true;
	}

	// Token: 0x06004DF6 RID: 19958 RVA: 0x001ECAB8 File Offset: 0x001EACB8
	public void Add(Mesh mesh, ArrayListMP<Vector3> vertices, ArrayListMP<int> indices, ArrayListMP<Vector3> normals, ArrayListMP<Vector4> tangents, ArrayListMP<Color> colors, ArrayListMP<Vector2> texCoord0s, ArrayListMP<Vector2> texCoord1s, bool cloneData = false, bool generateNormals = true, bool generateTangents = true, bool recalculateUvDistributionMetrics = false, bool sameFrameUpload = false, Action onJobComplete = null)
	{
		using (MeshDataManager.s_markerAdd.Auto())
		{
			if (!MeshDataManager.Enabled)
			{
				Log.Warning("[MeshDataManager] MeshDataJob was added while disabled.");
			}
			if (MeshDataManager.PreValidateJobData(mesh, MeshDataManager.ToReadOnlySpan<Vector3>(vertices), MeshDataManager.ToReadOnlySpan<int>(indices), MeshDataManager.ToReadOnlySpan<Vector3>(normals), MeshDataManager.ToReadOnlySpan<Vector4>(tangents), MeshDataManager.ToReadOnlySpan<Color>(colors), MeshDataManager.ToReadOnlySpan<Vector2>(texCoord0s), MeshDataManager.ToReadOnlySpan<Vector2>(texCoord1s)))
			{
				MeshDataManager.JobState item = this.CreateJobState(mesh, vertices, indices, normals, tangents, colors, texCoord0s, texCoord1s, cloneData, generateNormals, generateTangents, recalculateUvDistributionMetrics, onJobComplete);
				if (sameFrameUpload)
				{
					this.m_toStartForEndOfFrame.Add(item);
				}
				else
				{
					this.m_toStartForEndOfNextFrame.Add(item);
				}
			}
		}
	}

	// Token: 0x06004DF7 RID: 19959 RVA: 0x001ECB7C File Offset: 0x001EAD7C
	public void StartBatches()
	{
		using (MeshDataManager.s_markerStartBatches.Auto())
		{
			MeshDataManager.StartBatch(this.m_toStartForEndOfFrame, this.m_completeAtEndOfFrame);
			MeshDataManager.StartBatch(this.m_toStartForEndOfNextFrame, this.m_completeAtEndOfNextFrame);
		}
	}

	// Token: 0x06004DF8 RID: 19960 RVA: 0x001ECBDC File Offset: 0x001EADDC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void StartBatch(List<MeshDataManager.JobState> states, List<MeshDataManager.JobBatch> batches)
	{
		if (states.Count <= 0)
		{
			return;
		}
		bool flag = false;
		MeshDataManager.JobBatch jobBatch = null;
		try
		{
			jobBatch = new MeshDataManager.JobBatch(states);
			states.Clear();
			jobBatch.Start();
			batches.Add(jobBatch);
			flag = true;
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
		finally
		{
			if (!flag && jobBatch != null)
			{
				jobBatch.Dispose();
			}
		}
	}

	// Token: 0x06004DF9 RID: 19961 RVA: 0x001ECC48 File Offset: 0x001EAE48
	[PublicizedFrom(EAccessModifier.Private)]
	public static ReadOnlySpan<T> ToReadOnlySpan<T>(ArrayListMP<T> list) where T : new()
	{
		if (list != null)
		{
			return list.Items.AsSpan(0, list.Count);
		}
		return ReadOnlySpan<T>.Empty;
	}

	// Token: 0x06004DFA RID: 19962 RVA: 0x001ECC6C File Offset: 0x001EAE6C
	[PublicizedFrom(EAccessModifier.Private)]
	public MeshDataManager.JobState CreateJobState(Mesh mesh, ArrayListMP<Vector3> vertices, ArrayListMP<int> indices, ArrayListMP<Vector3> normals, ArrayListMP<Vector4> tangents, ArrayListMP<Color> colors, ArrayListMP<Vector2> texCoord0s, ArrayListMP<Vector2> texCoord1s, bool cloneData, bool generateNormals, bool generateTangents, bool recalculateUvDistributionMetrics, Action onJobComplete)
	{
		bool flag = false;
		List<PinnedBuffer> list = new List<PinnedBuffer>();
		MeshDataManager.JobState result;
		try
		{
			PinnedBuffer<Vector3> pinnedBuffer = PinnedBuffer.Create<Vector3>(vertices, cloneData);
			list.Add(pinnedBuffer);
			PinnedBuffer<int> pinnedBuffer2 = PinnedBuffer.Create<int>(indices, cloneData);
			list.Add(pinnedBuffer2);
			PinnedBuffer<Vector3> pinnedBuffer3;
			if (generateNormals && (normals == null || normals.Count <= 0))
			{
				pinnedBuffer3 = new PinnedBuffer<Vector3>(MemoryPools.poolVector3, pinnedBuffer.Length);
			}
			else
			{
				pinnedBuffer3 = PinnedBuffer.Create<Vector3>(normals, cloneData);
				generateNormals = false;
			}
			list.Add(pinnedBuffer3);
			PinnedBuffer<Vector4> pinnedBuffer4;
			if (generateTangents && (tangents == null || tangents.Count <= 0) && pinnedBuffer3.Length > 0)
			{
				pinnedBuffer4 = new PinnedBuffer<Vector4>(MemoryPools.poolVector4, pinnedBuffer.Length);
			}
			else
			{
				pinnedBuffer4 = PinnedBuffer.Create<Vector4>(tangents, cloneData);
				generateTangents = false;
			}
			list.Add(pinnedBuffer4);
			PinnedBuffer<Color> pinnedBuffer5 = PinnedBuffer.Create<Color>(colors, cloneData);
			list.Add(pinnedBuffer5);
			PinnedBuffer<Vector2> pinnedBuffer6 = PinnedBuffer.Create<Vector2>(texCoord0s, cloneData);
			list.Add(pinnedBuffer6);
			PinnedBuffer<Vector2> pinnedBuffer7 = PinnedBuffer.Create<Vector2>(texCoord1s, cloneData);
			list.Add(pinnedBuffer7);
			MeshDataManager.JobState jobState = new MeshDataManager.JobState(mesh, pinnedBuffer, pinnedBuffer2, pinnedBuffer3, pinnedBuffer4, pinnedBuffer5, pinnedBuffer6, pinnedBuffer7, generateNormals, generateTangents, recalculateUvDistributionMetrics, onJobComplete);
			flag = true;
			result = jobState;
		}
		finally
		{
			if (!flag)
			{
				foreach (PinnedBuffer pinnedBuffer8 in list)
				{
					pinnedBuffer8.Dispose();
				}
			}
		}
		return result;
	}

	// Token: 0x04003B8B RID: 15243
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerMarker s_markerCompleteBatches = new ProfilerMarker("MeshDataManager.CompleteBatches");

	// Token: 0x04003B8C RID: 15244
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerMarker s_markerAdd = new ProfilerMarker("MeshDataManager.Add");

	// Token: 0x04003B8D RID: 15245
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ProfilerMarker s_markerStartBatches = new ProfilerMarker("MeshDataManager.StartBatches");

	// Token: 0x04003B8E RID: 15246
	public static readonly MeshDataManager Instance = new MeshDataManager();

	// Token: 0x04003B8F RID: 15247
	public static bool Enabled;

	// Token: 0x04003B90 RID: 15248
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<MeshDataManager.JobState> m_toStartForEndOfFrame = new List<MeshDataManager.JobState>();

	// Token: 0x04003B91 RID: 15249
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<MeshDataManager.JobState> m_toStartForEndOfNextFrame = new List<MeshDataManager.JobState>();

	// Token: 0x04003B92 RID: 15250
	[PublicizedFrom(EAccessModifier.Private)]
	public List<MeshDataManager.JobBatch> m_completeAtEndOfFrame = new List<MeshDataManager.JobBatch>();

	// Token: 0x04003B93 RID: 15251
	[PublicizedFrom(EAccessModifier.Private)]
	public List<MeshDataManager.JobBatch> m_completeAtEndOfNextFrame = new List<MeshDataManager.JobBatch>();

	// Token: 0x020009F1 RID: 2545
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class JobBatch : IDisposable
	{
		// Token: 0x06004DFC RID: 19964 RVA: 0x001ECE09 File Offset: 0x001EB009
		public JobBatch(IEnumerable<MeshDataManager.JobState> stateSupplier)
		{
			this.m_states = stateSupplier.ToArray<MeshDataManager.JobState>();
			this.m_meshDataArray = Mesh.AllocateWritableMeshData(this.m_states.Length);
		}

		// Token: 0x06004DFD RID: 19965 RVA: 0x001ECE30 File Offset: 0x001EB030
		public void Start()
		{
			Task[] array = new Task[this.m_states.Length];
			for (int i = 0; i < this.m_states.Length; i++)
			{
				array[i] = this.m_states[i].Start(this.m_meshDataArray[i]);
			}
			this.m_statesAllComplete = Task.WhenAll(array);
		}

		// Token: 0x06004DFE RID: 19966 RVA: 0x001ECE88 File Offset: 0x001EB088
		public void ApplyAndDispose()
		{
			this.m_statesAllComplete.Wait();
			Mesh[] meshes = (from s in this.m_states
			select s.Mesh).ToArray<Mesh>();
			this.m_disposedMeshData = true;
			Mesh.ApplyAndDisposeWritableMeshData(this.m_meshDataArray, meshes, MeshUpdateFlags.DontRecalculateBounds);
			this.m_meshDataArray = default(Mesh.MeshDataArray);
			MeshDataManager.JobState[] states = this.m_states;
			for (int i = 0; i < states.Length; i++)
			{
				states[i].PostApply();
			}
			this.Dispose();
		}

		// Token: 0x06004DFF RID: 19967 RVA: 0x001ECF14 File Offset: 0x001EB114
		public void Dispose()
		{
			if (this.m_disposed)
			{
				return;
			}
			this.m_disposed = true;
			foreach (MeshDataManager.JobState jobState in this.m_states)
			{
				if (jobState != null)
				{
					jobState.Dispose();
				}
			}
			if (!this.m_disposedMeshData)
			{
				this.m_meshDataArray.Dispose();
				this.m_meshDataArray = default(Mesh.MeshDataArray);
				this.m_disposedMeshData = true;
			}
		}

		// Token: 0x170007FD RID: 2045
		// (get) Token: 0x06004E00 RID: 19968 RVA: 0x001ECF7A File Offset: 0x001EB17A
		public Mesh.MeshDataArray MeshDataArray
		{
			get
			{
				return this.m_meshDataArray;
			}
		}

		// Token: 0x04003B94 RID: 15252
		[PublicizedFrom(EAccessModifier.Private)]
		public Mesh.MeshDataArray m_meshDataArray;

		// Token: 0x04003B95 RID: 15253
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MeshDataManager.JobState[] m_states;

		// Token: 0x04003B96 RID: 15254
		[PublicizedFrom(EAccessModifier.Private)]
		public Task m_statesAllComplete;

		// Token: 0x04003B97 RID: 15255
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_disposed;

		// Token: 0x04003B98 RID: 15256
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_disposedMeshData;
	}

	// Token: 0x020009F3 RID: 2547
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class JobState : IDisposable
	{
		// Token: 0x06004E04 RID: 19972 RVA: 0x001ECF98 File Offset: 0x001EB198
		public JobState(Mesh mesh, PinnedBuffer<Vector3> vertices, PinnedBuffer<int> indices, PinnedBuffer<Vector3> normals, PinnedBuffer<Vector4> tangents, PinnedBuffer<Color> colors, PinnedBuffer<Vector2> texCoord0s, PinnedBuffer<Vector2> texCoord1s, bool generateNormals, bool generateTangents, bool recalculateUvDistributionMetrics, Action onJobComplete)
		{
			this.Mesh = mesh;
			this.Vertices = vertices;
			this.Indices = indices;
			this.Normals = normals;
			this.Tangents = tangents;
			this.Colors = colors;
			this.TexCoord0s = texCoord0s;
			this.TexCoord1s = texCoord1s;
			this.GenerateNormals = generateNormals;
			this.GenerateTangents = generateTangents;
			this.RecalculateUvDistributionMetrics = recalculateUvDistributionMetrics;
			this.OnJobComplete = onJobComplete;
		}

		// Token: 0x06004E05 RID: 19973 RVA: 0x001ED008 File Offset: 0x001EB208
		public void Dispose()
		{
			CancellationTokenSource copyTaskCancellation = this.CopyTaskCancellation;
			if (copyTaskCancellation != null)
			{
				copyTaskCancellation.Cancel();
			}
			Task copyTask = this.CopyTask;
			if (copyTask != null)
			{
				copyTask.Wait();
			}
			Task copyTask2 = this.CopyTask;
			if (copyTask2 != null)
			{
				copyTask2.Dispose();
			}
			this.CopyTask = null;
			CancellationTokenSource copyTaskCancellation2 = this.CopyTaskCancellation;
			if (copyTaskCancellation2 != null)
			{
				copyTaskCancellation2.Dispose();
			}
			this.CopyTaskCancellation = null;
			PinnedBuffer<VertexAttributeDescriptor> vertexAttributes = this.VertexAttributes;
			if (vertexAttributes != null)
			{
				vertexAttributes.Dispose();
			}
			this.VertexAttributes = null;
			PinnedBuffer<Vector3> vertices = this.Vertices;
			if (vertices != null)
			{
				vertices.Dispose();
			}
			this.Vertices = null;
			PinnedBuffer<int> indices = this.Indices;
			if (indices != null)
			{
				indices.Dispose();
			}
			this.Indices = null;
			PinnedBuffer<Vector2> texCoord0s = this.TexCoord0s;
			if (texCoord0s != null)
			{
				texCoord0s.Dispose();
			}
			this.TexCoord0s = null;
			PinnedBuffer<Vector2> texCoord1s = this.TexCoord1s;
			if (texCoord1s != null)
			{
				texCoord1s.Dispose();
			}
			this.TexCoord1s = null;
			PinnedBuffer<Vector3> normals = this.Normals;
			if (normals != null)
			{
				normals.Dispose();
			}
			this.Normals = null;
			PinnedBuffer<Vector4> tangents = this.Tangents;
			if (tangents != null)
			{
				tangents.Dispose();
			}
			this.Tangents = null;
			PinnedBuffer<Color> colors = this.Colors;
			if (colors != null)
			{
				colors.Dispose();
			}
			this.Colors = null;
			Action onJobComplete = this.OnJobComplete;
			if (onJobComplete != null)
			{
				onJobComplete();
			}
			this.OnJobComplete = null;
		}

		// Token: 0x06004E06 RID: 19974 RVA: 0x001ED140 File Offset: 0x001EB340
		public Task Start(Mesh.MeshData meshData)
		{
			this.CopyTaskCancellation = new CancellationTokenSource();
			return this.CopyTask = Task.Run(() => this.CopyToMeshData(meshData, this.CopyTaskCancellation.Token), this.CopyTaskCancellation.Token);
		}

		// Token: 0x06004E07 RID: 19975 RVA: 0x001ED194 File Offset: 0x001EB394
		[PublicizedFrom(EAccessModifier.Private)]
		public Task CopyToMeshData(Mesh.MeshData meshData, CancellationToken cancellationToken)
		{
			MeshDataManager.JobState.<CopyToMeshData>d__19 <CopyToMeshData>d__;
			<CopyToMeshData>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CopyToMeshData>d__.<>4__this = this;
			<CopyToMeshData>d__.meshData = meshData;
			<CopyToMeshData>d__.cancellationToken = cancellationToken;
			<CopyToMeshData>d__.<>1__state = -1;
			<CopyToMeshData>d__.<>t__builder.Start<MeshDataManager.JobState.<CopyToMeshData>d__19>(ref <CopyToMeshData>d__);
			return <CopyToMeshData>d__.<>t__builder.Task;
		}

		// Token: 0x06004E08 RID: 19976 RVA: 0x001ED1E7 File Offset: 0x001EB3E7
		public void PostApply()
		{
			this.Mesh.bounds = this.Bounds;
			if (this.RecalculateUvDistributionMetrics)
			{
				this.Mesh.RecalculateUVDistributionMetrics(1E-09f);
			}
		}

		// Token: 0x04003B9B RID: 15259
		public readonly Mesh Mesh;

		// Token: 0x04003B9C RID: 15260
		[PublicizedFrom(EAccessModifier.Private)]
		public PinnedBuffer<Vector3> Vertices;

		// Token: 0x04003B9D RID: 15261
		[PublicizedFrom(EAccessModifier.Private)]
		public PinnedBuffer<int> Indices;

		// Token: 0x04003B9E RID: 15262
		[PublicizedFrom(EAccessModifier.Private)]
		public PinnedBuffer<Vector3> Normals;

		// Token: 0x04003B9F RID: 15263
		[PublicizedFrom(EAccessModifier.Private)]
		public PinnedBuffer<Vector4> Tangents;

		// Token: 0x04003BA0 RID: 15264
		[PublicizedFrom(EAccessModifier.Private)]
		public PinnedBuffer<Color> Colors;

		// Token: 0x04003BA1 RID: 15265
		[PublicizedFrom(EAccessModifier.Private)]
		public PinnedBuffer<Vector2> TexCoord0s;

		// Token: 0x04003BA2 RID: 15266
		[PublicizedFrom(EAccessModifier.Private)]
		public PinnedBuffer<Vector2> TexCoord1s;

		// Token: 0x04003BA3 RID: 15267
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool GenerateNormals;

		// Token: 0x04003BA4 RID: 15268
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool GenerateTangents;

		// Token: 0x04003BA5 RID: 15269
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool RecalculateUvDistributionMetrics;

		// Token: 0x04003BA6 RID: 15270
		[PublicizedFrom(EAccessModifier.Private)]
		public Action OnJobComplete;

		// Token: 0x04003BA7 RID: 15271
		[PublicizedFrom(EAccessModifier.Private)]
		public CancellationTokenSource CopyTaskCancellation;

		// Token: 0x04003BA8 RID: 15272
		[PublicizedFrom(EAccessModifier.Private)]
		public Task CopyTask;

		// Token: 0x04003BA9 RID: 15273
		[PublicizedFrom(EAccessModifier.Private)]
		public PinnedBuffer<VertexAttributeDescriptor> VertexAttributes;

		// Token: 0x04003BAA RID: 15274
		[PublicizedFrom(EAccessModifier.Private)]
		public Bounds Bounds;
	}
}
