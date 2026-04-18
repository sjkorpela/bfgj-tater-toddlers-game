using Godot;

namespace Tater.Scripts;

public partial class Global : Node
{
    private static Global _instance;
    public static Global Instance => _instance;

    public override void _EnterTree()
    {
        _instance = this;
    }

    public GameManager GameManager
    {
        get;
        private set;
    }

    public void SetGameManager(GameManager gameManager)
    {
        GD.Print("Global: Assigning GameManager");
        GameManager = gameManager;
    }
}