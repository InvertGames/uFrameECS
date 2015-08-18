using uFrame.Attributes;
using UnityEngine;

namespace uFrame.Actions
{
    [ActionLibrary, uFrameCategory("Debug")]
    public class Log : UFAction
    {
        [In] public string Message;
        public override bool Execute()
        {
            Debug.Log(Message);
            return true;
        }
    }
}