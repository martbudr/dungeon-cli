namespace game.Controllers.Handlers;

class ExitHandler : InputHandler
{
    public event Action OnExit;

    public override bool Handle(ConsoleKey key)
    {
        if (key == ConsoleKey.X)
        {
            OnExit?.Invoke();
            return true;
        }

        return next?.Handle(key) ?? false;
    }
}