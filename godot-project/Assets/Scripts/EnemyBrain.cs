using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using Tater.Scripts.Components;
using Tater.Scripts.Static;

namespace Tater.Scripts;

public partial class EnemyBrain : CharacterBody3D
{
	[ExportCategory("Node References")]
	[Export] private Node3D _target;
	public Node3D Target
	{
		get => _target;
		set => _target = value;
	}
	[Export] private HealthComponent _health;
	public HealthComponent Health => _health;
	[Export] private AnimatedSprite3D _sprite;
	[Export] private CollisionShape3D _collider;
	
	[ExportCategory("Attributes")]
	[Export] private float _speed = 100f;
	[Export] private bool _hasAnimations = false;
	[Export] private Shape _shape;
	public Shape Shape
	{
		get => _shape;
		set => _shape = value;
	}
	
	private Vector3 _hidePosition;

	private bool _active = false;
	public bool Active => _active;

	private bool _stillDying = false;
	public bool StillDying => _stillDying;

	private bool _isFlipped;

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

		if (_hasAnimations)
		{
			_sprite.FlipH = _target.GlobalPosition.X > this.GlobalPosition.X ? !_isFlipped : _isFlipped;
		}
	}

	public void Initialize(Node3D target, Vector3 hidePosition)
	{
		_target = target;
		_hidePosition = hidePosition;
		_health.OnLethalDamage += () => Deactivate();
		if (_hasAnimations)
		{
			_isFlipped = _sprite.IsFlippedH();
		}
	}

	public void Activate(Vector3 startPosition)
	{
		_active = true;
		this.Visible = true;
		this.GlobalPosition = startPosition;
		_health.ResetHealth();
		
		if (_hasAnimations)
		{
			String walkAnimation = AnimationDictionaries.ParseEnemyAnimation.GetValueOrDefault(EnemyAnimation.Walk);
			_sprite.Play(walkAnimation);
		}
	}

	public void Deactivate()
	{
		_active = false;
		_stillDying = true;
		if (_hasAnimations)
		{
			_doDeathVisuals();
		}
		else
		{
			_hide();
		}
	}
	
	public void _doDeathVisuals()
	{
		Thread hurtThread = new Thread(_doDeathAnimationsAndThenHide);
		hurtThread.Start();
	}

	private void _doDeathAnimationsAndThenHide()
	{
		this.CallDeferred(nameof(_playDeathAnimation), null);
		// NTS: the death animation takes two seconds and the collider hangs around for all that time
		// pls fix
		Thread.Sleep(1000);
		this.CallDeferred(nameof(_hide), null);
	}

	public void _playDeathAnimation()
	{
		String deathAnimation = AnimationDictionaries.ParseEnemyAnimation.GetValueOrDefault(EnemyAnimation.Death);
		_sprite.Play(deathAnimation);
	}

	private void _hide()
	{
		this.Visible = false;
		this.GlobalPosition = _hidePosition;
		_stillDying = false;
	}
}
