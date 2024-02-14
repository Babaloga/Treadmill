using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public partial class ShaderCaster : MeshInstance3D
{
	[Export] public Node3D CasterNode;
	ArrayMesh dotMesh;

	public override void _Ready()
	{
		Godot.Collections.Array meshArray = new Godot.Collections.Array();

		meshArray.Resize((int)Mesh.ArrayType.Max);

		Vector3[] meshVerts = new Vector3[324000];
		int[] meshIndexes = new int[324000];
		System.Array.Fill(meshVerts, new Vector3(0, 0, 0));

		for (int i = 0; i < 324000; i++)
		{
			meshIndexes[i] = i;
		}


		meshArray[(int)Mesh.ArrayType.Vertex] = meshVerts;
		meshArray[(int)Mesh.ArrayType.Index] = meshIndexes;

		dotMesh = Mesh as ArrayMesh;

		if(dotMesh != null)
		{
			dotMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Points, meshArray);
		}

		//GD.Print("Executing ShaderCaster. Node: " + Name + ", Owner: " + Owner.Name);
		//GD.Print("Start Flag");
		//GD.PrintErr("Hello?");

		var rd = RenderingServer.CreateLocalRenderingDevice();

		var shaderFile = GD.Load<RDShaderFile>("res://Shaders/ComputeRayMarching.glsl");
		var shaderBytecode = shaderFile.GetSpirV();
		var shader = rd.ShaderCreateFromSpirV(shaderBytecode);

		// Prepare our data. We use floats in the shader, so we need 32 bit.
		InputStruct input = new InputStruct { casterForward = CasterNode.GlobalBasis.Z, casterPosition = CasterNode.GlobalPosition, fovDegrees = 360 };

		//var inputBytes = new byte[sizeof(float) * 7];
		//Buffer.BlockCopy(input, 0, inputBytes, 0, inputBytes.Length);

		var bufferSize = Marshal.SizeOf(input);
		var byteArray = new byte[bufferSize];

		IntPtr handle = Marshal.AllocHGlobal(bufferSize);
		Marshal.StructureToPtr(input, handle, true);
		Marshal.Copy(handle, byteArray, 0, bufferSize);

		// Create a storage buffer that can hold our float values.
		// Each float has 4 bytes (32 bit) so 10 x 4 = 40 bytes
		var buffer = rd.StorageBufferCreate((uint)byteArray.Length, byteArray);

		// Create a uniform to assign the buffer to the rendering device
		var uniform = new RDUniform
		{
			UniformType = RenderingDevice.UniformType.StorageBuffer,
			Binding = 0
		};
		uniform.AddId(buffer);
		

		RDTextureFormat textureFormat = new RDTextureFormat();
		textureFormat.Width = 3600;
		textureFormat.Height = 90;
		textureFormat.Format = RenderingDevice.DataFormat.R32G32B32A32Sfloat;
		textureFormat.UsageBits = RenderingDevice.TextureUsageBits.CanUpdateBit | RenderingDevice.TextureUsageBits.StorageBit | RenderingDevice.TextureUsageBits.CanCopyFromBit;

		RDTextureView view = new RDTextureView();

		Image outputImage = Image.Create(3600, 90, false, Image.Format.Rgbaf);
		var outputTex = rd.TextureCreate(textureFormat, view, new Array<byte[]>{ outputImage.GetData()});

		// Create a uniform to assign the buffer to the rendering device
		var textureUniform = new RDUniform
		{
			UniformType = RenderingDevice.UniformType.Image,
			Binding = 1
		};
		textureUniform.AddId(outputTex);

		var uniformSet = rd.UniformSetCreate(new Array<RDUniform> { uniform, textureUniform }, shader, 0);

		var pipeline = rd.ComputePipelineCreate(shader);
		var computeList = rd.ComputeListBegin();
		rd.ComputeListBindComputePipeline(computeList, pipeline);
		rd.ComputeListBindUniformSet(computeList, uniformSet, 0);
		rd.ComputeListDispatch(computeList, xGroups: 36, yGroups: 9, zGroups: 1);
		rd.ComputeListEnd();

		//GD.Print("Pre-Submit Flag");

		rd.Submit();

		//GD.Print("Post-Submit Flag");

		rd.Sync();

		//GD.Print("Post-Sync Flag");

		byte[] byteData = rd.TextureGetData(outputTex, 0);
		//GD.Print(byteData);
		Image imageReadout = Image.CreateFromData(3600, 90, false, Image.Format.Rgbaf, byteData);

		imageReadout.SavePng("res://ImageReadout.png");

		for (int x = 0; x < imageReadout.GetWidth(); x++)
		{
			for (int y = 0; y < imageReadout.GetHeight(); y++)
			{
				GD.Print(imageReadout.GetPixel(x, y));
			}
		}

		SetInstanceShaderParameter("points", ImageTexture.CreateFromImage(imageReadout));
	}

	struct InputStruct
	{
		public Vector3 casterPosition;
		public Vector3 casterForward;
		public float fovDegrees;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
