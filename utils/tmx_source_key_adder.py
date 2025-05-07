import os
import threading
from tkinter import Tk, Label, Entry, Button, filedialog, StringVar, IntVar, ttk, messagebox, Frame
import xml.etree.ElementTree as ET
import tkinter as tk

class TmxSourceKeyAdderApp:
    def __init__(self, master):
        self.master = master
        self.tmx_path = StringVar()
        self.progress = IntVar(value=0)
        self.total = IntVar(value=1)

        # 路径输入区
        path_frame = Frame(master)
        path_frame.pack(fill='x', pady=5)
        Label(path_frame, text='TMX文件/文件夹路径:').pack(anchor='w')
        entry_path = Entry(path_frame, textvariable=self.tmx_path, width=50)
        entry_path.pack(side='left', padx=2, expand=True, fill='x')
        Button(path_frame, text='选择文件', command=self.select_file, width=8).pack(side='left', padx=2)
        Button(path_frame, text='选择文件夹', command=self.select_folder, width=10).pack(side='left', padx=2)

        # 按钮区
        btn_frame = Frame(master)
        btn_frame.pack(fill='x', padx=10, pady=10)
        Button(btn_frame, text='处理单文件', command=self.start_single, bg="#4CAF50", fg="white", height=2, width=20).pack(side=tk.LEFT, padx=5)
        Button(btn_frame, text='批量处理', command=self.start_batch, bg="#FF9800", fg="white", height=2, width=20).pack(side=tk.LEFT, padx=5)

        # 进度条区
        progress_frame = Frame(master)
        progress_frame.pack(fill='x', pady=10)
        self.progressbar = ttk.Progressbar(progress_frame, length=400, variable=self.progress, maximum=100)
        self.progressbar.pack(pady=2, padx=10, fill='x')

        # 功能说明与使用说明
        desc = (
            "功能：为TMX文件的<tile>标签自动添加source_key属性（值等于source）。\n"
            "使用说明：\n"
            "1. 选择单个TMX文件或包含TMX文件的文件夹。\n"
            "2. 点击“处理单文件”或“批量处理”按钮。\n"
            "3. 处理完成后会自动保存修改。"
        )
        Label(master, text=desc, fg="#555", justify='left', anchor='w').pack(fill='x', padx=10, pady=(0,10))

    def select_file(self):
        path = filedialog.askopenfilename(filetypes=[('TMX Files', '*.tmx')])
        if path:
            self.tmx_path.set(path)

    def select_folder(self):
        path = filedialog.askdirectory()
        if path:
            self.tmx_path.set(path)

    def process_tmx(self, tmx_path):
        try:
            tree = ET.parse(tmx_path)
            root = tree.getroot()
            changed = False
            for tileset in root.findall('tileset'):
                for tile in tileset.findall('tile'):
                    image = tile.find('image')
                    if image is not None and 'source' in image.attrib:
                        src = image.attrib['source']
                        if 'source_key' not in image.attrib:
                            image.set('source_key', src)
                            changed = True
                        elif image.attrib['source_key'] != src:
                            image.set('source_key', src)
                            changed = True
            if changed:
                tree.write(tmx_path, encoding='utf-8', xml_declaration=True)
            return True
        except Exception as e:
            print(f'处理失败: {tmx_path}, 错误: {e}')
            return False

    def start_single(self):
        tmx_path = self.tmx_path.get()
        if not os.path.isfile(tmx_path) or not tmx_path.lower().endswith('.tmx'):
            messagebox.showerror('错误', '请选择有效的TMX文件路径')
            return
        self.progress.set(0)
        self.total.set(1)
        def task():
            ok = self.process_tmx(tmx_path)
            self.progress.set(100 if ok else 0)
            messagebox.showinfo('完成', '处理完成' if ok else '处理失败')
        threading.Thread(target=task).start()

    def start_batch(self):
        folder = self.tmx_path.get()
        if not os.path.isdir(folder):
            messagebox.showerror('错误', '请选择有效的TMX文件夹路径')
            return
        tmx_files = []
        for root, _, files in os.walk(folder):
            for f in files:
                if f.lower().endswith('.tmx'):
                    tmx_files.append(os.path.join(root, f))
        total = len(tmx_files)
        if total == 0:
            messagebox.showerror('错误', '文件夹下没有TMX文件')
            return
        self.progress.set(0)
        self.total.set(total)
        def batch_task():
            done = 0
            for f in tmx_files:
                self.process_tmx(f)
                done += 1
                self.progress.set(int(done / total * 100))
            messagebox.showinfo('完成', '批量处理完成')
        threading.Thread(target=batch_task).start()

if __name__ == '__main__':
    root = Tk()
    app = TmxSourceKeyAdderApp(root)
    root.mainloop()
