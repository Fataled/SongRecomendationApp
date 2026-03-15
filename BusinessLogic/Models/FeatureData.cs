using ProjectHellsParadise.BusinessLogic.Data_Transfer_Object;

namespace ProjectHellsParadise.BusinessLogic.Models;

public class FeatureData
{
    
        private static readonly Dictionary<string, string> EssentiaGenreMap = new()
{
    // Blues
    { "Blues---Boogie Woogie",          "blues" },
    { "Blues---Chicago Blues",          "blues" },
    { "Blues---Country Blues",          "blues" },
    { "Blues---Delta Blues",            "blues" },
    { "Blues---Electric Blues",         "blues" },
    { "Blues---Harmonica Blues",        "blues" },
    { "Blues---Jump Blues",             "blues" },
    { "Blues---Louisiana Blues",        "blues" },
    { "Blues---Modern Electric Blues",  "blues" },
    { "Blues---Piano Blues",            "blues" },
    { "Blues---Rhythm & Blues",         "r&b" },
    { "Blues---Soul-Blues",             "soul" },

    // Brass & Military
    { "Brass & Military---Brass Band",   "folk" },
    { "Brass & Military---Marching Band","folk" },
    { "Brass & Military---Military",     "folk" },

    // Children's
    { "Children's---Educational",        "kids" },
    { "Children's---Nursery Rhymes",     "kids" },
    { "Children's---Story",              "kids" },

    // Classical
    { "Classical---Baroque",             "classical" },
    { "Classical---Chamber Music",       "classical" },
    { "Classical---Choral",              "classical" },
    { "Classical---Classical",           "classical" },
    { "Classical---Contemporary",        "classical" },
    { "Classical---Impressionist",       "classical" },
    { "Classical---Medieval",            "classical" },
    { "Classical---Modern",              "classical" },
    { "Classical---Neo-Classical",       "classical" },
    { "Classical---Neo-Romantic",        "classical" },
    { "Classical---Opera",               "classical" },
    { "Classical---Romantic",            "classical" },
    { "Classical---Symphony",            "classical" },

    // Electronic
    { "Electronic---Abstract",           "electronic" },
    { "Electronic---Ambient",            "electronic" },
    { "Electronic---Baltimore Club",     "electronic" },
    { "Electronic---Bassline",           "electronic" },
    { "Electronic---Beatdown",           "electronic" },
    { "Electronic---Berlin-School",      "electronic" },
    { "Electronic---Big Beat",           "electronic" },
    { "Electronic---Breakbeat",          "electronic" },
    { "Electronic---Breakcore",          "electronic" },
    { "Electronic---Breaks",             "electronic" },
    { "Electronic---Broken Beat",        "electronic" },
    { "Electronic---Chillwave",          "electronic" },
    { "Electronic---Chiptune",           "electronic" },
    { "Electronic---Dance-pop",          "dance" },
    { "Electronic---Dark Ambient",       "electronic" },
    { "Electronic---Darkwave",           "electronic" },
    { "Electronic---Deep House",         "electronic" },
    { "Electronic---Detroit Techno",     "electronic" },
    { "Electronic---Disco",              "dance" },
    { "Electronic---Djent",              "metal" },
    { "Electronic---Drone",              "electronic" },
    { "Electronic---Drum n Bass",        "electronic" },
    { "Electronic---Dub",                "reggae" },
    { "Electronic---Dub Techno",         "electronic" },
    { "Electronic---Dubstep",            "electronic" },
    { "Electronic---Dungeon Synth",      "electronic" },
    { "Electronic---EBM",                "electronic" },
    { "Electronic---Electro",            "electronic" },
    { "Electronic---Electroclash",       "electronic" },
    { "Electronic---Euro House",         "dance" },
    { "Electronic---Euro-Disco",         "dance" },
    { "Electronic---Eurodance",          "dance" },
    { "Electronic---Experimental",       "electronic" },
    { "Electronic---Footwork",           "electronic" },
    { "Electronic---Freestyle",          "electronic" },
    { "Electronic---Future Jazz",        "jazz" },
    { "Electronic---Gabber",             "electronic" },
    { "Electronic---Garage House",       "electronic" },
    { "Electronic---Global Bass",        "electronic" },
    { "Electronic---Grime",              "hip hop" },
    { "Electronic---Halftime",           "electronic" },
    { "Electronic---Hands Up",           "dance" },
    { "Electronic---Happy Hardcore",     "electronic" },
    { "Electronic---Hard House",         "electronic" },
    { "Electronic---Hard Techno",        "electronic" },
    { "Electronic---Hard Trance",        "electronic" },
    { "Electronic---Hardcore",           "electronic" },
    { "Electronic---Hardstyle",          "electronic" },
    { "Electronic---Hi NRG",             "dance" },
    { "Electronic---Hip Hop",            "hip hop" },
    { "Electronic---House",              "electronic" },
    { "Electronic---IDM",                "electronic" },
    { "Electronic---Illbient",           "electronic" },
    { "Electronic---Industrial",         "electronic" },
    { "Electronic---Italo-Disco",        "dance" },
    { "Electronic---J-Core",             "electronic" },
    { "Electronic---Jazzdance",          "jazz" },
    { "Electronic---Juke",               "electronic" },
    { "Electronic---Jumpstyle",          "dance" },
    { "Electronic---Jungle",             "electronic" },
    { "Electronic---Latin House",        "latin music" },
    { "Electronic---Lo-Fi",              "electronic" },
    { "Electronic---Leftfield",          "electronic" },
    { "Electronic---Makina",             "electronic" },
    { "Electronic---Minimal",            "electronic" },
    { "Electronic---Minimal Techno",     "electronic" },
    { "Electronic---Modern Classical",   "classical" },
    { "Electronic---Musique Concrète",   "electronic" },
    { "Electronic---New Beat",           "electronic" },
    { "Electronic---New Wave",           "alternative" },
    { "Electronic---Noise",              "electronic" },
    { "Electronic---Nu-Disco",           "dance" },
    { "Electronic---Power Electronics",  "electronic" },
    { "Electronic---Progressive Breaks", "electronic" },
    { "Electronic---Progressive House",  "electronic" },
    { "Electronic---Progressive Trance", "electronic" },
    { "Electronic---Psy-Trance",         "electronic" },
    { "Electronic---Rhythmic Noise",     "electronic" },
    { "Electronic---Schranz",            "electronic" },
    { "Electronic---Sound Collage",      "electronic" },
    { "Electronic---Speedcore",          "electronic" },
    { "Electronic---Synthpop",           "pop" },
    { "Electronic---Synthwave",          "electronic" },
    { "Electronic---Tech House",         "electronic" },
    { "Electronic---Tech Trance",        "electronic" },
    { "Electronic---Techno",             "electronic" },
    { "Electronic---Trance",             "electronic" },
    { "Electronic---Tribal",             "electronic" },
    { "Electronic---Tribal House",       "electronic" },
    { "Electronic---Trip Hop",           "hip hop" },
    { "Electronic---Tropical House",     "electronic" },
    { "Electronic---UK Garage",          "electronic" },
    { "Electronic---Vaporwave",          "electronic" },

    // Folk World & Country
    { "Folk World & Country---African",          "african music" },
    { "Folk World & Country---Bluegrass",        "country" },
    { "Folk World & Country---Cajun",            "folk" },
    { "Folk World & Country---Celtic",           "folk" },
    { "Folk World & Country---Country",          "country" },
    { "Folk World & Country---Fado",             "folk" },
    { "Folk World & Country---Flamenco",         "latin music" },
    { "Folk World & Country---Folk",             "folk" },
    { "Folk World & Country---Gospel",           "soul" },
    { "Folk World & Country---Highlife",         "african music" },
    { "Folk World & Country---Hillbilly",        "country" },
    { "Folk World & Country---Hindustani",       "indian music" },
    { "Folk World & Country---Honky Tonk",       "country" },
    { "Folk World & Country---Indian Classical", "indian music" },
    { "Folk World & Country---Isicathamiya",     "african music" },
    { "Folk World & Country---Jazz",             "jazz" },
    { "Folk World & Country---Jugband",          "folk" },
    { "Folk World & Country---Jùjú",             "african music" },
    { "Folk World & Country---Latin",            "latin music" },
    { "Folk World & Country---Mbaqanga",         "african music" },
    { "Folk World & Country---Mbube",            "african music" },
    { "Folk World & Country---Merengue",         "latin music" },
    { "Folk World & Country---Native American",  "folk" },
    { "Folk World & Country---Polka",            "folk" },
    { "Folk World & Country---Romani",           "folk" },
    { "Folk World & Country---Soukous",          "african music" },
    { "Folk World & Country---Tango",            "latin music" },
    { "Folk World & Country---Volksmusik",       "folk" },
    { "Folk World & Country---Zydeco",           "folk" },

    // Funk Soul
    { "Funk Soul---Contemporary R&B",  "r&b" },
    { "Funk Soul---Disco",             "dance" },
    { "Funk Soul---Free Funk",         "soul" },
    { "Funk Soul---Funk",              "soul" },
    { "Funk Soul---Gospel",            "soul" },
    { "Funk Soul---Neo Soul",          "r&b" },
    { "Funk Soul---New Jack Swing",    "r&b" },
    { "Funk Soul---P.Funk",            "soul" },
    { "Funk Soul---Quiet Storm",       "r&b" },
    { "Funk Soul---Rhythm & Blues",    "r&b" },
    { "Funk Soul---Soul",              "soul" },

    // Hip Hop
    { "Hip Hop---Abstract",        "hip hop" },
    { "Hip Hop---Bass Music",      "hip hop" },
    { "Hip Hop---Beatbox",         "hip hop" },
    { "Hip Hop---Boom Bap",        "hip hop" },
    { "Hip Hop---Bounce",          "hip hop" },
    { "Hip Hop---Britcore",        "hip hop" },
    { "Hip Hop---Cloud Rap",       "rap" },
    { "Hip Hop---Conscious",       "hip hop" },
    { "Hip Hop---Crunk",           "hip hop" },
    { "Hip Hop---Cut-up/DJ",       "hip hop" },
    { "Hip Hop---Dirty South",     "rap" },
    { "Hip Hop---East Coast",      "rap" },
    { "Hip Hop---Freestyle",       "rap" },
    { "Hip Hop---G-Funk",          "rap" },
    { "Hip Hop---Gangsta",         "rap" },
    { "Hip Hop---Grime",           "hip hop" },
    { "Hip Hop---Hardcore",        "rap" },
    { "Hip Hop---Horrorcore",      "rap" },
    { "Hip Hop---Instrumental",    "hip hop" },
    { "Hip Hop---Jazz-Rap",        "hip hop" },
    { "Hip Hop---Latin",           "latin music" },
    { "Hip Hop---Lo-Fi",           "hip hop" },
    { "Hip Hop---Memphis Rap",     "rap" },
    { "Hip Hop---Miami Bass",      "hip hop" },
    { "Hip Hop---Midwest",         "rap" },
    { "Hip Hop---Mumble Rap",      "rap" },
    { "Hip Hop---Neo Soul",        "r&b" },
    { "Hip Hop---Nerdcore",        "hip hop" },
    { "Hip Hop---Old School",      "hip hop" },
    { "Hip Hop---Political",       "hip hop" },
    { "Hip Hop---Pop Rap",         "pop" },
    { "Hip Hop---Psychedelic",     "hip hop" },
    { "Hip Hop---Ragga HipHop",    "reggae" },
    { "Hip Hop---Rap",             "rap" },
    { "Hip Hop---Screwed",         "rap" },
    { "Hip Hop---Snap",            "hip hop" },
    { "Hip Hop---Southern",        "rap" },
    { "Hip Hop---Trap",            "rap" },
    { "Hip Hop---Trip Hop",        "hip hop" },
    { "Hip Hop---UK Hip Hop",      "hip hop" },
    { "Hip Hop---West Coast",      "rap" },

    // Jazz
    { "Jazz---Afro-Cuban Jazz",       "jazz" },
    { "Jazz---Avant-garde Jazz",      "jazz" },
    { "Jazz---Big Band",              "jazz" },
    { "Jazz---Bossa Nova",            "latin music" },
    { "Jazz---Contemporary Jazz",     "jazz" },
    { "Jazz---Cool Jazz",             "jazz" },
    { "Jazz---Dixieland",             "jazz" },
    { "Jazz---Ethio-Jazz",            "african music" },
    { "Jazz---European Free Jazz",    "jazz" },
    { "Jazz---Free Jazz",             "jazz" },
    { "Jazz---Fusion",                "jazz" },
    { "Jazz---Gypsy Jazz",            "jazz" },
    { "Jazz---Hard Bop",              "jazz" },
    { "Jazz---Jazz-Funk",             "jazz" },
    { "Jazz---Jazz-Rock",             "jazz" },
    { "Jazz---Latin Jazz",            "latin music" },
    { "Jazz---Modal",                 "jazz" },
    { "Jazz---Post Bop",              "jazz" },
    { "Jazz---Smooth Jazz",           "jazz" },
    { "Jazz---Soul-Jazz",             "jazz" },
    { "Jazz---Swing",                 "jazz" },
    { "Jazz---Vocal",                 "jazz" },

    // Latin
    { "Latin---Afrobeat",          "african music" },
    { "Latin---Baião",             "brazillian music" },
    { "Latin---Batucada",          "brazillian music" },
    { "Latin---Beguine",           "latin music" },
    { "Latin---Bolero",            "latin music" },
    { "Latin---Boogaloo",          "latin music" },
    { "Latin---Bossanova",         "brazillian music" },
    { "Latin---Cha-Cha",           "latin music" },
    { "Latin---Charanga",          "latin music" },
    { "Latin---Compas",            "latin music" },
    { "Latin---Cumbia",            "latin music" },
    { "Latin---Descarga",          "latin music" },
    { "Latin---Forró",             "brazillian music" },
    { "Latin---Guaguancó",         "latin music" },
    { "Latin---Guajira",           "latin music" },
    { "Latin---Guaracha",          "latin music" },
    { "Latin---Joropo",            "latin music" },
    { "Latin---Mambo",             "latin music" },
    { "Latin---Mariachi",          "latin music" },
    { "Latin---Merengue",          "latin music" },
    { "Latin---Mpb",               "brazillian music" },
    { "Latin---Norteño",           "latin music" },
    { "Latin---Nueva Cancion",     "latin music" },
    { "Latin---Pachanga",          "latin music" },
    { "Latin---Porro",             "latin music" },
    { "Latin---Ranchera",          "latin music" },
    { "Latin---Reggaeton",         "latin music" },
    { "Latin---Rumba",             "latin music" },
    { "Latin---Salsa",             "latin music" },
    { "Latin---Samba",             "brazillian music" },
    { "Latin---Son",               "latin music" },
    { "Latin---Son Montuno",       "latin music" },
    { "Latin---Tango",             "latin music" },
    { "Latin---Tejano",            "latin music" },
    { "Latin---Timba",             "latin music" },
    { "Latin---Tropicália",        "brazillian music" },
    { "Latin---Vallenato",         "latin music" },

    // Non-Music
    { "Non-Music---Spoken Word",   "kids" },
    { "Non-Music---Noise",         "electronic" },

    // Pop
    { "Pop---Ballad",          "pop" },
    { "Pop---Bubblegum",       "pop" },
    { "Pop---Chamber Pop",     "pop" },
    { "Pop---City Pop",        "pop" },
    { "Pop---Dance-pop",       "dance" },
    { "Pop---Europop",         "pop" },
    { "Pop---Indie Pop",       "alternative" },
    { "Pop---J-pop",           "asian music" },
    { "Pop---K-pop",           "asian music" },
    { "Pop---Kayokyoku",       "asian music" },
    { "Pop---Neo Soul",        "r&b" },
    { "Pop---Noise Pop",       "alternative" },
    { "Pop---Psychedelic Rock","rock" },
    { "Pop---Schlager",        "pop" },
    { "Pop---Soft Rock",       "rock" },
    { "Pop---Sophisti-Pop",    "pop" },
    { "Pop---Synth-pop",       "pop" },
    { "Pop---Teen Pop",        "pop" },
    { "Pop---Twee Pop",        "pop" },

    // Reggae
    { "Reggae---Calypso",      "reggae" },
    { "Reggae---Dancehall",    "reggae" },
    { "Reggae---Dub",          "reggae" },
    { "Reggae---Lovers Rock",  "reggae" },
    { "Reggae---Ragga",        "reggae" },
    { "Reggae---Reggae",       "reggae" },
    { "Reggae---Reggae-Pop",   "reggae" },
    { "Reggae---Rocksteady",   "reggae" },
    { "Reggae---Roots Reggae", "reggae" },
    { "Reggae---Ska",          "reggae" },
    { "Reggae---Soca",         "reggae" },

    // Rock
    { "Rock---Acid Rock",              "rock" },
    { "Rock---Acoustic",               "folk" },
    { "Rock---Alternative Rock",       "alternative" },
    { "Rock---Arena Rock",             "rock" },
    { "Rock---Art Rock",               "rock" },
    { "Rock---Atmospheric Black Metal","metal" },
    { "Rock---Avantgarde",             "alternative" },
    { "Rock---Beat",                   "rock" },
    { "Rock---Black Metal",            "metal" },
    { "Rock---Blues Rock",             "blues" },
    { "Rock---Brit Pop",               "pop" },
    { "Rock---Classic Rock",           "rock" },
    { "Rock---Country Rock",           "country" },
    { "Rock---Crust",                  "metal" },
    { "Rock---Death Metal",            "metal" },
    { "Rock---Deathcore",              "metal" },
    { "Rock---Doom Metal",             "metal" },
    { "Rock---Dream Pop",              "alternative" },
    { "Rock---Emo",                    "alternative" },
    { "Rock---Folk Rock",              "folk" },
    { "Rock---Glam Rock",              "rock" },
    { "Rock---Goregrind",              "metal" },
    { "Rock---Goth Rock",              "alternative" },
    { "Rock---Grindcore",              "metal" },
    { "Rock---Grunge",                 "alternative" },
    { "Rock---Hard Rock",              "rock" },
    { "Rock---Hardcore",               "metal" },
    { "Rock---Heavy Metal",            "metal" },
    { "Rock---Indie Rock",             "alternative" },
    { "Rock---Industrial",             "electronic" },
    { "Rock---J-Rock",                 "asian music" },
    { "Rock---NWOBHM",                 "metal" },
    { "Rock---Neo-Psychedelia",        "alternative" },
    { "Rock---New Wave",               "alternative" },
    { "Rock---No Wave",                "alternative" },
    { "Rock---Noise",                  "alternative" },
    { "Rock---Noise Rock",             "alternative" },
    { "Rock---Nu Metal",               "metal" },
    { "Rock---Oi",                     "rock" },
    { "Rock---Outsider",               "alternative" },
    { "Rock---Pop Rock",               "rock" },
    { "Rock---Post Rock",              "alternative" },
    { "Rock---Post-Hardcore",          "metal" },
    { "Rock---Post-punk",              "alternative" },
    { "Rock---Power Metal",            "metal" },
    { "Rock---Power Pop",              "pop" },
    { "Rock---Prog Rock",              "rock" },
    { "Rock---Psychedelic Rock",       "rock" },
    { "Rock---Punk",                   "rock" },
    { "Rock---Rockabilly",             "rock" },
    { "Rock---Shoegaze",               "alternative" },
    { "Rock---Ska",                    "reggae" },
    { "Rock---Slowcore",               "alternative" },
    { "Rock---Sludge Metal",           "metal" },
    { "Rock---Soul",                   "soul" },
    { "Rock---Southern Rock",          "rock" },
    { "Rock---Space Rock",             "rock" },
    { "Rock---Speed Metal",            "metal" },
    { "Rock---Stoner Rock",            "rock" },
    { "Rock---Surf",                   "rock" },
    { "Rock---Symphonic Rock",         "rock" },
    { "Rock---Technical Death Metal",  "metal" },
    { "Rock---Thrash",                 "metal" },
    { "Rock---Viking Metal",           "metal" },

    // Stage & Screen
    { "Stage & Screen---Musical",   "films" },
    { "Stage & Screen---Score",     "films" },
    { "Stage & Screen---Soundtrack","films" },
    { "Stage & Screen---Theme",     "games" },
};
        
