from huggingface_hub import snapshot_download

repo_id = "XLabs-AI/flux-lora-collection"
local_dir = "F:\LiblibAI-workspace\Models\loras"  # 替换为你想要保存的路径
snapshot_download(repo_id=repo_id, local_dir=local_dir)