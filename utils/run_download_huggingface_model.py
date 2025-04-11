from huggingface_hub import snapshot_download

repo_id = "JunhaoZhuang/PowerPaint_v2"
local_dir = "G:\\AI\\MapGenerator\\utils\\models\\brushnet"  # 替换为你想要保存的路径
snapshot_download(repo_id=repo_id, local_dir=local_dir)