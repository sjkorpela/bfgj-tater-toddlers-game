using Godot;

namespace Tater.Scripts.UI;

public partial class Heart: TextureRect
{
    [ExportCategory("File References")]
    [Export] private Texture2D _heartRed;
    [Export] private Texture2D _heartGrey;
    
    [ExportCategory("Settings")]
    [Export] private Vector2 _greyScale = new Vector2(0.8f, 0.8f);

    private Vector2 _baseScale;

    private bool _red = true;
    public bool Red => _red;

    public override void _Ready()
    {
        _baseScale = this.Scale;
    }

    public void LoseHeart()
    {
        _red = false;
        this.Texture = _heartGrey;
        this.Scale = _greyScale;
    }

    public void RestoreHeart()
    {
        _red = true;
        this.Texture = _heartRed;
        this.Scale = _baseScale;
    }
}