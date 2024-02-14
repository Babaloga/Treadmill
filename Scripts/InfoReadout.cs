using Godot;
using System;

public partial class InfoReadout : Label
{
	public static int hoz;
	public static int vert;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Text = "Hoz: " + hoz + "\nVert: " + vert + "\nFPS: " + Engine.GetFramesPerSecond();
	}
}
