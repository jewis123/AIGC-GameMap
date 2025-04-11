import os
import tkinter as tk
from tkinter import filedialog, messagebox
from PIL import Image
import re
import numpy as np

class ImageStitcherApp:
    def __init__(self, root):
        self.root = root
        self.root.title("图片拼接工具")
        self.root.geometry("500x300")
        
        # 创建界面元素
        self.create_widgets()
        
    def create_widgets(self):
        # 目录选择
        dir_frame = tk.Frame(self.root)
        dir_frame.pack(fill=tk.X, padx=10, pady=10)
        
        tk.Label(dir_frame, text="图片目录:").pack(side=tk.LEFT)
        self.dir_path = tk.StringVar()
        tk.Entry(dir_frame, textvariable=self.dir_path, width=40).pack(side=tk.LEFT, padx=5)
        tk.Button(dir_frame, text="浏览...", command=self.browse_directory).pack(side=tk.LEFT)
        
        # 尺寸设置
        size_frame = tk.Frame(self.root)
        size_frame.pack(fill=tk.X, padx=10, pady=10)
        
        tk.Label(size_frame, text="宽度图片数:").pack(side=tk.LEFT)
        self.width_count = tk.IntVar(value=2)
        tk.Entry(size_frame, textvariable=self.width_count, width=5).pack(side=tk.LEFT, padx=5)
        
        tk.Label(size_frame, text="高度图片数:").pack(side=tk.LEFT, padx=(20, 0))
        self.height_count = tk.IntVar(value=2)
        tk.Entry(size_frame, textvariable=self.height_count, width=5).pack(side=tk.LEFT, padx=5)
        
        # 执行按钮
        button_frame = tk.Frame(self.root)
        button_frame.pack(fill=tk.X, padx=10, pady=20)
        
        tk.Button(button_frame, text="合并图片", command=self.stitch_images, 
                  bg="#4CAF50", fg="white", height=2, width=20).pack()
        
        # 状态标签
        self.status_var = tk.StringVar(value="准备就绪")
        tk.Label(self.root, textvariable=self.status_var, bd=1, relief=tk.SUNKEN, anchor=tk.W).pack(side=tk.BOTTOM, fill=tk.X)
    
    def browse_directory(self):
        directory = filedialog.askdirectory()
        if directory:
            self.dir_path.set(directory)
    
    def stitch_images(self):
        dir_path = self.dir_path.get()
        width_count = self.width_count.get()
        height_count = self.height_count.get()
        
        if not dir_path:
            messagebox.showerror("错误", "请选择图片目录")
            return
        
        try:
            self.status_var.set("正在处理图片...")
            self.root.update()
            
            # 获取所有图片文件
            image_files = [f for f in os.listdir(dir_path) if f.lower().endswith(('.png', '.jpg', '.jpeg', '.bmp'))]
            
            if len(image_files) < width_count * height_count:
                messagebox.showerror("错误", f"目录中的图片数量不足 ({len(image_files)}个), 需要 {width_count * height_count}个")
                self.status_var.set("准备就绪")
                return
            
            # 尝试提取图片名称中的索引 (如 "tile_1_2.png")
            image_map = {}
            pattern = re.compile(r'.*?(\d+)[_-](\d+).*')
            
            # 首先尝试从文件名解析位置
            for img_file in image_files:
                match = pattern.match(img_file)
                if match:
                    try:
                        row = int(match.group(1))
                        col = int(match.group(2))
                        if 0 <= row < height_count and 0 <= col < width_count:
                            image_map[(row, col)] = os.path.join(dir_path, img_file)
                    except ValueError:
                        pass
            
            # 如果没有足够的匹配项，则按字母顺序排列
            if len(image_map) != width_count * height_count:
                image_map = {}
                sorted_files = sorted(image_files)
                for i in range(height_count):
                    for j in range(width_count):
                        idx = i * width_count + j
                        if idx < len(sorted_files):
                            image_map[(i, j)] = os.path.join(dir_path, sorted_files[idx])
            
            # 加载第一张图片以获取尺寸
            sample_img = Image.open(list(image_map.values())[0])
            img_width, img_height = sample_img.size
            
            # 创建拼接图片
            result_img = Image.new('RGB', (img_width * width_count, img_height * height_count))
            
            # 粘贴图片到结果中
            for (row, col), img_path in image_map.items():
                img = Image.open(img_path)
                result_img.paste(img, (col * img_width, row * img_height))
            
            # 获取文件夹名称作为默认文件名
            folder_name = os.path.basename(os.path.normpath(dir_path))
            default_filename = f"{folder_name}.png"
            
            # 保存结果
            save_path = filedialog.asksaveasfilename(
                defaultextension=".png",
                filetypes=[("PNG 文件", "*.png"), ("JPG 文件", "*.jpg"), ("所有文件", "*.*")],
                initialdir=os.path.dirname(dir_path),
                initialfile=default_filename
            )
            
            if save_path:
                result_img.save(save_path)
                messagebox.showinfo("成功", f"图片已成功合并并保存到:\n{save_path}")
                self.status_var.set(f"图片已保存到: {save_path}")
            else:
                self.status_var.set("已取消保存")
                
        except Exception as e:
            messagebox.showerror("错误", f"合并过程中出错:\n{str(e)}")
            self.status_var.set("发生错误")

if __name__ == "__main__":
    root = tk.Tk()
    app = ImageStitcherApp(root)
    root.mainloop()
