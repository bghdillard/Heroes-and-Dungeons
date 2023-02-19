using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node root;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            Debug.Log("Tree Start");
            root = TreeSetup();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            Debug.Log("Tree Update");
            if(root != null && gameObject.activeSelf)
            {
                root.Evaluate();
            }
        }

        protected abstract Node TreeSetup();
    }
}
