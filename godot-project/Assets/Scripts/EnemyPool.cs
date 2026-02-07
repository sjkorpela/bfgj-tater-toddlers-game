using Godot;
using System;
using System.Collections.Generic;

namespace Tater.Scripts;

public partial class EnemyPool : Node
{
	[ExportCategory("Node References")]
	[Export] private PackedScene _enemy;

	[Export] private Node3D _target;

	[Export] private Node3D _spawnLocation;
	
	[ExportCategory("Attributes")]
	[Export] private int _maxTotalAmount = 50;

	[Export] private int _timeBetweenSpawns = 2;
	

	private int _totalAmount = 0;
	private double _timeSinceLastSpawn = 0f;
	
	private List<EnemyBrain> _pawns = [];
	
	
	public override void _Ready()
	{
		if (_enemy == null || _target == null || _spawnLocation == null)
		{
			throw new Exception("EnemyPool is missing node references!");
		}
	}
	
	public void _BrainProcess(double delta)
	{
		if (_totalAmount < _maxTotalAmount && _timeSinceLastSpawn >= _timeBetweenSpawns)
		{
			_instantiateNewPawn();
			_timeSinceLastSpawn = 0f;
		}

		_timeSinceLastSpawn += delta;
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
		EnemyBrain temp = _enemy.Instantiate<EnemyBrain>();
		this.AddChild(temp);
		_totalAmount++;
		_pawns.Add(temp);
		temp.Target = _target;
		
		// memory leak?
		temp.OnDestroy += () =>
		{
			_pawns.Remove(temp);
			_totalAmount--;
		};
		
		temp.GlobalPosition = _spawnLocation.GlobalPosition;
	}

	private void _destroyAllPawns()
	{
		foreach (EnemyBrain pawn in _pawns)
		{
			pawn.Destroy();
		}
	}
}
