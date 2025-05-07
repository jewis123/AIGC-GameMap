using MapGenerator.Enums;

namespace MapGenerator
{
    public static class AppSettings
    {
        // 获取应用程序的根目录
        public static string AppRootDirectory => AppDomain.CurrentDomain.BaseDirectory;

        // 获取图片资源目录
        public static string AssetsDirectory => Path.Combine(AppRootDirectory,  "assets");

        // 获取地图输出目录
        public static string OutputDirectory
        {
            get
            {
                string outputDir = Path.Combine(AssetsDirectory, "output");
                // 如果目录不存在，创建它
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }
                return outputDir;
            }
        }

        public static string TempDrawDirectory => Path.Combine(OutputDirectory, "temp", "draw");
        public static string TempMaskDirectory => Path.Combine(OutputDirectory, "temp", "mask");

        public static string ArtChangesDirectory => Path.Combine(AssetsDirectory, "art_changes");

        public static string AssetTempPath => Path.Combine(OutputDirectory, "temp");

        public static string WorkflowPath => Path.Combine(AssetsDirectory, "workflow");
        public static string StyleTemplatePath => Path.Combine(AssetsDirectory, "style_template");

        public static string ProjectRawImgPath => Path.Combine(AssetsDirectory, "project_raw");
        public static string ProjectPreloadImgPath => Path.Combine(ProjectRawImgPath, "preload");

        public static string RefImagePath => Path.Combine(AssetsDirectory, "reference/ref");

        // 获取特定图片的完整路径
        public static string GetImagePath(string fileName)
        {
            return Path.Combine(AssetsDirectory, fileName);
        }

        // 获取带有时间戳的地图输出文件路径
        public static string GetMapDrawOutputPath(string fileName)
        {
            // 生成带时间戳的文件名，避免覆盖
            return Path.Combine(OutputDirectory, "temp", "draw", fileName);
        }

        public static string GetMaskDrawOutputPath(string fileName)
        {
            // 生成带时间戳的文件名，避免覆盖
            return Path.Combine(OutputDirectory, "temp", "mask", fileName);
        }

        public static string GetComfyUIOutputPath(string fileName)
        {
            // 生成带时间戳的文件名，避免覆盖
            return Path.Combine(OutputDirectory,"map", "comfyui", fileName);
        }

        // 笔刷颜色字典
        private static readonly List<MapBrush> BrushColors = new List<MapBrush>
        {
            new(0, Color.FromArgb(255, 31, 0), "小路"),
            new(1, Color.FromArgb(255, 9, 224), "房子"),
            new(2, Color.FromArgb(61, 230, 250), "水"),
            new(3, Color.FromArgb(4, 250, 7), "草地"),
            new(4, Color.FromArgb(160, 150, 20), "沙滩"),
            new(5, Color.FromArgb(255, 102, 0), "山丘"),
            new(6, Color.FromArgb(4, 200, 3), "树"),
        };

        public static int BrushCnt => BrushColors.Count;

        // 根据笔刷类型获取颜色
        public static MapBrush GetBrush(int brushType)
        {
            for (int i = 0; i < BrushColors.Count; i++)
            {
                if (BrushColors[i].Type == brushType)
                {
                    return BrushColors[i];
                }
            }

            throw new IndexOutOfRangeException("Invalid brush type.");
        }


    }
}
