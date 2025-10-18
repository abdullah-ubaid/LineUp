namespace LineUpGame
{
    public interface IPlayer
    {
        string Name { get; }
        DiscType Disc { get; }
        int ChooseColumn(Board board);
    }
}
