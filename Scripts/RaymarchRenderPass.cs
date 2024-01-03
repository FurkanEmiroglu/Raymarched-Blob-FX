using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BlobFighters.RayMarching
{
    public class RaymarchRenderPass : ScriptableRenderPass
    {
        private static RenderTexture _staticTemporaryRenderTexture;
        private RenderTargetIdentifier source; // a render target identifier points to a texture directly.

        public void SetSource(RenderTargetIdentifier s)
        {
            source = s;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!Application.isPlaying) return;
            if (_staticTemporaryRenderTexture == null)
                _staticTemporaryRenderTexture = new RenderTexture(renderingData.cameraData.cameraTargetDescriptor);

            CommandBuffer cmd = CommandBufferPool.Get("Ray marching as render feature");
            RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
            cameraTextureDesc.depthBufferBits = 0;

            Blit(cmd, source, _staticTemporaryRenderTexture);
            RenderTexture rmResult = RMMaster.Instance.ApplyRaymarching(_staticTemporaryRenderTexture);
            Blit(cmd, rmResult, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}