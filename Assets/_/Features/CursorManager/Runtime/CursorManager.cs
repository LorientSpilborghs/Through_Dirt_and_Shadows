using UnityEngine;

namespace CursorManagerFeature.Runtime
{
    public class CursorManager : MonoBehaviour
    {
        private void Start()
        {
            if (_cursorTexture == null) return;
            _cursorHotspot = new Vector2(_cursorTexture.width / 2, _cursorTexture.height / 2);
            Cursor.SetCursor(_cursorTexture, _cursorHotspot, CursorMode.Auto);
        }

        [SerializeField] private Texture2D _cursorTexture;

        private Vector2 _cursorHotspot;
    }
}
