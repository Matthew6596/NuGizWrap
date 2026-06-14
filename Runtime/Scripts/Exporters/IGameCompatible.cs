#if UNITY_EDITOR
using System.Linq;
using UnityEngine;

namespace TTModdingKit
{
    public interface IGameCompatible
    {
        public bool IsGameCompatible(TTGame game);
        public int GetMaxVersion(TTGame game);
    }

    public static class GameCompatibilityExt
    {
        public static bool CompareGames(this TTGame game, params TTGame[] otherGames) => otherGames.Contains(game);
        public static bool IsGameCompatible(this IGameCompatible gameCompatible) => gameCompatible.IsGameCompatible(TTUnityProject.Game);
        public static int MaxVersion(this IGameCompatible gameCompatible) => gameCompatible.GetMaxVersion(TTUnityProject.Game);
    }
}
#endif