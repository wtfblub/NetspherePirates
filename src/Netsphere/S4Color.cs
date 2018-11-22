namespace Netsphere
{
    public struct S4Color
    {
        public static S4Color Empty => new S4Color(255, 0, 0, 0);

        public static S4Color Red => new S4Color(255, 0, 0);

        public static S4Color Green => new S4Color(0, 255, 0);

        public static S4Color Blue => new S4Color(0, 0, 255);

        public int A { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public S4Color(int a, int r, int g, int b)
            : this()
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public S4Color(int r, int g, int b)
            : this(255, r, g, b)
        {
        }

        public static S4Color FromRgb(int r, int g, int b)
        {
            return new S4Color(r, g, b);
        }

        public static S4Color FromArgb(int a, int r, int g, int b)
        {
            return new S4Color(a, r, g, b);
        }

        public override string ToString()
        {
            return $"{{CB-{R},{G},{B},{A}}}";
        }
    }
}
