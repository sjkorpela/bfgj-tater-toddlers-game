using System.Collections.Generic;
using Tater.Scripts.Static;

namespace Tater.Scripts.Static;

public class InputDictionaries
{
    public static readonly Dictionary<InputMap2D, string> ParseInputMap2D = new Dictionary<InputMap2D, string>()
    {
        { InputMap2D.Move, "move" }
    };
}