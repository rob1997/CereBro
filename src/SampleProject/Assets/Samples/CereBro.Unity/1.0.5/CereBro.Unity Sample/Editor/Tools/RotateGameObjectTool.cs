using System.Collections.Generic;
using System.ComponentModel;
using ModelContextProtocol.Server;
using UnityEngine;

namespace CereBro.Unity.Editor.Tools
{
    [McpServerToolType]
    public static class RotateGameObjectTool
    {
        [McpServerTool("rotateGameObject"), Description("Rotates a GameObject in the scene to a new rotation along the x, y and z axis then return the rotation angles along each axis.")]
        public static List<float> RotateGameObject(
            [Description(SceneIndexer.IdentifierDescription)] List<int> identifier,
            [Description("The rotation of the GameObject in degrees along the X Axis")] float x,
            [Description("The rotation of the GameObject in degrees along the Y Axis")] float y,
            [Description("The rotation of the GameObject in degrees along the Z Axis")] float z)
        {
            var transform = SceneIndexer.GetTransform(identifier);

            Vector3 eulers = new Vector3(x, y, z);
            
            transform.rotation *= Quaternion.Euler(eulers);

            eulers = transform.rotation.eulerAngles;
            
            return new List<float>
            {
                eulers.x,
                eulers.y,
                eulers.z
            };
        }
    }
}