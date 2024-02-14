using Godot;
using System;

public partial class Scanner2D : Node2D
{
	public Vector3[] drawPoints;
	public float[] drawDistances;
	[Export] Camera3D camera;

	public override void _Draw()
	{
		if (drawPoints == null) return;

		//DrawCircle(new Vector2(0, 0), 100, new Color("White"));

		//GD.Print(drawPoints.Length);

		foreach(Vector3 v in drawPoints)
		{
			//if (v == null) continue;
			if(!camera.IsPositionBehind(v))
				DrawCircle(camera.UnprojectPosition(v), 2, new Color("Red"));
		}
	}

	public void Refresh()
	{
		QueueRedraw();
	}

}
