using UnityEngine;
using UnityGameFramework.Runtime;

namespace Flower
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        public static BuiltinInfoComponent BuiltinInfo
        {
            get;
            private set;
        }

        public static ItemComponent Item
        {
            get;
            private set;
        }

        public static DataComponent Data
        {
            get;
            private set;
        }


        private static void InitCustomComponents()
        {
            BuiltinInfo = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinInfoComponent>();
            Item = UnityGameFramework.Runtime.GameEntry.GetComponent<ItemComponent>();
            Data = UnityGameFramework.Runtime.GameEntry.GetComponent<DataComponent>();
        }
    }
}
