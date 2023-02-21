using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        protected NodeState state;
        protected List<Node> children = new List<Node>();
        public Node parent;
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                AddChild(child);
            }
        }

        public void AddCombatTree(List<Node> combatTree) //special function that specific monsters and heroes will use to add a subtree to the beginning of their behavior trees
        {
            foreach (Node child in children)
            {
                combatTree.Add(child);
            }
            children = combatTree;
        }

        private void AddChild(Node toAdd)
        {
            toAdd.parent = this;
            children.Add(toAdd);
        }

        public void SetData(string key, object value)
        {
            Debug.Log("We are creating data with key " + key);
            data.Add(key, value);
        }

        public object GetData(string key)
        {
            Debug.Log("Node Checking for Data " + key);
            if (data.ContainsKey(key))
            {
                Debug.Log("Data Found");
                return data[key];
            }
            if(parent != null)
            {
                Debug.Log("Data Not Found, but have a parent");
                return parent.GetData(key);
            }
            Debug.Log("I'm the root with out the data");
            return null;
        }

        public bool RemoveData(string key)
        {
            Debug.Log("We are Removing Data of key " + key); 
            if(data[key] != null)
            {
                data.Remove(key);
                return true;
            }
            if(parent != null)
            {
                return parent.RemoveData(key);
            }
            return false;
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;
    }
}
