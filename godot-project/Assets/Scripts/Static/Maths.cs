using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tater.Scripts.Static;

public static class Maths
{
    public static float GetThreePointsAngle(Vector2 a, Vector2 b,Vector2 c)
    {
        Vector2 u = new Vector2(
            a.X - b.X,
            a.Y - b.Y
        ).Normalized();
        Vector2 v = new Vector2(
            c.X - b.X,
            c.Y - b.Y
        ).Normalized();

        float dotProduct = u.Dot(v);

        double radAngle = Math.Acos(dotProduct);
        double degAngle = radAngle * (180f / Math.PI);
        
        // GD.Print(Math.Floor(degAngle), a, b, c);

        return (float)degAngle;
    }
}