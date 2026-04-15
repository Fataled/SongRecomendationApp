# PROG 10065 Term Project Report

Students use this file to complete the project proposal and then iterate on it to finalize it into a project report. Please consult the project proposal assignment which contains requirements for completing the proposal as well as the requirements for the project and its report.

# Project Hell's Paradise – Song Recommender

PROG10065 – Interactive Application Development  
Term Team Project – Project Proposal  
Team IA-11

- Oghenebrume Ako – ID  
- Obaid Waqas – ID  
- Rithvik Ganesh Konapala – 991845236

---

## 1. Project Overview

Project Hell's Paradise is a song recommendation application that helps users discover new music based on songs they already enjoy. The user selects or searches for a track, and the application computes a feature vector for that song and compares it against a collection of other songs to find the most similar ones. Recommendations are based on numerical similarity rather than just genre labels, making the suggestions more personalized and precise. 

The application is developed as a .NET MAUI app using C# and XAML in Visual Studio on Windows. It uses MAUI pages for the presentation layer, C# classes and collections (such as `List<T>` and `Dictionary<TKey, TValue>`) for the business logic, and a SQL database for its data layer. The app demonstrates object‑oriented principles such as encapsulation, separation of concerns, and reuse through well‑designed classes and methods. 

Users will interact with multiple screens to search for songs, view detailed information, see recommendations, and manage favorites or playlists. For users who have a premium music streaming subscription, Project Hell's Paradise can open the selected track in their external music player so they can listen to recommended songs directly. In addition, the project includes a “new” research component compared to the classroom examples: representing songs as numerical feature vectors and using similarity metrics to derive recommendations, together with using a SQL database as the persistence mechanism.

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

The main business classes of Project Hell's Paradise include: 

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

Project Hell's Paradise uses a relational SQL database as its primary data layer to persist songs, user favorites, playlists, and basic application settings. Data is stored in normalized tables and accessed through a small data‑access layer in C#, rather than directly via SQL from the UI. This design maintains a clear separation between presentation, business logic, and persistence while also treating SQL as a “new” technology component for the project.

### 4.1 Song Data

Songs are stored in a `Songs` table that includes both metadata and numerical feature vectors. Each row represents a single song and contains:

- A unique identifier (`Id` primary key).  
- Title, artist, album, duration, genre, and tempo.  
- A numeric feature vector used for similarity (stored either as a serialized string, JSON text, or via rows in a related `SongFeatures` table).  

Example schema (simplified):

```sql
CREATE TABLE User (
    Id              TEXT PRIMARY KEY,
    Email           TEXT NOT NULL,
    Name            TEXT,
    Hashed_picture  TEXT,
    created_at      DATE,
    totp_secret     INTEGER,
    mfa_enabled     Boolean,
    reset_password_token TEXT,
    reset_pasword_expiry DATE,
    refresh_token   TEXT   
);

CREATE TABLE Events(
    event_name TEXT,
    usder_id TEXT,
    timestamp Date,
    properties TEXT
)
```
## 5. Work Assignments

Each team member is responsible for a portion of the presentation layer, business layer, and data layer. Responsibilities are divided so that every member implements UI, logic, and data programming for their features, in line with the project requirements.

| Contribution        | Oghenebrume Ako (Brume)      | Obaid Waqas                           | Rithvik Ganesh Konapala (Rithvik) |
|---------------------|------------------------------|---------------------------------------|-----------------------------------|
| Presentation Layer  | Search System UI             | Random Song Choice, Add to Playlists | Favorites / Playlists Page        |
| Business Layer      | Auth, Search Functionality   | Song Recommendation Logic            | Encryption, SQL Queries           |
| Data Layer          | Genre Management for Songs   | Spotify Playlist Integration         | Database & Query Management       |

### 5.1 Oghenebrume Ako (Brume)

- **Presentation:**  
  - Implement the Search System UI using MAUI and XAML, including search controls, filters, and result list display.  
- **Business Logic:**  
  - Implement authentication logic and the core search functionality to query songs by title, artist, or genre.  
  - Manage user login and session handling.  
- **Data:**  
  - Design and implement genre management, including adding/updating genres to songs and organizing song data by genre.

### 5.2 Obaid Waqas

- **Presentation:**  
  - Implement the Random Song Choice feature and the ability to add songs to playlists directly from the UI.  
- **Business Logic:**  
  - Implement random song selection logic and recommendation algorithms to suggest songs based on user preferences.  
  - Handle the ability to add selected songs to Spotify playlists.  
- **Data:**  
  - Integrate with Spotify's playlist API to save user selections and manage Spotify playlist data.

### 5.3 Rithvik Ganesh Konapala (Rithvik)

- **Presentation:**  
  - Implement the Favorites / Playlists management page using MAUI and XAML for displaying and managing user collections.  
- **Business Logic:**  
  - Implement encryption logic for sensitive user data (e.g., authentication tokens, user preferences).  
  - Design core encryption/decryption utilities used throughout the app.  
- **Data:**  
  - Implement SQL queries for database operations (favorites, playlists, user data).  
  - Design and maintain the database schema and all data-access code for persistence.

---

## 6. Notes

- This proposal represents the initial planned design for Project Hell's Paradise and may be updated as the project evolves.  
- During development, features, screens, and classes may be refined, added, or removed while still respecting the course requirements for presentation, business logic, and data layers, as well as the use of MAUI, collections, and persistent storage.  
- The final project report will update this proposal with the completed design, diagrams, screenshots, and a link to the demonstration video.
- This is a pretty complex project though it sounds easy we will be using both C# Maui and python in order to make this work as well as quite a few different apis.
