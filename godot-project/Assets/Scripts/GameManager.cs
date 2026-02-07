using Godot;
using System;
using Tater.Scripts;

namespace Tater.Scripts;

public partial class GameManager : Node
{
	[ExportCategory("Node References")]
	[Export] private PlayerBrain _player;

	public PlayerBrain Player => _player;

	public override void _Ready()
	{
		if (_player == null)
		{
			throw new Exception("GameManager is missing node references!");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		_player._BrainPhysicsProcess(delta);
	}
}
