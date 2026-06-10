using game.Shared.DTOs.Patch;

namespace game.Server.Controllers.Handlers;

class ExitHandler : InputHandler
{
    public override List<IGamePatch> Handle(ConsoleKey key)
    {
        var patches = new List<IGamePatch>();
        
        if (key == ConsoleKey.X)
        {
            patches.Add(new ClientExitPatch());
            return patches;
        }

        patches.AddRange(base.Handle(key));
        return patches;
    }
}