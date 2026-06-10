using game.Shared.DTOs.Patch;

namespace game.Server.Controllers.Handlers;

abstract class InputHandler
{
    protected InputHandler? next;

    public InputHandler SetNext(InputHandler nextHandler)
    {
        next = nextHandler;
        return nextHandler;
    }

    public virtual List<IGamePatch> Handle(ConsoleKey key)
    {
        if (next != null)
            return next.Handle(key);

        return new List<IGamePatch>();
    }
}