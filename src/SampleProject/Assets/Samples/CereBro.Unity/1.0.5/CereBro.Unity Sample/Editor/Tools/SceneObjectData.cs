using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CereBro.Unity.Editor.Tools
{
    [Description("Represents the data of a GameObject in the scene hierarchy.")]
    public struct SceneObjectData
    {
        [JsonPropertyName("identifier"), Description(SceneIndexer.IdentifierDescription)]
        public List<int> Identifier { get; private set; }
        
        [JsonPropertyName("name"), Description("The name of the GameObject in the scene hierarchy.")]
        public string Name { get; private set; }

        public SceneObjectData(string name, List<int> identifier)
        {
            Name = name;
            
            Identifier = identifier;
        }
    }
}