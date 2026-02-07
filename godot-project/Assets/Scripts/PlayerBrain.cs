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
	[Export] private InputButtonBasic _debugInput;
	[Export] private AnimationPlayer _animationPlayer;
	
	[ExportCategory("Attributes")]
	[Export] private float _speed = 400f;

	private bool _casting = false;
	public bool Casting
	{
		get => _casting;
		set => _casting = value;
	}


	public override void _Ready()
	{
		if (_moveInput == null || _debugInput == null || _animationPlayer == null)
		{
			throw new Exception("PlayerBrain is missing node references!");
		}
	}

	public override void _EnterTree()
	{
		_debugInput.OnPress += _debug_function;
	}
	
	public override void _ExitTree()
	{
		_debugInput.OnPress -= _debug_function;
	}

	private void _debug_function()
	{
		GD.Print("debug function!");
	}

	public void _BrainPhysicsProcess(double delta)
	{
		Vector3 input = new Vector3(
			_moveInput.InputVector.X,
			0f,
			_moveInput.InputVector.Y
		);
		this.Velocity = input.Normalized() * _speed * (float)delta;
		
		this.MoveAndSlide();
		
		
		// Animation logic
		if (_casting)
		{
			String castAnimation = AnimationDictionaries.ParseAnimation.GetValueOrDefault(WizardAnimation.Casting);
			if (_animationPlayer.AssignedAnimation != castAnimation)
			{
				_animationPlayer.Play(castAnimation);
			}
		} else if (!input.IsZeroApprox())
		{
			String moveAnimation = AnimationDictionaries.ParseAnimation.GetValueOrDefault(WizardAnimation.Moving);
			if (_animationPlayer.AssignedAnimation != moveAnimation)
			{
				_animationPlayer.Play(moveAnimation);
			}
		} else
		{
			_animationPlayer.Play(AnimationDictionaries.ParseAnimation.GetValueOrDefault(WizardAnimation.Idle));
		}

		if (!input.IsZeroApprox())
		{
			this.LookAt(this.Position + -input);
		}
	}
}
