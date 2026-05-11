namespace ValheimCustomItemRegistry
{
    internal interface IItemPackParser
    {
        string FormatName { get; }
        string MissingDependencyMessage { get; }
        bool IsAvailable { get; }
        bool CanParse(string filePath);
        ItemPackDto Parse(string text);
    }
}
