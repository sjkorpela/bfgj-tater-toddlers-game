using Godot;

namespace Tater.Scripts.UI;

public partial class AudioBusSlider : HSlider
{
    public override void _Ready()
    {
        this.Value = AudioServer.GetBusVolumeDb(0) + (float)this.MaxValue;
    }

    public override void _Process(double delta)
    {
        float volume = (float)this.Value - (float)this.MaxValue;
        AudioServer.SetBusVolumeDb(0, volume < -24 ? -99 : volume);
    }
}