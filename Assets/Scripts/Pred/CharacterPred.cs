using System.Runtime.CompilerServices;

namespace CharacterPred
{
    readonly struct IsAlive : IPred<Character, Tile>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(Character t, in Tile c) => t.hp > 0;        
    }
}