using UnityEngine;

namespace FogOfWarFeature.Runtime
{
    public class FogOfWar : MonoBehaviour
    {
        private void Start()
        {
            for (int y = 0; y < _width; y++) 
            {
                for (int x = 0; x < _depth; x++)
                {
                    Vector3 pos = new Vector3(x, 0, y) * _radius;
                    Instantiate(_gameObject, pos, Quaternion.identity);
                }
            } 
        }

        [SerializeField] private float _width;
        [SerializeField] private float _depth;
        [SerializeField] private float _radius;
        [SerializeField] private GameObject _gameObject;
    }
}
