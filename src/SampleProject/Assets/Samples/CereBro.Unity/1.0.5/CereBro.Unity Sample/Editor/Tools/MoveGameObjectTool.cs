using System.Collections.Generic;
using System.ComponentModel;
using ModelContextProtocol.Server;
using UnityEngine;

namespace CereBro.Unity.Editor.Tools
{
    [McpServerToolType]
    public static class MoveGameObjectTool
    {
        [McpServerTool("moveGameObject"), Description("Moves a GameObject from its current position in x, y and z units then return its new position.")]
        public static List<float> MoveGameObject(
            [Description(SceneIndexer.IdentifierDescription)] List<int> identifier,
            [Description("The units to move the position of the GameObject along the X Axis")] float x = 0,
            [Description("The units to move the position of the GameObject along the Y Axis")] float y = 0,
            [Description("The units to move the position of the GameObject along the Z Axis")] float z = 0)
        {
            var transform = SceneIndexer.GetTransform(identifier);

            transform.position +=  new Vector3(x, y, z);

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