# -*- coding: utf-8 -*-
###【【批量重命名图片工具】】

# 批量重命名图片工具
# 1. 批量重命名图片，将图片名称改为以子目录名开头，后接编号，编号不带前导0
# 2. 批量重命名tmx文件，将tmx文件中的图片路径改为以子目录名开头，后接编号，编号不带前导0

###

import os
import re
import tkinter as tk
from tkinter import filedialog, messagebox, ttk
import threading
import xml.etree.ElementTree as ET


def correct_image_names(root_dir, tmx_dir=None, progress_callback=None):
    changed = 0
    problem_dirs = []
    tmx_map = {}
    # 预加载tmx文件（以子目录名为key，值为tmx文件绝对路径）
    if tmx_dir and os.path.isdir(tmx_dir):
        for fname in os.listdir(tmx_dir):
            if fname.lower().endswith('.tmx'):
                key = os.path.splitext(fname)[0]
                tmx_map[key] = os.path.join(tmx_dir, fname)
    subdirs = [d for d in os.listdir(root_dir) if os.path.isdir(os.path.join(root_dir, d))]
    total = len(subdirs)
    for idx, subdir in enumerate(subdirs, 1):
        subdir_path = os.path.join(root_dir, subdir)
        files = [f for f in os.listdir(subdir_path)]
        idx_img = 1
        subdir_changed = False
        rename_map = {}  # old_name: new_name
        for fname in files:
            name, ext = os.path.splitext(fname)
            if name[:3] != subdir[:3]:
                # 名称不符合子目录前缀，跳过
                continue
            # 检查是否符合“subdir_{编号}”且编号不带0开头
            match = re.fullmatch(rf'{re.escape(subdir)}_(\d+)', name)
            if match:
                num = match.group(1)
                if num.startswith('0') and len(num) > 1:
                    # 有前导0，重命名为无前导0
                    new_name = f"{subdir}_{int(num)}{ext}"
                else:
                    continue  # 已规范
            else:
                # 不规范，直接用递增编号重命名
                new_name = f"{subdir}_{idx_img}{ext}"
            new_path = os.path.join(subdir_path, new_name)
            old_path = os.path.join(subdir_path, fname)
            while os.path.exists(new_path):
                idx_img += 1
                new_name = f"{subdir}_{idx_img}{ext}"
                new_path = os.path.join(subdir_path, new_name)
            os.rename(old_path, new_path)
            rename_map[fname] = new_name
            changed += 1
            idx_img += 1
            subdir_changed = True
        # tmx同步
        if subdir_changed and subdir in tmx_map:
            tmx_path = tmx_map[subdir]
            tree = ET.parse(tmx_path)
            root = tree.getroot()
            for tileset in root.findall('tileset'):
                for tile in tileset.findall('tile'):
                    image = tile.find('image')
                    if image is not None and 'source' in image.attrib:
                        src = image.attrib['source']
                        src_name = os.path.basename(src)
                        if src_name in rename_map:
                            new_src = src.replace(src_name, rename_map[src_name])
                            image.set('source', new_src)
            tree.write(tmx_path, encoding='utf-8')
        if subdir_changed:
            problem_dirs.append(subdir)
        if progress_callback:
            progress_callback(idx, total)
    return changed, problem_dirs

class RenameCorrectorApp:
    def __init__(self, master):
        self.master = master
        self.dir_var = tk.StringVar()
        self.tmx_dir_var = tk.StringVar()

        # 目标目录和tmx目录选择控件
        frame_dirs = tk.Frame(master)
        frame_dirs.pack(padx=10, pady=10)
        
        # 目标目录选择控件
        frame_target_dir = tk.Frame(frame_dirs)
        frame_target_dir.pack(fill=tk.X, pady=5)
        tk.Label(frame_target_dir, text="目标目录:").pack(side=tk.LEFT, padx=5)
        tk.Entry(frame_target_dir, textvariable=self.dir_var, width=32).pack(side=tk.LEFT, padx=5)
        tk.Button(frame_target_dir, text="选择目录", command=self.browse_dir).pack(side=tk.LEFT, padx=5)
        
        # tmx目录选择控件
        frame_tmx_dir = tk.Frame(frame_dirs)
        frame_tmx_dir.pack(fill=tk.X, pady=5)
        tk.Label(frame_tmx_dir, text="tmx目录:").pack(side=tk.LEFT, padx=5)
        tk.Entry(frame_tmx_dir, textvariable=self.tmx_dir_var, width=32).pack(side=tk.LEFT, padx=5)
        tk.Button(frame_tmx_dir, text="选择tmx", command=self.browse_tmx_dir).pack(side=tk.LEFT, padx=5)
        
        
        # 命名纠错按钮
        frame_button = tk.Frame(master)
        frame_button.pack(pady=10)
        tk.Button(frame_button, text="命名纠错", bg="#FF9800", fg="white", command=self.start_correction_thread).pack(side=tk.LEFT, padx=5)


        # 进度条
        self.progress_var = tk.DoubleVar()
        self.progress_bar = ttk.Progressbar(master, variable=self.progress_var, maximum=100)
        self.progress_bar.pack(fill=tk.X, padx=10, pady=5)
        self.progress_label = tk.Label(master, text="")
        self.progress_label.pack(padx=10, anchor=tk.W)
        
        
        # 功能说明与使用说明
        desc = (
            "功能：批量重命名图片为“子目录名_编号”格式，并同步修正TMX文件中的图片路径。\n"
            "使用说明：\n"
            "1. 选择目标图片目录(散图根目录)和TMX目录（地图集根目录）。\n"
            "2. 点击“命名纠错”按钮，等待进度完成。\n"
            "3. 纠错完成后会弹窗提示结果。"
        )
        tk.Label(master, text=desc, fg="#555", justify='left', anchor='w').pack(fill='x', padx=10, pady=(0,10))

    def browse_dir(self):
        d = filedialog.askdirectory()
        if d:
            self.dir_var.set(d)

    def browse_tmx_dir(self):
        d = filedialog.askdirectory()
        if d:
            self.tmx_dir_var.set(d)

    def start_correction_thread(self):
        root_dir = self.dir_var.get().strip()
        tmx_dir = self.tmx_dir_var.get().strip()
        if not root_dir or not os.path.isdir(root_dir):
            messagebox.showerror("错误", "请选择有效的目录")
            return
        self.progress_var.set(0)
        self.progress_label.config(text="正在扫描并纠错...")
        threading.Thread(target=self.run_correction, args=(root_dir, tmx_dir), daemon=True).start()

    def update_progress(self, idx, total):
        percent = idx / total * 100
        self.progress_var.set(percent)
        self.progress_label.config(text=f"进度：{idx}/{total}")

    def run_correction(self, root_dir, tmx_dir):
        changed, problem_dirs = correct_image_names(
            root_dir, tmx_dir=tmx_dir,
            progress_callback=lambda idx, total: self.master.after(0, self.update_progress, idx, total)
        )
        msg = f"命名纠错完成，共修改 {changed} 个文件。"
        if problem_dirs:
            msg += "\n\n以下子目录存在命名问题并已修正：\n" + "\n".join(problem_dirs)
        else:
            msg += "\n\n所有子目录命名均已规范。"
        self.master.after(0, lambda: self.progress_label.config(text=""))
        self.master.after(0, lambda: messagebox.showinfo("完成", msg))

if __name__ == "__main__":
    root = tk.Tk()
    app = RenameCorrectorApp(root)
    root.mainloop()