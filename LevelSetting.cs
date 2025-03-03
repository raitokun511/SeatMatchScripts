using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelSetting : MonoBehaviour
{
    public static LevelSetting instance;
    public List<LevelConstructor> Levels;
    public GameObject[] BlockPrefab;
    public List<GameObject> ChairPrefab;
    public GameObject DoorPrefab;
    public List<GameObject> CitizenPrefab;
    public List<Material> CitizenMaterials;
    [Header("ArmChair Material")]
    public List<Material> ArmChairMaterial;
    [Header("Couch Material")]
    public List<Material> CouchMaterial;
    public Material transparentMaterial;
    //public GameObject ConfettiPrefab;

    public Dictionary<ColorType, GameObject> Colors;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject InitBlock(Vector3 position)
    {
        GameObject block = Instantiate(BlockPrefab[(int)(position.x + position.z) % 2 == 0 ? 0 : 1], position, Quaternion.identity);
        //Block blockComponent = block.GetComponent<Block>();
        //blockComponent.Coordinates = new Vector2(position.x, position.z);
        return block;
    }
    public Chair InitChair(ChairCoor chairCoor)
    {
        Vector3 position = new Vector3(chairCoor.Coor.x, 0, chairCoor.Coor.y);
        GameObject chairObject = Instantiate(ChairPrefab[(int)chairCoor.ChairType], position, Quaternion.identity);
        MeshRenderer chairMesh = chairObject.transform.GetChild(0).GetComponent<MeshRenderer>();
        if (chairMesh != null)
        {
            chairObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = ArmChairMaterial[(int)chairCoor.ColorType];
        }
        chairObject.transform.position += GM.GroundPadding;
        chairObject.transform.position += GM.ChairOffsets[chairCoor.ChairType];
        Chair chair = chairObject.GetComponent<Chair>();
        chair.chairCoor = chairCoor.Clone();

        return chair;
    }
    public GameObject InitDoor(Vector2Int doorCoor)
    {
        Vector3 position = new Vector3(doorCoor.x, 0, doorCoor.y);
        GameObject doorObject = Instantiate(DoorPrefab, position, Quaternion.identity);
        doorObject.transform.position += GM.GroundPadding;
        doorObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        return doorObject;
    }

    public Citizen InitCitizen(Vector2Int citizenCoor, ColorType citizenColor)
    {
        Vector3 position = new Vector3(citizenCoor.x, 0.5f, citizenCoor.y);
        GameObject citizenObject = Instantiate(CitizenPrefab[(int)citizenColor], position, Quaternion.identity);
        citizenObject.transform.position += GM.GroundPadding;
        citizenObject.transform.rotation = Quaternion.Euler(0, 85, 0);
        Citizen citizen = citizenObject.GetComponent<Citizen>();
        citizen.ColorType = citizenColor;
        return citizen;
    }


}
