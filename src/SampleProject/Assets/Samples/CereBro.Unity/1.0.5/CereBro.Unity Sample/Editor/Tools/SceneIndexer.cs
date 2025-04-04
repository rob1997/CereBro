using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ModelContextProtocol.Server;
using UnityEngine;

namespace CereBro.Unity.Editor.Tools
{
    [McpServerToolType]
    public static class SceneIndexer
    {
        public const string IdentifierDescription = "The identifier of the parent GameObject in the scene hierarchy. Represented by a list of integers. Each integer represents the sibling index of the GameObject in its parent's children.";
        
        [McpServerTool("getSceneData"), Description("Returns a list of all GameObjects in the active scene with their name and identifier.")]
        public static List<SceneObjectData> GetSceneData()
        {
            var indices = new List<SceneObjectData>();
            
            List<int> identifier = new List<int>();
            
            var gameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (var gameObject in gameObjects)
            {
                IndexTransform(gameObject.transform, ref indices, ref identifier);
            }
            
            return indices;
        }

        private static void IndexTransform(Transform transform, ref List<SceneObjectData> indices, ref List<int> identifier)
        {
            identifier.Add(transform.GetSiblingIndex());
            
            indices.Add(new SceneObjectData(transform.name, identifier.ToList()));

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                IndexTransform(child, ref indices, ref identifier);
            }

            identifier.RemoveAt(identifier.Count - 1);
        }

        public static Transform GetTransform(List<int> identifier)
        {
            var gameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            Transform transform = gameObjects[identifier[0]].transform;
            
            for (int i = 1; i < identifier.Count; i++)
            {
                transform = transform.GetChild(identifier[i]);
            }
            
            return transform;
        }

        public static List<int> GetSceneIdentifier(Transform transform)
        {
            List<int> identifier = new List<int>
            {
                transform.GetSiblingIndex()
            };

            // GameObject is a root object
            if (transform.parent == null)
            {
                return identifier;
            }
            
            while (transform.parent != null)
            {
                transform = transform.parent;

                identifier.Insert(0, transform.GetSiblingIndex());
            }

            return identifier;
        }
    }
}