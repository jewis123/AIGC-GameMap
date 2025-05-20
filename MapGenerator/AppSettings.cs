using MapGenerator.Enums;
using System.Text.Json;

namespace MapGenerator
{
    public static class AppSettings
    {
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        private static readonly Lazy<JsonElement> _config = new Lazy<JsonElement>(LoadConfig);

        private static JsonElement LoadConfig()
        {
            if (!File.Exists(ConfigFilePath))
            {
                // 默认配置内容，带中文注释
                var defaultConfig = new
                {
                    AssetsDirectory = "assets",
                    AssetsDirectory_comment = "图片资源目录",
                    OutputDirectory = "assets\\output",
                    OutputDirectory_comment = "AI输出目录",
                    TempDrawDirectory = "assets\\output\\temp\\draw",
                    TempDrawDirectory_comment = "临时手绘图目录",
                    TempMaskDirectory = "assets\\output\\temp\\mask",
                    TempMaskDirectory_comment = "临时手绘遮罩图目录",
                    MapOutputDir = "assets\\output\\map",
                    MapOutputDir_comment = "AI地图输出目录",
                    MapTmpOutputDir = "assets\\output\\temp\\map",
                    MapTmpOutputDir_comment = "AI地图临时输出目录",
                    ArtChangesDirectory = "assets\\art_changes",
                    ArtChangesDirectory_comment = "换皮图片保存目录",
                    WorkflowPath = "assets\\workflow",
                    WorkflowPath_comment = "comfyUI 工作流目录",
                    StyleTemplateDir = "assets\\style_template",
                    StyleTemplateDir_comment = "lora模板图目录",
                    ProjectRawImgDir = "assets\\project_raw",
                    ProjectRawImgDir_comment = "项目原始图片目录",
                    ProjectPreloadImgDir = "assets\\preload",
                    ProjectPreloadImgDir_comment = "项目小图目录",
                    RefImageDir = "assets\\reference\\ref",
                    RefImageDir_comment = "参考特征图片目录",
                    comfyuiServerUrl = "http://localhost:8080",
                    comfyuiServerUrl_comment = "ComfyUI 服务地址",
                    StyleMapping = new Dictionary<string, string>
                    {
                        { "吉卜力", "flux\\ghibli_fantasy_v1.safetensors" },
                        { "新海诚", "flux\\新海诚风格动漫超精细光影壁纸风_v1.0.safetensors" }
                    },
                    StyleMapping_comment = "风格名与lora文件名的映射"
                };
                var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFilePath, json);
            }
            var configJson = File.ReadAllText(ConfigFilePath);
            return JsonDocument.Parse(configJson).RootElement;
        }

        // 获取应用程序的根目录
        public static string AppRootDirectory => AppDomain.CurrentDomain.BaseDirectory;

        // 获取图片资源目录
        private static string? _assetsDirectory;
        public static string AssetsDirectory => _assetsDirectory ??_config.Value.GetProperty("AssetsDirectory").GetString() ?? Path.Combine(AppRootDirectory, "assets");

        // 获取地图输出目录
        private static string? _outputDirectory;
        
