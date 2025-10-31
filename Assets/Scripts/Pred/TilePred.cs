using System.Runtime.CompilerServices;

namespace TilePred
{
    // 타일에 갈 수 있는지 판단
    readonly struct IsMovable : IPred<Character, Tile>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(Character t, in Tile c)
        {
            int dx = t.tilePos.x - c.tilePos.x;
            int dy = t.tilePos.y - c.tilePos.y;
            int distance = dx + dy;            
            return distance <= t.maxMove;
        }
    }
}