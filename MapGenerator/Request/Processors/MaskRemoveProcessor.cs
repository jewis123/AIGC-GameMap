using System.Text.Json;

namespace MapGenerator.Request.ComfyUI{
    class MaskRemoveProcessor : BaseProcessor
    {
        public MaskRemoveProcessor(ref ComfyUIClient comfyClient) : base(ref comfyClient)
        {
        }
    }
}