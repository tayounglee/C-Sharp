using UnityEngine;

public class ChessBoard : ActorComponent
{
    Camera mainCamera;

    Pieces[,] boardPieces = new Pieces[8, 8];
    BoxCollider[,] boardTrigger = new BoxCollider[8, 8];

    SpawnManager spawnManager;
    ActionManager actionManager;
    HighlightManager highlightManager;

    bool bLeftCtrl = false;

    private void Start()
    {
        mainCamera = gameObject.AddSceneComponent<Camera>();
        mainCamera.transform.localPosition = new Vector3(3.5f, 9.0f, -1.0f);
        mainCamera.transform.localRotation = Quaternion.AngleAxis(67.0f, Vector3.right);

        spawnManager = new SpawnManager(this);
        actionManager = new ActionManager(this);
        highlightManager = new HighlightManager(this);

        spawnManager.InitialSpawn();
        InitialTriggerBoxes();

        actionManager.GameoverEvent += ActionManager_GameoverEvent;
    }

    private void Update()
    {
        UpdateInput();
        OnUpdateHover();

        if (Input.GetMouseButtonDown(0)) {
            OnClicked();
        }
    }

    public Vector3 QueryLocation(GridIndex gridIndex)
    {
        const float GridUnit = 1.0f;

        return new Vector3(
            gridIndex.X * GridUnit,
            0,
            gridIndex.Y * GridUnit
            );
    }

    public Vector3 QueryLocation(int x, int y) =>
        QueryLocation(new GridIndex(x, y));

    public void Turnover()
    {
        foreach (var actor in boardPieces) {
            actor?.Turnover();
        }
    }

    void ClearState()
    {
        highlightManager.ClearState();
        actionManager.ClearState();
    }

    void UpdateInput()
    {
        bool bHasInput = false;

        bLeftCtrl = Input.GetKey(KeyCode.LeftControl);

        if (/* bLeftCtrl &&*/  Input.GetKeyDown(KeyCode.Z)) {
            actionManager.Undo();
            bHasInput = true;
        }

        if (/* bLeftCtrl &&*/  Input.GetKeyDown(KeyCode.Y)) {
            actionManager.Redo();
            bHasInput = true;
        }

        if (bHasInput) {
            ClearState();
        }
    }

    void InitialTriggerBoxes()
    {
        for (int x = 0; x < 8; ++x) {
            for (int y = 0; y < 8; ++y) {
                var current = boardTrigger[x, y] = gameObject.AddSceneComponent<BoxCollider>($"TriggerBox[{x}, {y}]");
                current.transform.localPosition = QueryLocation(x, y);
                current.size = new Vector3(1.0f, 0.05f, 1.0f);
                current.gameObject.layer = 8;
                current.isTrigger = true;
            }
        }
    }

    void OnUpdateHover()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, 1000.0f, LayerMask.GetMask("TriggerBox"))) {
            var grid = FindTriggerCollider(hit.collider);
            highlightManager.HoverIndex = grid;
        }
        else {
            highlightManager.HoverIndex = null;
        }
    }

    void OnClicked()
    {
        actionManager.AddClick(highlightManager.HoverIndex);
        
        if (actionManager.HasSelection) {
            highlightManager.BuildMoveHighlight(actionManager.SelectionPiece);
            highlightManager.BuildAttackHighlight(actionManager.SelectionPiece);
            highlightManager.BuildSpecialHighligh(actionManager.SelectionPiece);
        }
        else {
            highlightManager.ClearMoveHighlight();
            highlightManager.ClearAttackHighlight();
            highlightManager.ClearSpecialHighlight();
        }
    }

    void ActionManager_GameoverEvent(object sender, GameoverType e)
    {
        Debug.Log($"{e}!");
    }

    public Pieces this[GridIndex index]
    {
        get => boardPieces[index.X, index.Y];
        set => boardPieces[index.X, index.Y] = value;
    }

    public Pieces this[GridIndex? index] => index.HasValue ? this[index.Value] : null;

    public Pieces this[int x, int y]
    {
        get => boardPieces[x, y];
        set => boardPieces[x, y] = value;
    }

    public Pieces[,] GetPieces() => boardPieces;

    public BuildedData BuildedData => actionManager.BuildedData;
    public SpawnManager SpawnMgr => spawnManager;

    GridIndex? FindTriggerCollider(Collider collider)
    {
        for (int x = 0; x < 8; ++x) {
            for (int y = 0; y < 8; ++y) {
                if (collider == boardTrigger[x, y]) {
                    return new GridIndex(x, y);
                }
            }
        }

        return null;
    }

    public static GridIndex ToBlack => new GridIndex(0, 1);
    public static GridIndex ToWhite => new GridIndex(0, -1);
}
