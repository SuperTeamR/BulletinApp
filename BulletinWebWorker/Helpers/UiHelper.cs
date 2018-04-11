using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Helpers
{
    static class UiHelper
    {
        static Action<string> ActionStateCache { get; set; }
        static Action<string> WorkStateCache { get; set; }
        static Action<string> ObjectStateCache { get; set; }

        public static void SetAction(Action<string> actionState, Action<string> workState, Action<string> objectState)
        {
            ActionStateCache = actionState;
            WorkStateCache = workState;
            ObjectStateCache = objectState;
        }

        public static void UpdateActionState(string text)
        {
            DCT.ExecuteMainThread(d =>
            {
                if (ActionStateCache != null)
                    ActionStateCache(text);
            });
        }
        public static void UpdateWorkState(string text)
        {
            DCT.ExecuteMainThread(d =>
            {
                if (WorkStateCache != null)
                WorkStateCache(text);
            });
        }
        public static void UpdateObjectState(string text)
        {
            DCT.ExecuteMainThread(d =>
            {
                if (ObjectStateCache != null)
                ObjectStateCache(text);
            });
        }
    }
}
