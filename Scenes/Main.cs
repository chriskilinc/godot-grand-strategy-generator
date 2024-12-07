using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Node2D
{
    PackedScene _regionAreaScene;

    [Export]
    private StaticData _staticData;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        GD.Print("Hello, World!");
        _staticData = GetNode<StaticData>("/root/StaticData");
        _regionAreaScene = GD.Load<PackedScene>("res://Scenes/region_area.tscn");

        GenerateRegions();
    }

    private void GenerateRegions()
    {
        var mapImage = GD.Load<Texture2D>("res://MapData/map.png").GetImage();
        var pixelColorDictionary = GetPixelColorDictionary(mapImage);
        var regions = CreateRegionNode();
        var mapData = _staticData.mapData;

        foreach (KeyValuePair<string, Godot.Collections.Dictionary<string, string>> region in mapData)
        {
            var id = region.Key;
            var regionData = region.Value;

            RegionArea regionArea = _regionAreaScene.Instantiate<RegionArea>();
            regionArea.Name = id;
            regionArea.UniqueNameInOwner = true;

            regionData.TryGetValue("owner", out var owner);
            regionData.TryGetValue("name", out var regionName);

            regionArea._regionId = id;
            regionArea._regionName = regionName;
            regionArea._owner = owner;

            regions.AddChild(regionArea);

            var polygons = GetPolygons(mapImage, region.Key, pixelColorDictionary);

            foreach (Vector2[] polygon in polygons)
            {
                CollisionPolygon2D regionCollisionPolygon = new CollisionPolygon2D();
                Polygon2D regionPolygon = new Polygon2D();

                regionCollisionPolygon.Polygon = polygon;
                regionPolygon.Polygon = polygon;

                regionArea.AddChild(regionPolygon);
                regionArea.AddChild(regionCollisionPolygon);
            }
        }
    }

    private static Dictionary<string, Vector2[]> GetPixelColorDictionary(Image image)
    {
        // Store each pixel color in a dictionary with the pixel's position
        Dictionary<string, Vector2[]> pixelColorDict = new Dictionary<string, Vector2[]>();

        for (int y = 0; y < image.GetHeight(); y++)
        {
            for (int x = 0; x < image.GetWidth(); x++)
            {
                Color pixelColor = image.GetPixel(x, y);
                string hexColor = "#" + pixelColor.ToHtml(false);

                if (!pixelColorDict.ContainsKey(hexColor))
                {
                    pixelColorDict[hexColor] = new Vector2[] { new Vector2(x, y) };
                }
                else
                {
                    pixelColorDict[hexColor] = pixelColorDict[hexColor].Append(new Vector2(x, y)).ToArray();
                }
            }
        }

        return pixelColorDict;
    }

    private Node CreateRegionNode()
    {
        Node regions = new Node
        {
            Name = "Regions",
            UniqueNameInOwner = true
        };

        AddChild(regions);
        return regions;
    }

    private static Godot.Collections.Array<Vector2[]> GetPolygons(Image image, string regionColor, Dictionary<string, Vector2[]> pixelColorDict)
    {
        Image targetImage = Image.CreateEmpty(image.GetWidth(), image.GetHeight(), false, Image.Format.Rgba8);
        foreach (Vector2 value in pixelColorDict[regionColor])
        {
            targetImage.SetPixel((int)value.X, (int)value.Y, new Color(1, 1, 1));
        }

        Bitmap bitmap = new Bitmap();
        bitmap.CreateFromImageAlpha(targetImage);
        Godot.Collections.Array<Vector2[]> polygons = bitmap.OpaqueToPolygons(new Rect2I(Vector2I.Zero, bitmap.GetSize()), 0.1f);
        return polygons;
    }
}
