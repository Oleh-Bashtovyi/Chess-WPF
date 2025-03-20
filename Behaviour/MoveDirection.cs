namespace Chess_game.Behaviour
{
    public class MoveDirection
    {
        public sbyte X_Direction { get; }
        public sbyte Y_Direction { get; }
        public sbyte MoveDistance { get; }


        public MoveDirection(sbyte x, sbyte y, sbyte distance)
        {
            X_Direction= x;
            Y_Direction= y;
            MoveDistance = distance;
        }


        public MoveDirection Clone()
        {
            return new MoveDirection(X_Direction, Y_Direction, MoveDistance);
        }
    }
}
