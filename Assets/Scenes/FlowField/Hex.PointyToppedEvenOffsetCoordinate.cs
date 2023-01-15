namespace Assets.Sources.Scripts
{
    public partial struct Hex
    {
        private class PointyToppedEvenOffsetCoordinate : CoordinateInterface
        {
            public Hex Hex { get; }
            public int X { get; }
            public int Y { get; }

            public PointyToppedEvenOffsetCoordinate(Hex hex)
            {
                Hex = hex;
                X = hex.Q + (hex.R + (hex.R & 1)) / 2;
                Y = hex.R;
            }

            // 생각해보니 동작하지 않을 거라서 주석 처리. 로직은 찾기 귀찮으니 남겨둠
            //public static implicit operator Hex(PointyToppedEvenOffsetCoordinate c)
            //{
            //    return new Hex(c.X - (c.Y + (c.Y & 1)) / 2, c.Y, c.Size, FormEnum.PointyTopped);
            //}
        }
    }
}
