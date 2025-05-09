import os
from concurrent.futures import ThreadPoolExecutor, as_completed

os.environ["HF_ENDPOINT"] = "https://hf-mirror.com"

download_cmds = [
    'huggingface-cli download --resume-download google/siglip-so400m-patch14-384 --local-dir G:\\LiblibAI-workspace\\Models\\clip\\siglip-so400m-patch14-384',
    'huggingface-cli download --resume-download unsloth/Meta-Llama-3.1-8B-Instruct-bnb-4bit --local-dir G:\\LiblibAI-workspace\\Models\\LLM\\Meta-Llama-3.1-8B-Instruct-bnb-4bit',
    'huggingface-cli download --resume-download John6666/Llama-3.1-8B-Lexi-Uncensored-V2-nf4 --local-dir G:\\LiblibAI-workspace\\Models\\LLM\\Llama-3.1-8B-Lexi-Uncensored-V2-nf4'
]

def run_cmd(cmd):
    os.system(cmd)

with ThreadPoolExecutor(max_workers=3) as executor:
    futures = [executor.submit(run_cmd, cmd) for cmd in download_cmds]
    for future in as_completed(futures):
        future.result()