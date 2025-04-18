using System.Text.Json;

namespace MapGenerator.Request.ComfyUI{
    public class PickDecoratorProcessor : BaseProcessor
    {
        public PickDecoratorProcessor(ref ComfyUIClient comfyClient) : base(ref comfyClient)
        {
        }
    }
}