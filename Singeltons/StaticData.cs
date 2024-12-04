using Godot;

public partial class StaticData : Node
{
    [Export]
    public Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, string>> data = new();
    private string filePath = "res://MapData/mapData.json";


    public override void _Ready()
    {
        data = LoadJsonFile(filePath);
    }

    private Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, string>> LoadJsonFile(string path)
    {
        if (FileAccess.FileExists(path))
        {
            var dataFile = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            var parsedData = Json.ParseString(dataFile.GetAsText()).AsGodotDictionary<string, Godot.Collections.Dictionary<string, string>>();

            if (parsedData != null)
            {
                return parsedData;
            }
            else
            {
                GD.Print("Error parsing JSON file");
            }
        }
        else
        {
            GD.Print("File does not exist");
        }
        return null;
    }

}