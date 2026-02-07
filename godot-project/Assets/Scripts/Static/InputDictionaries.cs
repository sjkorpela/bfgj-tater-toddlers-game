using System.Collections.Generic;
using Tater.Scripts.Static;

namespace Tater.Scripts.Static;

public class InputDictionaries
{
    public static readonly Dictionary<InputMapButton, string> ParseInputMapButton = new Dictionary<InputMapButton, string>()
    {
        { InputMapButton.Space, "space" }
    };
    
    public static readonly Dictionary<InputMap2D, string> ParseInputMap2D = new Dictionary<InputMap2D, string>()
    {
        { InputMap2D.Move, "move" }
    };
}