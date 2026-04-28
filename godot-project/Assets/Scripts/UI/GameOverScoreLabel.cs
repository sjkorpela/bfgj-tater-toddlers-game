using Godot;

namespace Tater.Scripts.UI;

public partial class GameOverScoreLabel : Label
{
    private GameManager _gm;
    
    public override void _EnterTree()
    {
        _gm = Global.Instance.GameManager;
        this.Text = "S c o r e : " + _gm.Score.ToString();
    }
}