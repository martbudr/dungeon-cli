using game.Models.Common.Events;
using game.Models.DTOs;

namespace game.Controllers.Handlers;


class UnknownHandler : InputHandler
{
    private readonly UiEventDispatcher uiDispatcher;

    public UnknownHandler(UiEventDispatcher uiDispatcher)
    {
        this.uiDispatcher = uiDispatcher;
    }

    public override List<IUpdateDto> Handle(ConsoleKey key)
    {
        // Jeśli doszliśmy tutaj, oznacza to, że żaden wcześniejszy handler 
        // nie rozpoznał tego klawisza
        uiDispatcher.Publish(new UnknownKeyEvent(key));
        
        return base.Handle(key);
    }
}