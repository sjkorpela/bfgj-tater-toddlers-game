using Godot;

namespace Tater.Scripts.UI;

public partial class ScoreLabel : Label
{
    private GameManager _gm;
    public override void _Ready()
    {
        _gm = Global.Instance.GameManager;
    }

    public override void _Process(double delta)
    {
        this.Text = "Score: " + _gm.Score;
    }
}