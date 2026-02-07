using Godot;
using System;

namespace Tater.Scripts;

public partial class EnemyBrain : CharacterBody3D
{
	public Action OnDestroy {get; set;}
	
	[ExportCategory("Node References")]
	[Export] private Node3D _target;
	
	[ExportCategory("Attributes")]
	[Export] private float _speed = 100f;

	public Node3D Target
	{
		get => _target;
		set => _target = value;
	}
	
	public void _BrainPhysicsProcess(double delta)
	{
		Vector3 toTarget = new Vector3(
			_target.GlobalPosition.X - this.GlobalPosition.X,
			0f,
			_target.GlobalPosition.Z - this.GlobalPosition.Z
		).Normalized();
		this.Velocity = toTarget * _speed * (float)delta;
		
		this.MoveAndSlide();
	}

	public void Destroy()
	{
		OnDestroy.Invoke();
		this.QueueFree();
	}
}
