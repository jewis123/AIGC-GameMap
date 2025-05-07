using System.Text.Json;

namespace MapGenerator.Request.ComfyUI{
    class DecoratorInpaintProcessor : BaseProcessor
    {
        public DecoratorInpaintProcessor(ref ComfyUIClient comfyClient) : base(ref comfyClient)
        {
        }

        // public async Task<string> Process(string prompt, string rawImagePath, string refImagePath, int batchSize, string style_key, IProgress<int> progress)
        // {
        // }
    }
}