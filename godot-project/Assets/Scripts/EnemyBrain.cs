using Godot;
using System;

public partial class EnemyBrain : CharacterBody3D
{
	[ExportCategory("Node References")]
	[Export] private Node3D _target;
	
	[ExportCategory("Attributes")]
	[Export] private float _speed = 100f;

	public Node3D Target
	{
		get => _target;
		set => _target = value;
	}

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector3 toTarget = new Vector3(
			_target.GlobalPosition.X - this.GlobalPosition.X,
			0f,
			_target.GlobalPosition.Z - this.GlobalPosition.Z
		).Normalized();
		this.Velocity = toTarget * _speed * (float)delta;
		
		this.MoveAndSlide();
	}
}
