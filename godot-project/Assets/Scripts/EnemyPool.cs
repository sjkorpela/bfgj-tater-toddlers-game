using Godot;
using System;
using System.Collections.Generic;
using Tater.Scripts.Components;

namespace Tater.Scripts;

public partial class EnemyPool : Node
{
	[ExportCategory("Node References")]
	[Export] private PackedScene[] _enemyTypes;
	[Export] private Node3D _target;
	[Export] private Node3D _spawnLocation;
	[Export] private RayCast3D _spawnRayCast;
	[Export] private Node3D _hideLocation;
	
	[ExportCategory("Attributes")]
	[Export] private int _maxTotalAmount = 50;
	private int _totalAmount = 0;
	private int _lastType = 0;
	[Export] private double _timeBetweenSpawns = 1f;
	private double _timeSinceLastSpawn = 0f;
	[Export] private float _playerSafeDistance = 10;

	[ExportCategory("Settings")]
	[Export] private int _minX = -31;
	[Export] private int _maxX = 31;
	[Export] private int _minZ = -25;
	[Export] private int _maxZ = 25;
	
	[ExportCategory("Sound Nodes")]
	[Export] private RandomAudioStreamPlayer2D _shapeDeathSound;
	[Export] private RandomAudioStreamPlayer2D _shapeSpawnSound;

	private int _deathSoundsToDo = 0;
	private int _spawnSoundsToDo = 0;
	
	private readonly List<EnemyBrain> _pawns = [];
	public List<EnemyBrain> Pawns => _pawns;

	private readonly Random _random = new Random();

	private GameManager _gm;


	public override void _EnterTree()
	{
		_gm = Global.Instance.GameManager;
		if (
			_enemyTypes == null || 
			_enemyTypes.Length == 0 ||
			_target == null ||
			_spawnLocation == null ||
			_hideLocation == null ||
			_shapeDeathSound == null ||
			_shapeSpawnSound == null
			)
		{
			throw new Exception("EnemyPool is missing references!");
		}
		
		_gm.OnGameReset += _onGameReset;
		_gm.OnGameStateChange += _onStateChange;
	}

	public override void _ExitTree()
	{
		_gm.OnGameReset -= _onGameReset;
		_gm.OnGameStateChange -= _onStateChange;
	}

	public void _PoolPhysicsProcess(double delta)
	{
		// Process pawns
		foreach (EnemyBrain pawn in _pawns)
		{
			pawn._PawnPhysicsProcess(delta);
		}
		
		// Fill up 
		if (_totalAmount < _maxTotalAmount)
		{
			InstantiateNewPawn();
		}

		
		
		// Spawning
		if (_timeSinceLastSpawn >= _timeBetweenSpawns)
		{
			ActivatePawn();
			_timeSinceLastSpawn = 0f;
		}
		_timeSinceLastSpawn += delta;

		
		
		// Spacing out sounds
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

	private void InstantiateNewPawn()
	{
		int type = _lastType + 1;
		if (type == 3) type = 0;
		_lastType = type;
		EnemyBrain temp = _enemyTypes[type].Instantiate<EnemyBrain>();
		
		this.AddChild(temp);
		_totalAmount++;
		_pawns.Add(temp);
		temp.OnPawnDied += _pawnDied;
	}

	private void _pawnDied(int _)
	{
		_deathSoundsToDo++;
	}

	public void ActivatePawn()
	{
		// bad
		for (int i = 0; i < 10; i++)
		{
			Vector3 temp = new Vector3(
				_random.Next(_minX, _maxX),
				_spawnLocation.GlobalPosition.Y,
				_random.Next(_minZ, _maxZ)
			);
			
			_spawnLocation.GlobalPosition = temp;

			GodotObject check = _spawnRayCast.GetCollider();

			if (check != null && check.GetType().IsSubclassOf(typeof(Node3D)))
			{
				Node3D node = (Node3D)check;
				if (!node.IsInGroup("spawn_blocker") && _target.GlobalPosition.DistanceTo(temp) > _playerSafeDistance)
				{
					break;
				}
			} else if (_target.GlobalPosition.DistanceTo(temp) > _playerSafeDistance)
			{
				break;
			}
		}
		
		
		foreach (EnemyBrain pawn in _pawns)
		{
			if (!pawn.Active && !pawn.CurrentlyDying)
			{
				pawn.Activate(_spawnLocation.GlobalPosition);
				_spawnSoundsToDo++;
				break;
			}
		}
	}

	private void _onGameReset()
	{
		foreach (EnemyBrain pawn in _pawns)
		{
			pawn.StartDying();
		}
		_deathSoundsToDo = 0;
		_spawnSoundsToDo = 0;
	}

	private void _onStateChange(GameState newState, GameState oldsState)
	{
		if (newState is GameState.GameActive or GameState.GameOver)
		{
			_shapeDeathSound.StreamPaused = false;
			_shapeSpawnSound.StreamPaused = false;
		}
		else
		{
			_shapeDeathSound.StreamPaused = true;
			_shapeSpawnSound.StreamPaused = true;
		}
	}
}