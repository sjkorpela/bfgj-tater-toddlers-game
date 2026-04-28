using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using Tater.Scripts.Components;
using Tater.Scripts.Static;

namespace Tater.Scripts;

public partial class EnemyBrain : CharacterBody3D
{
	[Signal] public delegate void OnPawnDiedEventHandler(int score);
	
	[ExportCategory("Node References")]
	[Export] private HealthComponent _health;
	public HealthComponent Health => _health;
	[Export] private AnimatedSprite3D _sprite;
	[Export] private CollisionShape3D _collider;
	
	[ExportCategory("Attributes")]
	[Export] private float _speed = 100f;
	[Export] private int _score = 100;
	[Export] private Shape _shape;
	public Shape Shape => _shape;
	
	private Node3D _target;
	private Vector3 _hidePosition = new Vector3(-100, -100, -100);

	private bool _active = false;
	public bool Active => _active;

	private bool _currentlyDying = false;
	public bool CurrentlyDying => _currentlyDying;

	private bool _isFlipped;

	private StringName _walkAnimation;
	private StringName _deathAnimation;

	private GameManager _gm;

	public override void _EnterTree()
	{
		_gm = Global.Instance.GameManager;
		
		if (_sprite == null)
		{
			throw new Exception("EnemyBrain is missing node references!");
		}
		
		_walkAnimation = AnimationDictionaries.ParseEnemyAnimation.GetValueOrDefault(EnemyAnimation.Walk);
		_deathAnimation = AnimationDictionaries.ParseEnemyAnimation.GetValueOrDefault(EnemyAnimation.Death);

		_sprite.AnimationFinished += _onAnimationFinished;
		_health.OnLethalDamage += StartDying;

		_target = _gm.Player;
		_isFlipped = _sprite.IsFlippedH();
		Deactivate();
	}

	public override void _ExitTree()
	{
		_sprite.AnimationFinished -= _onAnimationFinished;
		_health.OnLethalDamage -= StartDying;
	}
	
	private void _onAnimationFinished()
	{
		StringName anim = _sprite.Animation;
		if (anim == _deathAnimation)
		{
			_currentlyDying = false;
			Deactivate();
		}
	}
	
	public void StartDying()
	{
		_active = false;
		_currentlyDying = true;
		_collider.Disabled = true;
		_sprite.Play(_deathAnimation);
		
		if (_gm.GameState == GameState.GameActive) _gm.AddScore(_score);
		
		EmitSignalOnPawnDied(_score);
	}

	
	
	public void Activate(Vector3 startPosition)
	{
		_active = true;
		this.Visible = true;
		this.GlobalPosition = startPosition;
		_collider.Disabled = false;
		
		_health.ResetHealth();
		_sprite.Play(_walkAnimation);
	}

	private void Deactivate()
	{
		_active = false;
		this.Visible = false;
		this.GlobalPosition = _hidePosition;
		_collider.Disabled = true;
	}
	
	
	
	public void _PawnPhysicsProcess(double delta)
	{
		if (_active)
		{
			Vector3 toTarget = new Vector3(
				_target.GlobalPosition.X - this.GlobalPosition.X,
				0f,
				_target.GlobalPosition.Z - this.GlobalPosition.Z
			).Normalized();
			this.Velocity = (toTarget * _speed * (float)delta) + this.GetGravity();
			this.MoveAndSlide();
			_sprite.FlipH = _target.GlobalPosition.X > this.GlobalPosition.X ? !_isFlipped : _isFlipped;
		}
	}
	
}
