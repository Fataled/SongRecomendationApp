using ProjectHellsParadise.BusinessLogic.APIs;
using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;
using ProjectHellsParadise.BusinessLogic.Exceptions;
using ProjectHellsParadise.BusinessLogic.Models;

namespace ProjectHellsParadise.BusinessLogic.Services;

/// <summary>
/// Picks a random song from Deezer and runs it through the full feature
/// extraction pipeline, producing a FeatureData object ready to be used
/// as the base song for the recommendation engine.
///
/// This is Obaid's data tier contribution — the service that powers the
/// "Random Song" feature on the SongSearchPage.
/// </summary>
/// <author>Obaid Waqas</author>
public class RandomSongService
{
    /// <summary>
    /// The pool of genres we randomly pick from when choosing a random song.
    /// Uses the same genre strings that DeezerClient understands.
    /// </summary>
    private static readonly string[] GenrePool =
    [
        "pop", "hip hop", "rock", "dance", "r&b",
        "alternative", "electronic", "jazz", "country",
        "soul", "reggae", "classical", "metal", "folk"
    ];

    private readonly DeezerClient _deezerClient;
    private readonly FeatureExtractionApi _featureApi;
    private readonly Random _random = new();

    /// <summary>
    /// Constructor — DeezerClient and FeatureExtractionApi are injected
    /// from the DI container registered in MauiProgram.cs.
    /// </summary>
    public RandomSongService(DeezerClient deezerClient, FeatureExtractionApi featureApi)
    {
        _deezerClient = deezerClient;
        _featureApi = featureApi;
    }

    /// <summary>
    /// Picks a random genre, fetches songs for that genre from Deezer,
    /// selects one at random that has a valid 30-second preview,
    /// downloads the preview, and runs the full feature extraction pipeline.
    /// </summary>
    /// <returns>
    /// A fully populated FeatureData object — same as what SongSearch produces —
    /// ready to be placed in SongSessionService.BaseSong.
    /// </returns>
    /// <exception cref="ApiException">If Deezer returns no songs for the genre.</exception>
    /// <exception cref="ByteTransformationException">If the preview download fails.</exception>
    public async Task<FeatureData> GetRandomSongAsync()
    {
        // Pick a random genre from the pool.
        string genre = GenrePool[_random.Next(GenrePool.Length)];

        // Fetch songs for that genre from Deezer's chart playlists.
        DeezerDTO songs = await _deezerClient.GetGenreSongsAsync(genre);

        // Filter to only tracks that have a valid preview URL so we can analyse them.
        List<DeezerDTO.DeezerData> playableSongs = songs.Data
            .Where(s => !string.IsNullOrEmpty(s.Preview))
            .ToList();

        if (playableSongs.Count == 0)
            throw new ApiException(
                $"No playable songs found for genre '{genre}'. Try again.");

        // Pick one song at random from the filtered list.
        DeezerDTO.DeezerData picked = playableSongs[_random.Next(playableSongs.Count)];

        // Download the 30-second MP3 preview bytes from Deezer.
        ByteRecord preview = await _deezerClient.DownloadPreviewBytes(
            picked.Preview, picked.Title, picked.Artist.Name);

        // Run feature extraction through the Python ML backend —
        // this is the same call SongSearch makes, keeping the flow identical.
        FeatureData featureData = await _featureApi.GetFeaturesAsync("features", preview);

        // Classify the genres so the recommendation engine can use them.
        GenrePredictionDTO[] genres = await _featureApi.PostAsync<GenrePredictionDTO[]>(
            "classify", featureData.WavSongBytes);

        featureData.Genre = genres;

        return featureData;
    }
}