using Godot;
using System;
using System.Collections.Generic;
using Tater.Scripts.Static;

namespace Tater.Scripts.InputHandlers;

public partial class InputVector2D : Node
{
    [ExportCategory("Settings")]
    [Export] private InputMap2D _axis;
    private String _inputMap;

    private Vector2 _vectorInput;
    public Vector2 InputVector => _vectorInput;

    public override void _Ready()
    {
        _inputMap = InputDictionaries.ParseInputMap2D.GetValueOrDefault(_axis);
    }
	
    public override void _Process(double delta)
    {
        _vectorInput = Input.GetVector(
            $"{_inputMap}_left", 
            $"{_inputMap}_right",
            $"{_inputMap}_up", 
            $"{_inputMap}_down"
        );
    }
}