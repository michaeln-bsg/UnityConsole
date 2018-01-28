using System.Collections;
using UnityEditor;
using UnityEngine;

namespace BeardPhantom.UConsole.Modules
{
    public class ObjectPickingConsoleModule : AbstractConsoleModule
    {
        [SerializeField]
        private Shader _replacementShader;

        public Vector2 mouse;

        private Camera _camera;

        private RenderTexture _renderTexture;

        private Texture2D _tex;

        public ObjectPickingConsoleModule(Console console)
            : base(console) { }

        public override void Awake()
        {
            _renderTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
            _renderTexture.filterMode = FilterMode.Point;
            _camera = new GameObject().AddComponent<Camera>();
            _camera.transform.position = new Vector3(0f, 0f, -15f);
            _camera.targetTexture = _renderTexture;
            _camera.clearFlags = CameraClearFlags.Color;
            _camera.backgroundColor = Color.black;
            _camera.cullingMask = 0;
            _tex = new Texture2D(1920, 1080, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Point
            };
        }

        public override void Destroy() { }

        public override void Update()
        {
            var renderers = Object.FindObjectsOfType<Renderer>();
            foreach (var r in renderers)
            {
                Mesh mesh = null;
                if (r is MeshRenderer)
                {
                    mesh = r.GetComponent<MeshFilter>().sharedMesh;
                }
                else if (r is SkinnedMeshRenderer)
                {
                    mesh = (r as SkinnedMeshRenderer).sharedMesh;
                }
                else
                {
                    continue;
                }

                var bytes = System.BitConverter.GetBytes(r.GetInstanceID());
                var color = new Color32(bytes[0], bytes[1], bytes[2], 0);
                var material = new Material(_replacementShader);
                material.SetPass(0);
                material.color = color;
                Graphics.DrawMesh(mesh, r.localToWorldMatrix, material, 0, _camera);
            }
            Console.StartCoroutine(WaitThenCheck());

        }

        private IEnumerator WaitThenCheck()
        {
            yield return new WaitForEndOfFrame();
            var renderers = Object.FindObjectsOfType<Renderer>();
            var activeRT = RenderTexture.active;
            RenderTexture.active = _renderTexture;
            _tex.ReadPixels(_camera.pixelRect, 0, 0);

            RenderTexture.active = activeRT;
            mouse = new Vector2((int)Input.mousePosition.x, (int)Input.mousePosition.y);
            Color32 gotColor = _tex.GetPixel((int)mouse.x, (int)mouse.y);
            var colorBytes = new[] { gotColor.r, gotColor.g, gotColor.b, (byte)0 };
            foreach (var r in renderers)
            {
                r.material.color = Color.white;
                var idBytes = System.BitConverter.GetBytes(r.GetInstanceID());
                idBytes[3] = 0;
                var match = true;
                for (var i = 0; i < idBytes.Length; i++)
                {
                    if (idBytes[i] != colorBytes[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    r.material.color = Color.red;
                    Selection.activeGameObject = r.gameObject;
                    Debug.Log("FOUND MATCH");
                }
            }
        }
    }
}