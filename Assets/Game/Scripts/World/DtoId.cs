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

        public static string GetParentId(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            var idx = id.LastIndexOf('/') ;
            if (idx <= 0) return null;
            return id.Substring(0, idx);
        }

        public static int GetLevel(string id)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            int level = 1;
            for (int i = 0; i < id.Length; i++) if (id[i] == '/') level++;
            return level;
        }

        public static bool TrySplit(string id, out string parent, out string leaf)
        {
            parent = null; leaf = null;
            if (string.IsNullOrEmpty(id)) return false;
            var idx = id.LastIndexOf('/');
            if (idx < 0) { leaf = id; return true; }
            parent = idx == 0 ? null : id.Substring(0, idx);
            leaf = id.Substring(idx + 1);
            return true;
        }
    }
}