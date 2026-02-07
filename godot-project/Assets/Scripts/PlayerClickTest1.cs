using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Tater.Scripts.Static;

namespace Tater.Scripts;

public partial class PlayerClickTest1 : Node
{
	private float _timeBetweenDots = 0.01f;
	private double _timeSinceLastDot = 0f;
	
	private float _timeBetweenVisualDots = 0.01f;
	private double _timeSinceLastVisualDot = 0f;

	private bool _shapeActive = false;
	private readonly List<Vector2> _dots = [];
	private Line2D _visualLine;
	private readonly List<Vector2> _visualDots = [];

	public override void _Ready()
	{
		_visualLine = new Line2D();
		this.AddChild(_visualLine);
	}


	public override void _Process(double delta)
	{
		if (_shapeActive && !Input.IsMouseButtonPressed(MouseButton.Left))
		{
			GD.Print("end drawing shape!");
			_shapeActive = false;
			_createShape(_dots);
			_dots.Clear();
			_visualDots.Clear();
			_visualLine.Points = [];
		}

		if (!_shapeActive && Input.IsMouseButtonPressed(MouseButton.Left))
		{
			GD.Print("start drawing shape!");
			_shapeActive = true;
			_addNewDot();
		}

		if (_shapeActive)
		{
			if (_timeSinceLastDot >= _timeBetweenDots)
			{
				_addNewDot();
				_timeSinceLastDot = 0f;
				_visualLine.Points = _dots.ToArray();
			}
			
			if (_timeSinceLastVisualDot >= _timeBetweenVisualDots)
			{
				_addNewVisualDot();
				_timeSinceLastVisualDot = 0f;
				_visualLine.Points = _visualDots.ToArray();
			}

			_timeSinceLastDot += delta;
			_timeSinceLastVisualDot += delta;
		}
	}

	private void _addNewDot()
	{
		_dots.Add(new Vector2(
			GetViewport().GetMousePosition().X,
			GetViewport().GetMousePosition().Y
		));
	}
	
	private void _addNewVisualDot()
	{
		_visualDots.Add(new Vector2(
			GetViewport().GetMousePosition().X,
			GetViewport().GetMousePosition().Y
		));
	}

	private void _createShape(List<Vector2> dots)
	{
		// foreach (Vector2 dot in dots) { GD.Print(dot);}

		List<Vector2> rounded = [];

		foreach (Vector2 dot in dots)
		{
			rounded.Add(new Vector2(
				_snapPointToGrid(dot.X),
				_snapPointToGrid(dot.Y)
			));
		}

		int straights = 0;
		int corners = 0;
		int others = 0;
		String last = "";

		// MATH :[
		for (int i = 0; i < dots.Count; i++)
		{
			float degAngle;
			
			if (i == 0) { degAngle = Maths.GetThreePointsAngle(dots.Last(), dots[i], dots[i + 1]); }
			else if (i == dots.Count - 1) { degAngle = Maths.GetThreePointsAngle(dots[i - 1], dots[i], dots[0]); }
			else
			{
				degAngle = Maths.GetThreePointsAngle(dots[i - 1], dots[i], dots[i + 1]);
				if (degAngle >= 150 && last != "s")
				{
					straights++;
					last = "s";
				} else if (degAngle <= 100 && last != "c")
				{
					corners++;
					last = "c";
				}
				else
				{
					others++;
				}
			}

			
		}
		
		GD.Print("s: " + straights, "\nc: " + corners, "\no: " + others);

		if (straights + corners <= 2)
		{
			GD.Print("circle");
		} else if (straights + corners >= 7)
		{
			GD.Print("square");
		}
		else
		{
			GD.Print("triangle");
		}

		AttackShape temp = new AttackShape();
		this.AddChild(temp);
		temp.Points = dots.ToArray();
	}

	private float _snapPointToGrid(float value)
	{
		float snapAmount = 10f;
		return (float)Math.Floor(value / snapAmount) * snapAmount;
	}
}
