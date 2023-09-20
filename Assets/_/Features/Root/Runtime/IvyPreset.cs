using UnityEngine;

namespace RootFeature.Runtime
{
    [CreateAssetMenu(fileName = "New Ivy", menuName = "Ivy")]
    public class Ivy : ScriptableObject
    {
        public GameObject _ivyPrefab;
        public float _height;
        public Vector2Int _randomInBetweenXY;
    }
}
