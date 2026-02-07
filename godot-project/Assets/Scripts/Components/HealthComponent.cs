using Godot;
using System;

namespace Tater.Scripts.Components;

public partial class HealthComponent : Node
{
	[Signal] public delegate void OnTakingDamageEventHandler();
	[Signal] public delegate void OnHealthAtZeroEventHandler();
	
	[ExportCategory("Attributes")]
	[Export] private int _health = 3;

	public int Health => _health;

	public void TakeDamage(int damage)
	{
		if (damage >= _health)
		{
			_health = 0;
			EmitSignalOnHealthAtZero();
		}
		else
		{
			_health -= damage;
			EmitSignalOnTakingDamage();
		}
	}
}
