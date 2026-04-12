using System.Text.Json;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.Services;

/// <summary>
/// Handles persisting the user's Spotify search history to a local JSON file.
/// This is the data tier component for the Spotify feature — satisfies the
/// project requirement of file-based data persistence.
/// Stores the last 20 searched song names and the last 10 songs opened in Spotify.
/// </summary>
/// <author>Obaid Waqas</author>
public class SpotifyHistoryService
{
    /// <summary>
    /// The file path where search history is saved.
    /// Uses MAUI's FileSystem.AppDataDirectory so it works on all platforms.
    /// </summary>
    private readonly string _filePath =
        Path.Combine(FileSystem.AppDataDirectory, "spotify_history.json");

    /// <summary>
    /// Internal data structure that gets serialized to and from the JSON file.
    /// Holds both recent searches and recently opened songs.
    /// </summary>
    private class SpotifyHistoryData
    {
        /// <summary>List of recent search query strings the user has typed</summary>
        public List<string> RecentSearches { get; set; } = new();

        /// <summary>List of songs the user has opened in Spotify</summary>
        public List<RecentSongEntry> RecentlyOpened { get; set; } = new();
    }

    /// <summary>
    /// A lightweight record of a song that was opened in Spotify.
    /// Stored in the JSON file so it can be reloaded on next app launch.
    /// </summary>
    public class RecentSongEntry
    {
        /// <summary>The song name</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>The primary artist name</summary>
        public string Artist { get; set; } = string.Empty;

        /// <summary>The album name</summary>
        public string Album { get; set; } = string.Empty;

        /// <summary>The external Spotify URL so the song can be reopened</summary>
        public string ExternalUrl { get; set; } = string.Empty;

        /// <summary>Album art URL for display in the history list</summary>
        public string AlbumArtUrl { get; set; } = string.Empty;

        /// <summary>The date and time this song was opened, stored for sorting</summary>
        public DateTime OpenedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Loads the history data from the JSON file.
    /// Returns an empty history object if the file does not exist yet.
    /// </summary>
    private async Task<SpotifyHistoryData> LoadAsync()
    {
        // If the file doesn't exist yet, return empty history — first launch
        if (!File.Exists(_filePath))
            return new SpotifyHistoryData();

        try
        {
            string json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<SpotifyHistoryData>(json)
                   ?? new SpotifyHistoryData();
        }
        catch (Exception ex)
        {
            // If the file is corrupted, start fresh rather than crash
            Console.WriteLine($"[SpotifyHistoryService] Failed to load history: {ex.Message}");
            return new SpotifyHistoryData();
        }
    }

    /// <summary>
    /// Saves the history data to the JSON file.
    /// Creates the file if it does not exist.
    /// </summary>
    /// <param name="data">The history data to serialize and write</param>
    private async Task SaveAsync(SpotifyHistoryData data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SpotifyHistoryService] Failed to save history: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves a search query to the recent searches list.
    /// Keeps only the last 20 unique searches — duplicates are moved to the top.
    /// </summary>
    /// <param name="query">The search query the user typed</param>
    public async Task SaveSearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return;

        SpotifyHistoryData data = await LoadAsync();

        // Remove duplicate if it already exists so we can move it to the top
        data.RecentSearches.Remove(query);
        data.RecentSearches.Insert(0, query);

        // Keep only the 20 most recent searches
        if (data.RecentSearches.Count > 20)
            data.RecentSearches = data.RecentSearches.Take(20).ToList();

        await SaveAsync(data);
    }

    /// <summary>
    /// Saves a song that the user opened in Spotify to the recently opened list.
    /// Keeps only the last 10 unique songs — duplicates are moved to the top.
    /// </summary>
    /// <param name="song">The SpotifyMusicSong that was opened</param>
    public async Task SaveOpenedSongAsync(SpotifyMusicSong song)
    {
        SpotifyHistoryData data = await LoadAsync();

        // Remove duplicate entry for the same song if it exists
        data.RecentlyOpened.RemoveAll(s => s.ExternalUrl == song.ExternalUrl);

        data.RecentlyOpened.Insert(0, new RecentSongEntry
        {
            Name = song.Name,
            Artist = song.Artist,
            Album = song.Album,
            ExternalUrl = song.ExternalUrl,
            AlbumArtUrl = song.AlbumArtUrl,
            OpenedAt = DateTime.Now
        });

        // Keep only the 10 most recently opened songs
        if (data.RecentlyOpened.Count > 10)
            data.RecentlyOpened = data.RecentlyOpened.Take(10).ToList();

        await SaveAsync(data);
    }

    /// <summary>
    /// Returns the list of recent search queries loaded from the JSON file.
    /// Returns an empty list if no history exists yet.
    /// </summary>
    /// <returns>List of recent search query strings, most recent first</returns>
    public async Task<List<string>> GetRecentSearchesAsync()
    {
        SpotifyHistoryData data = await LoadAsync();
        return data.RecentSearches;
    }

    /// <summary>
    /// Returns the list of recently opened songs loaded from the JSON file.
    /// Returns an empty list if no history exists yet.
    /// </summary>
    /// <returns>List of RecentSongEntry objects, most recently opened first</returns>
    public async Task<List<RecentSongEntry>> GetRecentlyOpenedAsync()
    {
        SpotifyHistoryData data = await LoadAsync();
        return data.RecentlyOpened;
    }

    /// <summary>
    /// Clears all saved search history and recently opened songs from the file.
    /// </summary>
    public async Task ClearHistoryAsync()
    {
        await SaveAsync(new SpotifyHistoryData());
    }
}
