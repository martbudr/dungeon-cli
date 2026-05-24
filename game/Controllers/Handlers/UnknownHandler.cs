using game.Models.Common.Events;

namespace game.Controllers.Handlers;


class UnknownHandler : InputHandler
{
    private readonly EventDispatcher dispatcher;

    public UnknownHandler(EventDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    public override bool Handle(ConsoleKey key)
    {
        // Jeśli doszliśmy tutaj, oznacza to, że żaden wcześniejszy handler 
        // nie rozpoznał tego klawisza
        dispatcher.Publish(new GameMessageEvent($"The key {key} does nothing here."));
        
        return true; 
    }
}