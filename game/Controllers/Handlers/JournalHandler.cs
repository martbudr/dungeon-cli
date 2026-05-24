using game.Infrastructure.Services;
using game.Models.Common.Events;
using game.Views.Rendering;

namespace game.Controllers.Handlers;

class JournalHandler : InputHandler
{
    private readonly IMessageService logger;
    private readonly JournalRenderer renderer;
    private readonly EventDispatcher dispatcher;

    public JournalHandler(IMessageService logger, JournalRenderer renderer, EventDispatcher dispatcher)
    {
        this.logger = logger;
        this.renderer = renderer;
        this.dispatcher = dispatcher;
    }

    public override bool Handle(ConsoleKey key)
    {
        // Obsługa klawisza J zgodnie z wymaganiami
        if (key == ConsoleKey.J)
        {
            // Pobieramy historię z loggera
            // Rzutujemy na GameLogger, aby dostać się do GetFullHistory
            var history = logger.GetHistory();
            
            dispatcher.Publish(new GameMessageEvent("Reading journal... Press any key to close."));
            
            // Wywołujemy renderowanie na cały ekran
            JournalRenderer.Draw(history);
            
            // Zatrzymujemy ekran, dopóki gracz nie naciśnie klawisza
            Console.ReadKey(true);
            Console.Clear();
        
            return true; // Przerwanie łańcucha - klawisz obsłużony
        }

        return base.Handle(key);
    }
}