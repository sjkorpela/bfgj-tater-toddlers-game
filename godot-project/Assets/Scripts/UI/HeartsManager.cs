using Godot;

namespace Tater.Scripts.UI;

public partial class HeartsManager: Control
{
    [ExportCategory("Node References")]
    [Export] private Heart[] _hearts;

    private GameManager _gm;
    private PlayerBrain _player;

    private int _hp;

    public override void _EnterTree()
    {
        _gm = Global.Instance.GameManager;
        _player = _gm.Player;
        _hp = _player.Health.MaxHealth;

        _gm.OnGameReset += _onReset;
        _player.Health.OnTakingDamage += _onDamage;
        _player.Health.OnLethalDamage += _onDamage;
    }

    public override void _ExitTree()
    {
        _gm.OnGameReset -= _onReset;
        _player.Health.OnTakingDamage -= _onDamage;
        _player.Health.OnLethalDamage -= _onDamage;
    }

    private void _onReset()
    {
        _hp = _player.Health.MaxHealth;
        foreach (Heart heart in _hearts)
        {
            heart.RestoreHeart();
        }
    }

    private void _onDamage()
    {
        int diff = _hp - _player.Health.Health;
        _hp -= diff;
        LoseHearts(diff);
    }

    private void LoseHearts(int count)
    {
        for (int i = 0; i < count; i++)
        {
            for (int j = _hearts.Length - 1; j >= 0; j--)
            {
                if (_hearts[j].Red)
                {
                    _hearts[j].LoseHeart();
                    break;
                }
            }
        }
    }
}