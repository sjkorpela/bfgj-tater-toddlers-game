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
	[Export] private AnimationPlayer _rigAnimationPlayer;
	[Export] private AnimationPlayer _hurtAnimationPlayer;
	[Export] private ParticleHandler _wandParticles;
	[Export] private MeshInstance3D _mesh;
	[Export] private Area3D _hitReg;
	[Export] private StandardMaterial3D _hurtMaterial;
	
	[ExportCategory("Sounds")]
	[Export] private AudioStreamPlayer2D _walkSound;
	[Export] private RandomAudioStreamPlayer2D _castSound;
	[Export] private RandomAudioStreamPlayer2D _hurtSound;
	[Export] private RandomAudioStreamPlayer2D _deathSound;
	
	[ExportCategory("Attributes")]
	[Export] private float _speed = 400f;

	
	public bool Casting => _casting;
	public bool Hurting => _hurting;
	public bool Dying => _dying;
	
	private StringName _idleAnimation;
	private StringName _moveAnimation;
	private StringName _castAnimation;
	private StringName _hurtAnimation;
	private StringName _deathAnimation;
	
	private GameManager _gm;
	
	public void _PlayerPhysicsProcess(double delta)
	{
		if (_dying) return;
		
		// Movement logic
		Vector3 input = new Vector3(
			_moveInput.InputVector.X,
			0f,
			_moveInput.InputVector.Y
		);
		this.Velocity = (input.Normalized() * _speed * (float)delta * (_casting ? 0.5f : 1f)) + this.GetGravity();
		this.MoveAndSlide();
		
		
		// Animation/Sound logic
		if (!input.IsZeroApprox())
		{
			this.LookAt(this.Position + -input);
			if (!_walkSound.IsPlaying()) _walkSound.SetPlaying(true);
		}
		if (_casting)
		{
			if (_rigAnimationPlayer.AssignedAnimation != _castAnimation) _rigAnimationPlayer.Play(_castAnimation);
		} else if (!input.IsZeroApprox())
		{
			if (_rigAnimationPlayer.AssignedAnimation != _moveAnimation) _rigAnimationPlayer.Play(_moveAnimation);
		} else
		{
			_walkSound.SetPlaying(false);
			_rigAnimationPlayer.Play(_idleAnimation);
		}
	}
	
	#region EnterAndExitTree

	public override void _EnterTree()
	{
		_gm = Global.Instance.GameManager;
		
		if (_moveInput == null || _debugInput == null || _rigAnimationPlayer == null || _wandParticles == null || _hurtMaterial == null)
		{
			throw new Exception("PlayerBrain is missing node references!");
		}

		_idleAnimation = AnimationDictionaries.ParseWizardAnimation.GetValueOrDefault(WizardAnimation.Idle);
		_moveAnimation = AnimationDictionaries.ParseWizardAnimation.GetValueOrDefault(WizardAnimation.Moving);
		_castAnimation = AnimationDictionaries.ParseWizardAnimation.GetValueOrDefault(WizardAnimation.Casting);
		_hurtAnimation = "extra/HurtFlash";
		_deathAnimation = AnimationDictionaries.ParseWizardAnimation.GetValueOrDefault(WizardAnimation.Dying);
		
		_debugInput.OnPress += _debugFunction;
		_rigAnimationPlayer.AnimationFinished += _onAnimationFinish;
		_hurtAnimationPlayer.AnimationFinished += _onAnimationFinish;
		_health.OnTakingDamage += _onHurt;
		_health.OnLethalDamage += _onDied;
		_gm.OnGameReset += _onGameReset;
		_hitReg.Monitoring = true;
		_hitReg.BodyEntered += _onBodyEntered;
	}
	
	public override void _ExitTree()
	{
		_debugInput.OnPress -= _debugFunction;
		_rigAnimationPlayer.AnimationFinished -= _onAnimationFinish;
		_hurtAnimationPlayer.AnimationFinished -= _onAnimationFinish;
		_health.OnTakingDamage -= _onHurt;
		_health.OnLethalDamage -= _onDied;
		_gm.OnGameReset -= _onGameReset;
	}
	
	private void _debugFunction()
	{
		GD.Print("debug function!");
		// _health.TakeDamage(1);
	}

	private void _onAnimationFinish(StringName anim)
	{
		if (anim == _hurtAnimation)
		{
			_hurting = false;
			return;
		}

		if (anim == _deathAnimation)
		{
			_gm.GameEnd();
		}
	}

	private void _onGameReset()
	{
		_health.ResetHealth();
		_rigAnimationPlayer.Play(_idleAnimation);
		_dying = false;
	}

	#endregion



	#region Collision

	private void _onBodyEntered(Node3D node)
	{
		if (_hurting || _dying) return;

		if (node.IsInGroup("enemy") && node.GetType() == typeof(EnemyBrain))
		{
			EnemyBrain brain = (EnemyBrain)node;
			_health.TakeDamage(1);
			brain.StartDying();
		}
	}

	#endregion

	
	
	#region HurtingAndDying
	
	private bool _hurting = false;
	private bool _dying = false;

	private void _onHurt()
	{
		_hurting = true;
		_hurtAnimationPlayer.Play(_hurtAnimation);
		_hurtSound.PlayRandomSound();
	}
	
	public void _onDied()
	{
		_dying = true;
		_rigAnimationPlayer.Play(_deathAnimation);
		_deathSound.Play();
	}

	#endregion


	
	#region Casting
	
	private bool _casting = false;

	public void StartCasting()
	{
		_casting = true;
		_wandParticles.Emitting = true;
	}
	
	public void StopCasting()
	{
		_castSound.PlayRandomSound();
		_casting = false;
		_wandParticles.Emitting = false;
	}

	#endregion
}
