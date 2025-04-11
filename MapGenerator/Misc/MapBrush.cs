namespace MapGenerator.Enums
{
    public struct MapBrush
    {
        public int Type;
        public Color Color;
        public string Name;

        public MapBrush(int type, Color black, string v) : this()
        {
            this.Type = type;
            this.Color = black;
            this.Name = v;
        }
    }
}
