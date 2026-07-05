namespace WheelGame.Core
{
    /// <summary>
    /// Type of a zone. Determines which wheel config is used and whether the bomb exists.
    /// </summary>
    public enum ZoneType
    {
        Normal, // Bronze wheel, contains the bomb.
        Safe,   // Every 5th zone. Silver wheel, no bomb.
        Super   // Every 30th zone. Golden wheel, special rewards, no bomb.
    }
}
