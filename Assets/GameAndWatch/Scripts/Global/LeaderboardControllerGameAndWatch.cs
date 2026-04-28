using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles persistent storage and retrieval of leaderboard entries for the GameAndWatch mini-game.
/// Isolated from other mini-game leaderboards via a dedicated save file.
/// </summary>
public class LeaderboardControllerGameAndWatch
{
    private const string SaveFileName = "/leaderboard_gameandwatch.json";

    private string FilePath => Application.persistentDataPath + SaveFileName;

    /// <summary>Adds a new entry and saves immediately.</summary>
    public void AddEntry(string playerName, int score)
    {
        LeaderboardData data = Load();
        data.Entries.Add(new LeaderboardEntry { PlayerName = playerName, Score = score });
        Save(data);
    }

    /// <summary>Returns the top N entries sorted from highest to lowest score.</summary>
    public List<LeaderboardEntry> GetTopEntries(int count = 3)
    {
        return Load().Entries
            .OrderByDescending(e => e.Score)
            .Take(count)
            .ToList();
    }

    private void Save(LeaderboardData data)
    {
        File.WriteAllText(FilePath, JsonUtility.ToJson(data, true));
    }

    private LeaderboardData Load()
    {
        if (!File.Exists(FilePath)) return new LeaderboardData();
        return JsonUtility.FromJson<LeaderboardData>(File.ReadAllText(FilePath)) ?? new LeaderboardData();
    }
}
