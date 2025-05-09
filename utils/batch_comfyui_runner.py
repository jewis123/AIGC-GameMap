import os
import json
import tkinter as tk
from tkinter import filedialog, messagebox, ttk
import requests
from PIL import Image
import threading
import time
import base64

COMFYUI_URL = "http://192.168.1.12:8089"
WORKFLOW_PATH = os.path.join(os.path.dirname(__file__), "workflow", "2dmapIPAdapter2.json")
g_loras = {"吉卜力":""}

class BatchComfyUIApp:
    def __init__(self, root):
        self.root = root
        self.create_widgets()

    def create_widgets(self):
        frm = tk.Frame(self.root)
        frm.pack(pady=20)
        tk.Label(frm, text="图片目录:").pack(side=tk.LEFT)
        self.dir_var = tk.StringVar()
        tk.Entry(frm, textvariable=self.dir_var, width=40).pack(side=tk.LEFT, padx=5)
        tk.Button(frm, text="选择", command=self.choose_dir).pack(side=tk.LEFT)

        frm2 = tk.Frame(self.root)
        frm2.pack(pady=10)
        tk.Label(frm2, text="保存目录:").pack(side=tk.LEFT)
        self.save_var = tk.StringVar()
        tk.Entry(frm2, textvariable=self.save_var, width=40).pack(side=tk.LEFT, padx=5)
        tk.Button(frm2, text="选择", command=self.choose_save_dir).pack(side=tk.LEFT)

        # 新增lora选择控件
        frm3 = tk.Frame(self.root)
        frm3.pack(pady=10)
        tk.Label(frm3, text="风格模型:").pack(side=tk.LEFT)
        self.lora_var = tk.StringVar()
        self.lora_combo = ttk.Combobox(frm3, textvariable=self.lora_var, values=list(g_loras.keys()), state="readonly", width=20)
        self.lora_combo.pack(side=tk.LEFT, padx=5)
        if g_loras:
            self.lora_combo.current(0)

        self.progress = tk.DoubleVar()
        self.progressbar = ttk.Progressbar(self.root, length=500, variable=self.progress, maximum=100)
        self.progressbar.pack(pady=10, padx=10, fill='x')

        desc = (
            "功能：批量将目录下图片通过ComfyUI工作流转绘，支持进度显示。\n"
            "1. 选择图片目录和保存目录。\n"
            "2. 点击‘开始批量转绘’。\n"
            "3. 进度条显示处理进度，结果保存在保存目录。"
        )
        tk.Label(self.root, text=desc, fg="#555", justify='left', anchor='w').pack(fill='x', padx=10, pady=(0,10))

        self.status_var = tk.StringVar(value="准备就绪")
        status_frame = tk.Frame(self.root)
        status_frame.pack(fill=tk.X, side=tk.BOTTOM)
        tk.Label(status_frame, textvariable=self.status_var, bd=1, relief=tk.SUNKEN, anchor=tk.W).pack(side=tk.LEFT, fill=tk.X, expand=True)
        
        btn_frame = tk.Frame(self.root)
        btn_frame.pack(pady=10)
        self.batch_btn = tk.Button(btn_frame, text="开始批量转绘", command=self.start_batch, bg="#4CAF50", fg="white", height=2, width=20)
        self.batch_btn.pack(side=tk.LEFT, padx=10)
        self.stop_btn = tk.Button(btn_frame, text="终止任务", command=self.stop_batch, bg="#F44336", fg="white", height=2, width=20, state=tk.DISABLED)
        self.stop_btn.pack(side=tk.LEFT, padx=10)

    def choose_dir(self):
        path = filedialog.askdirectory()
        if path:
            self.dir_var.set(path)

    def choose_save_dir(self):
        path = filedialog.askdirectory()
        if path:
            self.save_var.set(path)

    def stop_batch(self):
        self._stop_flag = True
        self._animate_progress = False
        self.status_var.set("已请求终止，正在通知ComfyUI...")
        self.root.update()
        try:
            # 通知ComfyUI终止所有任务
            requests.post(f"{COMFYUI_URL}/interrupt")
            requests.post(f"{COMFYUI_URL}/queue/clear")
        except Exception as e:
            pass
        self.status_var.set("所有任务已请求终止。")
        self.batch_btn.config(state=tk.NORMAL)
        self.stop_btn.config(state=tk.DISABLED)

    def start_batch(self):
        img_dir = self.dir_var.get()
        save_dir = self.save_var.get()
        # 获取lora选择
        lora_key = self.lora_var.get()
        lora_value = g_loras.get(lora_key, "")
        if not os.path.isdir(img_dir) or not os.path.isdir(save_dir):
            messagebox.showerror("错误", "请选择有效的图片目录和保存目录！")
            return
        img_files = [f for f in os.listdir(img_dir) if f.lower().endswith((".png", ".jpg", ".jpeg"))]
        if not img_files:
            messagebox.showinfo("提示", "目录下没有可处理的图片文件")
            return
        with open(WORKFLOW_PATH, "r", encoding="utf-8") as f:
            workflow_template = json.load(f)
        total = len(img_files)
        self.progress.set(0)
        self.status_var.set("正在提交任务到队列...")
        self.root.update()
        self.batch_btn.config(state=tk.DISABLED)
        self.stop_btn.config(state=tk.NORMAL)
        self._stop_flag = False
        self._animate_progress = True
        def animate():
            val = 0
            while self._animate_progress:
                try:
                    prog = self.progress.get()
                except Exception:
                    prog = 0
                if prog < 99:
                    val = (val + 2) % 100
                    if prog < 1:
                        self.progress.set(val)
                time.sleep(0.05)
        def batch_thread():
            # 1. 先批量上传图片，记录本地文件名与服务器文件名的映射
            fname_to_server = {}
            for fname in img_files:
                img_path = os.path.join(img_dir, fname)
                try:
                    with open(img_path, 'rb') as fimg:
                        files = {'image': (fname, fimg, 'application/octet-stream')}
                        resp = requests.post(f"{COMFYUI_URL}/upload/image", files=files)
                        resp.raise_for_status()
                        result = resp.json()
                        server_fname = result.get('name', fname)
                        fname_to_server[fname] = server_fname
                except Exception as e:
                    print(f"上传{fname}失败: {e}")
                    fname_to_server[fname] = fname  # 失败时仍用原名，防止后续崩溃
            prompt_id_map = {}
            for idx, fname in enumerate(img_files):
                server_fname = fname_to_server.get(fname, fname)
                workflow = json.loads(json.dumps(workflow_template))
                workflow["194"]["inputs"]["image"] = server_fname  # 只用文件名
                # 替换lora_name
                if "209" in workflow and "inputs" in workflow["209"]:
                    workflow["209"]["inputs"]["lora_name"] = lora_value
                prompt = {"prompt": workflow}
                try:
                    resp = requests.post(f"{COMFYUI_URL}/prompt", json=prompt)
                    resp.raise_for_status()
                    pid = resp.json().get("prompt_id")
                    if pid:
                        prompt_id_map[pid] = fname
                except Exception as e:
                    print(f"提交{fname}失败: {e}")
            finished = set()
            self.status_var.set(f"已提交{len(prompt_id_map)}个任务，正在等待结果...")
            self.root.update()
            total = len(prompt_id_map)
            while len(finished) < total and not self._stop_flag:
                for pid, fname in prompt_id_map.items():
                    if pid in finished:
                        continue
                    if self._stop_flag:
                        break
                    try:
                        resp = requests.get(f"{COMFYUI_URL}/history/{pid}")
                        data = resp.json()
                        # 兼容ComfyUI新版history返回格式：{prompt_id: {...}}
                        prompt_data = None
                        if isinstance(data, dict):
                            if pid in data and isinstance(data[pid], dict):
                                prompt_data = data[pid]
                            elif 'outputs' in data and isinstance(data['outputs'], dict):
                                prompt_data = data
                        if prompt_data:
                            outputs = prompt_data.get('outputs', {})
                            node_out = outputs.get("229") if isinstance(outputs, dict) else None
                            if node_out and "images" in node_out and node_out["images"]:
                                img_info = node_out["images"][0]
                                if "filename" in img_info:
                                    url = f"{COMFYUI_URL}/view?filename={img_info['filename']}"
                                    img_data = requests.get(url).content
                                    out_path = os.path.join(save_dir, fname)
                                    with open(out_path, "wb") as f:
                                        f.write(img_data)
                                elif "data" in img_info:
                                    img_data = base64.b64decode(img_info["data"])
                                    out_path = os.path.join(save_dir, fname)
                                    with open(out_path, "wb") as f:
                                        f.write(img_data)
                                finished.add(pid)
                                self.root.after(0, self.progress.set, len(finished)/total*100)
                                self.root.after(0, self.status_var.set, f"已完成{len(finished)}/{total}，正在保存：{fname}")
                                self.root.update()
                            else:
                                # 检查是否执行完成但无图片
                                status = prompt_data.get('status', '')
                                if status in ("complete", "success"):
                                    finished.add(pid)
                                    self.root.after(0, self.progress.set, len(finished)/total*100)
                                    self.root.after(0, self.status_var.set, f"已完成{len(finished)}/{total}")
                                    self.root.update()
                    except Exception as e:
                        continue
                time.sleep(5)  # 轮询频率改为5秒1次
            self._animate_progress = False
            if self._stop_flag:
                self.root.after(0, self.status_var.set, "任务已终止！")
            else:
                self.root.after(0, self.progress.set, 100)
                self.root.after(0, self.status_var.set, "全部任务已完成！")
            self.root.after(0, self.batch_btn.config, {"state": tk.NORMAL})
            self.root.after(0, self.stop_btn.config, {"state": tk.DISABLED})
            if not self._stop_flag:
                messagebox.showinfo("完成", f"共处理{total}张图片。")
            self.progress.set(0)
        threading.Thread(target=animate, daemon=True).start()
        threading.Thread(target=batch_thread, daemon=True).start()

if __name__ == "__main__":
    root = tk.Tk()
    app = BatchComfyUIApp(root)
    root.mainloop()
