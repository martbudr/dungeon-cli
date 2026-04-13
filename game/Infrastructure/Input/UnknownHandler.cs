using Game.Services;

namespace Game.InputHandlers;


class UnknownHandler : InputHandler
{
    private readonly IMessageService _logger;

    public UnknownHandler(IMessageService logger)
    {
        _logger = logger;
    }

    public override bool Handle(ConsoleKey key)
    {
        // Jeśli doszliśmy tutaj, oznacza to, że żaden wcześniejszy handler 
        // nie rozpoznał tego klawisza
        _logger.ShowMessage($"The key {key} does nothing here.");
        
        return true; 
    }
}