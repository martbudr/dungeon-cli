using Game.Services;

namespace Game.InputHandlers;

class JournalHandler : InputHandler
{
    private readonly IMessageService _logger;
    private readonly JournalRenderer _renderer;

    public JournalHandler(IMessageService logger, JournalRenderer renderer)
    {
        _logger = logger;
        _renderer = renderer;
    }

    public override bool Handle(ConsoleKey key)
    {
        // Obsługa klawisza J zgodnie z wymaganiami
        if (key == ConsoleKey.J)
        {
            // Pobieramy historię z loggera
            // Rzutujemy na GameLogger, aby dostać się do GetFullHistory
            var history = _logger.GetHistory();
            
            // Wywołujemy renderowanie na cały ekran
            _renderer.Draw(history);
            
            // Zatrzymujemy ekran, dopóki gracz nie naciśnie klawisza
            Console.ReadKey(true);
            Console.Clear();
        
            return true; // Przerwanie łańcucha - klawisz obsłużony
        }

        return base.Handle(key);
    }
}