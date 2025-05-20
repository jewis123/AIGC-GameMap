import json
import math
import os,shutil
import tkinter as tk
from tkinter import filedialog, messagebox, ttk
from PIL import Image
import re

CACHE_FILE = os.path.join(os.path.dirname(__file__), 'image_stitcher_cache.json')

def load_cache():
    if os.path.exists(CACHE_FILE):
        try:
            with open(CACHE_FILE, 'r', encoding='utf-8') as f:
                return json.load(f)
        except Exception:
            return {}
    return {}

def save_cache(data):
    try:
        with open(CACHE_FILE, 'w', encoding='utf-8') as f:
            json.dump(data, f, ensure_ascii=False, indent=2)
    except Exception:
        pass

class ImageStitcherApp:
    def __init__(self, root):
        self.root = root
        self.cache = load_cache()
        self.create_widgets()
        # 恢复缓存
        self.dir_path.set(self.cache.get('dir_path', ''))
        self.save_dir.set(self.cache.get('save_dir', ''))
        self.preload_dir.set(self.cache.get('preload_dir', ''))
        self.tmx_dir.set(self.cache.get('tmx_dir', ''))
        self.whole_img_path.set(self.cache.get('whole_img_path', ''))
        self.split_save_dir.set(self.cache.get('split_save_dir', ''))

    def update_cache(self, key, value):
        self.cache[key] = value
        save_cache(self.cache)

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

        # 新增preload目录选择
        preload_dir_frame = tk.Frame(self.stitch_tab)
        preload_dir_frame.pack(fill=tk.X, padx=10, pady=5)
        tk.Label(preload_dir_frame, text="预载图片目录:").pack(side=tk.LEFT)
        self.preload_dir = tk.StringVar()
        tk.Entry(preload_dir_frame, textvariable=self.preload_dir, width=32).pack(side=tk.LEFT, padx=5)
        tk.Button(preload_dir_frame, text="选择...", command=self.browse_preload_directory).pack(side=tk.LEFT)

        # 新增Tiled图集目录选择
        tmx_dir_frame = tk.Frame(self.stitch_tab)
        tmx_dir_frame.pack(fill=tk.X, padx=10, pady=5)
        tk.Label(tmx_dir_frame, text="Tiled图集目录:").pack(side=tk.LEFT)
        self.tmx_dir = tk.StringVar()
        tk.Entry(tmx_dir_frame, textvariable=self.tmx_dir, width=32).pack(side=tk.LEFT, padx=5)
        tk.Button(tmx_dir_frame, text="选择...", command=self.browse_tmx_directory).pack(side=tk.LEFT)

        button_frame = tk.Frame(self.stitch_tab)
        button_frame.pack(fill=tk.X, padx=10, pady=10)
        tk.Button(button_frame, text="合并图片", command=self.stitch_images, 
                  bg="#4CAF50", fg="white", height=2, width=20).pack(side=tk.LEFT, padx=5)
        tk.Button(button_frame, text="批量合图", command=self.start_batch_stitch_thread, 
                  bg="#FF9800", fg="white", height=2, width=20).pack(side=tk.LEFT, padx=5)

        # 批量合图进度条
        batch_progress_frame = tk.Frame(self.stitch_tab)
        batch_progress_frame.pack(fill=tk.X, padx=10, pady=5)
        self.batch_progress_var = tk.DoubleVar()
        self.batch_count_var = tk.StringVar(value="")
        self.batch_progress_bar = ttk.Progressbar(batch_progress_frame, variable=self.batch_progress_var, maximum=100)
        self.batch_progress_bar.pack(side=tk.LEFT, fill=tk.X, expand=True, padx=5)
        tk.Label(batch_progress_frame, textvariable=self.batch_count_var, width=12, anchor=tk.E).pack(side=tk.LEFT)

        # 功能说明与使用说明
        desc = (
            "功能：批量合并子目录下的图片为整图，或将整图切分为小图。\n"
            "使用说明：\n"
            "1. 选择图片目录、保存目录等参数。\n"
            "2. 点击“合并图片”/“批量合图”或“切分整图”/“批量切图”。\n"
            "3. 操作完成后会弹窗提示结果。"
        )
        tk.Label(self.stitch_tab, text=desc, fg="#555", justify='left', anchor='w').pack(fill='x', padx=10, pady=(0,10))

        # 切图界面
        split_frame = tk.Frame(self.split_tab)
        split_frame.pack(fill=tk.X, padx=10, pady=10)
        tk.Label(split_frame, text="整图路径:").pack(side=tk.LEFT)
        self.whole_img_path = tk.StringVar()
        tk.Entry(split_frame, textvariable=self.whole_img_path, width=32).pack(side=tk.LEFT, padx=5)
        tk.Button(split_frame, text="选择整图...", command=self.browse_whole_image).pack(side=tk.LEFT)

        # 新增：切图输出目录选择
        split_save_dir_frame = tk.Frame(self.split_tab)
        split_save_dir_frame.pack(fill=tk.X, padx=10, pady=5)
        tk.Label(split_save_dir_frame, text="输出目录:").pack(side=tk.LEFT)
        self.split_save_dir = tk.StringVar()
        tk.Entry(split_save_dir_frame, textvariable=self.split_save_dir, width=36).pack(side=tk.LEFT, padx=5)
        tk.Button(split_save_dir_frame, text="选择...", command=self.browse_split_save_directory).pack(side=tk.LEFT)

        # 新增：图片类型选择
        split_image_type_frame = tk.Frame(self.split_tab)
        split_image_type_frame.pack(fill=tk.X, padx=10, pady=5)
        tk.Label(split_image_type_frame, text="图片类型:").pack(side=tk.LEFT)
        self.image_type = tk.StringVar(value="png")
        tk.Radiobutton(split_image_type_frame, text="PNG", variable=self.image_type, value="png").pack(side=tk.LEFT, padx=5)
        tk.Radiobutton(split_image_type_frame, text="JPG", variable=self.image_type, value="jpg").pack(side=tk.LEFT, padx=5)

        split_button_frame = tk.Frame(self.split_tab)
        split_button_frame.pack(fill=tk.X, padx=10, pady=20)
        tk.Button(split_button_frame, text="切分整图", command=self.split_image, 
                  bg="#2196F3", fg="white", height=2, width=20).pack(side=tk.LEFT, padx=5)
        tk.Button(split_button_frame, text="批量切图", command=self.start_batch_split_thread, 
                  bg="#FF9800", fg="white", height=2, width=20).pack(side=tk.LEFT, padx=5)

        # 状态栏
        self.status_var = tk.StringVar(value="准备就绪")
        tk.Label(self.root, textvariable=self.status_var, bd=1, relief=tk.SUNKEN, anchor=tk.W).pack(side=tk.BOTTOM, fill=tk.X)

    def browse_directory(self):
        directory = filedialog.askdirectory()
        if directory:
            self.dir_path.set(directory)
            self.update_cache('dir_path', directory)

    def browse_save_directory(self):
        directory = filedialog.askdirectory()
        if directory:
            self.save_dir.set(directory)
            self.update_cache('save_dir', directory)

    def browse_preload_directory(self):
        directory = filedialog.askdirectory()
        if directory:
            self.preload_dir.set(directory)
            self.update_cache('preload_dir', directory)

    def browse_whole_image(self):
        file_path = filedialog.askopenfilename(filetypes=[("Image files", "*.png;*.jpg;*.jpeg;*.bmp")])
        if file_path:
            self.whole_img_path.set(file_path)
            self.update_cache('whole_img_path', file_path)

    def browse_split_save_directory(self):
        directory = filedialog.askdirectory()
        if directory:
            self.split_save_dir.set(directory)
            self.update_cache('split_save_dir', directory)

    def browse_tmx_directory(self):
        directory = filedialog.askdirectory()
        if directory:
            self.tmx_dir.set(directory)
            self.update_cache('tmx_dir', directory)

    def natural_key(self, s):
        return [int(text) if text.isdigit() else text.lower() for text in re.split('(\d+)', s)]

    def get_auto_grid_with_preload(self, n, preload_img_path=None):
        # 先生成所有可能的(宽,高)组合
        sqrt_number = math.sqrt(n)
        if(sqrt_number.is_integer()):
            return (int(sqrt_number), int(sqrt_number))
        pairs = []
        for i in range(1, int(n ** 0.5) + 1):
            if n % i == 0:
                w, h = i, n // i
                pairs.append((w, h))
                if w != h:
                    pairs.append((h, w))
        if preload_img_path and os.path.exists(preload_img_path):
            try:
                with Image.open(preload_img_path) as img:
                    pw, ph = img.size
                    if pw == ph:
                        # 1:1，优先宽高相等
                        square_pairs = [(w, h) for w, h in pairs if w == h]
                        if square_pairs:
                            return square_pairs[0]
                    elif pw > ph:
                        # 宽>高，优先宽>高且差值最小
                        wide_pairs = [(w, h) for w, h in pairs if w > h]
                        if wide_pairs:
                            return min(wide_pairs, key=lambda x: (abs(x[0]-x[1]), x[0]))
                    else:
                        # 高>宽，优先高>宽且差值最小
                        high_pairs = [(w, h) for w, h in pairs if h > w]
                        if high_pairs:
                            return min(high_pairs, key=lambda x: (abs(x[1]-x[0]), x[1]))
            except Exception:
                pass  # 预载图片异常时降级为普通逻辑
        # 没有预载或异常，默认逻辑
        square_pairs = [(w, h) for w, h in pairs if w == h]
        if square_pairs:
            return square_pairs[0]
        high_pairs = [(w, h) for w, h in pairs if h > w]
        if high_pairs:
            return min(high_pairs, key=lambda x: (abs(x[1]-x[0]), x[1]))
        return pairs[0] if pairs else (1, n)

    def get_preload_img_dir(self, preload_dir, folder_name):
         # preload逻辑
        preload_dir = self.preload_dir.get().strip()
        preload_img_path = None
        if preload_dir:
            for ext in [".png", ".jpg", ".jpeg", ".bmp"]:
                candidate = os.path.join(preload_dir, folder_name + ext)
                if os.path.exists(candidate):
                    preload_img_path = candidate
                    break
        return preload_img_path

    def stitch_images_in_dir(self, dir_path, save_dir):
        folder_name = os.path.basename(os.path.normpath(dir_path))
        # 只保留前缀和目录名一致的图片，并且命名中包含tmx子图id的图片
        tmx_tile_ids = self.get_tmx_tile_ids(folder_name)
        def is_valid_image(f):
            name, ext = os.path.splitext(f)
            if(name.split('_')[0] != folder_name.split('_')[0] or ext.lower() not in (".png", ".jpg", ".jpeg", ".bmp")):
                return False
            match = re.search(r'_(\d+)', name)
            if not match:
                return False
            number = int(match.group(1))
            return tmx_tile_ids and number in tmx_tile_ids
        image_files = [f for f in os.listdir(dir_path) if is_valid_image(f)]
        n = len(image_files)
        if n == 0:
            return False, f"目录 {dir_path} 中没有符合tmx子图id的图片文件"
        # 预载图片逻辑
        width_count, height_count = self.get_auto_grid_with_preload(n, self.get_preload_img_dir(self.preload_dir.get(), folder_name))
        if width_count * height_count != n:
            return False, f"目录 {dir_path} 中的图片数量({n}个)无法组成规则矩阵"
        image_map = {}
        sorted_files = sorted(image_files, key=self.natural_key)
        for i in range(height_count):
            for j in range(width_count):
                idx = i * width_count + j
                if idx < len(sorted_files):
                    image_map[(i, j)] = os.path.join(dir_path, sorted_files[idx])
        with Image.open(os.path.join(dir_path, sorted_files[0])) as sample_img:
            img_width, img_height = sample_img.size
        result_img = Image.new('RGB', (img_width * width_count, img_height * height_count))
        for (row, col), img_path in image_map.items():
            with Image.open(img_path) as img:
                result_img.paste(img, (col * img_width, row * img_height))
        default_filename = f"{folder_name}_{width_count}x{height_count}.png"
        os.makedirs(save_dir, exist_ok=True)
        save_path = os.path.join(save_dir, default_filename)
        result_img.save(save_path)
        return True, save_path

    def batch_stitch_images_with_progress(self):
        dir_path = self.dir_path.get()
        save_dir = self.save_dir.get().strip()
        if not dir_path or not save_dir:
            self.root.after(0, lambda: messagebox.showerror("错误", "请选择图片目录和保存目录"))
            self.root.after(0, lambda: self.status_var.set("准备就绪"))
            return
        try:
            subdirs = [os.path.join(dir_path, d) for d in os.listdir(dir_path) if os.path.isdir(os.path.join(dir_path, d))]
            total = len(subdirs)
            if total == 0:
                self.root.after(0, lambda: messagebox.showerror("错误", "图片目录下没有子目录"))
                self.root.after(0, lambda: self.status_var.set("准备就绪"))
                self.root.after(0, lambda: self.enable_stitch_buttons())
                return
            success_count = 0
            fail_msgs = []
            for idx, subdir in enumerate(subdirs, 1):
                ok, msg = self.stitch_images_in_dir(subdir, save_dir)
                if ok:
                    success_count += 1
                else:
                    fail_msgs.append(msg)
                # 更新进度条
                self.root.after(0, lambda i=idx, t=total: self.batch_progress_var.set(i/t*100))
                self.root.after(0, lambda i=idx, t=total: self.batch_count_var.set(f"{i}/{t}"))
            msg = f"批量合图完成，成功：{success_count} 个子目录。"
            if fail_msgs:
                msg += "\n失败信息：\n" + "\n".join(fail_msgs)
            self.root.after(0, lambda: messagebox.showinfo("批量合图结果", msg))
            self.root.after(0, lambda: self.status_var.set("批量合图完成"))
            if getattr(self, 'auto_close', False):
                self.root.after(500, lambda: sys.exit(0))
        except Exception as e:
            self.root.after(0, lambda: messagebox.showerror("错误", f"批量合图过程中出错:\n{str(e)}"))
            self.root.after(0, lambda: self.status_var.set("发生错误"))
        finally:
            self.root.after(0, self.enable_stitch_buttons)

    def enable_stitch_buttons(self):
        # 恢复合图相关按钮
        for child in self.stitch_tab.winfo_children():
            if isinstance(child, tk.Frame):
                for btn in child.winfo_children():
                    if isinstance(btn, tk.Button):
                        btn.config(state=tk.NORMAL)

    def start_batch_stitch_thread(self):
        # 启动批量合图线程，避免界面卡死
        import threading
        self.batch_progress_var.set(0)
        self.batch_count_var.set("")
        self.status_var.set("正在批量处理图片...")
        self.root.update()
        # 禁用按钮
        for child in self.stitch_tab.winfo_children():
            if isinstance(child, tk.Frame):
                for btn in child.winfo_children():
                    if isinstance(btn, tk.Button):
                        btn.config(state=tk.DISABLED)
        thread = threading.Thread(target=self.batch_stitch_images_with_progress)
        thread.daemon = True
        thread.start()

    def stitch_images(self):
        dir_path = self.dir_path.get()
        save_dir = self.save_dir.get().strip()
        if not dir_path:
            messagebox.showerror("错误", "请选择图片目录")
            return
        if not save_dir:
            messagebox.showerror("错误", "请选择整图保存目录")
            return
        # 新增：保存目录不为空时弹窗提示并打断执行
        if os.path.exists(save_dir) and os.listdir(save_dir):
            messagebox.showwarning("提示", "保存目录不为空，请选择一个空目录！")
            self.status_var.set("保存目录不为空，操作已中断")
            return
        try:
            self.status_var.set("正在处理图片...")
            self.root.update()
            folder_name = os.path.basename(os.path.normpath(dir_path))
            tmx_tile_ids = self.get_tmx_tile_ids(folder_name)
            def is_valid_image(f):
                name, ext = os.path.splitext(f)
                if(name.split('_')[0] != folder_name.split('_')[0] and ext.lower() not in (".png", ".jpg", ".jpeg", ".bmp")):
                    return False
                
                match = re.search(r'_(\d+)$', name)
                if not match:
                    return False
                number = int(match.group(1))
                return tmx_tile_ids and number in tmx_tile_ids
            image_files = [f for f in os.listdir(dir_path) if is_valid_image(f)]
            n = len(image_files)
            if n == 0:
                messagebox.showerror("错误", "目录中没有符合tmx子图id的图片文件")
                self.status_var.set("准备就绪")
                return
            width_count, height_count = self.get_auto_grid_with_preload(n, self.get_preload_img_dir(self.preload_dir.get(), folder_name))
            if width_count * height_count != n:
                messagebox.showerror("错误", f"目录中的图片数量({n}个)无法组成规则矩阵")
                self.status_var.set("准备就绪")
                return
            image_map = {}
            sorted_files = sorted(image_files, key=self.natural_key)
            for i in range(height_count):
                for j in range(width_count):
                    idx = i * width_count + j
                    if idx < len(sorted_files):
                        image_map[(i, j)] = os.path.join(dir_path, sorted_files[idx])
            with Image.open(os.path.join(dir_path, sorted_files[0])) as sample_img:
                img_width, img_height = sample_img.size
            result_img = Image.new('RGB', (img_width * width_count, img_height * height_count))
            for (row, col), img_path in image_map.items():
                with Image.open(img_path) as img:
                    result_img.paste(img, (col * img_width, row * img_height))
            default_filename = f"{folder_name}_{width_count}x{height_count}.png"
            os.makedirs(save_dir, exist_ok=True)
            save_path = os.path.join(save_dir, default_filename)
            if save_path:
                result_img.save(save_path)
                messagebox.showinfo("成功", f"图片已成功合并并保存到:\n{save_path}")
                self.status_var.set(f"图片已保存到: {save_path}")
                if getattr(self, 'auto_close', False):
                    self.root.after(300, lambda: sys.exit(0))
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
        # 新增：获取用户指定输出目录
        save_dir = self.split_save_dir.get().strip()
        if not save_dir:
            # 兼容老逻辑，未指定则用默认
            save_dir = os.path.join(os.getcwd(), "utils/image_resizer", f"split_{base_name}")
        else:
            save_dir = os.path.join(save_dir)

        # 获取图片类型
        image_type = self.image_type.get()

        #清空目录save_dir
        self.clear_folder(save_dir)

        try:
            self.status_var.set("正在切分整图...")
            self.root.update()
            with Image.open(img_path) as img:
                # 如果图片不是RGB模式，转换为RGB模式
                if img.mode != 'RGB':
                    img = img.convert('RGB')
                img_width, img_height = img.size
                tile_width = img_width // width_count
                tile_height = img_height // height_count
                os.makedirs(save_dir, exist_ok=True)
                for i in range(height_count):
                    for j in range(width_count):
                        left = j * tile_width
                        upper = i * tile_height
                        right = left + tile_width
                        lower = upper + tile_height
                        tile = img.crop((left, upper, right, lower))
                        tile_name = f"{base_name[:-3]}{i*width_count+j+1}.{image_type}"
                        tile.save(os.path.join(save_dir, tile_name), quality=95 if image_type == "jpg" else None)
            messagebox.showinfo("成功", f"整图已切分为{width_count*height_count}张图片，保存于:\n{save_dir}")
            self.status_var.set(f"切分完成，保存于: {save_dir}")
            
        except Exception as e:
            messagebox.showerror("错误", f"切分过程中出错:\n{str(e)}")
            self.status_var.set("发生错误")
        finally:
            if getattr(self, 'auto_close', False):
                self.root.after(30, lambda: sys.exit(0))

    def batch_split_images(self):
        src_dir = self.dir_path.get().strip()
        save_dir = self.split_save_dir.get().strip()
        if not src_dir or not save_dir:
            self.root.after(0, lambda: messagebox.showerror("错误", "请选择图片目录和输出目录"))
            self.root.after(0, lambda: self.status_var.set("准备就绪"))
            return
        try:
            files = [f for f in os.listdir(src_dir) if f.lower().endswith((".png", ".jpg", ".jpeg", ".bmp"))]
            total = len(files)
            if total == 0:
                self.root.after(0, lambda: messagebox.showerror("错误", "目录下没有图片文件"))
                self.root.after(0, lambda: self.status_var.set("准备就绪"))
                return
            success_count = 0
            fail_msgs = []
            for idx, file in enumerate(files, 1):
                img_path = os.path.join(src_dir, file)
                base_name = os.path.splitext(os.path.basename(img_path))[0]
                match = re.search(r'_(\d+)x(\d+)$', base_name)
                if not match:
                    fail_msgs.append(f"{file}: 文件名需以 _宽x高 结尾")
                    continue
                width_count = int(match.group(1))
                height_count = int(match.group(2))
                folder_name = re.sub(r'_(\d+)x(\d+)$', '', base_name)
                out_dir = os.path.join(save_dir, folder_name)
                self.clear_folder(out_dir)
                try:
                    with Image.open(img_path) as img:
                        img_width, img_height = img.size
                        tile_width = img_width // width_count
                        tile_height = img_height // height_count
                        for i in range(height_count):
                            for j in range(width_count):
                                left = j * tile_width
                                upper = i * tile_height
                                right = left + tile_width
                                lower = upper + tile_height
                                tile = img.crop((left, upper, right, lower))
                                tile_name = f"{folder_name}{i*width_count+j+1:02d}.{self.image_type.get()}"
                                tile.save(os.path.join(out_dir, tile_name), quality=95 if self.image_type.get() == "jpg" else None)
                    success_count += 1
                except Exception as e:
                    fail_msgs.append(f"{file}: {str(e)}")
                self.root.after(0, lambda i=idx, t=total: self.status_var.set(f"批量切图进度: {i}/{t}"))
            msg = f"批量切图完成，成功：{success_count} 个文件。"
            if fail_msgs:
                msg += "\n失败信息：\n" + "\n".join(fail_msgs)
            self.root.after(0, lambda: messagebox.showinfo("批量切图结果", msg))
            self.root.after(0, lambda: self.status_var.set("批量切图完成"))
            if getattr(self, 'auto_close', False):
                self.root.after(500, lambda: sys.exit(0))
        except Exception as e:
            self.root.after(0, lambda: messagebox.showerror("错误", f"批量切图过程中出错:\n{str(e)}"))
            self.root.after(0, lambda: self.status_var.set("发生错误"))

    def start_batch_split_thread(self):
        import threading
        self.status_var.set("正在批量切分图片...")
        self.root.update()
        thread = threading.Thread(target=self.batch_split_images)
        thread.daemon = True
        thread.start()

    def clear_folder(self, folder_path):
        if os.path.exists(folder_path):
            shutil.rmtree(folder_path)  # 删除整个文件夹及其内容
        os.makedirs(folder_path, exist_ok=True)  # 重新创建空文件夹

    def get_tmx_tile_ids(self, folder_name):
        """
        在Tiled图集目录下查找同名tmx文件，解析tileset下所有tile标签，返回id列表（字符串型）
        """
        import xml.etree.ElementTree as ET
        tmx_dir = self.tmx_dir.get().strip()
        if not tmx_dir:
            return None
        tmx_path = os.path.join(tmx_dir, folder_name + '.tmx')
        if not os.path.exists(tmx_path):
            return None
        try:
            tree = ET.parse(tmx_path)
            root = tree.getroot()
            tile_ids = set()
            for tileset in root.findall('tileset'):
                for tile in tileset.findall('tile'):
                    tid = tile.get('id')
                    if tid is not None:
                        tile_ids.add(int(tid) +1 )
            return tile_ids
        except Exception:
            return None

if __name__ == "__main__":
    import sys
    auto_close = False
    if len(sys.argv) > 1 and sys.argv[1] == "1":
        auto_close = True
    root = tk.Tk()
    app = ImageStitcherApp(root)
    app.auto_close = auto_close
    root.mainloop()
