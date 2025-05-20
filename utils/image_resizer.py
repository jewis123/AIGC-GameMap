import os
import threading
from tkinter import Tk, Label, Entry, Button, filedialog, StringVar, IntVar, ttk, messagebox, Frame
from PIL import Image
from concurrent.futures import ThreadPoolExecutor, as_completed
import tkinter as tk

class ImageResizerApp:
    def __init__(self, master):
        self.master = master

        self.img_path = StringVar()
        self.save_dir = StringVar()
        self.target_pixel = IntVar(value=512)
        self.force_width = IntVar(value=0)   # 新增：强制宽
        self.force_height = IntVar(value=0)  # 新增：强制高
        self.progress = IntVar(value=0)
        self.total = IntVar(value=1)

        # 路径输入区
        path_frame = Frame(master)
        path_frame.pack(fill='x', pady=5)
        Label(path_frame, text='图片/文件夹路径:').pack(anchor='w')
        entry_path = Entry(path_frame, textvariable=self.img_path, width=50)
        entry_path.pack(side='left', padx=2, expand=True, fill='x')
        Button(path_frame, text='选择文件', command=self.select_file, width=8).pack(side='left', padx=2)
        Button(path_frame, text='选择文件夹', command=self.select_folder, width=10).pack(side='left', padx=2)

        # 保存目录区
        save_frame = Frame(master)
        save_frame.pack(fill='x', pady=5)
        Label(save_frame, text='保存目录:').pack(anchor='w')
        entry_save = Entry(save_frame, textvariable=self.save_dir, width=50)
        entry_save.pack(side='left', padx=2, expand=True, fill='x')
        Button(save_frame, text='选择保存目录', command=self.select_save_dir, width=12).pack(side='left', padx=2)

        # 目标像素区
        pixel_frame = Frame(master)
        pixel_frame.pack(fill='x', pady=5)
        Label(pixel_frame, text='目标像素(最长边):').pack(side='left', padx=2)
        Entry(pixel_frame, textvariable=self.target_pixel, width=10).pack(side='left', padx=2)

        # 新增：强制宽高设置
        force_size_frame = Frame(master)
        force_size_frame.pack(fill='x', pady=5)
        Label(force_size_frame, text='强制宽:').pack(side='left', padx=2)
        Entry(force_size_frame, textvariable=self.force_width, width=8).pack(side='left', padx=2)
        Label(force_size_frame, text='强制高:').pack(side='left', padx=2)
        Entry(force_size_frame, textvariable=self.force_height, width=8).pack(side='left', padx=2)
        Label(force_size_frame, text='(如需固定宽高缩放请填写)').pack(side='left', padx=2)

        # 按钮区
        btn_frame = Frame(master)
        btn_frame.pack(fill='x', padx=10, pady=10)
        Button(btn_frame, text='开始缩放', command=self.start_resize, bg="#4CAF50", fg="white", height=2, width=20).pack(side=tk.LEFT, padx=5)
        Button(btn_frame, text='批量缩放', command=self.start_batch_resize, bg="#FF9800", fg="white", height=2, width=20).pack(side=tk.LEFT, padx=5)

        # 进度条区
        progress_frame = Frame(master)
        progress_frame.pack(fill='x', pady=10)
        self.progressbar = ttk.Progressbar(progress_frame, length=400, variable=self.progress, maximum=100)
        self.progressbar.pack(pady=2, padx=10, fill='x')

        # 功能说明与使用说明
        desc = (
            "功能：批量缩放图片到指定最大边长。\n"
            "使用说明：\n"
            "1. 选择单张图片或图片文件夹。\n"
            "2. 选择保存目录，设置目标像素。保存路径选择地图编辑器中“assets/project_raw/preload目录“\n"
            "3. 点击“开始缩放”或“批量缩放”按钮。"
        )
        Label(master, text=desc, fg="#555", justify='left', anchor='w').pack(fill='x', padx=10, pady=(0,10))

    def select_file(self):
        path = filedialog.askopenfilename(filetypes=[('Image Files', '*.png;*.jpg;*.jpeg;*.bmp;*.gif')])
        if path:
            self.img_path.set(path)

    def select_folder(self):
        path = filedialog.askdirectory()
        if path:
            self.img_path.set(path)

    def select_save_dir(self):
        path = filedialog.askdirectory()
        if path:
            self.save_dir.set(path)

    def resize_image(self, img_path, save_dir, target_pixel):
        try:
            img = Image.open(img_path)
            w, h = img.size
            # 新增：强制宽高逻辑
            fw = self.force_width.get()
            fh = self.force_height.get()
            if fw > 0 and fh > 0:
                new_w, new_h = fw, fh
            else:
                if w >= h:
                    new_w = target_pixel
                    new_h = int(h * target_pixel / w)
                else:
                    new_h = target_pixel
                    new_w = int(w * target_pixel / h)
            img = img.resize((new_w, new_h), Image.LANCZOS)
            base = os.path.basename(img_path)
            save_path = os.path.join(save_dir, base)
            img.save(save_path)
            return True
        except Exception as e:
            print(f'处理失败: {img_path}, 错误: {e}')
            return False

    def start_resize(self):
        img_path = self.img_path.get()
        save_dir = self.save_dir.get()
        target_pixel = self.target_pixel.get()
        if not os.path.isfile(img_path):
            messagebox.showerror('错误', '请选择有效的图片文件路径')
            return
        if not os.path.isdir(save_dir):
            messagebox.showerror('错误', '请选择有效的保存目录')
            return
        self.progress.set(0)
        self.total.set(1)
        def task():
            ok = self.resize_image(img_path, save_dir, target_pixel)
            self.progress.set(100 if ok else 0)
            messagebox.showinfo('完成', '图片缩放完成' if ok else '图片缩放失败')
        threading.Thread(target=task).start()

    def start_batch_resize(self):
        folder = self.img_path.get()
        save_dir = self.save_dir.get()
        target_pixel = self.target_pixel.get()
        if not os.path.isdir(folder):
            messagebox.showerror('错误', '请选择有效的图片文件夹路径')
            return
        if not os.path.isdir(save_dir):
            messagebox.showerror('错误', '请选择有效的保存目录')
            return
        img_files = [os.path.join(folder, f) for f in os.listdir(folder)
                     if f.lower().endswith(('.png', '.jpg', '.jpeg', '.bmp', '.gif'))]
        total = len(img_files)
        if total == 0:
            messagebox.showerror('错误', '文件夹下没有图片文件')
            return
        self.progress.set(0)
        self.total.set(total)
        def batch_task():
            with ThreadPoolExecutor(max_workers=8) as executor:
                futures = [executor.submit(self.resize_image, f, save_dir, target_pixel) for f in img_files]
                done = 0
                for future in as_completed(futures):
                    done += 1
                    self.progress.set(int(done / total * 100))
            messagebox.showinfo('完成', '批量缩放完成')
        threading.Thread(target=batch_task).start()

if __name__ == '__main__':
    root = Tk()
    app = ImageResizerApp(root)
    root.mainloop()
