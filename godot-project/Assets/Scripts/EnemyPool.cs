using Godot;
using System;
using System.Collections.Generic;

namespace Tater.Scripts;

public partial class EnemyPool : Node
{
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
	
	private List<EnemyBrain> _pawns = [];
	public List<EnemyBrain> Pawns
	{
		get => _pawns;
		set => _pawns = value;
	}

	private Random _random = new Random();


	public override void _Ready()
	{
		if (_enemyTypes == null || _target == null || _spawnLocation == null)
		{
			throw new Exception("EnemyPool is missing node references!");
		}
	}
	
	public void _BrainProcess(double delta)
	{
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
	}

	public void ActivatePawn()
	{
		foreach (EnemyBrain pawn in _pawns)
		{
			if (!pawn.Active)
			{
				pawn.Activate(_spawnLocation.GlobalPosition);
				break;
			}
		}
	}

	public void DeactivatePawn(EnemyBrain pawn)
	{
		pawn.Deactivate(_hideLocation.GlobalPosition);
	}

	private void _destroyAllPawns()
	{
		foreach (EnemyBrain pawn in _pawns)
		{
			pawn.Deactivate(_hideLocation.GlobalPosition);
		}
	}

	public void CleanList()
	{
		for (int i = 0; i < _pawns.Count; i++)
		{
			if (_pawns[i] == null)
			{
				_pawns.RemoveAt(i);
				_totalAmount--;
			}
		}
	}
}
