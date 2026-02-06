using Godot;
using System;
using Tater.Scripts.InputHandlers;

namespace Tater.Scripts;

public partial class PlayerBrain : CharacterBody3D
{
	
	[ExportCategory("Node References")]
	[Export] private InputVector2D _moveInput;
	
	[ExportCategory("Attributes")]
	[Export] private float _speed = 5f;
	
	
	public override void _Ready()
	{
		if (_moveInput == null)
		{
			throw new Exception("PlayerBrain has no InputVector2D!");
		}
	}
	
	public override void _Process(double delta)
	{
		this.Velocity = new Vector3(
				_moveInput.InputVector.X * _speed,
				0f,
				_moveInput.InputVector.Y * _speed
			);
		this.MoveAndSlide();
	}
}
