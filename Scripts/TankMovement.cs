using Godot;
using Godot.Collections;
using System;

public partial class TankMovement : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public const float RotateSpeed = 0.05f;
	
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    Vector3 leg1 = new Vector3(0.2f, 0, 0.2f);
    Vector3 leg2 = new Vector3(-0.2f, 0, 0.2f);
    Vector3 leg3 = new Vector3(0.2f, 0, -0.2f);
    Vector3 leg4 = new Vector3(-0.2f, 0, -0.2f);

    [Export] Node3D foot1;
    [Export] Node3D foot2;
    [Export] Node3D foot3;
    [Export] Node3D foot4;

    [Export] float standingHeight = 2f;

    public override void _Ready()
    {

    }

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
        ProcessTerrain();
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.

		//Rotate
		float rotateAxis = Input.GetAxis("Left","Right");
		Transform3D transform = Transform;
		transform.Basis = Transform.Basis.Rotated(Vector3.Down, rotateAxis * RotateSpeed);
		Transform  = transform;
		
		float inputAxis = Input.GetAxis("Forward","Backward");

		velocity = Transform.Basis.Z * Speed * inputAxis;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;
		
		Velocity = velocity;
		MoveAndSlide();
	}

    void ProcessTerrain()
    {
        Vector3 contact1 = LegContact(leg1);
        Vector3 contact2 = LegContact(leg2);
        Vector3 contact3 = LegContact(leg3);
        Vector3 contact4 = LegContact(leg4);

        foot1.GlobalPosition = contact1;
        foot2.GlobalPosition = contact2;
        foot3.GlobalPosition = contact3;
        foot4.GlobalPosition = contact4;

        Vector3 average = (contact1 + contact2 + contact3 + contact4) / 4f;

        GlobalPosition = new Vector3(GlobalPosition.X, average.Y + (standingHeight/2f), GlobalPosition.Z);
    }

    Vector3 LegContact(Vector3 legPoint)
    {
        PhysicsDirectSpaceState3D space = GetWorld3D().DirectSpaceState;

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(ToGlobal(legPoint), ToGlobal(legPoint - (Vector3.Up * standingHeight * 2f)));
        Dictionary result = space.IntersectRay(query);

        if(result.Count > 0)
        {
            return (Vector3) result["position"];
        }
        else
        {
            //return legPoint - (Vector3.Up * standingHeight * 2f);
            return Vector3.Zero;
        }
    }
}
