using Godot;
using Godot.NativeInterop;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class CylinderCameraScanner : GpuParticles3D
{
	[Export] protected float hFOV = 360;
	[Export] protected float vFOV = 80;

	[Export] protected int cameraCount = 10;

	[Export] protected int hozPasses = 3600;
	[Export] protected int vertPasses = 25;

	[Export] protected MeshInstance3D testMesh;

	[Export] Camera3D cylinderCamera;
	[Export] SubViewport viewport;

	public override void _Ready()
	{
		InfoReadout.hoz = hozPasses;
		InfoReadout.vert = vertPasses;

		viewport = new SubViewport();
		viewport.Size = new Vector2I(hozPasses / cameraCount, vertPasses);
		viewport.UseHdr2D = true;
		viewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Always;

		Amount = hozPasses * vertPasses;
		OneShot = true;
		Lifetime = 600;
		SpeedScale = 1;
		Emitting = true;
		//CreateDebugTangents();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Image bigImage;
		if ((ProcessMaterial as ShaderMaterial).GetShaderParameter("points").VariantType != Variant.Type.Nil)
		{
			Image bigImage = (ProcessMaterial as ShaderMaterial).GetShaderParameter("points").As<Texture2D>().GetImage();
			bigImage.SavePng("res://bigImage.png");
		}
		else
		{
			GD.Print("Points was null");
		}


		RenderingServer.GlobalShaderParameterSet("caster_position", GlobalPosition);

		//GD.Print()

		GlobalRotation = Vector3.Zero;

		for (int i = 0; i < cameraCount; i++)
		{
			//cylinderCamera.Fov = hFOV;
			cylinderCamera.GlobalPosition = GlobalPosition;

			if (i % 2 == 0)
			{
				cylinderCamera.Rotation = new Vector3(0, Mathf.DegToRad((hFOV) * Mathf.CeilToInt(i / 2f)), 0);
			}
			else
			{
				cylinderCamera.Rotation = new Vector3(0, Mathf.DegToRad((-hFOV) * Mathf.CeilToInt(i / 2f)), 0);
			}


		}

		(ProcessMaterial as ShaderMaterial).SetShaderParameter("points", viewport.GetTexture());
		(ProcessMaterial as ShaderMaterial).SetShaderParameter("colorTest", new Color("Green"));
	}

	int gcf(int a, int b)
	{
		while (b != 0)
		{
			int temp = b;
			b = a % b;
			a = temp;
		}
		return a;
	}

	int lcm(int a, int b)
	{
		return (a / gcf(a, b)) * b;
	}

	void CameraAspects()
	{
		float aspectRatio = (hFOV / cameraCount) / vFOV; // aspect = h/v
		float resolutionRatio = (hozPasses / cameraCount) / vertPasses;
	}
}
