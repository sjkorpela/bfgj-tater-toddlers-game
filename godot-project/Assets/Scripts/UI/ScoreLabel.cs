using Godot;

namespace Tater.Scripts.UI;

public partial class ScoreLabel : Label
{
    private GameManager _gm;
    private int _score;
    private int _displayScore;
    
    public override void _EnterTree()
    {
        _gm = Global.Instance.GameManager;
        _score = _gm.Score;
        _displayScore = _gm.Score;
        this.Text = "S c o r e : " + _displayScore;
    }

    public override void _Process(double delta)
    {
        _score = _gm.Score;
        int diff = _score - _displayScore;
        if (diff <= 10) _displayScore = _score;
        else _displayScore += (int)(diff * 0.1f);
        this.Text = "S c o r e : " + _displayScore;
    }
}