import os
import tkinter as tk
from tkinter import filedialog, messagebox, ttk
from PIL import Image
import re

class ImageMergeSplitApp:
    def __init__(self, root):
        self.root = root
        self.root.title("图片合并与切分工具")
        self.root.geometry("650x500")
        notebook = ttk.Notebook(self.root)
        notebook.pack(fill=tk.BOTH, expand=True)

        # 合并功能页
        self.merge_tab = tk.Frame(notebook)
        self._init_merge_tab(self.merge_tab)
        notebook.add(self.merge_tab, text="批量合并")

        # 切分功能页
        self.split_tab = tk.Frame(notebook)
        self._init_split_tab(self.split_tab)
        notebook.add(self.split_tab, text="批量切分")

    def _init_merge_tab(self, tab):
        # 目录选择
        frm = tk.Frame(tab)
        frm.pack(pady=10, fill='x')
        tk.Label(frm, text="目标目录:").pack(side='left')
        self.merge_src_var = tk.StringVar()
        tk.Entry(frm, textvariable=self.merge_src_var, width=40).pack(side='left', padx=5)
        tk.Button(frm, text="选择", command=self._choose_merge_src).pack(side='left')

        frm2 = tk.Frame(tab)
        frm2.pack(pady=10, fill='x')
        tk.Label(frm2, text="保存目录:").pack(side='left')
        self.merge_dst_var = tk.StringVar()
        tk.Entry(frm2, textvariable=self.merge_dst_var, width=40).pack(side='left', padx=5)
        tk.Button(frm2, text="选择", command=self._choose_merge_dst).pack(side='left')

        # 进度条
        self.merge_progress = tk.DoubleVar()
        self.merge_progressbar = ttk.Progressbar(tab, length=500, variable=self.merge_progress, maximum=100)
        self.merge_progressbar.pack(pady=10, padx=10, fill='x')

        # 功能说明
        desc = (
            "功能：将目标目录下所有具有相同前缀不同数字索引的图片合并为一张大图，\n"
            "合并图片命名包含信息以便后续切分，统一保存到保存目录。\n"
            "使用说明：\n"
            "1. 选择目标目录和保存目录。\n"
            "2. 点击‘批量合并’按钮。\n"
            "3. 合并结果会保存在保存目录。"
        )
        tk.Label(tab, text=desc, fg="#555", justify='left', anchor='w').pack(fill='x', padx=10, pady=(0,10))

        # 按钮
        tk.Button(tab, text="批量合并", command=self._batch_merge).pack(pady=10)

    def _init_split_tab(self, tab):
        # 目录选择
        frm = tk.Frame(tab)
        frm.pack(pady=10, fill='x')
        tk.Label(frm, text="目标目录:").pack(side='left')
        self.split_src_var = tk.StringVar()
        tk.Entry(frm, textvariable=self.split_src_var, width=40).pack(side='left', padx=5)
        tk.Button(frm, text="选择", command=self._choose_split_src).pack(side='left')

        frm2 = tk.Frame(tab)
        frm2.pack(pady=10, fill='x')
        tk.Label(frm2, text="切分目录:").pack(side='left')
        self.split_dst_var = tk.StringVar()
        tk.Entry(frm2, textvariable=self.split_dst_var, width=40).pack(side='left', padx=5)
        tk.Button(frm2, text="选择", command=self._choose_split_dst).pack(side='left')

        # 进度条
        self.split_progress = tk.DoubleVar()
        self.split_progressbar = ttk.Progressbar(tab, length=500, variable=self.split_progress, maximum=100)
        self.split_progressbar.pack(pady=10, padx=10, fill='x')

        # 功能说明
        desc = (
            "功能：将目标目录下所有可切分图片切成散图，与不可切分图片一起保存到切分目录。\n"
            "使用说明：\n"
            "1. 选择目标目录和切分目录。\n"
            "2. 点击‘批量切分’按钮。\n"
            "3. 切分结果会保存在切分目录。"
        )
        tk.Label(tab, text=desc, fg="#555", justify='left', anchor='w').pack(fill='x', padx=10, pady=(0,10))

        # 按钮
        tk.Button(tab, text="批量切分", command=self._batch_split).pack(pady=10)

    def _choose_merge_src(self):
        path = filedialog.askdirectory()
        if path:
            self.merge_src_var.set(path)

    def _choose_merge_dst(self):
        path = filedialog.askdirectory()
        if path:
            self.merge_dst_var.set(path)

    def _choose_split_src(self):
        path = filedialog.askdirectory()
        if path:
            self.split_src_var.set(path)

    def _choose_split_dst(self):
        path = filedialog.askdirectory()
        if path:
            self.split_dst_var.set(path)

    def _batch_merge(self):
        src_dir = self.merge_src_var.get()
        dst_dir = self.merge_dst_var.get()
        if not os.path.isdir(src_dir) or not os.path.isdir(dst_dir):
            messagebox.showerror("错误", "请选择有效的目标目录和保存目录！")
            return
        # 分组图片
        files = [f for f in os.listdir(src_dir) if f.lower().endswith(('.png', '.jpg', '.jpeg'))]
        groups = {}
        for f in files:
            m = re.match(r'(.+?)(?:_|-)?(\d+)\.(png|jpg|jpeg)$', f, re.IGNORECASE)
            if m:
                prefix = m.group(1)
                groups.setdefault(prefix, []).append(f)
        total = len(groups)
        for idx, (prefix, flist) in enumerate(groups.items()):
            images = [Image.open(os.path.join(src_dir, fn)) for fn in sorted(flist, key=lambda x: int(re.findall(r'(\d+)', x)[-1]))]
            # 横向拼接
            widths, heights = zip(*(im.size for im in images))
            total_width = sum(widths)
            max_height = max(heights)
            merged = Image.new('RGBA', (total_width, max_height))
            x = 0
            for im in images:
                merged.paste(im, (x, 0))
                x += im.width
            # 命名规则：前缀_合并数量_mergeinfo.png
            out_name = f"{prefix}_{len(images)}_mergeinfo.png"
            merged.save(os.path.join(dst_dir, out_name))
            self.merge_progress.set((idx+1)/total*100)
            self.root.update()
        messagebox.showinfo("完成", f"共合并{total}组图片。")
        self.merge_progress.set(0)

    def _batch_split(self):
        src_dir = self.split_src_var.get()
        dst_dir = self.split_dst_var.get()
        if not os.path.isdir(src_dir) or not os.path.isdir(dst_dir):
            messagebox.showerror("错误", "请选择有效的目标目录和切分目录！")
            return
        files = [f for f in os.listdir(src_dir) if f.lower().endswith(('.png', '.jpg', '.jpeg'))]
        total = len(files)
        for idx, f in enumerate(files):
            path = os.path.join(src_dir, f)
            if '_mergeinfo' in f:
                # 解析合并信息
                m = re.match(r'(.+?)_(\d+)_mergeinfo\.(png|jpg|jpeg)$', f, re.IGNORECASE)
                if m:
                    prefix, count = m.group(1), int(m.group(2))
                    img = Image.open(path)
                    w, h = img.size
                    sub_w = w // count
                    for i in range(count):
                        crop = img.crop((i*sub_w, 0, (i+1)*sub_w, h))
                        out_name = f"{prefix}_{i+1}.{m.group(3)}"
                        crop.save(os.path.join(dst_dir, out_name))
                else:
                    # 命名不符，直接复制
                    Image.open(path).save(os.path.join(dst_dir, f))
            else:
                # 不可切分图片直接保存
                Image.open(path).save(os.path.join(dst_dir, f))
            self.split_progress.set((idx+1)/total*100)
            self.root.update()
        messagebox.showinfo("完成", f"共处理{total}张图片。")
        self.split_progress.set(0)

if __name__ == "__main__":
    root = tk.Tk()
    app = ImageMergeSplitApp(root)
    root.mainloop()
