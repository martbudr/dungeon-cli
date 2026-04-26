using Game;

namespace game.Domain.Services;

public class SoundPropagationService
{
    public Dictionary<(int x, int y), int> Propagate(Board board, int startX, int startY, int range)
    {
        var result = new Dictionary<(int, int), int>();
        var queue = new Queue<((int x, int y) pos, int dist)>();

        queue.Enqueue(((startX, startY), 0));
        result[(startX, startY)] = 0;

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            var (pos, dist) = queue.Dequeue();

            if (dist >= range)
                continue;

            for (int i = 0; i < 4; i++)
            {
                var next = (pos.x + dx[i], pos.y + dy[i]);

                if (!board.IsWalkable(next.Item1, next.Item2))
                    continue;

                if (result.ContainsKey(next))
                    continue;

                result[next] = dist + 1;
                queue.Enqueue((next, dist + 1));
            }
        }

        return result;
    }
}