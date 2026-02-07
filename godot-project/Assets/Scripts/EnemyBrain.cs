using Godot;
using System;
using Tater.Scripts.Static;

namespace Tater.Scripts;

public partial class EnemyBrain : CharacterBody3D
{
	public Action OnDestroy {get; set;}
	
	[ExportCategory("Node References")]
	[Export] private Node3D _target;
	public Node3D Target
	{
		get => _target;
		set => _target = value;
	}
	
	[ExportCategory("Attributes")]
	[Export] private float _speed = 100f;

	[Export] private Shape _shape;
	public Shape Shape
	{
		get => _shape;
		set => _shape = value;
	}

	private bool _active = false;
	public bool Active
	{
		get => _active;
		set => _active = value;
	}

	public void _BrainPhysicsProcess(double delta)
	{
		if (_active)
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

	public void Initialize(Node3D target, Vector3 hidePosition)
	{
		_target = target;
		this.GlobalPosition = hidePosition;
		_shape = Shape.Circle;
	}

	public void Activate(Vector3 startPosition)
	{
		_active = true;
		this.Visible = true;
		this.GlobalPosition = startPosition;
	}

	public void Deactivate(Vector3 hidePosition)
	{
		_active = false;
		this.Visible = false;
		this.GlobalPosition = hidePosition;
	}
}
