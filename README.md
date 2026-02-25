# PROG 10065 Term Project Report

Students use this file to complete the project proposal and then iterate on it to finalize it into a project report. Please consult the project proposal assignment which contains requirements for completing the proposal as well as the requirements for the project and its report.

# NAMEYETTOBEDECIDED – Song Recommender

PROG10065 – Interactive Application Development  
Term Team Project – Project Proposal  
Team IA-11

- Oghenebrume Ako – ID  
- Obaid Waqas – ID  
- Rithvik Ganesh Konapala – ID  

---

## 1. Project Overview

NAMEYETTOBEDECIDED is a song recommendation application that helps users discover new music based on songs they already enjoy. The user selects or searches for a track, and the application computes a feature vector for that song and compares it against a collection of other songs to find the most similar ones. Recommendations are based on numerical similarity rather than just genre labels, making the suggestions more personalized and precise. 

The application is developed as a .NET MAUI app using C# and XAML in Visual Studio on Windows. It uses MAUI pages for the presentation layer, C# classes and collections (such as `List<T>` and `Dictionary<TKey, TValue>`) for the business logic, and a SQL database for its data layer. The app demonstrates object‑oriented principles such as encapsulation, separation of concerns, and reuse through well‑designed classes and methods. 

Users will interact with multiple screens to search for songs, view detailed information, see recommendations, and manage favorites or playlists. For users who have a premium music streaming subscription, NAMEYETTOBEDECIDED can open the selected track in their external music player so they can listen to recommended songs directly. In addition, the project includes a “new” research component compared to the classroom examples: representing songs as numerical feature vectors and using similarity metrics to derive recommendations, together with using a SQL database as the persistence mechanism.

> **Note:** The course requirements emphasize a file‑based data layer; the use of SQL here is treated as an extension / “something new” and can be adjusted to a file‑based format if required by the instructor.

---

## 2. User Interface Wireframes

> **Note:** The actual wireframes will be created in Visual Paradigm using the Desktop Wireframe tool and exported as images to be included in the final proposal. Each screen below corresponds to one exported wireframe image. 

### 2.1 Home / Search Page

**Purpose:**  
This is the main entry point of the application, where the user can search for songs and browse initial suggestions.

**Inputs:**  
- Text search box for song title and/or artist.  
- Optional filters (e.g., “Use local library”, “Use online catalog”, simple genre filter).  
- Button to submit the search query.  

**Outputs:**  
- A list of matching songs, each showing title, artist, and possibly basic metadata (album, duration).  
- Status messages for empty search results or loading state. 

**User Interaction / Workflow:**  
- The user types a query and taps the search button.  
- The page queries the song collection (via the business logic layer) and displays the search results in a list.  
- Tapping a song in the list navigates to the Song Details & Recommendations page for that track.

---

### 2.2 Song Details & Recommendations Page

**Purpose:**  
This page shows detailed information about a selected song and displays similar tracks recommended by the application. 

**Inputs:**  
- Buttons or controls to trigger “Get Recommendations”.  
- Optional controls such as sliders or toggles for “Similarity strength”, “Tempo preference”, or “Mood preference”.  
- Buttons to add the song (or recommended songs) to favorites or to a playlist. 

**Outputs:**  
- Song details: title, artist, album, duration, and any key features the app exposes (e.g., tempo, energy, mood).  
- A list of recommended songs, each row showing the candidate song’s title, artist, and possibly a similarity score.  
- Status messages when no recommendations are found.

**User Interaction / Workflow:**  
- The page receives a selected song from the Home / Search page.  
- When the user taps “Get Recommendations”, the page calls into the `RecommendationEngine` to compute similar songs based on feature vectors.  
- Recommended songs are displayed in a list; tapping one can:  
  - Show it as the “current” song (refreshing the page), and/or  
  - Open the song in the user’s external music player if the premium setting is enabled.  
- Users can save songs to favorites or add them to playlists directly from this page. 

---

### 2.3 Favorites / Playlists Page

**Purpose:**  
This page allows users to manage their saved songs and playlists so they can easily revisit and play previously discovered tracks. 

**Inputs:**  
- Buttons to create a new playlist.  
- Controls to add or remove songs from favorites or playlists.  
- Optional filters to sort by artist, recently added, or playlist. 

**Outputs:**  
- A list of favorite songs.  
- A list of user‑defined playlists, each with a name and song count.  
- When a playlist is selected, a list of songs in that playlist. 

**User Interaction / Workflow:**  
- The user navigates here from other pages (e.g., via a navigation bar or menu).  
- The app loads the user’s favorites and playlists from the database and displays them.  
- Tapping a playlist opens its contents; tapping a song opens the Song Details & Recommendations page for that track.  
- Users can remove songs from favorites or playlists; changes are saved back through the data layer. 

---

