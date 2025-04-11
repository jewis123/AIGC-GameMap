import os
import tkinter as tk
from tkinter import filedialog, messagebox, ttk
from PIL import Image
import threading

class ImageResizerApp:
    def __init__(self, root):
        self.root = root
        self.root.title("图片缩放工具 - 512×512")
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
        
        # 进度条
        progress_frame = tk.Frame(self.root)
        progress_frame.pack(fill=tk.X, padx=10, pady=10)
        
        self.progress_var = tk.DoubleVar()
        self.progress_bar = ttk.Progressbar(progress_frame, variable=self.progress_var, maximum=100)
        self.progress_bar.pack(fill=tk.X, padx=5, pady=5)
        
        # 执行按钮
        button_frame = tk.Frame(self.root)
        button_frame.pack(fill=tk.X, padx=10, pady=20)
        
        self.resize_button = tk.Button(button_frame, text="开始缩放", command=self.start_resize,
                  bg="#4CAF50", fg="white", height=2, width=20)
        self.resize_button.pack()
        
        # 状态标签
        status_frame = tk.Frame(self.root)
        status_frame.pack(fill=tk.X, side=tk.BOTTOM)
        
        self.status_var = tk.StringVar(value="准备就绪")
        self.count_var = tk.StringVar(value="")
        
        tk.Label(status_frame, textvariable=self.count_var).pack(side=tk.RIGHT, padx=5)
        tk.Label(status_frame, textvariable=self.status_var, bd=1, relief=tk.SUNKEN, anchor=tk.W).pack(side=tk.LEFT, fill=tk.X, expand=True)
    
    def browse_directory(self):
        directory = filedialog.askdirectory()
        if directory:
            self.dir_path.set(directory)
    
    def start_resize(self):
        dir_path = self.dir_path.get()
        
        if not dir_path:
            messagebox.showerror("错误", "请选择图片目录")
            return
        
        # 禁用按钮避免重复操作
        self.resize_button.config(state=tk.DISABLED)
        
        # 使用线程执行耗时操作，避免界面卡死
        thread = threading.Thread(target=self.resize_images, args=(dir_path,))
        thread.daemon = True
        thread.start()
    
    def resize_images(self, dir_path):
        try:
            # 获取所有图片文件
            image_files = [f for f in os.listdir(dir_path) 
                          if f.lower().endswith(('.png', '.jpg', '.jpeg', '.bmp', '.tiff', '.webp'))]
            
            if not image_files:
                messagebox.showinfo("提示", "所选目录中没有发现图片文件")
                self.root.after(0, lambda: self.resize_button.config(state=tk.NORMAL))
                self.root.after(0, lambda: self.status_var.set("准备就绪"))
                return
            
            total_images = len(image_files)
            processed = 0
            
            self.root.after(0, lambda: self.status_var.set("正在处理图片..."))
            self.root.after(0, lambda: self.count_var.set(f"0/{total_images}"))
            
            for img_file in image_files:
                # 检查文件名是否已经有_512后缀
                name, ext = os.path.splitext(img_file)
                if name.endswith("_512"):
                    processed += 1
                    self.root.after(0, lambda p=processed, t=total_images: 
                        self.count_var.set(f"{p}/{t}"))
                    self.root.after(0, lambda p=processed, t=total_images: 
                        self.progress_var.set(p/t*100))
                    continue
                
                # 构建输出文件名
                output_filename = f"{name}_512{ext}"
                input_path = os.path.join(dir_path, img_file)
                output_path = os.path.join(dir_path, output_filename)
                
                # 打开图片并调整大小
                with Image.open(input_path) as img:
                    # 计算等比缩放后的尺寸
                    width, height = img.size
                    if width >= height:
                        # 宽边是较长边
                        new_width = 512
                        new_height = int(height * (512 / width))
                    else:
                        # 高边是较长边
                        new_height = 512
                        new_width = int(width * (512 / height))
                    
                    # 使用高质量的 LANCZOS 重采样方法
                    resized_img = img.resize((new_width, new_height), Image.LANCZOS)
                    
                    # 保存图片，使用最高质量设置
                    if ext.lower() in ['.jpg', '.jpeg']:
                        resized_img.save(output_path, quality=95, optimize=True)
                    elif ext.lower() == '.png':
                        resized_img.save(output_path, optimize=True)
                    else:
                        resized_img.save(output_path)
                
                processed += 1
                self.root.after(0, lambda p=processed, t=total_images: 
                    self.count_var.set(f"{p}/{t}"))
                self.root.after(0, lambda p=processed, t=total_images: 
                    self.progress_var.set(p/t*100))
            
            self.root.after(0, lambda: self.status_var.set(f"处理完成! 已处理 {processed} 张图片"))
            self.root.after(0, lambda: messagebox.showinfo("完成", f"所有图片已处理完成!\n共处理了 {processed} 张图片"))
        
        except Exception as e:
            self.root.after(0, lambda: messagebox.showerror("错误", f"处理图片时出错:\n{str(e)}"))
            self.root.after(0, lambda: self.status_var.set("处理过程中发生错误"))
        
        finally:
            self.root.after(0, lambda: self.resize_button.config(state=tk.NORMAL))

if __name__ == "__main__":
    root = tk.Tk()
    app = ImageResizerApp(root)
    root.mainloop()
