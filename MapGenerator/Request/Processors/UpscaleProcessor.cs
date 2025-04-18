using System.Text.Json;

namespace MapGenerator.Request.ComfyUI{
    public class UpscaleProcessor: BaseProcessor
    {
        private readonly string _workflowPath;

        public UpscaleProcessor(ref ComfyUIClient comfyClient):base(ref comfyClient)
        {
            // 将工作流 JSON 文件路径设置为工作流 JSON 文件位置
            _workflowPath = Path.Combine(AppSettings.WorkflowPath, "2dmap_changeUpscale.json");
        }


        //public async Task<string> Process(string imagePath, int[] pixcels)
        //{
        //}
    }
}