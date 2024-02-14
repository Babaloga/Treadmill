using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CamVisualizer : Node2D
{
	List<Camera3D> cameras;
	[Export] Camera3D camera;

	public override void _Ready()
	{
		base._Ready();
		cameras = new List<Camera3D>();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		QueueRedraw();

		if (cameras.Count == 0)
		{
			Node parent = GetParent();

			var camNodes = parent.FindChildren("*", "Camera3D", true, false);

			foreach (Node n in camNodes)
			{
				cameras.Add(n as Camera3D);
				//GD.Print("NODE: " + n);
			}
		}
	}

	public override void _Draw()
	{
		if (cameras == null) return;

		//DrawCircle(new Vector2(0, 0), 100, new Color("White"));

		foreach (Camera3D c in cameras)
		{
			//if (v == null) continue;
			SubViewport sv = c.GetViewport() as SubViewport;
			DrawLine(camera.UnprojectPosition(c.GlobalPosition), camera.UnprojectPosition(c.ProjectPosition(Vector2.Zero, 1)), new Color("Blue"), 2);
			//DrawLine(camera.UnprojectPosition(c.GlobalPosition), camera.UnprojectPosition(c.ProjectPosition(Vector2.Right * sv.Size.X, 1)), new Color("Green"), 2);
			//DrawLine(camera.UnprojectPosition(c.GlobalPosition), camera.UnprojectPosition(c.ProjectPosition(Vector2.Up * sv.Size.Y, 1)), new Color("Orange"), 2);
			DrawLine(camera.UnprojectPosition(c.GlobalPosition), camera.UnprojectPosition(c.ProjectPosition(new Vector2(sv.Size.X, sv.Size.Y), 1)), new Color("Yellow"), 2);
			DrawLine(camera.UnprojectPosition(c.GlobalPosition), camera.UnprojectPosition(c.GlobalPosition - (c.GlobalBasis.Z)), new Color("Red"), 2);
			//DrawCircle(camera.UnprojectPosition(c.GlobalPosition), 2, new Color("Red"));

		}
	}

	public void Refresh()
	{
		QueueRedraw();
	}
}
