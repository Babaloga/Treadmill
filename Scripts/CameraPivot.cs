using Godot;
using System;
using System.Diagnostics;

public partial class CameraPivot : Node3D
{
	[Export] float sensitivity = 1;
	[Export] Node3D target;
	Node3D camPivot;
	Node3D camPitch;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camPivot = this;
		GD.Print(GetChild(0));
		camPitch = GetChild(0) as Node3D;
		
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		camPivot.GlobalPosition = target.GlobalPosition;
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			Vector2 mouseRelative = eventMouseMotion.Relative;

			camPitch.RotationDegrees = new Vector3(Math.Clamp(camPitch.RotationDegrees.X + (mouseRelative.Y * sensitivity), -90, 90), 0, 0);

			camPitch.RotateX(-mouseRelative.Y * sensitivity);

			camPivot.GlobalRotate(Vector3.Up, -mouseRelative.X * sensitivity);

			//camPivot.RotationDegrees = new Vector3(camPivot.RotationDegrees.X, camPivot.RotationDegrees.Y, 0);
		}
		else if (@event is InputEventKey eventKey)
		{
			if(eventKey.Keycode == Key.Escape)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
		}
	}
}
