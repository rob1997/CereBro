using System.Collections.Generic;
using System.ComponentModel;
using ModelContextProtocol.Server;
using UnityEngine;

namespace CereBro.Unity.Editor.Tools
{
    [McpServerToolType]
    public static class GetCurrentPositionTool
    {
        [McpServerTool("getCurrentPosition"), Description("Gets the current position of a GameObject in the active scene.")]
        public static List<float> GetCurrentPosition([Description(SceneIndexer.IdentifierDescription)] List<int> identifier)
        {
            Transform transform = SceneIndexer.GetTransform(identifier);

            Vector3 position = transform.position;
            
            return new List<float>
            {
                position.x,
                position.y,
                position.z
            };
        }
    }
}