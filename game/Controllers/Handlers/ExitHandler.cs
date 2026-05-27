using game.Models.DTOs;

namespace game.Controllers.Handlers;

class ExitHandler : InputHandler
{
    public event Action OnExit;

    public override List<IUpdateDto> Handle(ConsoleKey key)
    {
        var updates = new List<IUpdateDto>();
        
        if (key == ConsoleKey.X)
        {
            OnExit?.Invoke();
        }

        return base.Handle(key);
    }
}