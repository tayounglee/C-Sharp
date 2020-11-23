public static class ChessTeamExtensions
{
    public static ChessTeam Invert(this ChessTeam @this)
    {
        if (@this == ChessTeam.Black) {
            return ChessTeam.White;
        }
        else {
            return ChessTeam.Black;
        }
    }
}