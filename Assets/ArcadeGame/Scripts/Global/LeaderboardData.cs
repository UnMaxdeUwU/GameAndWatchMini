using System;
using System.Collections.Generic;

/// <summary>
/// Represents a single leaderboard entry.
/// </summary>
[Serializable]
public class LeaderboardEntry
{
    public string PlayerName;
    public int Score;
}

/// <summary>
/// Wrapper for JSON serialization of the leaderboard list.
/// </summary>
[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> Entries = new List<LeaderboardEntry>();
}