        public static string OutputDirectory
        {
            get
            {
                if (_outputDirectory == null)
                {
                    string outputDir = _config.Value.GetProperty("OutputDirectory").GetString() ?? Path.Combine(AssetsDirectory, "output");
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }
                    _outputDirectory = outputDir;
                }
                return _outputDirectory;
            }
        }

        private static string? _editorRes;
        public static string EditorRes => _editorRes ??= Path.Combine(AppRootDirectory, "resources");

        private static string? _tempDrawDirectory;
        public static string TempDrawDirectory => _tempDrawDirectory ??= _config.Value.GetProperty("TempDrawDirectory").GetString() ?? Path.Combine(OutputDirectory, "temp", "draw");

        private static string? _tempMaskDirectory;
        public static string TempMaskDirectory => _tempMaskDirectory ??= _config.Value.GetProperty("TempMaskDirectory").GetString() ?? Path.Combine(OutputDirectory, "temp", "mask");

        private static string? _artChangesDirectory;
        public static string ArtChangesDirectory => _artChangesDirectory ??= _config.Value.GetProperty("ArtChangesDirectory").GetString() ?? Path.Combine(AssetsDirectory, "art_changes");

        private static string? _assetTempDir;
        public static string AssetTempDir => _assetTempDir ??= Path.Combine(OutputDirectory, "temp");

        private static string? _workflowPath;
        public static string WorkflowPath => _workflowPath ??= _config.Value.GetProperty("WorkflowPath").GetString() ?? Path.Combine(AssetsDirectory, "workflow");

        private static string? _styleTemplateDir;
        public static string StyleTemplateDir => _styleTemplateDir ??= _config.Value.GetProperty("StyleTemplateDir").GetString() ?? Path.Combine(AssetsDirectory, "style_template");

        private static string? _projectRawImgDir;
        public static string ProjectRawImgDir => _projectRawImgDir ??= _config.Value.GetProperty("ProjectRawImgDir").GetString() ?? Path.Combine(AssetsDirectory, "project_raw");

        private static string? _projectPreloadImgDir;
        public static string ProjectPreloadImgDir => _projectPreloadImgDir ??= _config.Value.GetProperty("ProjectPreloadImgDir").GetString() ?? Path.Combine(AssetsDirectory, "preload");

        private static string? _refImageDir;
        public static string RefImageDir => _refImageDir ??= _config.Value.GetProperty("RefImageDir").GetString() ?? Path.Combine(AssetsDirectory, "reference/ref");

        private static string? _mapOutputDir;
        public static string MapOutputDir => _mapOutputDir ??= _config.Value.GetProperty("MapOutputDir").GetString() ?? Path.Combine(OutputDirectory, "map");

        private static string? _mapTmpOutputDir;
        public static string MapTmpOutputDir => _mapTmpOutputDir ??= _config.Value.GetProperty("MapTmpOutputDir").GetString() ?? Path.Combine(OutputDirectory, "temp", "map");

        private static string? _comfyuiServerUrl;
        public static string comfyuiServerUrl => _comfyuiServerUrl ??= _config.Value.GetProperty("comfyuiServerUrl").GetString() ?? "http://localhost:8080";

        private static readonly Lazy<Dictionary<string, string>> _styleMapping = new Lazy<Dictionary<string, string>>(LoadStyleMapping);

        private static Dictionary<string, string> LoadStyleMapping()
        {
            if (_config.Value.TryGetProperty("StyleMapping", out var mappingElement) && mappingElement.ValueKind == JsonValueKind.Object)
            {
                var dict = new Dictionary<string, string>();
                foreach (var prop in mappingElement.EnumerateObject())
                {
                    dict[prop.Name] = prop.Value.GetString() ?? string.Empty;
                }
                return dict;
            }
            // 默认值
            return new Dictionary<string, string>
                {
                    { "吉卜力", "flux\\ghibli_fantasy_v1.safetensors" },
                    { "新海诚", "flux\\新海诚风格动漫超精细光影壁纸风_v1.0.safetensors" }
                };
        }

        public static string GetLoraPath(string styleName)
        {
            if (_styleMapping.Value.TryGetValue(styleName, out var loraPath))
            {
                return Path.Combine(WorkflowPath, loraPath);
            }
            else
            {
                throw new KeyNotFoundException($"风格 '{styleName}' 的 Lora 文件路径未找到。");
            }
        }

        // 获取特定图片的完整路径
        public static string GetImagePath(string fileName)
        {
            return Path.Combine(AssetsDirectory, fileName);
        }

        public static string GetMapOutputPath(string fileName)
        {
            // 生成带时间戳的文件名，避免覆盖
            return Path.Combine(OutputDirectory, "map", fileName);
        }

        // 获取带有时间戳的地图输出文件路径
        public static string GetTmpDrawPath(string fileName)
        {
            // 生成带时间戳的文件名，避免覆盖
            return Path.Combine(OutputDirectory, "temp", "draw", fileName);
        }

        public static string GetTmpMaskPath(string fileName)
        {
            // 生成带时间戳的文件名，避免覆盖
            return Path.Combine(OutputDirectory, "temp", "mask", fileName);
        }

        public static string GetTmpMapOutputPath(string fileName)
        {
            // 生成带时间戳的文件名，避免覆盖
            return Path.Combine(OutputDirectory, "temp", "map", fileName);
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
