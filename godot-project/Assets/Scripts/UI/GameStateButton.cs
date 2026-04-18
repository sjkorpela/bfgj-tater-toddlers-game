using Godot;

namespace Tater.Scripts.UI;

public partial class GameStateButton : Button
{
    [ExportCategory("Node References")]
    [Export] private GameState _targetState;

    private GameManager _gm;
    public override void _Ready()
    {
        _gm = Global.Instance.GameManager;
        this.Pressed += _setGameState;
    }

    public override void _ExitTree()
    {
        this.Pressed -= _setGameState;
    }

    private void _setGameState()
    {
        _gm.GameState = _targetState;
    }
}