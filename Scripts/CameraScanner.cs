using Godot;
using Godot.NativeInterop;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class CameraScanner : GpuParticles3D
{
	protected List<Camera3D> cameras;
	protected List<SubViewport> viewports;
	protected List<ViewportTexture> textures;

	[Export] float sweepScrollSensitivity = 1;

	[Export] protected float hFOV = 360;
	[Export] protected float vFOV = 80;

	[Export] protected int cameraCount = 10;

	[Export] protected int hozPasses = 3600;
	int hozPassesPerCamera;

	[Export] protected int vertPasses = 25;

	protected ArrayMesh dotMesh;

	public override void _Ready()
	{
		hozPassesPerCamera = hozPasses / cameraCount;
		GD.Print(hozPassesPerCamera);

		InfoReadout.hoz = hozPasses;
		InfoReadout.vert = vertPasses;
		
		cameras = new List<Camera3D>();
		viewports = new List<SubViewport>();
		textures = new List<ViewportTexture>();

		for (int i = 0; i < cameraCount; i++)
		{
			SubViewport newView = new SubViewport();
			newView.Size = new Vector2I(hozPassesPerCamera, vertPasses);
			newView.UseHdr2D = true;
			
			//newView.Size2DOverrideStretch = true;
			//newView.Size2DOverride = new Vector2I(hozPasses / cameraCount, vertPasses);
			AddChild(newView);
			Camera3D newCam = new Camera3D();
			newView.AddChild(newCam);
			newCam.SetCullMaskValue(2, false);
			newCam.KeepAspect = Camera3D.KeepAspectEnum.Width;
			newCam.Near = 0.1f;
			newCam.Far = 4000f;
			newCam.Fov = hFOV / cameraCount;
			newCam.Scale = new Vector3(1, 10, 1);
			//newCam.Projection = Camera3D.ProjectionType.Frustum;
			cameras.Insert(i, newCam);
			viewports.Insert(i, newView);

			newView.RenderTargetUpdateMode = SubViewport.UpdateMode.Always;

			if (cameras[i].GetViewport() != viewports[i])
			{
				GD.PrintErr("The viewport thing isn't working");
			}
		}

		Amount = hozPasses * vertPasses;
		OneShot = false;
		Lifetime = 5;
		SpeedScale = 1;
		Emitting = true;
		//CreateDebugTangents();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		RenderingServer.GlobalShaderParameterSet("caster_position", GlobalPosition);

		GlobalRotation = Vector3.Zero;

		if (Input.IsActionJustReleased("NarrowSweep"))
		{
			hFOV = Mathf.Clamp(hFOV - (sweepScrollSensitivity), cameraCount, 360);
		}


		if (Input.IsActionJustReleased("BroadenSweep"))
		{
			hFOV = Mathf.Clamp(hFOV + (sweepScrollSensitivity), cameraCount, 360);
		}


		SizeCameras();

		Texture2D[] slices = new Texture2D[cameraCount];


		for (int i = 0; i < cameraCount; i++)
		{
			slices[i] = viewports[i].GetTexture();
			//slices[i].GetImage().SavePng("res://slice" + i + ".png");
		}

		(ProcessMaterial as ShaderMaterial).SetShaderParameter("points", slices);

		//(ProcessMaterial as ShaderMaterial).SetShaderParameter("points", ImageTexture.CreateFromImage(Image.CreateFromData(hozPasses / cameraCount, vertPasses * cameraCount, false, Image.Format.Rgbh, masterByteList.ToArray())));
		(ProcessMaterial as ShaderMaterial).SetShaderParameter("vertResolution", vertPasses);
		(ProcessMaterial as ShaderMaterial).SetShaderParameter("hozResolution", hozPassesPerCamera);
		(ProcessMaterial as ShaderMaterial).SetShaderParameter("colorTest", new Color("Green"));

		slices[0].GetImage().SavePng("res://sliceZero.png");

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
		// aspect = hRes * x / vRes * x, 

	}

	void SizeCameras()
	{
		float hozFOVPerCamera = hFOV / cameraCount;

		float aspectRatio = hozFOVPerCamera / vFOV; // aspect = h/v
		float resolutionRatio = hozPassesPerCamera / vertPasses;


		RenderingServer.GlobalShaderParameterSet("fov", vFOV);
		RenderingServer.GlobalShaderParameterSet("hfov", hozFOVPerCamera);

		//Vector2I perCamRes = new Vector2I(hozPassesPerCamera, Mathf.CeilToInt(hozPassesPerCamera / aspectRatio));

		Vector2I perCamRes = new Vector2I(hozPassesPerCamera, vertPasses);

		for (int i = 0; i < cameraCount; i++)
		{
			cameras[i].Fov = hozFOVPerCamera;
			viewports[i].Size = perCamRes;
			cameras[i].GlobalPosition = GlobalPosition;

			if (i % 2 == 0)
			{
				cameras[i].Rotation = new Vector3(0, GetParentNode3D().GlobalRotation.Y + Mathf.DegToRad((hFOV / cameras.Count) * Mathf.CeilToInt(i / 2f)), 0);
			}
			else
			{
				cameras[i].Rotation = new Vector3(0, GetParentNode3D().GlobalRotation.Y + Mathf.DegToRad((-hFOV / cameras.Count) * Mathf.CeilToInt(i / 2f)), 0);
			}

		}
	}
}
