import os
import tkinter as tk
from tkinter import filedialog, messagebox, ttk
from PIL import Image
import re
import numpy as np

class ImageStitcherApp:
    def __init__(self, root):
        self.root = root
        self.root.title("图片拼接/切分工具")
        self.root.geometry("520x380")
        self.create_widgets()

    def create_widgets(self):
        notebook = ttk.Notebook(self.root)
        notebook.pack(fill=tk.BOTH, expand=True)

        # 合图tab
        self.stitch_tab = tk.Frame(notebook)
        notebook.add(self.stitch_tab, text="合并图片")
        # 切图tab
        self.split_tab = tk.Frame(notebook)
        notebook.add(self.split_tab, text="切分整图")

        # 合图界面
        dir_frame = tk.Frame(self.stitch_tab)
        dir_frame.pack(fill=tk.X, padx=10, pady=10)
        tk.Label(dir_frame, text="图片目录:").pack(side=tk.LEFT)
        self.dir_path = tk.StringVar()
        tk.Entry(dir_frame, textvariable=self.dir_path, width=40).pack(side=tk.LEFT, padx=5)
        tk.Button(dir_frame, text="浏览...", command=self.browse_directory).pack(side=tk.LEFT)

        # 新增保存目录选择
        save_dir_frame = tk.Frame(self.stitch_tab)
        save_dir_frame.pack(fill=tk.X, padx=10, pady=5)
        tk.Label(save_dir_frame, text="保存目录:").pack(side=tk.LEFT)
        self.save_dir = tk.StringVar()
        tk.Entry(save_dir_frame, textvariable=self.save_dir, width=36).pack(side=tk.LEFT, padx=5)
        tk.Button(save_dir_frame, text="选择...", command=self.browse_save_directory).pack(side=tk.LEFT)

        size_frame = tk.Frame(self.stitch_tab)
        size_frame.pack(fill=tk.X, padx=10, pady=10)
        tk.Label(size_frame, text="宽度图片数:").pack(side=tk.LEFT)
        self.width_count = tk.IntVar(value=2)
        tk.Entry(size_frame, textvariable=self.width_count, width=5).pack(side=tk.LEFT, padx=5)
        tk.Label(size_frame, text="高度图片数:").pack(side=tk.LEFT, padx=(20, 0))
        self.height_count = tk.IntVar(value=2)
        tk.Entry(size_frame, textvariable=self.height_count, width=5).pack(side=tk.LEFT, padx=5)

        button_frame = tk.Frame(self.stitch_tab)
        button_frame.pack(fill=tk.X, padx=10, pady=20)
        tk.Button(button_frame, text="合并图片", command=self.stitch_images, 
                  bg="#4CAF50", fg="white", height=2, width=20).pack()

        # 切图界面
        split_frame = tk.Frame(self.split_tab)
        split_frame.pack(fill=tk.X, padx=10, pady=10)
        tk.Label(split_frame, text="整图路径:").pack(side=tk.LEFT)
        self.whole_img_path = tk.StringVar()
        tk.Entry(split_frame, textvariable=self.whole_img_path, width=32).pack(side=tk.LEFT, padx=5)
        tk.Button(split_frame, text="选择整图...", command=self.browse_whole_image).pack(side=tk.LEFT)

        split_button_frame = tk.Frame(self.split_tab)
        split_button_frame.pack(fill=tk.X, padx=10, pady=20)
        tk.Button(split_button_frame, text="切分整图", command=self.split_image, 
                  bg="#2196F3", fg="white", height=2, width=20).pack()

        # 状态栏
        self.status_var = tk.StringVar(value="准备就绪")
        tk.Label(self.root, textvariable=self.status_var, bd=1, relief=tk.SUNKEN, anchor=tk.W).pack(side=tk.BOTTOM, fill=tk.X)

    def browse_directory(self):
        directory = filedialog.askdirectory()
        if directory:
            self.dir_path.set(directory)

    def browse_save_directory(self):
        directory = filedialog.askdirectory()
        if directory:
            self.save_dir.set(directory)

    def browse_whole_image(self):
        file_path = filedialog.askopenfilename(filetypes=[("Image files", "*.png;*.jpg;*.jpeg;*.bmp")])
        if file_path:
            self.whole_img_path.set(file_path)

    def natural_key(self, s):
        return [int(text) if text.isdigit() else text.lower() for text in re.split('(\d+)', s)]

    def stitch_images(self):
        dir_path = self.dir_path.get()
        width_count = self.width_count.get()
        height_count = self.height_count.get()
        save_dir = self.save_dir.get().strip()
        if not dir_path:
            messagebox.showerror("错误", "请选择图片目录")
            return
        if not save_dir:
            messagebox.showerror("错误", "请选择整图保存目录")
            return
        try:
            self.status_var.set("正在处理图片...")
            self.root.update()
            image_files = [f for f in os.listdir(dir_path) if f.lower().endswith(('.png', '.jpg', '.jpeg', '.bmp'))]
            if len(image_files) < width_count * height_count:
                messagebox.showerror("错误", f"目录中的图片数量不足 ({len(image_files)}个), 需要 {width_count * height_count}个")
                self.status_var.set("准备就绪")
                return
            image_map = {}
            sorted_files = sorted(image_files, key=self.natural_key)
            for i in range(height_count):
                for j in range(width_count):
                    idx = i * width_count + j
                    if idx < len(sorted_files):
                        image_map[(i, j)] = os.path.join(dir_path, sorted_files[idx])
            sample_img = Image.open(list(image_map.values())[0])
            img_width, img_height = sample_img.size
            result_img = Image.new('RGB', (img_width * width_count, img_height * height_count))
            for (row, col), img_path in image_map.items():
                img = Image.open(img_path)
                result_img.paste(img, (col * img_width, row * img_height))
            folder_name = os.path.basename(os.path.normpath(dir_path))
            default_filename = f"{folder_name}_{width_count}x{height_count}.png"
            os.makedirs(save_dir, exist_ok=True)
            save_path = os.path.join(save_dir, default_filename)
            if save_path:
                result_img.save(save_path)
                messagebox.showinfo("成功", f"图片已成功合并并保存到:\n{save_path}")
                self.status_var.set(f"图片已保存到: {save_path}")
            else:
                self.status_var.set("已取消保存")
        except Exception as e:
            messagebox.showerror("错误", f"合并过程中出错:\n{str(e)}")
            self.status_var.set("发生错误")

    def split_image(self):
        img_path = self.whole_img_path.get()
        if not img_path:
            messagebox.showerror("错误", "请选择要切分的整图")
            return
        # 自动从文件名解析宽高
        base_name = os.path.splitext(os.path.basename(img_path))[0]
        match = re.search(r'_(\d+)x(\d+)$', base_name)
        if not match:
            messagebox.showerror("错误", "整图文件名需以 _宽x高 结尾，如 xxx_3x4.png")
            return
        width_count = int(match.group(1))
        height_count = int(match.group(2))
        try:
            self.status_var.set("正在切分整图...")
            self.root.update()
            img = Image.open(img_path)
            img_width, img_height = img.size
            tile_width = img_width // width_count
            tile_height = img_height // height_count
            save_dir = os.path.join(os.getcwd(), "image_resizer", f"split_{base_name}")
            os.makedirs(save_dir, exist_ok=True)
            for i in range(height_count):
                for j in range(width_count):
                    left = j * tile_width
                    upper = i * tile_height
                    right = left + tile_width
                    lower = upper + tile_height
                    tile = img.crop((left, upper, right, lower))
                    tile_name = f"{base_name}_{i*width_count+j+1}.png"
                    tile.save(os.path.join(save_dir, tile_name))
            messagebox.showinfo("成功", f"整图已切分为{width_count*height_count}张图片，保存于:\n{save_dir}")
            self.status_var.set(f"切分完成，保存于: {save_dir}")
        except Exception as e:
            messagebox.showerror("错误", f"切分过程中出错:\n{str(e)}")
            self.status_var.set("发生错误")

if __name__ == "__main__":
    root = tk.Tk()
    app = ImageStitcherApp(root)
    root.mainloop()
