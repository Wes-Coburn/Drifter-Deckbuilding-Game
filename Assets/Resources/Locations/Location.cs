using UnityEngine;

[CreateAssetMenu(fileName = "New Location", menuName = "Locations/Location")]
public class Location : ScriptableObject
{
    [SerializeField] [TextArea] private string developerNotes;
    [SerializeField] private string locationName;
    [SerializeField] private string locationFullName;
    [SerializeField] private string locationDescription;
    [SerializeField] private string firstObjective;
    [SerializeField] private Vector2 worldMapPosition;
    [SerializeField] private NPCHero firstNPC;
    [SerializeField] private Sound locationSoundscape;
    [SerializeField] private bool isHomeBase;
    [SerializeField] private bool isRecruitment;
    [SerializeField] private bool isShop;
    [SerializeField] private bool isCloning;

    public string LocationName { get => locationName; }
    public string LocationFullName { get => locationFullName; }
    public string LocationDescription { get => locationDescription; }
    public string FirstObjective { get => firstObjective; }
    public string CurrentObjective { get; set; }
    public Vector2 WorldMapPosition { get => worldMapPosition; }
    public NPCHero FirstNPC { get => firstNPC; }
    public Sound LocationSoundscape { get => locationSoundscape; }
    public bool IsHomeBase { get => isHomeBase; }
    public bool IsRecruitment { get => isRecruitment; }
    public bool IsShop { get => isShop; }
    public bool IsCloning { get => isCloning; }
    public NPCHero CurrentNPC { get; set; }
    
    public void LoadLocation(Location location)
    {
        locationName = location.LocationName;
        locationFullName = location.LocationFullName;
        locationDescription = location.LocationDescription;
        firstObjective = location.FirstObjective;
        worldMapPosition = location.WorldMapPosition;
        firstNPC = location.FirstNPC;
        locationSoundscape = location.LocationSoundscape;
        isHomeBase = location.IsHomeBase;
        isRecruitment = location.IsRecruitment;
        isShop = location.IsShop;
        isCloning = location.IsCloning;
    }
}
