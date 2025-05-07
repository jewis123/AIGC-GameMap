namespace MapGenerator.Request.ComfyUI
{

    public class BaseProcessor
    {
        protected readonly ComfyUIClient _comfyClient;

        public BaseProcessor(ref ComfyUIClient comfyClient)
        {
            _comfyClient = comfyClient;
        }

        public async Task CancelCurrentExecution()
        {
            if (_comfyClient != null)
            {
                await _comfyClient.CancelCurrentExecution();
            }

            return;
        }

        public ComfyUIClient Client => _comfyClient;

        public string LastPromptId {get;protected set;}
        public string LastNodeId {get;protected set;}
    }
}