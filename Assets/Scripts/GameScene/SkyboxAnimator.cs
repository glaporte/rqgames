using UnityEngine;

namespace rqgames.Game
{
    [RequireComponent(typeof(Skybox))]
    public class SkyboxAnimator : MonoBehaviour
    {
        private Skybox _sky;
        private Material _mat;
        private void Start()
        {
            _sky = GetComponent<Skybox>();
            _mat = _sky.material;
        }

        private void Update()
        {
            _mat.SetFloat("_Rotation", Time.timeSinceLevelLoad * 10);
        }
    }
}
