import os
import tkinter as tk
from tkinter import filedialog, messagebox
from PIL import Image

class AlphaRemoverApp:
    def __init__(self, root):
        self.root = root
        self.root.title("透明通道移除工具")
        self.root.geometry("500x200")
        
        # 创建界面元素
        self.create_widgets()
        
    def create_widgets(self):
        # 文件选择
        file_frame = tk.Frame(self.root)
        file_frame.pack(fill=tk.X, padx=10, pady=10)
        
        tk.Label(file_frame, text="图片文件:").pack(side=tk.LEFT)
        self.file_path = tk.StringVar()
        tk.Entry(file_frame, textvariable=self.file_path, width=40).pack(side=tk.LEFT, padx=5)
        tk.Button(file_frame, text="浏览...", command=self.browse_file).pack(side=tk.LEFT)
        
        # 执行按钮
        button_frame = tk.Frame(self.root)
        button_frame.pack(fill=tk.X, padx=10, pady=20)
        
        self.process_button = tk.Button(
            button_frame, 
            text="移除透明通道", 
            command=self.process_image,
            bg="#4CAF50", 
            fg="white", 
            height=2, 
            width=20
        )
        self.process_button.pack()
        
        # 状态标签
        status_frame = tk.Frame(self.root)
        status_frame.pack(fill=tk.X, side=tk.BOTTOM)
        
        self.status_var = tk.StringVar(value="准备就绪")
        tk.Label(status_frame, textvariable=self.status_var, bd=1, relief=tk.SUNKEN, anchor=tk.W).pack(side=tk.LEFT, fill=tk.X, expand=True)
    
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
            # 禁用按钮避免重复操作
            self.process_button.config(state=tk.DISABLED)
            self.status_var.set("正在处理图片...")
            self.root.update()
            
            # 打开图片
            img = Image.open(file_path)
            
            # 检查是否有 Alpha 通道
            if img.mode in ('RGBA', 'LA') or (img.mode == 'P' and 'transparency' in img.info):
                # 准备一个黑色背景
                background = Image.new('RGB', img.size, (0, 0, 0))
                
                # 如果图片是调色板模式，先转换为 RGBA
                if img.mode == 'P':
                    img = img.convert('RGBA')
                
                # 将图片粘贴到黑色背景上，合并 Alpha 通道
                background.paste(img, mask=img.split()[3] if img.mode == 'RGBA' else img.split()[1])
                
                # 构建输出文件名
                filename, ext = os.path.splitext(file_path)
                output_path = f"{filename}_no_alpha.png"
                
                # 保存图片
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
            self.process_button.config(state=tk.NORMAL)

if __name__ == "__main__":
    root = tk.Tk()
    app = AlphaRemoverApp(root)
    root.mainloop()