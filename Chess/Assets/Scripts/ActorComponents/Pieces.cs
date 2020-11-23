using UnityEngine;

public abstract class Pieces : ActorComponent
{
    protected class MyActionData : ActionData
    {
        Pieces actor;
        GridIndex from;
        GridIndex to;
        Pieces killActor;

        public MyActionData(Pieces actor, GridIndex from, GridIndex to, Pieces killActor)
        {
            this.actor = actor;
            this.from = from;
            this.to = to;
            this.killActor = killActor;
        }

        public MyActionData(ActionData data)
        {
            if (data is MyActionData my) {
                this.actor = my.actor;
                this.from = my.from;
                this.to = my.to;
                this.killActor = my.killActor;
            }
        }

        public override void Undo()
        {
            actor.MoveTo(from);
            actor.board[to] = killActor;
            killActor?.gameObject.SetActive(true);
        }

        public override void Redo()
        {
            actor.MoveTo(to);
        }

        public override Pieces Owner => actor;
        public override GridIndex From => from;
        public override GridIndex To => to;
        public override Pieces KillActor => killActor;
    }

    Mesh mesh;
    ChessTeam team;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    GridIndex myIndex;
    ChessBoard board;

    protected void Construct(Mesh mesh, ChessTeam team, GridIndex myIndex, ChessBoard board)
    {
        base.Construct();

        this.mesh = mesh;
        this.team = team;
        this.board = board;

        this.myIndex = myIndex;
        var loc = board.QueryLocation(myIndex);
        gameObject.transform.localPosition = loc;
    }

    public virtual void Start()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh = mesh;
        Team = team;
    }

    public virtual ActionData MoveTo(GridIndex gridIndex)
    {
        
        var from = myIndex;

        
        myIndex = gridIndex;

        
        var loc = board.QueryLocation(gridIndex);
        gameObject.transform.localPosition = loc;

        
        var killActor = board[gridIndex];
        board[from] = null;
        board[gridIndex] = this;

        killActor?.gameObject.SetActive(false);

        return new MyActionData(this, from, gridIndex, killActor);
    }

    public virtual void Turnover()
    {

    }

    public abstract MoveSequence QueryMovable(MoveType type);

    public virtual ChessTeam Team
    {
        get => team;
        protected set => team = value;
    }

    public GridIndex CellIndex
    {
        get => myIndex;
    }

    public Mesh Mesh
    {
        get => meshFilter.mesh;
        private set => meshFilter.mesh = value;
    }

    public Material Material
    {
        get => meshRenderer.material;
        protected set => meshRenderer.material = value;
    }
}
