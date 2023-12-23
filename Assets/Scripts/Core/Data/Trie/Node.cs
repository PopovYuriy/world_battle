using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Data.Trie
{
    [Serializable]
    public sealed class Node
    {
        [JsonProperty("Children")] private Dictionary<char, Node> _children;
        
        [JsonProperty("IsEnd")] public bool IsEnd { get; set; }

        public Node()
        {
            IsEnd = false;
            _children = new Dictionary<char, Node>();
        }

        public bool HasChild(char key) => _children.ContainsKey(key);

        public void AddChild(char key, Node child)
        {
            if (HasChild(key))
            {
                Debug.LogError($"Node already has a child with key {key}");
                return;
            }
            
            _children.Add(key, child);
        }

        public Node GetChild(char key)
        {
            if (!HasChild(key))
                throw new Exception($"Node has no a child with key {key}");
            
            return _children[key];
        }
    }
}