## 3. Project Design (UML)

> **Note:** UML diagrams will be created in Visual Paradigm and exported as images. This section describes the intended design and the main classes and workflows. 

### 3.1 Class Diagram Overview

The main business classes of NAMEYETTOBEDECIDED include: 

- `Song`  
  - Represents a single track.  
  - Key properties: `Id`, `Title`, `Artist`, `Album`, `Duration`, `Genre`, `Tempo`, `FeatureVector` (a collection of numeric values), and other metadata as needed.

- `SongRepository`  
  - Responsible for loading and saving songs from/to the database.  
  - Holds collections such as `List<Song>` or `Dictionary<string, Song>` for efficient lookup.  
  - Provides methods like `LoadSongs()`, `SaveSong(Song song)`, `GetAllSongs()`, and `FindSongsByQuery(string query)`.  

- `RecommendationEngine`  
  - Uses the songs loaded by `SongRepository` to compute recommendations.  
  - Provides methods such as `GetSimilarSongs(Song target, int count)` that compare feature vectors and return a sorted list of similar songs.  
  - Encapsulates the similarity calculation (e.g., Euclidean distance or cosine similarity).  

- `SongVectorizer`  
  - Responsible for creating and updating `FeatureVector` values for songs.  
  - Provides methods such as `ExtractFeatures(Song song)` which returns a numeric vector for similarity comparisons.  

- `UserProfile`  
  - Stores user‑specific data such as `IsPremium`, `FavoriteSongs`, and `Playlists`.  
  - Provides methods to add and remove favorites and to manage playlists.  

- UI page classes (MAUI pages):  
  - `HomePage` – binds to a view model or directly to `SongRepository` to display search results.  
  - `SongDetailsPage` – interacts with `RecommendationEngine`, `UserProfile`, and `SongRepository`.  
  - `FavoritesPage` – interacts with `UserProfile` and `SongRepository`.  

The class diagram shows these classes with their relationships:  
- `SongRepository` manages a collection of `Song`.  
- `RecommendationEngine` depends on `SongRepository`.  
- `SongVectorizer` is used by `RecommendationEngine` and/or `SongRepository` to construct feature vectors.  
- `UserProfile` references `Song` objects in favorites and playlists.  
- UI pages depend on the business layer classes but not directly on the database. 

---

### 3.2 Sequence Diagrams

Two key sequence diagrams are planned: 

1. **Searching for a Song and Getting Recommendations**

   - The user enters a query on `HomePage` and taps “Search”.  
   - `HomePage` calls `SongRepository.FindSongsByQuery(query)` to get a list of matches.  
   - The user selects a song from the results, and `HomePage` navigates to `SongDetailsPage` with the selected `Song`.  
   - On `SongDetailsPage`, the user taps “Get Recommendations”.  
   - `SongDetailsPage` calls `RecommendationEngine.GetSimilarSongs(selectedSong, count)`.  
   - `RecommendationEngine` requests all songs from `SongRepository`, calculates similarity scores, and returns the most similar ones.  
   - `SongDetailsPage` displays the recommended songs to the user.  

2. **Adding a Song to Favorites**

   - From `SongDetailsPage`, the user taps “Add to Favorites”.  
   - `SongDetailsPage` calls `UserProfile.AddFavorite(song)`.  
   - `UserProfile` updates its internal favorites collection and calls the data layer to persist changes.  
   - The data layer writes or updates the corresponding record in the database (e.g., in a `Favorites` table).  
   - A confirmation message or visual indicator on the UI confirms that the song is now in favorites.  

These sequence diagrams illustrate how the UI layer, business layer, and data layer collaborate while maintaining separation of concerns.

---

## 4. Data Design

NAMEYETTOBEDECIDED uses a relational SQL database as its primary data layer to persist songs, user favorites, playlists, and basic application settings. Data is stored in normalized tables and accessed through a small data‑access layer in C#, rather than directly via SQL from the UI. This design maintains a clear separation between presentation, business logic, and persistence while also treating SQL as a “new” technology component for the project.

### 4.1 Song Data

Songs are stored in a `Songs` table that includes both metadata and numerical feature vectors. Each row represents a single song and contains:

- A unique identifier (`Id` primary key).  
- Title, artist, album, duration, genre, and tempo.  
- A numeric feature vector used for similarity (stored either as a serialized string, JSON text, or via rows in a related `SongFeatures` table).  

Example schema (simplified):

```sql
CREATE TABLE Songs (
    Id              INTEGER PRIMARY KEY,
    Title           TEXT NOT NULL,
    Artist          TEXT NOT NULL,
    Album           TEXT,
    DurationSeconds INTEGER,
    Genre           TEXT,
    Tempo           INTEGER,
    FeaturesJson    TEXT    -- serialized feature vector, e.g. [0.8,0.4,0.7]
);
