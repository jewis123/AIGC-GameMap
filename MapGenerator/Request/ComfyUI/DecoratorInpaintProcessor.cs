using System.Text.Json;

namespace MapGenerator.Request.ComfyUI{
    class DecoratorInpaintProcessor
    {
        private ComfyUIClient comfyUIClient;

        public DecoratorInpaintProcessor(ref ComfyUIClient comfyUIClient)
        {
            this.comfyUIClient = comfyUIClient;
        }
    }
}