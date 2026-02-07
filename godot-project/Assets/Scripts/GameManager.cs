using Godot;
using System;

namespace Tater.Scripts;

public enum GameState
{
	Menu,
	GameActive,
	GamePaused,
}

public partial class GameManager : Node
{
	[ExportCategory("Node References")]
	[Export] private PlayerBrain _player;
	public PlayerBrain Player => _player;
	[Export] private EnemyPool _pool;
	[Export] private Camera3D _camera;
	[Export] private ShapeDrawing _draw;

	private GameState _gameState = GameState.GameActive;
	public GameState GameState
	{
		get => _gameState;
		set => _gameState = value;
	}

	public override void _Ready()
	{
		if (_player == null || _pool == null || _camera == null || _draw == null)
		{
			throw new Exception("GameManagerNode is missing node references!");
		}
	}

	public override void _EnterTree()
	{
		_draw.OnDrawingStart += OnDrawingStart;
		_draw.OnCast += OnCast;
	}
	
	public override void _ExitTree()
	{
		_draw.OnDrawingStart -= OnDrawingStart;
		_draw.OnCast -= OnCast;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_gameState == GameState.GameActive)
		{
			_player._BrainPhysicsProcess(delta);
			_pool._BrainProcess(delta);
			_pool._BrainPhysicsProcess(delta);
		}
	}

	public void OnDrawingStart()
	{
		_player.StartCasting();
	}

	public void OnCast(AttackShape cast)
	{
		GD.Print(cast.Shape);
		foreach (EnemyBrain pawn in _pool.Pawns)
		{
			Vector2 onScreen = _camera.UnprojectPosition(pawn.GlobalPosition);
			if (Geometry2D.IsPointInPolygon(onScreen, cast.Points) && pawn.Shape == cast.Shape)
			{
				_pool.DamagePawn(pawn, 1);
			}
		}

		_player.StopCasting();
	}
}
