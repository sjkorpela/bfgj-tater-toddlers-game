using Godot;
using System;
using System.Collections.Generic;

namespace Tater.Scripts;

public partial class PlayerClickTest1 : Node
{
	private float _timeBetweenDots = 0.01f;
	private double _timeSinceLastDot = 0f;

	private bool _shapeActive = false;
	private readonly List<Vector2> _dots = [];
	private Line2D _line;

	public override void _Ready()
	{
		_line = new Line2D();
		this.AddChild(_line);
	}


	public override void _Process(double delta)
	{
		if (_shapeActive && !Input.IsMouseButtonPressed(MouseButton.Left))
		{
			GD.Print("end drawing shape!");
			_shapeActive = false;
			_createShape(_dots);
			_dots.Clear();
			_line.Points = [];
		}

		if (!_shapeActive && Input.IsMouseButtonPressed(MouseButton.Left))
		{
			GD.Print("start drawing shape!");
			_shapeActive = true;
		}

		if (_shapeActive)
		{
			if (_timeSinceLastDot >= _timeBetweenDots)
			{
				_addNewDot();
				_timeSinceLastDot = 0f;
				_line.Points = _dots.ToArray();
			}

			_timeSinceLastDot += delta;
		}
	}

	private void _addNewDot()
	{
		_dots.Add(new Vector2(
			GetViewport().GetMousePosition().X,
			GetViewport().GetMousePosition().Y
		));
		GD.Print("new dot at: " + GetViewport().GetMousePosition().X + ", " + GetViewport().GetMousePosition().Y);
	}

	private void _createShape(List<Vector2> dots)
	{
		foreach (Vector2 dot in dots) { GD.Print(dot);}

		AttackShape temp = new AttackShape();
		this.AddChild(temp);
		temp.Points = dots.ToArray();
		
		GD.Print("drew shape: " + temp + ", " + dots.Count);
	}
}
