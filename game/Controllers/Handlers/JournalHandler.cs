using game.Infrastructure.Services;
using game.Models.Common.Events;
using game.Models.DTOs;
using game.Views.Rendering;

namespace game.Controllers.Handlers;

class JournalHandler : InputHandler
{
    private readonly IMessageService logger;
    private readonly UiEventDispatcher uiDispatcher;

    public JournalHandler(IMessageService logger, UiEventDispatcher uiDispatcher)
    {
        this.logger = logger;
        this.uiDispatcher = uiDispatcher;
    }

    public override List<IUpdateDto> Handle(ConsoleKey key)
    {
        var updates = new List<IUpdateDto>();

        if (key == ConsoleKey.J)
        {
            var history = logger.GetHistory();
            
            uiDispatcher.Publish(new JournalOpenedEvent());
            
            // Wywołujemy renderowanie na cały ekran
            JournalRenderer.Draw(history);
            
            // Zatrzymujemy ekran, dopóki gracz nie naciśnie klawisza
            Console.ReadKey(true);
            Console.Clear();
        
            return updates; // Przerwanie łańcucha - klawisz obsłużony
        }

        return base.Handle(key);
    }
}