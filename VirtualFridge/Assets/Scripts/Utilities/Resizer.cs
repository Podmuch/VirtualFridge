using UnityEngine;
using UnityEngine.UI;

namespace Pixel_Empire.Utilities
{
    public class Resizer : MonoBehaviour
    {
        #region SCENE REFERENCES

        public RectTransform ResizerTransform;

        #endregion

        public Vector2 NativeResolution = new Vector2(1280, 720); //It is default value, overridden through Unity editor. If you want change it, do it in editor.
        private float WidthToHeightRatio { get { return NativeResolution.x / NativeResolution.y; } }
        private Vector3 originalScale;
        private float savedScreenWidth = 0;
        private float savedScreenHeight = 0;

        private void Start()
        {
            originalScale = ResizerTransform.localScale;
            Resize();
        }

        private void Update()
        {
            if (Screen.width != Mathf.RoundToInt(savedScreenWidth) ||
                Screen.height != Mathf.RoundToInt(savedScreenHeight))
            {
                Resize();
            }
        }

        private void Resize()
        {
            float screenWidth = (float)Screen.width;
            float screenHeight = (float)Screen.height;
            float currentWidth = screenHeight * WidthToHeightRatio;
            ResizerTransform.localScale = new Vector3((screenWidth / currentWidth) * originalScale.x, originalScale.y, originalScale.z);
            savedScreenWidth = Screen.width;
            savedScreenHeight = Screen.height;
        }
    }
}
