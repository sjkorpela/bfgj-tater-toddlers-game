using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using Tater.Scripts.Components;
using Tater.Scripts.InputHandlers;
using Tater.Scripts.Static;

namespace Tater.Scripts;

public partial class PlayerBrain : CharacterBody3D
{
	
	[ExportCategory("Node References")]
	[Export] private HealthComponent _health;
	public HealthComponent Health => _health;
	[Export] private InputVector2D _moveInput;
	[Export] private InputButtonBasic _debugInput;
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private ParticleHandler _wandParticles;
	[Export] private MeshInstance3D _mesh;
	[Export] private StandardMaterial3D _hurtMaterial;
	
	[ExportCategory("Attributes")]
	[Export] private float _speed = 400f;

	private bool _casting = false;
	public bool Casting => _casting;

	private bool _hurting = false;
	public bool Hurting => _hurting;


	public override void _Ready()
	{
		if (_moveInput == null || _debugInput == null || _animationPlayer == null || _wandParticles == null || _mesh == null || _hurtMaterial == null)
		{
			throw new Exception("PlayerBrain is missing node references!");
		}
	}

	public override void _EnterTree()
	{
		_debugInput.OnPress += _debugFunction;
		_health.OnTakingDamage += DoHurtVisuals;
		_health.OnLethalDamage += DoDeathVisuals;
	}
	
	public override void _ExitTree()
	{
		_debugInput.OnPress -= _debugFunction;
		_health.OnTakingDamage -= DoHurtVisuals;
	}

	public void StartCasting()
	{
		_casting = true;
		_wandParticles.Emitting = true;
	}
	
	public void StopCasting()
	{
		_casting = false;
		_wandParticles.Emitting = false;
	}

	public void DoHurtVisuals()
	{
		Thread hurtThread = new Thread(_hurtVisualsThread);
		hurtThread.Start();
	}

	private void _hurtVisualsThread()
	{
		_hurting = true;
		_mesh.MaterialOverlay = _hurtMaterial;
		Thread.Sleep(500);
		_mesh.MaterialOverlay = null;
		_hurting = false;
	}

	public void DoDeathVisuals()
	{
		Thread deathThread = new Thread(_deathVisualsThread);
		deathThread.Start();
	}
	
	private void _deathVisualsThread()
	{
		_mesh.MaterialOverlay = _hurtMaterial;
		this.CallDeferred(nameof(_playDeathAnimation), 1);
		Thread.Sleep(2000);
		this.CallDeferred(nameof(_playDeathAnimation), -1);
		_mesh.MaterialOverlay = null;
	}

	private void _playDeathAnimation(int fuck)
	{
		this.Rotate(Vector3.Forward, 1.57f * fuck);
	}
	

	private void _debugFunction()
	{
		GD.Print("debug function!");
		_health.TakeDamage(1);
	}

	public void _BrainPhysicsProcess(double delta)
	{
		Vector3 input = new Vector3(
			_moveInput.InputVector.X,
			0f,
			_moveInput.InputVector.Y
		);
		this.Velocity = input.Normalized() * _speed * (float)delta * (_casting ? 0.5f : 1f);
		
		this.MoveAndSlide();
		
		
		// Animation/Visuals logic
		
		// Animation
		if (_casting)
		{
			String castAnimation = AnimationDictionaries.ParseWizardAnimation.GetValueOrDefault(WizardAnimation.Casting);
			if (_animationPlayer.AssignedAnimation != castAnimation)
			{
				_animationPlayer.Play(castAnimation);
			}
		} else if (!input.IsZeroApprox())
		{
			String moveAnimation = AnimationDictionaries.ParseWizardAnimation.GetValueOrDefault(WizardAnimation.Moving);
			if (_animationPlayer.AssignedAnimation != moveAnimation)
			{
				_animationPlayer.Play(moveAnimation);
			}
		} else
		{
			_animationPlayer.Play(AnimationDictionaries.ParseWizardAnimation.GetValueOrDefault(WizardAnimation.Idle));
		}

		// Facing direction
		if (!input.IsZeroApprox())
		{
			this.LookAt(this.Position + -input);
		}
	}
}
