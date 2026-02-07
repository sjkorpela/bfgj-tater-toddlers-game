using Godot;
using System;
using Tater.Scripts.InputHandlers;

namespace Tater.Scripts;

public partial class PlayerBrain : CharacterBody3D
{
	
	[ExportCategory("Node References")]
	[Export] private InputVector2D _moveInput;
	
	[ExportCategory("Attributes")]
	[Export] private float _speed = 400f;
	
	
	public override void _Ready()
	{
		if (_moveInput == null)
		{
			throw new Exception("PlayerBrain has no InputVector2D!");
		}
	}
	
	public void _BrainPhysicsProcess(double delta)
	{
		this.Velocity = new Vector3(
			_moveInput.InputVector.X * _speed * (float)delta,
			0f,
			_moveInput.InputVector.Y * _speed * (float)delta
		);
		
		this.MoveAndSlide();
	}
}
