using Godot;
using System;
using Tater.Scripts;

namespace Tater.Scripts;

public partial class GameManagerNode : Node
{
	[ExportCategory("Node References")]
	[Export] private PlayerBrain _player;

	public PlayerBrain Player => _player;

	public override void _Ready()
	{
		if (_player == null)
		{
			throw new Exception("GameManagerNode is missing node references!");
		}
	}
    
	public override void _PhysicsProcess(double delta)
	{
		_player._BrainPhysicsProcess(delta);
	}
}
