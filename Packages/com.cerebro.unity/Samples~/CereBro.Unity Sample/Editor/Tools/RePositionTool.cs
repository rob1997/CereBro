using System.Collections.Generic;
using System.ComponentModel;
using ModelContextProtocol.Server;
using UnityEngine;

namespace CereBro.Unity.Editor.Tools
{
    [McpServerToolType]
    public static class RePositionTool
    {
        [McpServerTool("rePosition"), Description("Position a GameObject to a new position in the active scene and returns its new position.")]
        public static List<float> RePosition(
            [Description(SceneIndexer.IdentifierDescription)] List<int> identifier, 
            [Description("The new position of the GameObject along the X Axis")] float x, 
            [Description("The new position of the GameObject along the Y Axis")] float y,
            [Description("The new position of the GameObject along the Z Axis")] float z)
        {
            var transform = SceneIndexer.GetTransform(identifier);

            transform.position = new Vector3(x, y, z);
            
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