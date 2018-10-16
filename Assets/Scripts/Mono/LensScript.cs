using UnityEngine;

namespace Mono
{
    public sealed class LensScript : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private float _strength;
#pragma warning restore 649

        private LensFlare _lensFlare;

        private void Start()
        {
            _lensFlare = GetComponent<LensFlare>();
            var heading = gameObject.transform.position - Camera.main.transform.position;
            var dist = heading.magnitude;
            _lensFlare.brightness = _strength / dist;
        }

        private void Update()
        {
            var heading = gameObject.transform.position - Camera.main.transform.position;
            var dist = heading.magnitude;
            _lensFlare.brightness = Mathf.Clamp(_strength / dist, 1f, Mathf.Infinity);
        }
    }
}