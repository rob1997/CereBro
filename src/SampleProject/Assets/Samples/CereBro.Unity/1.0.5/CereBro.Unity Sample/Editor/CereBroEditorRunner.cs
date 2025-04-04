using UnityEditor;

namespace CereBro.Unity.Editor
{
    [InitializeOnLoad]
    public static class CereBroEditorRunner
    {
        static CereBroEditorRunner()
        {
            CereBroListener.RunForEditor(5050);
        }
    }
}