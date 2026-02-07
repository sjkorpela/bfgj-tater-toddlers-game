using Godot;
using System;
using System.Collections.Generic;
using Tater.Scripts.Static;

namespace Tater.Scripts;

public partial class AttackShape : Line2D
{

	private double _age = 0f;
	private Shape _shape;
	public Shape Shape
	{
		get => _shape;
		set => _shape = value;
	}
	
	private Polygon2D _area;
	public Polygon2D Area
	{
		get => _area;
		set => _area = value;
	}

	public void Initialize(Shape shape, List<Vector2> points)
	{
		this._shape = shape;
		this.Points = points.ToArray();
		_area.Polygon = points.ToArray();
	}
	

	private double _maxAge = 1f;
	public override void _Ready()
	{
		this.Closed = true;
		_area = new Polygon2D();
		_area.Visible = false;
		this.AddChild(_area);
	}

	public override void _Process(double delta)
	{
		if (_age > _maxAge)
		{
			this.QueueFree();
		}
		_age += delta;
	}
}
