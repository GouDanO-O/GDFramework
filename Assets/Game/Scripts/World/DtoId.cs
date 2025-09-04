namespace Game.World
{
    public static class DtoId
    {
        public static string Join(string parent, string child)
        {
            if (string.IsNullOrEmpty(parent)) return child ?? string.Empty;
            if (string.IsNullOrEmpty(child)) return parent ?? string.Empty;
            return $"{parent}/{child}";
        }
    }
}