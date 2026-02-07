using Godot;

namespace Tater.Scripts.UI;

public partial class ScoreLabel : Label
{
    [ExportCategory("Node References")]
    [Export] private GameManager _gameManager;
    public override void _Process(double delta)
    {
        this.Text = "Score: " + _gameManager.Score;
    }
}