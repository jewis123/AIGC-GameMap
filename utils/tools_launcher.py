import tkinter as tk
from tkinter import ttk
import sys

from batch_comfyui_runner import BatchComfyUIApp
from tmx_source_key_adder import TmxSourceKeyAdderApp
sys.path.append('./utils')

from alpha_remover import AlphaRemoverApp
from image_rename_corrector import RenameCorrectorApp
from image_resizer import ImageResizerApp
from image_stitcher import ImageStitcherApp

class ToolLauncherApp:
    def __init__(self, root):
        self.root = root
        self.root.title("换皮批处理工具箱")
        self.root.geometry("600x450")
        
        notebook = ttk.Notebook(self.root)
        notebook.pack(fill=tk.BOTH, expand=True)

        # 图片命名纠错
        frame_rename = tk.Frame(notebook)
        RenameCorrectorApp(frame_rename)
        notebook.add(frame_rename, text="图片命名纠错")

        # tmx图集纠错
        frame_rename = tk.Frame(notebook)
        TmxSourceKeyAdderApp(frame_rename)
        notebook.add(frame_rename, text="tmx图集纠错")

        # 图片缩放
        frame_resize = tk.Frame(notebook)
        ImageResizerApp(frame_resize)
        notebook.add(frame_resize, text="图片缩放")

        # 图片拼接/切分
        frame_stitch = tk.Frame(notebook)
        ImageStitcherApp(frame_stitch)
        notebook.add(frame_stitch, text="拼接/切分图片")
        
        # 图片消A通道
        frame_stitch = tk.Frame(notebook)
        AlphaRemoverApp(frame_stitch)
        notebook.add(frame_stitch, text="消A通道")
        
        # 图片转绘
        frame_stitch = tk.Frame(notebook)
        BatchComfyUIApp(frame_stitch)
        notebook.add(frame_stitch, text="图片转绘")

if __name__ == "__main__":
    root = tk.Tk()
    app = ToolLauncherApp(root)
    root.mainloop()
