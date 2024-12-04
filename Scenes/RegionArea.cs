using Godot;

public partial class RegionArea : Area2D
{

    [Export]
    public string regionId = "";

    [Export]
    public string regionName = "";

    [Export]
    public string owner = "";

    static StringName _inputMouseLeft = new StringName("mouse_left");

    public void OnInputEventSignal(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.IsActionPressed(_inputMouseLeft))
            {
                GD.Print($"Clicked RegionArea: {regionId}, {regionName}, {owner}");
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
                polygon.Color = new Color(1, 1, 1, 1);
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
                polygon.Color = new Color(1, 1, 1, 0.5f);
            }
        }
    }

    // Emitted when the child node enters the SceneTree
    public void OnChildEnteredTree(Node node)
    {
        if (node is Polygon2D)
        {
            var polygon = node as Polygon2D;
            polygon.Color = new Color(1, 1, 1, 0.5f);
            polygon.Name = "polygon: " + regionName;
        }
    }
}
