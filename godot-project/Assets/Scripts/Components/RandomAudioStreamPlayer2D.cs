using System;
using Godot;

namespace Tater.Scripts.Components;

public partial class RandomAudioStreamPlayer2D : AudioStreamPlayer2D
{
    [ExportCategory("Sounds")]
    [Export] private AudioStream[] _audioStreams;
    
    private Random _random = new Random();

    public void PlayRandomSound()
    {
        this.Stream = _audioStreams[_random.Next(0, _audioStreams.Length)];
        this.Play();
    }
}