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
    }
}