using System;

namespace Chess_game.Models;

public class GameStartupSettings
{
    //=================================================================================================================
    // FIELDS
    //=================================================================================================================
    private int _movesLimit = 1000;
    private int _whitesTimerDuration = 0;
    private int _blacksTimerDuration = 0;


    //=================================================================================================================
    // PROPERTIES
    //=================================================================================================================
    public Color MainPlayerColor { get; set; }
    public GameModeType GameMode { get; set; }
    public Difficulty WhitesDifficulty { get; set; }
    public Difficulty BlacksDifficulty { get; set; }
    public bool IsTimerOn { get; set; }
    public int WhitesTimerDuration {
        get { return _whitesTimerDuration; }
        set
        {
            if (value > 9999)
                _whitesTimerDuration = 9999;
            else if (value < 0)
                throw new ArgumentOutOfRangeException("White timer duration can not be less than 0!");
            else
                _whitesTimerDuration = value;
        }
    }
    public int BlacksTimerDuration
    {
        get { return _blacksTimerDuration; }
        set
        {
            if (value > 9999)
                _blacksTimerDuration = 9999;
            else if (value < 0)
                throw new ArgumentOutOfRangeException("White timer duration can not be less than 0!");
            else
                _blacksTimerDuration = value;
        }
    }
    public int MovesLimit
    {
        get
        {
            if (IsMovesLimitOn) return _movesLimit;
            else return 1000;
        }
        set
        {
            if(value > 1000)
                _movesLimit = 1000;
            else if(value < 0)
                throw new ArgumentOutOfRangeException("Moves limit can not be less than 0!");
            else
                _movesLimit = value;
        }
    }
    public bool IsMovesLimitOn { get; set; }
    public bool IsFogOfWarOn { get; set; }




    //=================================================================================================================
    // CONSTRUCCTORS
    //=================================================================================================================
    public GameStartupSettings()
    {
        MainPlayerColor= Color.White;
        GameMode= GameModeType.PvP;
        BlacksDifficulty = Difficulty.Easy;
        WhitesDifficulty= Difficulty.Easy;
        IsTimerOn = false;
        WhitesTimerDuration= 0;
        BlacksTimerDuration= 0;
        MovesLimit= 1000;
        IsFogOfWarOn= false;
        IsMovesLimitOn = false;
        MovesLimit = 1000;
    }


    public GameStartupSettings
    (
        Color mainPlayerColor,
        GameModeType gameMode, 
        Difficulty whitesDifficulty = Difficulty.Easy,
        Difficulty blacksDifficulty = Difficulty.Easy, 
        bool isTimerOn = false, 
        int whitesTimerDuration = 0,
        int blacksTimerDuration = 0,
        bool isMovesLimitOn = false, 
        int movesLimit = 1000, 
        bool isFogOvWarOn = false 
    )
    {
        MainPlayerColor = mainPlayerColor;
        GameMode = gameMode;

        WhitesDifficulty = whitesDifficulty;
        BlacksDifficulty = blacksDifficulty;

        IsTimerOn = isTimerOn;
        WhitesTimerDuration = whitesTimerDuration;
        BlacksTimerDuration = blacksTimerDuration;

        IsMovesLimitOn = isMovesLimitOn;
        MovesLimit = movesLimit;

        IsFogOfWarOn = isFogOvWarOn;
    }
}