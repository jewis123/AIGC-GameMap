import os
import tkinter as tk
from tkinter import filedialog, messagebox
from PIL import Image

class AlphaRemoverApp:
    def __init__(self, root):
        self.root = root
        
        # 创建界面元素
        self.create_widgets()
        
    def create_widgets(self):
        # 单图片选择
        file_frame = tk.Frame(self.root)
        file_frame.pack(fill=tk.X, padx=10, pady=(10, 0))
        tk.Label(file_frame, text="图片文件:").pack(side=tk.LEFT)
        self.file_path = tk.StringVar()
        tk.Entry(file_frame, textvariable=self.file_path, width=40).pack(side=tk.LEFT, padx=5)
        tk.Button(file_frame, text="浏览...", command=self.browse_file).pack(side=tk.LEFT)

        # 目录选择
        dir_frame = tk.Frame(self.root)
        dir_frame.pack(fill=tk.X, padx=10, pady=5)
        tk.Label(dir_frame, text="图片目录:").pack(side=tk.LEFT)
        self.dir_path = tk.StringVar()
        tk.Entry(dir_frame, textvariable=self.dir_path, width=40).pack(side=tk.LEFT, padx=5)
        tk.Button(dir_frame, text="浏览...", command=self.browse_dir).pack(side=tk.LEFT)

        # 按钮区
        button_frame = tk.Frame(self.root)
        button_frame.pack(fill=tk.X, padx=10, pady=10)
        self.single_button = tk.Button(
            button_frame, 
            text="单次去除", 
            command=self.process_image,
            bg="#2196F3", fg="white", height=2, width=15
        )
        self.single_button.pack(side=tk.LEFT, padx=10)
        self.batch_button = tk.Button(
            button_frame, 
            text="批量去除", 
            command=self.process_images,
            bg="#4CAF50", fg="white", height=2, width=15
        )
        self.batch_button.pack(side=tk.LEFT, padx=10)

        # 状态标签
        status_frame = tk.Frame(self.root)
        status_frame.pack(fill=tk.X, side=tk.BOTTOM)
        self.status_var = tk.StringVar(value="准备就绪")
        tk.Label(status_frame, textvariable=self.status_var, bd=1, relief=tk.SUNKEN, anchor=tk.W).pack(side=tk.LEFT, fill=tk.X, expand=True)

    def browse_dir(self):
        dir_path = filedialog.askdirectory()
        if dir_path:
            self.dir_path.set(dir_path)

    def process_images(self):
        dir_path = self.dir_path.get()
        if not dir_path or not os.path.isdir(dir_path):
            messagebox.showerror("错误", "请选择有效的图片目录")
            return
        files = [f for f in os.listdir(dir_path) if f.lower().endswith((".png", ".tiff", ".tif", ".webp"))]
        if not files:
            messagebox.showinfo("提示", "目录下没有可处理的图片文件")
            return
        self.batch_button.config(state=tk.DISABLED)
        count = 0
        for f in files:
            file_path = os.path.join(dir_path, f)
            try:
                img = Image.open(file_path)
                if img.mode in ('RGBA', 'LA') or (img.mode == 'P' and 'transparency' in img.info):
                    background = Image.new('RGB', img.size, (0, 0, 0))
                    if img.mode == 'P':
                        img = img.convert('RGBA')
                    background.paste(img, mask=img.split()[3] if img.mode == 'RGBA' else img.split()[1])
                    filename, ext = os.path.splitext(file_path)
                    output_path = f"{filename}_no_alpha.png"
                    background.save(output_path)
                    count += 1
            except Exception as e:
                continue
        self.status_var.set(f"批量处理完成，移除透明通道图片数：{count}")
        messagebox.showinfo("完成", f"批量处理完成，移除透明通道图片数：{count}")
        self.batch_button.config(state=tk.NORMAL)

    def browse_file(self):
        file_path = filedialog.askopenfilename(
            filetypes=[
                ("图片文件", "*.png;*.tiff;*.tif;*.webp"),
                ("PNG 文件", "*.png"),
                ("TIFF 文件", "*.tiff;*.tif"),
                ("WebP 文件", "*.webp"),
                ("所有文件", "*.*")
            ]
        )
        if file_path:
            self.file_path.set(file_path)
    
    def process_image(self):
        file_path = self.file_path.get()
        if not file_path:
            messagebox.showerror("错误", "请选择图片文件")
            return
        try:
            self.single_button.config(state=tk.DISABLED)
            self.status_var.set("正在处理图片...")
            self.root.update()
            img = Image.open(file_path)
            if img.mode in ('RGBA', 'LA') or (img.mode == 'P' and 'transparency' in img.info):
                background = Image.new('RGB', img.size, (0, 0, 0))
                if img.mode == 'P':
                    img = img.convert('RGBA')
                background.paste(img, mask=img.split()[3] if img.mode == 'RGBA' else img.split()[1])
                filename, ext = os.path.splitext(file_path)
                output_path = f"{filename}_no_alpha.png"
                background.save(output_path)
                self.status_var.set(f"处理完成! 已保存到: {output_path}")
                messagebox.showinfo("成功", f"透明通道已移除，图片已保存为:\n{output_path}")
            else:
                self.status_var.set("该图片没有透明通道，无需处理")
                messagebox.showinfo("提示", "该图片没有透明通道，无需处理")
        except Exception as e:
            messagebox.showerror("错误", f"处理图片时出错:\n{str(e)}")
            self.status_var.set("处理过程中发生错误")
        finally:
            self.single_button.config(state=tk.NORMAL)

if __name__ == "__main__":
    root = tk.Tk()
    app = AlphaRemoverApp(root)
    root.mainloop()