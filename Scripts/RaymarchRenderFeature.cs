using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BlobFighters.RayMarching
{
    public class RaymarchRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] private RenderPassEvent @event;

        private RaymarchRenderPass renderPass;

        public override void Create()
        {
            renderPass = new RaymarchRenderPass
            {
                renderPassEvent = @event
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderPass.SetSource(renderer.cameraColorTarget);
            renderer.EnqueuePass(renderPass);
        }
    }
}