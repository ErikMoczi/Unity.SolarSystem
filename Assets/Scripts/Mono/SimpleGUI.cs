using ECS.Utils;
using UnityEngine;

namespace Mono
{
    public sealed class SimpleGUI : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private GUISkin _guiSkin;
        [SerializeField] private Texture2D _plusIcon;
        [SerializeField] private Texture2D _minusIcon;
        [SerializeField] private Texture2D _playIcon;
        [SerializeField] private Texture2D _stopIcon;
#pragma warning restore 649

        private float _offsetX;
        private float _offsetY;
        private float _width;
        private const float Y1 = 5;
        private const float Y2 = 25;

        private void OnGUI()
        {
            GUI.skin = _guiSkin;
            _offsetX = 10;

            if (GUI.Button(new Rect(_offsetX, Y1, _playIcon.width, Y2), _playIcon))
            {
                GlobalTime.Running = true;
            }

            _offsetX += _playIcon.width;

            if (GUI.Button(new Rect(_offsetX, Y1, _stopIcon.width, Y2), _stopIcon))
            {
                GlobalTime.Running = false;
            }

            _offsetX += _stopIcon.width + 10f;

            if (GUI.Button(new Rect(_offsetX, Y1, _plusIcon.width, Y2), _plusIcon))
            {
                GlobalTime.IncreaseTimeScale();
            }

            _offsetX += _plusIcon.width;

            if (GUI.Button(new Rect(_offsetX, Y1, _minusIcon.width, Y2), _minusIcon))
            {
                GlobalTime.DecreaseTimeScale();
            }

            _offsetX += _minusIcon.width;

            GUI.Label(
                new Rect(_offsetX, Y1, Screen.width, Screen.height),
                GlobalTime.Running ? $"x{GlobalTime.CurrentTimeScale}" : "paused"
            );
        }
    }
}