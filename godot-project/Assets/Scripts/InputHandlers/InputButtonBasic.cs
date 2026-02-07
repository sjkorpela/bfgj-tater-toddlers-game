using Tater.Scripts.Static;

namespace Tater.Scripts.InputHandlers;

using System;
using System.Collections.Generic;
using Godot;

public partial class InputButtonBasic : Node
{
    public Action OnPress {get; set;}

    [ExportCategory("Settings")]
    [Export] private InputMapButton _button;

    private String _inputMap;
    
    private bool _buttonHeld = false;

    public override void _Ready()
    {
        base._Ready();

        _inputMap = InputDictionaries.ParseInputMapButton.GetValueOrDefault(_button);
    }

    public override void _Process(double delta)
    {
        if (!_buttonHeld && Input.IsActionJustPressed(_inputMap))
        {
            OnPress?.Invoke();
            _buttonHeld = true;
        } else if (Input.IsActionJustReleased(_inputMap))
        {
            _buttonHeld = false;
        }
    }
}
