namespace Game;

public class TutorialRenderer
{
    public void Render(UIPanel p, List<string> instructions)
    {
        p.WriteTitle("TUTORIAL");

        foreach (var i in instructions)
            p.WriteLine($"- {i}", ConsoleColor.DarkGray);

        for (int i = 0; i < 5; i++)
            p.WriteLine(" ");
    }
}