using System;
using System.Runtime.CompilerServices;

#region Interfaces
// 모든 조건 구조체가 구현할 제네릭 인터페이스
interface IPred<TTarget, TCtx>
{
    bool Test(TTarget t, in TCtx c);
}
#endregion

#region Static_Chain
// ---------- Fluent builder (TTarget, TCtx) ----------
static class PredChain<TTarget, TCtx>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Chain<TTarget, TCtx, TLeaf> Start<TLeaf>(TLeaf leaf)
        where TLeaf : struct, IPred<TTarget, TCtx> => new(leaf);

    // 항상 true로 시작하는 체인 (항등원)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Chain<TTarget, TCtx, TruePred<TTarget, TCtx>> All() => new(TruePred<TTarget, TCtx>.Instance);

    // 항상 false로 시작하는 체인
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Chain<TTarget, TCtx, FalsePred<TTarget, TCtx>> Any() => new(new FalsePred<TTarget, TCtx>());

    // 편의용 오버로드
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Chain<TTarget, TCtx, T1> All<T1>(T1 a) where T1 : struct, IPred<TTarget, TCtx> => new(a);
}
#endregion

#region Chain
// ---------- Chain (TTarget, TCtx) ----------
readonly struct Chain<TTarget, TCtx, TPred> where TPred : struct, IPred<TTarget, TCtx>
{
    public readonly TPred p;

    public Chain(TPred p)
    {
        this.p = p;
    }

    // &&
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Chain<TTarget, TCtx, And<TTarget, TCtx, TPred, TNext>> And<TNext>(TNext n)
        where TNext : struct, IPred<TTarget, TCtx>
        => new(new And<TTarget, TCtx, TPred, TNext>(p, n));

    // ||
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Chain<TTarget, TCtx, Or<TTarget, TCtx, TPred, TNext>> Or<TNext>(TNext n)
        where TNext : struct, IPred<TTarget, TCtx>
        => new(new Or<TTarget, TCtx, TPred, TNext>(p, n));

    // !
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Chain<TTarget, TCtx, Not<TTarget, TCtx, TPred>> Not() => new(new Not<TTarget, TCtx, TPred>(p));

    // (a && b) - AndGroup의 Func과 결과 타입에 TTarget, TCtx를 올바르게 전달
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Chain<TTarget, TCtx, And<TTarget, TCtx, TPred, TSub>> AndGroup<TSub>(
        Func<Chain<TTarget, TCtx, TruePred<TTarget, TCtx>>, Chain<TTarget, TCtx, TSub>> g)
        where TSub : struct, IPred<TTarget, TCtx>
        => new(new And<TTarget, TCtx, TPred, TSub>(p, g(PredChain<TTarget, TCtx>.All()).Build()));

    // (a || b) - OrGroup의 Func과 결과 타입에 TTarget, TCtx를 올바르게 전달
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Chain<TTarget, TCtx, Or<TTarget, TCtx, TPred, TSub>> OrGroup<TSub>(
        Func<Chain<TTarget, TCtx, TruePred<TTarget, TCtx>>, Chain<TTarget, TCtx, TSub>> g)
        where TSub : struct, IPred<TTarget, TCtx>
        => new(new Or<TTarget, TCtx, TPred, TSub>(p, g(PredChain<TTarget, TCtx>.All()).Build()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TPred Build() => p;
}
#endregion

#region Identities
// ---------- Identities ----------
readonly struct TruePred<TTarget, TCtx> : IPred<TTarget, TCtx>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Test(TTarget t, in TCtx c) => true;

    public static readonly TruePred<TTarget, TCtx> Instance = default;
}

readonly struct FalsePred<TTarget, TCtx> : IPred<TTarget, TCtx>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Test(TTarget t, in TCtx c) => false;
}
#endregion

#region Combinators
// ---------- Combinators ----------
readonly struct Not<TTarget, TCtx, A> : IPred<TTarget, TCtx>
    where A : struct, IPred<TTarget, TCtx>
{
    public readonly A a;

    public Not(A a)
    {
        this.a = a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Test(TTarget t, in TCtx c) => !a.Test(t, c);
}

readonly struct Or<TTarget, TCtx, A, B> : IPred<TTarget, TCtx>
    where A : struct, IPred<TTarget, TCtx>
    where B : struct, IPred<TTarget, TCtx>
{
    public readonly A a;
    public readonly B b;

    public Or(A a, B b)
    {
        this.a = a;
        this.b = b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Test(TTarget t, in TCtx c) => a.Test(t, c) || b.Test(t, c);
}

readonly struct And<TTarget, TCtx, A, B> : IPred<TTarget, TCtx>
    where A : struct, IPred<TTarget, TCtx>
    where B : struct, IPred<TTarget, TCtx>
{
    public readonly A a;
    public readonly B b;

    public And(A a, B b)
    {
        this.a = a;
        this.b = b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Test(TTarget t, in TCtx c) => a.Test(t, c) && b.Test(t, c);
}
#endregion