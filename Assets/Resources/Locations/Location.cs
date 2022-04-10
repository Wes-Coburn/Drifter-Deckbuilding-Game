using UnityEngine;

[CreateAssetMenu(fileName = "New Location", menuName = "Locations/Location")]
public class Location : ScriptableObject
{
    [Header("LOCATION DETAILS")]
    [SerializeField] private bool isPriorityLocation;
    [SerializeField] private string locationName;
    [SerializeField] private string locationFullName;
    [SerializeField] private string locationDescription;
    [SerializeField] private string firstObjective;
    [SerializeField] private Vector2 worldMapPosition;
    [SerializeField] private NPCHero firstNPC;
    [SerializeField] private Sound locationSoundscape;

    [Header("LOCATION TYPE")]
    [SerializeField] private bool isHomeBase;
    [SerializeField] private bool isAugmenter;
    [SerializeField] private bool isRecruitment;
    [SerializeField] private bool isShop;
    [SerializeField] private bool isCloning;
    [SerializeField] private bool isRandomEncounter;

    [Header("CLOSED HOURS")]
    [SerializeField] [Tooltip("Morning (Hour 1)")] private bool isClosed_Hour1;
    [SerializeField] [Tooltip("Day (Hour 2)")] private bool isClosed_Hour2;
    [SerializeField] [Tooltip("Evening (Hour 3)")] private bool isClosed_Hour3;

    public bool IsPriorityLocation { get => isPriorityLocation; }
    public string LocationName { get => locationName; }
    public string LocationFullName { get => locationFullName; }
    public string LocationDescription { get => locationDescription; }
    public string FirstObjective { get => firstObjective; }
    public string CurrentObjective { get; set; }
    public Vector2 WorldMapPosition { get => worldMapPosition; }
    public NPCHero FirstNPC { get => firstNPC; }
    public Sound LocationSoundscape { get => locationSoundscape; }
    public bool IsHomeBase { get => isHomeBase; }
    public bool IsAugmenter { get => isAugmenter; }
    public bool IsRecruitment { get => isRecruitment; }
    public bool IsShop { get => isShop; }
    public bool IsCloning { get => isCloning; }
    public bool IsRandomEncounter { get => isRandomEncounter; }
    public bool IsClosed_Hour1 { get => isClosed_Hour1; }
    public bool IsClosed_Hour2 { get => isClosed_Hour2; }
    public bool IsClosed_Hour3 { get => isClosed_Hour3; }

    public NPCHero CurrentNPC { get; set; }
    
    public void LoadLocation(Location location)
    {
        isPriorityLocation = location.IsPriorityLocation;
        locationName = location.LocationName;
        locationFullName = location.LocationFullName;
        locationDescription = location.LocationDescription;
        firstObjective = location.FirstObjective;
        worldMapPosition = location.WorldMapPosition;
        firstNPC = location.FirstNPC;
        locationSoundscape = location.LocationSoundscape;
        isHomeBase = location.IsHomeBase;
        isAugmenter = location.IsAugmenter;
        isRecruitment = location.IsRecruitment;
        isShop = location.IsShop;
        isCloning = location.IsCloning;
        isRandomEncounter = location.IsRandomEncounter;
        isClosed_Hour1 = location.IsClosed_Hour1;
        isClosed_Hour2 = location.IsClosed_Hour2;
        isClosed_Hour3 = location.IsClosed_Hour3;
    }
}
