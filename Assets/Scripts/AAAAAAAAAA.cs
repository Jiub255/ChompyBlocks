public class AAAAAAAAAA
{/*
  
    RISE AND FALL - Like tetris but both going toward the middle, the bottom player rising and the top one falling
    --------------------------------------------------------------------------------------------------------------

    FIX:

    BUGS SEEM TO BE GONE
        Making Line Chomper and Treat Chomper separate prefabs seems to have fixed everything.
        Definitely not sure about it though

    Weird pause bug
        One happens when completing a line, but it seems like only after having throw some treats 

    Some bug when pushing both treat buttons while chomp is clearing lines
        Not sure what happened, but it froze/paused the game

    When both players get full lines in the center on the same frame, they stack on top of each other
        Not a big deal, shouldn't really happen in real game play.
        Maybe alternate update frames for top/bottom player movement?
            But not input

    Change all prefab pieces to use brick prefab

    Make it so if you throw a treat on the last brick in a row, it moves all further bricks toward center?
        This would help the opponent, but it would be rare.
        Probably makes sense not to, but it looks kinds dumb having a empty line sitting there.
        
    --------------------------------------------------------------------------------------------------------------


    TODO:
        
    Balance
        How should scoring work?
            Should there be rounds? or should one win be a win?
            Points for completed lines?
        How fast should pieces fall? How quickly should they get faster?
        Which pieces to use?
        Switch top and bottom every other round?

    Art
        Make better looking pieces, chomper, and UI

    Main Menu, High scores?

    AI for second player?


    --------------------------------------------------------------------------------------------------------------

    When you complete a line, a cat pac man comes out and chomps the line.

    You can get cat treats for getting multiple lines at once. Throw them on the other player's
        board and Chompy will go eat that tile/tiles.

    The middle row is shared by both players

    Do a four player version?
        Pieces are diagonal
        "Bottom" border is a "V" shape
        four play spaces come together in "X" shape in the center
        Complete a v-shaped line to delete it and move further blocks toward center


*/
}