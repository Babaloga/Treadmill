using Godot;
using System;
using System.Linq;

public partial class Scanner : MeshInstance3D
{
	[Export] int hozResolution = 360;
	[Export] int vertResolution = 100;

	Scanner2D drawNode;
	[Export] Camera3D mainCamera;

	Vector3[] contactPoints;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		drawNode = GetChild(0) as Scanner2D;
		InfoReadout.hoz = hozResolution;
		InfoReadout.vert = vertResolution;
	}

	public override void _Process(double delta)
	{
		float hDelta = Mathf.DegToRad(360f / hozResolution);
		float vDelta = Mathf.DegToRad(90f / vertResolution);
		float vMin = Mathf.DegToRad(-45);

		//Restart();
		//ImmediateMesh mesh = new ImmediateMesh();
		//mesh.SurfaceBegin(Mesh.PrimitiveType.TriangleStrip, GD.Load<Material>("res://ParticleMat.tres"));
		//mesh.SurfaceSetColor(Color.FromHsv(0, 0, 1));

		contactPoints = new Vector3[hozResolution * vertResolution];
		RotateX(-Rotation.X + vMin);

		for (int v = 0; v < vertResolution; v++)
		{
			for (int h = 0; h < hozResolution; h++)
			{
				//mesh.SurfaceAddVertex(new Vector3(0, 10*v, 10*h));
				
				var spaceState = GetWorld3D().DirectSpaceState;
				PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(GlobalPosition, GlobalPosition + (1000 * -GlobalTransform.Basis.Z * 
					Quaternion.FromEuler(new Vector3(0, Mathf.RadToDeg(hDelta * h), 0)).Normalized())); 
				//query.Exclude = new Godot.Collections.Array<Rid>{ new Rid(GetOwner<Node3D>()) };
				var result = spaceState.IntersectRay(query);

				if(result.Count > 0)
				{
					//GD.Print("CONTACT! " + result);
					result.TryGetValue("position", out Variant contactVariant);
					Vector3 contact = contactVariant.AsVector3();
					contactPoints[(v * hozResolution) + h] = contact;
					//GD.Print(h + " " + v + ", Contact point: " + contact);
					//drawNode.DrawCircle(mainCamera.UnprojectPosition(contact), 2, new Color("Red"));
					//EmitParticle(new Transform3D(new Basis(), contact), Vector3.Zero, Color.FromHsv(0, 0, 1), Color.FromHsv(0,0,1), (uint)EmitFlags.Position);
					//mesh.SurfaceAddVertex(contact);
				}
				else
				{
					//GD.Print("-");
				}
				
			}

			RotateX(vDelta);
		}

		drawNode.drawPoints = contactPoints;
		drawNode.Refresh();
		//mesh.SurfaceEnd();
	}
}
