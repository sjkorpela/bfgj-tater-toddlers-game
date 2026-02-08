using Godot;

namespace Tater.Scripts.UI;

public partial class GameStateButton : Button
{
    [ExportCategory("Node References")]
    [Export] private GameManager _gameManager;
    [Export] private GameState _targetState;
    public override void _Ready()
    {
        this.Pressed += _setGameState;
    }

    private void _setGameState()
    {
        _gameManager.GameState = _targetState;
    }
}