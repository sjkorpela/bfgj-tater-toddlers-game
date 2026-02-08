using Godot;
using System;

namespace Tater.Scripts.Components;

public partial class HealthComponent : Node
{
	[Signal] public delegate void OnTakingDamageEventHandler();
	[Signal] public delegate void OnLethalDamageEventHandler();
	
	[ExportCategory("Attributes")]
	[Export] private int _maxHealth = 3;

	private bool _dead = false;
	public bool Dead => _dead;


	private int _health = 0;
	public int Health => _health;

	public override void _Ready()
	{
		ResetHealth();
	}

	public void TakeDamage(int damage)
	{
		if (damage >= _health)
		{
			_health = 0;
			EmitSignalOnLethalDamage();
		}
		else
		{
			_health -= damage;
			EmitSignalOnTakingDamage();
		}
	}

	public void ResetHealth()
	{
		_health = _maxHealth;
	}
}
