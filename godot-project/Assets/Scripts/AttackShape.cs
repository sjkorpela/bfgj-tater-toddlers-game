using Godot;
using System;

public partial class AttackShape : Line2D
{

	private double _age = 0f;

	private double _maxAge = 1f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Closed = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_age > _maxAge)
		{
			this.QueueFree();
		}
		_age += delta;
	}
}
