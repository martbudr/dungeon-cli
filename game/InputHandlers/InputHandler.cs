namespace Game.InputHandlers;

abstract class InputHandler
{
    protected InputHandler next;

    public InputHandler SetNext(InputHandler nextHandler)
    {
        next = nextHandler;
        return nextHandler;
    }

    public virtual bool Handle(ConsoleKey key)
    {
        if (next != null)
            return next.Handle(key);

        return false;
    }
}