from huggingface_hub import snapshot_download

repo_id = "MonsterMMORPG/tools"
local_dir = "G:\\AI\\MapGenerator\\utils\\models\\antelopev2"  # 替换为你想要保存的路径
snapshot_download(repo_id=repo_id, local_dir=local_dir)