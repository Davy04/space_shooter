[System.Serializable]
public class HighScoreEntry
{
    public string playerName;
    public float time;

    public HighScoreEntry(string name, float time)
    {
        this.playerName = name;
        this.time = time;
    }
}