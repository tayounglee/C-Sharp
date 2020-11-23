public abstract class ActionData
{
    public abstract void Undo();
    public abstract void Redo();

    public abstract Pieces Owner { get; }
    public abstract GridIndex From { get; }
    public abstract GridIndex To { get; }
    public abstract Pieces KillActor { get; }
}