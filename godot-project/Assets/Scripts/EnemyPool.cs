using Godot;
using System;
using System.Collections.Generic;
using Tater.Scripts.Components;

namespace Tater.Scripts;

public partial class EnemyPool : Node
{
	[Signal] public delegate void OnPawnKillEventHandler(int value);
	
	[ExportCategory("Node References")]
	[Export] private PackedScene[] _enemyTypes;
	[Export] private Node3D _target;
	[Export] private Node3D _spawnLocation;
	[Export] private Node3D _hideLocation;
	
	[ExportCategory("Attributes")]
	[Export] private int _maxTotalAmount = 50;
	private int _totalAmount = 0;
	[Export] private double _timeBetweenSpawns = 1f;
	private double _timeSinceLastSpawn = 0f;

	[ExportCategory("Settings")]
	[Export] private int minX = -31;
	[Export] private int maxX = 31;
	[Export] private int minZ = -25;
	[Export] private int maxZ = 25;
	
	[ExportCategory("Sound Nodes")]
	[Export] private RandomAudioStreamPlayer2D _shapeDeathSound;
	[Export] private RandomAudioStreamPlayer2D _shapeSpawnSound;

	private int _deathSoundsToDo = 0;
	private int _spawnSoundsToDo = 0;
	
	private List<EnemyBrain> _pawns = [];
	public List<EnemyBrain> Pawns => _pawns;

	private Random _random = new Random();


	public override void _Ready()
	{
		if (_enemyTypes == null || _target == null || _spawnLocation == null || _hideLocation == null)
		{
			throw new Exception("EnemyPool is missing node references!");
		}
	}
	
	public void _BrainProcess(double delta)
	{
		// Fill up 
		if (_totalAmount < _maxTotalAmount)
		{
			_instantiateNewPawn();
		}

		if (_timeSinceLastSpawn >= _timeBetweenSpawns)
		{
			ActivatePawn();
			_timeSinceLastSpawn = 0f;
		}

		_timeSinceLastSpawn += delta;

		if (_deathSoundsToDo > 0)
		{
			_shapeDeathSound.PlayRandomSound();
			_deathSoundsToDo--;
		}
		
		if (_spawnSoundsToDo > 0)
		{
			_shapeSpawnSound.PlayRandomSound();
			_spawnSoundsToDo--;
		}
	}

	public void _BrainPhysicsProcess(double delta)
	{
		foreach (EnemyBrain pawn in _pawns)
		{
			pawn._BrainPhysicsProcess(delta);
		}
	}

	private void _instantiateNewPawn()
	{
		EnemyBrain temp = _enemyTypes[_random.Next(0, 3)].Instantiate<EnemyBrain>();
		this.AddChild(temp);
		_totalAmount++;
		_pawns.Add(temp);
		temp.Initialize(_target, _hideLocation.GlobalPosition);
		temp.Health.OnLethalDamage += _pawnDied;
	}

	private void _pawnDied()
	{
		EmitSignalOnPawnKill(100);
		_deathSoundsToDo++;
	}

	public void ActivatePawn()
	{
		foreach (EnemyBrain pawn in _pawns)
		{
			if (!pawn.Active && !pawn.StillDying)
			{
				pawn.Activate(_spawnLocation.GlobalPosition);
				_spawnSoundsToDo++;
				break;
			}
		}
	}

	public void DamagePawn(EnemyBrain pawn, int damage)
	{
		pawn.Health.TakeDamage(damage);
	}

	public void DeactivatePawn(EnemyBrain pawn)
	{
		pawn.Deactivate();
	}

	public void DeactivateAllPawns()
	{
		foreach (EnemyBrain pawn in _pawns)
		{
			pawn.Deactivate();
		}
	}
}
