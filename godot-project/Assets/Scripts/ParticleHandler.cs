using Godot;
using System;

namespace Tater.Scripts;

public partial class ParticleHandler : Node
{
	[ExportCategory("Node References")]
	[Export] private CpuParticles3D[] _cpuParticles = [];

	[ExportCategory("Settings")]
	[Export] private bool _emitting = true;
	public bool Emitting
	{
		get { return _emitting; }
		set
		{
			_setState(value);
			_emitting = value;
		}
		
	}

	private void _setState(bool emit)
	{
		foreach (CpuParticles3D part in _cpuParticles)
		{
			part.Emitting = emit;
		}
	}
}
