using System.Linq;
using Godot;

public partial class RegionArea : Area2D
{

    [Export]
    public string _regionId = "";

    [Export]
    public string _regionName = "";

    [Export]
    public string _owner = "";

    [Export]
    public Color _color;    // country color, TODO: update if owner change


    [Export]
    private StaticData _staticData;

    static StringName _inputMouseLeft = new StringName("mouse_left");

    public override void _Ready()
    {
        _staticData = GetNode<StaticData>("/root/StaticData");
        var countriesData = _staticData.countriesData;
        var countryData = countriesData.FirstOrDefault(x => x.Key == _owner).Value;
        countryData.TryGetValue("color", out var color);
        _color = new Color(color);
    }

    public void OnInputEventSignal(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.IsActionPressed(_inputMouseLeft))
            {
                GD.Print($"Clicked RegionArea: {_regionId}, {_regionName}, {_owner}");
            }
        }
    }

    public void OnMouseEntered()
    {
        foreach (Node child in GetChildren())
        {
            if (child is Polygon2D)
            {
                var polygon = child as Polygon2D;
                polygon.Color = new Color(1,1,1);
            }
        }
    }

    public void OnMouseExited()
    {
        foreach (Node child in GetChildren())
        {
            if (child is Polygon2D)
            {
                var polygon = child as Polygon2D;
                polygon.Color = _color;
            }
        }
    }

    // Emitted when the child node enters the SceneTree
    public void OnChildEnteredTree(Node node)
    {
        if (node is Polygon2D)
        {
            var polygon = node as Polygon2D;
            polygon.Color = _color;
            polygon.Name = "polygon: " + _regionName;
        }
    }
}
