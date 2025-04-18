using System.Text.Json;

namespace MapGenerator.Request.ComfyUI{
    class DecoratorInpaintProcessor : BaseProcessor
    {
        public DecoratorInpaintProcessor(ref ComfyUIClient comfyClient) : base(ref comfyClient)
        {
        }
    }
}