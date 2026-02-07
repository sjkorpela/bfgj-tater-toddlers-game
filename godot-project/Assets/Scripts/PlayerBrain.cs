using Godot;
using System;
using System.Collections.Generic;
using Tater.Scripts.InputHandlers;
using Tater.Scripts.Static;

namespace Tater.Scripts;

public partial class PlayerBrain : CharacterBody3D
{
	
	[ExportCategory("Node References")]
	[Export] private InputVector2D _moveInput;
	[Export] private AnimationPlayer _animationPlayer;
	
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
		Vector3 input = new Vector3(
			_moveInput.InputVector.X,
			0f,
			_moveInput.InputVector.Y
		);
		this.Velocity = input.Normalized() * _speed * (float)delta;

		if (!input.IsZeroApprox())
		{
			// very long line of text to not deal with string in middle of code :P
			String moveAnimation = AnimationDictionaries.ParseAnimation.GetValueOrDefault(WizardAnimation.Moving);
			if (_animationPlayer.AssignedAnimation != moveAnimation)
			{
				_animationPlayer.Play(moveAnimation);
			}
			
			this.LookAt(this.Position + -input);
		}
		else
		{
			_animationPlayer.Play(AnimationDictionaries.ParseAnimation.GetValueOrDefault(WizardAnimation.Idle));
		}
		
		
		this.MoveAndSlide();
	}
}
