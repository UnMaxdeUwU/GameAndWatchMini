using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles persistent storage and retrieval of leaderboard entries for the Runner mode.
/// Uses a dedicated save file to stay isolated from other mini-game leaderboards.
/// </summary>
public class LeaderboardControllerRunner
{
    private const string SaveFileName = "/leaderboard_runner.json";
    private const int TopCount = 3;

    private string FilePath => Application.persistentDataPath + SaveFileName;

    /// <summary>
    /// Adds a new entry to the Runner leaderboard and saves immediately.
    /// </summary>
    public void AddEntry(string playerName, int score)
    {
        LeaderboardData data = Load();
        data.Entries.Add(new LeaderboardEntry { PlayerName = playerName, Score = score });
        Save(data);
    }

    /// <summary>
    /// Returns the top N entries sorted from highest to lowest score.
    /// </summary>
    public List<LeaderboardEntry> GetTopEntries(int count = TopCount)
    {
        LeaderboardData data = Load();
        return data.Entries
            .OrderByDescending(e => e.Score)
            .Take(count)
            .ToList();
    }

    private void Save(LeaderboardData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }

    private LeaderboardData Load()
    {
        if (!File.Exists(FilePath))
            return new LeaderboardData();

        string json = File.ReadAllText(FilePath);
        return JsonUtility.FromJson<LeaderboardData>(json) ?? new LeaderboardData();
    }
}
