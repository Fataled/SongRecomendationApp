using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectHellsParadise;

public partial class FavoritesPlaylistPage : ContentPage
{
    public ObservableCollection<FavoriteSongItem> Favorites { get; } = new();
    public ObservableCollection<PlaylistItem> Playlists { get; } = new();
    public ObservableCollection<FavoriteSongItem> PlaylistSongs { get; } = new();

    private PlaylistItem? _selectedPlaylist;

    public string SelectedPlaylistTitle =>
        _selectedPlaylist == null
            ? "No playlist selected"
            : $"Songs in {_selectedPlaylist.Name}";

    public FavoritesPlaylistPage()
    {
        InitializeComponent();
        SeedSampleData();
    }

    private void SeedSampleData()
    {
        var song1 = new FavoriteSongItem { Title = "Blinding Lights", Artist = "The Weeknd" };
        var song2 = new FavoriteSongItem { Title = "505", Artist = "Arctic Monkeys" };
        var song3 = new FavoriteSongItem { Title = "After Dark", Artist = "Mr.Kitty" };

        Favorites.Add(song1);
        Favorites.Add(song2);
        Favorites.Add(song3);

        var playlist1 = new PlaylistItem { Name = "Late Night" };
        playlist1.AddSong(song1.Clone());
        playlist1.AddSong(song3.Clone());

        var playlist2 = new PlaylistItem { Name = "Study Mix" };
        playlist2.AddSong(song2.Clone());

        Playlists.Add(playlist1);
        Playlists.Add(playlist2);
    }

    private async void BackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void AddPlaylistClicked(object sender, EventArgs e)
    {
        string playlistName = PlaylistNameEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(playlistName))
        {
            await DisplayAlert("Missing name", "Enter a playlist name first.", "OK");
            return;
        }

        bool exists = Playlists.Any(p =>
            p.Name.Equals(playlistName, StringComparison.OrdinalIgnoreCase));

        if (exists)
        {
            await DisplayAlert("Duplicate playlist", "A playlist with that name already exists.", "OK");
            return;
        }

        Playlists.Add(new PlaylistItem { Name = playlistName });
        PlaylistNameEntry.Text = string.Empty;

        await DisplayAlert("Created", $"Playlist '{playlistName}' created.", "OK");
    }

    private void PlaylistSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedPlaylist = e.CurrentSelection.FirstOrDefault() as PlaylistItem;
        RefreshPlaylistSongs();
    }

    private async void AddFavoriteToSelectedPlaylistClicked(object sender, EventArgs e)
    {
        if (_selectedPlaylist == null)
        {
            await DisplayAlert("No playlist selected", "Select a playlist first, then add a favorite to it.", "OK");
            return;
        }

        if (sender is not Button button || button.BindingContext is not FavoriteSongItem song)
            return;

        bool alreadyExists = _selectedPlaylist.Songs.Any(s =>
            s.Title.Equals(song.Title, StringComparison.OrdinalIgnoreCase) &&
            s.Artist.Equals(song.Artist, StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            await DisplayAlert("Already added", "That song is already in the selected playlist.", "OK");
            return;
        }

        _selectedPlaylist.AddSong(song.Clone());
        RefreshPlaylistSongs();

        await DisplayAlert("Added", $"'{song.Title}' was added to '{_selectedPlaylist.Name}'.", "OK");
    }

    private async void RemoveFavoriteClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not FavoriteSongItem song)
            return;

        Favorites.Remove(song);

        foreach (PlaylistItem playlist in Playlists)
        {
            playlist.RemoveSong(song);
        }

        RefreshPlaylistSongs();

        await DisplayAlert("Removed", $"'{song.Title}' was removed from favorites.", "OK");
    }

    private async void RemovePlaylistClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not PlaylistItem playlist)
            return;

        bool confirm = await DisplayAlert("Delete playlist",
            $"Delete '{playlist.Name}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        if (_selectedPlaylist == playlist)
        {
            _selectedPlaylist = null;
        }

        Playlists.Remove(playlist);
        RefreshPlaylistSongs();
    }

    private async void OpenFavoriteClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not FavoriteSongItem song)
            return;

        await DisplayAlert("Open song", $"Pretend to open: {song.Title} by {song.Artist}", "OK");
    }

    private async void OpenPlaylistSongClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not FavoriteSongItem song)
            return;

        await DisplayAlert("Open song", $"Pretend to open: {song.Title} by {song.Artist}", "OK");
    }

    private void RefreshPlaylistSongs()
    {
        PlaylistSongs.Clear();

        if (_selectedPlaylist != null)
        {
            foreach (FavoriteSongItem song in _selectedPlaylist.Songs)
            {
                PlaylistSongs.Add(song);
            }
        }

        OnPropertyChanged(nameof(SelectedPlaylistTitle));
    }
}

public class FavoriteSongItem
{
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;

    public FavoriteSongItem Clone()
    {
        return new FavoriteSongItem
        {
            Title = Title,
            Artist = Artist
        };
    }
}

public class PlaylistItem : INotifyPropertyChanged
{
    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<FavoriteSongItem> Songs { get; } = new();

    public string SongCountText => $"{Songs.Count} song(s)";

    public event PropertyChangedEventHandler? PropertyChanged;

    public void AddSong(FavoriteSongItem song)
    {
        Songs.Add(song);
        OnPropertyChanged(nameof(SongCountText));
    }

    public void RemoveSong(FavoriteSongItem targetSong)
    {
        FavoriteSongItem? songToRemove = Songs.FirstOrDefault(s =>
            s.Title.Equals(targetSong.Title, StringComparison.OrdinalIgnoreCase) &&
            s.Artist.Equals(targetSong.Artist, StringComparison.OrdinalIgnoreCase));

        if (songToRemove != null)
        {
            Songs.Remove(songToRemove);
            OnPropertyChanged(nameof(SongCountText));
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}