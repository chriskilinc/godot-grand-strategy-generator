using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Node2D
{
    PackedScene _regionAreaScene;
    Node _regions;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        GD.Print("Hello, World!");
        _regionAreaScene = GD.Load<PackedScene>("res://Scenes/region_area.tscn");
        GenerateRegions();
    }

    private void GenerateRegions()
    {
        var mapImage = GD.Load<Texture2D>("res://MapData/map.png").GetImage();
        var pixelColorDict = GetPixelColorDict(mapImage);
        var mapData = ImportFile("MapData/mapData.json").AsGodotDictionary<string, string>();
        CreateRegionNode();
        CreateRegionAreas(mapImage, pixelColorDict, mapData);
    }

    private void CreateRegionNode()
    {
        // Only call this once in _Ready
        Node regions = new Node
        {
            Name = "Regions",
            UniqueNameInOwner = true
        };

        AddChild(regions);
        _regions = regions;
    }

    private void CreateRegionAreas(Image image, Dictionary<string, Vector2[]> pixelColorDict, Godot.Collections.Dictionary<string, string> mapData)
    {
        foreach (KeyValuePair<string, string> region in mapData)
        {
            // region.Key is Color in hex format
            // region.Value is Region Name

            RegionArea regionArea = _regionAreaScene.Instantiate<RegionArea>();
            regionArea.regionName = region.Value;
            regionArea.regionId = region.Key;
            regionArea.Name = region.Key;

            _regions.AddChild(regionArea);

            Godot.Collections.Array<Vector2[]> polygons = GetPolygons(image, region.Key, pixelColorDict);

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

    private Dictionary<string, Vector2[]> GetPixelColorDict(Image image)
    {
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

    private Variant ImportFile(string path)
    {
        FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.Print("File not found");
            return new Variant();
        }
        else
        {
            GD.Print("File found");
            return Json.ParseString(file.GetAsText().Replace("_", " "));
        }
    }

    private Godot.Collections.Array<Vector2[]> GetPolygons(Image image, string regionColor, Dictionary<string, Vector2[]> pixelColorDict)
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
