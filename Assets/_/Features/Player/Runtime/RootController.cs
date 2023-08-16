using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Runtime
{
    public class RootController : MonoBehaviour
    {
        public static RootController m_instance;
        public bool m_isControllingRoot
        {
            get;
            set;
        }

        private void Awake()
        {
            m_instance = this;
        }

        public void ControllingRoot()
        {
            m_isControllingRoot = true;
        }

    }
}
