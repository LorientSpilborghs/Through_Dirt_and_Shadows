using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootFeature.Runtime
{
    [CreateAssetMenu(fileName = "New Ivy", menuName = "Ivy")]
    public class Ivy : ScriptableObject
    {
        public GameObject _ivyPrefab;
        public float _height;
    }
}
