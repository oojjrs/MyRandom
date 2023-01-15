namespace Assets.Sources.Scripts
{
    public partial struct Hex
    {
        private class FlatToppedDoubleOffsetCoordinate : CoordinateInterface
        {
            public Hex Hex { get; }
            public int X { get; }
            public int Y { get; }

            public FlatToppedDoubleOffsetCoordinate(Hex hex)
            {
                Hex = hex;
                X = hex.Q;
                Y = hex.Q + hex.R * 2;
            }

            // 생각해보니 동작하지 않을 거라서 주석 처리. 로직은 찾기 귀찮으니 남겨둠
            //public static implicit operator Hex(FlatToppedDoubleOffsetCoordinate c)
            //{
            //    return new Hex(c.X, (c.Y - c.X) / 2, c.Size, FormEnum.FlatTopped);
            //}
        }
    }
}
