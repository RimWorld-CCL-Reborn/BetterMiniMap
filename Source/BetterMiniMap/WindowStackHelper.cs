using System.Reflection;
using Verse;
using Harmony;

namespace BetterMiniMap
{
    public static class WindowStackHelper
    {
        private static FieldInfo FI_uniqueWindowID = AccessTools.Field(typeof(WindowStack), "uniqueWindowID");
        private static MethodInfo MI_InsertAtCorrectPositionInList = AccessTools.Method(typeof(WindowStack), "InsertAtCorrectPositionInList");
        private static MethodInfo MI_FocusAfterInsertIfShould = AccessTools.Method(typeof(WindowStack), "FocusAfterInsertIfShould");
        private static FieldInfo FI_updateInternalWindowsOrderLater = AccessTools.Field(typeof(WindowStack), "updateInternalWindowsOrderLater");

        public static void CustomAdd(this WindowStack windowStack, Window window)
        {
            int uniqueWindowID = (int)FI_uniqueWindowID.GetValue(windowStack);
            window.ID = uniqueWindowID;
            FI_uniqueWindowID.SetValue(windowStack, uniqueWindowID + 1);
            window.PreOpen();
            MI_InsertAtCorrectPositionInList.Invoke(windowStack, new object[] { window });
            MI_FocusAfterInsertIfShould.Invoke(windowStack, new object[] { window });
            FI_updateInternalWindowsOrderLater.SetValue(windowStack, true);
            window.PostOpen();
        }
    }
}
