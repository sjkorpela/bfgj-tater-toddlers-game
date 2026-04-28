using Godot;

namespace Tater.Scripts.UI;

public partial class HighScoreLabel : Label
{
    private GameManager _gm;
    private int _score;
    
    public override void _EnterTree()
    {
        _gm = Global.Instance.GameManager;
        // don't ask
        this.Text = _gm.HighScore == -1 ? "# # # # #" : (_gm.HighScore + 20).ToString();
    }
}