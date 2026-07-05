namespace WheelGame.Core
{
    /// <summary>
    /// High level state of the game flow. GameManager is the single owner of transitions.
    /// </summary>
    public enum GameState
    {
        Idle,        // Wheel is stopped. Player can spin or leave (cash out).
        Spinning,    // Wheel is animating. All input except pause is locked.
        BombResolve, // Bomb was hit. Popup is open: give up / revive.
        CashedOut    // Player left with rewards. End screen is open.
    }
}
