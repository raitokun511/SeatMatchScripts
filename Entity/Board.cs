using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;
    public BoardState state;
    public GameObject[] ConfettiEffect;
    Plane plane;

    public LevelSetting levelSetting;
    List<GameObject> blockList = new List<GameObject>();
    public List<Chair> chairList = new List<Chair> ();
    public List<List<Citizen>> citizenList = new List<List<Citizen>>();
    public Dictionary<(int, int), Seat> seatDict = new Dictionary<(int, int), Seat>();
    public Dictionary<(int, int), SeatState> BoardSeatValue = new Dictionary<(int, int), SeatState>();
    public Dictionary<(int, int), GameObject> BlockDictionary = new Dictionary<(int, int), GameObject>();

    Chair selectedChair = null;
    public float moveSpeed = 0.2f;
    bool flag = true;
    int numberCitizen = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        state = BoardState.Init;
        plane = new Plane(gameObject.transform.up, gameObject.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //For debug
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("[BoardState]:" + state);
        }

        if (state == BoardState.CitizenRun)
        {

        }

        if (state == BoardState.CitizenFind)
        {
            //Citizen citizen = citizenList[0][0];
            StartCoroutine(CitizenFind());
            Debug.Log("[Set]CitiRun");
        }


        if (state == BoardState.WaitChoose)
        {
            /*
            if (flag)
            {
                foreach (Chair chair in chairList)
                {
                    if (chair.chairCoor.Coor.x == 0)
                    {
                        //chair.gameObject.SetActive(false);
                    }
                }
                flag = false;
            }
            */
            if (selectedChair == null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    int chairLayer = LayerMask.NameToLayer("Chair");
                    int chairLayerMask = 1 << chairLayer;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, chairLayerMask))
                    {
                        Chair tmpChair = hit.collider.GetComponent<Chair>();
                        if (tmpChair != null && !tmpChair.LockedByCitizen && tmpChair.chairCoor.ColorType != ColorType.Gray)
                        {
                            //Debug.Log("Raycast hit:" + hit.collider.name);
                            selectedChair = hit.collider.GetComponent<Chair>();
                            RemoveBoardSeatState(selectedChair.chairCoor, selectedChair.chairCoor.Coor);
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    /*
                    float deltaY = Input.GetAxis("Mouse X");
                    float deltaX = Input.GetAxis("Mouse Y");
                    Vector3 moveDirection = new Vector3(deltaX, 0, -deltaY) * moveSpeed;
                    selectedChair.transform.position += moveDirection;
                    
                    Vector3 mouseScreenPosition = Input.mousePosition;
                    mouseScreenPosition.z = 4 - Camera.main.transform.position.y;
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
                    selectedChair.transform.position = new Vector3(mouseWorldPosition.x, transform.position.y, mouseWorldPosition.z);
                    
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (plane.Raycast(ray, out float distance))
                    {
                        Vector3 hitPoint = ray.GetPoint(distance);
                        Vector3 hitPosition = new Vector3(hitPoint.x, 0, hitPoint.z);
                        selectedChair.transform.position = hitPosition;
                    }
                    */
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    int groundLayer = LayerMask.NameToLayer("GroundPlane");
                    int groundLayerMask = 1 << groundLayer;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
                    {
                        //Debug.Log("Raycast hit:" + hit.collider.name);
                        //Vector2Int oldCoor = selectedChair.chairCoor.Coor;
                        selectedChair.SetPosition(hit.point);
                        //Vector2Int newCoor = selectedChair.chairCoor.Coor;
                        
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (selectedChair != null)
                {
                    Vector2Int oldCoor = selectedChair.chairCoor.Coor;
                    selectedChair.SetPositionAuto();
                    selectedChair.SetCoorForChair();
                    RemoveBoardSeatState(selectedChair.chairCoor, oldCoor);
                    SetBoardSeatState(selectedChair);

                    if (seatDict.ContainsKey((oldCoor.x, oldCoor.y)))
                    {
                        Seat selectSeat = seatDict[(oldCoor.x, oldCoor.y)];
                        seatDict.Remove((oldCoor.x, oldCoor.y));
                        selectedChair.AddToDictionary(seatDict);
                        //seatDict[(selectedChair.chairCoor.Coor.x, selectedChair.chairCoor.Coor.y)] = selectSeat;
                    }
                    selectedChair = null;
                    state = BoardState.CitizenFind;
                }
            }
        }
    }


    public IEnumerator LoadObjects(Action onComplete)
    {
        if (chairList.Count > 0)
        {
            chairList.Clear();
        }
        if (blockList.Count > 0)
        {
            blockList.Clear();
        }
        LevelConstructor levelInfo = levelSetting.Levels[GM.CurrentLevel];
        numberCitizen = 0;
        List<ChairCoor> listChairCoor = levelInfo.ChairList;
        GM.CurrentBoardSize = levelInfo.BoardSize;
        int chairListCount = listChairCoor.Count;
        GM.bridgeIndex = levelInfo.BridgeIndex;
        GameMainController.Time = levelInfo.Time;

        for (int i = -1; i <= GM.CurrentBoardSize.x; i++)
            for (int j = -1; j <= GM.CurrentBoardSize.y; j++)
            {
                GameObject block = levelSetting.InitBlock(new Vector3(i, 0, j));
                if (GM.bridgeIndex > 0)
                {
                    if (j >= GM.bridgeIndex)
                    {
                        block.transform.position += new Vector3(0, 0, 0.5f);
                    }
                }
                if (i < 0 || j < 0 || i == GM.CurrentBoardSize.x || j == GM.CurrentBoardSize.y)
                {
                    block.GetComponent<MeshRenderer>().material = levelSetting.transparentMaterial;
                }
                blockList.Add(block);
                BlockDictionary[(i, j)] = block;
                Debug.Log($"[Block] Set Block {i} ,{j} = {block.transform.position}");

                block.transform.parent = this.transform;
                yield return null;
            }

        for (int i = -1; i <= GM.CurrentBoardSize.x; i++)
            for (int j = -1; j <= GM.CurrentBoardSize.y; j++)
            {
                BoardSeatValue[(i,j)] = SeatState.None;
            }

        for (int i = 0; i < chairListCount; i++)
        {
            ChairCoor chairCoor = listChairCoor[i];
            Chair chair = levelSetting.InitChair(chairCoor);
            if (chairCoor.Coor.y > 2)
            {
                chair.transform.position += new Vector3(0, 0, 0.5f);
            }
            chairList.Add(chair);
            chair.AddToDictionary(seatDict);
            SetBoardSeatState(chair);
        }

        int doorCount = 0;
        for (int i = 0; i < levelInfo.Doors.Count; i++)
        {
            Door doorModel = levelInfo.Doors[i];
            citizenList.Add(new List<Citizen>());
            levelSetting.InitDoor(doorModel.doorCoor);
            int citizenListCount = doorModel.citizenTypeColors.Count;
            numberCitizen += citizenListCount;
            Vector2Int citizenCoor = doorModel.doorCoor;
            for (int j = 0; j < citizenListCount; j++)
            {
                if (doorModel.doorDirection == Direction.Up)
                {
                    citizenCoor += new Vector2Int(-1, 0);
                }
                Citizen citizen = levelSetting.InitCitizen(citizenCoor, doorModel.citizenTypeColors[j]);
                citizenList[doorCount].Add(citizen);
            }
            doorCount++;
        }


        onComplete?.Invoke();
        state = BoardState.CitizenFind;
        Debug.Log($"All objects loaded {BoardSeatValue.Count}");
        yield return null;
    }

    void SetBoardSeatState(Chair chair)
    {
        switch (chair.chairCoor.ChairType)
        {
            case ChairType.ArmChair:
                BoardSeatValue[(chair.chairCoor.Coor.x, chair.chairCoor.Coor.y)] = chair.SeatState;//SeatState.Free;
                //Debug.Log($"[SeatState] SeatState Set {chair.chairCoor.Coor.x},{chair.chairCoor.Coor.y} = {BoardSeatValue[(chair.chairCoor.Coor.x, chair.chairCoor.Coor.y)]}");
                break;
            case ChairType.Couch:
                BoardSeatValue[(chair.chairCoor.Coor.x, chair.chairCoor.Coor.y)] = chair.SeatState; //SeatState.Free;
                BoardSeatValue[(chair.chairCoor.Coor.x, chair.chairCoor.Coor.y - 1)] = chair.SeatState; //SeatState.Free;
                //Debug.Log($"[SeatState] SeatState Set {chair.chairCoor.Coor.x},{chair.chairCoor.Coor.y} = {BoardSeatValue[(chair.chairCoor.Coor.x, chair.chairCoor.Coor.y)]}");
                //Debug.Log($"[SeatState] SeatState Set {chair.chairCoor.Coor.x},{chair.chairCoor.Coor.y - 1} = {BoardSeatValue[(chair.chairCoor.Coor.x, chair.chairCoor.Coor.y - 1)]}");
                break;
        }
    }
    void RemoveBoardSeatState(ChairCoor chairCoor, Vector2Int oldCoor)
    {
        switch (chairCoor.ChairType)
        {
            case ChairType.ArmChair:
                BoardSeatValue[(oldCoor.x, oldCoor.y)] = SeatState.None;
                Debug.Log($"[SeatState] SeatState Remove {oldCoor.x},{oldCoor.y} = {BoardSeatValue[(oldCoor.x, oldCoor.y)]}");
                break;
            case ChairType.Couch:
                BoardSeatValue[(oldCoor.x, oldCoor.y)] = SeatState.None;
                BoardSeatValue[(oldCoor.x, oldCoor.y - 1)] = SeatState.None;
                break;
        }
    }
    IEnumerator CitizenFind()
    {
        for (int i = 0; i < citizenList.Count; i++)
        {
            for (int j = 0; j < citizenList[i].Count; j++)
                if (!citizenList[i][j].IsSeated)
                {
                    Debug.Log($"FIND PATH Begin Door:{i} ,Citizen: {j}");
                    bool findSuccess = findCitizenMovePath(i, j);
                    if (!findSuccess)
                    {
                        state = BoardState.WaitChoose;
                        yield break;
                    }
                    yield return new WaitForSeconds(0.5f);
                }
        }
        if (numberCitizen == 0)
        {
            if (Time.time - GM.lastTimeMove > 0.5f)
            {
                foreach (GameObject confettiEff in ConfettiEffect)
                {
                    confettiEff.GetComponent<ParticleSystem>().Play();
                }
                numberCitizen = -1;
                state = BoardState.Win;
            }
        }
        //state = BoardState.CitizenRun;
    }
    bool findCitizenMovePath(int doorId, int citizenId)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        LevelConstructor levelInfo = levelSetting.Levels[GM.CurrentLevel];
        Vector2Int beginBlock = levelInfo.Doors[doorId].doorCoor;

        int count = 0;
        Queue<Vector2Int> bfsQueue = new Queue<Vector2Int>();
        Dictionary<(int, int), (int, int, int)> markTable = new Dictionary<(int, int), (int, int, int)>();
        markTable[(beginBlock.x, beginBlock.y)] = (beginBlock.x, beginBlock.y, 0);
        Debug.Log($"[FindMove] Begin Node: {markTable[(beginBlock.x, beginBlock.y)]}");
        bfsQueue.Enqueue(beginBlock);

        while (bfsQueue.Count > 0 && count++ < 500)
        {
            Vector2Int currentNode = bfsQueue.Dequeue();
            Debug.Log($"[FindMove] [WhileStep] {count} , QueueCount: {bfsQueue.Count} , GetCurrentNode {currentNode}");
            int pathWeight = markTable[(currentNode.x, currentNode.y)].Item3;
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (i != j && Mathf.Abs(i) + Mathf.Abs(j) == 1)
                    {
                        Vector2Int newNode = currentNode + new Vector2Int(i, j);
                        //If added node can move
                        if (count == 1 || BoardSeatValue[(currentNode.x, currentNode.y)] == SeatState.None)
                        {
                            if (i + currentNode.x >= 0 && j + currentNode.y >= 0
                            && i + currentNode.x < GM.CurrentBoardSize.x && j + currentNode.y < GM.CurrentBoardSize.y
                            && (!markTable.ContainsKey((newNode.x, newNode.y)))
                            && BoardSeatValue[(newNode.x, newNode.y)] != SeatState.Filled)
                                {
                                    Debug.Log($"[FindMove] [Add New Node] {newNode} from {currentNode} QueueRemain: {bfsQueue.Count}");
                                    bfsQueue.Enqueue(newNode);
                                    markTable[(newNode.x, newNode.y)] = (currentNode.x, currentNode.y, pathWeight + 1);
                                    //Debug.Log($"[Track] {newNode} = {currentNode}");
                                }
                            }
                    }
        }

        Debug.Log($"FIND END BoardSize {GM.CurrentBoardSize}");
        Debug.Log($"Begin at DoorIndex:{doorId} , Door: {levelInfo.Doors[doorId].doorCoor} and end {count} node");

        Citizen citizen = citizenList[doorId][citizenId];
        foreach (var seatValue in BoardSeatValue)
        {
            //Debug.Log($"[Found] citizen ({doorId} - {citizenId}) Seat :[{seatValue.Key.Item1},{seatValue.Key.Item2}] = {seatValue.Value} contain {seatDict.ContainsKey((seatValue.Key.Item1, seatValue.Key.Item2))}");
            
            if (seatValue.Value == SeatState.Free && seatDict.ContainsKey((seatValue.Key.Item1, seatValue.Key.Item2)))
            {
                Seat foundSeat = seatDict[(seatValue.Key.Item1, seatValue.Key.Item2)];

                if (foundSeat != null && foundSeat.EqualCitizenColor(citizen.ColorType)
                    && !citizen.IsSeated && markTable.ContainsKey((seatValue.Key.Item1, seatValue.Key.Item2)))
                {
                    Debug.Log($"[Found] seat {seatValue.Key} With SEAT value: {foundSeat} vs citizen({citizenId}) , Color:{citizen.ColorType}");
                    GM.lastTimeMove = Time.time;
                    List<Vector2> trackPath = GetPath(seatValue.Key.Item1, seatValue.Key.Item2, markTable);
                    citizen.MoveTo(seatValue.Key.Item1, seatValue.Key.Item2, trackPath, foundSeat.parentChair);
                    foundSeat.parentChair.LockedByCitizen = true;
                    foundSeat.parentChair.SeatState = SeatState.Filled;
                    BoardSeatValue[seatValue.Key] = SeatState.Filled;
                    citizen.IsSeated = true;
                    numberCitizen--;
                    return true;
                }
            }
        }

        return false;
    }

    private List<Vector2> GetPath(int xsource, int ysource, Dictionary<(int,int), (int,int, int)> tablePrev)
    {
        List<(int,int)> trackPath = new List<(int,int)> ();
        List<Vector2> positionTrackPath = new List<Vector2>();
        int isBreak = 0;
        int xprobe = xsource;
        int yprobe = ysource;
        Debug.Log($"[Track] begin: {xsource} , {ysource}");
        int x = 0;
        foreach (var tab in tablePrev)
        {
            //Debug.Log($"[Track] Log {x++}: {tab}");
        }

        while (isBreak++ < 500)
        {
            Debug.Log($"[Track] {xprobe} , {yprobe} ,Contain?:{BlockDictionary.ContainsKey((xprobe, yprobe))} ,  TrackList Count {tablePrev.Count}");
            trackPath.Add((xprobe, yprobe));

            //Get position of block(x,y) and Add to List Path
            if (!BlockDictionary.ContainsKey((xprobe, yprobe)))
                positionTrackPath.Add(new Vector2(6, -1));
            else
            {
                Vector2 blockPosition = new Vector2(BlockDictionary[(xprobe, yprobe)].transform.position.x, BlockDictionary[(xprobe, yprobe)].transform.position.z);
                positionTrackPath.Add(blockPosition);
            }
            (int, int, int) probe = tablePrev[(xprobe, yprobe)];
            if (probe.Item1 == xprobe && probe.Item2 == yprobe)
            {
                Debug.Log($"[Track] end {xprobe} , {yprobe}");
                break;
            }
            xprobe = probe.Item1;
            yprobe = probe.Item2;
        }
        return positionTrackPath;
    }

    public void SetLevelSetting(LevelSetting levelSetting, GameObject[] ConfettiEffect)
    {
        this.levelSetting = levelSetting;
        this.ConfettiEffect = ConfettiEffect;

    }
}
