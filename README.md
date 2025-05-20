### 介绍
通过COMFYUI实现对游戏地图资产的生成、转绘。
### 目录结构
1. MapGenerator：Winform前端，目的是简化用户端操作。包装comfyui工作流需要上传的内容，并通过requests进行文件上转/下载。
2. utils：python工具箱，提供一些批处理图片操作、文件操作、批量转绘等功能。
