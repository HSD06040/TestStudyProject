using System.Runtime.CompilerServices;

namespace TilePred
{
    // Ÿ�Ͽ� �� �� �ִ��� �Ǵ�
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