    private string _songName;
    private string _artist;
    private FeatureExtractionDTO _features;
    private GenrePredictionDTO[] _genre;
    private byte[] _songBytes;
    private float[] _vector;
    
    public FeatureData(string songName, string artist, FeatureExtractionDTO DTO, byte[] songBytes)
    {
        _songName = songName;
        _artist = artist;
        _features = DTO;
        _songBytes = songBytes;
        _genre = [];
    }

    public FeatureData()
    {
        _songName = "";
        _artist = "";
        _features = new FeatureExtractionDTO();
        _songBytes = [];
        _genre = [];
    }
    
    public string SongName => _songName;

    public string Artist => _artist;

    public FeatureExtractionDTO Features => _features;
    
    public GenrePredictionDTO[] Genre { get => _genre; set => _genre = value; }
    
    public byte[] SongBytes => _songBytes;
    
    public float[] Vector { get => _vector; set => _vector = value; }
    
    string MapGenre(string essentiaLabel)
    {
        return EssentiaGenreMap.TryGetValue(essentiaLabel, out var mapped) 
            ? mapped 
            : "pop"; // fallback
    }
    public override string ToString()
    {
        return $"{_songName} - {_artist}: {_features} {string.Join(", ", _genre.Select(p => $"{p.label} ({p.score:P2})"))}";
    }

    public GenrePredictionDTO[] GetMainGenres()
    {
        return _genre
            .Where(s => s.score > 0.1f)
            .Select(s => new GenrePredictionDTO
            {
                label = MapGenre(s.label),
                score = s.score
            }).DistinctBy(s => s.label)
            .ToArray();
    }
    
}