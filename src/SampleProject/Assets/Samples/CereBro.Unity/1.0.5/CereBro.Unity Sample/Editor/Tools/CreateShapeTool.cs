using System.Collections.Generic;
using System.ComponentModel;
using ModelContextProtocol.Server;
using UnityEngine;

namespace CereBro.Unity.Editor.Tools
{
    [McpServerToolType]
    public static class CreateShapeTool
    {
        [McpServerTool("createShape"),
         Description("Creates a primitive shape such as a cube, sphere, etc. in the scene then returns its identifier.")]
        public static List<int> CreateShape(
            [Description("Type of the primitive shape to instantiate")]
            PrimitiveType type,
            [Description(SceneIndexer.IdentifierDescription)]
            List<int> parentIdentifier = null)
        {
            Transform transform = GameObject.CreatePrimitive(type).transform;

            if (parentIdentifier != null)
            {
                Transform parent = SceneIndexer.GetTransform(parentIdentifier);
                
                transform.SetParent(parent);
                
                transform.localPosition = Vector3.zero;
            }
            
            return SceneIndexer.GetSceneIdentifier(transform);
        }
    }
}