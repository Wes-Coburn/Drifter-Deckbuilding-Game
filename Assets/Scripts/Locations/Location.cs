using UnityEngine;

[CreateAssetMenu(fileName = "New Location", menuName = "Locations/Location")]
public class Location : ScriptableObject
{
    [Header("LOCATION DETAILS")]
    [SerializeField] private bool isPriorityLocation;
    [SerializeField] private string locationName, locationFullName, locationDescription, firstObjective;
    [SerializeField] private Vector2 worldMapPosition;
    [SerializeField] private NPCHero firstNPC;

    [Header("LOCATION ASSETS")]
    [SerializeField] private Sound locationSoundscape;
    [SerializeField] private Background locationBackground;

    [Header("LOCATION TYPE")]
    [SerializeField] private bool isRecurring;
    [SerializeField] private bool isCombatOnly, isRandomEncounter, isHomeBase,
        isRecruitment, isActionShop, isItemShop, isCloner, isAugmenter, isHealer;

    [Header("CLOSED HOURS")]
    [SerializeField][Tooltip("Morning (Hour 1)")] private bool isClosed_Hour1;
    [SerializeField][Tooltip("Day (Hour 2)")] private bool isClosed_Hour2;
    [SerializeField][Tooltip("Evening (Hour 3)")] private bool isClosed_Hour3;

    public enum Background
    {
        City,
        Wasteland
    }

    public bool IsPriorityLocation { get => isPriorityLocation; }
    public string LocationName { get => locationName; }
    public string LocationFullName { get => locationFullName; }
    public string LocationDescription { get => locationDescription; }
    public string FirstObjective { get => firstObjective; }
    public Vector2 WorldMapPosition { get => worldMapPosition; }
    public NPCHero FirstNPC { get => firstNPC; }
    public Sound LocationSoundscape { get => locationSoundscape; }
    public Background LocationBackground { get => locationBackground; }
    public bool IsRecurring { get => isRecurring; }
    public bool IsCombatOnly { get => isCombatOnly; }
    public bool IsRandomEncounter { get => isRandomEncounter; }
    public bool IsHomeBase { get => isHomeBase; }
    public bool IsRecruitment { get => isRecruitment; }
    public bool IsActionShop { get => isActionShop; }
    public bool IsItemShop { get => isItemShop; }
    public bool IsCloner { get => isCloner; }
    public bool IsHealer { get => isHealer; }
    public bool IsAugmenter { get => isAugmenter; }
    public bool IsClosed_Hour1 { get => isClosed_Hour1; }
    public bool IsClosed_Hour2 { get => isClosed_Hour2; }
    public bool IsClosed_Hour3 { get => isClosed_Hour3; }

    public string CurrentObjective { get; set; }
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
        locationBackground = location.LocationBackground;
        isRecurring = location.IsRecurring;
        isCombatOnly = location.IsCombatOnly;
        isRandomEncounter = location.IsRandomEncounter;
        isHomeBase = location.IsHomeBase;
        isRecruitment = location.IsRecruitment;
        isActionShop = location.IsActionShop;
        isItemShop = location.IsItemShop;
        isCloner = location.IsCloner;
        isAugmenter = location.IsAugmenter;
        isHealer = location.IsHealer;
        isClosed_Hour1 = location.IsClosed_Hour1;
        isClosed_Hour2 = location.IsClosed_Hour2;
        isClosed_Hour3 = location.IsClosed_Hour3;
    }
}
