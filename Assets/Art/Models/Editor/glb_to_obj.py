import os
import trimesh
import tkinter as tk
from tkinter import filedialog, messagebox

selected_files = []

def select_glb_files():
    global selected_files
    files = filedialog.askopenfilenames(
        title="Select .glb files",
        filetypes=[("GLB files", "*.glb")],
    )
    selected_files = list(files)
    files_var.set(f"{len(selected_files)} file(s) selected")

def select_output_folder():
    folder = filedialog.askdirectory(title="Select output folder")
    output_var.set(folder)

def convert_glb_to_obj():
    if not selected_files:
        messagebox.showerror("Error", "No .glb files selected.")
        return
    if not output_var.get():
        messagebox.showerror("Error", "Please select an output folder.")
        return

    os.makedirs(output_var.get(), exist_ok=True)
    converted = 0

    for path in selected_files:
        try:
            mesh = trimesh.load(path)
            filename = os.path.basename(path).replace(".glb", ".obj")
            output_path = os.path.join(output_var.get(), filename)
            mesh.export(output_path)
            converted += 1
        except Exception as e:
            print(f"Failed to convert {path}: {e}")

    messagebox.showinfo("Done", f"Successfully converted {converted} file(s) to .obj!")

# GUI
root = tk.Tk()
root.title("GLB to OBJ Converter (File Mode)")
root.geometry("420x220")

files_var = tk.StringVar()
output_var = tk.StringVar()

tk.Label(root, text="Selected Files:").pack(pady=5)
tk.Entry(root, textvariable=files_var, width=50, state="readonly").pack()
tk.Button(root, text="Select .glb Files", command=select_glb_files).pack()

tk.Label(root, text="Output Folder:").pack(pady=5)
tk.Entry(root, textvariable=output_var, width=50).pack()
tk.Button(root, text="Select Output Folder", command=select_output_folder).pack()

tk.Button(root, text="Convert to .obj", command=convert_glb_to_obj, bg="#4CAF50", fg="white").pack(pady=15)

root.mainloop()
